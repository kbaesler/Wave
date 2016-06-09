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
        public static IEnumerable<IHierarchy<TValue>> SelectHierarchy<TValue, TProperty>(this IEnumerable<TValue> source, Func<TValue, TProperty> primaryKeySelector, Func<TValue, TProperty> foreignKeySelector)
            where TValue : class
        {
            return SelectHierarchyImpl(source, default(TValue), primaryKeySelector, foreignKeySelector, null, 0, 0);
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
        public static IEnumerable<IHierarchy<TValue>> SelectHierarchy<TValue, TProperty>(this IEnumerable<TValue> source, Func<TValue, TProperty> primaryKeySelector, Func<TValue, TProperty> foreignKeySelector,
            object rootPrimaryKey, int maxDepth)
            where TValue : class
        {
            return SelectHierarchyImpl(source, default(TValue), primaryKeySelector, foreignKeySelector, rootPrimaryKey, maxDepth, 0);
        }

        /// <summary>
        ///     Traverses the hierarchical data structure and executes the specified action.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        public static void Traverse<TValue>(this IEnumerable<IHierarchy<TValue>> source, Action<IHierarchy<TValue>> action)
        {
            foreach (var child in source)
            {
                action(child);

                if (child.Children != null && child.Children.Any())
                {
                    child.Children.Traverse(action);
                }
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
        private static IEnumerable<IHierarchy<TValue>> SelectHierarchyImpl<TValue, TProperty>(IEnumerable<TValue> source, TValue parentItem, Func<TValue, TProperty> primaryKeySelector, Func<TValue, TProperty> foreignKeySelector,
            object rootPrimaryKey, int maxDepth, int depth)
            where TValue : class
        {
            var items = new List<TValue>(source);

            IEnumerable<TValue> children = null;
            if (source != null)
            {
                if (rootPrimaryKey != null)
                {
                    children = items.Where(i => primaryKeySelector(i).Equals(rootPrimaryKey));
                }
                else
                {
                    if (parentItem == null)
                    {
                        children = items.Where(i => foreignKeySelector(i).Equals(default(TProperty)));
                    }
                    else
                    {
                        children = items.Where(i => foreignKeySelector(i).Equals(primaryKeySelector(parentItem)));
                    }
                }
            }

            if (children != null)
            {
                depth++;

                if ((depth <= maxDepth) || (maxDepth == 0))
                {
                    foreach (var item in children)
                        yield return
                            new Hierarchy<TValue>
                            {
                                Value = item,
                                Children = SelectHierarchyImpl(items, item, primaryKeySelector, foreignKeySelector, null, maxDepth, depth),
                                Depth = depth,
                                Parent = parentItem
                            };
                }
            }
        }

        #endregion
    }
}