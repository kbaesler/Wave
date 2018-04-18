using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ADODB;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Framework;

namespace Miner.Interop
{
    /// <summary>
    ///     An abstract class that can be used to customize the user interface of the ArcFM Login
    /// </summary>
    /// <remarks>
    ///     It uses the <see cref="Miner.Interop.MMFrameworkLoginObjectClass" />
    ///     to handling logging into the ArcFM Framework.
    /// </remarks>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseLogin : BaseLogin<Form>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseLogin" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        protected BaseLogin(Form window)
            : base(window)
        {
        }

        #endregion
    }

    /// <summary>
    ///     An abstract class that can be used to customize the user interface of the ArcFM Login
    /// </summary>
    /// <typeparam name="TWindow">The type of the window.</typeparam>
    /// <remarks>
    ///     It uses the <see cref="Miner.Interop.MMFrameworkLoginObjectClass" />
    ///     to handling logging into the ArcFM Framework.
    /// </remarks>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseLogin<TWindow> : IMMLoginObject, IMMChangeDefaultVersion, IMMAdoConnection
        where TWindow : Form
    {
        #region Constants

        /// <summary>
        ///     The property set name that the internal product uses to locate the process framework connection string.
        /// </summary>
        private const string PxConnectionString = "PXCONNECTIONSTRING";

        #endregion

        #region Fields

        private readonly MMFrameworkLoginObjectClass _LoginObject;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseLogin{TWindow}" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        protected BaseLogin(TWindow window)
        {
            _LoginObject = new MMFrameworkLoginObjectClass();
            Window = window;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the connection.
        /// </summary>
        /// <value>
        ///     The connection.
        /// </value>
        public virtual Connection Connection
        {
            get { return _LoginObject.Connection; }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is valid login.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is valid login; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsValidLogin
        {
            get { return _LoginObject.IsValidLogin; }
        }

        /// <summary>
        ///     Gets the login workspace.
        /// </summary>
        /// <value>
        ///     The login workspace.
        /// </value>
        public virtual IWorkspace LoginWorkspace
        {
            get { return _LoginObject.LoginWorkspace; }
        }

        /// <summary>
        ///     Gets the name of the user.
        /// </summary>
        /// <value>
        ///     The name of the user.
        /// </value>
        public virtual string UserName
        {
            get { return _LoginObject.UserName; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the connection properties for the user selected default connection.
        /// </summary>
        /// <returns>A property set of the connection properties for the default connection.</returns>
        protected abstract IPropertySet ConnectionProperties { get; }

        /// <summary>
        ///     Gets the window.
        /// </summary>
        /// <value>
        ///     The window.
        /// </value>
        protected TWindow Window { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Changes the default version.
        /// </summary>
        /// <param name="pVersion">The version.</param>
        public virtual void ChangeDefaultVersion(IVersion pVersion)
        {
            _LoginObject.ChangeDefaultVersion(pVersion);
        }

        /// <summary>
        ///     Gets the full name of the table.
        /// </summary>
        /// <param name="bstrBaseTableName">Name of the BSTR base table.</param>
        /// <returns>
        ///     The full name of the table name.
        /// </returns>
        public virtual string GetFullTableName(string bstrBaseTableName)
        {
            return _LoginObject.GetFullTableName(bstrBaseTableName);
        }

        /// <summary>
        ///     Attempts the log into the ArcMap session. This method is called within exception handling.
        /// </summary>
        /// <param name="vbInitialLogin">if set to <c>true</c> this is the initial login.</param>
        /// <returns>
        ///     <c>true</c> if the login is successful otherwise <c>false</c>
        /// </returns>
        /// <remarks>
        ///     The DialogResult will be used to determine the execution path.
        ///     OK - Attempt to log into the geodatabase and process framework.
        ///     Retry - Display the login form again.
        ///     Cancel - Closes the login form and terminates the ArcMap session.
        ///     All Others - Shows the default OOTB login.
        /// </remarks>
        public virtual bool Login(bool vbInitialLogin)
        {
            switch (Window.ShowDialog(Document.ParentWindow))
            {
                case DialogResult.OK:

                    // Set the database connection information
                    _LoginObject.SetConnectionProperties(this.ConnectionProperties);
                    _LoginObject.ShowDialog = false;

                    if (!_LoginObject.Login(vbInitialLogin))
                        return this.Login(vbInitialLogin);

                    // Validate the system tables are present.
                    if (!((IMMDefaultLoginObject2) _LoginObject).ValidateSystemTables(this.LoginWorkspace, Window.Text))
                        return this.Login(vbInitialLogin);

                    return true;

                case DialogResult.Retry:
                    return this.Login(vbInitialLogin);

                case DialogResult.Cancel:
                    return true;

                default:
                    _LoginObject.ShowDialog = true;
                    return _LoginObject.Login(vbInitialLogin);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Updates the connection string and sets the value in the PXCONNECTIONSTRING property.
        /// </summary>
        /// <param name="propertySet">The property set.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        ///     The updated property set.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">propertySet</exception>
        protected IPropertySet AddPxConnection(IPropertySet propertySet, string userName, string password)
        {
            if (propertySet == null) throw new ArgumentNullException("propertySet");

            string connectionString = propertySet.GetProperty(PxConnectionString, string.Empty);
            if (!string.IsNullOrEmpty(connectionString))
            {
                if (propertySet.GetProperty("AUTHENTICATION_MODE", "DBMS") == "OSA")
                    connectionString = string.Format(CultureInfo.InvariantCulture, "{0}User Id={1};Password={2}", connectionString, userName, password);
                else
                    connectionString = string.Format(CultureInfo.InvariantCulture, "{0}User Id={1};", connectionString, userName);

                propertySet.SetProperty(PxConnectionString, connectionString);
            }

            return propertySet;
        }

        #endregion
    }
}