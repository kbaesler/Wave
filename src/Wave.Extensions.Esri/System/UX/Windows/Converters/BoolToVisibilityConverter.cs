using System.Windows.Data;

namespace System.Windows.Converters
{
    /// <summary>
    ///     A converter that converts the boolean value to either <see cref="Visibility.Collapsed" /> if the value is
    ///     <c>false</c>; otherwise <see cref="Visibility.Visible" /> when the value is <c>true</c>.
    /// </summary>
    [ValueConversion(typeof (bool), typeof (Visibility))]
    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoolToVisibilityConverter" /> class.
        /// </summary>
        public BoolToVisibilityConverter()
            : base(Visibility.Visible, Visibility.Collapsed)
        {
        }

        #endregion
    }
}