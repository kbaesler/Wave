using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace System.Data
{
    /// <summary>
    ///     An enumeration that indicates the command text type.
    /// </summary>
    public enum CommandTextType
    {
        /// <summary>
        ///     The insert command
        /// </summary>
        InsertCommand,

        /// <summary>
        ///     The update command
        /// </summary>
        UpdateCommand,

        /// <summary>
        ///     The delete command
        /// </summary>
        DeleteCommand
    }

    /// <summary>
    ///     Provides extension methods for the <see cref="DbConnection" /> class.
    /// </summary>
    public static class DbConnectionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the parameter to the collection with the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public static void AddWithValue<TDbParameter>(this DbParameterCollection source, string name, DbType type, object value)
            where TDbParameter : DbParameter, new()
        {
            source.AddWithValue<TDbParameter>(name, type, ParameterDirection.Input, value);
        }

        /// <summary>
        ///     Adds the parameter to the collection with the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The value.</param>
        public static void AddWithValue<TDbParameter>(this DbParameterCollection source, string name, DbType type, ParameterDirection direction, object value)
            where TDbParameter : DbParameter, new()
        {
            source.Add(new TDbParameter
            {
                ParameterName = name,
                DbType = type,
                Direction = direction,
                Value = value
            });
        }

        /// <summary>
        ///     Creates the parameterized query.
        /// </summary>
        /// <param name="source">The nodes.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commandTextType">Type of the command text.</param>
        /// <returns>
        ///     Returns a <see cref="String" /> representing the parameterized query.
        /// </returns>
        public static string CreateCommandText(this DbParameterCollection source, string tableName, CommandTextType commandTextType)
        {
            if (source == null)
                return null;

            string commandText;

            if (commandTextType == CommandTextType.InsertCommand)
            {
                string values = string.Join(", ", source.Cast<DbParameter>().Select(o => ":" + o.ParameterName).ToArray());
                string columns = string.Join(", ", source.Cast<DbParameter>().Select(o => o.ParameterName).ToArray());
                commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, columns, values);
            }
            else if (commandTextType == CommandTextType.UpdateCommand)
            {
                string values = string.Join(", ", source.Cast<DbParameter>().Select(o => o + " = :" + o.ParameterName).ToArray());
                commandText = string.Format("UPDATE {0} SET {1}", tableName, values);
            }
            else
            {
                string values = string.Join(" AND ", source.Cast<DbParameter>().Select(o => o + " = :" + o.ParameterName).ToArray());
                commandText = string.Format("DELETE FROM {0} WHERE {1}", tableName, values);
            }

            foreach (DbParameter parameter in source)
                parameter.ParameterName = string.Format(":{0}", parameter.ParameterName.ToUpperInvariant());

            return commandText;
        }

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
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery(this DbTransaction source, string commandText)
        {
            return source.Connection.ExecuteNonQuery(commandText, source);
        }

        /// <summary>
        /// Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commandTextType">Type of the command text.</param>
        /// <param name="addWithValue">The add with value.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery(this DbTransaction source, string tableName, CommandTextType commandTextType, Action<DbParameterCollection> addWithValue, Func<DbParameterCollection, string> commandText)
        {
            return source.Connection.ExecuteNonQuery(tableName, commandTextType, source, addWithValue, commandText);
        }

        /// <summary>
        /// Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commandTextType">Type of the command text.</param>
        /// <param name="addWithValue">The add with value.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery(this DbTransaction source, string tableName, CommandTextType commandTextType, Action<DbParameterCollection> addWithValue)
        {
            return source.Connection.ExecuteNonQuery(tableName, commandTextType, source, addWithValue, collection => collection.CreateCommandText(tableName, commandTextType));
        }

        /// <summary>
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery<TDbParameter>(this DbConnection source, string commandText, TDbParameter[] parameters)
            where TDbParameter : DbParameter
        {
            using (var cmd = source.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery<TDbParameter>(this DbTransaction source, string commandText, TDbParameter[] parameters)
            where TDbParameter : DbParameter
        {
            using (var cmd = source.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Transaction = source;
                cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
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
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commandTextType">Type of the command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="addWithValue">The add with value.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery(this DbConnection source, string tableName, CommandTextType commandTextType, DbTransaction transaction, Action<DbParameterCollection> addWithValue)
        {
            return source.ExecuteNonQuery(tableName, commandTextType, transaction, addWithValue, collection => collection.CreateCommandText(tableName, commandTextType));
        }

        /// <summary>
        /// Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commandTextType">Type of the command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="addWithValue">The add with value.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public static int ExecuteNonQuery(this DbConnection source, string tableName, CommandTextType commandTextType, DbTransaction transaction, Action<DbParameterCollection> addWithValue, Func<DbParameterCollection, string> commandText)
        {
            using (DbCommand cmd = source.CreateCommand())
            {
                addWithValue(cmd.Parameters);

                cmd.CommandText = commandText(cmd.Parameters);
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
        public static DbDataReader ExecuteReader(this DbTransaction source, string commandText)
        {
            return source.Connection.ExecuteReader<DbDataReader>(commandText, source);
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
            return source.ExecuteReader<DbDataReader>(commandText, null);
        }

        /// <summary>
        ///     Executes the given SELECT statement and returns the rows as part of a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        ///     A <see cref="DbDataReader" /> of the results.
        /// </returns>
        public static DbDataReader ExecuteReader(this DbConnection source, string commandText, DbTransaction transaction)
        {
            return source.ExecuteReader<DbDataReader>(commandText, transaction);
        }

        /// <summary>
        ///     Executes the given SELECT statement and returns the rows as part of a <see cref="DbDataReader" />.
        /// </summary>
        /// <typeparam name="TDataReader">The type of the data reader.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        ///     A <see cref="DbDataReader" /> of the results.
        /// </returns>
        public static TDataReader ExecuteReader<TDataReader>(this DbConnection source, string commandText, DbTransaction transaction)
            where TDataReader : DbDataReader
        {
            // Create a new select command for the connection.
            using (DbCommand cmd = source.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = transaction;

                // Return the reader.
                return (TDataReader)cmd.ExecuteReader();
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
        public static TValue ExecuteScalar<TValue>(this DbTransaction source, string commandText)
        {
            return source.Connection.ExecuteScalar<TValue>(commandText, source);
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
        ///     Retrieves a single value (for example, an aggregate value) from a database using the specified
        ///     <paramref name="commandText" /> statement.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        ///     The value from the statement.
        /// </returns>
        public static TValue ExecuteScalar<TValue>(this DbConnection source, string commandText, DbTransaction transaction)
        {
            // Create a new select command for the connection.
            using (DbCommand cmd = source.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = transaction;

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
        /// <param name="driverName">Name of the ODBC driver (for example Driver={Oracle in OraClient11g_home1}).</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of records inserted into the table.
        /// </returns>
        public static int ExportToMdb(this OleDbConnection connection, string mdbTableName, string tableName, string server, string driverName)
        {
            using (var command = connection.CreateCommand())
            {
                if (connection.GetSchema("Tables", new[] { null, null, mdbTableName, "TABLE" }).Rows.Count > 0)
                {
                    command.CommandText = "DROP TABLE " + mdbTableName;
                    command.ExecuteNonQuery();
                }

                command.CommandText = "SELECT * INTO " + mdbTableName + " FROM [" + tableName + "] IN '' [ODBC;" + driverName + ";" + server + "]";

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Exports the table from the database connection to the Excel (XLS) file.
        /// </summary>
        /// <param name="source">The connection to the access database.</param>
        /// <param name="excelFileName">Name of the excel file.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of records inserted into the spreadsheet.
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