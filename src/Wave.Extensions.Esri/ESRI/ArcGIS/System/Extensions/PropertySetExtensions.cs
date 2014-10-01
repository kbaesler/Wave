using System;
using System.Collections.Generic;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.esriSystem.IPropertySet" />
    /// </summary>
    public static class PropertySetExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IPropertySet" />
        /// </summary>
        /// <param name="source">An <see cref="IPropertySet" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the property set from the input source.</returns>
        public static IEnumerable<KeyValuePair<string, object>> AsEnumerable(this IPropertySet source)
        {
            if (source != null)
            {
                object names;
                object values;

                source.GetAllProperties(out names, out values);

                string[] n = (string[]) names;
                object[] v = (object[]) values;

                for (int i = 0; i < n.Length; i++)
                {
                    yield return new KeyValuePair<string, object>(n[i], v[i]);
                }
            }
        }

        /// <summary>
        ///     Gets the value from the property set that has the given <paramref name="name" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The property set.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        ///     Returns the value for the property with the specified name; otherwise the <paramref name="defaultValue" /> will be
        ///     returned.
        /// </returns>
        public static TValue GetProperty<TValue>(this IPropertySet source, string name, TValue defaultValue)
        {
            if (source != null)
            {
                foreach (var entry in source.AsEnumerable())
                {
                    if (string.Equals(name, entry.Key, StringComparison.CurrentCultureIgnoreCase))
                        return TypeCast.Cast(entry.Value, defaultValue);
                }
            }

            return defaultValue;
        }

        #endregion
    }
}