using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    ///     Adorner for the watermark
    /// </summary>
    internal class WatermarkAdorner : Adorner
    {
        #region Fields

        /// <summary>
        ///     <see cref="FrameworkElement" /> that holds the watermark
        /// </summary>
        private readonly FrameworkElement _ContentPresenter;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WatermarkAdorner" /> class
        /// </summary>
        /// <param name="adornedElement">The <see cref="UIElement" /> to be adorned</param>
        /// <param name="watermarkElement">The watermark element.</param>
        public WatermarkAdorner(UIElement adornedElement, FrameworkElement watermarkElement)
            : base(adornedElement)
        {
            this.IsHitTestVisible = false;

            _ContentPresenter = watermarkElement;

            if (this.Control is ItemsControl && !(this.Control is ComboBox))
            {
                _ContentPresenter.VerticalAlignment = VerticalAlignment.Center;
                _ContentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
            }

            // Hide the control adorner when the adorned element is hidden
            Binding binding = new Binding("IsVisible");
            binding.Source = adornedElement;
            binding.Converter = new BooleanToVisibilityConverter();
            this.SetBinding(VisibilityProperty, binding);
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the number of children for the <see cref="ContainerVisual" />.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        #endregion

        #region Private Properties

        /// <summary>
        ///     Gets the control that is being adorned
        /// </summary>
        private Control Control
        {
            get { return (Control) this.AdornedElement; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     When overridden in a derived class, positions child elements and determines a size for a
        ///     <see cref="FrameworkElement" /> derived class.
        /// </summary>
        /// <param name="finalSize">
        ///     The final area within the parent that this element should use to arrange itself and its
        ///     children.
        /// </param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _ContentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        ///     Returns a specified child <see cref="Visual" /> for the parent <see cref="ContainerVisual" />.
        /// </summary>
        /// <param name="index">
        ///     A 32-bit signed integer that represents the index value of the child <see cref="Visual" />. The
        ///     value of index must be between 0 and <see cref="VisualChildrenCount" /> - 1.
        /// </param>
        /// <returns>The child <see cref="Visual" />.</returns>
        protected override Visual GetVisualChild(int index)
        {
            return _ContentPresenter;
        }

        /// <summary>
        ///     Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>A <see cref="Size" /> object representing the amount of layout space needed by the adorner.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            // Here's the secret to getting the adorner to cover the whole control
            _ContentPresenter.Measure(this.Control.RenderSize);
            return this.Control.RenderSize;
        }

        #endregion
    }
}