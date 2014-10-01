using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Data
{
    /// <summary>
    ///     A supporting class for handling simple queries against an Access connection using the
    ///     <see cref="System.Data.OleDb" /> drivers.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AccessDatabaseConnection : OleDbDatabaseConnection
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccessDatabaseConnection" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public AccessDatabaseConnection(string fileName)
            : base(string.Format(CultureInfo.InvariantCulture, "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}", fileName))
        {
        }

        #endregion
    }
}