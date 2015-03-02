using System;
using System.Linq;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.Process.IMMPxState" /> interface.
    /// </summary>
    public static class PxStateExtensions
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the specified <paramref name="state" /> exists in the enumeration of <paramref name="source" />.
        /// </summary>
        /// <param name="source">The enumeration of states.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Returns a <see cref="bool" /> representing <c>true</c> if the state exists; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">state</exception>
        public static bool Contains(this IMMEnumPxState source, IMMPxState state)
        {
            if (source == null) return false;
            if (state == null) throw new ArgumentNullException("state");

            return source.AsEnumerable().Any(testState => testState.Name == state.Name && testState.NodeType == state.NodeType);
        }

        #endregion
    }
}