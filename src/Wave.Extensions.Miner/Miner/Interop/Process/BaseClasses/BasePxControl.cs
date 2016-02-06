using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Miner.ComCategories;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Base class for Process Framework node controls.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Px")]
    [ComVisible(true)]
    public abstract class BasePxControl : IMMPxControl, IMMPxControl2, IMMPxDisplayName
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxControl" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="control">The control.</param>
        /// <param name="displayName">The display name.</param>
        protected BasePxControl(string name, IPxControlUI control, string displayName)
        {
            this.Name = name;
            this.Control = control;
            this.DisplayName = displayName;
            this.Enabled = true;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the control.
        /// </summary>
        /// <value>
        ///     The control.
        /// </value>
        protected IPxControlUI Control { get; private set; }

        /// <summary>
        ///     Gets the process application reference.
        /// </summary>
        /// <value>
        ///     The process application reference.
        /// </value>
        protected IMMPxApplication PxApplication { get; private set; }

        #endregion

        #region IMMPxControl Members

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="BasePxControl" /> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets the handle to the control.
        /// </summary>
        /// <value>The handle to the control.</value>
        public int hWnd
        {
            get
            {
                if (this.Control == null) return 0;

                return this.Control.Handle;
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        ///     Sets the node.
        /// </summary>
        /// <value>The node.</value>
        public IMMPxNode Node
        {
            set
            {
                InitControlUI(value);
                SetLockIcon();
                //this.Control.LoadControl(this.PxApplication, value);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsInitialized
        {
            get { return (this.PxApplication != null); }
        }

        /// <summary>
        ///     Gets a value indicating whether the state of the node is completed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the state of the node is completed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool StateComplete { get; protected set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="BasePxControl" /> is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Visible
        {
            get
            {
                if (this.Control == null)
                    return false;

                return this.Control.Visible;
            }
            set
            {
                if (this.Control == null)
                    return;

                this.Control.Visible = value;
            }
        }

        /// <summary>
        ///     Initializes the control with the specified <paramref name="vInitData" />.
        /// </summary>
        /// <param name="vInitData">The initalization data.</param>
        public virtual void Initialize(object vInitData)
        {
            this.PxApplication = vInitData as IMMPxApplication;
        }

        #endregion

        #region IMMPxControl2 Members

        /// <summary>
        ///     Called when the control is losing focus within the application.
        /// </summary>
        /// <param name="bTerminate">if set to <c>true</c> the control can be closed; otherwise <c>false</c> to stop the closing.</param>
        public virtual void Terminate(ref bool bTerminate)
        {
            if (this.Control.PendingUpdates)
            {
                DialogResult result = MessageBox.Show(@"Do you want to apply the changes?",
                    this.DisplayName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    this.Control.ApplyUpdates();
            }
        }

        #endregion

        #region IMMPxDisplayName Members

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; private set; }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComRegisterFunction]
        internal static void Register(string registryKey)
        {
            MMProcessMgrControl.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MMProcessMgrControl.Unregister(registryKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the controls information.
        /// </summary>
        /// <param name="pxNode">The node.</param>
        /// <returns></returns>
        protected abstract Hashtable GetControlsInfo(IMMPxNode pxNode);

        /// <summary>
        ///     Sets the lock icon.
        /// </summary>
        protected abstract void SetLockIcon();

        #endregion

        #region Private Methods

        private void InitControlUI(IMMPxNode node)
        {
            Hashtable controlsInfo = GetControlsInfo(node);
            PopulateControlUI(Control as Control, controlsInfo);


            IPxControlUI pxCtrlUI = (IPxControlUI) Control;
            pxCtrlUI.LoadControl(PxApplication, node);
        }


        private void PopulateControlUI(Control parentCtrl, Hashtable controlsInfo)
        {
            if (parentCtrl == null || controlsInfo.Count == 0)
            {
                return;
            }

            foreach (Control aCtrl in parentCtrl.Controls)
            {
                if (aCtrl is TextBox && aCtrl.Tag != null)
                {
                    object aCtrlTag = aCtrl.Tag;

                    if (controlsInfo.ContainsKey(aCtrlTag) && controlsInfo[aCtrlTag] != null)
                    {
                        aCtrl.Text = controlsInfo[aCtrlTag].ToString();
                        controlsInfo.Remove(aCtrlTag);
                    }
                }
                else
                {
                    PopulateControlUI(aCtrl, controlsInfo);
                }
            }
        }

        #endregion
    }
}