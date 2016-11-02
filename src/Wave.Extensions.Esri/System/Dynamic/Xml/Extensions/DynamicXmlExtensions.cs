using System.Collections.Generic;
using System.Xml.Linq;

namespace System.Dynamic
{
    /// <summary>
    ///     Provides methods for the XML dynamic types.
    /// </summary>
    public static class DynamicXmlExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the element to a <see cref="DynamicXmlNode" />
        /// </summary>
        /// <param name="source">The dictionary.</param>
        /// <returns>
        ///     Returns a <see cref="DynamicXmlNode" /> representing the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException">source;xml is null.</exception>
        public static dynamic ToDynamic(this XElement source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "element is null.");

            return new DynamicXmlNode(source);
        }

        /// <summary>
        ///     Converts the document to a <see cref="DynamicXmlNode" />
        /// </summary>
        /// <param name="source">The dictionary.</param>
        /// <returns>
        ///     Returns a <see cref="DynamicXmlNode" /> representing the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException">source;xml is null.</exception>
        public static dynamic ToDynamic(this XDocument source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "document is null.");

            return new DynamicXmlNode(source);
        }

        /// <summary>
        ///     Converts the elements to a <see cref="DynamicXmlNodeList" />
        /// </summary>
        /// <param name="source">The dictionary.</param>
        /// <returns>
        ///     Returns a <see cref="DynamicXmlNodeList" /> representing the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException">source;elements is null.</exception>
        public static dynamic ToDynamic(this IEnumerable<XElement> source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "elements is null.");

            return new DynamicXmlNodeList(source);
        }

        #endregion
    }
}