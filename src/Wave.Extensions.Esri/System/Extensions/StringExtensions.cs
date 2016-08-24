using System.Linq;

namespace System
{
    /// <summary>
    ///     Provides extensions for the <see cref="String" /> object.
    /// </summary>
    public static class StringExtensions
    {
        #region Public Methods

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

        #endregion
    }
}