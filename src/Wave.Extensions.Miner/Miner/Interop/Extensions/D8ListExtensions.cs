using System;
using System.Collections.Generic;
using System.Linq;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.ID8ListItem" />,
    ///     <see cref="Miner.Interop.ID8EnumListItem" />
    ///     and <see cref="Miner.Interop.ID8List" /> interfaces.
    /// </summary>
    public static class D8ListExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ID8EnumListItem" />
        /// </summary>
        /// <param name="source">An <see cref="ID8EnumListItem" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <param name="depth">The depth of the recursion.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the list items from the input source.
        /// </returns>
        public static IEnumerable<ID8ListItem> AsEnumerable(this ID8EnumListItem source, int depth)
        {
            return Where(source, o => o != null, depth).Select(o => o.Value);
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ID8EnumListItem" />
        /// </summary>
        /// <param name="source">An <see cref="ID8EnumListItem" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the list items from the input source.
        /// </returns>
        public static IEnumerable<ID8ListItem> AsEnumerable(this ID8EnumListItem source)
        {
            return source.AsEnumerable(Recursion<ID8ListItem>.Infinite);
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ID8List" />
        /// </summary>
        /// <param name="source">An <see cref="ID8List" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <param name="depth">The depth of the recursion.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the list items from the input source.
        /// </returns>
        public static IEnumerable<ID8ListItem> AsEnumerable(this ID8List source, int depth)
        {
            return Where(source, o => o != null, depth).Select(o => o.Value);
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ID8List" />
        /// </summary>
        /// <param name="source">An <see cref="ID8List" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the list items from the input source.
        /// </returns>
        public static IEnumerable<ID8ListItem> AsEnumerable(this ID8List source)
        {
            return source.AsEnumerable(Recursion<ID8ListItem>.Infinite);
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively counting all of the
        ///     <see cref="Miner.Interop.ID8ListItem" /> in the tree.
        /// </summary>
        /// <param name="source">The list to traverse.</param>
        /// <returns>
        ///     Returns the <see cref="int" /> representing the number of elements in the tree.
        /// </returns>
        public static int Count(this ID8List source)
        {
            return Where(source, o => o != null).Count();
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively counting those
        ///     <see cref="Miner.Interop.ID8ListItem" />
        ///     that satisfies the <paramref name="selector" /> match test.
        /// </summary>
        /// <param name="source">The list to traverse.</param>
        /// <param name="selector">A function to test an element for a condition.</param>
        /// <param name="depth">The maximum depth of the recursion.</param>
        /// <returns>
        ///     Returns the <see cref="Int32" /> representing the number of elements that match the selector.
        /// </returns>
        public static int Count(this ID8List source, Func<ID8ListItem, bool> selector, int depth)
        {
            return Where(source, selector, depth).Count();
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively counting those
        ///     <see cref="Miner.Interop.ID8ListItem" />
        ///     that satisfies the <paramref name="selector" /> match test.
        /// </summary>
        /// <param name="source">The list to traverse.</param>
        /// <param name="selector">A function to test an element for a condition.</param>
        /// <returns>
        ///     Returns the <see cref="Int32" /> representing the number of elements that match the selector.
        /// </returns>
        public static int Count(this ID8List source, Func<ID8ListItem, bool> selector)
        {
            return Where(source, selector).Count();
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively counting those
        ///     <see cref="Miner.Interop.ID8ListItem" />
        ///     that satisfies the <paramref name="selector" /> match test.
        /// </summary>
        /// <param name="source">The items to traverse.</param>
        /// <param name="selector">A function to test an element for a condition.</param>
        /// <param name="depth">The maximum depth of the recursion.</param>
        /// <returns>
        ///     Returns the <see cref="Int32" /> representing the number of elements that match the selector.
        /// </returns>
        public static int Count(this ID8EnumListItem source, Func<ID8ListItem, bool> selector, int depth)
        {
            return Where(source, selector, depth).Count();
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively counting those
        ///     <see cref="Miner.Interop.ID8ListItem" />
        ///     that satisfies the <paramref name="selector" /> match test.
        /// </summary>
        /// <param name="source">The items to traverse.</param>
        /// <param name="selector">A function to test an element for a condition.</param>
        /// <returns>
        ///     Returns the <see cref="Int32" /> representing the number of elements that match the selector.
        /// </returns>
        public static int Count(this ID8EnumListItem source, Func<ID8ListItem, bool> selector)
        {
            return Where(source, selector).Count();
        }

        /// <summary>
        ///     Notifies event listeners that the list has been rebuilt.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Notify(this ID8List source)
        {
            IMMCoreEvents eventHandler = (IMMCoreEvents) source;
            eventHandler.ItemRebuilt(source as ID8ListItem);
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively selecting only those
        ///     <see cref="Miner.Interop.ID8ListItem" /> that satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The list to traverse.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <param name="depth">The maximum depth of the recursion.</param>
        /// <returns>
        ///     Returns an
        ///     <see>
        ///         <cref>T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}</cref>
        ///     </see>
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IRecursion<ID8ListItem>> Where(this ID8List source, Func<ID8ListItem, bool> selector, int depth)
        {
            return WhereImpl(source, selector, 0, depth);
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively selecting those
        ///     <see cref="Miner.Interop.ID8ListItem" /> that satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The items to traverse.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <param name="depth">The maximum depth of the recursion.</param>
        /// <returns>
        ///     Returns an
        ///     <see>
        ///         <cref>T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}</cref>
        ///     </see>
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IRecursion<ID8ListItem>> Where(this ID8EnumListItem source, Func<ID8ListItem, bool> selector, int depth)
        {
            return WhereImpl(source, selector, 0, depth);
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively selecting only those
        ///     <see cref="Miner.Interop.ID8ListItem" /> that satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The list to traverse.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        ///     Returns an
        ///     <see>
        ///         <cref>T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}</cref>
        ///     </see>
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IRecursion<ID8ListItem>> Where(this ID8List source, Func<ID8ListItem, bool> selector)
        {
            return Where(source, selector, Recursion<ID8ListItem>.Infinite);
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively selecting those
        ///     <see cref="Miner.Interop.ID8ListItem" /> that satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The items to traverse.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        ///     Returns an
        ///     <see>
        ///         <cref>T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}</cref>
        ///     </see>
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IRecursion<ID8ListItem>> Where(this ID8EnumListItem source, Func<ID8ListItem, bool> selector)
        {
            return Where(source, selector, Recursion<ID8ListItem>.Infinite);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively selecting only those
        ///     <see cref="Miner.Interop.ID8ListItem" /> that satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The list to traverse.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <param name="depth">The depth of the recursion.</param>
        /// <param name="maximum">The maximum depth of the recursion.</param>
        /// <returns>
        ///     Returns an
        ///     <see>
        ///         <cref>T:System.Collections.Generic.IEnumerable{Miner.Collection.IRecursion{Miner.Interop.ID8ListItem}}</cref>
        ///     </see>
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        private static IEnumerable<IRecursion<ID8ListItem>> WhereImpl(ID8List source, Func<ID8ListItem, bool> selector, int depth, int maximum)
        {
            depth++;

            source.Reset();
            ID8ListItem child;
            while ((child = source.Next()) != null)
            {
                if (selector != null && selector(child))
                    yield return new Recursion<ID8ListItem>(depth, child);

                if ((depth <= maximum) || (maximum == Recursion<ID8ListItem>.Infinite))
                {
                    ID8List list = child as ID8List;
                    if (list != null)
                    {
                        foreach (var item in WhereImpl(list, selector, depth, maximum))
                            yield return item;
                    }
                }
            }
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> tree structure recursively selecting only those
        ///     <see cref="Miner.Interop.ID8ListItem" /> that satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The list to traverse.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <param name="depth">The depth of the recursion.</param>
        /// <param name="maximum">The maximum depth of the recursion.</param>
        /// <returns>
        ///     Returns an
        ///     <see>
        ///         <cref>T:System.Collections.Generic.IEnumerable{Miner.Collection.IRecursion{Miner.Interop.ID8ListItem}}</cref>
        ///     </see>
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        private static IEnumerable<IRecursion<ID8ListItem>> WhereImpl(ID8EnumListItem source, Func<ID8ListItem, bool> selector, int depth, int maximum)
        {
            depth++;

            source.Reset();
            ID8ListItem child;
            while ((child = source.Next()) != null)
            {
                if (selector != null && selector(child))
                    yield return new Recursion<ID8ListItem>(depth, child);

                if ((depth <= maximum) || (maximum == -1))
                {
                    ID8List2 list = child as ID8List2;
                    if (list != null)
                    {
                        ID8EnumListItem children = list.Items;
                        foreach (var item in WhereImpl(children, selector, depth, maximum))
                            yield return item;
                    }
                }
            }
        }

        #endregion
    }
}