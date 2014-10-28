using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    ///     Provides extension methods for the <see cref="T:System.IHierarchy{TValue}" /> interface.
    /// </summary>
    public static class HierarchyExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the flat <paramref name="source" /> structure into a hierarchical structure using the
        ///     <paramref name="primaryKeySelector" />
        ///     and <paramref name="foreignKeySelector" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The flat collection structure.</param>
        /// <param name="primaryKeySelector">The delegate used to determine the value of the primary key.</param>
        /// <param name="foreignKeySelector">The delegate used to determine the value of the foreign key.</param>
        /// <returns>
        ///     Returns an enumeration of the collection composed in a hierarchical structure.
        /// </returns>
        public static IEnumerable<IHierarchy<TValue>> CreateHierarchy<TValue, TProperty>(this IEnumerable<TValue> source, Func<TValue, TProperty> primaryKeySelector, Func<TValue, TProperty> foreignKeySelector)
            where TValue : class
        {
            return CreateHierarchyImpl(source, default(TValue), primaryKeySelector, foreignKeySelector, null, 0, 0);
        }

        /// <summary>
        ///     Converts the flat <paramref name="source" /> structure into a hierarchical structure using the
        ///     <paramref name="primaryKeySelector" />
        ///     and <paramref name="foreignKeySelector" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The flat collection structure.</param>
        /// <param name="primaryKeySelector">The delegate used to determine the value of the primary key.</param>
        /// <param name="foreignKeySelector">The delegate used to determine the value of the foreign key.</param>
        /// <param name="rootPrimaryKey">The root primary key value.</param>
        /// <param name="maxDepth">The maximum depth to create the hierarchy.</param>
        /// <returns>
        ///     Returns an enumeration of the collection composed in a hierarchical structure.
        /// </returns>
        public static IEnumerable<IHierarchy<TValue>> CreateHierarchy<TValue, TProperty>(this IEnumerable<TValue> source, Func<TValue, TProperty> primaryKeySelector, Func<TValue, TProperty> foreignKeySelector,
            object rootPrimaryKey, int maxDepth)
            where TValue : class
        {
            return CreateHierarchyImpl(source, default(TValue), primaryKeySelector, foreignKeySelector, rootPrimaryKey, maxDepth, 0);
        }

        /// <summary>
        ///     Recursively traverses the <paramref name="node" /> children nodes executing the <paramref name="action" />
        ///     for each child node.
        /// </summary>
        /// <typeparam name="TValue">The type of the entity.</typeparam>
        /// <param name="node">The root node.</param>
        /// <param name="action">The function that will be performed on for each element in the tree.</param>
        public static void ForEach<TValue>(this IHierarchy<TValue> node, Action<IHierarchy<TValue>> action)
        {
            if (node == null || node.Children == null)
                return;

            foreach (IHierarchy<TValue> child in node.Children)
            {
                action(child);

                child.ForEach(action);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates a hierarchical data structure using the <paramref name="source" /> and
        ///     <paramref name="primaryKeySelector" />
        ///     and <paramref name="foreignKeySelector" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The items.</param>
        /// <param name="parentItem">The parent item.</param>
        /// <param name="primaryKeySelector">The delegate used to determine the value of the primary key.</param>
        /// <param name="foreignKeySelector">The delegate used to determine the value of the foreign key.</param>
        /// <param name="rootPrimaryKey">The root primary key value.</param>
        /// <param name="maxDepth">The maximum depth to create the hierarchy.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>
        ///     Returns an enumeration of the collection composed in a hierarchical structure.
        /// </returns>
        private static IEnumerable<IHierarchy<TValue>> CreateHierarchyImpl<TValue, TProperty>(IEnumerable<TValue> source, TValue parentItem, Func<TValue, TProperty> primaryKeySelector, Func<TValue, TProperty> foreignKeySelector,
            object rootPrimaryKey, int maxDepth, int depth)
            where TValue : class
        {
            IEnumerable<TValue> children = null;
            if (source != null)
            {
                if (rootPrimaryKey != null)
                {
                    children = source.Where(i => primaryKeySelector(i).Equals(rootPrimaryKey));
                }
                else
                {
                    if (parentItem == null)
                    {
                        children = source.Where(i => foreignKeySelector(i).Equals(default(TProperty)));
                    }
                    else
                    {
                        children = source.Where(i => foreignKeySelector(i).Equals(primaryKeySelector(parentItem)));
                    }
                }
            }

            if (children != null && children.Any())
            {
                depth++;

                if ((depth <= maxDepth) || (maxDepth == 0))
                {
                    foreach (var item in children)
                        yield return
                            new Hierarchy<TValue>
                            {
                                Value = item,
                                Children = CreateHierarchyImpl(source, item, primaryKeySelector, foreignKeySelector, null, maxDepth, depth),
                                Depth = depth,
                                Parent = parentItem
                            };
                }
            }
        }

        #endregion
    }
}