using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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
        ///     Converts the contents returned from the attribute query into an XML document.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="whereClause">The where clause for the attribute query.</param>
        /// <param name="predicate">
        ///     The predicate to determine if the field should be included; otherwise <c>null</c> for all
        ///     fields.
        /// </param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        ///     Returns a <see cref="XmlDocument" /> representing the contents of the query.
        /// </returns>
        public static XmlDocument GetAsXmlDocument(this IFeatureClass source, string whereClause, Predicate<IField> predicate, string elementName = "Table")
        {
            IQueryFilter filter = new QueryFilterClass()
            {
                WhereClause = whereClause
            };

            return source.GetAsXmlDocument(filter, predicate, elementName);
        }

        /// <summary>
        ///     Converts the contents returned from the attribute or spatial query into an XML document.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute or spatial query filter.</param>
        /// <param name="predicate">
        ///     The predicate to determine if the field should be included; otherwise <c>null</c> for all
        ///     fields.
        /// </param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        ///     Returns a <see cref="XmlDocument" /> representing the contents of the query.
        /// </returns>
        public static XmlDocument GetAsXmlDocument(this IFeatureClass source, IQueryFilter filter, Predicate<IField> predicate, string elementName = "Table")
        {
            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.Search(filter, true);
                cr.ManageLifetime(cursor);

                return ((ICursor) cursor).GetAsXmlDocument(elementName, predicate);
            }
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