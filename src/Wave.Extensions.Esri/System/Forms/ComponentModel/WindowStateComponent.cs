using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

namespace System.Forms.ComponentModel
{
    /// <summary>
    ///     A component that can be used to save the window state to the registry or xml file.
    /// </summary>
    public partial class WindowStateComponent : Component
    {
        #region Enumerations

        /// <summary>
        ///     An enumeration of the avaiable presist methods.
        /// </summary>
        public enum PersistWindowState
        {
            /// <summary>
            ///     Persists the window position to the registry.
            /// </summary>
            Registry,

            /// <summary>
            ///     Persists the window position using a custom implementation.
            /// </summary>
            Custom
        }

        #endregion

        #region Fields

        private Form _Parent;
        private string _RegistryPath = "";
        private WindowStateInfo _WindowInfo = new WindowStateInfo();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowStateComponent" /> class.
        /// </summary>
        public WindowStateComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowStateComponent" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WindowStateComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when the window state is loaded.
        /// </summary>
        public event EventHandler<WindowStateInfoEventArgs> Load;

        /// <summary>
        ///     Occurs when the window state is saved.
        /// </summary>
        public event EventHandler<WindowStateInfoEventArgs> Save;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the form.
        /// </summary>
        /// <value>
        ///     The form.
        /// </value>
        [Browsable(false)]
        public Form Form
        {
            get
            {
                if (_Parent == null)
                {
                    if (this.Site.DesignMode)
                    {
                        IDesignerHost dh = (IDesignerHost) this.GetService(typeof (IDesignerHost));

                        if (dh != null)
                        {
                            Object obj = dh.RootComponent;
                            if (obj != null)
                            {
                                _Parent = (Form) obj;
                            }
                        }
                    }
                }

                return _Parent;
            }

            set
            {
                if (_Parent != null)
                    return;

                if (value != null)
                {
                    _Parent = value;

                    // subscribe to parent form's events
                    _Parent.Closing += OnClosing;
                    _Parent.Resize += OnResize;
                    _Parent.Move += OnMove;
                    _Parent.Load += OnLoad;

                    // get initial width and height in case form is never resized
                    _WindowInfo.Width = _Parent.Width;
                    _WindowInfo.Height = _Parent.Height;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the persist method.
        /// </summary>
        /// <value>
        ///     The persist method.
        /// </value>
        [DefaultValue(PersistWindowState.Registry)]
        [Description("Method of persisting window state information."),
         Category("Persist Configuration")]
        public PersistWindowState PersistMethod { get; set; }

        /// <summary>
        ///     Gets or sets the registry path for the current user.
        /// </summary>
        /// <value>
        ///     The registry path.
        /// </value>
        [DefaultValue("")]
        [Description("The path to the current user registry."),
         Category("Persist Configuration")]
        public string RegistryPath
        {
            get { return _RegistryPath; }
            set
            {
                _RegistryPath = value;

                this.LoadInfoFromRegistry();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Sets the window info.
        /// </summary>
        /// <param name="windowInfo">The window info.</param>
        public void SetWindowInfo(WindowStateInfo windowInfo)
        {
            _WindowInfo = windowInfo;

            _Parent.Location = new Point(_WindowInfo.Left, _WindowInfo.Top);
            _Parent.Size = new Size(_WindowInfo.Width, _WindowInfo.Height);
            _Parent.WindowState = _WindowInfo.WindowState;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Raises the <see cref="Load" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Forms.ComponentModel.WindowStateInfoEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        protected virtual void OnLoad(WindowStateInfoEventArgs e)
        {
            EventHandler<WindowStateInfoEventArgs> eventHandler = this.Load;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="Save" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Forms.ComponentModel.WindowStateInfoEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        protected virtual void OnSave(WindowStateInfoEventArgs e)
        {
            EventHandler<WindowStateInfoEventArgs> eventHandler = this.Save;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Determines whether the specified border style is fixed.
        /// </summary>
        /// <param name="borderStyle">The border style.</param>
        /// <returns>
        ///     <c>true</c> if the specified border style is fixed; otherwise, <c>false</c>.
        /// </returns>
        private bool IsFormBorderStyleFixed(FormBorderStyle borderStyle)
        {
            return (borderStyle == FormBorderStyle.Fixed3D || borderStyle == FormBorderStyle.FixedDialog || borderStyle == FormBorderStyle.FixedSingle || borderStyle == FormBorderStyle.FixedToolWindow);
        }

        /// <summary>
        ///     Determines whether the <paramref name="rectangle" /> is visible on any of the available screens.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the rectangle is visible on any of the available screens; otherwise, <c>false</c>.
        /// </returns>
        private bool IsVisibleWithinAnyScreen(Rectangle rectangle)
        {
            Screen[] screens = Screen.AllScreens;
            foreach (Screen screen in screens)
            {
                if (screen.WorkingArea.Contains(rectangle))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Loads the info from registry.
        /// </summary>
        private void LoadInfoFromRegistry()
        {
            if (string.IsNullOrEmpty(_RegistryPath) || _Parent == null)
                return;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(_RegistryPath))
            {
                if (key != null)
                {
                    string prefix = _Parent.GetType().Namespace + "." + _Parent.GetType().Name;
                    int left = (int) key.GetValue(prefix + "_Left", _Parent.Left);
                    int top = (int) key.GetValue(prefix + "_Top", _Parent.Top);
                    int width = (int) key.GetValue(prefix + "_Width", _Parent.Width);
                    int height = (int) key.GetValue(prefix + "_Height", _Parent.Height);

                    FormWindowState windowState = (FormWindowState) key.GetValue(prefix + "_WindowState", (int) _Parent.WindowState);
                    FormBorderStyle borderStyle = (FormBorderStyle) key.GetValue(prefix + "_BorderStyle", (int) _Parent.FormBorderStyle);

                    Rectangle rectangle = new Rectangle(left, top, width, height);
                    if (this.IsVisibleWithinAnyScreen(rectangle))
                    {
                        if (!this.IsFormBorderStyleFixed(borderStyle))
                            _Parent.Size = new Size(width, height);

                        _Parent.Location = new Point(left, top);
                        _Parent.WindowState = windowState;

                        _WindowInfo.Left = _Parent.Left;
                        _WindowInfo.Top = _Parent.Top;
                        _WindowInfo.Height = _Parent.Height;
                        _WindowInfo.Width = _Parent.Width;
                        _WindowInfo.WindowState = _Parent.WindowState;
                        _WindowInfo.BorderStyle = _Parent.FormBorderStyle;
                    }
                }
            }
        }

        /// <summary>
        ///     Raised when the parent window is being closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void OnClosing(object sender, CancelEventArgs e)
        {
            // Check if we are allowed to save the state as minimized (not normally)
            if (!_WindowInfo.AllowSaveMinimized)
            {
                if (_WindowInfo.WindowState == FormWindowState.Minimized)
                    _WindowInfo.WindowState = FormWindowState.Normal;
            }

            switch (this.PersistMethod)
            {
                case PersistWindowState.Registry:

                    this.SaveInfoToRegistry();

                    break;

                case PersistWindowState.Custom:

                    this.OnSave(new WindowStateInfoEventArgs(_WindowInfo));

                    break;
            }
        }

        /// <summary>
        ///     Raised when the parent window is being loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnLoad(object sender, EventArgs e)
        {
            switch (this.PersistMethod)
            {
                case PersistWindowState.Registry:

                    this.LoadInfoFromRegistry();

                    break;

                case PersistWindowState.Custom:

                    this.OnLoad(new WindowStateInfoEventArgs(_WindowInfo));

                    break;
            }
        }

        /// <summary>
        ///     Raised when the parent window is being moved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnMove(object sender, EventArgs e)
        {
            // Save position
            if (_Parent.WindowState == FormWindowState.Normal)
            {
                _WindowInfo.Left = _Parent.Left;
                _WindowInfo.Top = _Parent.Top;
            }

            // Save state
            _WindowInfo.WindowState = _Parent.WindowState;
        }

        /// <summary>
        ///     Raised when the parent window is being resized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnResize(object sender, EventArgs e)
        {
            // Save width and height
            if (_Parent.WindowState == FormWindowState.Normal)
            {
                _WindowInfo.Width = _Parent.Width;
                _WindowInfo.Height = _Parent.Height;
            }
        }

        /// <summary>
        ///     Saves the info to registry.
        /// </summary>
        private void SaveInfoToRegistry()
        {
            if (string.IsNullOrEmpty(_RegistryPath) || _Parent == null)
                return;

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(_RegistryPath))
            {
                string prefix = _Parent.GetType().Namespace + "." + _Parent.GetType().Name;

                if (key != null)
                {
                    key.SetValue(prefix + "_Left", _WindowInfo.Left);
                    key.SetValue(prefix + "_Top", _WindowInfo.Top);
                    key.SetValue(prefix + "_Width", _WindowInfo.Width);
                    key.SetValue(prefix + "_Height", _WindowInfo.Height);
                    key.SetValue(prefix + "_WindowState", (int) _WindowInfo.WindowState);
                    key.SetValue(prefix + "_BorderStyle", (int) _WindowInfo.BorderStyle);
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     A structure to hold the window state info.
    /// </summary>
    public class WindowStateInfo
    {
        #region Public Properties

        /// <summary>
        ///     Determines whether the window information can be saved if it's been minimized.
        /// </summary>
        public bool AllowSaveMinimized { get; set; }

        /// <summary>
        ///     The border style of the window.
        /// </summary>
        public FormBorderStyle BorderStyle { get; set; }

        /// <summary>
        ///     The height of the window.
        /// </summary>
        public Int32 Height { get; set; }

        /// <summary>
        ///     The left window position.
        /// </summary>
        public Int32 Left { get; set; }

        /// <summary>
        ///     The top window position.
        /// </summary>
        public Int32 Top { get; set; }

        /// <summary>
        ///     The width of the window.
        /// </summary>
        public Int32 Width { get; set; }

        /// <summary>
        ///     The window state.
        /// </summary>
        public FormWindowState WindowState { get; set; }

        #endregion
    }

    /// <summary>
    ///     The event instance data for the window state.
    /// </summary>
    public class WindowStateInfoEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowStateInfoEventArgs" /> class.
        /// </summary>
        /// <param name="windowInfo">The window info.</param>
        public WindowStateInfoEventArgs(WindowStateInfo windowInfo)
        {
            this.State = windowInfo;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the state of the window.
        /// </summary>
        public WindowStateInfo State { get; private set; }

        #endregion
    }
}