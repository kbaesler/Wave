using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data
{
    /// <summary>
    ///     Provides extension methods for the <see cref="DataTable" />
    /// </summary>
    public static class DataTableExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Batches the specified table into groups of the specified amount.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> of the <see cref="DataRow" /> objects in the table in groups.
        /// </returns>
        public static IEnumerable<List<DataRow>> Batch(this DataTable source, int size)
        {
            return source.Batch(size, false);
        }

        /// <summary>
        ///     Batches the specified table into groups of the specified amount.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="size">The size.</param>
        /// <param name="reverse">The table is traversed in reverse order.</param>
        /// <returns>Returns a <see cref="IEnumerable{T}" /> of the <see cref="DataRow" /> objects in the table in groups.</returns>
        public static IEnumerable<List<DataRow>> Batch(this DataTable source, int size, bool reverse)
        {
            var partition = new List<DataRow>();

            if (reverse)
            {
                for (int i = source.Rows.Count - 1; i >= 0; i--)
                {
                    partition.Add(source.Rows[i]);

                    if (partition.Count == size)
                    {
                        yield return partition;
                        partition = new List<DataRow>();
                    }
                }
            }
            else
            {
                foreach (DataRow row in source.Rows)
                {
                    partition.Add(row);

                    if (partition.Count == size)
                    {
                        yield return partition;
                        partition = new List<DataRow>();
                    }
                }
            }

            if (partition.Count > 0) yield return partition;
        }

        /// <summary>
        ///     Selects all rows from both tables as long as there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="child">The child.</param>
        /// <param name="parentDataColumns">The parent data columns.</param>
        /// <param name="childDataColumns">The child data columns.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> representing the joined table.
        /// </returns>
        /// <remarks>
        ///     This JOIN method is equivalent to the TSQL INNER JOIN expression using equality.
        /// </remarks>
        public static DataTable Join(this DataTable source, DataTable child, DataColumn[] parentDataColumns, DataColumn[] childDataColumns)
        {
            DataTable table = new DataTable(source.TableName + "_" + child.TableName);

            using (DataSet ds = new DataSet())
            {
                ds.Tables.AddRange(new[] {source.Copy(), child.Copy()});

                DataColumn[] parentColumns = new DataColumn[parentDataColumns.Length];
                for (int i = 0; i < parentColumns.Length; i++)
                {
                    parentColumns[i] = ds.Tables[0].Columns[parentDataColumns[i].ColumnName];
                }

                DataColumn[] childColumns = new DataColumn[childDataColumns.Length];
                for (int i = 0; i < childColumns.Length; i++)
                {
                    childColumns[i] = ds.Tables[1].Columns[childDataColumns[i].ColumnName];
                }

                DataRelation r = new DataRelation(string.Empty, parentColumns, childColumns, false);
                ds.Relations.Add(r);

                for (int i = 0; i < source.Columns.Count; i++)
                {
                    table.Columns.Add(source.Columns[i].ColumnName, source.Columns[i].DataType);
                }

                var dataColumns = new List<DataColumn>();
                for (int i = 0; i < child.Columns.Count; i++)
                {
                    if (!table.Columns.Contains(child.Columns[i].ColumnName))
                        table.Columns.Add(child.Columns[i].ColumnName, child.Columns[i].DataType);
                    else
                        dataColumns.Add(child.Columns[i]);
                }               

                table.BeginLoadData();

                foreach (DataRow parentRow in ds.Tables[0].Rows)
                {
                    var parent = parentRow.ItemArray;
                    var childRows = parentRow.GetChildRows(r);
                    if (childRows.Any())
                    {
                        foreach (DataRow childRow in childRows)
                        {
                            var children = childRow.ItemArray.Where((value, i) => !dataColumns.Select(o => o.Ordinal).Contains(i)).ToArray();
                            var join = new object[parent.Length + children.Length];

                            Array.Copy(parent, 0, join, 0, parent.Length);
                            Array.Copy(children, 0, join, parent.Length, children.Length);

                            table.LoadDataRow(join, true);
                        }
                    }
                }

                table.EndLoadData();
            }

            return table;
        }

        /// <summary>
        ///     Selects all rows from both tables as long as there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="child">The child.</param>
        /// <param name="parentDataColumn">The parent data column.</param>
        /// <param name="childDataColumn">The child data column.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> representing the joined table.
        /// </returns>
        /// <remarks>
        ///     This JOIN method is equivalent to the TSQL INNER JOIN expression using equality.
        /// </remarks>
        public static DataTable Join(this DataTable source, DataTable child, DataColumn parentDataColumn, DataColumn childDataColumn)
        {
            return source.Join(child, new[] {parentDataColumn}, new[] {childDataColumn});
        }

        /// <summary>
        ///     Selects all rows from both tables as long as there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="child">The child.</param>
        /// <param name="parentColumnName">Name of the parent column.</param>
        /// <param name="childColumnName">Name of the child column.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> representing the joined table.
        /// </returns>
        /// <remarks>
        ///     This JOIN method is equivalent to the TSQL INNER JOIN expression using equality.
        /// </remarks>
        public static DataTable Join(this DataTable source, DataTable child, string parentColumnName, string childColumnName)
        {
            return source.Join(child, new[] {source.Columns[parentColumnName]}, new[] {child.Columns[childColumnName]});
        }

        /// <summary>
        ///     Reads the contents of the specified file into a <see cref="DataTable" />.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void ReadCsv(this DataTable table, string fileName)
        {
            string connectionString = string.Format(CultureInfo.InvariantCulture, @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Text"";",
                Path.GetDirectoryName(fileName));

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM " + Path.GetFileName(fileName);
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            table.TableName = Path.GetFileNameWithoutExtension(fileName);
                            table.Load(dr, LoadOption.PreserveChanges);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Writes the specified data to the file name using the <see cref="DataTable" />.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void WriteCsv(this DataTable table, string fileName)
        {
            // Create a new file using the stream writer.
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                string delimiter = String.Empty;

                // Add the column headers from the table.
                foreach (DataColumn column in table.Columns)
                {
                    sw.Write(delimiter);
                    sw.Write("\"");
                    sw.Write(column.ColumnName);
                    sw.Write("\"");
                    delimiter = ",";
                }

                // Add a new line.
                sw.Write(Environment.NewLine);

                // Iterate through all of the rows.
                foreach (DataRow row in table.Rows)
                {
                    delimiter = String.Empty;

                    // Add the data from the rows for each column.
                    foreach (object o in row.ItemArray)
                    {
                        sw.Write(delimiter);

                        string s = o.ToString();

                        // Escape any quotes.
                        if (s.Contains("\""))
                            s = s.Replace("\"", "\"\"");

                        // Quote those characters that need to be quoted.
                        if (s.IndexOfAny(new[] {',', '"', '\n'}) > -1)
                            s = "\"" + s + "\"";

                        sw.Write(s);

                        delimiter = ",";
                    }

                    // Add a new line.
                    sw.Write(Environment.NewLine);
                }
            }
        }

        /// <summary>
        ///     Writes the contents of the data set to a Microsoft Excel Spreadsheet (XLS) file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="excelFileName">Name of the excel file.</param>
        public static void WriteXls(this DataSet source, string excelFileName)
        {
            if (excelFileName == null)
                throw new ArgumentNullException("excelFileName");

            var connectionString = GetConnectionString(excelFileName);
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                connection.TruncateXls();

                foreach (DataTable table in source.Tables)
                {
                    table.AddXls(connection);
                }
            }
        }

        /// <summary>
        ///     Writes the contents of the table to a Microsoft Excel Spreadsheet (XLS) file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="excelFileName">Name of the excel file.</param>
        /// <exception cref="ArgumentNullException">excelFileName</exception>
        public static void WriteXls(this DataTable source, string excelFileName)
        {
            if (excelFileName == null)
                throw new ArgumentNullException("excelFileName");

            var connectionString = GetConnectionString(excelFileName);
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                connection.TruncateXls();

                source.AddXls(connection);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds a row to the XLS table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="connection">The connection.</param>
        /// <exception cref="ArgumentNullException">connection</exception>
        private static void AddXls(this DataRow source, OleDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            var commandText = new StringBuilder();
            var columns = source.Table.Columns.Cast<DataColumn>().ToList();
            var names = String.Join(",", columns.Select(x => x.ColumnName).ToArray());
            var values = String.Join(",", columns.Select(x => "?").ToArray());

            commandText.AppendFormat("INSERT INTO [{0}] (", source.Table.TableName);
            commandText.Append(names);
            commandText.Append(") VALUES(");
            commandText.Append(values);
            commandText.Append(")");

            using (OleDbCommand command = new OleDbCommand(commandText.ToString(), connection))
            {
                var paramNumber = 1;

                for (int i = 0; i <= source.Table.Columns.Count - 1; i++)
                {
                    object value = source[i];

                    if (value is string)
                    {
                        value = value.ToString().Replace("'", "''");
                    }

                    command.Parameters.AddWithValue("@p" + paramNumber, value);
                    paramNumber = paramNumber + 1;
                }

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a new XLS table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="connection">The connection.</param>
        /// <exception cref="ArgumentNullException">source;The TableName must have a value.</exception>
        private static void AddXls(this DataTable source, OleDbConnection connection)
        {           
            if(string.IsNullOrEmpty(source.TableName))
                throw new ArgumentNullException("source", "The TableName must have a value.");

            StringBuilder commandText = new StringBuilder();
            string comma = null;

            commandText.AppendFormat("CREATE TABLE [{0}] (", source.TableName);

            foreach (DataColumn column in source.Columns)
            {
                commandText.Append(comma);
                commandText.AppendFormat("{0} CHAR(255)", column.ColumnName);
                comma = ",";
            }

            commandText.Append(")");

            using (OleDbCommand cmd = new OleDbCommand(commandText.ToString(), connection))
                cmd.ExecuteNonQuery();

            foreach (DataRow row in source.Rows)
                row.AddXls(connection);
        }

        /// <summary>
        ///     Gets the connection string based on the provider that has been installed.
        /// </summary>
        /// <param name="excelFileName">Name of the excel file.</param>
        /// <returns>Returns a <see cref="string" /> representing the connection string.</returns>
        private static string GetConnectionString(string excelFileName)
        {
            OleDbEnumerator enumerator = new OleDbEnumerator();
            DataTable table = enumerator.GetElements();

            bool name = false;
            bool clsid = false;

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if ((col.ColumnName.Contains("SOURCES_NAME")) && (row[col].ToString().Contains("Microsoft.ACE.OLEDB.12.0")))
                        name = true;
                    if ((col.ColumnName.Contains("SOURCES_CLSID")) && (row[col].ToString().Contains("{3BE786A0-0366-4F5C-9434-25CF162E475E}")))
                        clsid = true;
                }
            }

            if (name && clsid)
            {
                return string.Format(CultureInfo.InvariantCulture, "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=YES\";", excelFileName);
            }

            return string.Format(CultureInfo.InvariantCulture, @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Text"";", Path.GetDirectoryName(excelFileName));
        }

        /// <summary>
        ///     Truncates all of the XLS tables.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void TruncateXls(this OleDbConnection connection)
        {
            var tables = connection.GetSchema("Tables", new[] {null, null, null, "TABLE"});
            foreach (DataRow row in tables.Rows)
            {
                var commandText = String.Format("DROP TABLE [{0}]", row[2]);

                using (OleDbCommand command = new OleDbCommand(commandText, connection))
                    command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}