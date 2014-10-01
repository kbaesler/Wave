using System.Collections.Generic;
using System.Linq;

namespace System.Collections
{
    /// <summary>
    ///     Provides extension methods for the .NET language for batching collections.
    /// </summary>
    public static class BatchExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Batches the source sequence into sized buckets.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in <paramref name="source" /> sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">Size of buckets.</param>
        /// <returns>A sequence of equally sized buckets containing elements of the source collection.</returns>
        /// <remarks> This operator uses deferred execution and streams its results (buckets and bucket content).</remarks>
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            return BatchImpl(source, size);
        }


        /// <summary>
        ///     Batches the source sequence into sized buckets and applies a projection to each bucket.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in <paramref name="source" /> sequence.</typeparam>
        /// <typeparam name="TResult">Type of result returned by <paramref name="selector" />.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">Size of buckets.</param>
        /// <param name="selector">The projection to apply to each bucket.</param>
        /// <returns>A sequence of projections on equally sized buckets containing elements of the source collection.</returns>
        /// <remarks> This operator uses deferred execution and streams its results (buckets and bucket content).</remarks>
        public static IEnumerable<TResult> Batch<TSource, TResult>(this IEnumerable<TSource> source, int size,
            Func<IEnumerable<TSource>, TResult> selector)
        {
            return BatchImpl(source, size, selector);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Batches the source sequence into sized buckets.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in <paramref name="source" /> sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">Size of buckets.</param>
        /// <returns>
        ///     A sequence of projections on equally sized buckets containing elements of the source collection.
        /// </returns>
        /// <remarks>
        ///     This operator uses deferred execution and streams its results (buckets and bucket content).
        /// </remarks>
        private static IEnumerable<IEnumerable<TSource>> BatchImpl<TSource>(
            this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }

        /// <summary>
        ///     Batches the source sequence into sized buckets and applies a projection to each bucket.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in <paramref name="source" /> sequence.</typeparam>
        /// <typeparam name="TResult">Type of result returned by <paramref name="selector" />.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">Size of buckets.</param>
        /// <param name="selector">The projection to apply to each bucket.</param>
        /// <returns>A sequence of projections on equally sized buckets containing elements of the source collection.</returns>
        /// <remarks> This operator uses deferred execution and streams its results (buckets and bucket content).</remarks>
        private static IEnumerable<TResult> BatchImpl<TSource, TResult>(this IEnumerable<TSource> source, int size,
            Func<IEnumerable<TSource>, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, @"The size must be greater than 0.");

            if (selector == null)
                throw new ArgumentNullException("selector");

            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                {
                    bucket = new TSource[size];
                }

                bucket[count++] = item;

                // The bucket is fully buffered before it's yielded
                if (count != size)
                {
                    continue;
                }

                // Select is necessary so bucket contents are streamed too
                yield return selector(bucket.Select(x => x));

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
            {
                yield return selector(bucket.Take(count));
            }
        }

        #endregion
    }
}