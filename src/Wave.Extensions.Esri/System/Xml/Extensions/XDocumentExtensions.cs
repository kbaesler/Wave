using System.IO;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace System.Xml
{
    /// <summary>
    ///     Provides extension methods for the <see cref="XDocument" />
    /// </summary>
    public static class XDocumentExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Transforms the specified document using the XSLT stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="stream">The stream containing the XSLT document.</param>
        /// <param name="fileName">Name of the file the becomes the output.</param>
        public static void Transform(this XDocument source, Stream stream, string fileName)
        {
            XDocument output = new XDocument();

            using (XmlWriter writer = output.CreateWriter())
            {
                // Load the style sheet.
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(XmlReader.Create(stream));

                // Execute the transform and output the results to a writer.
                xslt.Transform(source.CreateReader(), writer);
            }

            output.Save(fileName);
        }

        /// <summary>
        ///     Transforms the specified document using the XSLT stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="stream">The stream containing the XSLT document.</param>
        /// <returns>Returns a <see cref="XDocument" /> representing the results of the output.</returns>
        public static XDocument Transform(this XDocument source, Stream stream)
        {
            XDocument output = new XDocument();

            using (XmlWriter writer = output.CreateWriter())
            {
                // Load the style sheet.
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(XmlReader.Create(stream));

                // Execute the transform and output the results to a writer.
                xslt.Transform(source.CreateReader(), writer);
            }

            return output;
        }

        #endregion
    }
}