using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Behaviors
{
    /// <summary>
    /// Behaviors for the text box.
    /// </summary>
    public class TextBoxBehavior
    {
        #region Fields

        /// <summary>
        ///     The select all text on focus property
        /// </summary>
        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllTextOnFocus",
                typeof (bool),
                typeof (TextBoxBehavior),
                new UIPropertyMetadata(false, OnSelectAllTextOnFocusChanged));

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the select all text on focus.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <returns></returns>
        public static bool GetSelectAllTextOnFocus(TextBox textBox)
        {
            return (bool) textBox.GetValue(SelectAllTextOnFocusProperty);
        }

        /// <summary>
        ///     Sets the select all text on focus.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetSelectAllTextOnFocus(TextBox textBox, bool value)
        {
            textBox.SetValue(SelectAllTextOnFocusProperty, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Ignores the mouse button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private static void IgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || textBox.IsKeyboardFocusWithin) return;

            e.Handled = true;
            textBox.Focus();
        }

        /// <summary>
        ///     Called when [select all text on focus changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnSelectAllTextOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox == null) return;

            if (e.NewValue is bool == false) return;

            if ((bool) e.NewValue)
            {
                textBox.GotFocus += SelectAll;
                textBox.PreviewMouseDown += IgnoreMouseButton;
            }
            else
            {
                textBox.GotFocus -= SelectAll;
                textBox.PreviewMouseDown -= IgnoreMouseButton;
            }
        }

        /// <summary>
        ///     Selects all.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private static void SelectAll(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox == null) return;

            textBox.SelectAll();
            textBox.Focus();
        }

        #endregion
    }
}