using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase.Internal;
using ESRI.ArcGIS.GeoDatabaseUI;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.ITable" /> interface.
    /// </summary>
    public static class TableExtensions
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
        public static string CreateExpression(this ITable source, string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, params string[] fieldNames)
        {
            List<IField> fields = new List<IField>();

            foreach (var fieldName in fieldNames)
            {
                int index = source.FindField(fieldName);
                if (index == -1)
                    throw new IndexOutOfRangeException(string.Format("The '{0}' doesn't have a {1} field.", ((IDataset) source).Name, fieldName));

                var field = source.Fields.Field[index];
                fields.Add(field);
            }

            return source.CreateExpression(keyword, comparisonOperator, logicalOperator, fields.ToArray());
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
        public static string CreateExpression(this ITable source, string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator)
        {
            return source.CreateExpression(keyword, comparisonOperator, logicalOperator, source.Fields.AsEnumerable().ToArray());
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
        public static string CreateExpression(this ITable source, string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, params IField[] fields)
        {
            return new QueryBuilder(source).Build(keyword, comparisonOperator, logicalOperator, fields);
        }

        /// <summary>
        ///     Creates a row in the table with the default values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IRow" /> representing the new row.
        /// </returns>
        public static IRow CreateNew(this ITable source)
        {
            if (source == null) return null;

            var row = source.CreateRow();
            IRowSubtypes rowSubtypes = row as IRowSubtypes;
            if (rowSubtypes != null) rowSubtypes.InitDefaultValues();

            return row;
        }

        /// <summary>
        ///     Deletes the table. You must have exlusive rights to the table in order to delete it.
        ///     Otherwise an error will be thrown.
        /// </summary>
        public static void Delete(this ITable source)
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
        ///     Determines if the table exists.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="bool" /> representing <c>true</c> when the table exists otherwise false</returns>
        public static bool Exists(this ITable source)
        {
            IDataset ds = (IDataset) source;
            IWorkspace2 workspace = (IWorkspace2) ds.Workspace;

            return workspace.NameExists[esriDatasetType.esriDTTable, ds.Name];
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
        public static ITable Export(this ITable source, IQueryFilter filter, string outputTableName, IWorkspace outputWorkspace, int handle)
        {
            var ds = (IDataset) source;
            var inputDatasetName = (IDatasetName) ds.FullName;

            var outputClassName = new TableNameClass();
            outputClassName.WorkspaceName = (IWorkspaceName) ((IDataset) outputWorkspace).FullName;
            outputClassName.Name = outputTableName;

            ISelectionSet selection = null;

            if (source.HasOID)
            {
                IScratchWorkspaceFactory2 factory = new ScratchWorkspaceFactoryClass();
                var selectionContainer = factory.DefaultScratchWorkspace;

                selection = source.Select(filter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, selectionContainer);
            }

            outputWorkspace.Delete(outputClassName);

            IExportOperation operation = new ExportOperationClass();
            operation.ExportTable(inputDatasetName, filter, selection, outputClassName, handle);

            return outputWorkspace.GetTable("", outputTableName);
        }

        /// <summary>
        ///     Queries for the rows that have the specified object ids.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oids">The list of object ids.</param>
        /// <returns>
        ///     Returns a <see cref="List{IRow}" /> representing the rows returned from the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">oids</exception>
        public static IList<IRow> Fetch(this ITable source, params int[] oids)
        {
            if (source == null) return null;
            if (oids == null) throw new ArgumentNullException("oids");

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.GetRows(oids, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().ToList();
            }
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
        public static IList<TResult> Fetch<TResult>(this ITable source, IQueryFilter filter, Func<IRow, TResult> selector)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().Select(selector).ToList();
            }
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
        public static IList<TResult> Fetch<TResult>(this ITable source, Func<IRow, TResult> selector, params int[] oids)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");
            if (oids == null) throw new ArgumentNullException("oids");

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.GetRows(oids, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().Select(selector).ToList();
            }
        }

        /// <summary>
        ///     Queries for the rows that satisfy the attribute query as specified by an <paramref name="filter" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute requirement that the rows must satisify.</param>
        /// <returns>
        ///     Returns a <see cref="List{IRow}" /> representing the rows returned from the query.
        /// </returns>
        public static IList<IRow> Fetch(this ITable source, IQueryFilter filter)
        {
            if (source == null) return null;

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().ToList();
            }
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
        public static int Fetch(this ITable source, IQueryFilter filter, Func<IRow, bool> action)
        {
            return source.Fetch(filter, action, true);
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
        public static int Fetch(this ITable source, IQueryFilter filter, Func<IRow, bool> action, bool recycling)
        {
            if (source == null) return 0;
            if (action == null) throw new ArgumentNullException("action");

            int recordsAffected = 0;

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, recycling);
                cr.ManageLifetime(cursor);

                foreach (var row in cursor.AsEnumerable())
                {
                    if (!action(row))
                        return recordsAffected;

                    recordsAffected++;
                }
            }

            return recordsAffected;
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
        public static int Fetch(this ITable source, IQueryFilter filter, Action<IRow> action)
        {
            return source.Fetch(filter, action, true);
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
        public static int Fetch(this ITable source, IQueryFilter filter, Action<IRow> action, bool recycling)
        {
            if (source == null) return 0;
            if (action == null) throw new ArgumentNullException("action");

            int recordsAffected = 0;

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, recycling);
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
        ///     Returns the row from the <paramref name="source" /> with the specified <paramref name="oid" /> when the row doesn't
        ///     exist it will return <c>null</c>.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="oid">The key for the row in the table.</param>
        /// <returns>
        ///     Returns an <see cref="IRow" /> representing the row for the oid; otherwise <c>null</c>
        /// </returns>
        public static IRow Fetch(this ITable source, int oid)
        {
            try
            {
                if (source == null) return null;

                return source.GetRow(oid);
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == (int) fdoError.FDO_E_ROW_NOT_FOUND)
                    return null;

                throw;
            }
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
        public static string GetDeltaTableName(this ITable source, string delta)
        {
            if (source == null) return null;
            if (delta == null) throw new ArgumentNullException("delta");

            if (delta.Length != 1)
                throw new ArgumentException("The delta string must be 1 char long.");

            if (delta.Any(@char => @char != 'A' && @char != 'D' && @char != 'a' && @char != 'd'))
                throw new ArgumentException("The delta string must contain only 'A' or 'D' chars.");

            IVersionedTable versionedTable = source as IVersionedTable;
            if (versionedTable == null)
                throw new ArgumentException("The table must be versioned for it have a delta table.");

            string className = ((IDataset) source).Name;
            int index = className.IndexOf('.');
            if (index > 0)
            {
                string ownerName = source.GetSchemaName();
                string tableName = source.GetTableName();

                using (var cr = new ComReleaser())
                {
                    IWorkspace workspace = ((IDataset) source).Workspace;
                    var fws = (IFeatureWorkspace) workspace;
                    var syntax = (ISQLSyntax) workspace;
                    string functionName = syntax.GetFunctionName(esriSQLFunctionName.esriSQL_UPPER);

                    IQueryDef queryDef = fws.CreateQueryDef();
                    queryDef.Tables = "sde.table_registry";
                    queryDef.SubFields = "registration_id";
                    queryDef.WhereClause = string.Format("{2}(table_name) = {2}('{0}') AND {2}(owner) = {2}('{1}')", tableName, ownerName, functionName);

                    ICursor cursor = queryDef.Evaluate();
                    cr.ManageLifetime(cursor);

                    IRow row = cursor.NextRow();
                    return (row != null) ? string.Format("{0}.{1}{2}", ownerName, delta, row.Value[0]) : null;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the name of the owner or schema name of the table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the owner.
        /// </returns>
        public static string GetSchemaName(this ITable source)
        {
            if (source == null) return null;

            string className = ((IDataset) source).Name;
            int index = className.IndexOf('.');
            if (index > 0)
            {
                string ownerName = className.Substring(0, index);
                return ownerName;
            }

            return null;
        }

        /// <summary>
        ///     Finds the code of the subtype that has the specified <paramref name="subtypeName" />.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="subtypeName">Name of the subtype.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the code of the subtype; otherwise <c>-1</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">subtypeName</exception>
        public static int GetSubtypeCode(this ITable source, string subtypeName)
        {
            if (source == null) return -1;
            if (subtypeName == null) throw new ArgumentNullException("subtypeName");

            ISubtypes subtypes = (ISubtypes) source;
            if (subtypes.HasSubtype) return subtypes.DefaultSubtypeCode;

            foreach (var subtype in subtypes.Subtypes.AsEnumerable().Where(subtype => subtype.Value.Equals(subtypeName, StringComparison.OrdinalIgnoreCase)))
            {
                return subtype.Key;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the subtype name that has the specified <paramref name="subtypeCode" />.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="subtypeCode">The subtype code.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the name of the subtype; otherwise <c>null</c>.
        /// </returns>
        public static string GetSubtypeName(this ITable source, int subtypeCode)
        {
            if (source == null) return null;

            ISubtypes subtypes = (ISubtypes) source;
            if (subtypes.HasSubtype) return null;

            foreach (var subtype in subtypes.Subtypes.AsEnumerable().Where(subtype => subtype.Key == subtypeCode))
            {
                return subtype.Value;
            }

            return null;
        }

        /// <summary>
        ///     Gets the subtype code and name that are assigned to the source.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing code and name of the subtypes; otherwise <c>null</c>.
        /// </returns>
        public static IEnumerable<KeyValuePair<int, string>> GetSubtypes(this ITable source)
        {
            if (source == null) return null;

            ISubtypes subtypes = source as ISubtypes;
            if (subtypes == null) return null;

            return subtypes.Subtypes.AsEnumerable();
        }

        /// <summary>
        ///     Gets the name of the table (without the owner or schema name).
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the table.
        /// </returns>
        public static string GetTableName(this ITable source)
        {
            if (source == null) return null;

            string className = ((IDataset) source).Name;
            int index = className.IndexOf('.');
            if (index > 0)
            {
                string tableName = className.Substring(index + 1, className.Length - index - 1);
                return tableName;
            }

            return className;
        }

        /// <summary>
        ///     Converts the contents returned from the attribute query into an XML document.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute query filter.</param>
        /// <param name="predicate">
        ///     The predicate to determine if the field should be included; otherwise <c>null</c> for all
        ///     fields.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="XDocument" /> representing the contents of the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static XDocument GetXDocument(this ITable source, IQueryFilter filter, Predicate<IField> predicate)
        {
            return source.GetXDocument(filter, predicate, "Table");
        }

        /// <summary>
        ///     Converts the contents returned from the attribute query into an XML document.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute query filter.</param>
        /// <param name="predicate">
        ///     The predicate to determine if the field should be included; otherwise <c>null</c> for all
        ///     fields.
        /// </param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        ///     Returns a <see cref="XDocument" /> representing the contents of the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static XDocument GetXDocument(this ITable source, IQueryFilter filter, Predicate<IField> predicate, string elementName)
        {
            if (source == null) return null;
            if (predicate == null) throw new ArgumentNullException("predicate");

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, true);
                cr.ManageLifetime(cursor);

                return cursor.GetXDocument(elementName, predicate);
            }
        }

        #endregion
    }
}