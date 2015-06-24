using System.Windows.Data;

namespace System.Windows.Converters
{
    /// <summary>
    ///     A value converter that converts the boolean value to it's corresponding negated value.
    /// </summary>
    [ValueConversion(typeof (bool), typeof (bool))]
    public class BoolToBoolConverter : BoolToValueConverter<bool>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoolToBoolConverter" /> class.
        /// </summary>
        public BoolToBoolConverter()
            : base(true, false)
        {
        }

        #endregion
    }
}