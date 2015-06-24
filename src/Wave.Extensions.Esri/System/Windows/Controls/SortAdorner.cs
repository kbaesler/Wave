using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    ///     An Adorner class used for rendering an arrow on a list header to show the direction
    ///     that a list sorted by.
    /// </summary>
    public class SortAdorner : Adorner
    {
        #region Fields

        private static readonly Geometry Ascending =
            Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

        private static readonly Geometry Descending =
            Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SortAdorner" /> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="direction">The direction.</param>
        public SortAdorner(UIElement element, ListSortDirection direction)
            : base(element)
        {
            this.Direction = direction;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public ListSortDirection Direction { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Method to render the sort adorner
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            drawingContext.PushTransform(
                new TranslateTransform(
                    AdornedElement.RenderSize.Width - 15,
                    (AdornedElement.RenderSize.Height - 5)/2));

            drawingContext.DrawGeometry(Brushes.Black, null,
                Direction == ListSortDirection.Ascending ?
                    Ascending : Descending);

            drawingContext.Pop();
        }

        #endregion
    }
}