using System.ComponentModel;
using System.Native;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security
{
    /// <summary>
    ///     Security impersonation levels govern the degree to which a server process can act on behalf of a client process.
    /// </summary>
    public enum SecurityImpersonationLevel
    {
        /// <summary>
        ///     The server process cannot obtain identification information about the client,
        ///     and it cannot impersonate the client. It is defined with no value given, and thus,
        ///     by ANSI C rules, defaults to a value of zero.
        /// </summary>
        SecurityAnonymous = 0,

        /// <summary>
        ///     The server process can obtain information about the client, such as security identifiers and privileges,
        ///     but it cannot impersonate the client. This is useful for servers that export their own objects,
        ///     for example, database products that export tables and views.
        ///     Using the retrieved client-security information, the server can make access-validation decisions without
        ///     being able to use other services that are using the client's security context.
        /// </summary>
        SecurityIdentification = 1,

        /// <summary>
        ///     The server process can impersonate the client's security context on its local system.
        ///     The server cannot impersonate the client on remote systems.
        /// </summary>
        SecurityImpersonation = 2,

        /// <summary>
        ///     The server process can impersonate the client's security context on remote systems.
        ///     NOTE: Windows NT:  This impersonation level is not supported.
        /// </summary>
        SecurityDelegation = 3,
    }

    /// <summary>
    ///     Impersonation of a user. Allows to execute code under another
    ///     user context.
    /// </summary>
    /// <remarks>
    ///     Please note that the account that instantiates the Impersonator class
    ///     needs to have the 'Act as part of operating system' privilege set.
    /// </remarks>
    public class Impersonation : IDisposable
    {
        #region Fields

        private WindowsImpersonationContext _ImpersonationContext;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Impersonation" /> class.
        /// </summary>
        /// <param name="userName">The name of the user to act as.</param>
        /// <param name="domainName">The domain name of the user to act as.</param>
        /// <param name="password">The password of the user to act as.</param>
        public Impersonation(string userName, string domainName, string password)
        {
            Impersonate(userName, domainName, password, SecurityImpersonationLevel.SecurityImpersonation);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Impersonation" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="password">The password.</param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        public Impersonation(string userName, string domainName, string password, SecurityImpersonationLevel impersonationLevel)
        {
            Impersonate(userName, domainName, password, impersonationLevel);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            if (disposing && _ImpersonationContext != null)
            {
                _ImpersonationContext.Undo();
            }
        }

        /// <summary>
        ///     Impersonates the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="password">The password.</param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        /// <exception cref="Win32Exception">
        /// </exception>
        private void Impersonate(string userName, string domain, string password, SecurityImpersonationLevel impersonationLevel)
        {
            if (UnsafeWindowMethods.RevertToSelf())
            {
                SafeTokenHandle safeTokenHandle;
                if (UnsafeWindowMethods.LogonUser(userName, domain, password, UnsafeWindowMethods.LogonType.Interactive, UnsafeWindowMethods.LogonProvider.Default, out safeTokenHandle) != 0)
                {
                    using (safeTokenHandle)
                    {
                        SafeTokenHandle safeDuplicateTokenHandle;
                        if (UnsafeWindowMethods.DuplicateToken(safeTokenHandle.DangerousGetHandle(), (int) impersonationLevel, out safeDuplicateTokenHandle) != 0)
                        {
                            using (safeDuplicateTokenHandle)
                            {
                                var windowsIdentity = new WindowsIdentity(safeDuplicateTokenHandle.DangerousGetHandle());
                                _ImpersonationContext = windowsIdentity.Impersonate();
                            }
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
            else
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        #endregion
    }
}