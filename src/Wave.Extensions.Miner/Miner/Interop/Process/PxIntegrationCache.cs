using Miner.Framework;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     A singleton for accessing the <see cref="IMMPxIntegrationCache" /> instance.
    /// </summary>
    public static class PxIntegrationCache
    {
        #region Public Properties

        /// <summary>
        ///     This returns the current running instance of application.
        /// </summary>
        /// <value>
        ///     The application.
        /// </value>
        public static IMMPxApplication Application
        {
            get
            {
                IMMPxIntegrationCache2 cache = Instance;
                return (cache != null) ? cache.Application : null;
            }
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static IMMPxIntegrationCache2 Instance
        {
            get
            {
                try
                {
                    return Document.FindExtensionByName(ArcFM.Extensions.Name.ProcessFrameworkCache) as IMMPxIntegrationCache2;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion
    }
}