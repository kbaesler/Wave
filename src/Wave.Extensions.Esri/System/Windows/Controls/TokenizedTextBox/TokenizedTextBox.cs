using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace System.Windows.Controls
{
    /// <summary>
    ///     A RichTextBox control that parses text on the fly. Once a delimiter (e.g., ";") is detected,
    ///     this control converts the text preceding the delimeter into a "token", which is a distinct
    ///     UI element.
    ///     This code is adapted from http://blog.pixelingene.com/2010/10/tokenizing-control-convert-text-to-tokens/
    /// </summary>
    public class TokenizedTextBox : RichTextBox
    {
        #region Fields

        /// <summary>
        ///     The text property
        /// </summary>
        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register("Text", typeof (string), typeof (TokenizedTextBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextPropertyChanged, CoerceTextProperty, true, UpdateSourceTrigger.LostFocus));

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


        private static bool _SuppressTextChanged;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenizedTextBox" /> class.
        /// </summary>
        public TokenizedTextBox()
        {
            this.AcceptsReturn = false;
            this.IsDocumentEnabled = true;

            this.TextChanged += OnTextChanged;
            this.CommandBindings.Add(new CommandBinding(TokenizedTextBoxCommands.Delete, DeleteToken));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string Text
        {
            get { return (string) this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

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
        ///     Clears this instance.
        /// </summary>
        private void Clear()
        {
            this.Document.Blocks.Clear();
        }

        /// <summary>
        ///     Coerces the text property.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static object CoerceTextProperty(DependencyObject d, object value)
        {
            return value ?? "";
        }

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
            var presenter = new Token(this.TokenDelimiter, inputText)
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
                        return tokenContentControl != null && (tokenContentControl.Key.Equals(e.Parameter));
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
            if (_SuppressTextChanged)
                return;

            var text = this.CaretPosition.GetTextInRun(LogicalDirection.Backward);
            var tokens = text.Split(new[] {this.TokenDelimiter}, StringSplitOptions.RemoveEmptyEntries);
            var token = this.Tokenize(text);

            if (!string.IsNullOrEmpty(token))
            {
                this.ReplaceTextWithToken(text, token);
            }
            else if (tokens.Length > 1)
            {
                // When copy-paste is used the caret position will be the end of the pasted text.
                // and when multiple "tokens" have been pasted, they need to be broken into individual tokens
                // thus the existing run needs to be deleted and each token needs to be created as a "Run".               
                var para = this.CaretPosition.Paragraph;
                if (para != null)
                {
                    var matchedRun = para.Inlines.FirstOrDefault(inline =>
                    {
                        var run = inline as Run;
                        return (run != null && run.Text.Equals(text));
                    }) as Run;

                    if (matchedRun != null)
                    {
                        para.Inlines.Remove(matchedRun);

                        foreach (var t in tokens)
                        {
                            var textData = t + this.TokenDelimiter;
                            para.Inlines.Add(new Run(textData));

                            this.ReplaceTextWithToken(textData, t);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the Text dependency propery is changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency property.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnTextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TokenizedTextBox tokenizedTextBox = (TokenizedTextBox) dependencyObject;

            // To help with performance this is placed on the dispatcher for processing. For some reason when this is done the TextChanged event is fired multiple times
            // forcing the UpdateText method to be called multiple times and the setter of the source property to be set multiple times. 
            // To fix this, we simply set the suppress property
            // member to true before the operation and set it to false when the operation completes. This will prevent the Text property from being set multiple times.
            DispatcherOperation dop = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate()
            {
                _SuppressTextChanged = true;

                string text = e.NewValue as string;
                if (string.IsNullOrEmpty(text))
                {
                    tokenizedTextBox.Clear();
                }
            }), DispatcherPriority.Background);
            dop.Completed += (sender, ea) =>
            {
                tokenizedTextBox.ReplaceTextWithTokens();

                _SuppressTextChanged = false;
            };
        }


        /// <summary>
        ///     Replaces the text with token.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="token">The token.</param>
        private void ReplaceTextWithToken(string inputText, string token)
        {
            _SuppressTextChanged = true;

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
                _SuppressTextChanged = false;
            }
        }


        /// <summary>
        ///     Replaces the strings in the Text property with tokens.
        /// </summary>
        private void ReplaceTextWithTokens()
        {
            // The "Text" property is not linked to the RichTextBox contents, thus we need to clear the RichTextBox
            // and add each token individually to the contents.
            this.Clear();

            if (!string.IsNullOrEmpty(this.Text))
            {
                var tokens = this.Text.Split(new[] {this.TokenDelimiter}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var t in tokens)
                {
                    string textData = t + this.TokenDelimiter;
                    this.AppendText(textData);

                    var text = this.CaretPosition.GetTextInRun(LogicalDirection.Forward);
                    var token = this.Tokenize(textData);

                    if (!string.IsNullOrEmpty(token))
                    {
                        this.ReplaceTextWithToken(text, token);
                    }
                }
            }
        }

        /// <summary>
        ///     Tokenizes the specified text into a token.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Returns the <see cref="string" /> representing the token.</returns>
        private string Tokenize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            if (text.EndsWith(this.TokenDelimiter))
                return text.Substring(0, text.Length - 1).Trim();

            return null;
        }

        #endregion
    }
}