using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;

namespace System.Native
{
    /// <summary>
    ///     Assists with P/Invoke API calls between Windows Presentation Foundation (WPF) and Win32 code.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static class UnsafeWindowMethods
    {
        #region Enumerations

        /// <summary>
        ///     The zero-based offset to the value to be retrieved. Valid values are in the range zero through the number of bytes
        ///     of extra window memory, minus the size of an integer.
        ///     To retrieve any other value, specify one of the following values.
        /// </summary>
        private enum WindowIndex
        {
            /// <summary>
            ///     Retrieves the extended window styles.
            /// </summary>
            ExtendedWindowStyles = -20,

            /// <summary>
            ///     Retrieves a handle to the application instance.
            /// </summary>
            InstanceHandle = -6,

            /// <summary>
            ///     Retrieves a handle to the parent window, if there is one.
            /// </summary>
            InstanceHandleParent = -8,

            /// <summary>
            ///     Retrieves the identifier of the window.
            /// </summary>
            WindowID = -12,

            /// <summary>
            ///     Retrieves the window styles.
            /// </summary>
            WindowStyles = -16,

            /// <summary>
            ///     Retrieves the user data associated with the window. This data is intended for use by the application that created
            ///     the window. Its value is initially zero.
            /// </summary>
            UserData = -21,

            /// <summary>
            ///     Retrieves the pointer to the window procedure, or a handle representing the pointer to the window procedure. You
            ///     must use the CallWindowProc function to call the window procedure.
            /// </summary>
            WindowProc = -4,
        }

        /// <summary>
        ///     Enumeration of the different ways of showing a window using
        ///     ShowWindow
        /// </summary>
        public enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,

            /// <summary>
            ///     Activates and displays a window. If the window is minimized
            ///     or maximized, the system restores it to its original size and
            ///     position. An application should specify this flag when displaying
            ///     the window for the first time.
            /// </summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,

            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,

            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,

            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,

            /// <summary>
            ///     Displays a window in its most recent size and position.
            ///     This value is similar to "ShowNormal", except the window is not
            ///     activated.
            /// </summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,

            /// <summary>
            ///     Activates the window and displays it in its current size
            ///     and position.
            /// </summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,

            /// <summary>
            ///     Minimizes the specified window and activates the next
            ///     top-level window in the Z order.
            /// </summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,

            /// <summary>
            ///     Displays the window as a minimized window. This value is
            ///     similar to "ShowMinimized", except the window is not activated.
            /// </summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,

            /// <summary>
            ///     Displays the window in its current size and position. This
            ///     value is similar to "Show", except the window is not activated.
            /// </summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,

            /// <summary>
            ///     Activates and displays the window. If the window is
            ///     minimized or maximized, the system restores it to its original size
            ///     and position. An application should specify this flag when restoring
            ///     a minimized window.
            /// </summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,

            /// <summary>
            ///     Sets the show state based on the SW_ value specified in the
            ///     STARTUPINFO structure passed to the CreateProcess function by the
            ///     program that started the application.
            /// </summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,

            /// <summary>
            ///     Windows 2000/XP: Minimizes a window, even if the thread
            ///     that owns the window is hung. This flag should only be used when
            ///     minimizing windows from a different thread.
            /// </summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        /// <summary>
        ///     The following are the window styles. After the window has been created, these styles cannot be modified, except as
        ///     noted.
        /// </summary>
        private enum WindowStyles
        {
            /// <summary>
            ///     The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must
            ///     also be specified.
            /// </summary>
            MaximizeBox = 0x00010000,

            /// <summary>
            ///     The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must
            ///     also be specified.
            /// </summary>
            MinimizeBox = 0x00020000,

            /// <summary>
            ///     The window has a window menu on its title bar. The WS_CAPTION style must also be specified.
            /// </summary>
            WindowMenu = 0x00080000
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Sets the maximize box visibility.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="visibility">The visibility.</param>
        public static void SetMaximizeBoxVisibility(Window window, Visibility visibility)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            Int32 windowStyle = GetWindowLongPtr(hWnd, WindowIndex.WindowStyles);

            if (visibility == Visibility.Visible)
            {
                SetWindowLongPtr(hWnd, WindowIndex.WindowStyles, windowStyle | (int) WindowStyles.MaximizeBox);
            }
            else
            {
                SetWindowLongPtr(hWnd, WindowIndex.WindowStyles, windowStyle & ~(int) WindowStyles.MaximizeBox);
            }
        }

        /// <summary>
        ///     Sets the minimize box visibility.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="visibility">The visibility.</param>
        public static void SetMinimizeBoxVisibility(Window window, Visibility visibility)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            Int32 windowStyle = GetWindowLongPtr(hWnd, WindowIndex.WindowStyles);

            if (visibility == Visibility.Visible)
            {
                SetWindowLongPtr(hWnd, WindowIndex.WindowStyles, windowStyle | (int) WindowStyles.MinimizeBox);
            }
            else
            {
                SetWindowLongPtr(hWnd, WindowIndex.WindowStyles, windowStyle & ~(int) WindowStyles.MinimizeBox);
            }
        }

