using System.Runtime.InteropServices;

namespace System.Data
{
    /// <summary>
    ///     Provides access to dynamically add DSN-names to the system profile.
    /// </summary>
    /// <seealso cref="System.Data.ConfigDataSource" />
    public class SystemConfigDataSource : ConfigDataSource
    {
        #region Public Methods

        /// <summary>
        ///     Creates the data source.
        /// </summary>
        /// <param name="driver">
        ///     Driver description (usually the name of the
        ///     associated DBMS) presented to users instead of the physical driver name.
        /// </param>
        /// <param name="attributes">
        ///     List of attributes in the form of keyword-value
        ///     pairs.
        /// </param>
        /// <returns>
        ///     The function returns TRUE if it is successful, FALSE if it fails.
        ///     If no entry exists in the system information when this function is called,
        ///     the function returns FALSE.
        /// </returns>
        public override bool Create(string driver, string attributes)
        {
            string value = ConvertToDoublyNullTerminated(attributes);
            return SQLConfigDataSourceW(IntPtr.Zero, RequestType.ODBC_ADD_SYS_DSN, driver, value);
        }

        /// <summary>
        ///     Deletes the specified data source name.
        /// </summary>
        /// <param name="driver">
        ///     Driver description (usually the name of the
        ///     associated DBMS) presented to users instead of the physical driver name.
        /// </param>
        /// <param name="attributes">
        ///     List of attributes in the form of keyword-value
        ///     pairs.
        /// </param>
        /// <returns>
        ///     The function returns TRUE if it is successful, FALSE if it fails.
        ///     If no entry exists in the system information when this function is called,
        ///     the function returns FALSE.
        /// </returns>
        public override bool Delete(string driver, string attributes)
        {
            string value = ConvertToDoublyNullTerminated(attributes);
            return SQLConfigDataSourceW(IntPtr.Zero, RequestType.ODBC_REMOVE_SYS_DSN, driver, value);
        }

        #endregion
    }

    /// <summary>
    ///     Provides access to dynamically add DSN-names to the user profile.
    /// </summary>
    /// <seealso cref="System.Data.ConfigDataSource" />
    public class UserConfigDataSource : ConfigDataSource
    {
        #region Public Methods

        /// <summary>
        ///     Creates the data source.
        /// </summary>
        /// <param name="driver">
        ///     Driver description (usually the name of the
        ///     associated DBMS) presented to users instead of the physical driver name.
        /// </param>
        /// <param name="attributes">
        ///     List of attributes in the form of keyword-value
        ///     pairs.
        /// </param>
        /// <returns>
        ///     The function returns TRUE if it is successful, FALSE if it fails.
        ///     If no entry exists in the system information when this function is called,
        ///     the function returns FALSE.
        /// </returns>
        public override bool Create(string driver, string attributes)
        {
            string value = ConvertToDoublyNullTerminated(attributes);
            return SQLConfigDataSourceW(IntPtr.Zero, RequestType.ODBC_ADD_DSN, driver, value);
        }

        /// <summary>
        ///     Deletes the specified data source name.
        /// </summary>
        /// <param name="driver">
        ///     Driver description (usually the name of the
        ///     associated DBMS) presented to users instead of the physical driver name.
        /// </param>
        /// <param name="attributes">
        ///     List of attributes in the form of keyword-value
        ///     pairs.
        /// </param>
        /// <returns>
        ///     The function returns TRUE if it is successful, FALSE if it fails.
        ///     If no entry exists in the system information when this function is called,
        ///     the function returns FALSE.
        /// </returns>
        public override bool Delete(string driver, string attributes)
        {
            string value = ConvertToDoublyNullTerminated(attributes);
            return SQLConfigDataSourceW(IntPtr.Zero, RequestType.ODBC_REMOVE_DSN, driver, value);
        }

        #endregion
    }

    /// <summary>
    ///     Provides access to dynamically add DSN-names to the system.
    /// </summary>
    public abstract class ConfigDataSource
    {
        #region Enumerations

