using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Web
{
    /// <summary>
    /// Provides helper methods for serializing and deserializing data contracts to and from JSON.
    /// </summary>
    public class JsonObject
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToString<T>(T json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, json);

                stream.Position = 0;

                using (StreamReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>
        ///     Creates a copy of the object using a the JSON Data Contract Serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns a <see cref="T" /> repersenting a new instance of the object.</returns>
        public static T Copy<T>(T json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));

            using (Stream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, json);

                stream.Seek(0, SeekOrigin.Begin);

                return (T) serializer.ReadObject(stream);
            }
        }

        /// <summary>
        ///     Minifies blocks of JSON-like content into valid JSON by removing all whitespace and comments.
        /// </summary>
        /// <param name="json">The data.</param>
        /// <returns>Returns a <see cref="string" /> representing the minified JSON data.</returns>
        public static string Minify(string json)
        {
            var min = Regex.Replace(json, @"(\""(?:[^\""\\\\]|\\\\.)*\"")|\\s+|[\/\/].+", "$1");
            return min;
        }

        /// <summary>
        ///     Parses the specified json file into the JSON object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFile">The json file.</param>
        /// <returns>
        ///     Returns a <see cref="T" /> repesenting the JSON object.
        /// </returns>
        public static T Read<T>(string jsonFile)
        {
            var json = File.ReadAllText(jsonFile);
            var mini = Minify(json);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(mini)))
                return (T) serializer.ReadObject(stream);
        }

        /// <summary>
        ///     Writes the JSON data to the specified file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The data.</param>
        /// <param name="jsonFile">The json file.</param>
        public static void Write<T>(T json, string jsonFile)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
            using (FileStream stream = new FileStream(jsonFile, FileMode.Create))
                serializer.WriteObject(stream, json);
        }

        #endregion
    }
}