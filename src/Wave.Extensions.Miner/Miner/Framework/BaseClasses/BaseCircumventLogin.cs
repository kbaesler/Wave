using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

using ESRI.ArcGIS.esriSystem;

using Miner.Controls;
using Miner.Controls.XmlLogin;
using Miner.FrameworkUI;
using Miner.Interop;

namespace Miner.Framework
{
    /// <summary>
    ///     An abstract implementation of the ArcFM Viewer for Engine login that avoids displaying the login screen to the
    ///     users by allowing the caller to choose the database connection that will be used to log into the system.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseCircumventLogin : BaseLogin, IDisposable
    {
        #region Fields

        private XmlLoginObject _LoginObject;
        private MinerXMLLogin _XmlLogin;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseCircumventLogin" /> class.
        /// </summary>
        protected BaseCircumventLogin()
            : base(new Form {Text = @"ArcFM Viewer for Engine (Circumvent)", Visible = false})
        {
            this.Window.Load += Window_Load;

            _XmlLogin = new MinerXMLLogin();
            _LoginObject = new XmlLoginObject {Visible = false};
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the connection properties for the user selected default connection.
        /// </summary>
        /// <returns>A property set of the connection properties for the default connection.</returns>
        protected override IPropertySet ConnectionProperties
        {
            get
            {
                if (_LoginObject.Visible)
                {
                    return _LoginObject.LoginWorkspace.ConnectionProperties;
                }

                IPropertySet propset = _XmlLogin.ConnectionProperties();
                propset.SetProperty("USER", Environment.UserName);
                return propset;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Attempts to log into the system without requiring user interaction.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="Boolean" /> representing <c>true</c> if the login was completed; otherwise <c>false</c>
        ///     meaning that user interaction is required.
        /// </returns>
        protected virtual bool Circumvent()
        {
            IMMRegistry reg = new MMRegistryClass();
            reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmEngineViewer, "Login");

            string fileName = (string) reg.Read("DatabaseConnectionsXMLPath", string.Empty);
            if (_XmlLogin.Initialize(fileName))
            {
                Connections connections = XmlSerialization.Deserialize<Connections>(fileName);
                Connection connection = this.GetConnection(connections.Connection);
                if (connection != null)
                {
                    return _XmlLogin.SetConnection(connection.Name);
                }
            }

            return false;
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_XmlLogin != null)
                {
                    _XmlLogin.Dispose();
                    _XmlLogin = null;
                }

                if (_LoginObject != null)
                {
                    _LoginObject.Dispose();
                    _LoginObject = null;
                }

                if (this.Window != null)
                {
                    this.Window.Load -= Window_Load;
                }
            }
        }

        /// <summary>
        ///     Returns the connection that will be used to log into the system.
        /// </summary>
        /// <param name="connections">The list of connections.</param>
        /// <returns>
        ///     Returns a <see cref="Miner.Controls.XmlLogin.Connection" /> representing the connection; otherwise <c>null</c>
        ///     to abort the login.
        /// </returns>
        protected abstract Connection GetConnection(Connection[] connections);

        /// <summary>
        ///     Provides the ability to display or handle additional action when the login to the system fails because of invalid
        ///     connection information.
        /// </summary>
        /// <remarks>
        ///     By default this will present the user with the Product ArcFM Login window.
        /// </remarks>
        protected virtual void ShowFallback()
        {
            if (_LoginObject != null)
            {
                _LoginObject.Visible = false;
                _LoginObject.ShowDialog(Document.ParentWindow);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles the Load event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Window_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.Circumvent())
                {
                    this.ShowFallback();
                }
            }
            finally
            {
                this.Window.DialogResult = DialogResult.OK;
                this.Window.Close();
            }
        }

        #endregion
    }
}