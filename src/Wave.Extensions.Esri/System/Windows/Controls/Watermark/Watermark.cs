using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls
{
    /// <summary>
    ///     Class that provides the Watermark attached property. The watermark can be any supported FrameworkElement.
    /// </summary>
    public static class Watermark
    {
        #region Fields

        /// <summary>
        ///     The content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
            "Content",
            typeof (FrameworkElement),
            typeof (Watermark),
            new FrameworkPropertyMetadata(null, OnContentChanged));

        /// <summary>
        ///     Dictionary of ItemsControls
        /// </summary>
        private static readonly Dictionary<object, ItemsControl> ItemsControls = new Dictionary<object, ItemsControl>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the content property.  This dependency property indicates the watermark for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject" /> to get the property from</param>
        /// <returns>The value of the content property</returns>
        public static FrameworkElement GetContent(DependencyObject d)
        {
            return (FrameworkElement) d.GetValue(ContentProperty);
        }

        /// <summary>
        ///     Sets the content property.  This dependency property indicates the watermark for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject" /> to set the property on</param>
        /// <param name="value">value of the property</param>
        public static void SetContent(DependencyObject d, FrameworkElement value)
        {
            d.SetValue(ContentProperty, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handle the GotFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs" /> that contains the event data.</param>
        private static void Control_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            Control c = (Control) sender;
            c.TryRemoveAdorners<WatermarkAdorner>();
        }

        /// <summary>
        ///     Handle the Loaded and LostFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs" /> that contains the event data.</param>
        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            Control c = (Control) sender;
            UpdateWatermark(c);
        }

        /// <summary>
        ///     Event handler for the items changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ItemsChangedEventArgs" /> that contains the event data.</param>
        private static void ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            ItemsControl c;
            if (ItemsControls.TryGetValue(sender, out c))
            {
                UpdateWatermark(c);
            }
        }

        /// <summary>
        ///     Event handler for the items source changed event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        private static void ItemsSourceChanged(object sender, EventArgs e)
        {
            ItemsControl c = (ItemsControl) sender;
            if (c.ItemsSource != null)
            {
                UpdateWatermark(c);
            }
            else
            {
                ShowWatermark(c);
            }
        }

        /// <summary>
        ///     Handles changes to the Watermark property.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject" /> that fired the event</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs" /> that contains the event data.</param>
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control) d;
            control.Loaded += Control_Loaded;

            ComboBox comboBox = d as ComboBox;
            TextBox textBox = d as TextBox;
            AutoCompleteTextBox autoCompleteTextBox = d as AutoCompleteTextBox;

            if (comboBox != null || textBox != null || autoCompleteTextBox != null)
            {
                control.GotKeyboardFocus += Control_GotKeyboardFocus;
                control.LostKeyboardFocus += Control_Loaded;

                if (textBox != null)
                    textBox.TextChanged += TextBox_TextChanged;
                else if (autoCompleteTextBox != null)
                    autoCompleteTextBox.TextChanged += TextBox_TextChanged;
            }

            ItemsControl i = d as ItemsControl;
            if (i != null && comboBox == null)
            {
                // for Items property
                i.ItemContainerGenerator.ItemsChanged += ItemsChanged;
                ItemsControls.Add(i.ItemContainerGenerator, i);

                // for ItemsSource property
                DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, i.GetType());
                prop.AddValueChanged(i, ItemsSourceChanged);
            }
        }

        /// <summary>
        ///     Indicates whether or not the watermark should be shown on the specified control
        /// </summary>
        /// <param name="c"><see cref="Control" /> to test</param>
        /// <returns>true if the watermark should be shown; false otherwise</returns>
        private static bool ShouldShowWatermark(Control c)
        {
            ComboBox comboBox = c as ComboBox;
            TextBox textBox = c as TextBox;
            ItemsControl itemsControl = c as ItemsControl;
            AutoCompleteTextBox autoCompleteTextBox = c as AutoCompleteTextBox;
            
            if (comboBox != null)
            {
                return string.IsNullOrEmpty(comboBox.Text);
            }
            if (textBox != null)
            {
                return string.IsNullOrEmpty(textBox.Text);
            }
            if (itemsControl != null)
            {
                return itemsControl.Items.Count == 0;
            }
            if (autoCompleteTextBox != null)
            {
                if (autoCompleteTextBox.IsDelayed)
                    return false;

                return string.IsNullOrEmpty(autoCompleteTextBox.Text);
            }
            return false;
        }

        /// <summary>
        ///     Show the watermark on the specified control
        /// </summary>
        /// <param name="control">Control to show the watermark on</param>
        private static void ShowWatermark(Control control)
        {
            // Remove the old watermark.
            control.TryRemoveAdorners<WatermarkAdorner>();

            // Add the new watermark.
            FrameworkElement content = GetContent(control);
            control.TryAddAdorner<WatermarkAdorner>(new WatermarkAdorner(control, content));
        }

        /// <summary>
        ///     Handles the TextChanged event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs" /> instance containing the event data.</param>
        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Control c = (Control) sender;
            UpdateWatermark(c);
        }

        /// <summary>
        ///     Updates the watermark.
        /// </summary>
        /// <param name="control">The control.</param>
        private static void UpdateWatermark(Control control)
        {
            if (ShouldShowWatermark(control))
            {
                ShowWatermark(control);
            }
            else
            {
                control.TryRemoveAdorners<WatermarkAdorner>();
            }
        }

        #endregion
    }
}