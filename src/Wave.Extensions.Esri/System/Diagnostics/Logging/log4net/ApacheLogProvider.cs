using System.IO;
using System.Linq;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace System.Diagnostics
{
    /// <summary>
    ///     Provides extensions for the <see cref="log4net" /> Apache Foundation.
    /// </summary>
    public static class ApacheLogProviderExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the appender to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="appender">The appender.</param>
        public static void AddAppender(this ILogger logger, IAppender appender)
        {
            var skeleton = FindAppender<IAppender>(logger, appender.Name);
            if (skeleton == null)
            {
                var hierarchy = (Hierarchy) logger.Repository;
                hierarchy.Root.AddAppender(appender);
                hierarchy.Configured = true;
                hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Finds the appender that is attached to a logger.
        /// </summary>
        /// <typeparam name="TAppender">The type of the appender.</typeparam>
        /// <param name="logger">The logger.</param>
        /// <param name="appenderName">Name of the appender.</param>
        /// <returns></returns>
        public static TAppender FindAppender<TAppender>(this ILogger logger, string appenderName)
            where TAppender : IAppender
        {
            foreach (IAppender appender in logger.Repository.GetAppenders())
            {
                if (appender.Name == appenderName)
                {
                    return (TAppender) appender;
                }
            }

            return default(TAppender);
        }

        /// <summary>
        ///     Finds the appender that is attached to a logger and removes it.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="appender">The appender.</param>
        public static void RemoveAppender(this ILogger logger, IAppender appender)
        {
            var hierarchy = (Hierarchy) logger.Repository;
            hierarchy.Root.RemoveAppender(appender);
            hierarchy.Configured = true;
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
        }


        /// <summary>
        ///     Covnerts to level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns></returns>
        public static Level ToLevel(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Error:
                    return Level.Error;
                case LogLevel.Fatal:
                    return Level.Fatal;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Warn:
                    return Level.Warn;
                default:
                    return Level.All;
            }
        }

        #endregion
    }

    /// <summary>
    ///     A provider for the <see cref="log4net" /> Apache Foundation.
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILogProvider" />
    public class ApacheLogProvider : ILogProvider
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApacheLogProvider" /> class.
        /// </summary>
        public ApacheLogProvider()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApacheLogProvider" /> class.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        public ApacheLogProvider(FileInfo fileInfo)
        {
            XmlConfigurator.ConfigureAndWatch(fileInfo);
        }

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
            return new Logger(LogManager.GetLogger(loggerName));
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

                return new Logger(LogManager.GetLogger(repositoryName, loggerName));
            }

            return new Logger(LogManager.GetLogger(repositoryName, loggerName));
        }

        #endregion

        #region Nested Type: Logger

        /// <summary>
        ///     The logger for the <see cref="log4net" /> component.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ILogFile" />
        internal class Logger : ILogFile
        {
            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Logger" /> class.
            /// </summary>
            /// <param name="logger">The logger.</param>
            public Logger(log4net.ILog logger)
            {
                InternaLogger = logger;
            }

            #endregion

            #region Internal Properties

            /// <summary>
            ///     Gets or sets the interna logger.
            /// </summary>
            /// <value>
            ///     The interna logger.
            /// </value>
            internal log4net.ILog InternaLogger { get; }

            #endregion

            #region IFileLog Members

            /// <summary>
            ///     The file based on the name from the logger.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns>
            ///     Retunrns the path to the file.
            /// </returns>
            public string GetPath(string name)
            {
                var wrapper = (ILoggerWrapper) InternaLogger;
                var appender = wrapper.Logger.FindAppender<FileAppender>(name);
                return appender?.File;
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
            public virtual bool Log(LogLevel logLevel, string message, Exception exception = null)
            {
                if (exception != null)
                {
                    return LogException(logLevel, message, exception);
                }

                switch (logLevel)
                {
                    case LogLevel.Info:
                        if (InternaLogger.IsInfoEnabled)
                        {
                            InternaLogger.Info(message);
                            return true;
                        }

                        break;
                    case LogLevel.Warn:
                        if (InternaLogger.IsWarnEnabled)
                        {
                            InternaLogger.Warn(message);
                            return true;
                        }

                        break;
                    case LogLevel.Error:
                        if (InternaLogger.IsErrorEnabled)
                        {
                            InternaLogger.Error(message);
                            return true;
                        }

                        break;
                    case LogLevel.Fatal:
                        if (InternaLogger.IsFatalEnabled)
                        {
                            InternaLogger.Fatal(message);
                            return true;
                        }

                        break;
                    default:
                        if (InternaLogger.IsDebugEnabled)
                        {
                            InternaLogger.Debug(message);
                            return true;
                        }

                        break;
                }

                return false;
            }

            #endregion

            #region Private Methods

            private bool LogException(LogLevel logLevel, string message, Exception exception)
            {
                switch (logLevel)
                {
                    case LogLevel.Info:
                        if (InternaLogger.IsDebugEnabled)
                        {
                            InternaLogger.Info(message, exception);
                            return true;
                        }

                        break;
                    case LogLevel.Warn:
                        if (InternaLogger.IsWarnEnabled)
                        {
                            InternaLogger.Warn(message, exception);
                            return true;
                        }

                        break;
                    case LogLevel.Error:
                        if (InternaLogger.IsErrorEnabled)
                        {
                            InternaLogger.Error(message, exception);
                            return true;
                        }

                        break;
                    case LogLevel.Fatal:
                        if (InternaLogger.IsFatalEnabled)
                        {
                            InternaLogger.Fatal(message, exception);
                            return true;
                        }

                        break;
                    default:
                        if (InternaLogger.IsDebugEnabled)
                        {
                            InternaLogger.Debug(message, exception);
                            return true;
                        }

                        break;
                }

                return false;
            }

            #endregion
        }

        #endregion

        #region Nested Type: RepositoryLogger

        internal class RepositoryLogger : ILog
        {
            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="RepositoryLogger" /> class.
            /// </summary>
            /// <param name="repositories">The repositories.</param>
            /// <param name="loggerName">Name of the logger.</param>
            public RepositoryLogger(ILoggerRepository[] repositories, string loggerName)
            {
                Repositories = repositories;
                Name = loggerName;
            }

            #endregion

            #region Protected Properties

            /// <summary>
            ///     Gets or sets the name.
            /// </summary>
            /// <value>
            ///     The name.
            /// </value>
            protected string Name { get; set; }

            /// <summary>
            ///     Gets or sets the repositories.
            /// </summary>
            /// <value>
            ///     The repositories.
            /// </value>
            protected ILoggerRepository[] Repositories { get; set; }

            #endregion

            #region ILog Members

            /// <summary>
            ///     Log a message the specified log level.
            /// </summary>
            /// <param name="logLevel">The log level.</param>
            /// <param name="message">The message.</param>
            /// <param name="exception">An optional exception.</param>
            /// <returns>
            ///     true if the message was logged. Otherwise false.
            /// </returns>
            public bool Log(LogLevel logLevel, string message, Exception exception = null)
            {
                foreach (var repository in Repositories)
                {
                    var logger = new Logger(LogManager.GetLogger(repository.Name, Name));
                    logger.Log(logLevel, message, exception);
                }

                return true;
            }

            #endregion
        }

        #endregion
    }
}