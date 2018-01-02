using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    ///     Provides extensions for the <see cref="String" /> object.
    /// </summary>
    public static class StringExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Determines whether a specific character string matches a specified pattern.
        /// </summary>
        /// <param name="source">The string to be searched for a match.</param>
        /// <param name="pattern">The pattern, which may include wildcard characters, to match in <paramref name="source" />.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if <paramref name="source" /> matches the
        ///     <paramref name="pattern" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Like(this string source, string pattern)
        {
            /* Turn "off" all regular expression related syntax in
            * the pattern string. */
            var expression = Regex.Escape(pattern);

            /* Replace the SQL LIKE wildcard metacharacters with the
            * equivalent regular expression metacharacters. */
            expression = expression.Replace("%", ".*?").Replace("_", ".");

            /* The previous call to Regex.Escape actually turned off
            * too many metacharacters, i.e. those which are recognized by
            * both the regular expression engine and the SQL LIKE
            * statement ([...] and [^...]). Those metacharacters have
            * to be manually unescaped here. */
            expression = expression.Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\^", "^");

            return Regex.IsMatch(source, expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        ///     Repeats the specified string n-times.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="n">The number of times to repeat.</param>
        /// <returns>Returns a <see cref="string" /> representing the repeated string.</returns>
        public static string Repeat(this string source, int n)
        {
            return new String(Enumerable.Range(0, n).SelectMany(x => source).ToArray());
        }

        /// <summary>
        ///     Repeats the specified character n-times.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="n">The number of times to repeat.</param>
        /// <returns>Returns a <see cref="string" /> representing the repeated character.</returns>
        public static string Repeat(this char source, int n)
        {
            return new String(source, n);
        }

        /// <summary>
        ///     Creates a secure string for the value.
        /// </summary>
        /// <param name="source">The value.</param>
        /// <returns>Returns a <see cref="SecureString" /> representing the string.</returns>
        public static SecureString Secure(this string source)
        {
            return source.ToCharArray()
                .Aggregate(new SecureString()
                    , (s, c) =>
                    {
                        s.AppendChar(c);
                        return s;
                    }
                    , (s) =>
                    {
                        s.MakeReadOnly();
                        return s;
                    }
                );
        }

        /// <summary>
        ///     Unprotects the specified secure string into it's plain text.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="String" /> representing the plain-text string.</returns>
        public static string ToString(this SecureString source)
        {
            if (source == null) return "";
            if (source.Length == 0) return "";

            IntPtr pointer = IntPtr.Zero;
            
            try
            {
                pointer = Marshal.SecureStringToGlobalAllocUnicode(source);
                return Marshal.PtrToStringUni(pointer);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(pointer);
            }
        }

        #endregion
    }
}