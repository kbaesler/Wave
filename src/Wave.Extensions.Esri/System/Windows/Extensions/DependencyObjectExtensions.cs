using System.Linq;

namespace System.Windows.Controls
{
    /// <summary>
    ///     Provides extensions for the <see cref="DependencyObject" /> object.
    /// </summary>
    public static class DependencyObjectExtension
    {
        #region Public Methods

        /// <summary>
        ///     Returns true if there are not validation errors
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     <c>true</c> if the specified object is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(this DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
                   LogicalTreeHelper.GetChildren(obj)
                       .OfType<DependencyObject>()
                       .All(IsValid);
        }

        #endregion
    }
}