using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Wave.Searchability.Data.Configuration
{
    public class JsonConfigurationFile<T> : ConfigurationFile<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfigurationFile{T}"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public JsonConfigurationFile(string fileName)
            : base(fileName)
        {
            
        }

        /// <summary>
        /// Gets the contents of the stream that are in the json format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns a <see cref="T"/> representing the contents of the stream.</returns>
        protected override T GetContents(StreamReader stream)
        {
            if (stream == null) return default(T);

            using (JsonTextReader reader = new JsonTextReader(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }
    }
}
