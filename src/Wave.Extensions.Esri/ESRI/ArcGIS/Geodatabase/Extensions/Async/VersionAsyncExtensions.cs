#if NET45
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IVersion" /> interface.
    /// </summary>
    public static class VersionAsyncExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the differences between the <paramref name="source" /> and <paramref name="target" /> versions that need to be
        ///     checked-in or exported.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        /// <param name="dataChangeTypes">The data change types.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DeltaRowCollection}" /> representing the differences for the table (or
        ///     key).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     target
        ///     or
        ///     dataChangeTypes
        /// </exception>
        public static Task<Dictionary<string, DeltaRowCollection>> GetDataChangesAsync(this IVersion source, IVersion target, params esriDataChangeType[] dataChangeTypes)
        {
            return Task.Run(() => source.GetDataChanges(target, dataChangeTypes));
        }

        /// <summary>
        ///     Gets the differences between the <paramref name="source" /> and <paramref name="target" /> versions that need to be
        ///     checked-in or exported.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        /// <param name="predicate">
        ///     The predicate that defines a set of criteria and determines whether the specified differences
        ///     will be loaded.
        /// </param>
        /// <param name="dataChangeTypes">The data change types.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DeltaRowCollection}" /> representing the differences for the table (or
        ///     key).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     target
        ///     or
        ///     predicate
        ///     or
        ///     dataChangeTypes
        /// </exception>
        public static Task<Dictionary<string, DeltaRowCollection>> GetDataChangesAsync(this IVersion source, IVersion target, Func<string, ITable, bool> predicate, params esriDataChangeType[] dataChangeTypes)
        {
            return Task.Run(() => source.GetDataChanges(target, predicate, dataChangeTypes));
        }

        /// <summary>
        ///     Gets the changes between the <paramref name="source" /> (or child) and <paramref name="table" /> (or parent)
        ///     version of the table.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="table">The table (or parent) version.</param>
        /// <param name="filter">The predicate used to filter the differences.</param>
        /// <param name="differenceTypes">The types of differences that are detected.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{DifferenceRow}" /> representing the differences for the table.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     table
        ///     or
        ///     differenceTypes
        /// </exception>
        public static Task<IEnumerable<DifferenceRow>> GetDifferencesAsync(this IVersionedTable source, ITable table, IQueryFilter filter, params esriDifferenceType[] differenceTypes)
        {
            return Task.Run(() => source.GetDifferences(table, filter, differenceTypes));
        }

        /// <summary>
        ///     Gets the differences between the <paramref name="source" /> and <paramref name="target" /> versions.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        /// <param name="filter">The predicate to filter the results.</param>
        /// <param name="differenceTypes">The type of differences that will be determined.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DifferenceRow}" /> representing the differences for the
        ///     table (or key).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     target
        ///     or
        ///     differenceTypes
        /// </exception>
        public static Task<Dictionary<string, IEnumerable<DifferenceRow>>> GetDifferencesAsync(this IVersion source, IVersion target, IQueryFilter filter, params esriDifferenceType[] differenceTypes)
        {
            return Task.Run(() => source.GetDifferences(target, filter, differenceTypes));
        }

        /// <summary>
        ///     Gets the differences between the <paramref name="source" /> and <paramref name="target" /> versions.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        /// <param name="filter">The predicate to filter the results.</param>
        /// <param name="predicate">
        ///     The predicate that defines a set of criteria and determines whether the specified differences
        ///     will be loaded.
        /// </param>
        /// <param name="differenceTypes">The type of differences that will be determined.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DifferenceRow}" /> representing the differences for the
        ///     table (or key).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     target
        ///     or
        ///     differenceTypes
        /// </exception>
        public static Task<Dictionary<string, IEnumerable<DifferenceRow>>> GetDifferencesAsync(this IVersion source, IVersion target, IQueryFilter filter, Func<string, ITable, bool> predicate, params esriDifferenceType[] differenceTypes)
        {
            return Task.Run(() => source.GetDifferences(target, filter, predicate, differenceTypes));
        }

        #endregion
    }
}

#endif