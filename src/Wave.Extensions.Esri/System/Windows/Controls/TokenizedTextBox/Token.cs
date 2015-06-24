namespace System.Windows.Controls
{
    /// <summary>
    ///     The content control for the token.
    /// </summary>
    public class Token : ContentControl
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Token" /> class.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        public Token(string delimiter)
        {
            this.Delimiter = delimiter;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the delimiter.
        /// </summary>
        /// <value>
        ///     The delimiter.
        /// </value>
        public string Delimiter { get; private set; }

        #endregion
    }
}