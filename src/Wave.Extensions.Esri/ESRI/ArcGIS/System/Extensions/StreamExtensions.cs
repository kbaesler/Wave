using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using ESRI.ArcGIS.esriSystem.Internal;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     Provides extension methods for loading and saving binary data using the
    ///     <see cref="ESRI.ArcGIS.esriSystem.IStream" />. This class cannot be inherited.
    /// </summary>
    public static class StreamExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Loads the binary data from the specified <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="eventHandler">The event handler used to resolve assembly references.</param>
        /// <returns>
        ///     The deserialized object that was stored in the stream.
        /// </returns>
        public static object Deserialize(this IStream stream, ResolveEventHandler eventHandler)
        {
            try
            {
                if (eventHandler != null)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += eventHandler;
                }

                // Use the COM wrapper for the stream.
                using (ComStream cs = new ComStream(stream))
                {
                    // Get the size of the object.
                    byte[] header = new Byte[4];
                    cs.Read(header, 0, header.Length);

                    int size = BitConverter.ToInt32(header, 0);

                    // Get the byte array of the object.
                    byte[] buffer = new byte[size];
                    cs.Read(buffer, 0, buffer.Length);

                    // Read the object in the memory stream in binary format.
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        return bf.Deserialize(ms);
                    }
                }
            }
            finally
            {
                if (eventHandler != null)
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= eventHandler;
                }
            }
        }

        /// <summary>
        ///     Loads the binary data from the specified <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The deserialized object that was stored in the stream.</returns>
        public static object Deserialize(this IStream stream)
        {
            return stream.Deserialize((sender, e) =>
            {
                string name = e.Name.Split(",".ToCharArray())[0];
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                return assemblies.FirstOrDefault(resolve => { return (name.Equals(resolve.FullName.Split(",".ToCharArray())[0], StringComparison.OrdinalIgnoreCase)); });
            });
        }

        /// <summary>
        ///     Saves the data in the specified stream in a binary format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="data">The data.</param>
        public static void Serialize(this IStream stream, object data)
        {
            // Use the COM wrapper for the stream.
            using (ComStream cs = new ComStream(stream))
            {
                // Write the object into the memory stream in binary format.
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, data);

                    byte[] buffer = ms.ToArray();
                    byte[] header = BitConverter.GetBytes(buffer.Length);
                    cs.Write(header, 0, header.Length); // Write the size of the object.
                    cs.Write(buffer, 0, buffer.Length); // Write the object.
                }
            }
        }

        #endregion
    }
}