using System.Windows.Media.Animation;

namespace System.Windows.Controls
{

    #region Enumerations

    /// <summary>
    ///     An enumeration that handles the type of horizontal reveal.
    /// </summary>
    public enum HorizontalRevealMode
    {
        /// <summary>
        ///     No horizontal reveal animation.
        /// </summary>
        None,

        /// <summary>
        ///     Reveal from the left to the right.
        /// </summary>
        FromLeftToRight,

        /// <summary>
        ///     Reveal from the right to the left.
        /// </summary>
        FromRightToLeft,

        /// <summary>
        ///     Reveal from the center to the bounding edge.
        /// </summary>
        FromCenterToEdge,
    }

    /// <summary>
    ///     An enumeration that handles the type of vertical reveal.
    /// </summary>
    public enum VerticalRevealMode
    {
        /// <summary>
        ///     No vertical reveal animation.
        /// </summary>
        None,

        /// <summary>
        ///     Reveal from top to bottom.
        /// </summary>
        FromTopToBottom,

        /// <summary>
        ///     Reveal from bottom to top.
        /// </summary>
        FromBottomToTop,

        /// <summary>
        ///     Reveal from the center to the bounding edge.
        /// </summary>
        FromCenterToEdge,
    }

    #endregion

    /// <summary>
    ///     A custom decorator that provides animation for revealing it's contents either horizontally or vertically.
    /// </summary>
    public class Reveal : Decorator
    {
        #region Fields

        /// <summary>
        ///     Using a DependencyProperty as the backing store for AnimationProgress. This enables animation, styling, binding,
        ///     etc...
        /// </summary>
        public static readonly DependencyProperty AnimationProgressProperty =
            DependencyProperty.Register("AnimationProgress", typeof (double), typeof (Reveal), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, null, OnCoerceAnimationProgress));

        /// <summary>
        ///     Using a DependencyProperty as the backing store for Duration. This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof (double), typeof (Reveal), new UIPropertyMetadata(250.0));

        /// <summary>
        ///     Using a DependencyProperty as the backing store for HorizontalReveal. This enables animation, styling, binding,
        ///     etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalRevealProperty =
            DependencyProperty.Register("HorizontalReveal", typeof (HorizontalRevealMode), typeof (Reveal), new UIPropertyMetadata(HorizontalRevealMode.None));

        /// <summary>
        ///     Using a DependencyProperty as the backing store for IsExpanded. This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof (bool), typeof (Reveal), new UIPropertyMetadata(false, OnIsExpandedChanged));

