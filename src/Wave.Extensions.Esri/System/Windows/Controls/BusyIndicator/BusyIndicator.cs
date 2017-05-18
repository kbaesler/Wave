using System.Collections;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    /// <summary>
    ///     The rotation direction.
    /// </summary>
    public enum RotateDirection
    {
        /// <summary>
        ///     The clockwise
        /// </summary>
        Clockwise = 0,

        /// <summary>
        ///     The counter clockwise
        /// </summary>
        CounterClockwise = 1
    }


    /// <summary>
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Control" />
    [TemplatePart(Name = "PART_Container", Type = typeof (Canvas))]
    public class BusyIndicator : Control
    {
        #region Fields

        /// <summary>
        ///     The circle color property
        /// </summary>
        public static readonly DependencyProperty CircleColorProperty =
            DependencyProperty.Register("CircleColor", typeof (Brush), typeof (BusyIndicator),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(27, 161, 226)),
                    FrameworkPropertyMetadataOptions.AffectsRender)); // windows 7 blue

        /// <summary>
        ///     The circle count property
        /// </summary>
        public static readonly DependencyProperty CircleCountProperty =
            DependencyProperty.Register("CircleCount", typeof (int), typeof (BusyIndicator),
                new FrameworkPropertyMetadata(12, FrameworkPropertyMetadataOptions.AffectsMeasure, OnCircleCountChanged));

        /// <summary>
        ///     The circle radius property
        /// </summary>
        public static readonly DependencyProperty CircleRadiusProperty =
            DependencyProperty.Register("CircleRadius", typeof (double), typeof (BusyIndicator),
                new FrameworkPropertyMetadata(3.0, FrameworkPropertyMetadataOptions.AffectsArrange,
                    OnCircleRadiusChanged));

        /// <summary>
        ///     The direction property
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof (RotateDirection), typeof (BusyIndicator),
                new UIPropertyMetadata(RotateDirection.Clockwise, OnDirectionChanged));

        /// <summary>
        ///     The is spinning property
        /// </summary>
        public static readonly DependencyProperty IsSpinningProperty =
            DependencyProperty.Register("IsSpinning", typeof (bool), typeof (BusyIndicator),
                new UIPropertyMetadata(true, OnIsSpinningChanged));

        /// <summary>
        ///     The radius property
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof (double), typeof (BusyIndicator),
                new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.AffectsMeasure, OnRadiusChanged));

        /// <summary>
        ///     The speed property
        /// </summary>
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof (double), typeof (BusyIndicator),
                new UIPropertyMetadata(1.0, OnSpeedChanged));

        /// <summary>
        ///     The symmetrical arrange property
        /// </summary>
        public static readonly DependencyProperty SymmetricalArrangeProperty =
            DependencyProperty.Register("SymmetricalArrange", typeof (bool), typeof (BusyIndicator),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange, OnRadiusChanged));

        private readonly DoubleAnimation _RotateAnimation = new DoubleAnimation(0, 360,
            new Duration(TimeSpan.FromSeconds(1)));

        private readonly Storyboard _StoryBoard = new Storyboard();
        private Canvas _Canvas;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="BusyIndicator" /> class.
        /// </summary>
        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BusyIndicator),
                new FrameworkPropertyMetadata(typeof (BusyIndicator)));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the color of the circle.
        /// </summary>
        /// <value>
        ///     The color of the circle.
        /// </value>
        public Brush CircleColor
        {
            get { return (Brush) GetValue(CircleColorProperty); }
            set { SetValue(CircleColorProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the circle count.
        /// </summary>
        /// <value>
        ///     The circle count.
        /// </value>
        public int CircleCount
        {
            get { return (int) GetValue(CircleCountProperty); }
            set { SetValue(CircleCountProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the circle radius.
        /// </summary>
        /// <value>
        ///     The circle radius.
        /// </value>
        public double CircleRadius
        {
            get { return (double) GetValue(CircleRadiusProperty); }
            set { SetValue(CircleRadiusProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the direction.
        /// </summary>
        /// <value>
        ///     The direction.
        /// </value>
        public RotateDirection Direction
        {
            get { return (RotateDirection) GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is spinning.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is spinning; otherwise, <c>false</c>.
        /// </value>
        public bool IsSpinning
        {
            get { return (bool) GetValue(IsSpinningProperty); }
            set { SetValue(IsSpinningProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the radius.
        /// </summary>
        /// <value>
        ///     The radius.
        /// </value>
        public double Radius
        {
            get { return (double) GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the speed.
        /// </summary>
        /// <value>
        ///     The speed.
        /// </value>
        public double Speed
        {
            get { return (double) GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether [symmetrical arrange].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [symmetrical arrange]; otherwise, <c>false</c>.
        /// </value>
        public bool SymmetricalArrange
        {
            get { return (bool) GetValue(SymmetricalArrangeProperty); }
            set { SetValue(SymmetricalArrangeProperty, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Canvas = GetTemplateChild("PART_Container") as Canvas;

            InitializeControl();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Calculates the position.
        /// </summary>
        /// <param name="radian">The radian.</param>
        /// <returns></returns>
        private Point CalculatePosition(double radian)
        {
            double x = 0 + Radius*Math.Cos(radian);
            double y = 0 + Radius*Math.Sin(radian);

            return new Point(x - CircleRadius, y - CircleRadius);
        }

        /// <summary>
        ///     Creates the animation.
        /// </summary>
        private void CreateAnimation()
        {
            _RotateAnimation.RepeatBehavior = RepeatBehavior.Forever;
            _RotateAnimation.SpeedRatio = Speed;

            if (Direction == RotateDirection.CounterClockwise)
            {
                _RotateAnimation.To *= -1;
            }

            Storyboard.SetTarget(_RotateAnimation, _Canvas);
            Storyboard.SetTargetProperty(_RotateAnimation,
                new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

            _StoryBoard.Children.Add(_RotateAnimation);
        }

        /// <summary>
        ///     Creates the ellipse.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns></returns>
        private Ellipse CreateEllipse(int counter)
        {
            var ellipse = new Ellipse();
            ellipse.Fill = CircleColor;
            ellipse.Width = CircleRadius*2;
            ellipse.Height = CircleRadius*2;
            ellipse.Opacity = counter/(double) CircleCount;

            SetEllipsePosition(ellipse, counter);

            return ellipse;
        }

        /// <summary>
        ///     Generates the circles.
        /// </summary>
        private void GenerateCircles()
        {
            for (int i = 0; i < CircleCount; i++)
            {
                Ellipse ellipse = CreateEllipse(i);

                _Canvas.Children.Add(ellipse);
            }
        }

        /// <summary>
        ///     Initializes the control.
        /// </summary>
        private void InitializeControl()
        {
            GenerateCircles();
            CreateAnimation();

            ToggleSpinning(IsSpinning);
        }

        private static void OnCircleCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wheel = obj as BusyIndicator;
            if (wheel != null && wheel._Canvas != null && e.NewValue != null)
            {
                wheel._Canvas.Children.RemoveRange(0, (int) e.OldValue);

                wheel.GenerateCircles();
            }
        }

        private static void OnCircleRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wheel = obj as BusyIndicator;
            if (wheel != null && wheel._Canvas != null && e.NewValue != null)
            {
                var newRadius = (double) e.NewValue;
                UpdateEllipses(wheel._Canvas.Children, (c, ellipse) =>
                {
                    ellipse.Width = newRadius*2;
                    ellipse.Height = newRadius*2;

                    wheel.SetEllipsePosition(ellipse, c);
                });
            }
        }

        private static void OnDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wheel = obj as BusyIndicator;
            if (wheel != null && e.NewValue != null && wheel._StoryBoard != null)
            {
                bool prevState = wheel.IsSpinning;

                wheel.ToggleSpinning(false);
                wheel._RotateAnimation.To *= -1;
                wheel.ToggleSpinning(prevState);
            }
        }

        private static void OnIsSpinningChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wheel = obj as BusyIndicator;
            if (wheel != null && e.NewValue != null && wheel._StoryBoard != null)
            {
                wheel.ToggleSpinning((bool) e.NewValue);
            }
        }

        private static void OnRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wheel = obj as BusyIndicator;
            if (wheel != null && wheel._Canvas != null && e.NewValue != null)
            {
                UpdateEllipses(wheel._Canvas.Children, (c, ellipse) => wheel.SetEllipsePosition(ellipse, c));
            }
        }

        private static void OnSpeedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wheel = obj as BusyIndicator;
            if (wheel != null && wheel._StoryBoard != null)
            {
                // don't ask
                wheel._StoryBoard.SetSpeedRatio((double) e.NewValue);
                wheel._RotateAnimation.SpeedRatio = (double) e.NewValue;
            }
        }

        /// <summary>
        ///     Sets the ellipse position.
        /// </summary>
        /// <param name="ellipse">The ellipse.</param>
        /// <param name="ellipseCounter">The ellipse counter.</param>
        private void SetEllipsePosition(Ellipse ellipse, int ellipseCounter)
        {
            double maxCount = SymmetricalArrange ? CircleCount : (2*Radius*Math.PI)/(2*CircleRadius + 2);

            Point position = CalculatePosition(ellipseCounter*2*Math.PI/maxCount);
            Canvas.SetLeft(ellipse, position.X);
            Canvas.SetTop(ellipse, position.Y);
        }

        /// <summary>
        ///     Toggles the spinning.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void ToggleSpinning(bool value)
        {
            if (value)
            {
                _StoryBoard.Begin();
            }
            else
            {
                _StoryBoard.Stop();
            }
        }

        /// <summary>
        ///     Updates the ellipses.
        /// </summary>
        /// <param name="ellipses">The ellipses.</param>
        /// <param name="updateMethod">The update method.</param>
        private static void UpdateEllipses(IEnumerable ellipses, Action<int, Ellipse> updateMethod)
        {
            if (updateMethod != null && ellipses != null)
            {
                int i = 1;
                foreach (object child in ellipses)
                {
                    var ellipse = (child as Ellipse);
                    if (ellipse != null)
                    {
                        updateMethod(i++, ellipse);
                    }
                }
            }
        }

        #endregion
    }
}