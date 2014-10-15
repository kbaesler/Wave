using System;
using System.Xml;

using ESRI.ArcGIS.ADF;
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
        public static XDocument GetXDocument(this ICursor source, string elementName)
        {
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
            XElement xtable = new XElement("Table", new XAttribute("Timestamp", DateTime.Now.ToString("f")));
            XDocument xdoc = new XDocument(xtable);

            // An element that contains the individual rows (or records).
            using (ComReleaser cr = new ComReleaser())
            {
                IRow row;
                while ((row = source.NextRow()) != null)
                {
                    cr.ManageLifetime(row);

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

                            XElement xfield = new XElement("Field");
                            xfield.Add(new XAttribute("Name", field.Name));
                            xfield.Add(new XAttribute("Value", value));

                            // Add to the collection of field values.
                            xrow.Add(xfield);
                        }
                    }

                    // Add the rows to the table.
                    xtable.Add(xrow);
                }
            }

            return xdoc;
        }

        #endregion
    }
}