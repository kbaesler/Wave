using System.Text;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     A factory for making a connection to an Oracle or SQL Server process framework database.
    /// </summary>
    internal class RemotePxApplicationFactory : PxApplicationFactory
    {
        #region Public Methods

        /// <summary>
        ///     Opens a connection to the process framework database.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="dataSource">The data source or SID of the database instance.</param>
        /// <param name="database">The database (used for SQL Server).</param>
        /// <param name="isOSA">if set to <c>true</c> the connection should be made using operating system authentication.</param>
        /// <param name="extensionNames">The names of process framework extensions.</param>
        /// <returns>
        ///     Returns the <see cref="IMMPxApplication" /> application reference; otherwise <c>null</c>.
        /// </returns>
        public override IMMPxApplication Open(string userName, string password, string dataSource, string database, bool isOSA, params string[] extensionNames)
        {
            var connectionString = new StringBuilder();
            if (string.IsNullOrEmpty(database))
            {
                connectionString.Append("Provider=OraOLEDB.Oracle");
                connectionString.Append("; Persist Security Info=True; Data Source=");
                connectionString.Append(dataSource);

                if (isOSA)
                {
                    connectionString.Append("; OSAuthent=1;");
                }
                else
                {
                    connectionString.AppendFormat(";User Id={0};Password={1};", userName, password);
                }
            }
            else
            {
                connectionString.Append("Provider=SQLOLEDB");
                connectionString.Append("; Data Source=");
                connectionString.Append(dataSource);
                connectionString.Append("; Initial Catalog=");
                connectionString.Append(database);

                if (isOSA)
                {
                    connectionString.Append("; Integrated Security=SSPI;");
                }
                else
                {
                    connectionString.AppendFormat(";User Id={0};Password={1};", userName, password);
                }
            }

            return base.Open(connectionString.ToString(), userName, extensionNames);
        }

        #endregion
    }
}