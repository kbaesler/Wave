using System;
using System.Collections.Generic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IField" /> interface.
    /// </summary>
    public static class FieldExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the <see cref="esriFieldType" /> enumeration to the corresponding <see cref="Type" /> in the .NET
        ///     language.
        /// </summary>
        /// <param name="source">Type of the field.</param>
        /// <returns>
        ///     Returns a <see cref="Type" /> representing the type in the .NET language.
        /// </returns>
        public static Type AsType(this esriFieldType source)
        {
            switch (source)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return Type.GetType("System.Byte[]");
                case esriFieldType.esriFieldTypeDate:
                    return Type.GetType("System.DateTime");
                case esriFieldType.esriFieldTypeDouble:
                    return Type.GetType("System.Double");
                case esriFieldType.esriFieldTypeRaster:
                case esriFieldType.esriFieldTypeGeometry:
                    return Type.GetType("System.Object");
                case esriFieldType.esriFieldTypeOID:
                    return Type.GetType("System.Int64");
                case esriFieldType.esriFieldTypeSingle:
                    return Type.GetType("System.Single");
                case esriFieldType.esriFieldTypeInteger:
                    return Type.GetType("System.Int32");
                case esriFieldType.esriFieldTypeSmallInteger:
                    return Type.GetType("System.Int16");
                case esriFieldType.esriFieldTypeGUID:
                case esriFieldType.esriFieldTypeGlobalID:
                case esriFieldType.esriFieldTypeString:
                case esriFieldType.esriFieldTypeXML:
                    return Type.GetType("System.String");
                default:
                    return Type.GetType("System.Object");
            }
        }

        /// <summary>
        ///     Creates an <see cref="IDictionary{TKey, TValue}" /> from an <see cref="IFields" />
        /// </summary>
        /// <param name="source">An <see cref="IFields" /> to create an <see cref="IDictionary{TKey, TValue}" /> from.</param>
        /// <returns>
        ///     An <see cref="IDictionary{TKey, TValue}" /> that contains the fields from the input source.
        /// </returns>
        public static IDictionary<string, int> ToDictionary(this IFields source)
        {
            IDictionary<string, int> dictionary = new Dictionary<string, int>();

            if (source != null)
            {
                for (int i = 0; i < source.FieldCount; i++)
                {
                    IField field = source.get_Field(i);
                    dictionary.Add(field.Name, i);
                }
            }

            return dictionary;
        }

        #endregion
    }
}