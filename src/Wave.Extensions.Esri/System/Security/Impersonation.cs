using System.ComponentModel;
using System.Native;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.Security
{
    #region Enumerations

    /// <summary>
    ///     Security impersonation levels govern the degree to which a server process can act on behalf of a client process.
    /// </summary>
    public enum ImpersonationLevel : int
    {
        /// <summary>
        ///     The server process cannot obtain identification information about the client,
        ///     and it cannot impersonate the client. It is defined with no value given, and thus,
        ///     by ANSI C rules, defaults to a value of zero.
        /// </summary>
        Anonymous = 0,

        /// <summary>
        ///     The server process can obtain information about the client, such as security identifiers and privileges,
        ///     but it cannot impersonate the client. This is useful for servers that export their own objects,
        ///     for example, database products that export tables and views.
        ///     Using the retrieved client-security information, the server can make access-validation decisions without
        ///     being able to use other services that are using the client's security context.
        /// </summary>
        Identification = 1,

        /// <summary>
        ///     The server process can impersonate the client's security context on its local system.
        ///     The server cannot impersonate the client on remote systems.
        /// </summary>
        Impersonation = 2,

        /// <summary>
        ///     The server process can impersonate the client's security context on remote systems.
        ///     NOTE: Windows NT:  This impersonation level is not supported.
        /// </summary>
        Delegation = 3,
    }

    #endregion

    /// <summary>
    ///     The type of logon operation to perform.
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
    ///     Impersonation of a user. Allows to execute code under another
    ///     user context.
    /// </summary>
    /// <remarks>
    ///     Please note that the account that instantiates the Impersonator class
    ///     needs to have the 'Act as part of operating system' privilege set.
    /// </remarks>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class Impersonation : IDisposable
    {
        #region Fields

        private WindowsImpersonationContext _ImpersonationContext;
        private SafeTokenHandle _SafeDuplicateTokenHandle;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Impersonation" /> class.
        /// </summary>
        /// <param name="userName">The name of the user to act as.</param>
        /// <param name="domainName">The domain name of the user to act as.</param>
        /// <param name="password">The password of the user to act as.</param>
        public Impersonation(string userName, string domainName, SecureString password)
            : this(userName, domainName, password, ImpersonationLevel.Identification)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Impersonation" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="password">The password.</param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        public Impersonation(string userName, string domainName, SecureString password, ImpersonationLevel impersonationLevel)
            : this(userName, domainName, password, impersonationLevel, LogonType.Interactive)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Impersonation" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="password">The password.</param>
        /// <param name="logonType">Type of the logon.</param>
        public Impersonation(string userName, string domainName, SecureString password, LogonType logonType)
            : this(userName, domainName, password, ImpersonationLevel.Identification, logonType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Impersonation" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="password">The password.</param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        /// <param name="logonType">Type of the logon.</param>
        public Impersonation(string userName, string domainName, SecureString password, ImpersonationLevel impersonationLevel, LogonType logonType)
        {
            Impersonate(userName, domainName, password, impersonationLevel, logonType);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ImpersonationContext != null)
                {
                    _ImpersonationContext.Undo();
                    _ImpersonationContext = null;
                }

                if (_SafeDuplicateTokenHandle != null)
                {
                    _SafeDuplicateTokenHandle.Dispose();
                    _SafeDuplicateTokenHandle = null;
                }
            }
        }

        /// <summary>
        ///     Impersonates the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="password">The password.</param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        /// <param name="logonType">Type of the logon.</param>
        /// <exception cref="Win32Exception">
        /// </exception>
        private void Impersonate(string userName, string domain, SecureString password, ImpersonationLevel impersonationLevel, LogonType logonType)
        {
            if (UnsafeWindowMethods.RevertToSelf())
            {
                var token = Marshal.SecureStringToGlobalAllocUnicode(password);
                var logonProvider = (logonType == LogonType.NewCredentials)
                    ? UnsafeWindowMethods.LogonProvider.WinNT50
                    : UnsafeWindowMethods.LogonProvider.Default;

                try
                {
                    SafeTokenHandle safeTokenHandle;
                    if (UnsafeWindowMethods.LogonUser(userName, domain, token, logonType, logonProvider, out safeTokenHandle) != 0)
                    {
                        using (safeTokenHandle)
                        {
                            if (UnsafeWindowMethods.DuplicateToken(safeTokenHandle.DangerousGetHandle(), (int) impersonationLevel, out _SafeDuplicateTokenHandle) != 0)
                            {
                                _ImpersonationContext = WindowsIdentity.Impersonate(_SafeDuplicateTokenHandle.DangerousGetHandle());
                            }
                            else
                            {
                                throw new Win32Exception(Marshal.GetLastWin32Error());
                            }
                        }
                    }
                    else
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    // Perform cleanup whether or not the call succeeded.
                    // Zero-out and free the unmanaged string reference.
                    Marshal.ZeroFreeGlobalAllocUnicode(token);
                }
            }
            else
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        #endregion
    }
}