using System;
using System.Collections.Generic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IRow" /> interface.
    /// </summary>
    public static class RowExtensions
    {
        #region Fields

        private static readonly Dictionary<IRow, ReentrancyMonitor> _ReentrancyMonitors = new Dictionary<IRow, ReentrancyMonitor>(new RowEqualityComparer());

        #endregion

        #region Public Methods

        /// <summary>
        ///     Disallow reentrant attempts to save changes to the object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IDisposable" /> implementation used to remove the block on dispose.
        /// </returns>
        /// <remarks>
        ///     The reentrancy is only implemented in the <see cref="SaveChanges" /> method. The blocking will not take affect if
        ///     the <see cref="IRow.Store" /> method is invoked.
        /// </remarks>
        public static IDisposable BlockReentrancy(this IRow source)
        {
            if (!_ReentrancyMonitors.ContainsKey(source))
                _ReentrancyMonitors.Add(source, new ReentrancyMonitor());

            var reentrancyMonitor = _ReentrancyMonitors[source];
            reentrancyMonitor.Set();

            return reentrancyMonitor;
        }

        /// <summary>
        ///     Gets the indexes of the fields that have changed values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{Int32}" /> representing the indexes of the fields that have changed.
        /// </returns>
        public static IEnumerable<int> GetChanges(this IRow source)
        {
            IRowChanges rowChanges = (IRowChanges) source;
            for (int i = 0; i < source.Fields.FieldCount; i++)
            {
                if (rowChanges.ValueChanged[i])
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        ///     Gets the original value for those fields that have changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldNames">The field names.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{Object}" /> representing the original values for those fields that have changed.
        /// </returns>
        public static IEnumerable<object> GetChanges(this IRow source, params string[] fieldNames)
        {
            IRowChanges rowChanges = (IRowChanges) source;
            for (int i = 0; i < source.Fields.FieldCount; i++)
            {
                foreach (var fieldName in fieldNames)
                {
                    if (source.Fields.Field[i].Name.Equals(fieldName))
                    {
                        if (rowChanges.ValueChanged[i])
                        {
                            yield return rowChanges.OriginalValue[i];
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the domain that is assigned to the field.
        /// </summary>
        /// <param name="source">The row.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        ///     Returns a <see cref="IDomain" /> representing the domain for the field.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static IDomain GetDomain(this IRow source, int index)
        {
            if (index < 0 || index > source.Fields.FieldCount)
                throw new IndexOutOfRangeException();

            ISubtypes subtypes = (ISubtypes) source.Table;
            if (subtypes.HasSubtype)
            {
                string fieldName = source.Fields.Field[index].Name;
                IRowSubtypes rowSubtypes = (IRowSubtypes) source;
                return subtypes.Domain[rowSubtypes.SubtypeCode, fieldName];
            }

            return source.Fields.Field[index].Domain;
        }

        /// <summary>
        ///     Returns the field value that has the specified <paramref name="fieldName" /> field name for the
        ///     <paramref name="source" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The row.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fallbackValue">The default value.</param>
        /// <returns>
        ///     Returns an <see cref="object" /> representing the converted value to the specified type.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static TValue GetValue<TValue>(this IRow source, string fieldName, TValue fallbackValue)
        {
            int index = source.Table.FindField(fieldName);
            return source.GetValue(index, fallbackValue);
        }

        /// <summary>
        ///     Returns the field value that at the specified <paramref name="index" /> for the <paramref name="source" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The row.</param>
        /// <param name="index">The index.</param>
        /// <param name="fallbackValue">The default value.</param>
        /// <returns>
        ///     Returns an <see cref="object" /> representing the converted value to the specified type.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static TValue GetValue<TValue>(this IRow source, int index, TValue fallbackValue)
        {
            if (index < 0 || index > source.Fields.FieldCount)
                throw new IndexOutOfRangeException();

            return TypeCast.Cast(source.Value[index], fallbackValue);
        }

        /// <summary>
        ///     Deletes the row from the database but does not trigger the OnDelete event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <remarks>
        ///     Any associated object behavior is not triggered, thus should only be used when implementing custom features that
        ///     bypass <see cref="IRow.Store" /> method.
        /// </remarks>
        public static void Remove(this IRow source)
        {
            ITableWrite tableWrite = source.Table as ITableWrite;
            if (tableWrite != null)
            {
                tableWrite.RemoveRow(source);
            }
        }

        /// <summary>
        ///     Commits the changes to the database when one or more field values have changed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the store was called on the row; otherwise <c>false</c>
        /// </returns>
        /// <remarks>
        ///     The changes will not be saved, if there was a call to BlockReentrancy of which the IDisposable return value has not
        ///     yet been disposed of.
        /// </remarks>
        public static bool SaveChanges(this IRow source)
        {
            source.CheckReentrancy();

            bool saveChanges = false;
            IRowChanges rowChanges = (IRowChanges) source;

            for (int i = 0; i < source.Fields.FieldCount; i++)
            {
                if (rowChanges.ValueChanged[i])
                {
                    saveChanges = true;
                    break;
                }
            }

            if (saveChanges)
            {
                source.Store();
            }

            return saveChanges;
        }

        /// <summary>
        ///     Updates the column on the row with the value when the original value and the specified
        ///     <paramref name="value" /> are different.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the row updated; otherwise <c>false</c>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">fieldName</exception>
        public static bool Update(this IRow source, string fieldName, object value)
        {
            int i = source.Table.FindField(fieldName);
            return source.Update(i, value);
        }

        /// <summary>
        ///     Updates the column index on the row with the value when the original value and the specified
        ///     <paramref name="value" /> are different.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the row updated; otherwise <c>false</c>
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static bool Update(this IRow source, int index, object value)
        {
            return source.Update(index, value, null);
        }

        /// <summary>
        ///     Updates the column index on the row with the value when the original value and the specified
        ///     <paramref name="value" /> are different.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="equalityComparer">
        ///     The equality comparer to use to determine whether or not values are equal.
        ///     If null, the default equality comparer for object is used.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the row updated; otherwise <c>false</c>
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static bool Update(this IRow source, int index, object value, IEqualityComparer<object> equalityComparer)
        {
            if (index < 0 || index > source.Fields.FieldCount)
                throw new IndexOutOfRangeException();

            if (equalityComparer == null)
                equalityComparer = EqualityComparer<object>.Default;

            bool pendingChanges = false;

            IRowChanges rowChanges = (IRowChanges) source;
            object oldValue = rowChanges.OriginalValue[index];

            if (!equalityComparer.Equals(oldValue, value))
            {
                source.Value[index] = value;
                pendingChanges = true;
            }

            return pendingChanges;
        }

        /// <summary>
        ///     Stores a row into the database but does not trigger the OnStore event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <remarks>
        ///     Any associated object behavior is not triggered, thus should only be used when implementing custom features that
        ///     bypass <see cref="IRow.Store" /> method.
        /// </remarks>
        public static void Write(this IRow source)
        {
            ITableWrite tableWrite = source.Table as ITableWrite;
            if (tableWrite != null)
            {
                tableWrite.WriteRow(source);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Checks and aserts for reentrant attempts to change the object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="InvalidOperationException">
        ///     There was a call to BlockReentrancy of which the IDisposable return
        ///     value has not yet been disposed of.
        /// </exception>
        private static void CheckReentrancy(this IRow source)
        {
            if (_ReentrancyMonitors.ContainsKey(source))
            {
                var reentrancyMonitor = _ReentrancyMonitors[source];
                if (reentrancyMonitor.IsBusy)
                    throw new InvalidOperationException("There was a call to BlockReentrancy of which the IDisposable return value has not yet been disposed of.");
            }
        }

        #endregion

        #region Nested Type: ReentrancyMonitor

        /// <summary>
        ///     An internal class used to interupt a method in the middle of its execution and then safely called again
        ///     ("re-entered") before its previous invocations complete execution.
        ///     The interruption could be caused by an internal action such as a jump or call, or by an external action such as a
        ///     hardware interrupt or signal. Once the reentered invocation completes, the previous invocations will resume correct
        ///     execution.
        /// </summary>
        private class ReentrancyMonitor : IDisposable
        {
            #region Fields

            private int _BusyCount;

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets a value indicating whether this instance is busy.
            /// </summary>
            /// <value>
            ///     <c>true</c> if this instance is busy; otherwise, <c>false</c>.
            /// </value>
            public bool IsBusy
            {
                get { return _BusyCount > 0; }
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                --_BusyCount;
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///     Increments the busy counter for the monitor.
            /// </summary>
            public void Set()
            {
                ++_BusyCount;
            }

            #endregion
        }

        #endregion
    }
}