        /// <summary>
        ///     Using a DependencyProperty as the backing store for VerticalReveal. This enables animation, styling, binding,
        ///     etc...
        /// </summary>
        public static readonly DependencyProperty VerticalRevealProperty =
            DependencyProperty.Register("VerticalReveal", typeof (VerticalRevealMode), typeof (Reveal), new UIPropertyMetadata(VerticalRevealMode.FromTopToBottom));

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="Reveal" /> class.
        /// </summary>
        static Reveal()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof (Reveal), new FrameworkPropertyMetadata(true));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Value between 0 and 1 (inclusive) to move the reveal along.
        ///     This is not meant to be used with IsExpanded.
        /// </summary>
        public double AnimationProgress
        {
            get { return (double) GetValue(AnimationProgressProperty); }
            set { SetValue(AnimationProgressProperty, value); }
        }

        /// <summary>
        ///     The duration in milliseconds of the reveal animation.
        ///     Will apply to the next animation that occurs (not to currently running animations).
        /// </summary>
        public double Duration
        {
            get { return (double) GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the horizontal reveal.
        /// </summary>
        /// <value>
        ///     The horizontal reveal.
        /// </value>
        public HorizontalRevealMode HorizontalReveal
        {
            get { return (HorizontalRevealMode) GetValue(HorizontalRevealProperty); }
            set { SetValue(HorizontalRevealProperty, value); }
        }

        /// <summary>
        ///     Whether the child is expanded or not.
        ///     Note that an animation may be in progress when the value changes.
        ///     This is not meant to be used with AnimationProgress and can overwrite any
        ///     animation or values in that property.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool) GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the vertical reveal.
        /// </summary>
        /// <value>
        ///     The vertical reveal.
        /// </value>
        public VerticalRevealMode VerticalReveal
        {
            get { return (VerticalRevealMode) GetValue(VerticalRevealProperty); }
            set { SetValue(VerticalRevealProperty, value); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Arranges the content of a <see cref="T:System.Windows.Controls.Decorator" /> element.
        /// </summary>
        /// <param name="arrangeSize">The <see cref="T:System.Windows.Size" /> this element uses to arrange its child content.</param>
        /// <returns>
        ///     The <see cref="T:System.Windows.Size" /> that represents the arranged size of this
        ///     <see cref="T:System.Windows.Controls.Decorator" /> element and its child.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElement child = Child;
            if (child != null)
            {
                double percent = AnimationProgress;
                HorizontalRevealMode horizontalReveal = HorizontalReveal;
                VerticalRevealMode verticalReveal = VerticalReveal;

                double childWidth = child.DesiredSize.Width;
                double childHeight = child.DesiredSize.Height;

                if (this.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    childWidth = arrangeSize.Width;
                    childHeight = arrangeSize.Height;
                }

                double x = CalculateLeft(childWidth, percent, horizontalReveal);
                double y = CalculateTop(childHeight, percent, verticalReveal);

                child.Arrange(new Rect(x, y, childWidth, childHeight));

                childWidth = child.RenderSize.Width;
                childHeight = child.RenderSize.Height;
                double width = CalculateWidth(childWidth, percent, horizontalReveal);
                double height = CalculateHeight(childHeight, percent, verticalReveal);
                return new Size(width, height);
            }

            return new Size();
        }

        /// <summary>
        ///     Measures the child element of a <see cref="T:System.Windows.Controls.Decorator" /> to prepare for arranging it
        ///     during the <see cref="M:System.Windows.Controls.Decorator.ArrangeOverride(System.Windows.Size)" /> pass.
        /// </summary>
        /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size" /> that should not be exceeded.</param>
        /// <returns>
        ///     The target <see cref="T:System.Windows.Size" /> of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            UIElement child = Child;
            if (child != null)
            {
                child.Measure(constraint);

                double percent = AnimationProgress;
                double width = CalculateWidth(child.DesiredSize.Width, percent, HorizontalReveal);
                double height = CalculateHeight(child.DesiredSize.Height, percent, VerticalReveal);
                return new Size(width, height);
            }

            return new Size();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Calculates the height.
        /// </summary>
        /// <param name="originalHeight">Height of the original.</param>
        /// <param name="percent">The percent.</param>
        /// <param name="reveal">The reveal.</param>
        /// <returns></returns>
        private static double CalculateHeight(double originalHeight, double percent, VerticalRevealMode reveal)
        {
            if (reveal == VerticalRevealMode.None)
            {
                return originalHeight;
            }

            return originalHeight*percent;
        }

        /// <summary>
        ///     Calculates the left.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="percent">The percent.</param>
        /// <param name="reveal">The reveal.</param>
        /// <returns></returns>
        private static double CalculateLeft(double width, double percent, HorizontalRevealMode reveal)
        {
            if (reveal == HorizontalRevealMode.FromRightToLeft)
            {
                return (percent - 1.0)*width;
            }

            if (reveal == HorizontalRevealMode.FromCenterToEdge)
            {
                return (percent - 1.0)*width*0.5;
            }

            return 0.0;
        }

        /// <summary>
        ///     Calculates the top.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="percent">The percent.</param>
        /// <param name="reveal">The reveal.</param>
        /// <returns></returns>
        private static double CalculateTop(double height, double percent, VerticalRevealMode reveal)
        {
            if (reveal == VerticalRevealMode.FromBottomToTop)
            {
                return (percent - 1.0)*height;
            }
            if (reveal == VerticalRevealMode.FromCenterToEdge)
            {
                return (percent - 1.0)*height*0.5;
            }
            return 0.0;
        }

        /// <summary>
        ///     Calculates the width.
        /// </summary>
        /// <param name="originalWidth">Width of the original.</param>
        /// <param name="percent">The percent.</param>
        /// <param name="reveal">The reveal.</param>
        /// <returns></returns>
        private static double CalculateWidth(double originalWidth, double percent, HorizontalRevealMode reveal)
        {
            if (reveal == HorizontalRevealMode.None)
            {
                return originalWidth;
            }

            return originalWidth*percent;
        }

        private static object OnCoerceAnimationProgress(DependencyObject d, object baseValue)
        {
            double num = (double) baseValue;
            if (num < 0.0)
            {
                return 0.0;
            }

            if (num > 1.0)
            {
                return 1.0;
            }

            return baseValue;
        }

        private static void OnIsExpandedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((Reveal) sender).SetupAnimation((bool) e.NewValue);
        }

        /// <summary>
        ///     Setups the animation.
        /// </summary>
        /// <param name="isExpanded">if set to <c>true</c> [is expanded].</param>
        private void SetupAnimation(bool isExpanded)
        {
            // Adjust the time if the animation is already in progress
            double currentProgress = AnimationProgress;
            if (isExpanded)
            {
                currentProgress = 1.0 - currentProgress;
            }

            DoubleAnimation animation = new DoubleAnimation();
            animation.To = isExpanded ? 1.0 : 0.0;
            animation.Duration = TimeSpan.FromMilliseconds(Duration*currentProgress);
            animation.FillBehavior = FillBehavior.HoldEnd;

            this.BeginAnimation(AnimationProgressProperty, animation);
        }

        #endregion
    }
}