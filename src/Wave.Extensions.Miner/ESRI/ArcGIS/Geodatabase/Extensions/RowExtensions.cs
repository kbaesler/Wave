using System;
using System.Collections.Generic;

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
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static IDomain GetDomain(this IRow source, string modelName, bool throwException = true)
        {
            int index = source.Table.GetFieldIndex(modelName, throwException);
            if (index == -1) throw new IndexOutOfRangeException();

            return source.GetDomain(index);
        }

        /// <summary>
        /// Gets the field manager for the specified <paramref name="source" />
        /// </summary>
        /// <param name="source">The row.</param>
        /// <param name="auxiliaryFieldBuilder">The auxiliary field builder.</param>
        /// <returns>
        /// Returns the <see cref="IMMFieldManager" /> representing the properties for the row.
        /// </returns>
        public static IMMFieldManager GetFieldManager(this IRow source, IMMAuxiliaryFieldBuilder auxiliaryFieldBuilder = null)
        {
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
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static TValue GetValue<TValue>(this IRow source, string modelName, TValue fallbackValue, bool throwException = true)
        {
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
        /// <param name="equalityComparer">The equality comparer to use to determine whether or not values are equal.
        /// If null, the default equality comparer for object is used.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the row updated; otherwise <c>false</c>
        /// </returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static bool Update(this IRow source, string modelName, object value, bool throwException, IEqualityComparer<object> equalityComparer = null)
        {
            int index = source.Table.GetFieldIndex(modelName, throwException);
            if (index == -1)
                throw new IndexOutOfRangeException();

            return source.Update(index, value, equalityComparer);
        }

        #endregion
    }
}