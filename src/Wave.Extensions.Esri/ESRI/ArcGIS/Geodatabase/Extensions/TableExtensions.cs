using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
        ///     Returns a list of the unique values in the column for the table.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> of unique objects for the given column.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">columnName</exception>
        public static List<IRow> Distinct(this ITable source, string columnName, string whereClause)
        {
            if (source == null) return null;
            if (columnName == null) throw new ArgumentNullException("columnName");

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
        ///     Queries for the rows that have the specified object ids.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oids">The list of object ids.</param>
        /// <returns>
        ///     Returns a <see cref="List{IRow}" /> representing the rows returned from the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">oids</exception>
        public static List<IRow> Fetch(this ITable source, params int[] oids)
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
        public static List<TResult> Fetch<TResult>(this ITable source, IQueryFilter filter, Func<IRow, TResult> selector)
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
        public static List<TResult> Fetch<TResult>(this ITable source, Func<IRow, TResult> selector, params int[] oids)
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
        public static List<IRow> Fetch(this ITable source, IQueryFilter filter)
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
        /// <exception cref="ArgumentNullException">action</exception>
        /// <remarks>
        ///     Uses a recycling cursors rehydrate a single row object on each fetch and can be used to optimize read-only
        ///     access
        /// </remarks>
        public static int Fetch(this ITable source, IQueryFilter filter, Func<IRow, bool> action)
        {
            if (source == null) return 0;
            if (action == null) throw new ArgumentNullException("action");

            int recordsAffected = 0;

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, true);
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
            if (source == null) return 0;
            if (action == null) throw new ArgumentNullException("action");

            int recordsAffected = 0;

            using (ComReleaser cr = new ComReleaser())
            {
                ICursor cursor = source.Search(filter, true);
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
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        ///     Returns a <see cref="XDocument" /> representing the contents of the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public static XDocument GetXDocument(this ITable source, IQueryFilter filter, Predicate<IField> predicate, string elementName = "Table")
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