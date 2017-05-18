namespace System.Windows.Controls
{
    /// <summary>
    /// The containiner used to host the token.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ContentControl" />
    public class TokenContainer : ContentControl
    {
        #region Fields

        /// <summary>
        ///     The key property
        /// </summary>
        public static readonly DependencyProperty KeyProperty
            = DependencyProperty.Register("Key", typeof(string), typeof(TokenContainer), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="TokenContainer" /> class.
        /// </summary>
        static TokenContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TokenContainer), new FrameworkPropertyMetadata(typeof(TokenContainer)));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenContainer" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public TokenContainer(string key)
        {
            this.Key = key;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        #endregion
    }
}