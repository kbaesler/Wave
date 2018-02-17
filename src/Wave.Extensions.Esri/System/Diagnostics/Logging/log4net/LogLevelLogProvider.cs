using System.Linq;

using log4net;
using log4net.Config;

namespace System.Diagnostics
{
    /// <summary>
    ///     A provider for the <see cref="log4net" /> that caches the logs that have been captured based on the specified log
    ///     levels.
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILogProvider" />
    public class LogLevelLogProvider : ILogProvider
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLevelLogProvider" /> class.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        public LogLevelLogProvider(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the log levels.
        /// </summary>
        /// <value>
        ///     The log levels.
        /// </value>
        public LogLevel LogLevel { get; }

        #endregion

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
            return new LogLevelLog(LogManager.GetLogger(loggerName), LogLevel);
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

                return new LogLevelLog(LogManager.GetLogger(repositoryName, loggerName), LogLevel);
            }

            return new LogLevelLog(LogManager.GetLogger(repositoryName, loggerName), LogLevel);
        }

        #endregion
    }
}