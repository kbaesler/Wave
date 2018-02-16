using System.IO;

using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace System.Diagnostics
{
    /// <summary>
    ///     A log4net provider that allow for creating a <see cref="RollingFileAppender" /> that is attached
    ///     to the specific logger.
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILogProvider" />
    public class FileAppenderLogProvider : LogManagerLogProvider
    {
        #region Fields

        private readonly string _File;
        private readonly LogLevel _LogLevel;

        private readonly string _Name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileAppenderLogProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <param name="logLevel">The log level.</param>
        public FileAppenderLogProvider(string name, string file, LogLevel logLevel)
        {
            _Name = name;
            _File = file;
            _LogLevel = logLevel;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        ///     Returns a <see cref="ILog" /> implementation.
        /// </returns>
        public override ILog GetLogger(string loggerName)
        {
            var log = (DynamicLog) base.GetLogger(loggerName);

            var appender = CreateAppender(_Name, _File, _LogLevel);
            var wrapper = (ILoggerWrapper) log.Logger;
            AddAppender(wrapper.Logger, appender);

            return log;
        }


        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>
        ///     Returns a <see cref="ILog" /> implementation.
        /// </returns>
        public override ILog GetLogger(string loggerName, string repositoryName)
        {
            var log = (DynamicLog) base.GetLogger(loggerName, repositoryName);

            var appender = CreateAppender(_Name, _File, _LogLevel);
            var wrapper = (ILoggerWrapper) log.Logger;
            AddAppender(wrapper.Logger, appender);

            return log;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Adds the appender to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="appender">The appender.</param>
        protected void AddAppender(ILogger logger, IAppender appender)
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
        ///     Covnerts to level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns></returns>
        protected Level ConvertToLevel(LogLevel logLevel)
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

        /// <summary>
        ///     Creates the appender.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        ///     Returns a <see cref="IAppender" /> representing the appender.
        /// </returns>
        protected virtual IAppender CreateAppender(string name, string file, LogLevel logLevel)
        {
            PatternLayout layout = new PatternLayout("%date{yyyy-MM-dd hh:mm:ss tt} - [%level]: %message%newline%exception");

            LevelMatchFilter filter = new LevelMatchFilter();
            filter.LevelToMatch = ConvertToLevel(logLevel);
            filter.ActivateOptions();

            var appender = new FileAppender();
            appender.Name = name;
            appender.File = file;
            appender.ImmediateFlush = true;
            appender.AppendToFile = true;
            appender.LockingModel = new FileAppender.MinimalLock();
            appender.AddFilter(filter);
            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }

        /// <summary>
        ///     Finds the appender that is attached to a logger.
        /// </summary>
        /// <typeparam name="TAppender">The type of the appender.</typeparam>
        /// <param name="logger">The logger.</param>
        /// <param name="appenderName">Name of the appender.</param>
        /// <returns></returns>
        protected TAppender FindAppender<TAppender>(ILogger logger, string appenderName)
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
        protected void RemoveAppender(ILogger logger, IAppender appender)
        {
            var hierarchy = (Hierarchy) logger.Repository;
            hierarchy.Root.RemoveAppender(appender);
            hierarchy.Configured = true;
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
        }

        #endregion
    }


    /// <summary>
    ///     A log4net provider that allow for creating a <see cref="RollingFileAppender" /> that is attached
    ///     to the specific logger.
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILogProvider" />
    public class RollingFileAppenderLogProvider : FileAppenderLogProvider
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RollingFileAppenderLogProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <param name="logLevel">The log level.</param>
        public RollingFileAppenderLogProvider(string name, string file, LogLevel logLevel) : base(name, file, logLevel)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the appender.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <returns>
        ///     Returns a <see cref="IAppender" /> representing the appender.
        /// </returns>
        protected override IAppender CreateAppender(string name, string file, LogLevel logLevel)
        {
            PatternLayout layout = new PatternLayout("%date{yyyy-MM-dd hh:mm:ss tt} - [%level]: %message%newline%exception");

            LevelMatchFilter filter = new LevelMatchFilter();
            filter.LevelToMatch = ConvertToLevel(logLevel);
            filter.ActivateOptions();

            var ext = Path.GetExtension(file);

            var appender = new RollingFileAppender();
            appender.Name = name;
            appender.File = !string.IsNullOrEmpty(ext) ? file.Replace(ext, "") : file;
            appender.ImmediateFlush = true;
            appender.RollingStyle = RollingFileAppender.RollingMode.Date;
            appender.StaticLogFileName = false;
            appender.DatePattern = "_yyyy-MM-dd'.log'";
            appender.AppendToFile = true;
            appender.LockingModel = new FileAppender.MinimalLock();
            appender.AddFilter(filter);
            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }

        #endregion
    }
}