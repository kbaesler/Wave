using System;
using System.Diagnostics;
using System.IO;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;

using Fclp;

namespace Wave.IEV
{
    internal class ProgramArguments
    {
        #region Fields

        public string ConnectionFile;
        public string Delta;
        public ProgramTask Task = ProgramTask.Export;
        public string VersionName;

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
                var p = new FluentCommandLineParser<ProgramArguments>();
                p.Setup(arg => arg.ConnectionFile)
                    .As('c', "connection")
                    .Required();

                p.Setup(arg => arg.Delta)
                    .As('d', "delta")
                    .Required();

                p.Setup(arg => arg.Task)
                    .As('t', "task")
                    .Required();

                p.Setup(arg => arg.VersionName)
                    .As('v', "version")
                    .Required();

                p.Parse(args);

                new Program().Run(p.Object);
            }
            catch (Exception e)
            {
                Log.Error(typeof(Program), e);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Exports or imports the version edits as XML files.
        /// </summary>
        /// <param name="args">The arguments.</param>
        internal void Run(ProgramArguments args)
        {
            using (EsriRuntimeAuthorization lic = new EsriRuntimeAuthorization(ProductCode.EngineOrDesktop))
            {
                if (lic.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard))
                {
                    var workspace = WorkspaceFactories.Open(Path.GetFullPath(args.ConnectionFile));
                    var versionedWorkspace = (IVersionedWorkspace)workspace;

                    switch (args.Task)
                    {
                        case ProgramTask.Import:

                            var version = versionedWorkspace.DefaultVersion.CreateVersion(args.VersionName);
                            var workspaceName = (IWorkspaceName)((IDataset)version).FullName;

                            string changesFileName = Path.GetFullPath(args.Delta);

                            IDeltaDataChangesInit2 ddci = new DeltaDataChangesClass();
                            ddci.Init2(changesFileName, esriExportDataChangesOption.esriExportToXML, false);

                            IImportDataChanges idc = new DataChangesImporterClass();
                            idc.ImportDataChanges(workspaceName, (IDeltaDataChanges)ddci, true, true);

                            break;

                        case ProgramTask.Export:

                            var source = versionedWorkspace.FindVersion(args.VersionName);
                            var target = source.GetParent();

                            IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)source).FullName;
                            IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)target).FullName;

                            var exportFileName = Path.Combine(Path.GetFullPath(args.Delta), args.VersionName + ".xml");

                            IVersionDataChangesInit vdci = new VersionDataChangesClass();
                            vdci.Init(wsNameSource, wsNameTarget);

                            IExportDataChanges2 edc = new DataChangesExporterClass();
                            edc.ExportDataChanges(exportFileName, esriExportDataChangesOption.esriExportToXML, (IDataChanges)vdci, true);
                            break;
                    }
                }
            }
        }

        #endregion
    }
}