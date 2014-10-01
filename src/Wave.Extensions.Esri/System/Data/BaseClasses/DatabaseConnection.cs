using System.Data.Common;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Data
{
    /// <summary>
    ///     A supporting class used to handle querying a <see cref="DbConnection" /> for information.
    /// </summary>
    /// <typeparam name="TConnection">The type of the connection.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class DatabaseConnection<TConnection> : IDatabaseConnection
        where TConnection : DbConnection
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseConnection&lt;TConnection&gt;" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <exception cref="NullReferenceException">The connection cannot be null.</exception>
        protected DatabaseConnection(TConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection", @"The connection cannot be null.");

            this.Connection = connection;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        protected TConnection Connection { get; private set; }

        #endregion

        #region IDatabaseConnection Members

        /// <summary>
        ///     Gets a value indicating whether the database connection is open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the database connection is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen
        {
            get { return this.Connection.State == ConnectionState.Open; }
        }

        /// <summary>
        ///     Begins the database transaction using the <see cref="IsolationLevel.ReadCommitted" /> isolation level.
        /// </summary>
        /// <returns>
        ///     The <see cref="DbTransaction" /> transaction.
        /// </returns>
        public DbTransaction BeginTransaction()
        {
            return this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        ///     Begins the database transaction using the specified <paramref name="isolationLevel" />.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>
        ///     The <see cref="DbTransaction" /> transaction.
        /// </returns>
        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            // Open the connection.
            this.Open();

            return this.Connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        ///     Closes the connection to the databaSE.
        /// </summary>
        public void Close()
        {
            if (this.IsOpen)
                this.Connection.Close();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(string commandText)
        {
            return this.ExecuteNonQuery(commandText, null);
        }

        /// <summary>
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(string commandText, DbTransaction transaction)
        {
            // Open the connection.
            this.Open();

            // Create a new INSERT, UPDATE or DELETE command for the connection.
            using (DbCommand cmd = this.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Connection = this.Connection;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = transaction;

                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Executes the given SELECT statement and returns the rows as part of a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     A <see cref="DbDataReader" /> of the results.
        /// </returns>
        public DbDataReader ExecuteReader(string commandText)
        {
            // Open the connection.
            this.Open();

            // Create a new select command for the connection.
            using (DbCommand cmd = this.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Connection = this.Connection;
                cmd.CommandType = CommandType.Text;

                // Return the reader.
                return cmd.ExecuteReader();
            }
        }

        /// <summary>
        ///     Retrieves a single value (for example, an aggregate value) from a database using the specified
        ///     <paramref name="commandText" /> statement.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     The value from the statement.
        /// </returns>
        public TValue ExecuteScalar<TValue>(string commandText)
        {
            // Open the connection.
            this.Open();

            // Create a new select command for the connection.
            using (DbCommand cmd = this.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Connection = this.Connection;
                cmd.CommandType = CommandType.Text;

                return TypeCast.Cast(cmd.ExecuteScalar(), default(TValue));
            }
        }

        /// <summary>
        ///     Fills a <see cref="DataTable" /> with table data from the specified <paramref name="commandText" /> statement.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     The <see cref="DataTable" /> containing the data.
        /// </returns>
        public DataTable Fill(string commandText, string tableName)
        {
            // Read the results from the reader into a table.
            using (DbDataReader dr = this.ExecuteReader(commandText))
            {
                // Load the table.
                DataTable dt = new DataTable(tableName);
                dt.Locale = CultureInfo.InvariantCulture;
                dt.Load(dr, LoadOption.PreserveChanges);
                return dt;
            }
        }

        /// <summary>
        ///     Opens the connection to the databaSE.
        /// </summary>
        public void Open()
        {
            if (!this.IsOpen)
                this.Connection.Open();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the adapter that is used by the <see cref="DbConnection" />.
        /// </summary>
        /// <returns>The <see cref="DbDataAdapter" /> for the specified connection.</returns>
        protected abstract DbDataAdapter CreateAdapter();

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }
        }

        #endregion
    }
}