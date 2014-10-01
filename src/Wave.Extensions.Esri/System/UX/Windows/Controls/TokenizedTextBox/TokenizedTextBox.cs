using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    ///     A textbox that allows for creating tokens based on the delimiters.
    /// </summary>
    public class TokenizedTextBox : RichTextBox
    {
        #region Fields

        /// <summary>
        ///     The token delimiter property
        /// </summary>
        public static readonly DependencyProperty TokenDelimiterProperty =
            DependencyProperty.Register("TokenDelimiter", typeof (string), typeof (TokenizedTextBox));

        /// <summary>
        ///     The token template property
        /// </summary>
        public static readonly DependencyProperty TokenTemplateProperty =
            DependencyProperty.Register("TokenTemplate", typeof (DataTemplate), typeof (TokenizedTextBox));

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenizedTextBox" /> class.
        /// </summary>
        public TokenizedTextBox()
        {
            this.TextChanged += OnTextChanged;
            this.CommandBindings.Add(new CommandBinding(TokenizedTextBoxCommands.Delete, DeleteToken));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the token delimiter.
        /// </summary>
        /// <value>
        ///     The token delimiter.
        /// </value>
        public string TokenDelimiter
        {
            get { return (string) this.GetValue(TokenDelimiterProperty); }
            set { this.SetValue(TokenDelimiterProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the token template.
        /// </summary>
        /// <value>
        ///     The token template.
        /// </value>
        public DataTemplate TokenTemplate
        {
            get { return (DataTemplate) this.GetValue(TokenTemplateProperty); }
            set { this.SetValue(TokenTemplateProperty, value); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the token container.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     Returns the <see cref="InlineUIContainer" /> representing the container.
        /// </returns>
        private InlineUIContainer CreateTokenContainer(string inputText, object token)
        {
            var presenter = new Token(this.TokenDelimiter)
            {
                Content = token,
                ContentTemplate = TokenTemplate,
            };

            // BaselineAlignment is needed to align with Run
            return new InlineUIContainer(presenter) {BaselineAlignment = BaselineAlignment.TextBottom};
        }

        /// <summary>
        ///     Deletes the token.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void DeleteToken(object sender, ExecutedRoutedEventArgs e)
        {
            var para = this.CaretPosition.Paragraph;
            if (para != null)
            {
                Inline inlineToRemove = para.Inlines.Where(inline =>
                {
                    var inlineUiContainer = inline as InlineUIContainer;
                    if (inlineUiContainer != null)
                    {
                        var tokenContentControl = inlineUiContainer.Child as Token;
                        return tokenContentControl != null && (tokenContentControl.Content.Equals(e.Parameter));
                    }

                    return false;
                }).FirstOrDefault();

                if (inlineToRemove != null)
                    para.Inlines.Remove(inlineToRemove);
            }
        }

        /// <summary>
        ///     Handles when the Text is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = this.CaretPosition.GetTextInRun(LogicalDirection.Backward);
            var token = this.Tokenize(text);
            if (!string.IsNullOrEmpty(token))
            {
                this.ReplaceTextWithToken(text, token);
            }
        }

        /// <summary>
        ///     Replaces the text with token.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="token">The token.</param>
        private void ReplaceTextWithToken(string inputText, object token)
        {
            this.TextChanged -= OnTextChanged;

            try
            {
                var para = this.CaretPosition.Paragraph;
                if (para != null)
                {
                    var matchedRun = para.Inlines.FirstOrDefault(inline =>
                    {
                        var run = inline as Run;
                        return (run != null && run.Text.EndsWith(inputText));
                    }) as Run;

                    if (matchedRun != null) // Found a Run that matched the inputText
                    {
                        var tokenContainer = this.CreateTokenContainer(inputText, token);
                        para.Inlines.InsertBefore(matchedRun, tokenContainer);

                        // Remove only if the Text in the Run is the same as inputText, else split up
                        if (matchedRun.Text == inputText)
                        {
                            para.Inlines.Remove(matchedRun);
                        }
                        else // Split up
                        {
                            var index = matchedRun.Text.IndexOf(inputText, StringComparison.Ordinal) + inputText.Length;
                            var tailEnd = new Run(matchedRun.Text.Substring(index));
                            para.Inlines.InsertAfter(matchedRun, tailEnd);
                            para.Inlines.Remove(matchedRun);
                        }
                    }
                }
            }
            finally
            {
                this.TextChanged += OnTextChanged;
            }
        }

        /// <summary>
        ///     Tokenizes the specified text into a token.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Returns the <see cref="string" /> representing the token.</returns>
        private string Tokenize(string text)
        {
            if (text.EndsWith(this.TokenDelimiter))
            {
                return text.Substring(0, text.Length - 1).Trim();
            }

            return null;
        }

        #endregion
    }
}