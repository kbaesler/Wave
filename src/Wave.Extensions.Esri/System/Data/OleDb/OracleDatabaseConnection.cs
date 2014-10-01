using System.Data.Common;
using System.Data.OracleClient;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data
{
    /// <summary>
    ///     A supporting class for handling simple queries against an Oracle connection using the
    ///     <see cref="System.Data.OleDb" /> drivers.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class OracleDatabaseConnection : OleDbDatabaseConnection
    {
        #region Fields

        private string _CommandText;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OracleDatabaseConnection" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public OracleDatabaseConnection(string dataSource, string userName, string password)
            : base(string.Format(CultureInfo.InvariantCulture, "Provider=OraOLEDB.Oracle;Data Source={0};User Id={1};Password={2};OLEDB.NET=True;", dataSource, userName, password))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OracleDatabaseConnection" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public OracleDatabaseConnection(string dataSource)
            : base(string.Format(CultureInfo.InvariantCulture, "Provider=OraOLEDB.Oracle;Data Source={0};OSAuthent=1;OLEDB.NET=True;", dataSource))
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the <see cref="OracleDataAdapter" />.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     The <see cref="OracleDataAdapter" />.
        /// </returns>
        public OracleDataAdapter GetAdapter(string commandText)
        {
            _CommandText = commandText;
            return this.CreateAdapter() as OracleDataAdapter;
        }

        /// <summary>
        ///     Reads the contents of a LOB field.
        /// </summary>
        /// <param name="commandText">The command text (SELECT).</param>
        /// <param name="tableName">The name for the data table.</param>
        /// <param name="fieldName">The LOB field name.</param>
        /// <returns>
        ///     The value of the LOB field as a string.
        /// </returns>
        public string ReadLob(string commandText, string tableName, string fieldName)
        {
            string output = string.Empty;
            DataTable table = this.Fill(commandText, tableName);
            int columnIndex = table.Columns[fieldName].Ordinal;

            OracleDataReader reader = this.ExecuteReader(commandText) as OracleDataReader;
            if (reader == null) return output;

            reader.Read();

            if (reader.IsDBNull(columnIndex)) return output;

            OracleLob blob = reader.GetOracleLob(columnIndex);

            byte[] buffer = new byte[100];
            blob.Read(buffer, 0, buffer.Length);
            char[] chars = Encoding.Unicode.GetChars(buffer);
            output = new string(chars);
            return output;
        }

        /// <summary>
        ///     Writes the <paramref name="input" /> to the LOB field identified by the <paramref name="fieldName" />.
        /// </summary>
        /// <param name="commandText">The command text (SELECT).</param>
        /// <param name="tableName">The name for the data table.</param>
        /// <param name="fieldName">The LOB field name.</param>
        /// <param name="input">The string to write to the LOB field.</param>
        public void WriteLob(string commandText, string tableName, string fieldName, string input)
        {
            DbTransaction transaction = this.BeginTransaction();
            OracleDataReader reader = this.ExecuteReader(commandText) as OracleDataReader;
            DataTable table = this.Fill(commandText, tableName);
            int columnIndex = table.Columns[fieldName].Ordinal;
            if (reader == null) return;

            OracleLob lob = reader.GetOracleLob(columnIndex);
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] bytes = encoding.GetBytes(input);

            //update the OracleLob
            lob.BeginBatch();
            lob.Write(bytes, 0, bytes.Length);
            lob.EndBatch();

            //update DataTable
            table.Rows[0][fieldName] = lob.Value;

            OracleDataAdapter adapter = this.GetAdapter(commandText);
            adapter.SelectCommand.Transaction = transaction as OracleTransaction;

            //update Oracle table
            OracleCommandBuilder commandBuilder = new OracleCommandBuilder(adapter);
            adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            adapter.UpdateCommand.Transaction = transaction as OracleTransaction;
            adapter.Update(table);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the adapter that is used by the <see cref="System.Data.Common.DbConnection" />.
        /// </summary>
        /// <returns>
        ///     The <see cref="System.Data.Common.DbDataAdapter" /> for the specified connection.
        /// </returns>
        protected override DbDataAdapter CreateAdapter()
        {
            DbConnection dbConn = Connection;
            OracleConnection orcConn = dbConn as OracleConnection;

            return new OracleDataAdapter(_CommandText, orcConn);
        }

        #endregion
    }
}