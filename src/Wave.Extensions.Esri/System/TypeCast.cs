using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System
{
    /// <summary>
    ///     Provides helper methods to translate data between the internal data storage value and the user-display value and
    ///     vice versa.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TypeCast")]
    public static class TypeCast
    {
        #region Public Methods

        /// <summary>
        ///     Casts an object to the type of the given default value. If the object is null
        ///     or DBNull, the default value specified will be returned. If the object is
        ///     convertible to the type of the default value, the explicit conversion will
        ///     be performed.
        /// </summary>
        /// <typeparam name="T">The target data type of the conversion.</typeparam>
        /// <param name="value">The object to be cast.</param>
        /// <param name="defaultValue">The default fail back value</param>
        /// <returns>The value of the object cast to the type of the default value.</returns>
        /// <exception cref="System.InvalidCastException">
        ///     Thrown if the type of the object
        ///     cannot be cast to the type of the default value.
        /// </exception>
        public static T Cast<T>(object value, T defaultValue)
        {
            if (value != null)
            {
                if (value is T)
                {
                    if (IsEmpty(value))
                        return defaultValue;

                    return (T) value;
                }

                if (IsEmpty(value))
                    return defaultValue;

                if (!Convert.IsDBNull(value))
                {
                    return (T) Convert.ChangeType(value, typeof (T), CultureInfo.InvariantCulture);
                }
            }

            return defaultValue;
        }

        /// <summary>
        ///     Attempts to casts an object to the type of the given default value. If the object is null
        ///     or DBNull, the default value specified will be returned. If the object is
        ///     convertible to the type of the default value, the explicit conversion will
        ///     be performed.
        /// </summary>
        /// <typeparam name="T">The target data type of the conversion.</typeparam>
        /// <param name="value">The object to be cast.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        ///     The value of the object cast to the type of the default value.
        /// </returns>
        /// <exception cref="System.InvalidCastException">
        ///     Thrown if the type of the object
        ///     cannot be cast to the type of the default value.
        /// </exception>
        public static bool TryCast<T>(object value, out T result)
        {
            result = default(T);

            try
            {
                result = Cast(value, result);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Determines whether the specified object is an empty string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///     <c>true</c> if the specified object is an empty string; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEmpty(object obj)
        {
            if (!(obj is string)) return false;

            return string.IsNullOrEmpty(obj.ToString().Trim());
        }

        #endregion
    }
}