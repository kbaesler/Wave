using System.Globalization;
using System.Windows.Data;

namespace System.Windows.Converters
{
    /// <summary>
    ///     A generic boolean to type converter.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class BoolToValueConverter<TValue> : IValueConverter
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoolToValueConverter{TValue}" /> class.
        /// </summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        protected BoolToValueConverter(TValue trueValue, TValue falseValue)
        {
            this.TrueValue = trueValue;
            this.FalseValue = falseValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the false value.
        /// </summary>
        /// <value>
        ///     The false value.
        /// </value>
        public TValue FalseValue { get; set; }

        /// <summary>
        ///     Gets or sets the true value.
        /// </summary>
        /// <value>
        ///     The true value.
        /// </value>
        public TValue TrueValue { get; set; }

        #endregion

        #region IValueConverter Members

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return this.FalseValue;

            return (bool) value ? this.TrueValue : this.FalseValue;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(this.TrueValue);
        }

        #endregion
    }
}