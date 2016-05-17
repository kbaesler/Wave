using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner;
using Miner.CommandLine;
using Miner.Interop;
using Miner.Interop.msxml2;

namespace Wave.IEXML
{
    internal class ProgramArguments
    {
        #region Fields

        [Argument(ArgumentType.Required, ShortName = "f")] public string ConnectionFile;
        [Argument(ArgumentType.LastOccurenceWins, ShortName = "d")] public string Directory = AppDomain.CurrentDomain.BaseDirectory;
        [Argument(ArgumentType.LastOccurenceWins, ShortName = "t")] public ProgramTask Task = ProgramTask.Export;
        [Argument(ArgumentType.LastOccurenceWins, ShortName = "v")] public string VersionNumber;

        #endregion
    }

    internal enum ProgramTask
    {
        Import,
        Export
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
                Log.Error(typeof (Program), e);
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
            using (RuntimeAuthorization lic = new RuntimeAuthorization(ProductCode.EngineOrDesktop))
            {
                if (lic.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard, mmLicensedProductCode.mmLPArcFM))
                {
                    var workspace = WorkspaceFactories.Open(args.ConnectionFile);
                    var dbi = (IDataset) workspace;

                    string name = dbi.BrowseName;
                    IMMProductData pi = new BrandingResource();
                    string versionNumber = args.VersionNumber ?? pi.ProductVersion();

                    if (args.Task == ProgramTask.Export)
                    {
                        string directory = Path.Combine(args.Directory, name);
                        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                        Log.Info(this, "Exporting ArcFM XML Properties");
                        Log.Info(this, "- Database: \t{0}", name);
                        Log.Info(this, "- Directory: \t{0}", args.Directory);
                        Log.Info(this, "- Version: \t{0}", versionNumber);

                        List<IXMLDOMElement> elements = new List<IXMLDOMElement>();
                        elements.AddRange(this.ExportFeatureClasses(workspace, directory, versionNumber));
                        elements.AddRange(this.ExportTables(workspace, directory, versionNumber));
                        elements.AddRange(this.ExportRelationships(workspace, directory, versionNumber));

                        if (elements.Any())
                        {
                            var doc = this.CreateDocument(versionNumber);

                            foreach (var element in elements)
                            {
                                doc.lastChild.appendChild(element);
                            }

                            string fileName = Path.Combine(args.Directory, name + ".xml");
                            XDocument xdoc = XDocument.Parse(doc.xml);
                            xdoc.Save(fileName, SaveOptions.None);

                            Log.Info(this, "{0}: \t{1}", name, fileName);
                        }
                    }
                    else
                    {
                        Log.Info(this, "Importing ArcFM XML Properties");
                        Log.Info(this, "- Database: \t{0}", name);
                        Log.Info(this, "- Directory: \t{0}", args.Directory);
                        Log.Info(this, "- Version: \t{0}", versionNumber);

                        var files = Directory.GetFiles(Path.GetFullPath(args.Directory), "*.xml", SearchOption.AllDirectories);
                        this.Import(workspace, files);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

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
        ///     Exports the objects represeted in the grouped list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory">The directory.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="ie">The import / export interface used to create the XML.</param>
        /// <param name="list">The list.</param>
        /// <returns>Returns a <see cref="IEnumerable{IXMLDOMElement}" /> representing the XML of the objects.</returns>
        private IEnumerable<IXMLDOMElement> Export<T>(string directory, string versionNumber, IMMXMLImportExport4 ie, IEnumerable<IGrouping<string, T>> list)
        {
            foreach (var grouping in list)
            {
                Log.Info(this, "Feature Dataset: {0}", grouping.Key);

                string subdirectory = Path.Combine(directory, grouping.Key);
                if (!Directory.Exists(subdirectory)) Directory.CreateDirectory(subdirectory);

                foreach (var table in grouping)
                {
                    IXMLDOMDocument doc = this.CreateDocument(versionNumber);
                    var element = ie.Export(((IDataset) table));
                    if (element != null)
                    {
                        doc.lastChild.appendChild(element);

                        string name = ((IDataset) table).Name;
                        string fileName = Path.Combine(subdirectory, name + ".XML");

                        XDocument xdoc = XDocument.Parse(doc.xml);
                        xdoc.Save(fileName, SaveOptions.None);

                        Log.Info(this, "\t{0}", name);

                        yield return element;
                    }
                }
            }
        }

        /// <summary>
        ///     Exports the feature classes.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <returns>Returns a <see cref="IEnumerable{IXMLDOMElement}" /> representing the XML of the objects.</returns>
        private IEnumerable<IXMLDOMElement> ExportFeatureClasses(IWorkspace workspace, string directory, string versionNumber)
        {
            IMMXMLImportExport4 ie = new MMFieldInfoIEClass();
            return this.Export(directory, versionNumber, ie, workspace.GetFeatureClasses().GroupBy(kvp => (kvp.FeatureDataset != null) ? kvp.FeatureDataset.Name : ""));
        }

        /// <summary>
        ///     Exports the relationships.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <returns>Returns a <see cref="IEnumerable{IXMLDOMElement}" /> representing the XML of the objects.</returns>
        private IEnumerable<IXMLDOMElement> ExportRelationships(IWorkspace workspace, string directory, string versionNumber)
        {
            IMMXMLImportExport4 ie = new MMRelClassIEClass();
            return this.Export(directory, versionNumber, ie, workspace.GetRelationshipClasses().GroupBy(kvp => (kvp.FeatureDataset != null) ? kvp.FeatureDataset.Name : ""));
        }

        /// <summary>
        ///     Exports the tables.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <returns>Returns a <see cref="IEnumerable{IXMLDOMElement}" /> representing the XML of the objects.</returns>
        private IEnumerable<IXMLDOMElement> ExportTables(IWorkspace workspace, string directory, string versionNumber)
        {
            IMMXMLImportExport4 ie = new MMFieldInfoIEClass();
            return this.Export(directory, versionNumber, ie, workspace.GetTables().GroupBy(kvp => ""));
        }

        /// <summary>
        ///     Imports the specified files into the workspace.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="fileNames">The file names.</param>
        private void Import(IWorkspace workspace, string[] fileNames)
        {
            Log.Info(this, "- Import File(s): \t{0}", fileNames.Length);

            var ies = new Dictionary<string, IMMXMLImportExport4>()
            {
                {"MMFieldInfoIE", new MMFieldInfoIEClass()},
                {"MMObjClassIE", new MMObjClassIEClass()},
                {"MMRelClassIE", new MMRelClassIEClass()}
            };

            var utils = new mmFrameworkUtilitiesClass();

            foreach (var fileName in fileNames)
            {
                var fileName1 = fileName;

                workspace.PerformOperation(() =>
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
                            var ie = ies.FirstOrDefault(kvp => element.text.EndsWith(kvp.Key, StringComparison.InvariantCultureIgnoreCase));
                            this.Import(workspace, ie.Value, (IXMLDOMElement) element.parentNode);
                        }
                    }

                    return true;
                });
            }
        }

        /// <summary>
        ///     Imports the specified element into the workspace using the importer.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="ie">The importer.</param>
        /// <param name="element">The element.</param>
        private void Import(IWorkspace workspace, IMMXMLImportExport4 ie, IXMLDOMElement element)
        {
            if (ie == null) return;

            var node = element.selectSingleNode("FEATURENAME") ?? element.selectSingleNode("NAME");

            Log.Info(this, "{0}:", node.text);

            try
            {
                var success = ie.Import(workspace, element, mmGxXMLOptions.mmGXOOverwrite, mmGxXMLSubtypeOptions.mmGXOReplace);
                Log.Info(this, "\t{0} => {1}", ie.DisplayName, success ? "SUCCESS" : "FAILED");
            }
            catch (Exception e)
            {
                Log.Error(this, string.Format("\t{0} => {1}", ie.DisplayName, e.Message));
            }
        }

        #endregion
    }
}