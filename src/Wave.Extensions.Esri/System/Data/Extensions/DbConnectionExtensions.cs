using System.Data.Common;

namespace System.Data
{
    /// <summary>
    /// Provides extension methods for the <see cref="DbConnection"/> class.
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