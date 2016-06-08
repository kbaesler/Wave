#if NET45
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IClass" /> and
    ///     <see cref="ESRI.ArcGIS.Geodatabase.IObjectClass" /> interfaces.
    /// </summary>
    public static class ClassAsyncExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Queries for the features that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object and projects the results into a new form.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the features must satisify.</param>
        /// <param name="selector">Projects each element of a sequence into a new form.</param>
        /// <returns>
        ///     Returns a <see cref="List{TResult}" /> representing the results of the query projected to the type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     filter
        ///     or
        ///     selector
        /// </exception>
        public static Task<IList<TResult>> FetchAsync<TResult>(this IFeatureClass source, IQueryFilter filter, Func<IFeature, TResult> selector)
        {
            return Task.Run(() => source.Fetch(filter, selector));
        }

        /// <summary>
        ///     Returns the row from the <paramref name="source" /> with the specified <paramref name="oid" /> when feature row
        ///     doesn't
        ///     exist it will return <c>null</c>.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="oid">The key for the feature in the table.</param>
        /// <returns>
        ///     Returns an <see cref="IFeature" /> representing the feature for the oid; otherwise <c>null</c>
        /// </returns>
        public static Task<IFeature> FetchAsync(this IFeatureClass source, int oid)
        {
            return Task.Run(() => source.Fetch(oid));
        }

        /// <summary>
        ///     Queries for the features that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the features must satisify.</param>
        /// <returns>
        ///     Returns a <see cref="List{IFeature}" /> representing the results of the query projected to the type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">filter</exception>
        public static Task<IList<IFeature>> FetchAsync(this IFeatureClass source, IQueryFilter filter)
        {
            return Task.Run(() => source.Fetch(filter));
        }

        /// <summary>
        ///     Queries for the features that have the specified object ids and projects the results into a new form.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">Projects each element of a sequence into a new form.</param>
        /// <param name="oids">The list of object ids.</param>
        /// <returns>
        ///     Returns a <see cref="List{TResult}" /> representing the results of the query projected to the type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     selector
        ///     or
        ///     oids
        /// </exception>
        public static Task<IList<TResult>> FetchAsync<TResult>(this IFeatureClass source, Func<IFeature, TResult> selector, params int[] oids)
        {
            return Task.Run(() => source.Fetch(selector, oids));
        }

        /// <summary>
        ///     Queries for the features that have the specified object ids.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oids">The list of object ids.</param>
        /// <returns>
        ///     Returns a <see cref="List{IFeature}" /> representing the features returned from the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">oids</exception>
        public static Task<IList<IFeature>> FetchAsync(this IFeatureClass source, params int[] oids)
        {
            return Task.Run(() => source.Fetch(oids));
        }

        /// <summary>
        ///     Queries for the all features and executes the specified <paramref name="action" /> on each feature returned from
        ///     the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        /// <remarks>
        ///     Uses a recycling cursors rehydrate a single feature object on each fetch and can be used to optimize read-only
        ///     access
        /// </remarks>
        public static Task<int> FetchAsync(this IFeatureClass source, Func<IFeature, bool> action)
        {
            return Task.Run(() => source.Fetch(action));
        }

        /// <summary>
        ///     Queries for the all features and executes the specified <paramref name="action" /> on each feature returned from
        ///     the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <param name="recycling">
        ///     if set to <c>true</c> when the cursor rehydrates a single row object on each fetch and can be
        ///     used to optimize read-only access.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        /// <remarks>
        ///     Uses a recycling cursors rehydrate a single feature object on each fetch and can be used to optimize read-only
        ///     access
        /// </remarks>
        public static Task<int> FetchAsync(this IFeatureClass source, Func<IFeature, bool> action, bool recycling)
        {
            return Task.Run(() => source.Fetch(action, recycling));
        }

        /// <summary>
        ///     Queries for the features that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each feature returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the features must satisify.</param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        public static Task<int> FetchAsync(this IFeatureClass source, IQueryFilter filter, Action<IFeature> action)
        {
            return Task.Run(() => source.Fetch(filter, action));
        }

        /// <summary>
        ///     Queries for the features that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each feature returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the features must satisify.</param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <param name="recycling">
        ///     if set to <c>true</c> when the cursor rehydrates a single row object on each fetch and can be
        ///     used to optimize read-only access.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        public static Task<int> FetchAsync(this IFeatureClass source, IQueryFilter filter, Action<IFeature> action, bool recycling)
        {
            return Task.Run(() => source.Fetch(filter, action, recycling));
        }

        /// <summary>
        ///     Queries for the features that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each feature returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the features must satisify.</param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        public static Task<int> FetchAsync(this IFeatureClass source, IQueryFilter filter, Func<IFeature, bool> action)
        {
            return Task.Run(() => source.Fetch(filter, action));
        }

        /// <summary>
        ///     Queries for the features that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each feature returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the features must satisify.</param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <param name="recycling">if set to <c>true</c> when a recycling memory for the features is used.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        public static Task<int> FetchAsync(this IFeatureClass source, IQueryFilter filter, Func<IFeature, bool> action, bool recycling)
        {
            return Task.Run(() => source.Fetch(filter, action, recycling));
        }

        #endregion
    }
}

#endif