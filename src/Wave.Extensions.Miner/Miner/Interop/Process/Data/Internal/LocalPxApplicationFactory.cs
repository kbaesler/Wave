using System.Text;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     A factory used to make a connection to an Access Database (or MDB) process framework database.
    /// </summary>
    internal class LocalPxApplicationFactory : PxApplicationFactory
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
        public override IMMPxApplication Open(string userName, string password, string dataSource, string database, bool isOSA, params string[] extensionNames)
        {
            var connectionString = new StringBuilder();

            connectionString.Append("Provider=Microsoft.Jet.OLEDB.4.0");
            connectionString.Append("; Data Source=");
            connectionString.Append(dataSource);
            connectionString.Append(";User Id=admin;Password=;");

            return base.Open(connectionString.ToString(), userName, extensionNames);
        }

        #endregion
    }
}