using System.Collections.Generic;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for enumerations for the process framework.
    /// </summary>
    public static class PxEnumerableExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMPxNodeHistory" />
        /// </summary>
        /// <param name="source">An <see cref="IMMPxNodeHistory" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the fields from the input source.</returns>
        public static IEnumerable<IMMPxHistory> AsEnumerable(this IMMPxNodeHistory source)
        {
            if (source != null)
            {
                source.Reset();
                IMMPxHistory history = source.Next();
                while (history != null)
                {
                    yield return history;
                    history = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMPxTransition" />
        /// </summary>
        /// <param name="source">An <see cref="IMMPxTransition" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the fields from the input source.</returns>
        public static IEnumerable<IMMPxTransition> AsEnumerable(this IMMEnumPxTransition source)
        {
            if (source != null)
            {
                source.Reset();
                IMMPxTransition transition = source.Next();
                while (transition != null)
                {
                    yield return transition;
                    transition = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumPxState" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumPxState" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the fields from the input source.</returns>
        public static IEnumerable<IMMPxNode> AsEnumerable(this IMMPxNode source)
        {
            if (source != null)
            {
                while (source != null)
                {
                    source = ((ID8ListItem) source).ContainedBy as IMMPxNode;
                    yield return source;
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumPxState" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumPxState" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the fields from the input source.</returns>
        public static IEnumerable<IMMPxState> AsEnumerable(this IMMEnumPxState source)
        {
            if (source != null)
            {
                source.Reset();
                IMMPxState state = source.Next();
                while (state != null)
                {
                    yield return state;
                    state = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumPxTasks" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumPxTasks" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the fields from the input source.
        /// </returns>
        public static IEnumerable<IMMPxTask> AsEnumerable(this IMMEnumPxTasks source)
        {
            if (source != null)
            {
                source.Reset();
                IMMPxTask task = source.Next();
                while (task != null)
                {
                    yield return task;
                    task = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumPxUser" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumPxUser" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the fields from the input source.
        /// </returns>
        public static IEnumerable<IMMPxUser> AsEnumerable(this IMMEnumPxUser source)
        {
            if (source != null)
            {
                source.Reset();
                IMMPxUser user = source.Next();
                while (user != null)
                {
                    yield return user;
                    user = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumExtension" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumExtension" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the fields from the input source.
        /// </returns>
        public static IEnumerable<IMMExtension> AsEnumerable(this IMMEnumExtension source)
        {
            if (source != null)
            {
                source.Reset();
                IMMExtension extension = source.Next();
                while (extension != null)
                {
                    yield return extension;
                    extension = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumExtensionNames" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumExtensionNames" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the fields from the input source.
        /// </returns>
        public static IEnumerable<string> AsEnumerable(this IMMEnumExtensionNames source)
        {
            if (source != null)
            {
                source.Reset();
                string name = source.Next();
                while (name != null)
                {
                    yield return name;
                    name = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumExtensionNames" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumExtensionNames" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the fields from the input source.
        /// </returns>
        public static IEnumerable<string> AsEnumerable(this IMMEnumPxRole source)
        {
            if (source != null)
            {
                source.Reset();
                string role = source.Next();
                while (role != null)
                {
                    yield return role;
                    role = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMEnumPxFilter" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumPxFilter" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the fields from the input source.
        /// </returns>
        public static IEnumerable<IMMPxFilter> AsEnumerable(this IMMEnumPxFilter source)
        {
            if (source != null)
            {
                source.Reset();
                IMMPxFilter filter = source.Next();
                while (filter != null)
                {
                    yield return filter;
                    filter = source.Next();
                }
            }
        }

        #endregion
    }
}