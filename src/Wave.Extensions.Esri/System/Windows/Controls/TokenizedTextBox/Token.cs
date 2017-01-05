using System.Collections;

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
            Value = value;
            Key = Guid.NewGuid().ToString();
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

        /// <summary>
        ///     Gets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public string Value { get; set; }

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
            return Value;
        }

        #endregion
    }

    /// <summary>
    ///     A collection of tokens.
    /// </summary>
    /// <seealso cref="string.Windows.Controls.Token}" />
    public class TokenCollection : ObservableKeyedCollection<string, Token>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenCollection" /> class.
        /// </summary>
        public TokenCollection()
            : base(token => token.Key)
        {
        }

        #endregion
    }
}