using System.Windows.Controls;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Provides the ability to DataBinding to the <see cref="PasswordBox" /> using a dependency property.
    /// </summary>
    public static class PasswordBoxBehavior
    {
        #region Fields

        /// <summary>
        ///     The dependency property for the updating property.
        /// </summary>
        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof (bool),
                typeof (PasswordBoxBehavior));

        /// <summary>
        ///     The dependency property for the password property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
                typeof (string), typeof (PasswordBoxBehavior),
                new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the Password dependency property.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static string GetPassword(DependencyObject dp)
        {
            return (string) dp.GetValue(PasswordProperty);
        }

        /// <summary>
        ///     Sets the Password dependency property.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">The value.</param>
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the IsUpdating dependency property.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool) dp.GetValue(IsUpdatingProperty);
        }

        /// <summary>
        ///     Handles when the Password dependency property changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.PasswordChanged -= PasswordChanged;

                if (!GetIsUpdating(passwordBox))
                {
                    passwordBox.Password = (string) e.NewValue;
                }
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        ///     Handles the <see cref="PasswordChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            if (passwordBox != null)
            {
                SetPassword(passwordBox, passwordBox.Password);
                SetIsUpdating(passwordBox, false);
            }
        }

        /// <summary>
        ///     Sets the IsUpdating dependency property.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        #endregion
    }
}