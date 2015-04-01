using System;
using System.Collections.Generic;
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
        ///     Batches the source cursor into sized buckets with the contents of the cursor.
        /// </summary>
        /// <param name="source">The cursor.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     A sequence of projections on equally sized buckets containing the rows in the cursor.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">size;The size must be greater than zero.</exception>
        /// <remarks>
        ///     This operator uses deferred execution and streams its results (buckets and bucket content).
        /// </remarks>
        public static IEnumerable<IEnumerable<IFeature>> Batch(this IFeatureCursor source, int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size", @"The size must be greater than zero.");

            List<IFeature> bucket = null;

            IFeature row;
            while ((row = source.NextFeature()) != null)
            {
                if (bucket == null)
                    bucket = new List<IFeature>(size);

                bucket.Add(row);

                if (bucket.Count != size)
                    continue;

                yield return bucket;

                bucket = null;
            }

            if (bucket != null && bucket.Count > 0)
                yield return bucket;
        }

        /// <summary>
        ///     Batches the source cursor into sized buckets with the contents of the specified field.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The cursor.</param>
        /// <param name="fieldName">The field name that contains the unique key value.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     A sequence of projections on equally sized buckets containing values of the fields for the rows in the cursor.
        /// </returns>
        /// <exception cref="ArgumentNullException">fieldName</exception>
        /// <exception cref="ArgumentOutOfRangeException">size;The size must be greater than zero.</exception>
        /// <remarks>
        ///     This operator uses deferred execution and streams its results (buckets and bucket content).
        /// </remarks>
        public static IEnumerable<Dictionary<TValue, IFeature>> Batch<TValue>(this IFeatureCursor source, string fieldName, int size)
        {
            if (fieldName == null) throw new ArgumentNullException("fieldName");
            if (size < 0) throw new ArgumentOutOfRangeException("size", @"The size must be greater than zero.");

            Dictionary<TValue, IFeature> bucket = null;

            IFeature row;
            while ((row = source.NextFeature()) != null)
            {
                int pos = row.Fields.FindField(fieldName);
                TValue value = row.GetValue(pos, default(TValue));

                if (bucket == null)
                    bucket = new Dictionary<TValue, IFeature>(size);

                bucket.Add(value, row);

                if (bucket.Count != size)
                    continue;

                yield return bucket;

                bucket = null;
            }

            if (bucket != null && bucket.Count > 0)
                yield return bucket;
        }

        /// <summary>
        ///     Batches the source cursor into sized buckets with the rows of the cursor.
        /// </summary>
        /// <param name="source">The cursor.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     A sequence of projections on equally sized buckets containing the rows in the cursor.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">size; The size must be greater than zero.</exception>
        /// <remarks>
        ///     This operator uses deferred execution and streams its results (buckets and bucket content).
        /// </remarks>
        public static IEnumerable<IEnumerable<IRow>> Batch(this ICursor source, int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size", @"The size must be greater than zero.");

            List<IRow> bucket = null;

            IRow row;
            while ((row = source.NextRow()) != null)
            {
                if (bucket == null)
                    bucket = new List<IRow>(size);

                bucket.Add(row);

                if (bucket.Count != size)
                    continue;

                yield return bucket;

                bucket = null;
            }

            if (bucket != null && bucket.Count > 0)
                yield return bucket;
        }

        /// <summary>
        ///     Batches the source cursor into sized buckets with the contents of the specified field.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The cursor.</param>
        /// <param name="fieldName">The field name that contains the unique key value.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     A sequence of projections on equally sized buckets containing values of the fields for the rows in the cursor.
        /// </returns>
        /// <exception cref="ArgumentNullException">fieldName</exception>
        /// <exception cref="ArgumentOutOfRangeException">size;The size must be greater than zero.</exception>
        /// <remarks>
        ///     This operator uses deferred execution and streams its results (buckets and bucket content).
        /// </remarks>
        public static IEnumerable<Dictionary<TValue, IRow>> Batch<TValue>(this ICursor source, string fieldName, int size)
        {
            if (fieldName == null) throw new ArgumentNullException("fieldName");
            if (size < 0) throw new ArgumentOutOfRangeException("size", @"The size must be greater than zero.");

            Dictionary<TValue, IRow> bucket = null;

            IRow row;
            while ((row = source.NextRow()) != null)
            {
                int pos = row.Fields.FindField(fieldName);
                TValue value = row.GetValue(pos, default(TValue));

                if (bucket == null)
                    bucket = new Dictionary<TValue, IRow>(size);

                bucket.Add(value, row);

                if (bucket.Count != size)
                    continue;

                yield return bucket;

                bucket = null;
            }

            if (bucket != null && bucket.Count > 0)
                yield return bucket;
        }

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

            XElement table = new XElement(elementName, new XAttribute("Timestamp", DateTime.Now.ToString("f")));
            XDocument doc = new XDocument(table);

            // Iterate through all of the records.
            foreach (var row in source.AsEnumerable())
            {
                // An element that represents a row.
                XElement element = new XElement("Row");

                // Iterate through all of the fields and output their values.
                for (int i = 0; i < row.Fields.FieldCount; i++)
                {
                    IField field = row.Fields.Field[i];
                    if (predicate(field))
                    {
                        object o = row.Value[i];
                        object value = (DBNull.Value == o || o == null) ? "" : o.ToString();

                        element.Add(new XElement("Field",
                            new XAttribute("Name", field.Name),
                            new XAttribute("Value", value)));
                    }
                }

                // Add the rows to the table.
                table.Add(element);
            }

            return doc;
        }

        #endregion
    }
}