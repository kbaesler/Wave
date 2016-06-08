using System.Collections.Generic;
using System.Linq;

namespace System.Data
{
    /// <summary>
    ///     Provides extension methods for the <see cref="DataSet" />.
    /// </summary>
    public static class DataSetExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Flattens the data tables using the relations that have been defined to selects all rows from both tables as long as
        ///     there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The function used to determine if the relation is used as part of the flatten result set.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> of the flatten data structure.
        /// </returns>
        public static DataTable Flatten(this DataSet source, Predicate<DataRelation> predicate)
        {
            return source.Flatten(predicate, c => c.ColumnName.Contains("_Id"));
        }


        /// <summary>
        ///     Flattens the data tables using the relations that have been defined to selects all rows from both tables as long as
        ///     there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The function used to determine if the column is removed from the flatten result set.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> of the flatten data structure.
        /// </returns>
        public static DataTable Flatten(this DataSet source, Predicate<DataColumn> predicate)
        {
            return source.Flatten(r => r != null, predicate);
        }

        /// <summary>
        ///     Flattens the data tables using the relations that have been defined to selects all rows from both tables as long as
        ///     there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> of the flatten data structure.
        /// </returns>
        public static DataTable Flatten(this DataSet source)
        {
            return source.Flatten(r => r != null, c => c.ColumnName.Contains("_Id"));
        }

        /// <summary>
        ///     Flattens the data tables using the relations that have been defined to selects all rows from both tables as long as
        ///     there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="relations">The function used to determine if the relation is used as part of the flatten result set.</param>
        /// <param name="columns">The function used to determine if the column is removed from the flatten result set.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> of the flatten data structure.
        /// </returns>
        public static DataTable Flatten(this DataSet source, Predicate<DataRelation> relations, Predicate<DataColumn> columns)
        {
            if (relations == null) throw new ArgumentNullException("relations");

            foreach (DataTable table in source.Tables)
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (!column.ColumnName.Contains("_Id"))
                        column.ColumnName = table.TableName + "." + column.ColumnName;
                }
            }

            DataTable t = source.Relations[0].ChildTable;
            foreach (DataRelation r in source.Relations)
            {
                if (relations(r))
                {
                    t = r.ParentTable.Join(t, r.ParentColumns, r.ChildColumns);
                }
            }

            if (columns != null)
            {
                for (int i = t.Columns.Count - 1; i >= 0; i--)
                {
                    if (columns(t.Columns[i]))
                        t.Columns.RemoveAt(i);
                }
            }

            return t;
        }

        /// <summary>
        ///     Gets the relationship hierarchy that exists between the tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{T}" /> representing the relatonship hierarchy.</returns>
        public static IEnumerable<IHierarchy<DataRelation>> GetRelationHierarchy(this DataSet source)
        {
            List<IHierarchy<DataRelation>> nodes = new List<IHierarchy<DataRelation>>();
            foreach (DataRelation r in source.Relations)
            {
                var children = new List<IHierarchy<DataRelation>>();

                var node = new Hierarchy<DataRelation>();
                node.Value = r;
                node.Children = children;

                var list = nodes.Where(o => o.Value.ParentTable.Equals(r.ChildTable));
                children.AddRange(list);

                foreach (var child in children)
                {
                    child.Parent = r;
                    nodes.Remove(child);
                }

                nodes.Add(node);
            }

            return nodes;
        }

        /// <summary>
        ///     Selects all rows from both tables as long as there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> representing the joined table.
        /// </returns>
        /// <remarks>
        ///     This JOIN method is equivalent to the TSQL INNER JOIN expression using equality.
        /// </remarks>
        public static DataTable Join(this IEnumerable<IHierarchy<DataRelation>> source)
        {
            DataTable t = null;
            foreach (var node in source)
            {
                if (node.Children.Any())
                {
                    t = Join(node.Children);
                }

                var join = node.Value.Join();
                t = t == null ? join : t.Join(join, node.Parent.ParentColumns, node.Parent.ChildColumns);
            }

            return t;
        }

        /// <summary>
        ///     Selects all rows from both tables as long as there is a match between the columns in both tables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> representing the joined table.
        /// </returns>
        /// <remarks>
        ///     This JOIN method is equivalent to the TSQL INNER JOIN expression using equality.
        /// </remarks>
        public static DataTable Join(this DataRelation source)
        {
            return source.ParentTable.Join(source.ChildTable, source.ParentColumns, source.ChildColumns);
        }

        #endregion
    }
}