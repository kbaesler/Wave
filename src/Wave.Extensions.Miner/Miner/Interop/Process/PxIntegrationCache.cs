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
            get { return GetCache(ArcFM.Extensions.Name.ProcessFrameworkCache) as IMMPxIntegrationCache2; }
        }

        /// <summary>
        ///     Gets the session manager.
        /// </summary>
        /// <value>
        ///     The session manager.
        /// </value>
        public static IMMSessionManager SessionManager
        {
            get
            {
                var cache = GetCache(ArcFM.Extensions.Name.SessionManager);
                if (cache == null) return null;

                var app = cache.Application;
                if (app != null) return app.GetSessionManager();

                return null;
            }
        }

        /// <summary>
        ///     Gets the workflow manager.
        /// </summary>
        /// <value>
        ///     The workflow manager.
        /// </value>
        public static IMMWorkflowManager WorkflowManager
        {
            get
            {
                var cache = GetCache(ArcFM.Extensions.Name.WorkflowManager);
                if (cache == null) return null;

                var app = cache.Application;
                if (app != null) return app.GetWorkflowManager();

                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Initalizes the cache.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        /// <returns></returns>
        private static IMMPxIntegrationCache GetCache(string extensionName)
        {
            try
            {
                return Document.FindExtensionByName(extensionName) as IMMPxIntegrationCache;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}