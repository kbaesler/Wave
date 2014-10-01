namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.Process.IMMPxUser" /> interface.
    /// </summary>
    public static class PxUserExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Determines whether the given <paramref name="source" /> has at least one matching
        ///     role for the <paramref name="transition" />.
        /// </summary>
        /// <param name="source">The user.</param>
        /// <param name="transition">The transition.</param>
        /// <returns>
        ///     <c>true</c> if the user has at least one matching role with the transition; otherwise, <c>false</c>.
        /// </returns>
        public static bool AnyRoleForTransition(this IMMPxUser source, IMMPxTransition transition)
        {
            foreach (var userRole in source.Roles.AsEnumerable())
            {
                foreach (var transitionRole in transition.Roles.AsEnumerable())
                {
                    if (userRole == transitionRole)
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}