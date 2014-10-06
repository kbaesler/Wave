using System;
using System.Xml;

using ESRI.ArcGIS.ADF;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ICursor" /> object.
    /// </summary>
    public static class CursorExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the contents of the cursor into an XML document.
        /// </summary>
        /// <param name="source">The cursor.</param>
        /// <returns>
        ///     Returns a <see cref="XmlDocument" /> representing the contents of the cursor.
        /// </returns>
        public static XmlDocument GetAsXmlDocument(this ICursor source)
        {
            return source.GetAsXmlDocument("Table");
        }

        /// <summary>
        ///     Converts the contents of the cursor into an XML document.
        /// </summary>
        /// <param name="source">The cursor.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        ///     Returns a <see cref="XmlDocument" /> representing the contents of the cursor.
        /// </returns>
        public static XmlDocument GetAsXmlDocument(this ICursor source, string elementName)
        {
            return source.GetAsXmlDocument(elementName, field => (field.Type != esriFieldType.esriFieldTypeGeometry &&
                                                                  field.Type != esriFieldType.esriFieldTypeBlob &&
                                                                  field.Type != esriFieldType.esriFieldTypeRaster &&
                                                                  field.Type != esriFieldType.esriFieldTypeXML));
        }

        /// <summary>
        ///     Converts the contents of the cursor into an XML document.
        /// </summary>
        /// <param name="source">The cursor.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="predicate">The predicate to determine if the field should be included.</param>
        /// <returns>
        ///     Returns a <see cref="XmlDocument" /> representing the contents of the cursor.
        /// </returns>
        public static XmlDocument GetAsXmlDocument(this ICursor source, string elementName, Predicate<IField> predicate)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = (XmlElement) doc.CreateNode(XmlNodeType.Element, elementName, "");
            doc.AppendChild(root);

            // An element that contains the individual rows (or records).
            using (ComReleaser cr = new ComReleaser())
            {
                IRow row;
                while ((row = source.NextRow()) != null)
                {
                    cr.ManageLifetime(row);

                    // An element that represents a row.
                    XmlElement rowNode = (XmlElement) doc.CreateNode(XmlNodeType.Element, "Row", string.Empty);
                    root.AppendChild(rowNode);

                    // Iterate through all of the fields and output their values.
                    for (int i = 0; i < row.Fields.FieldCount; i++)
                    {
                        IField field = row.Fields.Field[i];
                        if (predicate(field))
                        {
                            // The field value for the row.
                            XmlElement fieldNode = (XmlElement) doc.CreateNode(XmlNodeType.Element, "Field", string.Empty);

                            XmlAttribute nameAttribute = doc.CreateAttribute("Name");
                            nameAttribute.Value = field.Name;

                            object o = row.Value[i];
                            XmlAttribute valueAttribute = doc.CreateAttribute("Value");
                            valueAttribute.Value = (DBNull.Value == o || o == null) ? "" : o.ToString();

                            fieldNode.Attributes.Append(nameAttribute);
                            fieldNode.Attributes.Append(valueAttribute);

                            // Add to the collection of field values.
                            rowNode.AppendChild(fieldNode);
                        }
                    }
                }
            }

            return doc;
        }

        #endregion
    }
}