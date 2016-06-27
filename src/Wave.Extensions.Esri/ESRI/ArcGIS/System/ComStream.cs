using System;
using System.IO;
using System.Runtime.InteropServices;

using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     Provides a .NET Stream for the <see cref="ESRI.ArcGIS.esriSystem.IStream" /> COM stream interface.
    /// </summary>
    public class ComStream : Stream
    {
        #region Fields

        /// <summary>
        ///     The COM stream being wrapped
        /// </summary>
        private global::System.Runtime.InteropServices.ComTypes.IStream _Stream;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComStream" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public ComStream(IStream stream)
            : this((global::System.Runtime.InteropServices.ComTypes.IStream) stream)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComStream" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public ComStream(global::System.Runtime.InteropServices.ComTypes.IStream stream)
        {
            _Stream = stream;
        }

        #endregion

        #region Destructors

        /// <summary>
        ///     Releases unmanaged resources and performs other cleanup operations before the
        ///     <see cref="ComStream" /> is reclaimed by garbage collection.
        /// </summary>
        ~ComStream()
        {
            Close();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>
        ///     true if the stream supports reading; otherwise, false.
        /// </returns>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>
        ///     true if the stream supports seeking; otherwise, false.
        /// </returns>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        ///     When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>
        ///     true if the stream supports writing; otherwise, false.
        /// </returns>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        ///     When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        ///     A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        ///     A class derived from Stream does not support seeking.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        public override long Length
        {
            get
            {
                STATSTG stat;
                _Stream.Stat(out stat, 1);

                return stat.cbSize;
            }
        }

        /// <summary>
        ///     When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        ///     The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        ///     An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support seeking.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        public override long Position
        {
            get { return Seek(0, SeekOrigin.Current); }
            set { Seek(value, SeekOrigin.Begin); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Closes the current stream and releases any resources (such as sockets and file handles) associated with the current
        ///     stream.
        /// </summary>
        public override void Close()
        {
            if (_Stream != null)
            {
                _Stream.Commit(0);

                if (Marshal.IsComObject(_Stream))
                    Marshal.ReleaseComObject(_Stream);

                _Stream = null;
            }
        }

        /// <summary>
        ///     When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written
        ///     to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        ///     An I/O error occurs.
        /// </exception>
        public override void Flush()
        {
            _Stream.Commit(0);
        }

        /// <summary>
        ///     When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position
        ///     within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the buffer contains the specified byte array with the
        ///     values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced
        ///     by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read
        ///     from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        ///     The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many
        ///     bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support reading.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            byte[] read = new byte[count];

            IntPtr length = new IntPtr();
            _Stream.Read(read, count, length);

            int pos = 0;
            while (pos < count)
                buffer[offset++] = read[pos++];

            return count;
        }

        /// <summary>
        ///     When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">
        ///     A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to
        ///     obtain the new position.
        /// </param>
        /// <returns>
        ///     The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        ///     An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            IntPtr position = new IntPtr();
            _Stream.Seek(offset, (int) origin, position);

            return position.ToInt64();
        }

        /// <summary>
        ///     When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">
        ///     An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console
        ///     output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        public override void SetLength(long value)
        {
            _Stream.SetSize(value);
        }

        /// <summary>
        ///     When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
        ///     position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. This method copies <paramref name="count" /> bytes from
        ///     <paramref name="buffer" /> to the current stream.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the
        ///     current stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support writing.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] write = new byte[count];
            for (int i = 0; i < count; i++)
                write[i] = buffer[offset + i];

            _Stream.Write(write, count, IntPtr.Zero);
        }

        #endregion
    }
}