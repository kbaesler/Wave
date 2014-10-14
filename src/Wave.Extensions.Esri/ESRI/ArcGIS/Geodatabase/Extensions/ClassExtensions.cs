using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IClass" /> and
    ///     <see cref="ESRI.ArcGIS.Geodatabase.IObjectClass" /> interfaces.
    /// </summary>
    public static class ClassExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Queries for the features that satisfies the attribute query as specified by an
        ///     <paramref name="whereClause" /> statement.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="whereClause">The where clause for the attribute query.</param>
        /// <returns>
        ///     Returns a <see cref="List{IFeature}" /> representing the features returned from the query.
        /// </returns>
        public static List<IFeature> Fetch(this IFeatureClass source, string whereClause)
        {
            var filter = new QueryFilterClass
            {
                WhereClause = whereClause
            };

            return source.Fetch(filter);
        }

        /// <summary>
        ///     Queries for the features that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the features must satisify.</param>
        /// <returns>
        ///     Returns a <see cref="List{IFeature}" /> representing the features returned from the query.
        /// </returns>
        public static List<IFeature> Fetch(this IFeatureClass source, IQueryFilter filter)
        {
            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.Search(filter, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().ToList();
            }
        }

        /// <summary>
        ///     Queries for the features that satisfies the attribute query as specified by an
        ///     <paramref name="whereClause" /> statement.
        ///     and executes the specified <paramref name="action" /> on each feature returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="whereClause">The where clause for the attribute query.</param>
        /// <param name="recycling">
        ///     The recycling parameter controls row object allocation behavior. Recycling cursors rehydrate a
        ///     single feature object on each fetch and can be used to optimize read-only access.
        /// </param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public static int Fetch(this IFeatureClass source, string whereClause, bool recycling, Func<IFeature, bool> action)
        {
            var filter = new QueryFilterClass
            {
                WhereClause = whereClause
            };

            return source.Fetch(filter, recycling, action);
        }

        /// <summary>
        ///     Queries for the features that satisfies the attribute and/or spatial query as specified by an
        ///     <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each feature returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute and/or spatial requirement that the features must satisify.</param>
        /// <param name="recycling">
        ///     The recycling parameter controls row object allocation behavior. Recycling cursors rehydrate a
        ///     single feature object on each fetch and can be used to optimize read-only access.
        /// </param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of features affected by the action.
        /// </returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public static int Fetch(this IFeatureClass source, IQueryFilter filter, bool recycling, Func<IFeature, bool> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            int recordsAffected = 0;

            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.Search(filter, recycling);
                cr.ManageLifetime(cursor);

                foreach (var feature in cursor.AsEnumerable())
                {
                    if (!action(feature))
                        return recordsAffected;

                    recordsAffected++;
                }
            }

            return recordsAffected;
        }

        /// <summary>
        ///     Queries all of the rows that have the specified <paramref name="oids" /> in the array and
        ///     executes the specified <paramref name="action" /> on each row returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oids">The list of object ids.</param>
        /// <param name="recycling">
        ///     The recycling parameter controls row object allocation behavior. Recycling cursors rehydrate a single row object on
        ///     each fetch and
        ///     can be used to optimize read-only access.
        /// </param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of rows affected by the action.
        /// </returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public static int Fetch(this IFeatureClass source, IEnumerable<int> oids, bool recycling, Action<IFeature> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            int recordsAffected = 0;

            using (ComReleaser cr = new ComReleaser())
            {
                object values = oids.ToArray();
                IFeatureCursor cursor = source.GetFeatures(values, recycling);
                cr.ManageLifetime(cursor);

                foreach (var row in cursor.AsEnumerable())
                {
                    action(row);

                    recordsAffected++;
                }
            }

            return recordsAffected;
        }


        /// <summary>
        ///     Gets the name of the delta (either the A or D) table for the versioned <paramref name="source" />.
        /// </summary>
        /// <param name="source">The versioned table or feature class.</param>
        /// <param name="delta">The delta (indicate the A or D) table.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the delta table.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Delta string must be 1 char long.
        ///     or
        ///     Delta string must contain only 'A' or 'D' chars.
        /// </exception>
        public static string GetDeltaTableName(this IObjectClass source, string delta)
        {
            return ((ITable) source).GetDeltaTableName(delta);
        }

        /// <summary>
        ///     Gets index of the <see cref="IField" /> that has the specified <paramref name="fieldName" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the index of the field.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">fieldName</exception>
        public static int GetFieldIndex(this IObjectClass source, string fieldName)
        {
            return ((ITable) source).GetFieldIndex(fieldName);
        }


        /// <summary>
        ///     Gets the name of the owner or schema name of the table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the owner.
        /// </returns>
        public static string GetSchemaName(this IObjectClass source)
        {
            return ((ITable) source).GetSchemaName();
        }

        /// <summary>
        ///     Finds the code of the subtype that has the specified <paramref name="subtypeName" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="subtypeName">Name of the subtype.</param>
        /// <returns>Returns a <see cref="int" /> representing the code of the subtype; otherwise <c>-1</c>.</returns>
        public static int GetSubtypeCode(this IObjectClass source, string subtypeName)
        {
            return ((ITable) source).GetSubtypeCode(subtypeName);
        }

        /// <summary>
        ///     Gets the name of the table (without the owner or schema name).
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the table.
        /// </returns>
        public static string GetTableName(this IObjectClass source)
        {
            return ((ITable) source).GetTableName();
        }

        /// <summary>
        ///     Determines whether the connected user has the specificed privileges to the feature class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="privilege">The privilege (values may be bitwise OR'd together if more than one privilege applies).</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> the privilage is supported; otherwise <c>false</c>
        /// </returns>
        public static bool HasPrivileges(this IFeatureClass source, esriSQLPrivilege privilege)
        {
            if (source == null) return false;

            IDatasetName datasetName = source.FeatureDataset.FullName as IDatasetName;
            ISQLPrivilege sqlPrivilege = datasetName as ISQLPrivilege;

            if (sqlPrivilege == null) return false;

            int supportedPrivileges = (int) privilege & sqlPrivilege.SQLPrivileges;
            return supportedPrivileges > 0;
        }


        /// <summary>
        ///     Updates the minimum display extent to reflect the changes to the features to provide visual feedback.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="display">The display.</param>
        /// <param name="featureRenderer">The feature renderer.</param>
        /// <param name="screenCache">The screen cache.</param>
        /// <param name="features">The features.</param>
        public static void Invalidate(this IFeatureClass source, IScreenDisplay display, IFeatureRenderer featureRenderer, esriScreenCache screenCache = esriScreenCache.esriAllScreenCaches, params IFeature[] features)
        {
            if (display == null || source == null || featureRenderer == null)
                return;

            IInvalidArea3 invalidArea = new InvalidAreaClass();
            invalidArea.Display = display;

            foreach (var feature in features)
            {
                ISymbol symbol = featureRenderer.SymbolByFeature[feature];
                invalidArea.AddFeature(feature, symbol);
            }

            invalidArea.Invalidate((short) screenCache);
        }

        #endregion
    }
}