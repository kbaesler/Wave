using System;
using System.IO;

using ESRI.ArcGIS.Geodatabase;

namespace Wave.Samples.ConnectionFiles
{
    internal class Program
    {
        #region Public Methods

        public void Run(string[] args)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("Wave Sample: Connection Files");
            Console.WriteLine("==========================================");

            var connectionFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\ESRI\\Desktop\\ArCatalog\\Minerville.gdb");
            var workspace = WorkspaceFactories.Open(connectionFile);
            var dbms = workspace.GetDBMS();
            Console.WriteLine("\tDBMS: {0}", dbms);
        }

        #endregion

        #region Private Methods

        private static void Main(string[] args)
        {
            try
            {
                new Program().Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
        }

        #endregion
    }
}