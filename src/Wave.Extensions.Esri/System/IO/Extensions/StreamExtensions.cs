namespace System.IO.Extensions
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Stream" /> class.
    /// </summary>
    public static class StreamExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Copies the contents of input to output.
        /// </summary>
        /// <param name="source">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="size">The size.</param>
        /// <remarks>
        ///     Doesn't close either stream.
        /// </remarks>
        public static void CopyStream(this Stream source, Stream output, int size)
        {
            byte[] buffer = new byte[8*size];
            int len;
            while ((len = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        /// <summary>
        ///     Copies the contents of input to output.
        /// </summary>
        /// <param name="source">The input.</param>
        /// <param name="output">The output.</param>
        /// <remarks>Doesn't close either stream.</remarks>
        public static void CopyStream(this Stream source, Stream output)
        {
            source.CopyStream(output, 1024);
        }

        #endregion
    }
}