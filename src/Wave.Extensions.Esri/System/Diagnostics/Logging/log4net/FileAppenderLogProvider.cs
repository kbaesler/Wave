using System.IO;

using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace System.Diagnostics
{
    /// <summary>
    ///     A log4net provider that allow for creating a <see cref="RollingFileAppender" /> that is attached
    ///     to the specific logger.
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILogProvider" />
    public class FileAppenderLogProvider : ApacheLogProvider, IDisposable
    {
        #region Fields

        private readonly string _File;
        private readonly FileMode _FileMode;
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
        /// <param name="fileInfo">The file information.</param>
        public FileAppenderLogProvider(string name, string file, LogLevel logLevel, FileInfo fileInfo)
            : base(fileInfo)
        {
            _Name = name;
            _File = file;
            _LogLevel = logLevel;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileAppenderLogProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <param name="logLevel">The log level.</param>
        public FileAppenderLogProvider(string name, string file, LogLevel logLevel)
            : this(name, file, FileMode.Append, logLevel)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileAppenderLogProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="logLevel">The log level.</param>
        public FileAppenderLogProvider(string name, string file, FileMode fileMode, LogLevel logLevel)
        {
            _Name = name;
            _File = file;
            _FileMode = fileMode;
            _LogLevel = logLevel;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     The appender
        /// </summary>
        protected IAppender Appender { get; set; }

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
        ///     Gets the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        ///     Returns a <see cref="ILog" /> implementation.
        /// </returns>
        public override ILog GetLogger(string loggerName)
        {
            var logger = (Logger) base.GetLogger(loggerName);
            AddAppender(logger);

            return logger;
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
            var logger = (Logger) base.GetLogger(loggerName, repositoryName);
            AddAppender(logger);

            return logger;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the appender.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        ///     Returns a <see cref="IAppender" /> representing the appender.
        /// </returns>
        protected virtual FileAppender CreateAppender(string name, string file, LogLevel logLevel)
        {
            PatternLayout layout = new PatternLayout("%date{yyyy-MM-dd hh:mm:ss tt} - [%level]: %message%newline%exception");

            LevelMatchFilter filter = new LevelMatchFilter();
            filter.LevelToMatch = logLevel.ToLevel();
            filter.ActivateOptions();

            var appender = new FileAppender();
            appender.Name = name;
            appender.File = file;
            appender.ImmediateFlush = true;
            appender.AppendToFile = _FileMode == FileMode.Append;
            appender.LockingModel = new FileAppender.MinimalLock();
            appender.AddFilter(filter);
            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }

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
                Appender?.Close();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds the appender.
        /// </summary>
        /// <param name="log">The log.</param>
        private void AddAppender(Logger log)
        {
            var wrapper = (ILoggerWrapper) log.InternaLogger;
            var appender = wrapper.Logger.FindAppender<FileAppender>(_Name);
            if (appender == null)
            {
                appender = CreateAppender(_Name, _File, _LogLevel);
                wrapper.Logger.AddAppender(appender);
            }

            this.Appender = appender;
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
        /// Initializes a new instance of the <see cref="RollingFileAppenderLogProvider"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        public RollingFileAppenderLogProvider(string name, string file)
            : this(name, file, LogLevel.Debug)
        {

        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RollingFileAppenderLogProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <param name="logLevel">The log level.</param>
        public RollingFileAppenderLogProvider(string name, string file, LogLevel logLevel)
            : base(name, file, FileMode.Create, logLevel)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the appender.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        ///     Returns a <see cref="IAppender" /> representing the appender.
        /// </returns>
        protected override FileAppender CreateAppender(string name, string file, LogLevel logLevel)
        {
            PatternLayout layout = new PatternLayout("%date{yyyy-MM-dd hh:mm:ss tt} - [%level]: %message%newline%exception");

            LevelMatchFilter filter = new LevelMatchFilter();
            filter.LevelToMatch = logLevel.ToLevel();
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