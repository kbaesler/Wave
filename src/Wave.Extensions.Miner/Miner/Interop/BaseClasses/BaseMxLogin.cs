using System.Windows.Forms;

using ADODB;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop
{
    /// <summary>
    ///     An abstract class that can be used to customize the user interface of the ArcFM Login
    /// </summary>
    /// <typeparam name="TWindow">The type of the window.</typeparam>
    /// <remarks>
    ///     It uses the <see cref="Miner.Interop.MMDefaultLoginObjectClass" />
    ///     to handling logging into the ArcFM Desktop.
    /// </remarks>
    public abstract class BaseMxLogin<TWindow> : BaseLogin<TWindow>
        where TWindow : Form
    {
        #region Fields

        private readonly MMDefaultLoginObjectClass _LoginObject;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxLogin{TWindow}" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        protected BaseMxLogin(TWindow window)
            : base(window)
        {
            _LoginObject = new MMDefaultLoginObjectClass();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the connection.
        /// </summary>
        /// <value>
        ///     The connection.
        /// </value>
        public override Connection Connection
        {
            get { return _LoginObject.Connection; }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is valid login.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is valid login; otherwise, <c>false</c>.
        /// </value>
        public override bool IsValidLogin
        {
            get { return _LoginObject.IsValidLogin; }
        }

        /// <summary>
        ///     Gets the login workspace.
        /// </summary>
        /// <value>
        ///     The login workspace.
        /// </value>
        public override IWorkspace LoginWorkspace
        {
            get { return _LoginObject.LoginWorkspace; }
        }

        /// <summary>
        ///     Gets the name of the user.
        /// </summary>
        /// <value>
        ///     The name of the user.
        /// </value>
        public override string UserName
        {
            get { return _LoginObject.UserName; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Changes the default version.
        /// </summary>
        /// <param name="pVersion">The version.</param>
        public override void ChangeDefaultVersion(IVersion pVersion)
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
        public override string GetFullTableName(string bstrBaseTableName)
        {
            return _LoginObject.GetFullTableName(bstrBaseTableName);
        }

        #endregion
    }
}