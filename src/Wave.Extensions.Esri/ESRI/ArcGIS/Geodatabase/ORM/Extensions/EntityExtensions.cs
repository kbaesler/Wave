using System;
using System.Collections.Generic;
using System.Linq;
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
        ///     Gets the feature class based on the entity type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The entity type (which must inherit from <see cref="Entity" />).</param>
        /// <returns>Returns a <see cref="IFeatureClass" /> representing the table for the entity type.</returns>
        /// <exception cref="System.ArgumentNullException">type - The type must be assigned the 'EntityTableAttribute'</exception>
        public static IFeatureClass GetFeatureClass(this IWorkspace source, Type type)
        {
            return source.GetTable(type) as IFeatureClass;
        }

        /// <summary>
        ///     Gets the table based on the entity type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The entity type (which must inherit from <see cref="Entity" />).</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table for the entity type.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The type must be assiable from the Entity class</exception>
        /// <exception cref="System.ArgumentNullException">type - The type must be assigned the 'EntityTableAttribute'</exception>
        public static ITable GetTable(this IWorkspace source, Type type)
        {
            if (!typeof(Entity).IsAssignableFrom(type))
                throw new ArgumentOutOfRangeException("type", @"The object must be assignable from the Entity class");

            var attribute = type.GetCustomAttributes(typeof(EntityTableAttribute), true).OfType<EntityTableAttribute>().SingleOrDefault();
            if (attribute == null)
                throw new ArgumentNullException("type", @"The object must be assigned the EntityTableAttribute attribute.");

            return source.GetTable(attribute.TableName);
        }

        /// <summary>
        ///     Inserts multiple items into a table using an insert cursor.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> array representing the object ids of the inserted records.
        /// </returns>
        public static int[] Insert<TEntity>(this ITable source, IEnumerable<TEntity> items) where TEntity : Entity
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
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        /// <returns>Returns a <see cref="int" /> array representing the object ids of the inserted records.</returns>
        public static int[] Insert<TEntity>(this IFeatureClass source, IEnumerable<TEntity> items) where TEntity : Entity
        {
            return ((ITable)source).Insert(items);
        }

        /// <summary>
        ///     Reads database rows as a (lazily-evaluated) sequence of objects that are converted into the entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{TEntity}" /> representing the entity object.
        /// </returns>
        public static IEnumerable<TEntity> Map<TEntity>(this ITable source, IQueryFilter filter) where TEntity : Entity
        {
            var cursor = source.Search(filter, false);

            try
            {
                foreach (var row in cursor.AsEnumerable())
                {
                    yield return row.ToEntity<TEntity>();
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
        ///     Reads database feature as a (lazily-evaluated) sequence of objects that are converted into the entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{TEntity}" /> representing the entity object.
        /// </returns>
        public static IEnumerable<TEntity> Map<TEntity>(this IFeatureClass source, IQueryFilter filter) where TEntity : Entity
        {
            return ((ITable)source).Map<TEntity>(filter);
        }

        /// <summary>
        ///     Converts the row into the entity object equivalent.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="TEntity" /> representing the entity object
        /// </returns>
        public static TEntity ToEntity<TEntity>(this IRow source) where TEntity : Entity
        {
            return Entity.Create<TEntity>(source);
        }

        #endregion
    }
}