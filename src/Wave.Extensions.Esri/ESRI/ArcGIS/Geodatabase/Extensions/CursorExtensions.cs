using System;
using System.Xml.Linq;

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
        ///     Returns a <see cref="XDocument" /> representing the contents of the cursor.
        /// </returns>
        public static XDocument GetXDocument(this ICursor source)
        {
            if (source == null) return null;

            return source.GetXDocument("Table");
        }

        /// <summary>
        ///     Converts the contents of the cursor into an XML document.
        /// </summary>
        /// <param name="source">The cursor.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        ///     Returns a <see cref="XDocument" /> representing the contents of the cursor.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">elementName</exception>
        public static XDocument GetXDocument(this ICursor source, string elementName)
        {
            if (source == null) return null;
            if (elementName == null) throw new ArgumentNullException("elementName");

            return source.GetXDocument(elementName, field => (field.Type != esriFieldType.esriFieldTypeGeometry &&
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
        ///     Returns a <see cref="XDocument" /> representing the contents of the cursor.
        /// </returns>
        public static XDocument GetXDocument(this ICursor source, string elementName, Predicate<IField> predicate)
        {
            if (source == null) return null;
            if (elementName == null) throw new ArgumentNullException("elementName");
            if (predicate == null) throw new ArgumentNullException("predicate");

            XElement xtable = new XElement(elementName, new XAttribute("Timestamp", DateTime.Now.ToString("f")));
            XDocument xdoc = new XDocument(xtable);

            // Iterate through all of the records.
            foreach (var row in source.AsEnumerable())
            {
                // An element that represents a row.
                XElement xrow = new XElement("Row");

                // Iterate through all of the fields and output their values.
                for (int i = 0; i < row.Fields.FieldCount; i++)
                {
                    IField field = row.Fields.Field[i];
                    if (predicate(field))
                    {
                        object o = row.Value[i];
                        object value = (DBNull.Value == o || o == null) ? "" : o.ToString();

                        xrow.Add(new XElement("Field",
                            new XAttribute("Name", field.Name),
                            new XAttribute("Value", value)));
                    }
                }

                // Add the rows to the table.
                xtable.Add(xrow);
            }

            return xdoc;
        }

        #endregion
    }
}