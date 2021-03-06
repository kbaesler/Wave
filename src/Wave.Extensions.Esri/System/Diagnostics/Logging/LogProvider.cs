﻿using System.Globalization;
using System.Windows.Forms;

namespace System.Diagnostics
{
    /// <summary>
    ///     Provides methods for logging at different levels using the <see cref="ILog" /> interface
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

            source.Log(LogLevel.Debug, message);
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
        ///     Log a message object with the Fatal level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Fatal(this ILog source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Fatal, message);
        }

        /// <summary>
        ///     Log a message object with the Fatal level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Fatal(this ILog source, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Fatal, null, exception);
        }

        /// <summary>
        ///     Log a message object with the Fatal level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Fatal(this ILog source, string format, params object[] args)
        {
            Fatal(source, string.Format(CultureInfo.CurrentCulture, format, args));
        }


        /// <summary>
        ///     Log a message object with the Fatal level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Fatal(this ILog source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Fatal, message, exception);
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
        bool Log(LogLevel logLevel, string message, Exception exception = null);

        #endregion
    }

    /// <summary>
    ///     The log level.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The debug
        /// </summary>
        Debug,
        /// <summary>
        /// The information
        /// </summary>
        Info,
        /// <summary>
        /// The warn
        /// </summary>
        Warn,
        /// <summary>
        /// The error
        /// </summary>
        Error,
        /// <summary>
        /// The fatal
        /// </summary>
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
        ///     Gets a logger with the specified name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        ///     An instance of <see cref="ILog" />
        /// </returns>
        public static ILog GetLogger(string loggerName)
        {
            ILogProvider logProvider = ResolveLogProvider();
            return logProvider.GetLogger(loggerName);
        }

        /// <summary>
        ///     Sets the current log provider.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        public static void SetLogProvider(ILogProvider logProvider)
        {
            _LogProvider = logProvider;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Resolves the log providers.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="ILogProvider" />
        /// </returns>
        private static ILogProvider ResolveLogProvider()
        {
            return _LogProvider ?? new ApacheLogProvider();
        }

        #endregion
    }

    /// <summary>
    ///     A dynamic log that has a physical file locations.
    /// </summary>
    /// <seealso cref="System.Diagnostics.ILog" />
    public interface ILogFile : ILog
    {
        #region Public Methods

        /// <summary>
        ///     The path to the file used the by log.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///     Retunrns the path to the file.
        /// </returns>
        string GetPath(string name);

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
        LogLevel LogLevel { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the number of times the a log message with that log level has been captured.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of messages logged.
        /// </returns>
        int Captures(LogLevel logLevel);

        #endregion
    }
}