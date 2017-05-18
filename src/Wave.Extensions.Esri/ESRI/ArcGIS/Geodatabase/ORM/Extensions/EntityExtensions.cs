using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for tables and feature classes.
    /// </summary>
    public static class EntityExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Inserts multiple items into a table using an insert cursor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static int[] Insert<T>(this ITable source, IEnumerable<T> items) where T : Entity
        {
            var cursor = source.Insert(true);

            try
            {
                var oids = new List<int>();

                foreach (var item in items)
                {
                    if (oids.Contains(item.OID))
                        break;

                    var buffer = source.CreateRowBuffer();

                    item.CopyTo(buffer);

                    oids.Add((int)cursor.InsertRow(buffer));
                }

                return oids.ToArray();
            }
            finally
            {
                while (Marshal.ReleaseComObject(cursor) != 0)
                {
                }
            }
        }

        /// <summary>
        ///     Inserts multiple items into a table using an insert cursor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static int[] Insert<T>(this IFeatureClass source, IEnumerable<T> items) where T : Entity
        {
            return ((ITable)source).Insert(items);
        }

        /// <summary>
        /// Reads database rows as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// Returns a <see cref="IEnumerable{T}" /> representing the entity object.
        /// </returns>
        public static IEnumerable<T> Map<T>(this ITable source, IQueryFilter filter) where T : Entity
        {
            var cursor = source.Search(filter, false);

            try
            {
                foreach (var row in cursor.AsEnumerable())
                {
                    yield return Entity.Create<T>(row);
                }
            }
            finally
            {
                while (Marshal.ReleaseComObject(cursor) != 0)
                {
                }
            }
        }

        /// <summary>
        ///     Reads database rows as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IFeatureClass source, IQueryFilter filter) where T : Entity
        {
            return ((ITable)source).Map<T>(filter);
        }

        #endregion
    }
}