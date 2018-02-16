using System.Linq;

using log4net;
using log4net.Config;

namespace System.Diagnostics
{
    /// <summary>
    ///     A provider for the <see cref="log4net" /> Apache Foundation.
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILogProvider" />
    public class LogManagerLogProvider : ILogProvider
    {
        #region ILogProvider Members

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        ///     Returns a <see cref="ILog" /> implementation.
        /// </returns>
        public virtual ILog GetLogger(string loggerName)
        {
            return new DynamicLog(LogManager.GetLogger(loggerName));
        }


        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>
        ///     Returns a <see cref="ILog" /> implementation.
        /// </returns>
        public virtual ILog GetLogger(string loggerName, string repositoryName)
        {
            if (LogManager.GetAllRepositories().All(o => o.Name != repositoryName))
            {
                var repository = LogManager.CreateRepository(repositoryName);
                BasicConfigurator.Configure(repository);

                return new DynamicLog(LogManager.GetLogger(repositoryName, loggerName));
            }

            return new DynamicLog(LogManager.GetLogger(repositoryName, loggerName));
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
            if (disposing)
            {
            }
        }

        #endregion
    }
}