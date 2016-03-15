using System;

using ADODB;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An abstract factory used to make a connection to the process framework instance.
    /// </summary>
    public abstract class PxApplicationFactory : IPxApplicationFactory
    {
        #region Public Methods

        /// <summary>
        ///     Opens a connection to the process framework database.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="database">The database.</param>
        /// <param name="isOSA">if set to <c>true</c> the connection should be made using operating system authentication.</param>
        /// <param name="extensionNames">The names of process framework extensions.</param>
        /// <returns>
        ///     Returns the <see cref="IMMPxApplication" /> application reference; otherwise <c>null</c>.
        /// </returns>
        public abstract IMMPxApplication Open(string userName, string password, string dataSource, string database, bool isOSA, params string[] extensionNames);

        /// <summary>
        /// Opens a connection to the process framework database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="extensionNames">The names of process framework extensions.</param>
        /// <returns>
        /// Returns the <see cref="IMMPxApplication" /> application reference; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// userName
        /// or
        /// extensionNames
        /// </exception>
        public virtual IMMPxApplication Open(string connectionString, string userName, params string[] extensionNames)
        {
            if(connectionString == null) throw new ArgumentNullException("connectionString");
            if(userName == null) throw  new ArgumentNullException("userName");
            if(extensionNames == null) throw new ArgumentNullException("extensionNames");
            
            // Open the connection with the given properties.
            Connection connection = new ConnectionClass();
            connection.Open(connectionString);

            // Create PxLogin object.
            IMMPxLogin pxLogin = new PxLoginClass();
            pxLogin.Connection = connection;

            // Load the extensions.
            IMMEnumExtensionNames extensions = new PxExtensionNamesClass();
            foreach (var extension in extensionNames)
                extensions.Add(extension);

            // Initialize the application using the user name and extensions.
            IMMPxApplication pxApp = new PxApplicationClass();
            var pxAppEx3 = (IMMPxApplicationEx3) pxApp;
            pxAppEx3.Startup(pxLogin, extensions, ref userName);

            return pxApp;
        }

        #endregion
    }
}