        /// <summary>
        ///     The requested flags.
        /// </summary>
        protected enum RequestType : ushort
        {
            /// <summary>
            ///     Add a new user data source.
            /// </summary>
            ODBC_ADD_DSN = 1,

            /// <summary>
            ///     Configure (modify) an existing user data source.
            /// </summary>
            ODBC_CONFIG_DSN = 2,

            /// <summary>
            ///     Remove an existing user data source.
            /// </summary>
            ODBC_REMOVE_DSN = 3,

            /// <summary>
            ///     The ODBC add system DSN
            /// </summary>
            ODBC_ADD_SYS_DSN = 4,

            /// <summary>
            ///     The ODBC configuration system DSN
            /// </summary>
            ODBC_CONFIG_SYS_DSN = 5,

            /// <summary>
            ///     The ODBC remove system DSN
            /// </summary>
            ODBC_REMOVE_SYS_DSN = 6,

            /// <summary>
            ///     The ODBC remove default DSN
            /// </summary>
            ODBC_REMOVE_DEFAULT_DSN = 7
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Converts to doubly null terminated.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="c">The character that is replaced with the null terminator.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> that is double null terminated.
        /// </returns>
        public static string ConvertToDoublyNullTerminated(string value, char c = ';')
        {
            return value.Replace(c, Convert.ToChar(0));
        }

        /// <summary>
        ///     Creates the data source.
        /// </summary>
        /// <param name="driver">
        ///     Driver description (usually the name of the
        ///     associated DBMS) presented to users instead of the physical driver name.
        /// </param>
        /// <param name="attributes">
        ///     List of attributes in the form of keyword-value
        ///     pairs.
        /// </param>
        /// <returns>
        ///     The function returns TRUE if it is successful, FALSE if it fails.
        ///     If no entry exists in the system information when this function is called,
        ///     the function returns FALSE.
        /// </returns>
        public abstract bool Create(string driver, string attributes);


        /// <summary>
        ///     Deletes the specified data source name.
        /// </summary>
        /// <param name="driver">
        ///     Driver description (usually the name of the
        ///     associated DBMS) presented to users instead of the physical driver name.
        /// </param>
        /// <param name="attributes">
        ///     List of attributes in the form of keyword-value
        ///     pairs.
        /// </param>
        /// <returns>
        ///     The function returns TRUE if it is successful, FALSE if it fails.
        ///     If no entry exists in the system information when this function is called,
        ///     the function returns FALSE.
        /// </returns>
        public abstract bool Delete(string driver, string attributes);

        #endregion

        #region Protected Methods

        /// <summary>
        ///     A method to dynamically add DSN-names to the system. This method also
        ///     aids with the creation, and subsequent manipulation, of Microsoft
        ///     Access database files.
        ///     <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/odbc/htm/odbcsqlconfigdatasource.asp" />
        /// </summary>
        /// <param name="hwndParent">
        ///     Parent window handle. The function will not display
        ///     any dialog boxes if the handle is null.
        /// </param>
        /// <param name="fRequest">
        ///     One of the OdbcConstant enum values to specify the
        ///     type of the request (RequestFlags.ODBC_ADD_DSN to create an MDB).
        /// </param>
        /// <param name="lpszDriver">
        ///     Driver description (usually the name of the
        ///     associated DBMS) presented to users instead of the physical driver name.
        /// </param>
        /// <param name="lpszAttributes">
        ///     List of attributes in the form of keyword-value
        ///     pairs. For more information, see
        ///     <see cref="http://msdn.microsoft.com/library/en-us/odbc/htm/odbcconfigdsn.asp">ConfigDSN</see>
        ///     in Chapter 22: Setup DLL Function Reference.
        /// </param>
        /// <returns>
        ///     The function returns TRUE if it is successful, FALSE if it fails.
        ///     If no entry exists in the system information when this function is called,
        ///     the function returns FALSE.
        /// </returns>
        [DllImport("ODBCCP32.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
        protected static extern bool SQLConfigDataSourceW(IntPtr hwndParent, RequestType fRequest, string lpszDriver, string lpszAttributes);

        #endregion
    }
}