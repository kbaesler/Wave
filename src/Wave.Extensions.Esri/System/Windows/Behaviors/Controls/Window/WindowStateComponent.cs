using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

using Microsoft.Win32;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Provides access to members that save and restore the state of a window.
    /// </summary>
    public interface IWindowStateComponent
    {
        #region Public Methods

        /// <summary>
        ///     Loads the state of the window.
        /// </summary>
        void Restore(Window window);

        /// <summary>
        ///     Saves the state of the window.
        /// </summary>
        void Save(Window window);

        #endregion
    }

    /// <summary>
    ///     Provides an attached behavior that will save and restore the window state to the registry for the current user.
    /// </summary>
    public class WindowStateComponent
    {
        #region Fields

        private readonly string _Name;
        private readonly string _RegistryKey;

        private readonly Window _Window;


        /// <summary>
        ///     The registry key property
        /// </summary>
        public static readonly DependencyProperty RegistryKeyProperty
            = DependencyProperty.RegisterAttached("RegistryKey", typeof(string), typeof(WindowStateComponent),
                new FrameworkPropertyMetadata(OnRegistryKeyInvalidated));

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowStateComponent" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="registryKey">The registry key.</param>
        public WindowStateComponent(Window window, string registryKey)
            : this(window, registryKey, window.GetType().Name)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowStateComponent" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="registryKey">The registry key.</param>
        /// <param name="name">The name.</param>
        public WindowStateComponent(Window window, string registryKey, string name)
        {
            _Window = window;
            _RegistryKey = registryKey;
            _Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is tracked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tracked; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get { return this.GetRegistryKey(false) != null; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the registry key.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static string GetRegistryKey(DependencyObject d)
        {
            return (string)d.GetValue(RegistryKeyProperty);
        }


        /// <summary>
        ///     Loads the info from registry.
        /// </summary>
        public virtual void Restore()
        {
            try
            {
                Action action = () =>
                {
                    using (var key = GetRegistryKey(false))
                    {
                        if (key != null)
                        {
                            int left = (int)key.GetValue("Left", SystemParameters.PrimaryScreenWidth / 2 - _Window.Width / 2);
                            int top = (int)key.GetValue("Top", SystemParameters.PrimaryScreenHeight / 2 - _Window.Height / 2);
                            int width = (int)key.GetValue("Width", SystemParameters.PrimaryScreenWidth);
                            int height = (int)key.GetValue("Height", SystemParameters.PrimaryScreenHeight);

                            Rectangle rect = new Rectangle(left, top, width, height);
                            if (this.IsVisibleWithinAnyScreen(rect))
                            {
                                _Window.Left = left;
                                _Window.Top = top;
                                _Window.Height = height;
                                _Window.Width = width;
                            }

                            _Window.WindowState = (WindowState)key.GetValue("WindowState", (int)_Window.WindowState);

                            this.SizeToFit();
                            this.MoveIntoView();
                        }
                    }
                };

                _Window.Dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
            catch (Exception e)
            {
                Log.Error(this, e);
            }
        }

        /// <summary>
        ///     Saves the state of the window.
        /// </summary>
        public virtual void Save()
        {
            try
            {
                using (var key = GetRegistryKey(true))
                {
                    if (key != null)
                    {
                        key.SetValue("Left", _Window.RestoreBounds.Left, RegistryValueKind.DWord);
                        key.SetValue("Top", _Window.RestoreBounds.Top, RegistryValueKind.DWord);
                        key.SetValue("Width", _Window.RestoreBounds.Width, RegistryValueKind.DWord);
                        key.SetValue("Height", _Window.RestoreBounds.Height, RegistryValueKind.DWord);
                        key.SetValue("WindowState", (int)_Window.WindowState, RegistryValueKind.DWord);
                        key.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(this, e);
            }
        }


        /// <summary>
        ///     Sets the registry key.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetRegistryKey(DependencyObject d, string value)
        {
            d.SetValue(RegistryKeyProperty, value);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the registry key.
        /// </summary>
        /// <param name="writable">if set to <c>true</c> [writable].</param>
        /// <returns></returns>
        protected virtual RegistryKey GetRegistryKey(bool writable)
        {
            if (string.IsNullOrEmpty(_RegistryKey) || _Window == null)
                return null;

            if (!writable)
            {
                return Registry.CurrentUser.OpenSubKey(string.Format("{0}\\Windows\\{1}", _RegistryKey, _Name));
            }

            return Registry.CurrentUser.CreateSubKey(string.Format("{0}\\Windows\\{1}", _RegistryKey, _Name));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Attaches this instance.
        /// </summary>
        private void Attach()
        {
            _Window.Closing += Window_Closing;
            _Window.Initialized += Window_Initialized;
        }

        /// <summary>
        ///     Detaches this instance.
        /// </summary>
        private void Detach()
        {
            _Window.Closing -= Window_Closing;
            _Window.Initialized -= Window_Initialized;
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
        ///     Moves the window onto the desktop if it is more than half out of view
        /// </summary>
        private void MoveIntoView()
        {
            if (_Window.Top + _Window.Height / 2 > SystemParameters.VirtualScreenHeight)
            {
                _Window.Top = SystemParameters.VirtualScreenHeight - _Window.Height;
            }

            if (_Window.Left + _Window.Width / 2 > SystemParameters.VirtualScreenWidth)
            {
                _Window.Left = SystemParameters.VirtualScreenWidth - _Window.Width;
            }

            if (_Window.Top < 0)
            {
                _Window.Top = 0;
            }

            if (_Window.Left < 0)
            {
                _Window.Left = 0;
            }
        }

        /// <summary>
        ///     Called when the registry key dependency property changes.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnRegistryKeyInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window != null)
            {
                string value = (string)e.NewValue;
                if (!string.IsNullOrEmpty(value))
                {
                    WindowStateComponent component = new WindowStateComponent(window, value);
                    component.Attach();
                }
            }
        }

        /// <summary>
        ///     Sizes to fit.
        /// </summary>
        private void SizeToFit()
        {
            if (_Window.SizeToContent == SizeToContent.Manual)
            {
                if (_Window.Height > SystemParameters.VirtualScreenHeight)
                {
                    _Window.Height = SystemParameters.VirtualScreenHeight;
                }

                if (_Window.Width > SystemParameters.VirtualScreenWidth)
                {
                    _Window.Width = SystemParameters.VirtualScreenWidth;
                }
            }
        }

        /// <summary>
        ///     Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Save();
        }

        /// <summary>
        ///     Handles the Initialized event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            Restore();
        }

        #endregion
    }
}