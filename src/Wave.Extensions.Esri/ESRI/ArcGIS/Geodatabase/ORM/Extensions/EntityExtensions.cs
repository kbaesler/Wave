using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for tables and feature classes.
    /// </summary>
    public static class EntityExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Determines whether the workspace contains the table name and type combination.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the workspace contains the table name and type.
        /// </returns>
        public static bool Contains<T>(this IWorkspace source, esriDatasetType type) where T : Entity
        {
            return source.Contains(type, Entity.GetFullName<T>());
        }


        /// <summary>
        ///     Gets the table based on the entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the table for the entity type.
        /// </returns>
        public static IFeatureClass GetFeatureClass<T>(this IWorkspace source) where T : Entity
        {
            return source.GetFeatureClass(Entity.GetFullName<T>());
        }

        /// <summary>
        ///     Gets the table based on the entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the table for the entity type.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The type must be assiable from the Entity class</exception>
        /// <exception cref="System.ArgumentNullException">type - The type must be assigned the 'EntityTableAttribute'</exception>
        public static IFeatureClass GetFeatureClass<T>(this IVersion source) where T : Entity
        {
            return ((IWorkspace) source).GetFeatureClass<T>();
        }

        /// <summary>
        ///     Gets the relationship based on the entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IRelationshipClass" /> representing the table for the entity type.
        /// </returns>
        public static IRelationshipClass GetRelationshipClass<T>(this IWorkspace source) where T : Entity
        {
            return source.GetRelationshipClass(Entity.GetFullName<T>());
        }

        /// <summary>
        ///     Gets the relationship based on the entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IRelationshipClass" /> representing the table for the entity type.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The type must be assiable from the Entity class</exception>
        /// <exception cref="System.ArgumentNullException">type - The type must be assigned the 'EntityTableAttribute'</exception>
        public static IRelationshipClass GetRelationshipClass<T>(this IVersion source) where T : Entity
        {
            return ((IWorkspace) source).GetRelationshipClass<T>();
        }

        /// <summary>
        ///     Gets the table based on the entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the table for the entity type.
        /// </returns>
        public static ITable GetTable<T>(this IWorkspace source) where T : Entity
        {
            return source.GetTable(Entity.GetFullName<T>());
        }

        /// <summary>
        ///     Gets the table based on the entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the table for the entity type.
        /// </returns>
        public static ITable GetTable<T>(this IVersion source) where T : Entity
        {
            return source.GetTable(Entity.GetFullName<T>());
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

                    oids.Add((int) cursor.InsertRow(buffer));
                }

                if (oids.Any())
                {
                    cursor.Flush();
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
        public static int[] Insert<TEntity>(this IFeatureClass source, params TEntity[] items) where TEntity : Entity
        {
            return ((ITable) source).Insert(items);
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
        public static int[] InsertAsync<TEntity>(this ITable source, IEnumerable<TEntity> items) where TEntity : Entity
        {
            var cursor = source.Insert(true);

            try
            {
                var oids = new List<int>();

                foreach (var item in items)
                {
                    var buffer = source.CreateRowBuffer();

                    item.CopyTo(buffer);

                    oids.Add((int) cursor.InsertRow(buffer));
                }

                if (oids.Any())
                {
                    cursor.Flush();
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
        ///     Reads database rows as a (lazily-evaluated) sequence of objects that are converted into the entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="workspace">The workspace.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{TEntity}" /> representing the entity object.
        /// </returns>
        public static IEnumerable<TEntity> Map<TEntity>(this IWorkspace workspace, string commandText) where TEntity : Entity
        {
            var cursor = ((ISqlWorkspace) workspace).OpenQueryCursor(commandText);
            return cursor.Map<TEntity>();
        }

        /// <summary>
        ///     Reads database rows as a (lazily-evaluated) sequence of objects that are converted into the entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{TEntity}" /> representing the entity object.
        /// </returns>
        public static IEnumerable<TEntity> Map<TEntity>(this ICursor source) where TEntity : Entity
        {
            foreach (var row in source.AsEnumerable())
            {
                yield return row.ToEntity<TEntity>();
            }
        }

        /// <summary>
        ///     Reads database rows as a (lazily-evaluated) sequence of objects that are converted into the entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{TEntity}" /> representing the entity object.
        /// </returns>
        public static IEnumerable<TEntity> Map<TEntity>(this IQueryDef source) where TEntity : Entity
        {
            var cursor = source.Evaluate();

            try
            {
                foreach (var row in cursor.Map<TEntity>())
                    yield return row;
            }
            finally
            {
                while (Marshal.ReleaseComObject(cursor) != 0)
                {
                }
            }
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
            return ((ITable) source).Map<TEntity>(filter);
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