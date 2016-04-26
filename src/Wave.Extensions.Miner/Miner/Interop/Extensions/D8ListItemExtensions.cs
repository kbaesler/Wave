using System;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ID8ListItem" />
    /// </summary>
    public static class D8ListItemExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the parent that matches the specified predicate
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a type of the representing the parent.
        /// </returns>
        public static TValue GetParent<TValue>(this ID8ListItem source)
            where TValue : class
        {
            return source.GetParent<TValue>(value => true);
        }

        /// <summary>
        ///     Gets the parent that matches the specified predicate
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate used to determine if the value should be used.</param>
        /// <returns>
        ///     Returns a a type of the representing the parent.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static TValue GetParent<TValue>(this ID8ListItem source, Predicate<TValue> predicate)
            where TValue : class
        {
            if (source == null) return null;

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            for (var i = source.ContainedBy as ID8ListItem; i != null; i = i.ContainedBy as ID8ListItem)
            {
                var value = i as TValue;
                if (value != null && predicate(value)) return value;
            }

            return null;
        }

        #endregion
    }
}