using System;
using System.Collections.Generic;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMStoredDisplayManager" /> interface.
    /// </summary>
    public static class StoredDisplayManagerExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMStoredDisplayName" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumStoredDisplayName" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the layers from the input source.
        /// </returns>
        public static IEnumerable<IMMStoredDisplayName> AsEnumerable(this IMMEnumStoredDisplayName source)
        {
            source.Reset();

            IMMStoredDisplayName name;
            while ((name = source.Next()) != null)
            {
                yield return name;
            }
        }

        /// <summary>
        ///     Gets the unopened stored display with the specified type nad name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="storedDisplayType">Display type of the stored.</param>
        /// <param name="storedDisplayName">Display name of the stored.</param>
        /// <returns>Returns a <see cref="IMMStoredDisplay" /> representing the stored display; otherwise <c>null</c></returns>
        public static IMMStoredDisplay GetUnopenedStoredDisplay(this IMMStoredDisplayManager source, mmStoredDisplayType storedDisplayType, string storedDisplayName)
        {
            var items = source.GetStoredDisplayNames(storedDisplayType);

            foreach (var i in items.AsEnumerable())
            {
                if (i.Name.Equals(storedDisplayName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return source.GetUnopenedStoredDisplay(i);
                }
            }

            return null;
        }

        #endregion
    }
}