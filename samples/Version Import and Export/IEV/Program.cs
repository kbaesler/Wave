using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;

using Miner;
using Miner.CommandLine;
using Miner.Interop;
using Miner.Interop.msxml2;

namespace Wave.IEV
{
    internal class ProgramArguments
    {
        #region Fields

        [Argument(ArgumentType.Required, ShortName = "f")] public string ConnectionFile;
        [Argument(ArgumentType.LastOccurenceWins, ShortName = "d")] public string Directory = AppDomain.CurrentDomain.BaseDirectory;
        [Argument(ArgumentType.LastOccurenceWins, ShortName = "t")] public ProgramTask Task = ProgramTask.Export;
        [Argument(ArgumentType.LastOccurenceWins, ShortName = "v")] public string VersionName;

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
        /// Exports or imports the version edits as XML files.
        /// </summary>
        /// <param name="args">The arguments.</param>
        internal void Run(ProgramArguments args)
        {
            using (RuntimeAuthorization lic = new RuntimeAuthorization(ProductCode.EngineOrDesktop))
            {
                if (lic.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard, mmLicensedProductCode.mmLPArcFM))
                {
                    var workspace = WorkspaceFactories.Open(Path.GetFullPath(args.ConnectionFile));
                    var source = workspace.FindVersion(args.VersionName);                    
                    var target = GetParent(source);

                    if (source != null && target != null)
                    {
                        var exportFileName = Path.Combine(args.Directory, args.VersionName + ".xml");
                        ExportDataChanges(source, target, exportFileName, esriExportDataChangesOption.esriExportToXML, true);
                    }
                }
            }
        }

        #endregion
        /// <summary>
        ///     Gets the parent version.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IVersion" /> representing the parent version.</returns>
        public static IVersion GetParent(IVersion source)
        {
            if (source.HasParent())
            {
                return ((IVersionedWorkspace)source).FindVersion(source.VersionInfo.Parent.VersionName);
            }

            return null;
        }

        /// <summary>
        ///     Exports the version differences to the specified export file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="exportFileName">Name of the export file.</param>
        /// <param name="exportOption">The export option.</param>
        /// <param name="overwrite">if set to <c>true</c> when the delta file should be overwritten when it exists.</param>
        public static void ExportDataChanges(IVersion source, IVersion target, string exportFileName, esriExportDataChangesOption exportOption, bool overwrite)
        {
            IVersionDataChangesInit vdci = new VersionDataChangesClass();
            IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)source).FullName;
            IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)target).FullName;
            vdci.Init(wsNameSource, wsNameTarget);

            IDataChanges dataChanges = (IDataChanges)vdci;

            IExportDataChanges2 edc = new DataChangesExporterClass();
            edc.ExportDataChanges(exportFileName, exportOption, dataChanges, overwrite);
        }
    }
}