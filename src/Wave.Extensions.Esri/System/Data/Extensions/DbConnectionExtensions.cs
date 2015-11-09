using System.Data.Common;
using System.Data.OleDb;
using System.IO;

namespace System.Data
{
    /// <summary>
    ///     Provides extension methods for the <see cref="DbConnection" /> class.
    /// </summary>
    public static class DbConnectionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery(this DbConnection source, string commandText)
        {
            return source.ExecuteNonQuery(commandText, null);
        }

        /// <summary>
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery(this DbConnection source, string commandText, DbTransaction transaction)
        {
            // Create a new INSERT, UPDATE or DELETE command for the connection.
            using (DbCommand cmd = source.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = transaction;

                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Executes the given SELECT statement and returns the rows as part of a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     A <see cref="DbDataReader" /> of the results.
        /// </returns>
        public static DbDataReader ExecuteReader(this DbConnection source, string commandText)
        {
            return source.ExecuteReader<DbDataReader>(commandText);
        }

        /// <summary>
        ///     Executes the given SELECT statement and returns the rows as part of a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     A <see cref="DbDataReader" /> of the results.
        /// </returns>
        public static TDataReader ExecuteReader<TDataReader>(this DbConnection source, string commandText)
            where TDataReader : DbDataReader
        {
            // Create a new select command for the connection.
            using (DbCommand cmd = source.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;

                // Return the reader.
                return (TDataReader) cmd.ExecuteReader();
            }
        }

        /// <summary>
        ///     Retrieves a single value (for example, an aggregate value) from a database using the specified
        ///     <paramref name="commandText" /> statement.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     The value from the statement.
        /// </returns>
        public static TValue ExecuteScalar<TValue>(this DbConnection source, string commandText)
        {
            // Create a new select command for the connection.
            using (DbCommand cmd = source.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;

                return TypeCast.Cast(cmd.ExecuteScalar(), default(TValue));
            }
        }

        /// <summary>
        ///     Exports the table into the access database provided by the connection.
        /// </summary>
        /// <param name="connection">The connection to the access database.</param>
        /// <param name="mdbTableName">Name of the MDB table.</param>
        /// <param name="tableName">Name of the source table.</param>
        /// <param name="server">The name of the ODBC server.</param>
        /// <param name="driverName">Name of the ODBC driver.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of records inserted into the table.
        /// </returns>
        public static int ExportToMdb(this OleDbConnection connection, string mdbTableName, string tableName, string server, string driverName = "Driver={Oracle in OraClient11g_home1}")
        {
            using (var command = connection.CreateCommand())
            {
                if (connection.GetSchema("Tables", new[] {null, null, mdbTableName, "TABLE"}).Rows.Count > 0)
                {
                    command.CommandText = "DROP TABLE " + mdbTableName;
                    command.ExecuteNonQuery();
                }

                command.CommandText = "SELECT * INTO " + mdbTableName + " FROM [" + tableName + "] IN '' [ODBC;" + driverName + ";" + server + "]";

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Exports the table from the database connection to the Excel (XLS) file.
        /// </summary>
        /// <param name="source">The connection to the access database.</param>
        /// <param name="excelFileName">Name of the excel file.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns>
        /// Returns a <see cref="int" /> representing the number of records inserted into the spreadsheet.
        /// </returns>
        /// <exception cref="ArgumentNullException">excelFileName</exception>
        /// <exception cref="ArgumentException">The excel file must be created with an '.xls' extension.;excelFileName</exception>
        public static int ExportToXls(this OleDbConnection source, string excelFileName, string tableName, string sheetName)
        {
            if (excelFileName == null)
                throw new ArgumentNullException("excelFileName");

            if (!Path.GetExtension(excelFileName).Equals(".xls", StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("The excel file must be created with an '.xls' extension.", "excelFileName");

            using (var command = source.CreateCommand())
            {
                command.CommandText = string.Format("SELECT * INTO [Excel 8.0;HDR=Yes;DATABASE={0}].[{1}] FROM [{2}]", excelFileName, sheetName, tableName);

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Fills a <see cref="DataTable" /> with table data from the specified <paramref name="commandText" /> statement.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     The <see cref="DataTable" /> containing the data.
        /// </returns>
        public static DataTable Fill(this DbConnection source, string commandText, string tableName)
        {
            // Read the results from the reader into a table.
            using (DbDataReader dr = source.ExecuteReader(commandText))
            {
                // Load the table.
                DataTable dt = new DataTable(tableName);
                dt.Load(dr, LoadOption.PreserveChanges);
                return dt;
            }
        }

        #endregion
    }
}