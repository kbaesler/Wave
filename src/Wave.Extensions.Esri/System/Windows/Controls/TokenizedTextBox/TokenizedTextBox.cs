using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        ///     The items source property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof (IEnumerable<object>), typeof (TokenizedTextBox), new FrameworkPropertyMetadata(OnItemsSourceChanged));

        /// <summary>
        ///     The member path property
        /// </summary>
        public static readonly DependencyProperty MemberPathProperty =
            DependencyProperty.Register("MemberPath", typeof (string), typeof (TokenizedTextBox), new UIPropertyMetadata(string.Empty));

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
            AcceptsReturn = false;
            IsDocumentEnabled = true;

            TextChanged += OnTextChanged;
            CommandBindings.Add(new CommandBinding(TokenizedTextBoxCommands.Delete, DeleteToken));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the items source.
        /// </summary>
        /// <value>
        ///     The items source.
        /// </value>
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value member path.
        /// </summary>
        /// <value>
        ///     The value member path.
        /// </value>
        public string MemberPath
        {
            get { return (string) GetValue(MemberPathProperty); }
            set { SetValue(MemberPathProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the token delimiter.
        /// </summary>
        /// <value>
        ///     The token delimiter.
        /// </value>
        public string TokenDelimiter
        {
            get { return (string) GetValue(TokenDelimiterProperty); }
            set { SetValue(TokenDelimiterProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the token template.
        /// </summary>
        /// <value>
        ///     The token template.
        /// </value>
        public DataTemplate TokenTemplate
        {
            get { return (DataTemplate) GetValue(TokenTemplateProperty); }
            set { SetValue(TokenTemplateProperty, value); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets or sets the tokens.
        /// </summary>
        /// <value>
        ///     The tokens.
        /// </value>
        /// <summary>
        ///     Clears this instance.
        /// </summary>
        private void Clear()
        {
            Document.Blocks.Clear();
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
        /// <param name="token">The token.</param>
        /// <returns>
        ///     Returns the <see cref="InlineUIContainer" /> representing the container.
        /// </returns>
        private InlineUIContainer CreateTokenContainer(Token token)
        {
            var presenter = new TokenContainer(token.Key)
            {
                Content = token,
                ContentTemplate = TokenTemplate,
            };

            if (this.TokenTemplate == null && token.Content != null)
            {
                if (!string.IsNullOrEmpty(this.MemberPath))
                {
                    var property = token.Content.GetType().GetProperty(this.MemberPath);
                    if (property != null)
                    {
                        var value = property.GetValue(token.Content, null);
                        if (value != null)
                            presenter.Content = value;
                    }
                }
            }

            return new InlineUIContainer(presenter)
            {
                BaselineAlignment = BaselineAlignment.TextBottom
            };
        }

        /// <summary>
        ///     Deletes the token.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void DeleteToken(object sender, ExecutedRoutedEventArgs e)
        {
            Paragraph para = CaretPosition.Paragraph;
            if (para != null)
            {
                Inline inlineToRemove = para.Inlines.Where(inline =>
                {
                    var inlineUiContainer = inline as InlineUIContainer;
                    if (inlineUiContainer != null)
                    {
                        TokenContainer tokenContainer = inlineUiContainer.Child as TokenContainer;
                        return tokenContainer != null && (tokenContainer.Key.Equals(e.Parameter));
                    }

                    return false;
                }).FirstOrDefault();

                if (inlineToRemove != null)
                {
                    para.Inlines.Remove(inlineToRemove);
                }
            }
        }

        /// <summary>
        ///     Gets the item by token key.
        /// </summary>
        /// <param name="token">The key.</param>
        /// <returns></returns>
        private object GetContentByToken(Token token)
        {
            if (this.ItemsSource != null)
            {
                foreach (object item in this.ItemsSource)
                {
                    var property = item.GetType().GetProperty(this.MemberPath);
                    if (property != null)
                    {
                        var value = property.GetValue(item, null);
                        if (token.Key.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            return item;
                    }
                }
            }

            return token.Content;
        }

        /// <summary>
        ///     Gets the token by item source.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private Token GetTokenByItemSource(string text)
        {
            if (this.ItemsSource != null)
            {
                foreach (var item in this.ItemsSource)
                {
                    var property = item.GetType().GetProperty(this.MemberPath);
                    if (property != null)
                    {
                        var value = property.GetValue(item, null);
                        if (text.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            Token token = new Token(this.TokenDelimiter, value.ToString());
                            return token;
                        }
                    }
                }
            }

            return new Token(this.TokenDelimiter, text);
        }

        /// <summary>
        ///     Called when the ItemsSource dependency propery is changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency property.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var tokenizedTextBox = (TokenizedTextBox) dependencyObject;

            if (e.NewValue != null)
            {
                var text = new StringBuilder();

                foreach (var item in (IEnumerable<object>) e.NewValue)
                {
                    var property = item.GetType().GetProperty(tokenizedTextBox.MemberPath);
                    if (property != null)
                    {
                        var value = property.GetValue(item, null);
                        text.AppendFormat("{0}{1}", value, tokenizedTextBox.TokenDelimiter);
                    }
                    else
                    {
                        text.AppendFormat("{0}{1}", item, tokenizedTextBox.TokenDelimiter);
                    }
                }

                tokenizedTextBox.Text = text.ToString();
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

            string text = CaretPosition.GetTextInRun(LogicalDirection.Backward);
            Token token = Tokenize(text);

            if (token != null)
            {
                ReplaceTextWithToken(text, token);
            }

            SetText(text);
        }

        /// <summary>
        ///     Called when the Text dependency propery is changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency property.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnTextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (_SuppressTextChanged)
                return;

            var tokenizedTextBox = (TokenizedTextBox) dependencyObject;

            // To help with performance this is placed on the dispatcher for processing. For some reason when this is done the TextChanged event is fired multiple times
            // forcing the UpdateText method to be called multiple times and the setter of the source property to be set multiple times. 
            // To fix this, we simply set the suppress property
            // member to true before the operation and set it to false when the operation completes. This will prevent the Text property from being set multiple times.
            DispatcherOperation dop = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
            {
                _SuppressTextChanged = true;

                var text = e.NewValue as string;
                if (string.IsNullOrEmpty(text))
                {
                    tokenizedTextBox.Clear();
                }
                else
                {
                    tokenizedTextBox.ReplaceTextWithTokens();
                }
            }), DispatcherPriority.Background);
            dop.Completed += (sender, ea) => { _SuppressTextChanged = false; };
        }


        /// <summary>
        ///     Replaces the text with token.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="token">The token.</param>
        private void ReplaceTextWithToken(string inputText, Token token)
        {
            _SuppressTextChanged = true;

            try
            {
                Paragraph para = CaretPosition.Paragraph;
                if (para != null)
                {
                    var matchedRun = para.Inlines.FirstOrDefault(inline =>
                    {
                        var run = inline as Run;
                        return (run != null && run.Text.EndsWith(inputText));
                    }) as Run;

                    if (matchedRun != null) // Found a Run that matched the inputText
                    {
                        InlineUIContainer tokenContainer = CreateTokenContainer(token);
                        para.Inlines.InsertBefore(matchedRun, tokenContainer);

                        // Remove only if the Text in the Run is the same as inputText, else split up
                        if (matchedRun.Text == inputText)
                        {
                            para.Inlines.Remove(matchedRun);
                        }
                        else // Split up
                        {
                            int index = matchedRun.Text.IndexOf(inputText, StringComparison.Ordinal) + inputText.Length;
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
            Clear();

            if (!string.IsNullOrEmpty(Text))
            {
                Paragraph para = CaretPosition.Paragraph ?? new Paragraph();
                if (para != null)
                {
                    string[] text = Text.Split(new[] {TokenDelimiter}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string t in text)
                    {
                        var token = this.GetTokenByItemSource(t);
                        token.Content = this.GetContentByToken(token);

                        InlineUIContainer tokenContainer = CreateTokenContainer(token);
                        para.Inlines.Add(tokenContainer);
                    }
                }

                if (!Document.Blocks.Contains(para))
                    Document.Blocks.Add(para);
            }
        }

        /// <summary>
        ///     Sets the text.
        /// </summary>
        /// <param name="data">The data.</param>
        private void SetText(string data)
        {
            _SuppressTextChanged = true;
            this.Text = data;
            _SuppressTextChanged = false;
        }

        /// <summary>
        ///     Tokenizes the specified text into a token.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Returns the <see cref="string" /> representing the token.</returns>
        private Token Tokenize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            if (text.EndsWith(TokenDelimiter))
            {
                string item = text.Substring(0, text.Length - 1).Trim();

                var token = this.GetTokenByItemSource(item);
                if (token == null)
                {
                    return new Token(this.TokenDelimiter, item);
                }

                return token;
            }

            return null;
        }

        #endregion
    }
}