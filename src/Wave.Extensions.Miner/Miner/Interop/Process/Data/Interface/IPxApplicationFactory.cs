namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides methods for creating an <see cref="IMMPxApplication" /> object.
    /// </summary>
    public interface IPxApplicationFactory
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
        IMMPxApplication Open(string userName, string password, string dataSource, string database, bool isOSA, params string[] extensionNames);

        /// <summary>
        ///     Opens a connection to the process framework database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="extensionNames">The names of process framework extensions.</param>
        /// <returns>
        ///     Returns the <see cref="IMMPxApplication" /> application reference; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     connectionString
        ///     or
        ///     userName
        ///     or
        ///     extensionNames
        /// </exception>
        IMMPxApplication Open(string connectionString, string userName, params string[] extensionNames);

        #endregion
    }
}