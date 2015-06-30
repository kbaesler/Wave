namespace System.Windows.Controls
{
    /// <summary>
    ///     The content control for the token.
    /// </summary>
    public class Token : ContentControl
    {
        #region Fields

        /// <summary>
        ///     The key property
        /// </summary>
        public static readonly DependencyProperty KeyProperty
            = DependencyProperty.Register("Key", typeof (string), typeof (Token), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="Token" /> class.
        /// </summary>
        static Token()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (Token), new FrameworkPropertyMetadata(typeof (Token)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token" /> class.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="key">The key.</param>
        public Token(string delimiter, string key)
        {
            this.Delimiter = delimiter;
            this.Key = key;
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
        ///     Gets or sets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public string Key
        {
            get { return (string) this.GetValue(KeyProperty); }
            set { this.SetValue(KeyProperty, value); }
        }

        #endregion
    }
}