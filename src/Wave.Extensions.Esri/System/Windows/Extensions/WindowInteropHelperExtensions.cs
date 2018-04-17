using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Threading;

using IWin32Window = System.Windows.Forms.IWin32Window;

namespace System.Windows
{
    /// <summary>
    ///     Assists interoperation between Windows Presentation Foundation (WPF) and Windows Forms (Win32) code.
    /// </summary>
    public static class WindowInteropHelperExtensions
    {
        #region Fields

        private static readonly Dictionary<string, Window> Windows = new Dictionary<string, Window>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="singleton">if set to <c>true</c> only once instance of the window will be shown.</param>
        public static void Show(this Window window, bool singleton)
        {
            window.Show(IntPtr.Zero, singleton);
        }

        /// <summary>
        ///     Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="singleton">if set to <c>true</c> only once instance of the window will be shown.</param>
        public static void Show(this Window window, IntPtr owner, bool singleton)
        {
            if (singleton)
            {
                if (!Windows.ContainsKey(window.GetType().Name))
                {
                    window.Show(owner);
                    window.Closed += (sender, args) =>
                    {
                        Windows.Remove(window.GetType().Name);
                    };

                    Windows.Add(window.GetType().Name, window);
                }
                else
                {
                    Windows[window.GetType().Name].Show(owner);
                }
            }
            else
            {
                window.Show(owner);
            }
        }

        /// <summary>
        ///     Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="owner">The owner.</param>
        public static void Show(this Window window, IntPtr owner)
        {
            // When launching the window, use ElementHost.EnableModelessKeyboardInterop(window1).
            // This is required because WPF and WinForms having two very different ways of handling text input.
            ElementHost.EnableModelessKeyboardInterop(window);

            // We need to set the parent window for the window but need to use interop because of the mixed winform and wpf combination.
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = owner;

            // Show the window using the dispatcher.
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) (window.Show));
        }

        /// <summary>
        ///     Opens a window and returns without waiting for the newly opened window to cloSE.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="owner">The owner.</param>
        public static void Show(this Window window, IWin32Window owner)
        {
            Show(window, owner.Handle, false);
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>
        ///     A System.Nullable value of type System.Boolean that signifies how a window was closed by the user.
        /// </returns>
        public static bool? ShowDialog(this Window window, IWin32Window owner)
        {
            return ShowDialog(window, owner.Handle);
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>
        ///     A System.Nullable value of type System.Boolean that signifies how a window was closed by the user.
        /// </returns>
        public static bool? ShowDialog(this Window window, IntPtr owner)
        {
            // We need to set the parent window for the window but need to use interop because of the mixed winform and wpf combination.
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = owner;

            return window.ShowDialog();
        }

        #endregion
    }
}