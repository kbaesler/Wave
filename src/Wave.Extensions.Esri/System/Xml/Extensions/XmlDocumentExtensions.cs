using System.IO;
using System.Xml.Xsl;

namespace System.Xml
{
    /// <summary>
    ///     Provides extension methods for the <see cref="XmlDocument" /> object.
    /// </summary>
    public static class XmlDocumentExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Transforms the specified document using the XSLT stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="stream">The stream containing the XSLT document.</param>
        /// <param name="outputFileName">Name of the output file.</param>
        public static void Transform(this XmlDocument source, Stream stream, string outputFileName)
        {
            XmlReader styleSheet = XmlReader.Create(stream);

            try
            {
                XslCompiledTransform xsl = new XslCompiledTransform();
                xsl.Load(styleSheet);

                using (var ms = new MemoryStream())
                {
                    xsl.Transform(source, null, ms);

                    // Reset the stream.
                    ms.Position = 0;

                    using (var sr = new StreamReader(ms))
                    {
                        File.WriteAllText(outputFileName, sr.ReadToEnd());
                    }
                }
            }
            finally
            {
                styleSheet.Close();
            }
        }

        #endregion
    }
}