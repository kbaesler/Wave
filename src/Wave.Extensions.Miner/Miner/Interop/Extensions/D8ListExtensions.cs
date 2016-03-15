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
            if (source == null) return null;

            return source.Where(o => o != null, depth).Select(o => o.Value);
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
            if (source == null) return null;

            return source.AsEnumerable(Recursion<ID8ListItem>.Infinity);
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
            if (source == null) return null;

            return source.Where(o => o != null, depth).Select(o => o.Value);
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
            if (source == null) return null;

            return source.AsEnumerable(Recursion<ID8ListItem>.Infinity);
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
            if (source == null) return 0;

            return source.Where(o => o != null).Count();
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
        /// <exception cref="ArgumentNullException">selector</exception>
        public static int Count(this ID8List source, Func<ID8ListItem, bool> selector, int depth)
        {
            if (source == null) return 0;
            if (selector == null) throw new ArgumentNullException("selector");

            return source.Where(selector, depth).Count();
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
        /// <exception cref="ArgumentNullException">selector</exception>
        public static int Count(this ID8List source, Func<ID8ListItem, bool> selector)
        {
            if (source == null) return 0;
            if (selector == null) throw new ArgumentNullException("selector");

            return source.Where(selector).Count();
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
        /// <exception cref="ArgumentNullException">selector</exception>
        public static int Count(this ID8EnumListItem source, Func<ID8ListItem, bool> selector, int depth)
        {
            if (source == null) return 0;
            if (selector == null) throw new ArgumentNullException("selector");

            return source.Where(selector, depth).Count();
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
        /// <exception cref="ArgumentNullException">selector</exception>
        public static int Count(this ID8EnumListItem source, Func<ID8ListItem, bool> selector)
        {
            if (source == null) return 0;
            if (selector == null) throw new ArgumentNullException("selector");

            return source.Where(selector).Count();
        }

        /// <summary>
        ///     Notifies event listeners that the list has been rebuilt.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Notify(this ID8List source)
        {
            if (source == null) return;

            IMMCoreEvents eventHandler = (IMMCoreEvents) source;
            eventHandler.ItemRebuilt(source as ID8ListItem);
        }

        /// <summary>
        ///     Replaces the source list with the contents of the specified list.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="list">The list that will replace the source.</param>
        public static void Update(this ID8List source, ID8List list)
        {
            source.Clear();

            ((ID8ListItem) source).AllowCoreEvents = false;

            try
            {
                list.Reset();

                ID8ListItem item;
                while ((item = list.Next(false)) != null)
                    source.Add(item);
            }
            finally
            {
                ((ID8ListItem) source).AllowCoreEvents = true;
            }
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
        ///     <see cref="T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}" />
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">selector</exception>
        public static IEnumerable<IRecursion<ID8ListItem>> Where(this ID8List source, Func<ID8ListItem, bool> selector, int depth)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

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
        ///     <see cref="T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}" />
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">selector</exception>
        public static IEnumerable<IRecursion<ID8ListItem>> Where(this ID8EnumListItem source, Func<ID8ListItem, bool> selector, int depth)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

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
        ///     <see cref="T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}" />
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">selector</exception>
        public static IEnumerable<IRecursion<ID8ListItem>> Where(this ID8List source, Func<ID8ListItem, bool> selector)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            return source.Where(selector, Recursion<ID8ListItem>.Infinity);
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
        ///     <see cref="T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}" />
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IRecursion<ID8ListItem>> Where(this ID8EnumListItem source, Func<ID8ListItem, bool> selector)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            return source.Where(selector, Recursion<ID8ListItem>.Infinity);
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
        ///     <see cref="T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}" />
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     source
        ///     or
        ///     selector
        /// </exception>
        private static IEnumerable<IRecursion<ID8ListItem>> WhereImpl(ID8List source, Func<ID8ListItem, bool> selector, int depth, int maximum)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            depth++;

            source.Reset();
            ID8ListItem child;
            while ((child = source.Next()) != null)
            {
                if (selector(child))
                    yield return new Recursion<ID8ListItem>(depth, child);

                if ((depth <= maximum) || (maximum == Recursion<ID8ListItem>.Infinity))
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
        ///     <see cref="T:System.Collections.Generic.IEnumerable{Miner.Collections.IRecursion{Miner.Interop.ID8ListItem}}" />
        ///     whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        private static IEnumerable<IRecursion<ID8ListItem>> WhereImpl(ID8EnumListItem source, Func<ID8ListItem, bool> selector, int depth, int maximum)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            depth++;

            source.Reset();
            ID8ListItem child;
            while ((child = source.Next()) != null)
            {
                if (selector(child))
                    yield return new Recursion<ID8ListItem>(depth, child);

                if ((depth <= maximum) || (maximum == Recursion<ID8ListItem>.Infinity))
                {
                    ID8List2 list = child as ID8List2;
                    if (list != null)
                    {
                        ID8EnumListItem children = list.Items;
                        if (children != null)
                        {
                            foreach (var item in WhereImpl(children, selector, depth, maximum))
                                yield return item;
                        }
                    }
                }
            }
        }

        #endregion
    }
}