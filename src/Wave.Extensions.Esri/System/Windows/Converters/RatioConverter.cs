using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace System.Windows.Converters
{
    /// <summary>
    ///     Convert the value to a ratio based on the parameter.
    /// </summary>
    /// <example>
    /// <![CDATA[ MaxWidth="{Binding Source={x:Static SystemParameters.PrimaryScreenWidth}, Converter={RatioConverter}, ConverterParameter='0.9'}" ]]>
    /// </example>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    [ValueConversion(typeof (string), typeof (string))]
    public class RatioConverter : MarkupExtension, IValueConverter
    {
        #region Fields

        private static RatioConverter _Instance;

        #endregion

        #region IValueConverter Members

        /// <summary>
        ///     Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = System.Convert.ToDouble(value)*System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            return size.ToString("G0", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     When implemented in a derived class, returns an object that is set as the value of the target property for this
        ///     markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        ///     The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _Instance ?? (_Instance = new RatioConverter());
        }

        #endregion
    }
}