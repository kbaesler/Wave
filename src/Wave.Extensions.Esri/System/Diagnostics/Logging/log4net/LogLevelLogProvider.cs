using System.Collections;
using System.Collections.Generic;
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
        ///     Initializes a new instance of the <see cref="LogLevelLogProvider" /> class.
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

        #region Nested Type: LogLevelLog

        /// <summary>
        ///     A dynamic log that assumes it is compatible with <see cref="ILog" />
        ///     that tracks the log messages.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ILog" />
        internal class LogLevelLog : DynamicLog, ILogLevelLog
        {
            #region Fields

            private readonly Dictionary<LogLevel, int> _Logs = new Dictionary<LogLevel, int>();

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="LogLevelLog" /> class.
            /// </summary>
            /// <param name="logger">The logger.</param>
            /// <param name="logLevel">The log level.</param>
            public LogLevelLog(dynamic logger, LogLevel logLevel)
            {
                base.Logger = logger;
                this.LogLevel = logLevel;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     The log levels that will be captured.
            /// </summary>
            public LogLevel LogLevel { get; }

            #endregion

            #region ILogLevelLog Members

            /// <summary>
            ///     Gets the number of times the a log message with that log level has been captured.
            /// </summary>
            /// <param name="logLevel">The log level.</param>
            /// <returns>
            ///     Returns a <see cref="int" /> representing the number of messages logged.
            /// </returns>
            public int Captures(LogLevel logLevel)
            {
                return _Logs.GetOrAdd(logLevel, 0);
            }

            /// <summary>
            ///     Log a message the specified log level.
            /// </summary>
            /// <param name="logLevel">The log level.</param>
            /// <param name="message">The message.</param>
            /// <param name="exception">An optional exception.</param>
            /// <returns>
            ///     true if the message was logged. Otherwise false.
            /// </returns>
            /// <remarks>
            ///     Note to implementers: the message func should not be called if the loglevel is not enabled
            ///     so as not to incur performance penalties.
            ///     To check IsEnabled call Log with only LogLevel and check the return value, no event will be written
            /// </remarks>
            public override bool Log(LogLevel logLevel, string message, Exception exception = null)
            {
                if (logLevel <= LogLevel)
                {
                    _Logs[logLevel] = _Logs.GetOrAdd(logLevel, 0) + 1;
                }

                return base.Log(logLevel, message, exception);
            }

            #endregion
        }

        #endregion
    }
}