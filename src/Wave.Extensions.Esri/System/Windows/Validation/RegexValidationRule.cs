using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace System.Windows.Controls
{
    /// <summary>
    ///     A <see cref="System.Windows.Controls.ValidationRule" />-derived class which
    ///     supports the use of regular expressions for validation.
    /// </summary>
    public class RegexValidationRule : ValidationRule
    {
        #region Constructors

        /// <summary>
        ///     Parameterless constructor.
        /// </summary>
        public RegexValidationRule()
        {
        }

        /// <summary>
        ///     Creates a RegexValidationRule with the specified regular expression.
        /// </summary>
        /// <param name="regexText">The regular expression used by the new instance.</param>
        public RegexValidationRule(string regexText)
        {
            this.Pattern = regexText;
        }

        /// <summary>
        ///     Creates a RegexValidationRule with the specified regular expression
        ///     and error message.
        /// </summary>
        /// <param name="regexText">The regular expression used by the new instance.</param>
        /// <param name="errorMessage">The error message used when validation fails.</param>
        public RegexValidationRule(string regexText, string errorMessage)
            : this(regexText, errorMessage, RegexOptions.None)
        {
        }

        /// <summary>
        ///     Creates a RegexValidationRule with the specified regular expression,
        ///     error message, and RegexOptions.
        /// </summary>
        /// <param name="regexText">The regular expression used by the new instance.</param>
        /// <param name="errorMessage">The error message used when validation fails.</param>
        /// <param name="regexOptions">The RegexOptions used by the new instance.</param>
        public RegexValidationRule(string regexText, string errorMessage, RegexOptions regexOptions)
            : this(regexText)
        {
            this.Options = regexOptions;
            this.Error = errorMessage;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets/sets the error message to be used when validation fails.
        /// </summary>
        /// <value>
        ///     The error message.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        ///     Gets/sets the RegexOptions to be used during validation.
        ///     This property's default value is 'None'.
        /// </summary>
        /// <value>
        ///     The regex options.
        /// </value>
        public RegexOptions Options { get; set; }

        /// <summary>
        ///     Gets/sets the regular expression used during validation.
        /// </summary>
        /// <value>
        ///     The regex text.
        /// </value>
        public string Pattern { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Validates the 'value' argument using the regular
        ///     expression and RegexOptions associated with this object.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>
        ///     A <see cref="T:System.Windows.Controls.ValidationResult" /> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            // If there is no regular expression to evaluate,
            // then the data is considered to be valid.
            if (!string.IsNullOrEmpty(this.Pattern))
            {
                string text = this.GetBoundValue(value) as string ?? string.Empty;

                // If the string does not match the regex, return a value
                // which indicates failure and provide an error mesasge.
                if (!Regex.IsMatch(text, this.Pattern, this.Options))
                    result = new ValidationResult(false, this.Error);
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the bound value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private object GetBoundValue(object value)
        {
            var binding = value as BindingExpression;
            if (binding != null)
            {
                // ValidationStep was UpdatedValue or CommittedValue (Validate after setting)
                // Get the bound object and name of the property
                object dataItem = binding.DataItem;
                string propertyName = binding.ParentBinding.Path.Path;

                // Extract the value of the property.
                object propertyValue = dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);
                return propertyValue;
            }

            // ValidationStep was RawProposedValue or ConvertedProposedValue
            return value;
        }

        #endregion
    }
}