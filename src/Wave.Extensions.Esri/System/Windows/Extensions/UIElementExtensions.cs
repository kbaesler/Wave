using System.Windows.Interop;
using System.Windows.Media;

namespace System.Windows
{
    /// <summary>
    ///     Provides extension methods for the <see cref="UIElement" /> objects.
    /// </summary>
    public static class UIElementExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the size of the element in pixels.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///     Returns a <see cref="Size" /> representing the size of the element in pixels.
        /// </returns>
        public static Size GetPixelSize(this UIElement element)
        {
            Matrix transformToDevice = new Matrix();
            var source = PresentationSource.FromVisual(element);
            if (source != null)
            {
                if (source.CompositionTarget != null)
                    transformToDevice = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var hwnd = new HwndSource(new HwndSourceParameters()))
                    if (hwnd.CompositionTarget != null)
                        transformToDevice = hwnd.CompositionTarget.TransformToDevice;
            }

            if (element.DesiredSize == new Size())
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return (Size) transformToDevice.Transform((Vector) element.DesiredSize);
        }

        #endregion
    }
}