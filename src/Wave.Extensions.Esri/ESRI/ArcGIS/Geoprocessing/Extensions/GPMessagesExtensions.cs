using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGPMessages" /> interface.
    /// </summary>
    public static class GPMessagesExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds a new message to the <paramref name="source" /> of messages.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args"> An System.Object array containing zero or more objects to format.</param>
        public static void Add(this IGPMessages source, esriGPMessageType messageType, string format, params object[] args)
        {
            source.Add(new GPMessageClass()
            {
                Description = string.Format(format, args),
                Type = messageType
            });
        }

        /// <summary>
        ///     Adds a new message to the <paramref name="source" /> of messages.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args"> An System.Object array containing zero or more objects to format.</param>
        public static void Add(this IGPMessages source, esriGPMessageType messageType, int errorCode, string format, params object[] args)
        {
            source.Add(new GPMessageClass()
            {
                Description = string.Format(format, args),
                Type = messageType,
                ErrorCode = errorCode
            });
        }

        /// <summary>
        ///     Adds the error to the messages.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void AddError(this IGPMessages source, int errorCode, string format, params object[] args)
        {
            source.AddError(errorCode, string.Format(format, args));
        }

        /// <summary>
        ///     Adds the message to the messages.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void AddMessage(this IGPMessages source, string format, params object[] args)
        {
            source.AddMessage(string.Format(format, args));
        }

        #endregion
    }
}