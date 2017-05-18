using System.Runtime.InteropServices;
using System.Security;

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
        /// The type of logon operation to perform.
        /// </summary>
        public enum LogonType
        {
            /// <summary>
            ///     This logon type is intended for batch servers, where processes may be executing on behalf of a user without
            ///     their direct intervention. This type is also for higher performance servers that process many plaintext
            ///     authentication attempts at a time, such as mail or Web servers.
            ///     The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            Batch = 4,

            /// <summary>
            ///     This logon type is intended for users who will be interactively using the computer, such as a user being logged on
            ///     by a terminal server, remote shell, or similar process.
            ///     This logon type has the additional expense of caching logon information for disconnected operations,
            ///     therefore, it is inappropriate for some client/server applications,
            ///     such as a mail server.
            /// </summary>
            Interactive = 2,

            /// <summary>
            ///     This logon type is intended for high performance servers to authenticate plaintext passwords.
            ///     The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            Network = 3,

            /// <summary>
            ///     This logon type preserves the name and password in the authentication package, which allows the server to make
            ///     connections to other network servers while impersonating the client. A server can accept plaintext credentials
            ///     from a client, call LogonUser, verify that the user can access the system across the network, and still
            ///     communicate with other servers.
            ///     NOTE: Windows NT:  This value is not supported.
            /// </summary>
            NetworkClearText = 8,

            /// <summary>
            ///     This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
            ///     The new logon session has the same local identifier but uses different credentials for other network connections.
            ///     NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
            ///     NOTE: Windows NT:  This value is not supported.
            /// </summary>
            NewCredentials = 9,

            /// <summary>
            ///     Indicates a service-type logon. The account provided must have the service privilege enabled.
            /// </summary>
            Service = 5,

            /// <summary>
            ///     This logon type is for GINA DLLs that log on users who will be interactively using the computer.
            ///     This logon type can generate a unique audit record that shows when the workstation was unlocked.
            /// </summary>
            Unlock = 7,
        }

        /// <summary>
        ///     The zero-based offset to the value to be retrieved. Valid values are in the range zero through the number of bytes
        ///     of extra window memory, minus the size of an integer.
        ///     To retrieve any other value, specify one of the following values.
        /// </summary>
        public enum WindowIndex
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
        public enum WindowStyles
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
        ///     Closes the handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        ///     Duplicates the token.
        /// </summary>
        /// <param name="hToken">The h token.</param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        /// <param name="hNewToken">The h new token.</param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, out SafeTokenHandle hNewToken);

        /// <summary>
        /// The type of logon provider
        /// </summary>
        public enum LogonProvider
        {
            /// <summary>
            /// Use the standard logon provider for the system. 
            /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name 
            /// is not in UPN format. In this case, the default provider is NTLM. 
            /// </summary>
            Default = 0,
            WinNT35 = 1,
            WinNT40 = 2,
            WinNT50 = 3
        }

        /// <summary>
        ///     The LogonUser function attempts to log a user on to the local computer. The local computer is the computer from
        ///     which LogonUser was called. You cannot use LogonUser to log on to a remote computer.
        ///     You specify the user with a user name and domain and authenticate the user with a plaintext password. If the
        ///     function succeeds, you receive a handle to a token that represents the logged-on user.
        ///     You can then use this token handle to impersonate the specified user or, in most cases, to create a process that
        ///     runs in the context of the specified user.
        /// </summary>
        /// <param name="lpszUserName">
        ///     A pointer to a null-terminated string that specifies the name of the user. This is the name
        ///     of the user account to log on to. If you use the user principal name (UPN) format, User@DNSDomainName, the
        ///     lpszDomain parameter must be NULL.
        /// </param>
        /// <param name="lpszDomain">
        ///     A pointer to a null-terminated string that specifies the name of the domain or server whose
        ///     account database contains the lpszUsername account. If this parameter is NULL, the user name must be specified in
        ///     UPN format. If this parameter is ".", the function validates the account by using only the local account database.
        /// </param>
        /// <param name="lpszPassword">
        ///     A pointer to a null-terminated string that specifies the plaintext password for the user
        ///     account specified by lpszUsername. When you have finished using the password, clear the password from memory by
        ///     calling the SecureZeroMemory function. For more information about protecting passwords, see Handling Passwords.
        /// </param>
        /// <param name="dwLogonType">The type of logon operation to perform.</param>
        /// <param name="dwLogonProvider">Specifies the logon provider.</param>
        /// <param name="phToken">
        ///     A pointer to a handle variable that receives a handle to a token that represents the specified
        ///     user.
        /// </param>
        /// <returns>If the function succeeds, the function returns nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int LogonUser(string lpszUserName, string lpszDomain, IntPtr lpszPassword, LogonType dwLogonType, LogonProvider dwLogonProvider, out SafeTokenHandle phToken);

        /// <summary>
        ///     The RevertToSelf function terminates the impersonation of a client application.
        /// </summary>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        /// <remarks>
        ///     A process should call the RevertToSelf function after finishing any impersonation begun by using the
        ///     DdeImpersonateClient, ImpersonateDdeClientWindow, ImpersonateLoggedOnUser, ImpersonateNamedPipeClient,
        ///     ImpersonateSelf, ImpersonateAnonymousToken or SetThreadToken function.
        ///     An RPC server that used the RpcImpersonateClient function to impersonate a client must call the RpcRevertToSelf or
        ///     RpcRevertToSelfEx to end the impersonation.
        ///     If RevertToSelf fails, your application continues to run in the context of the client, which is not appropriate.
        ///     You should shut down the process if RevertToSelf fails.
        /// </remarks>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

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