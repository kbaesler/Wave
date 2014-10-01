using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace System.Xml
{
    /// <summary>
    ///     A supporting class used to serialize XML data from various streams.
    /// </summary>
    public static class XmlSerialization
    {
        #region Public Methods

        /// <summary>
        ///     Deserialize the file into the provided class type.
        /// </summary>
        /// <typeparam name="T">The type of class.</typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///     The serialized class type; otherwise null.
        /// </returns>
        public static T Deserialize<T>(string fileName)
            where T : class
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                return Deserialize<T>(fs, new XmlReaderSettings());
        }

        /// <summary>
        ///     Deserialize the string contents into the provided class type.
        /// </summary>
        /// <typeparam name="T">The type of class.</typeparam>
        /// <param name="xmlFragment">The XML contents.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        ///     The serialized class type; otherwise null.
        /// </returns>
        public static T Deserialize<T>(string xmlFragment, XmlReaderSettings settings)
            where T : class
        {
            using (StringReader sr = new StringReader(xmlFragment))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                XmlReader xr = XmlReader.Create(sr, settings);
                return serializer.Deserialize(xr) as T;
            }
        }

        /// <summary>
        ///     Deserialize the contents of the stream into the provided class type.
        /// </summary>
        /// <typeparam name="T">The type of class.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        ///     The serialized class type; otherwise null.
        /// </returns>
        /// <exception cref="FileNotFoundException">The file was not found.</exception>
        public static T Deserialize<T>(Stream stream, XmlReaderSettings settings)
            where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof (T));
            using (XmlReader xr = XmlReader.Create(stream, settings))
                return serializer.Deserialize(xr) as T;
        }

        /// <summary>
        ///     Serializes the contents of the specified data to the stream using UTF8 encoding.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="namespaces">The namespaces.</param>
        public static void Serialize<T>(T data, Stream stream, XmlWriterSettings settings, XmlSerializerNamespaces namespaces)
            where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof (T));
            using (XmlWriter xw = XmlWriter.Create(stream, settings))
                serializer.Serialize(xw, data, namespaces);
        }

        /// <summary>
        ///     Serializes the contents of the specified data to the file using the default settings.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void Serialize<T>(T data, string fileName)
            where T : class
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            Serialize(data, fileName, settings, ns);
        }

        /// <summary>
        ///     Serializes the contents of the specified data to the file using the specified <paramref name="settings" />.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="settings">The settings.</param>
        public static void Serialize<T>(T data, string fileName, XmlWriterSettings settings)
            where T : class
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            Serialize(data, fileName, settings, ns);
        }

        /// <summary>
        ///     Serializes the contents of the specified data to the file.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="namespaces">The namespaces.</param>
        public static void Serialize<T>(T data, string fileName, XmlWriterSettings settings, XmlSerializerNamespaces namespaces)
            where T : class
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                Serialize(data, fs, settings, namespaces);
        }

        /// <summary>
        ///     Serializes the contents of the specified data to an XML fragment.
        /// </summary>
        /// <typeparam name="T">The type of data</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        ///     An XML fragment of the contents.
        /// </returns>
        public static string Serialize<T>(T data, XmlWriterSettings settings)
            where T : class
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                using (XmlWriter xw = XmlWriter.Create(sw, settings))
                    serializer.Serialize(xw, data, ns);

                return sw.ToString();
            }
        }

        #endregion
    }
}