using System.Collections.Generic;

using log4net.Appender;

namespace System.Diagnostics
{
    /// <summary>
    ///     Provides management of the log scopes
    /// </summary>
    public static class LogScopeManager
    {
        #region Fields

        private static readonly Dictionary<string, int> References = new Dictionary<string, int>();
        private static readonly Dictionary<string, LogScope> Scopes = new Dictionary<string, LogScope>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds the scope.
        /// </summary>
        /// <param name="appender">The appender.</param>
        /// <returns></returns>
        public static LogScope AddScope(IAppender appender)
        {
            LogScope ls = FindScope(appender);
            if (ls != null)
            {
                ls = Scopes[ls.Name];
                References[ls.Name]++;
            }
            else
            {
                ls = new LogScope(appender);
                Scopes.Add(ls.Name, ls);
                References.Add(ls.Name, 1);

                Log.AddAppender(appender);
            }

            return ls;
        }

        /// <summary>
        /// Adds the scope.
        /// </summary>
        /// <param name="logScope">The log scope.</param>
        public static void AddScope(LogScope logScope)
        {
            LogScope ls = FindScope(logScope.Name);
            if (ls != null)
            {
                References[ls.Name]++;
            }
            else
            {
                Scopes.Add(logScope.Name, logScope);
                References.Add(logScope.Name, 1);
            }
        }

        /// <summary>
        ///     Determines whether the specified appender contains scope.
        /// </summary>
        /// <param name="appender">The appender.</param>
        /// <returns>
        ///     <c>true</c> if the specified appender contains scope; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsScope(IAppender appender)
        {
            return ContainsScope(appender.Name);
        }

        /// <summary>
        ///     Determines whether the specified appender name contains scope.
        /// </summary>
        /// <param name="appenderName">Name of the appender.</param>
        /// <returns>
        ///     <c>true</c> if the specified appender name contains scope; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsScope(string appenderName)
        {
            return References.ContainsKey(appenderName) && Scopes.ContainsKey(appenderName);
        }

        /// <summary>
        ///     Finds the scope.
        /// </summary>
        /// <param name="appenderName">Name of the appender.</param>
        /// <returns></returns>
        public static LogScope FindScope(string appenderName)
        {
            if (ContainsScope(appenderName))
            {
                return Scopes[appenderName];
            }

            return null;
        }

        /// <summary>
        ///     Finds the scope.
        /// </summary>
        /// <param name="appender">The appender.</param>
        /// <returns></returns>
        public static LogScope FindScope(IAppender appender)
        {
            return FindScope(appender.Name);
        }

        /// <summary>
        ///     Removes the scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public static void RemoveScope(LogScope scope)
        {
            if (ContainsScope(scope.Name))
            {
                if (References[scope.Name] <= 1)
                {
                    References.Remove(scope.Name);
                    Scopes.Remove(scope.Name);

                    Log.RemoveAppender(scope.Name);
                }
                else
                {
                    References[scope.Name] = References[scope.Name] - 1;
                }
            }
        }

        #endregion
    }
}