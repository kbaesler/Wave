using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoDatabaseUI;

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
        ///     Creates a "google-like" attribute expression query filter based on the specified keyword.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="keyword">The keyword.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="fieldNames">The field names.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the query necessary to locate the keyword.
        /// </returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public static string CreateExpression(this IFeatureClass source, string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, params string[] fieldNames)
        {
            return ((ITable) source).CreateExpression(keyword, comparisonOperator, logicalOperator, fieldNames);
        }

        /// <summary>
        ///     Creates a "google-like" attribute expression query filter based on the specified keyword.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="keyword">The keyword.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the query necessary to locate the keyword.
        /// </returns>
        public static string CreateExpression(this IFeatureClass source, string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator)
        {
            return ((ITable) source).CreateExpression(keyword, comparisonOperator, logicalOperator, source.Fields.AsEnumerable().ToArray());
        }

        /// <summary>
        ///     Creates a "google-like" attribute expression query filter based on the specified keyword and fields.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="keyword">The keyword.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the query necessary to locate the keyword.
        /// </returns>
        public static string CreateExpression(this IFeatureClass source, string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, params IField[] fields)
        {
            return ((ITable) source).CreateExpression(keyword, comparisonOperator, logicalOperator, fields);
        }

        /// <summary>
        ///     Creates a feature in the table with the default values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IFeature" /> representing the new feature.
        /// </returns>
        public static IFeature CreateNew(this IFeatureClass source)
        {
            if (source == null) return null;

            var row = source.CreateFeature();
            IRowSubtypes rowSubtypes = row as IRowSubtypes;
            if (rowSubtypes != null) rowSubtypes.InitDefaultValues();

            return row;
        }

        /// <summary>
        ///     Deletes the feature class. You must have exlusive rights to the table in order to delete it.
        ///     Otherwise an error will be thrown.
        /// </summary>
        public static void Delete(this IFeatureClass source)
        {
            IDataset ds = (IDataset) source;
            ISchemaLock schemaLock = (ISchemaLock) ds;

            try
            {
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);

                ds.Delete();
            }
            finally
            {
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
        }

        /// <summary>
        ///     Determines if the feature class exists.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="bool" /> representing <c>true</c> when the feature class exists otherwise false</returns>
        public static bool Exists(this IFeatureClass source)
        {
            IDataset ds = (IDataset) source;
            IWorkspace2 workspace = (IWorkspace2) ds.Workspace;

            return workspace.NameExists[esriDatasetType.esriDTFeatureClass, ds.Name];
        }

        /// <summary>
        ///     Exports the source table using the query filter to the table in the output workspace.
        /// </summary>
        /// <param name="source">The source table.</param>
        /// <param name="filter">The filter used to create a subset of the data.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="outputWorkspace">The workspace that will contain the table.</param>
        /// <param name="handle">The handle to the parent application.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the exported table.</returns>
        public static IFeatureClass Export(this IFeatureClass source, IQueryFilter filter, string outputTableName, IWorkspace outputWorkspace, int handle)
        {
            var ds = (IDataset) source;
            var datasetName = (IDatasetName) ds.FullName;

            var outputClassName = new FeatureClassNameClass();
            outputClassName.WorkspaceName = (IWorkspaceName) ((IDataset) outputWorkspace).FullName;
            outputClassName.Name = outputTableName;

            ISelectionSet selection = null;

            if (source.HasOID)
            {
                IScratchWorkspaceFactory2 factory = new ScratchWorkspaceFactoryClass();
                var selectionContainer = factory.DefaultScratchWorkspace;

                selection = source.Select(filter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, selectionContainer);
            }

            var i = source.Fields.FindField(source.ShapeFieldName);
            var field = source.Fields.Field[i];
            var clone = (IClone) field.GeometryDef;
            var geometryDef = (IGeometryDef) clone.Clone();

            outputWorkspace.Delete(outputClassName);

            IExportOperation operation = new ExportOperationClass();
            operation.ExportFeatureClass(datasetName, filter, selection, geometryDef, outputClassName, handle);

            return outputWorkspace.GetFeatureClass("", outputTableName);
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
        /// <exception cref="System.ArgumentNullException">
        ///     filter
        ///     or
        ///     selector
        /// </exception>
        public static IList<TResult> Fetch<TResult>(this IFeatureClass source, IQueryFilter filter, Func<IFeature, TResult> selector)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.Search(filter, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().Select(selector).ToList();
            }
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
        public static IFeature Fetch(this IFeatureClass source, int oid)
        {
            try
            {
                if (source == null) return null;

                return source.GetFeature(oid);
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == (int) fdoError.FDO_E_FEATURE_NOT_FOUND)
                    return null;

                throw;
            }
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
        public static IList<IFeature> Fetch(this IFeatureClass source, IQueryFilter filter)
        {
            if (source == null) return null;

            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.Search(filter, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().ToList();
            }
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
        public static IList<TResult> Fetch<TResult>(this IFeatureClass source, Func<IFeature, TResult> selector, params int[] oids)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");
            if (oids == null) throw new ArgumentNullException("oids");

            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.GetFeatures(oids, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().Select(selector).ToList();
            }
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
        public static IList<IFeature> Fetch(this IFeatureClass source, params int[] oids)
        {
            if (source == null) return null;
            if (oids == null) throw new ArgumentNullException("oids");

            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.GetFeatures(oids, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().ToList();
            }
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
        public static int Fetch(this IFeatureClass source, Func<IFeature, bool> action)
        {
            return source.Fetch(action, true);
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
        public static int Fetch(this IFeatureClass source, Func<IFeature, bool> action, bool recycling)
        {
            if (source == null) return 0;
            if (action == null) throw new ArgumentNullException("action");

            IQueryFilter filter = new QueryFilterClass();
            return source.Fetch(filter, action, recycling);
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
        public static int Fetch(this IFeatureClass source, IQueryFilter filter, Action<IFeature> action)
        {
            return source.Fetch(filter, action, true);
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
        public static int Fetch(this IFeatureClass source, IQueryFilter filter, Action<IFeature> action, bool recycling)
        {
            if (source == null) return 0;
            if (action == null) throw new ArgumentNullException("action");

            int recordsAffected = 0;

            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.Search(filter, recycling);
                cr.ManageLifetime(cursor);

                foreach (var feature in cursor.AsEnumerable())
                {
                    action(feature);
                    recordsAffected++;
                }
            }

            return recordsAffected;
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
        public static int Fetch(this IFeatureClass source, IQueryFilter filter, Func<IFeature, bool> action)
        {
            return source.Fetch(filter, action, true);
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
        public static int Fetch(this IFeatureClass source, IQueryFilter filter, Func<IFeature, bool> action, bool recycling)
        {
            if (source == null) return 0;
            if (action == null) throw new ArgumentNullException("action");

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
        ///     Gets the name of the delta (either the A or D) table for the versioned <paramref name="source" />.
        /// </summary>
        /// <param name="source">The versioned table or feature class.</param>
        /// <param name="delta">The delta (indicate the A or D) table.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the delta table.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">delta</exception>
        /// <exception cref="System.ArgumentException">
        ///     The delta string must be 1 char long.
        ///     or
        ///     The delta string must contain only 'A' or 'D' chars.
        ///     or
        ///     The table must be versioned for it have a delta table.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The delta string must be 1 char long.
        ///     or
        ///     The delta string must contain only 'A' or 'D' chars.
        /// </exception>
        public static string GetDeltaTableName(this IObjectClass source, string delta)
        {
            if (source == null) return null;

            return ((ITable) source).GetDeltaTableName(delta);
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
            if (source == null) return null;

            return ((ITable) source).GetSchemaName();
        }

        /// <summary>
        ///     Gets the subtype code and name that are assigned to the source.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing code and name of the subtypes; otherwise <c>null</c>.
        /// </returns>
        public static IEnumerable<KeyValuePair<int, string>> GetSubtypeCode(this IObjectClass source)
        {
            if (source == null) return null;

            return ((ITable) source).GetSubtypes();
        }

        /// <summary>
        ///     Gets the subtype name that has the specified <paramref name="subtypeCode" />.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="subtypeCode">The subtype code.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the name of the subtype; otherwise <c>null</c>.
        /// </returns>
        public static string GetSubtypeName(this IObjectClass source, int subtypeCode)
        {
            if (source == null) return null;

            return ((ITable) source).GetSubtypeName(subtypeCode);
        }

        /// <summary>
        ///     Gets the subtype code and name that are assigned to the source.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing code and name of the subtypes; otherwise <c>null</c>.
        /// </returns>
        public static IEnumerable<KeyValuePair<int, string>> GetSubtypes(this IObjectClass source)
        {
            if (source == null) return null;

            return ((ITable) source).GetSubtypes();
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
        /// <returns>
        ///     Returns a <see cref="XDocument" /> representing the contents of the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static XDocument GetXDocument(this IFeatureClass source, IQueryFilter filter, Predicate<IField> predicate)
        {
            return source.GetXDocument(filter, predicate, "FeatureClass");
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
        ///     Returns a <see cref="XDocument" /> representing the contents of the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static XDocument GetXDocument(this IFeatureClass source, IQueryFilter filter, Predicate<IField> predicate, string elementName)
        {
            if (source == null) return null;
            if (predicate == null) throw new ArgumentNullException("predicate");

            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureCursor cursor = source.Search(filter, true);
                cr.ManageLifetime(cursor);

                return ((ICursor) cursor).GetXDocument(elementName, predicate);
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
        /// <param name="features">The features.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     display
        ///     or
        ///     featureRenderer
        ///     or
        ///     features
        /// </exception>
        public static void Invalidate(this IFeatureClass source, IScreenDisplay display, IFeatureRenderer featureRenderer, params IFeature[] features)
        {
            if (source == null) return;
            if (display == null) throw new ArgumentNullException("display");
            if (featureRenderer == null) throw new ArgumentNullException("featureRenderer");
            if (features == null) throw new ArgumentNullException("features");

            source.Invalidate(display, featureRenderer, esriScreenCache.esriAllScreenCaches, features);
        }

        /// <summary>
        ///     Updates the minimum display extent to reflect the changes to the features to provide visual feedback.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="display">The display.</param>
        /// <param name="featureRenderer">The feature renderer.</param>
        /// <param name="screenCache">The screen cache.</param>
        /// <param name="features">The features.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     display
        ///     or
        ///     featureRenderer
        ///     or
        ///     features
        /// </exception>
        public static void Invalidate(this IFeatureClass source, IScreenDisplay display, IFeatureRenderer featureRenderer, esriScreenCache screenCache, params IFeature[] features)
        {
            if (source == null) return;
            if (display == null) throw new ArgumentNullException("display");
            if (featureRenderer == null) throw new ArgumentNullException("featureRenderer");
            if (features == null) throw new ArgumentNullException("features");

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