using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using log4net;
using log4net.Appender;
using log4net.Core;

namespace System.Diagnostics
{
    /// <summary>
    ///     Provides methods for logging at different levels using the Apache log4net framework.
    /// </summary>
    public static class Log
    {
        #region Constants

        /// <summary>
        ///     The name of the log configuration file.
        /// </summary>
        public const string FileName = "Wave.log4net.config";
      
        #endregion

        #region Fields

        private static readonly object Lock = new object();
        private static readonly Dictionary<Type, ILog> Loggers = new Dictionary<Type, ILog>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds the appender.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="appender">The appender.</param>
        public static void AddAppender(object source, IAppender appender)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            AddAppender(source.GetType(), appender);
        }

        /// <summary>
        ///     Adds the appender.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="appender">The appender.</param>
        public static void AddAppender(Type source, IAppender appender)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            IAppenderAttachable appenderAttachable = logger.Logger as IAppenderAttachable;
            if (appenderAttachable != null) appenderAttachable.AddAppender(appender);
        }


        /// <summary>
        ///     Log a message object with the Debug level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Debug(Type source, string message)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message);
            }
        }

        /// <summary>
        ///     Log a message object with the Debug level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Debug(object source, string format, params object[] args)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Debug(source.GetType(), string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Log a message object with the Debug level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Debug(Type source, string format, params object[] args)
        {
            Debug(source, string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Log a message object with the Debug level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Debug(object source, string message)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Debug(source.GetType(), message);
        }

        /// <summary>
        ///     Log a message object with the Debug level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Debug(Type source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message, exception);
            }
        }

        /// <summary>
        ///     Log a message object with the Debug level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Debug(object source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Debug(source.GetType(), message, exception);
        }

        /// <summary>
        ///     Log a message object with the Error level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(object source, Exception exception)
        {
            Error(source, exception.Message, exception);
        }

        /// <summary>
        ///     Log a message object with the Error level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(Type source, Exception exception)
        {
            Error(source, exception.Message, exception);
        }

        /// <summary>
        ///     Log a message object with the Error level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(Type source, string message)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            if (logger.IsErrorEnabled)
            {
                logger.Error(message);
            }
        }

        /// <summary>
        ///     Log a message object with the Error level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(object source, string format, params object[] args)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Error(source.GetType(), string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Log a message object with the Error level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Error(Type source, string format, params object[] args)
        {
            Error(source, string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Log a message object with the Error level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(object source, string message)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Error(source.GetType(), message);
        }

        /// <summary>
        ///     Log a message object with the Error level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(Type source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            if (logger.IsErrorEnabled)
            {
                logger.Error(message, exception);
            }
        }

        /// <summary>
        ///     Log a message object with the Error level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(object source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Error(source.GetType(), message, exception);
        }

        /// <summary>
        ///     Log a message object with the Error level including the stack trace of the exception
        ///     and displays a message box to the user with the contents of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="owner">The owner of the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(Type source, IWin32Window owner, string title, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Error(source, title, exception);
            MessageBox.Show(owner, exception.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        ///     Log a message object with the Error level including the stack trace of the exception
        ///     and displays a message box to the user with the contents of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="owner">The owner of the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Error(object source, IWin32Window owner, string title, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Error(source.GetType(), owner, title, exception);
        }

        /// <summary>
        ///     Returns the appenders that exist in the repository of the given type.
        /// </summary>
        /// <typeparam name="TAppender">The type of the appender.</typeparam>
        /// <param name="selector">A function used to select the appenders.</param>
        /// <returns>
        ///     Returns an enumeration of <see cref="IAppender" /> interfaces.
        /// </returns>
        public static IEnumerable<TAppender> GetAppenders<TAppender>(Func<TAppender, bool> selector)
            where TAppender : IAppender
        {
            return LogManager.GetAllRepositories().SelectMany(o => o.GetAppenders()).OfType<TAppender>().Where(selector);
        }

        /// <summary>
        ///     Log a message object with the Info level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Info(Type source, string message)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
            }
        }

        /// <summary>
        ///     Log a message object with the Info level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Info(object source, string format, params object[] args)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Info(source.GetType(), string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Log a message object with the Info level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Info(Type source, string format, params object[] args)
        {
            Info(source, string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Log a message object with the Info level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Info(object source, string message)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Info(source.GetType(), message);
        }

        /// <summary>
        ///     Log a message object with the Info level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Info(Type source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            if (logger.IsInfoEnabled)
            {
                logger.Info(message, exception);
            }
        }

        /// <summary>
        ///     Log a message object with the Info level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Info(object source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Info(source.GetType(), message, exception);
        }

        /// <summary>
        ///     Removes the appender.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="appender">The appender.</param>
        public static void RemoveAppender(object source, IAppender appender)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            RemoveAppender(source.GetType(), appender);
        }

        /// <summary>
        ///     Removes the appender.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="appender">The appender.</param>
        public static void RemoveAppender(Type source, IAppender appender)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            IAppenderAttachable appenderAttachable = logger.Logger as IAppenderAttachable;
            if (appenderAttachable != null) appenderAttachable.RemoveAppender(appender);
        }

        /// <summary>
        ///     Log a message object with the Warn level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Warn(Type source, string message)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message);
            }
        }

        /// <summary>
        ///     Log a message object with the Warn level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Warn(object source, string format, params object[] args)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Warn(source.GetType(), string.Format(format, args));
        }

        /// <summary>
        ///     Log a message object with the Warn level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Warn(Type source, string format, params object[] args)
        {
            Warn(source, string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Log a message object with the Warn level.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Warn(object source, string message)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Warn(source.GetType(), message);
        }

        /// <summary>
        ///     Log a message object with the Warn level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Warn(Type source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ILog logger = GetLogger(source);
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message, exception);
            }
        }

        /// <summary>
        ///     Log a message object with the Warn level including the stack trace of the exception.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void Warn(object source, string message, Exception exception)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Warn(source.GetType(), message, exception);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the logger from the dictionary.
        /// </summary>
        /// <param name="source">The source of the logger.</param>
        /// <returns>Returns the <see cref="ILog" /> representing the logger for the source.</returns>
        private static ILog GetLogger(Type source)
        {
            lock (Lock)
            {
                if (Loggers.ContainsKey(source))
                {
                    return Loggers[source];
                }

                ILog logger = LogManager.GetLogger(source);
                Loggers.Add(source, logger);
                return logger;
            }
        }

        #endregion
    }
}