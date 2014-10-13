using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml;

using ESRI.ArcGIS.ADF;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.ITable" /> interface.
    /// </summary>
    public static class TableExtensions
    {
        #region Public Methods

        /// <summary>
        /// Gets index of the <see cref="IField"/> that has the specified <paramref name="fieldName"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Returns a <see cref="int"/> representing the index of the field.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">fieldName</exception>
        public static int GetFieldIndex(this ITable source, string fieldName)
        {
            return ((IObjectClass) source).GetFieldIndex(fieldName);
        }

        /// <summary>
        ///     Converts the <paramref name="source" /> into a <see cref="DataTable" /> and loads it with the results from the
        ///     query using the <paramref name="filter" />.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="filter">The where clause used to filter the results in the table.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> representing the contents of the results from the query.
        /// </returns>
        public static DataTable AsDataTable(this ITable source, IQueryFilter filter)
        {
            DataTable table = new DataTable(((IDataset) source).Name)
            {
                Locale = CultureInfo.InvariantCulture
            };

            // Add each field as a column in the data table.
            foreach (IField field in source.Fields.AsEnumerable())
            {
                if (field.Type != esriFieldType.esriFieldTypeGeometry)
                {
                    DataColumn column = new DataColumn(field.Name, field.Type.AsType());
                    column.Caption = field.AliasName;
                    table.Columns.Add(column);
                }
            }

            // Fetch the records matching the query and add them them to the data table.
            source.Fetch(filter, true, row =>
            {
                DataRow newRow = table.NewRow();

                // Update each column in the table.
                foreach (DataColumn column in table.Columns)
                {
                    int pos = row.Fields.FindField(column.ColumnName);
                    newRow[column] = row.Value[pos];
                }

                table.Rows.Add(newRow);
            });


            return table;
        }

        /// <summary>
        ///     Returns a list of the unique values in the column for the table.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> of unique objects for the given column.
        /// </returns>
        public static List<IRow> Distinct(this ITable source, string columnName, string whereClause)
        {
            IDataset dataset = (IDataset) source;
            IWorkspace workspace = dataset.Workspace;
            IFeatureWorkspace fws = (IFeatureWorkspace) workspace;

            IQueryDef queryDef = fws.CreateQueryDef();
            queryDef.Tables = dataset.Name;
            queryDef.SubFields = string.Format("DISTINCT({0})", columnName);
            queryDef.WhereClause = whereClause;

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = queryDef.Evaluate();
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().ToList();
            }
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
        /// <param name="action">The action to take for each row in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of rows affected by the action.
        /// </returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public static int Fetch(this ITable source, IEnumerable<int> oids, bool recycling, Action<IRow> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            int recordsAffected = 0;

            using (ComReleaser cr = new ComReleaser())
            {
                object values = oids.ToArray();
                ICursor cursor = source.GetRows(values, recycling);
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
        ///     Searches for the rows that satisfy the attribute query as specified by an <paramref name="filter" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute requirement that the rows must satisify.</param>
        /// <returns>
        ///     Returns a <see cref="List{IRow}" /> representing the rows returned from the query.
        /// </returns>
        public static List<IRow> Fetch(this ITable source, IQueryFilter filter)
        {
            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, false);
                cr.ManageLifetime(cursor);

                return cursor.AsEnumerable().ToList();
            }
        }

        /// <summary>
        ///     Searches for the rows that satisfy the attribute query as specified by an <paramref name="filter" /> object
        ///     and executes the specified <paramref name="action" /> on each row returned from the query.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The attribute requirement that features must satisify.</param>
        /// <param name="recycling">
        ///     The recycling parameter controls row object allocation behavior. Recycling cursors rehydrate a
        ///     single row object on each fetch and can be used to optimize read-only access.
        /// </param>
        /// <param name="action">The action to take for each feature in the cursor.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of rows affected by the action.
        /// </returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public static int Fetch(this ITable source, IQueryFilter filter, bool recycling, Action<IRow> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

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
        ///     Returns a <see cref="XmlDocument" /> representing the contents of the query.
        /// </returns>
        public static XmlDocument GetAsXmlDocument(this ITable source, IQueryFilter filter, Predicate<IField> predicate, string elementName = "Table")
        {
            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, true);
                cr.ManageLifetime(cursor);

                return cursor.GetAsXmlDocument(elementName, predicate);
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
        /// <exception cref="ArgumentException">
        ///     The delta string must be 1 char long.
        ///     or
        ///     The delta string must contain only 'A' or 'D' chars.
        /// </exception>
        public static string GetDeltaTableName(this ITable source, string delta)
        {
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
        public static int GetSubtypeCode(this ITable source, string subtypeName)
        {
            ISubtypes subtypes = (ISubtypes) source;
            if (subtypes.HasSubtype) return -1;

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
            ISubtypes subtypes = (ISubtypes) source;
            if (subtypes.HasSubtype) return null;

            foreach (var subtype in subtypes.Subtypes.AsEnumerable().Where(subtype => subtype.Key == subtypeCode))
            {
                return subtype.Value;
            }

            return null;
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
            string className = ((IDataset) source).Name;
            int index = className.IndexOf('.');
            if (index > 0)
            {
                string tableName = className.Substring(index + 1, className.Length - index - 1);
                return tableName;
            }

            return className;
        }

        #endregion
    }
}