using System;
using System.Collections.Generic;
using System.Linq;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IDomain" /> interface.
    /// </summary>
    public static class DomainExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ICodedValueDomain" />
        /// </summary>
        /// <param name="source">An <see cref="ICodedValueDomain" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the domain from the input source.</returns>
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this ICodedValueDomain source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.CodeCount; i++)
                {
                    yield return new KeyValuePair<string, string>(source.Name[i], TypeCast.Cast(source.Value[i], string.Empty));
                }
            }
        }

        /// <summary>
        ///     Finds the description in the domain that matches the specified <paramref name="value" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns a <see cref="string" /> representing the name (or description) otherwise <c>null</c>.</returns>
        public static string GetDescription(this ICodedValueDomain source, object value)
        {
            if ((source == null) || (value == null) || Convert.IsDBNull(value))
            {
                return null;
            }

            return (from entry in source.AsEnumerable() where entry.Value.Equals(value.ToString()) select entry.Key).FirstOrDefault();
        }

        /// <summary>
        ///     Finds the value in the domain that matches the specified <paramref name="name" />
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="fallbackValue">The fallback value.</param>
        /// <returns>
        ///     Returns the value representing the name (or description) otherwise the fallback value is used.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public static TValue GetValue<TValue>(this ICodedValueDomain source, string name, TValue fallbackValue)
        {
            if (source == null) return fallbackValue;
            if (name == null) throw new ArgumentNullException("name");

            object o = null;
            foreach (KeyValuePair<string, string> entry in source.AsEnumerable())
            {
                if (entry.Key.Equals(name))
                {
                    o = entry.Value;
                    break;
                }
            }
            return TypeCast.Cast(o, fallbackValue);
        }

        #endregion
    }
}