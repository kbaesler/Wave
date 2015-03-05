using System;
using System.IO;

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides access to remote and local geodatabases.
    /// </summary>
    public static class WorkspaceFactories
    {
        #region Public Methods

        /// <summary>
        /// Gets the workspace factory.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// Returns a <see cref="IWorkspaceFactory" /> representing the supported factory for the file.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The workspace factory cannot be determined because the file was not found.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The workspace factory cannot be determined because the file was not found.</exception>
        /// <exception cref="System.NotSupportedException">The workspace factory for the file is not supported.</exception>
        public static IWorkspaceFactory GetFactory(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");

            if ((fileName.EndsWith(".gdb", StringComparison.OrdinalIgnoreCase)))
            {
                if (!Directory.Exists(fileName))
                    throw new DirectoryNotFoundException("The workspace factory cannot be determined because the file was not found.");

                return new FileGDBWorkspaceFactoryClass();
            }

            if (!File.Exists(fileName))
                throw new FileNotFoundException("The workspace factory cannot be determined because the file was not found.", fileName);

            IWorkspaceFactory[] list =
            {
                new AccessWorkspaceFactoryClass(),
                new SdeWorkspaceFactoryClass(),
            };

            foreach (var l in list)
            {
                if (l.IsWorkspace(fileName))
                    return l;
            }

            throw new NotSupportedException("The workspace factory for the file is not supported.");
        }

        /// <summary>
        /// Connects to the geodatabase given the specified parameters.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// Returns the <see cref="IWorkspace" /> representing the connection to the geodatabase; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        public static IWorkspace Open(string fileName)
        {
            if(fileName == null) throw new ArgumentNullException("fileName");

            IWorkspaceFactory factory = GetFactory(fileName);
            return factory.OpenFromFile(fileName, 0);
        }

        /// <summary>
        ///     Connects to the remote geodatabase given the specified parameters.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="version">The version.</param>
        /// <param name="database">The database.</param>
        /// <param name="password">The password.</param>
        /// <param name="username">The username.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="authentication">The authentication (either DBMS or OSA).</param>
        /// <returns>
        ///     Returns the <see cref="IWorkspace" /> representing the connection to the geodatabase; otherwise <c>null</c>.
        /// </returns>
        public static IWorkspace Open(string server, string instance, string version, string database, string password, string username,
            DateTime? timestamp = null, string authentication = "DBMS")
        {           
            IWorkspaceFactory factory;
            DBMS type = GetDBMS(server, instance, database, version);
            switch (type)
            {
                case DBMS.Access:
                    factory = new AccessWorkspaceFactoryClass();
                    return factory.OpenFromFile(database, 0);

                case DBMS.Oracle:
                case DBMS.SqlServer:

                    IPropertySet propset = new PropertySetClass();
                    propset.SetProperty("SERVER", server);
                    propset.SetProperty("INSTANCE", instance);
                    propset.SetProperty("USER", username);
                    propset.SetProperty("PASSWORD", password);
                    propset.SetProperty("VERSION", version);
                    propset.SetProperty("AUTHENTICATION_MODE", authentication);

                    if (type == DBMS.SqlServer)
                    {
                        propset.SetProperty("DATABASE", database);
                    }

                    if (timestamp.HasValue)
                    {
                        propset.SetProperty("HISTORICAL_TIMESTAMP", timestamp.Value);
                    }

                    factory = new SdeWorkspaceFactoryClass();
                    return factory.Open(propset, 0);

                case DBMS.File:

                    factory = new FileGDBWorkspaceFactoryClass();
                    return factory.OpenFromFile(database, 0);
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Determines the <see cref="DBMS" /> enumeration type based on the parameters.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="database">The database.</param>
        /// <param name="version">The version.</param>
        /// <returns>
        ///     Returns the <see cref="DBMS" /> enumeration representing the type.
        /// </returns>
        private static DBMS GetDBMS(string server, string instance, string database, string version)
        {
            if (string.IsNullOrEmpty(database))
            {
                if (!string.IsNullOrEmpty(instance))
                {
                    return DBMS.Oracle;
                }

                return DBMS.Unknown;
            }

            if (!string.IsNullOrEmpty(instance))
            {
                return DBMS.SqlServer;
            }

            if (string.IsNullOrEmpty(server) && string.IsNullOrEmpty(version))
            {
                if (database.EndsWith(".gdb", StringComparison.OrdinalIgnoreCase))
                {
                    return DBMS.File;
                }

                return DBMS.Access;
            }

            return DBMS.Unknown;
        }

        #endregion
    }
}