using System;
using System.Collections.Generic;
using System.Linq;

using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IRow" /> interface.
    /// </summary>
    public static class RowExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the indexes and values for all of the fields that have changed values based on the fields assigned the
        ///     specified model names.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="checkFields">
        ///     if set to <c>true</c> when the <paramref name="names" /> list should be checked as both field
        ///     model names and field names; otherwise only the field model names are included.
        /// </param>
        /// <param name="names">The list of names (field or model names).</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{TKey, TValue}" /> representing the model name and values of the fields that have
        ///     changed.
        /// </returns>
        /// <exception cref="ArgumentNullException">names</exception>
        public static Dictionary<string, Dictionary<string, object>> GetChanges(this IRow source, bool checkFields, params string[] names)
        {
            if (source == null) return null;
            if (names == null) throw new ArgumentNullException("names");

            Dictionary<string, Dictionary<string, object>> list = new Dictionary<string, Dictionary<string, object>>();

            IObjectClass table = (IObjectClass) source.Table;
            IRowChanges rowChanges = (IRowChanges) source;

            foreach (var name in names)
            {
                Dictionary<string, object> changes = new Dictionary<string, object>();
                for (int i = 0; i < source.Fields.FieldCount; i++)
                {
                    if (rowChanges.ValueChanged[i])
                    {
                        if (table.IsAssignedFieldModelName(source.Fields.Field[i], name))
                            changes.Add(source.Fields.Field[i].Name, rowChanges.OriginalValue[i]);
                    }
                }

                if (checkFields)
                {
                    var fields = source.GetChanges(name);
                    foreach (var f in fields.Where(f => !changes.ContainsKey(f.Key)))
                        changes.Add(f.Key, f.Value);
                }

                list.Add(name, changes);
            }

            return list;
        }

        /// <summary>
        ///     Returns the domain assigned to the <see cref="IField" /> that is assigned the field model name
        ///     on the specified object.
        /// </summary>
        /// <param name="source">The row object.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="IDomain" /> representing the domain.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static IDomain GetDomain(this IRow source, string modelName, bool throwException = true)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            int index = source.Table.GetFieldIndex(modelName, throwException);
            if (index == -1) throw new IndexOutOfRangeException();

            return source.GetDomain(index);
        }

        /// <summary>
        ///     Gets the field manager for the specified <paramref name="source" />
        /// </summary>
        /// <param name="source">The row.</param>
        /// <param name="auxiliaryFieldBuilder">The auxiliary field builder.</param>
        /// <returns>
        ///     Returns the <see cref="IMMFieldManager" /> representing the properties for the row.
        /// </returns>
        public static IMMFieldManager GetFieldManager(this IRow source, IMMAuxiliaryFieldBuilder auxiliaryFieldBuilder = null)
        {
            if (source == null) return null;

            IMMObjectBuilder builder = new MMObjectBuilderClass();
            builder.Build(source);

            IMMFieldManager fieldManager = new MMFieldManagerClass();
            fieldManager.Build((IMMFieldBuilder) builder, auxiliaryFieldBuilder);

            return fieldManager;
        }

        /// <summary>
        ///     Returns the field value that has been assigned the <paramref name="modelName" /> that is within the specified
        ///     <paramref name="source" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The row.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="fallbackValue">The default value.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns an <see cref="object" /> representing the converted value to the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static TValue GetValue<TValue>(this IRow source, string modelName, TValue fallbackValue, bool throwException = true)
        {
            if (source == null) return fallbackValue;
            if (modelName == null) throw new ArgumentNullException("modelName");

            int index = source.Table.GetFieldIndex(modelName, throwException);
            if (index == -1)
                throw new IndexOutOfRangeException();

            return TypeCast.Cast(source.Value[index], fallbackValue);
        }


        /// <summary>
        ///     Updates the field assigned the <paramref name="modelName" /> with the <paramref name="value" /> for the specified
        ///     <paramref name="source" /> when the value is different than the original value.
        /// </summary>
        /// <param name="source">The row.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="value">The value.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <param name="equalityComparer">if set to <c>true</c> when the changes need to be compared prior to updating.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the row updated; otherwise <c>false</c>
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static bool Update(this IRow source, string modelName, object value, bool throwException, bool equalityComparer = true)
        {
            if (source == null) return false;
            if (modelName == null) throw new ArgumentNullException("modelName");

            int index = source.Table.GetFieldIndex(modelName, throwException);
            if (index == -1)
                throw new IndexOutOfRangeException();

            IMMFieldManager fieldManager = source.GetFieldManager();
            IMMFieldAdapter fieldAdapter = (fieldManager != null) ? fieldManager.FieldByIndex(index) : null;
            return source.Update(index, value, equalityComparer, fieldAdapter);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Updates the column index on the row with the value when the original value and the specified
        ///     <paramref name="value" /> are different.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index of the field.</param>
        /// <param name="value">The value for the field.</param>
        /// <param name="equalityCompare">if set to <c>true</c> when the changes need to be compared prior to updating.</param>
        /// <param name="fieldAdapter">The field adapter used to read the values from the ArcFM attribute editor.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the row updated; otherwise <c>false</c>
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private static bool Update(this IRow source, int index, object value, bool equalityCompare = true, IMMFieldAdapter fieldAdapter = null)
        {
            if (index < 0 || index > source.Fields.FieldCount - 1)
                throw new IndexOutOfRangeException();

            if (equalityCompare)
            {
                switch (source.Table.Fields.Field[index].Type)
                {
                    case esriFieldType.esriFieldTypeOID:
                    case esriFieldType.esriFieldTypeInteger:
                        return source.Update(index, TypeCast.Cast(value, default(long)), EqualityComparer<long>.Default, fieldAdapter);

                    case esriFieldType.esriFieldTypeSmallInteger:
                        return source.Update(index, TypeCast.Cast(value, default(int)), EqualityComparer<int>.Default, fieldAdapter);

                    case esriFieldType.esriFieldTypeSingle:
                        return source.Update(index, TypeCast.Cast(value, default(float)), EqualityComparer<float>.Default, fieldAdapter);

                    case esriFieldType.esriFieldTypeDouble:
                        return source.Update(index, TypeCast.Cast(value, default(double)), EqualityComparer<double>.Default, fieldAdapter);

                    case esriFieldType.esriFieldTypeString:
                    case esriFieldType.esriFieldTypeDate:
                    case esriFieldType.esriFieldTypeGUID:
                    case esriFieldType.esriFieldTypeGlobalID:
                        return source.Update(index, TypeCast.Cast(value, default(string)), EqualityComparer<string>.Default, fieldAdapter);
                }

                return source.Update(index, value, EqualityComparer<object>.Default, fieldAdapter);
            }

            return source.Update(index, value, null, null);
        }

        /// <summary>
        ///     Updates the column index on the row with the value when the original value and the specified
        ///     <paramref name="value" /> are different.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The index of the field.</param>
        /// <param name="value">The value for the field.</param>
        /// <param name="equalityComparer">
        ///     The equality comparer to use to determine whether or not values are equal.
        ///     If null, the default equality comparer for object is used.
        /// </param>
        /// <param name="fieldAdapter">The field adapter used to read the values from the ArcFM attribute editor.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the row updated; otherwise <c>false</c>
        /// </returns>
        private static bool Update<TValue>(this IRow source, int index, TValue value, IEqualityComparer<TValue> equalityComparer, IMMFieldAdapter fieldAdapter)
        {
            bool pendingChanges = true;
            if (equalityComparer != null)
            {
                IRowChanges rowChanges = (IRowChanges) source;
                object originalValue = (fieldAdapter != null) ? fieldAdapter.OriginalValue : rowChanges.OriginalValue[index];

                TValue oldValue = TypeCast.Cast(originalValue, default(TValue));
                pendingChanges = !equalityComparer.Equals(oldValue, value);
            }

            if (pendingChanges)
            {
                if (Equals(value, default(TValue)) && source.Fields.Field[index].IsNullable)
                {
                    source.Value[index] = DBNull.Value;
                }
                else
                {
                    source.Value[index] = value;
                }
            }

            return pendingChanges;
        }

        #endregion
    }
}