        /// <summary>
        ///     Sets the window menu visibility.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="visibility">The visibility.</param>
        public static void SetWindowMenuVisibility(Window window, Visibility visibility)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            Int32 windowStyle = GetWindowLongPtr(hWnd, WindowIndex.WindowStyles);

            if (visibility == Visibility.Visible)
            {
                SetWindowLongPtr(hWnd, WindowIndex.WindowStyles, windowStyle | (int) WindowStyles.WindowMenu);
            }
            else
            {
                SetWindowLongPtr(hWnd, WindowIndex.WindowStyles, windowStyle & ~(int) WindowStyles.WindowMenu);
            }
        }

        /// <summary>Shows a Window</summary>
        /// <remarks>
        ///     <para>
        ///         To perform certain special effects when showing or hiding a
        ///         window, use AnimateWindow.
        ///     </para>
        ///     <para>
        ///         The first time an application calls ShowWindow, it should use
        ///         the WinMain function's nCmdShow parameter as its nCmdShow parameter.
        ///         Subsequent calls to ShowWindow must use one of the values in the
        ///         given list, instead of the one specified by the WinMain function's
        ///         nCmdShow parameter.
        ///     </para>
        ///     <para>
        ///         As noted in the discussion of the nCmdShow parameter, the
        ///         nCmdShow value is ignored in the first call to ShowWindow if the
        ///         program that launched the application specifies startup information
        ///         in the structure. In this case, ShowWindow uses the information
        ///         specified in the STARTUPINFO structure to show the window. On
        ///         subsequent calls, the application must call ShowWindow with nCmdShow
        ///         set to SW_SHOWDEFAULT to use the startup information provided by the
        ///         program that launched the application. This behavior is designed for
        ///         the following situations:
        ///     </para>
        ///     <list type="">
        ///         <item>
        ///             Applications create their main window by calling CreateWindow
        ///             with the WS_VISIBLE flag set.
        ///         </item>
        ///         <item>
        ///             Applications create their main window by calling CreateWindow
        ///             with the WS_VISIBLE flag cleared, and later call ShowWindow with the
        ///             SW_SHOW flag set to make it visible.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">
        ///     Specifies how the window is to be shown.
        ///     This parameter is ignored the first time an application calls
        ///     ShowWindow, if the program that launched the application provides a
        ///     STARTUPINFO structure. Otherwise, the first time ShowWindow is called,
        ///     the value should be the value obtained by the WinMain function in its
        ///     nCmdShow parameter. In subsequent calls, this parameter can be one of
        ///     the WindowShowStyle members.
        /// </param>
        /// <returns>
        ///     If the window was previously visible, the return value is nonzero.
        ///     If the window was previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Retrieves information about the specified window. The function also retrieves the value at a specified offset into
        ///     the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">
        ///     The zero-based offset to the value to be retrieved. Valid values are in the range zero through the
        ///     number of bytes of extra window memory, minus the size of an integer.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the requested value. If the function fails, the return value is
        ///     zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     To write code that is compatible with both 32-bit and 64-bit versions of Windows, use GetWindowLongPtr. When
        ///     compiling for 32-bit Windows, GetWindowLongPtr is defined as a call to the GetWindowLong function.
        /// </remarks>
        [DllImport("User32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLongPtr(IntPtr hWnd, WindowIndex nIndex);

        /// <summary>
        ///     Changes an attribute of the specified window. The function also sets a value at the specified offset in the extra
        ///     window memory.
        /// </summary>
        /// <param name="hWnd">
        ///     A handle to the window and, indirectly, the class to which the window belongs. The SetWindowLongPtr
        ///     function fails if the process that owns the window specified by the hWnd parameter is at a higher process privilege
        ///     in the UIPI hierarchy than the process the calling thread resides in.
        /// </param>
        /// <param name="nIndex">
        ///     The zero-based offset to the value to be set. Valid values are in the range zero through the
        ///     number of bytes of extra window memory, minus the size of an integer.
        /// </param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the previous value of the specified offset. If the function
        ///     fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     To write code that is compatible with both 32-bit and 64-bit versions of Windows, use SetWindowLongPtr. When
        ///     compiling for 32-bit Windows, SetWindowLongPtr is defined as a call to the SetWindowLong function.
        /// </remarks>
        [DllImport("User32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLongPtr(IntPtr hWnd, WindowIndex nIndex, int dwNewLong);

        #endregion
    }
}