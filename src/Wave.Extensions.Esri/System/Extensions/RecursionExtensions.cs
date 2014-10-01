using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    ///     Provides extension methods for the
    ///     <see>
    ///         <cref>T:System.IRecursion{TValue}</cref>
    ///     </see>
    ///     interface.
    /// </summary>
    public static class RecursionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IRecursion<TSource>> SelectRecursive<TSource>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> selector)
        {
            return SelectRecursive(source, selector, null);
        }

        /// <summary>
        ///     Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of
        ///     invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IRecursion<TSource>> SelectRecursive<TSource>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> selector, Func<IRecursion<TSource>, bool> predicate)
        {
            return SelectRecursiveImpl(source, selector, predicate, 0);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        private static IEnumerable<IRecursion<TSource>> SelectRecursiveImpl<TSource>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> selector, Func<IRecursion<TSource>, bool> predicate, int depth)
        {
            var items = source
                .Select(item => new Recursion<TSource>(depth, item))
                .Cast<IRecursion<TSource>>();

            if (predicate != null)
            {
                items = items.Where(predicate);
            }

            foreach (var item in items)
            {
                yield return item;
                foreach (var child in SelectRecursiveImpl(selector(item.Value), selector, predicate, depth + 1))
                    yield return child;
            }
        }

        #endregion
    }
}