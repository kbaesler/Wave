namespace System.Windows.Controls
{
    /// <summary>
    ///     Provides access to the token value and delimiter.
    /// </summary>
    public class Token
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Token" /> class.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="value">The input text.</param>
        public Token(string delimiter, string value)
        {
            Delimiter = delimiter;
            Content = value;
            Key = Guid.NewGuid().ToString();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>
        ///     The content.
        /// </value>
        public object Content { get; set; }

        /// <summary>
        ///     Gets the delimiter.
        /// </summary>
        /// <value>
        ///     The delimiter.
        /// </value>
        public string Delimiter { get; private set; }

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public string Key { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}", Content);
        }

        #endregion
    }
}