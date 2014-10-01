using System.Data.Common;
using System.Runtime.InteropServices;

namespace System.Data
{
    /// <summary>
    ///     Provides methods for sending and receiving information from a databaSE.
    /// </summary>
    [ComVisible(false)]
    public interface IDatabaseConnection : IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether the database connection is open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the database connection is open; otherwise, <c>false</c>.
        /// </value>
        bool IsOpen { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Begins the database transaction using the <see cref="IsolationLevel.ReadCommitted" /> isolation level.
        /// </summary>
        /// <returns>The <see cref="DbTransaction" /> transaction.</returns>
        DbTransaction BeginTransaction();

        /// <summary>
        ///     Begins the database transaction using the specified <paramref name="isolationLevel" />.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>
        ///     The <see cref="DbTransaction" /> transaction.
        /// </returns>
        DbTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        ///     Closes the connection to the databaSE.
        /// </summary>
        void Close();

        /// <summary>
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        int ExecuteNonQuery(string commandText);

        /// <summary>
        ///     Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        int ExecuteNonQuery(string commandText, DbTransaction transaction);

        /// <summary>
        ///     Executes the given SELECT statement and returns the rows as part of a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     A <see cref="DbDataReader" /> of the results.
        /// </returns>
        DbDataReader ExecuteReader(string commandText);

        /// <summary>
        ///     Retrieves a single value (for example, an aggregate value) from a database using the specified
        ///     <paramref name="commandText" /> statement.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <returns>The value from the statement.</returns>
        TValue ExecuteScalar<TValue>(string commandText);

        /// <summary>
        ///     Fills a <see cref="DataTable" /> with table data from the specified <paramref name="commandText" /> statement.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The <see cref="DataTable" /> containing the data.</returns>
        DataTable Fill(string commandText, string tableName);

        /// <summary>
        ///     Opens the connection to the databaSE.
        /// </summary>
        void Open();

        #endregion
    }
}