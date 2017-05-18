using System.Globalization;

namespace System.Windows.Controls
{
    /// <summary>
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <ListView MinHeight="20" ItemsSource="{Binding VmItems}">
    /// <local:RangeValidationRule.Min>
    ///     <Binding Path="Items.Count" RelativeSource="{RelativeSource Self}">
    ///         <Binding.ValidationRules>
    ///             <local:RangeValidationRule Error="Collection must have at least one item" Min="1" />
    ///         </Binding.ValidationRules>
    ///     </Binding>
    /// </local:RangeValidationRule.Min>
    /// </ListView>
    /// ]]>
    /// </example>
    /// <seealso cref="System.Windows.Controls.ValidationRule" />
    public class RangeValidationRule : ValidationRule
    {
        #region Fields

        /// <summary>
        ///     The minimum property
        /// </summary>
        public static readonly DependencyProperty MinProperty = DependencyProperty.RegisterAttached(
            "Min",
            typeof(int),
            typeof(RangeValidationRule),
            new PropertyMetadata(default(int)));

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RangeValidationRule" /> class.
        /// </summary>
        public RangeValidationRule()
            : base(ValidationStep.ConvertedProposedValue, true)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the error.
        /// </summary>
        /// <value>
        ///     The error.
        /// </value>
        public string Error { get; set; }


        /// <summary>
        ///     Gets or sets the minimum.
        /// </summary>
        /// <value>
        ///     The minimum.
        /// </value>
        public int Min { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the minimum.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static int GetMin(DependencyObject element)
        {
            return (int)element.GetValue(MinProperty);
        }

        /// <summary>
        ///     Sets the minimum.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetMin(DependencyObject element, int value)
        {
            element.SetValue(MinProperty, value);
        }

        /// <summary>
        ///     When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>
        ///     A <see cref="T:System.Windows.Controls.ValidationResult" /> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            if ((int?)value < Min)
                result = new ValidationResult(false, Error);

            return result;
        }

        #endregion
    }
}