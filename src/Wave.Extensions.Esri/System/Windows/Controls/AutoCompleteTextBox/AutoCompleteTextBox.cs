using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace System.Windows.Controls
{
    /// <summary>
    ///     A <see cref="TextBox" /> control that supports type-ahead (or auto complete).
    /// </summary>
    public class AutoCompleteTextBox : Control, IDisposable
    {
        #region Fields

        private readonly ComboBox _ComboBox;
        private readonly VisualCollection _Controls;
        private readonly DelayedTextBox _TextBox;

        /// <summary>
        ///     The automatic complete source property
        /// </summary>
        public static readonly DependencyProperty AutoCompleteSourceProperty =
            DependencyProperty.Register("AutoCompleteSource", typeof(IEnumerable<string>), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(OnAutoCompleteSourceChanged));


        /// <summary>
        ///     The ignore case property
        /// </summary>
        public static readonly DependencyProperty IgnoreCaseProperty =
            DependencyProperty.Register("IgnoreCase", typeof(bool), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        ///     The text property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoCompleteTextBox" /> class.
        /// </summary>
        public AutoCompleteTextBox()
        {
            _Controls = new VisualCollection(this);

            // Set up the text box and the combo box
            _ComboBox = new ComboBox();
            _ComboBox.IsSynchronizedWithCurrentItem = true;
            _ComboBox.IsTabStop = false;
            _ComboBox.MaxDropDownHeight = 150;
            _ComboBox.SelectionChanged += ComboBox_SelectionChanged;

            _TextBox = new DelayedTextBox();
            _TextBox.VerticalContentAlignment = VerticalAlignment.Center;
            _TextBox.DelayedTextChanging += TextBox_DelayedTextChanging;
            _TextBox.TextChanged += TextBox_OnTextChanged;
            _TextBox.KeyDown += TextBox_OnKeyDown;
            _TextBox.DelayedTextChanged += TextBox_DelayedTextChanged;
            _TextBox.PreviewKeyDown += TextBox_OnPreviewKeyDown;

            _Controls.Add(_ComboBox);
            _Controls.Add(_TextBox);
        }

        #endregion

        #region Events

        /// <summary>
        ///     The text changed
        /// </summary>
        public event TextChangedEventHandler TextChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is delayed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is delayed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDelayed
        {
            get { return _TextBox.IsDelayed; }
        }

        /// <summary>
        ///     Gets or sets the automatic complete source.
        /// </summary>
        /// <value>
        ///     The automatic complete source.
        /// </value>
        public IEnumerable<string> AutoCompleteSource
        {
            get { return (IEnumerable<string>) this.GetValue(AutoCompleteSourceProperty); }
            set { this.SetValue(AutoCompleteSourceProperty, value); }
        }

        /// <summary>
        ///     Gets and Sets the amount of time (in miliseconds) to wait after the text has changed before updating the binding.
        /// </summary>
        public int DelayTime
        {
            get { return _TextBox.DelayTime; }
            set { _TextBox.DelayTime = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is ignore case.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is ignore case; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreCase
        {
            get { return (bool) this.GetValue(IgnoreCaseProperty); }
            set { this.SetValue(IgnoreCaseProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the maximum height of the drop down.
        /// </summary>
        /// <value>
        ///     The maximum height of the drop down.
        /// </value>
        public double MaxDropDownHeight
        {
            get { return _ComboBox.MaxDropDownHeight; }
            set { _ComboBox.MaxDropDownHeight = value; }
        }

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
        ///     Gets or sets the minium number of characters before populating the drop down.
        /// </summary>
        /// <value>
        ///     The threshold.
        /// </value>
        public int Threshold { get; set; }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the number of child <see cref="T:System.Windows.Media.Visual" /> objects in this instance of
        ///     <see cref="T:System.Windows.Controls.Panel" />.
        /// </summary>
        /// <returns>
        ///     The number of child <see cref="T:System.Windows.Media.Visual" /> objects.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get { return _Controls.Count; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Arranges the content of a <see cref="T:System.Windows.Controls.Canvas" /> element.
        /// </summary>
        /// <param name="arrangeSize">
        ///     The size that this <see cref="T:System.Windows.Controls.Canvas" /> element should use to
        ///     arrange its child elements.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.Windows.Size" /> that represents the arranged size of this
        ///     <see cref="T:System.Windows.Controls.Canvas" /> element and its descendants.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            _TextBox.Arrange(new Rect(arrangeSize));
            _ComboBox.Arrange(new Rect(arrangeSize));

            return base.ArrangeOverride(arrangeSize);
        }

        /// <summary>
        ///     Gets a <see cref="T:System.Windows.Media.Visual" /> child of this <see cref="T:System.Windows.Controls.Panel" /> at
        ///     the specified index position.
        /// </summary>
        /// <param name="index">The index position of the <see cref="T:System.Windows.Media.Visual" /> child.</param>
        /// <returns>
        ///     A <see cref="T:System.Windows.Media.Visual" /> child of the parent <see cref="T:System.Windows.Controls.Panel" />
        ///     element.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return _Controls[index];
        }

        /// <summary>
        ///     Raises the <see cref="E:TextChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        protected void OnTextChanged(TextChangedEventArgs e)
        {
            var eventHandler = this.TextChanged;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles the SelectionChanged event of the ComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_ComboBox.SelectedItem != null)
            {
                var item = (ComboBoxItem) _ComboBox.SelectedItem;
                var text = string.Format("{0}", item.Tag);
                _TextBox.Text = text;
                _TextBox.SelectAll();
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="dissposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool dissposing)
        {
            if (dissposing)
            {
                _TextBox.Dispose();
            }
        }

        /// <summary>
        ///     Called when <see cref="AutoCompleteTextBox.AutoCompleteSource" /> dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnAutoCompleteSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox autoCompleteTextBox = dependencyObject as AutoCompleteTextBox;
            if (autoCompleteTextBox != null)
                autoCompleteTextBox.AutoCompleteSource = (IEnumerable<string>) e.NewValue;
        }

        /// <summary>
        ///     Called when <see cref="AutoCompleteTextBox.Text" /> dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox autoCompleteTextBox = dependencyObject as AutoCompleteTextBox;
            if (autoCompleteTextBox != null)
                autoCompleteTextBox.Text = (string) e.NewValue;
        }

        /// <summary>
        ///     Populates the combo box and shows the drop-down.
        /// </summary>
        private void ShowDropDown()
        {
            _ComboBox.Items.Clear();

            if (_TextBox.Text.Length > this.Threshold && this.AutoCompleteSource != null && _TextBox.IsEnabled)
            {
                StringComparison comparison = (this.IgnoreCase) ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

                foreach (string source in this.AutoCompleteSource)
                {
                    if (source.StartsWith(_TextBox.Text, comparison))
                    {
                        var textBlock = new TextBlock();
                        textBlock.Inlines.Add(new Run
                        {
                            Text = source.Substring(0, _TextBox.Text.Length),
                            FontWeight = FontWeights.Bold
                        });
                        textBlock.Inlines.Add(new Run
                        {
                            Text = source.Substring(_TextBox.Text.Length)
                        });

                        _ComboBox.Items.Add(new ComboBoxItem
                        {
                            Content = textBlock,
                            Tag = source
                        });
                    }
                }

                if (_ComboBox.Items.Count == 1)
                    _ComboBox.SelectedIndex = 0;
                else
                    _ComboBox.IsDropDownOpen = _ComboBox.HasItems;
            }
            else
            {
                _ComboBox.IsDropDownOpen = false;
            }
        }

        /// <summary>
        ///     Handles the DelayedTextChanged event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TextBox_DelayedTextChanged(object sender, EventArgs eventArgs)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) this.ShowDropDown);
        }

        /// <summary>
        ///     Handles the DelayedTextChanging event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void TextBox_DelayedTextChanging(object sender, TextChangedEventArgs e)
        {
            this.OnTextChanged(e);
        }

        /// <summary>
        ///     Handles the OnKeyDown event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (_ComboBox.IsDropDownOpen && _ComboBox.Items.Count == 1)
                    _ComboBox.SelectedIndex = 0;

                _ComboBox.IsDropDownOpen = false;
            }
        }

        /// <summary>
        ///     Handles the OnPreviewKeyDown event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_ComboBox.IsDropDownOpen && e.Key == Key.Down)
            {
                _ComboBox.Focus();
            }
        }

        /// <summary>
        ///     Handles the OnTextChanged event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.Text = ((DelayedTextBox) sender).Text;
            this.OnTextChanged(e);
        }

        #endregion
    }
}