using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner;
using Miner.CommandLine;
using Miner.Geodatabase;
using Miner.Interop;
using Miner.Interop.msxml2;

using Array = System.Array;

namespace Utils.IEXML
{
    internal class ProgramArguments
    {
        #region Fields

        [Argument(ArgumentType.MultipleUnique, ShortName = "f")]
        public string[] ConnectionFiles;
        [Argument(ArgumentType.Multiple, ShortName = "d")]
        public string[] Directories;
        [Argument(ArgumentType.LastOccurenceWins, ShortName = "t")]
        public ProgramTask Task = ProgramTask.Export;

        #endregion
    }

    internal enum ProgramTask
    {
        Import,
        Export,
        Compare
    }

    internal class Program
    {
        #region Public Methods

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var pa = new ProgramArguments();
                if (!Parser.ParseArgumentsWithUsage(args, pa))
                    return;

                new Program().Run(pa);
            }
            catch (Exception e)
            {
                Log.Error(typeof(Program), e);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Exports or imports the XML files.
        /// </summary>
        /// <param name="args">The arguments.</param>
        internal void Run(ProgramArguments args)
        {
            // Validate the arguments so that the number of connection files and directories match.
            this.ValidateArguments(args);

            // Create a dictionary of the connection files and directory arguments.
            var zip = args.ConnectionFiles.Zip(args.Directories, (k, v) => new { k, v }).ToDictionary(x => Path.GetFullPath(x.k), x => Path.GetFullPath(x.v));
            if (zip.Any())
            {
                using (RuntimeAuthorization lic = new RuntimeAuthorization(ProductCode.EngineOrDesktop))
                {
                    if (lic.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard, mmLicensedProductCode.mmLPArcFM))
                    {
                        switch (args.Task)
                        {
                            case ProgramTask.Export:
                                this.Export(zip);

                                break;

                            case ProgramTask.Import:
                                this.Import(zip);

                                break;

                            case ProgramTask.Compare:
                                this.Compare(zip);

                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Compares contents of the files and generates an output file of the differences.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void Compare(Dictionary<string, string> args)
        {
            var tables = new List<DataTable>();
            var directory = args.Values.First();

            Log.Info(this, "Comparing ArcFM XML Properties");
            Log.Info(this, "- Directory: \t{0}", directory);

            var files = args.Keys.Where(k => k.EndsWith(".CSV", StringComparison.InvariantCultureIgnoreCase));
            foreach (var file in files)
            {
                Log.Info(this, "- File: \t{0}", file);
                var table = this.ReadCsv(file);
                tables.Add(table);
            }

            for (int i = 0; i < tables.Count; i++)
            {
                var f = tables.First();
                var l = tables.Last();

                var fileName = Path.Combine(directory, string.Format("{0} vs {1}.csv", f.TableName, l.TableName));
                Log.Info(this, "{0}", Path.GetFileNameWithoutExtension(fileName));

                var count = 0;
                var rows = f.AsEnumerable().Except(l.AsEnumerable(), DataRowComparer.Default).ToArray();
                if (rows.Any())
                {
                    var table = rows.CopyToDataTable();
                    table.WriteCsv(fileName);

                    count = table.Rows.Count;
                }

                Log.Info(this, "\tDifferences: {0}", count);

                tables.Reverse();
            }
        }

        /// <summary>
        ///     Creates container for the CSV data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Returns a <see cref="DataTable" /> represeting the container for the data.</returns>
        private DataTable CreateCsv(string fileName)
        {
            var table = new DataTable(Path.GetFileNameWithoutExtension(fileName));

            table.Columns.Add("FEATUREDATASET");
            table.Columns.Add("OBJECTCLASSNAME");
            table.Columns.Add("SUBTYPECODE");
            table.Columns.Add("SUBTYPENAME");
            table.Columns.Add("EDITEVENT");
            table.Columns.Add("NAME");
            table.Columns.Add("PROGID");

            return table;
        }

        /// <summary>
        ///     Creates the root document for the XML.
        /// </summary>
        /// <param name="versionNumber">The version number.</param>
        /// <returns>Returns a <see cref="IXMLDOMDocument" /> representing the root document for the XML.</returns>
        private IXMLDOMDocument CreateDocument(string versionNumber)
        {
            IXMLDOMDocument doc = new DOMDocumentClass();
            var element = doc.createElement("GXXML");
            doc.appendChild(element);

            element = doc.createElement("EXPORT_VERSION");
            element.text = versionNumber;
            doc.firstChild.appendChild(element);

            return doc;
        }

        /// <summary>
        ///     Exports the ArcFM XML Properties for the database connections specified in the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void Export(Dictionary<string, string> args)
        {
            foreach (var arg in args)
            {
                var workspace = WorkspaceFactories.Open(Path.GetFullPath(arg.Key));
                var dbi = (IDataset)workspace;

                string name = dbi.BrowseName;
                var versionNumber = ArcFM.Version;

                var directory = Path.Combine(Path.GetFullPath(arg.Value), name);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                Log.Info(this, "Exporting ArcFM XML Properties");
                Log.Info(this, "- Database: \t{0}", name);
                Log.Info(this, "- Directory: \t{0}", directory);
                Log.Info(this, "- Version: \t{0}", versionNumber);

                string fileName = Path.Combine(Path.GetFullPath(arg.Value), name + ".xml");
                using (var table = this.CreateCsv(fileName))
                {
                    List<IXMLDOMElement> elements = new List<IXMLDOMElement>();
                    elements.AddRange(this.ExportFeatureClasses(workspace, directory, versionNumber, table));
                    elements.AddRange(this.ExportTables(workspace, directory, versionNumber, table));
                    elements.AddRange(this.ExportRelationships(workspace, directory, versionNumber, table));

                    if (elements.Any())
                    {
                        var doc = this.CreateDocument(versionNumber);

                        foreach (var element in elements)
                        {
                            doc.lastChild.appendChild(element);
                        }

                        XDocument xdoc = XDocument.Parse(doc.xml);
                        xdoc.Save(fileName, SaveOptions.None);

                        Log.Info(this, "{0}: \t{1}", name, fileName);
                    }

                    table.WriteCsv(Path.ChangeExtension(fileName, ".csv"));
                }
            }
        }

        /// <summary>
        ///     Exports the objects represented in the grouped list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory">The directory.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="ie">The import / export interface used to create the XML.</param>
        /// <param name="list">The list.</param>
        /// <returns>Returns a <see cref="IEnumerable{IXMLDOMElement}" /> representing the XML of the objects.</returns>
        private IEnumerable<Tuple<IXMLDOMElement, T>> Export<T>(string directory, string versionNumber, IMMXMLImportExport4 ie, IEnumerable<IGrouping<string, T>> list)
        {
            foreach (var grouping in list)
            {
                Log.Info(this, "Feature Dataset: {0}", grouping.Key);

                string subdirectory = Path.Combine(directory, grouping.Key);
                if (!Directory.Exists(subdirectory)) Directory.CreateDirectory(subdirectory);

                foreach (var table in grouping)
                {
                    var element = ie.Export(((IDataset)table));
                    if (element != null)
                    {
                        IXMLDOMDocument doc = this.CreateDocument(versionNumber);
                        doc.lastChild.appendChild(element);

                        string name = ((IDataset)table).Name;
                        string fileName = Path.Combine(subdirectory, name + ".XML");

                        XDocument xdoc = XDocument.Parse(doc.xml);
                        xdoc.Save(fileName, SaveOptions.None);

                        Log.Info(this, "\t{0}", name);

                        yield return Tuple.Create(element, table);
                    }
                }
            }
        }

        /// <summary>
        ///     Exports the ArcFM Edit Event Properties to a CSV file.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="source">The source.</param>
        /// <param name="editEvents">The edit events.</param>
        private void Export<T>(DataTable table, IObjectClass source, params mmEditEvent[] editEvents)
        {
            if (source == null) return;

            var configTopLevel = ConfigTopLevel.Instance;
            var dataset = source as IDataset;
            var featureDataset = (source is IFeatureClass) ? ((IFeatureClass)source).FeatureDataset : null;

            var subtypes = new Dictionary<int, string>();
            var list = source.GetSubtypes().ToList();
            if (list.Any())
            {
                subtypes = list.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            foreach (var editEvent in editEvents)
            {
                var values = configTopLevel.GetAutoValues(source, editEvent);
                if (values.Any())
                {
                    foreach (var value in values)
                    {
                        var subtypeCode = value.Key;
                        var subtypeName = subtypes.ContainsKey(subtypeCode) ? subtypes[subtypeCode] : "All";

                        foreach (var a in value.Value)
                        {
                            var row = this.ExportRow<T>(a, subtypeCode, subtypeName, dataset, featureDataset);
                            table.Rows.Add(row);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Exports the ArcFM Edit Event Properties to a CSV file.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="source">The source.</param>
        /// <param name="editEvents">The edit events.</param>
        private void Export(DataTable table, IRelationshipClass source, params mmEditEvent[] editEvents)
        {
            if (source == null) return;

            var configTopLevel = ConfigTopLevel.Instance;
            var dataset = source as IDataset;
            var featureDataset = source.FeatureDataset;

            foreach (var editEvent in editEvents)
            {
                var values = configTopLevel.GetAutoValues(source, editEvent);
                foreach (var value in values)
                {
                    var row = this.ExportRow<IMMRelationshipAUStrategy>(value, -1, null, dataset, featureDataset);
                    table.Rows.Add(row);
                }
            }
        }

        /// <summary>
        ///     Exports the feature classes.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="table">The table.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IXMLDOMElement}" /> representing the XML of the objects.
        /// </returns>
        private IEnumerable<IXMLDOMElement> ExportFeatureClasses(IWorkspace workspace, string directory, string versionNumber, DataTable table)
        {
            IMMXMLImportExport4 ie = new MMFieldInfoIEClass();

            var featureClasses = workspace.GetFeatureClasses().GroupBy(kvp => (kvp.FeatureDataset != null) ? kvp.FeatureDataset.Name : "");
            var exports = this.Export(directory, versionNumber, ie, featureClasses);

            foreach (var tuple in exports)
            {
                this.Export<IMMSpecialAUStrategyEx>(table, tuple.Item2, mmEditEvent.mmEventFeatureCreate,
                    mmEditEvent.mmEventFeatureUpdate,
                    mmEditEvent.mmEventFeatureDelete,
                    mmEditEvent.mmEventBeforeFeatureSplit,
                    mmEditEvent.mmEventFeatureSplit,
                    mmEditEvent.mmEventAfterFeatureSplit);

                this.Export<IMMAbandonAUStrategy>(table, tuple.Item2, mmEditEvent.mmEventAbandon);
                this.Export<IMMValidationRule>(table, tuple.Item2, mmEditEvent.mmEventValidationRule);

                yield return tuple.Item1;
            }
        }

        /// <summary>
        ///     Exports the relationships.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="table">The table.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IXMLDOMElement}" /> representing the XML of the objects.
        /// </returns>
        private IEnumerable<IXMLDOMElement> ExportRelationships(IWorkspace workspace, string directory, string versionNumber, DataTable table)
        {
            IMMXMLImportExport4 ie = new MMRelClassIEClass();

            var relationships = workspace.GetRelationshipClasses().GroupBy(kvp => (kvp.FeatureDataset != null) ? kvp.FeatureDataset.Name : "");
            var exports = this.Export(directory, versionNumber, ie, relationships);

            foreach (var tuple in exports)
            {
                this.Export(table, tuple.Item2, mmEditEvent.mmEventRelationshipCreated,
                    mmEditEvent.mmEventRelationshipUpdated,
                    mmEditEvent.mmEventRelationshipDeleted);

                yield return tuple.Item1;
            }
        }

        /// <summary>
        ///     Exports the row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="subtypeCode">The subtype code.</param>
        /// <param name="subtypeName">Name of the subtype.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="featureDataset">The feature dataset.</param>
        /// <returns></returns>
        private object[] ExportRow<T>(IMMAutoValue value, int subtypeCode, string subtypeName, IDataset dataset, IFeatureDataset featureDataset)
        {
            var row = new List<object>();

            var name = this.GetComponentName<T>(value.AutoGenID);

            row.Add(featureDataset != null ? featureDataset.Name : "");
            row.Add(dataset != null ? dataset.Name : "");
            row.Add(subtypeCode);
            row.Add(subtypeName);
            row.Add(((ID8ListItem)value).DisplayName);
            row.Add(name);
            row.Add(value.AutoGenID.Value);

            return row.ToArray();
        }

        /// <summary>
        ///     Exports the tables.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="table">The table.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IXMLDOMElement}" /> representing the XML of the objects.
        /// </returns>
        private IEnumerable<IXMLDOMElement> ExportTables(IWorkspace workspace, string directory, string versionNumber, DataTable table)
        {
            IMMXMLImportExport4 ie = new MMFieldInfoIEClass();

            var tables = workspace.GetTables().GroupBy(kvp => "");
            var exports = this.Export(directory, versionNumber, ie, tables);

            foreach (var tuple in exports)
            {
                var source = tuple.Item2 as IObjectClass;

                this.Export<IMMSpecialAUStrategyEx>(table, source, mmEditEvent.mmEventFeatureCreate,
                    mmEditEvent.mmEventFeatureUpdate,
                    mmEditEvent.mmEventFeatureDelete);

                this.Export<IMMValidationRule>(table, source, mmEditEvent.mmEventValidationRule);

                yield return tuple.Item1;
            }
        }

        /// <summary>
        ///     Gets the name of the component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid">The uid.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the component.
        /// </returns>
        /// <remarks>
        ///     Defaults to the name specified on the <see cref="IMMExtObject" />
        ///     interface.
        /// </remarks>
        private string GetComponentName<T>(IUID uid)
        {
            object component;

            try
            {
                component = uid.Create<T>();
            }
            catch (Exception)
            {
                return "UNREGISTERED PROGRAM";
            }

            if (component is IMMSpecialAUStrategy)
            {
                return ((IMMSpecialAUStrategy)component).Name;
            }
            if (component is IMMSpecialAUStrategyEx)
            {
                return ((IMMSpecialAUStrategyEx)component).Name;
            }
            if (component is IMMAbandonAUStrategy)
            {
                return ((IMMAbandonAUStrategy)component).Name;
            }
            if (component is IMMAttrAUStrategy)
            {
                return ((IMMAttrAUStrategy)component).Name;
            }
            if (component is IMMRelationshipAUStrategy)
            {
                return ((IMMRelationshipAUStrategy)component).Name;
            }
            if (component is IMMExtObject)
            {
                return ((IMMExtObject)component).Name;
            }

            return "UNREGISTERED PROGRAM";
        }

        /// <summary>
        ///     Imports the ArcFM Properties from the XML files specified by the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void Import(Dictionary<string, string> args)
        {
            foreach (var arg in args)
            {
                var workspace = WorkspaceFactories.Open(Path.GetFullPath(arg.Key));
                var dbi = (IDataset)workspace;

                var directory = Path.GetFullPath(arg.Value);
                string name = dbi.BrowseName;
                var versionNumber = ArcFM.Version;

                Log.Info(this, "Importing ArcFM XML Properties");
                Log.Info(this, "- Database: \t{0}", name);
                Log.Info(this, "- Directory: \t{0}", directory);
                Log.Info(this, "- Version: \t{0}", versionNumber);

                var files = Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories);
                var results = this.Import(workspace, files);

                var errors = results.Where(o => !o.Value).ToArray();
                if (errors.Any())
                {
                    Log.Warn(this, "- Errors: \t{0}", errors.Length);
                    foreach (var error in errors)
                        Log.Warn(this, "\t{0}", error.Key);
                }
            }
        }

        /// <summary>
        ///     Imports the specified files into the workspace.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="fileNames">The file names.</param>
        private Dictionary<string, bool> Import(IWorkspace workspace, string[] fileNames)
        {
            Log.Info(this, "- Import File(s): \t{0}", fileNames.Length);

            var ies = new Dictionary<string, IMMXMLImportExport4>()
            {
                {"MMFieldInfoIE", new MMFieldInfoIEClass()},
                {"MMObjClassIE", new MMObjClassIEClass()},
                {"MMRelClassIE", new MMRelClassIEClass()}
            };

            var utils = new mmFrameworkUtilitiesClass();
            var results = new Dictionary<string, bool>();

            foreach (var fileName in fileNames)
            {
                var fileName1 = fileName;

                workspace.PerformOperation(true, () =>
                {
                    IXMLDOMDocument doc = new DOMDocumentClass();
                    if (doc.load(fileName1))
                    {
                        var dtd = utils.AddDTD(mmTopLevelType.GXXMLTOPLEVEL, ref doc);
                        if (dtd != 0) return false;

                        var nodes = doc.getElementsByTagName("IEPROGID");
                        nodes.reset();

                        IXMLDOMElement element;
                        while ((element = nodes.nextNode() as IXMLDOMElement) != null)
                        {
                            var parentNode = (IXMLDOMElement)element.parentNode;
                            var node = parentNode.selectSingleNode("FEATURENAME") ?? element.selectSingleNode("NAME");

                            Log.Info(this, "{0}:", node.text);

                            var ie = ies.Select(o => o.Value).FirstOrDefault(kvp => element.text.EndsWith(kvp.ProgID, StringComparison.InvariantCultureIgnoreCase));
                            if (ie != null)
                            {
                                bool success;

                                try
                                {
                                    success = ie.Import(workspace, element, mmGxXMLOptions.mmGXOOverwrite, mmGxXMLSubtypeOptions.mmGXOReplace);
                                    Log.Info(this, "\t{0} => {1}", ie.DisplayName, success ? "SUCCESS" : "FAILED");
                                }
                                catch (Exception e)
                                {
                                    success = false;
                                    Log.Error(this, string.Format("\t{0} => {1}", ie.DisplayName, e.Message));
                                }

                                results.Add(node.text, success);
                            }
                        }

                        return results.Last().Value;
                    }

                    return false;
                });
            }

            return results;
        }

        /// <summary>
        ///     Reads the contents of the specified file into a <see cref="DataTable" />.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private DataTable ReadCsv(string fileName)
        {
            string connectionString = string.Format(CultureInfo.InvariantCulture, @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Text"";",
                Path.GetDirectoryName(fileName));

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM " + Path.GetFileName(fileName);
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            var table = new DataTable();
                            table.TableName = Path.GetFileNameWithoutExtension(fileName);
                            table.Load(dr);
                            return table;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Validates the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void ValidateArguments(ProgramArguments args)
        {
            if (!args.Directories.Any())
            {
                args.Directories = args.ConnectionFiles.Select(f => AppDomain.CurrentDomain.BaseDirectory).ToArray();
            }
            else if (args.Directories.Length < args.ConnectionFiles.Length)
            {
                Array.Resize(ref args.Directories, args.ConnectionFiles.Length);

                string p = AppDomain.CurrentDomain.BaseDirectory;

                for (int i = 0; i < args.Directories.Length; i++)
                {
                    if (!string.IsNullOrEmpty(args.Directories[i]))
                        p = args.Directories[i];
                    else
                        args.Directories[i] = p;
                }
            }
        }

        #endregion
    }
}