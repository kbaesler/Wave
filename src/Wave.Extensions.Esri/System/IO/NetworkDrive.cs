using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

namespace System.IO
{
    /// <summary>
    ///     Provides methods for making a connection to a network resource and can redirect a local device to the network
    ///     resource
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class NetworkDrive : IDisposable
    {
        #region Enumerations

        /// <summary>
        ///     The display options for the network object in a network browsing user interface.
        /// </summary>
        internal enum ResourceDisplayOptions
        {
            /// <summary>
            ///     The generic
            /// </summary>
            Generic = 0x0,

            /// <summary>
            ///     The domain
            /// </summary>
            Domain = 0x01,

            /// <summary>
            ///     The server
            /// </summary>
            Server = 0x02,

            /// <summary>
            ///     The share
            /// </summary>
            Share = 0x03,

            /// <summary>
            ///     The file
            /// </summary>
            File = 0x04,

            /// <summary>
            ///     The group
            /// </summary>
            Group = 0x05,

            /// <summary>
            ///     The network
            /// </summary>
            Network = 0x06,

            /// <summary>
            ///     The root
            /// </summary>
            Root = 0x07,

            /// <summary>
            ///     The shareadmin
            /// </summary>
            Shareadmin = 0x08,

            /// <summary>
            ///     The directory
            /// </summary>
            Directory = 0x09,

            /// <summary>
            ///     The tree
            /// </summary>
            Tree = 0x0a,

            /// <summary>
            ///     The ndscontainer
            /// </summary>
            Ndscontainer = 0x0b
        }

        /// <summary>
        ///     The scope of the enumeration.
        /// </summary>
        internal enum ResourceScope
        {
            /// <summary>
            ///     Enumerate currently connected resources.
            /// </summary>
            Connected = 1,

            /// <summary>
            ///     Enumerate all resources on the network.
            /// </summary>
            GlobalNetwork,

            /// <summary>
            ///     Enumerate remembered (persistent) connections
            /// </summary>
            Remembered
        };

        /// <summary>
        ///     The type of resource.
        /// </summary>
        internal enum ResourceType
        {
            /// <summary>
            ///     All resources.
            /// </summary>
            Any = 0,

            /// <summary>
            ///     Disk resources.
            /// </summary>
            Disk = 1,

            /// <summary>
            ///     Print resources.
            /// </summary>
            Print = 2,
        }

        #endregion

        #region Fields

        private readonly NetworkResource _Resource;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkDrive" /> class.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="credentials">The credentials.</param>
        /// <exception cref="Win32Exception">Error connecting to remote share</exception>
        internal NetworkDrive(NetworkResource resource, NetworkCredential credentials)
        {
            _Resource = resource;

            var userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            var result = WNetAddConnection2(
                resource,
                credentials.Password,
                userName,
                0);

            if (result != 0)
            {
                throw new Win32Exception(result, "Error connecting to remote share");
            }
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

        #region Public Methods

        /// <summary>
        ///     Maps the specified path using the credentials.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public static IDisposable Map(string path, NetworkCredential credentials)
        {
            var resource = new NetworkResource
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplayOptions.Share,
                RemoteName = path,
            };

            return new NetworkDrive(resource, credentials);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_Resource.RemoteName, 0, true);
        }

        #endregion

        #region Private Methods

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetworkResource resource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);

        #endregion

        #region Nested Type: NetworkResource

        /// <summary>
        ///     The NETRESOURCE structure contains information about a network resource.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class NetworkResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplayOptions DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        #endregion
    }
}