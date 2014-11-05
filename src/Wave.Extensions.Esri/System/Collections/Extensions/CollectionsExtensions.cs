using System.Collections.Generic;

namespace System.Collections
{
    /// <summary>
    ///     A static class providing extension methods for working with the collections tree.
    /// </summary>
    public static class CollectionsExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the <paramref name="collection" /> to the <paramref name="source" /> collection.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The items.</param>
        /// <param name="collection">The collection.</param>
        public static void AddRange<TSource>(this ICollection<TSource> source, IEnumerable<TSource> collection)
        {
            foreach (var item in collection)
            {
                source.Add(item);
            }
        }

        /// <summary>
        ///     Performs that specified <paramref name="action" /> on all of the items prior to clearing the collection.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        public static void Clear<TSource>(this ICollection<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
            {
                action(item);
            }

            source.Clear();
        }

        /// <summary>
        ///     Returns true if <paramref name="source" /> has no items in it; otherwise, falSE.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     <c>true</c> if the specified source is empty; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If an <see cref="ICollection{TSource}" /> is provided,
        ///         <see cref="ICollection{TSource}.Count" /> is used.
        ///     </para>
        ///     <para>
        ///         Yes, this does basically the same thing as the
        ///         <see cref="System.Linq.Enumerable.Any{TSource}(IEnumerable{TSource})" /> extension. The differences: 'IsEmpty'
        ///         is easier to remember and it leverages
        ///         <see cref="ICollection{TSource}.Count">ICollection.Count</see> if it exists.
        ///     </para>
        /// </remarks>
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            ICollection<TSource> o = source as ICollection<TSource>;
            if (o != null)
            {
                return o.Count == 0;
            }

            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                return !enumerator.MoveNext();
            }
        }

        /// <summary>
        ///     Removes each element from the <paramref name="source" /> if it satisfies the specific <paramref name="predicate" />
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The list from which to remove the elements that satisfy the <paramref name="predicate" />.</param>
        /// <param name="predicate">
        ///     The predicate that defines a set of criteria and determines whether the specified object meets
        ///     those criteria.
        /// </param>
        public static void Remove<TSource>(this IList<TSource> source, Predicate<TSource> predicate)
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (predicate(source[i]))
                    source.RemoveAt(i);
            }
        }

        /// <summary>
        ///     Removes the last element from <paramref name="source" />.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The list from which to remove the last element.</param>
        /// <returns>
        ///     The last element.
        /// </returns>
        /// <remarks>
        ///     <paramref name="source" /> must have at least one element and allow changes.
        /// </remarks>
        public static TSource RemoveLast<TSource>(this IList<TSource> source)
        {
            TSource item = source[source.Count - 1];
            source.RemoveAt(source.Count - 1);

            return item;
        }

        #endregion
    }
}