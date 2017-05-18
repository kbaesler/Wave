using System.Globalization;
using System.Windows.Data;

namespace System.Windows.Converters
{
    /// <summary>
    ///     A converter that converts the null value to either <see cref="Visibility.Collapsed" /> if the value is
    ///     <c>false</c>; otherwise <see cref="Visibility.Visible" /> when the value is <c>true</c>.
    /// </summary>
    [ValueConversion(typeof (object), typeof (Visibility))]
    public class NullToVisibilityConverter : BoolToValueConverter<Visibility>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NullToVisibilityConverter" /> class.
        /// </summary>
        public NullToVisibilityConverter()
            : base(Visibility.Visible, Visibility.Collapsed)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to uSE.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? this.TrueValue : this.FalseValue;
        }

        /// <summary>
        ///     Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to uSE.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? this.TrueValue : this.FalseValue;
        }

        #endregion
    }
}