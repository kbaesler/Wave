using System.IO;
using System.Linq;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
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
            return new ApacheLog(LogManager.GetLogger(loggerName));
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

                return new ApacheLog(LogManager.GetLogger(repositoryName, loggerName));
            }

            return new ApacheLog(LogManager.GetLogger(repositoryName, loggerName));
        }

        #endregion

        #region Nested Type: ApacheLog

        /// <summary>
        ///     The logger for the <see cref="log4net" /> component.
        /// </summary>
        /// <seealso cref="System.Diagnostics.DynamicLog" />
        /// <seealso cref="System.Diagnostics.IFileLog" />
        class ApacheLog : DynamicLog, IFileLog
        {
            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ApacheLog" /> class.
            /// </summary>
            /// <param name="logger">The logger.</param>
            public ApacheLog(log4net.ILog logger)
                : base(logger)
            {
            }

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
                var wrapper = (ILoggerWrapper) Logger;
                var appender = wrapper.Logger.FindAppender<FileAppender>(name);
                return appender?.File;
            }

            #endregion
        }

        #endregion
    }
}