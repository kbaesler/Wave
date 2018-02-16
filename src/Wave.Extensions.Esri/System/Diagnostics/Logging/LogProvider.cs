using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace System.Diagnostics
{
    /// <summary>
    ///     Provides methods for logging at different levels using the Apache log4net framework.
    /// </summary>
    public static class LogExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Log a message object with the Debug level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Debug(this ILog source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Error, message);
        }


        /// <summary>
        ///     Log a message object with the Debug level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Debug(this ILog source, string format, params object[] args)
        {
            Debug(source, string.Format(CultureInfo.CurrentCulture, format, args));
        }


        /// <summary>
        ///     Log a message object with the Debug level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Debug(this ILog source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Debug, message, exception);
        }


        /// <summary>
        ///     Log a message object with the Error level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(this ILog source, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Error, null, exception);
        }

        /// <summary>
        ///     Log a message object with the Error level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(this ILog source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Error, message);
        }


        /// <summary>
        ///     Log a message object with the Error level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(this ILog source, string title, Exception exception)
        {
            Error(source, exception);
            MessageBox.Show(exception.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        ///     Log a message object with the Error level including the stack trace of the exception
        ///     and displays a message box to the user with the contents of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(this ILog source, string title, string message)
        {
            Error(source, message);
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        /// <summary>
        ///     Log a message object with the Info level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Info(this ILog source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Info, message);
        }


        /// <summary>
        ///     Log a message object with the Info level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Info(this ILog source, string format, params object[] args)
        {
            Info(source, string.Format(CultureInfo.CurrentCulture, format, args));
        }


        /// <summary>
        ///     Log a message object with the Info level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Info(this ILog source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Info, message, exception);
        }


        /// <summary>
        ///     Log a message object with the Warn level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Warn(this ILog source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Warn, message);
        }


        /// <summary>
        ///     Log a message object with the Warn level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Warn(this ILog source, string format, params object[] args)
        {
            Warn(source, string.Format(CultureInfo.CurrentCulture, format, args));
        }


        /// <summary>
        ///     Log a message object with the Warn level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Warn(this ILog source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Warn, message, exception);
        }

        #endregion
    }

    /// <summary>
    ///     Simple interface that represent a logger.
    /// </summary>
    public interface ILog
    {
        #region Public Methods

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
        bool Log(LogLevel logLevel, string message, Exception exception = null);

        #endregion
    }

    /// <summary>
    ///     The log level.
    /// </summary>
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    /// <summary>
    ///     Represents a way to get a <see cref="ILog" />
    /// </summary>
    public interface ILogProvider
    {
        #region Public Methods

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>Returns a <see cref="ILog" /> implementation.</returns>
        ILog GetLogger(string loggerName);

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>Returns a <see cref="ILog" /> implementation.</returns>
        ILog GetLogger(string loggerName, string repositoryName);

        #endregion
    }

    /// <summary>
    ///     Provides a mechanism to create instances of <see cref="ILog" /> objects.
    /// </summary>
    public static class LogProvider
    {
        #region Fields

        private static readonly Dictionary<string, ILogProvider> _LogProviders = new Dictionary<string, ILogProvider>();

        private static ILogProvider _LogProvider;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets a logger for the specified type.
        /// </summary>
        /// <typeparam name="T">The type whose name will be used for the logger.</typeparam>
        /// <returns>An instance of <see cref="ILog" /></returns>
        public static ILog For<T>()
        {
            return GetLogger(typeof(T));
        }

        /// <summary>
        ///     Gets a logger for the specified type.
        /// </summary>
        /// <typeparam name="T">The type whose name will be used for the logger.</typeparam>
        /// <param name="logProvider">The log provider.</param>
        /// <returns>
        ///     An instance of <see cref="ILog" />
        /// </returns>
        public static ILog For<T>(ILogProvider logProvider)
        {
            SetLogProvider(logProvider);

            return GetLogger(typeof(T));
        }

        /// <summary>
        ///     Gets a logger for the specified type.
        /// </summary>
        /// <typeparam name="T">The type whose name will be used for the logger.</typeparam>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>
        ///     An instance of <see cref="ILog" />
        /// </returns>
        public static ILog For<T>(string repositoryName)
        {
            return GetLogger(typeof(T), repositoryName);
        }

        /// <summary>
        ///     Gets a logger for the current class.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="ILog" />
        /// </returns>
        public static ILog GetCurrentLogger()
        {
            var stackFrame = new StackFrame(1, false);
            return GetLogger(stackFrame.GetMethod().DeclaringType);
        }

        /// <summary>
        ///     Gets a logger for the specified type.
        /// </summary>
        /// <param name="type">The type whose name will be used for the logger.</param>
        /// <returns>
        ///     An instance of <see cref="ILog" />
        /// </returns>
        public static ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        /// <summary>
        ///     Gets a logger for the specified type.
        /// </summary>
        /// <param name="type">The type whose name will be used for the logger.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>
        ///     An instance of <see cref="ILog" />
        /// </returns>
        public static ILog GetLogger(Type type, string repositoryName)
        {
            return GetLogger(type.FullName, repositoryName);
        }

        /// <summary>
        ///     Gets a logger with the specified name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        ///     An instance of <see cref="ILog" />
        /// </returns>
        public static ILog GetLogger(string loggerName)
        {
            ILogProvider logProvider = ResolveLogProviders();
            return logProvider.GetLogger(loggerName);
        }


        /// <summary>
        ///     Gets a logger with the specified name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>
        ///     An instance of <see cref="ILog" />
        /// </returns>
        public static ILog GetLogger(string loggerName, string repositoryName)
        {
            ILogProvider logProvider = ResolveLogProviders(repositoryName);
            return logProvider.GetLogger(loggerName, repositoryName);
        }

        /// <summary>
        ///     Sets the current log provider.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        public static void SetLogProvider(ILogProvider logProvider)
        {
            _LogProvider = logProvider;
        }

        /// <summary>
        ///     Sets the current log provider for the unique repository.
        /// </summary>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="logProvider">The log provider.</param>
        public static void SetLogProvider(string repositoryName, ILogProvider logProvider)
        {
            if (!_LogProviders.ContainsKey(repositoryName))
                _LogProviders.Add(repositoryName, logProvider);
            else
                _LogProviders[repositoryName] = logProvider;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Resolves the log providers.
        /// </summary>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>
        ///     An instance of <see cref="ILogProvider" />
        /// </returns>
        private static ILogProvider ResolveLogProviders(string repositoryName = null)
        {
            if (repositoryName != null && _LogProviders.ContainsKey(repositoryName))
                return _LogProviders[repositoryName];

            ILogProvider logProvider = _LogProvider ?? new LogManagerLogProvider();
            return logProvider;
        }

        #endregion
    }


    /// <summary>
    ///     A dynamic log that assumes it is compatible with <see cref="ILog" />
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILog" />
    public class DynamicLog : ILog
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicLog" /> class.
        /// </summary>
        protected DynamicLog()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicLog" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DynamicLog(dynamic logger)
        {
            this.Logger = logger;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <value>
        ///     The logger.
        /// </value>
        public dynamic Logger { get; protected set; }

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
        /// <remarks>
        ///     Note to implementers: the message func should not be called if the loglevel is not enabled
        ///     so as not to incur performance penalties.
        ///     To check IsEnabled call Log with only LogLevel and check the return value, no event will be written
        /// </remarks>
        public virtual bool Log(LogLevel logLevel, string message, Exception exception = null)
        {
            if (exception != null)
            {
                return LogException(logLevel, message, exception);
            }

            switch (logLevel)
            {
                case LogLevel.Info:
                    if (Logger.IsInfoEnabled)
                    {
                        Logger.Info(message);
                        return true;
                    }

                    break;
                case LogLevel.Warn:
                    if (Logger.IsWarnEnabled)
                    {
                        Logger.Warn(message);
                        return true;
                    }

                    break;
                case LogLevel.Error:
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error(message);
                        return true;
                    }

                    break;
                case LogLevel.Fatal:
                    if (Logger.IsFatalEnabled)
                    {
                        Logger.Fatal(message);
                        return true;
                    }

                    break;
                default:
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug(message);
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
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Info(message, exception);
                        return true;
                    }

                    break;
                case LogLevel.Warn:
                    if (Logger.IsWarnEnabled)
                    {
                        Logger.Warn(message, exception);
                        return true;
                    }

                    break;
                case LogLevel.Error:
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error(message, exception);
                        return true;
                    }

                    break;
                case LogLevel.Fatal:
                    if (Logger.IsFatalEnabled)
                    {
                        Logger.Fatal(message, exception);
                        return true;
                    }

                    break;
                default:
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug(message, exception);
                        return true;
                    }

                    break;
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    ///     A dynamic log that assumes it is compatible with <see cref="ILog" />
    ///     that tracks the log messages.
    /// </summary>
    public interface ILogLevelLog : ILog
    {
        #region Public Properties

        /// <summary>
        ///     Returns the log levels that are captured.
        /// </summary>
        LogLevel[] LogLevels { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the logs that have been captured for the log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        ///     Returns a <see cref="List{T}" /> representing the log messages captured.
        /// </returns>
        List<string> Captures(LogLevel logLevel);

        #endregion
    }

    /// <summary>
    ///     A dynamic log that assumes it is compatible with <see cref="ILog" />
    ///     that tracks the log messages.
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILog" />
    public class LogLevelLog : DynamicLog, ILogLevelLog
    {
        #region Fields

        private readonly Dictionary<LogLevel, List<string>> _Logs = new Dictionary<LogLevel, List<string>>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogLevelLog" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="logLevels">The log levels.</param>
        public LogLevelLog(dynamic logger, params LogLevel[] logLevels)
        {
            base.Logger = logger;
            this.LogLevels = logLevels;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The log levels that will be captured.
        /// </summary>
        public LogLevel[] LogLevels { get; }

        #endregion

        #region ILogLevelLog Members

        /// <summary>
        ///     Gets the logs that have been captured for the log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        ///     Returns a <see cref="List{T}" /> representing the log messages captured.
        /// </returns>
        public List<string> Captures(LogLevel logLevel)
        {
            if (_Logs.ContainsKey(logLevel))
                return _Logs[logLevel];

            return null;
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
            if (base.Log(logLevel, message, exception))
            {
                if (LogLevels == null || LogLevels.Contains(logLevel))
                {
                    var log = _Logs.GetOrAdd(logLevel, new List<string>());
                    log.Add(message);
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}