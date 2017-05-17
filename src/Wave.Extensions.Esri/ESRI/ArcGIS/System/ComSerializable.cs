using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using ESRI.ArcGIS.ADF;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     Provides the ability to serialize an ESRI object using the <see cref="BinaryFormatter" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Runtime.Serialization.ISerializable" />
    [Serializable]
    public class ComSerializable<T> : ISerializable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComSerializable{T}" /> class.
        /// </summary>
        /// <param name="blob">The BLOB data.</param>
        /// <exception cref="ArgumentException">
        ///     The object cannot be serialized because it doesn't implement the IClass or
        ///     IPersistStream interfaces.
        /// </exception>
        public ComSerializable(T blob)
        {
            if (!(blob is IClassID) || !(blob is IPersistStream))
                throw new ArgumentException("The object cannot be serialized because it doesn't implement the IClass or IPersistStream interfaces.");

            this.Value = blob;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComSerializable{T}" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected ComSerializable(SerializationInfo info, StreamingContext context)
        {
            byte[] data = (byte[]) info.GetValue("DATA", typeof (byte[]));
            string progId = info.GetString("PROGID");

            using (ComReleaser com = new ComReleaser())
            {
                IMemoryBlobStream blob = new MemoryBlobStreamClass();
                com.ManageLifetime(blob);

                IMemoryBlobStreamVariant variant = (IMemoryBlobStreamVariant) blob;
                variant.ImportFromVariant(data);

                IObjectStream stream = new ObjectStreamClass();
                stream.Stream = blob;

                com.ManageLifetime(stream);

                Type t = Type.GetTypeFromProgID(progId);

                IPersistStream persist = (IPersistStream) Activator.CreateInstance(t);
                persist.Load(stream);

                this.Value = (T) persist;
            }            
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the prog identifier.
        /// </summary>
        /// <value>
        ///     The prog identifier.
        /// </value>
        public string ProgId
        {
            get { return ((IClassID) this.Value).GetProgID(); }
        }

        /// <summary>
        ///     Gets the data.
        /// </summary>
        /// <value>
        ///     The data.
        /// </value>
        public T Value { get; protected set; }

        #endregion

        #region ISerializable Members

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using (ComReleaser cr = new ComReleaser())
            {
                IPersistStream persist = this.Value as IPersistStream;
                if (persist == null) return;

                IObjectStream stream = new ObjectStreamClass();
                cr.ManageLifetime(stream);

                IMemoryBlobStream blob = new MemoryBlobStreamClass();
                cr.ManageLifetime(blob);

                stream.Stream = blob;

                persist.Save(stream, 0);

                IMemoryBlobStreamVariant variant = (IMemoryBlobStreamVariant) blob;

                object value;
                variant.ExportToVariant(out value);

                var data = (byte[]) value;
                info.AddValue("DATA", data);
                info.AddValue("PROGID", this.ProgId);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deserializes the specified stream from a binary format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static ComSerializable<T> Deserialize(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(stream) as ComSerializable<T>;
        }

        /// <summary>
        ///     Serializes the specified stream into a binary format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="serializable">The serializable.</param>
        public static void Serialize(Stream stream, ComSerializable<T> serializable)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, serializable);
        }

        #endregion
    }
}