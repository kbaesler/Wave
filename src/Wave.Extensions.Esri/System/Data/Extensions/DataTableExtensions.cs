using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Globalization;
using System.IO;

namespace System.Data
{
    /// <summary>
    ///     A static class that reads and writes comma separated value files using a <see cref="DataTable" /> as the primary
    ///     facilitator of the data.
    /// </summary>
    public static class DataTableExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Batches the specified table into groups of the specified amount.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="size">The size.</param>
        /// <param name="reverse">The table is traversed in reverse order.</param>
        /// <returns>Returns a <see cref="IEnumerable{T}" /> of the <see cref="DataRow" /> objects in the table in groups.</returns>
        public static IEnumerable<List<DataRow>> Batch(this DataTable source, int size, bool reverse = false)
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
                string delimiter = string.Empty;

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
                    delimiter = string.Empty;

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

        #endregion
    }
}