using System;
using System.IO;
using System.Linq;

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
        ///     Gets the workspace factory.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="factories">The list of factories that will be used to make the connection.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspaceFactory" /> representing the supported factory for the file.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        ///     The workspace factory cannot be determined because the file was
        ///     not found.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        ///     The workspace factory cannot be determined because the file was not
        ///     found.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The workspace factory for the file is not supported.</exception>
        public static IWorkspaceFactory GetFactory(string fileName, params IWorkspaceFactory[] factories)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");

            if ((fileName.EndsWith(".gdb", StringComparison.OrdinalIgnoreCase)))
            {
                if (!Directory.Exists(fileName))
                    throw new DirectoryNotFoundException("The workspace factory cannot be determined because the file was not found.");
            }
            else if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("The workspace factory cannot be determined because the file was not found.", fileName);
            }

            foreach (var l in factories)
            {
                if (l.IsWorkspace(fileName))
                    return l;
            }

            throw new NotSupportedException("The workspace factory for the file is not supported.");
        }


        /// <summary>
        ///     Gets the workspace factory.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspaceFactory" /> representing the supported factory for the file.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        ///     The workspace factory cannot be determined because the file was
        ///     not found.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        ///     The workspace factory cannot be determined because the file was not
        ///     found.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The workspace factory for the file is not supported.</exception>
        public static IWorkspaceFactory GetFactory(string fileName)
        {
            return GetFactory(fileName, new AccessWorkspaceFactoryClass(),
                new SdeWorkspaceFactoryClass(),
                new FileGDBWorkspaceFactoryClass());
        }

        /// <summary>
        ///     Gets the workspace factory used by the connection properties.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspaceFactory" /> representing the workspace for the connection properties; otherwise
        ///     <c>null</c>
        /// </returns>
        public static IWorkspaceFactory GetFactory(IPropertySet connectionProperties)
        {
            var list = connectionProperties.AsEnumerable()
                .ToDictionary(o => o.Key, o => o.Value.ToString());

            string server;
            list.TryGetValue("SERVER", out server);

            string instance;
            list.TryGetValue("INSTANCE", out instance);

            string database;
            list.TryGetValue("DATABASE", out database);

            string version;
            list.TryGetValue("VERSION", out version);

            DBMS type = GetDBMS(server, instance, database, version);
            switch (type)
            {
                case DBMS.Access:
                    return new AccessWorkspaceFactoryClass();

                case DBMS.Oracle:
                case DBMS.SqlServer:

                    return new SdeWorkspaceFactoryClass();

                case DBMS.File:

                    return new FileGDBWorkspaceFactoryClass();
            }

            throw new NotSupportedException("The workspace factory cannot be determined from the connection properties.");
        }

        /// <summary>
        ///     Connects to the geodatabase given the specified parameters.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///     Returns the <see cref="IWorkspace" /> representing the connection to the geodatabase; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        public static IWorkspace Open(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            IWorkspaceFactory factory = GetFactory(fileName);
            IWorkspace workspace = factory.OpenFromFile(fileName, 0);

            // When using a versioned geodatabase, make sure the version has been refreshed.
            IVersionedWorkspace versionedWorkspace = workspace as IVersionedWorkspace;
            if (versionedWorkspace != null)
            {
                IVersion version = (IVersion) workspace;
                version.RefreshVersion();

                return (IWorkspace)version;
            }

            return workspace;
        }

        /// <summary>
        ///     Connects to the version in the geodatabase that is specified in the connection file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="versionName">Name of the version.</param>
        /// <returns>Returns a <see cref="IVersion" /> representing the version in the geodatabase.</returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        public static IVersion Open(string fileName, string versionName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");

            var factory = GetFactory(fileName);
            var workspace = factory.OpenFromFile(fileName, 0);
            return ((IVersionedWorkspace) workspace).GetVersion(versionName);
        }

        /// <summary>
        ///     Opens a connection to the workspace using specified connection properties.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the connection to the geodatabase.
        /// </returns>
        public static IWorkspace Open(IPropertySet connectionProperties)
        {
            var factory = GetFactory(connectionProperties);
            return factory.Open(connectionProperties, 0);
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