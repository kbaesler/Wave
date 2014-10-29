using System;
using System.Collections.Generic;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.Process.IMMWMSPropertySet" /> interface.
    /// </summary>
    public static class PxPropertySetExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMWMSPropertySet" />
        /// </summary>
        /// <param name="source">An <see cref="IMMWMSPropertySet" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the property set from the input source.
        /// </returns>
        public static IEnumerable<KeyValuePair<string, object>> AsEnumerable(this IMMWMSPropertySet source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    yield return new KeyValuePair<string, object>(source.GetNameByIndex(i), source.GetPropertyByIndex(i));
                }
            }
        }

        /// <summary>
        ///     Returns the property value with the specified property name (if it exists), otherwise the fallback value is
        ///     returned.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The propset.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="fallbackValue">The fallback value.</param>
        /// <returns>
        ///     Returns the <see cref="object" /> casted to the specified
        ///     <param ref="TValue" />
        ///     for the property.
        /// </returns>
        public static TValue GetValue<TValue>(this IMMWMSPropertySet source, string propertyName, TValue fallbackValue)
        {
            if (source == null || !source.Exists(propertyName)) return fallbackValue;

            return TypeCast.Cast(source.GetProperty(propertyName), fallbackValue);
        }

        /// <summary>
        ///     Sets the property value for the specified name (if it exists).
        /// </summary>
        /// <param name="source">The propset.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the property was updated; otherwise <c>false</c>.
        /// </returns>
        public static bool SetValue(this IMMWMSPropertySet source, string propertyName, object propertyValue)
        {
            if (source == null || !source.Exists(propertyName)) return false;

            source.SetProperty(propertyName, propertyValue);

            return true;
        }

        #endregion
    }
}