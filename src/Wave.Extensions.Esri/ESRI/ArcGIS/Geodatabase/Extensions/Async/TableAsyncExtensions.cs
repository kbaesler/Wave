using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.ITable" /> interface.
    /// </summary>
    public static class TableAsyncExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Queries for the rows that have the specified object ids.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oids">The list of object ids.</param>
        /// <returns>
        ///     Returns a <see cref="List{IRow}" /> representing the rows returned from the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">oids</exception>
        public static IList<IRow> FetchAsync(this ITable source, params int[] oids)
        {
            return Task.Wait(() => source.Fetch(oids));
        }

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
        /// <exception cref="System.ArgumentNullException">selector</exception>
        public static IList<TResult> FetchAsync<TResult>(this ITable source, IQueryFilter filter, Func<IRow, TResult> selector)
        {
            return Task.Wait(() => source.Fetch(filter, selector));
        }

        /// <summary>
        ///     Queries for the rows that have the specified object ids and projects the results into a new form.
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
        public static IList<TResult> FetchAsync<TResult>(this ITable source, Func<IRow, TResult> selector, params int[] oids)
        {
            return Task.Wait(() => source.Fetch(selector, oids));
        }

        /// <summary>
        ///     Queries for the rows that satisfy the attribute query as specified by an <paramref name="filter" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute requirement that the rows must satisify.</param>
        /// <returns>
        ///     Returns a <see cref="List{IRow}" /> representing the rows returned from the query.
        /// </returns>
        public static IList<IRow> FetchAsync(this ITable source, IQueryFilter filter)
        {
            return Task.Wait(() => source.Fetch(filter));
        }

        /// <summary>
        ///     Queries for the rows that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each feature returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the rows must satisify.</param>
        /// <param name="action">The action to take for each row in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        public static int FetchAsync(this ITable source, IQueryFilter filter, Func<IRow, bool> action)
        {
            return Task.Wait(() => source.Fetch(filter, action));
        }

        /// <summary>
        ///     Queries for the rows that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each feature returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the rows must satisify.</param>
        /// <param name="action">The action to take for each row in the cursor.</param>
        /// <param name="recycling">
        ///     if set to <c>true</c> when the cursor rehydrates a single row object on each fetch and can be
        ///     used to optimize read-only access.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        public static int FetchAsync(this ITable source, IQueryFilter filter, Func<IRow, bool> action, bool recycling)
        {
            return Task.Wait(() => source.Fetch(filter, action, recycling));
        }

        /// <summary>
        ///     Queries for the rows that satisfy the attribute query as specified by an <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each row returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute requirement that features must satisify.</param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of rows affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        /// <remarks>
        ///     Uses a recycling cursors rehydrate a single feature object on each fetch and can be used to optimize read-only
        ///     access
        /// </remarks>
        public static int FetchAsync(this ITable source, IQueryFilter filter, Action<IRow> action)
        {
            return Task.Wait(() => source.Fetch(filter, action));
        }


        /// <summary>
        ///     Returns the row from the <paramref name="source" /> with the specified <paramref name="oid" /> when the row doesn't
        ///     exist it will return <c>null</c>.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="oid">The key for the row in the table.</param>
        /// <returns>
        ///     Returns an <see cref="IRow" /> representing the row for the oid; otherwise <c>null</c>
        /// </returns>
        public static IRow FetchAsync(this ITable source, int oid)
        {
            return Task.Wait(() => source.Fetch(oid));
        }

        /// <summary>
        ///     Queries for the rows that satisfy the attribute query as specified by an <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each row returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute requirement that features must satisify.</param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <param name="recycling">
        ///     if set to <c>true</c> when the cursor rehydrates a single row object on each fetch and can be
        ///     used to optimize read-only access.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of rows affected by the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="ArgumentNullException">action</exception>
        /// <remarks>
        ///     Uses a recycling cursors rehydrate a single feature object on each fetch and can be used to optimize read-only
        ///     access
        /// </remarks>
        public static int FetchAync(this ITable source, IQueryFilter filter, Action<IRow> action, bool recycling)
        {
            return Task.Wait(() => source.Fetch(filter, action, recycling));
        }

        #endregion
    }
}