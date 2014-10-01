using System.Linq;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMPxTransition" /> interface
    /// </summary>
    public static class PxTransitionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Determines if a transition exists between the <paramref name="fromState" /> and
        ///     the <paramref name="toState" /> states for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="source">The transitions.</param>
        /// <param name="user">The user.</param>
        /// <param name="fromState">The From state.</param>
        /// <param name="toState">The To state.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if there exists a transition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidTransition(this IMMPxTransition source, IMMPxUser user, IMMPxState fromState, IMMPxState toState)
        {
            // Compare the from and to states from the enumerations.
            if (source.FromStates.Contains(fromState) && source.ToStates.Contains(toState))
            {
                // Found a transition with matching To/From states, now check to see
                // if the user has the proper role assigned to run the transition.
                if (user.AnyRoleForTransition(source))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines if a transition exists between the <paramref name="fromState" /> and
        ///     the <paramref name="toState" /> states for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="source">The transitions.</param>
        /// <param name="user">The user.</param>
        /// <param name="fromState">The From state.</param>
        /// <param name="toState">The To state.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if there exists a transition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidTransition(this IMMEnumPxTransition source, IMMPxUser user, IMMPxState fromState, IMMPxState toState)
        {
            return source.AsEnumerable().Any(transition => transition.IsValidTransition(user, fromState, toState));
        }

        #endregion
    }
}