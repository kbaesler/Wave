using System.Data.Common;
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
        ///     Reads the contents of the specified file into a <see cref="DataTable" />.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void ReadCsv(this DataTable table, string fileName)
        {
            string connectionString = string.Format(CultureInfo.InvariantCulture, @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Text"";",
                Path.GetDirectoryName(fileName));

            using (OleDbDatabaseConnection cn = new OleDbDatabaseConnection(connectionString))
            {
                using (DbDataReader dr = cn.ExecuteReader(@"SELECT * FROM " + Path.GetFileName(fileName)))
                {
                    table.TableName = Path.GetFileNameWithoutExtension(fileName);
                    table.Load(dr, LoadOption.PreserveChanges);
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