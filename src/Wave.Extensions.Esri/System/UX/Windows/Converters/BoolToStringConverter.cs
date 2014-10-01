using System.Windows.Data;

namespace System.Windows.Converters
{
    /// <summary>
    ///     A converter that converts the boolean value to either <see cref="bool.TrueString" /> if the value is <c>true</c>;
    ///     otherwise <see cref="bool.FalseString" /> when the value is <c>false</c>.
    /// </summary>
    [ValueConversion(typeof (bool), typeof (string))]
    public class BoolToStringConverter : BoolToValueConverter<string>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoolToStringConverter" /> class.
        /// </summary>
        public BoolToStringConverter()
            : base(bool.TrueString, bool.FalseString)
        {
        }

        #endregion
    }
}