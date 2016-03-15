using System.Collections.Generic;
using System.Linq;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IDictionary" /> interface.
    /// </summary>
    public static class DictionaryExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the <see cref="IDictionary" /> into a <see cref="Dictionary{String, Object}" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="Dictionary{String, Object}" /> representing the dictionary.</returns>
        public static Dictionary<string, object> ToDictionary(this IDictionary source)
        {
            object[] keys = (object[]) source.Keys();
            return keys.ToDictionary(key => (string) key, key => source.get_Item(ref key));
        }

        #endregion
    }
}