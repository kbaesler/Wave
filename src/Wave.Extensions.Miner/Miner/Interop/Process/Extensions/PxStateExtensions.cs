namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.Process.IMMPxState" /> interface.
    /// </summary>
    public static class PxStateExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <paramref name="state" /> exists in the enumeration of <paramref name="source" />.
        /// </summary>
        /// <param name="source">The enumeration of states.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        ///     <c>true</c> if the state exists; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this IMMEnumPxState source, IMMPxState state)
        {
            foreach (var testState in source.AsEnumerable())
            {
                if (testState.Name == state.Name && testState.NodeType == state.NodeType)
                    return true;
            }

            return false;
        }

        #endregion
    }
}