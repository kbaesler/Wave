using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Xsl;

namespace System.Xml
{
    /// <summary>
    ///     A supporting class used to transform XML using an Style Sheet.
    /// </summary>
    public static class XmlTransformation
    {
        #region Public Methods

        /// <summary>
        ///     Transforms the specified input XML file using the stylesheet.
        /// </summary>
        /// <param name="styleSheetUri">The style sheet URI.</param>
        /// <param name="inputUri">The input URI.</param>
        /// <param name="resultsFile">The results file.</param>
        public static void Transform(Uri styleSheetUri, Uri inputUri, string resultsFile)
        {
            XslCompiledTransform xsl = new XslCompiledTransform();
            xsl.Load(styleSheetUri.AbsolutePath);
            xsl.Transform(inputUri.AbsolutePath, resultsFile);
        }

        /// <summary>
        ///     Transforms the specified input XML file using the stylesheet.
        /// </summary>
        /// <param name="styleSheetUri">The style sheet URI.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="styleSheetResolver">The style sheet resolver.</param>
        /// <param name="inputUri">The input URI.</param>
        /// <param name="resultsFile">The results file.</param>
        public static void Transform(Uri styleSheetUri, XsltSettings settings, XmlResolver styleSheetResolver, Uri inputUri, string resultsFile)
        {
            XslCompiledTransform xsl = new XslCompiledTransform();
            xsl.Load(styleSheetUri.AbsolutePath, settings, styleSheetResolver);
            xsl.Transform(inputUri.AbsolutePath, resultsFile);
        }

        /// <summary>
        ///     Transform the <paramref name="xmlFragment" /> using the specified stylesheet within memory.
        /// </summary>
        /// <param name="styleSheet">The style sheet.</param>
        /// <param name="xmlFragment">The XML fragment.</param>
        /// <returns>
        ///     A string of the transformed XML fragment.
        /// </returns>
        public static string Transform(XmlReader styleSheet, string xmlFragment)
        {
            XslCompiledTransform xsl = new XslCompiledTransform();
            xsl.Load(styleSheet);

            using (StringWriter output = new StringWriter(new StringBuilder(), CultureInfo.InvariantCulture))
            {
                using (XmlWriter writer = XmlWriter.Create(output))
                {
                    using (StringReader input = new StringReader(xmlFragment))
                    {
                        using (XmlTextReader reader = new XmlTextReader(input))
                        {
                            XmlReaderSettings settings = new XmlReaderSettings();
                            settings.ConformanceLevel = ConformanceLevel.Auto;

                            xsl.Transform(XmlReader.Create(reader, settings), writer);
                        }
                    }
                }

                return output.ToString();
            }
        }

        #endregion
    }
}