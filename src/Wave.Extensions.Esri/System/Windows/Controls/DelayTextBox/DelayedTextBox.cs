using System.Timers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace System.Windows.Controls
{
    /// <summary>
    ///     Represents a TextBox whose Text will get updated after a specified interval when the user stops entering text
    /// </summary>
    public class DelayedTextBox : TextBox
    {
        #region Fields

        /// <summary>
        ///     The delay time property
        /// </summary>
        public static readonly DependencyProperty DelayTimeProperty =
            DependencyProperty.Register("DelayTime", typeof (int), typeof (DelayedTextBox), new UIPropertyMetadata(667, OnDelayTimeChanged));

        private readonly Timer _KeypressTimer;

        private Action _KeypressAction;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelayedTextBox" /> class.
        /// </summary>
        public DelayedTextBox()
        {
            _KeypressTimer = new Timer();
            _KeypressTimer.Elapsed += OnTimeElapsed;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets and Sets the amount of time (in miliseconds) to wait after the text has changed before updating the binding.
        /// </summary>
        public int DelayTime
        {
            get { return (int) this.GetValue(DelayTimeProperty); }
            set { this.SetValue(DelayTimeProperty, value); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Invoked whenever an unhandled key attached routed event reaches an element derived from this class in its route.
        ///     Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // We dont update the source if we accept enter
            if (this.AcceptsReturn)
                return;

            // Update the binding if enter or return is pressed
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                // Get the binding
                BindingExpression bindingExpression = this.GetBindingExpression(TextProperty);

                // If the binding is valid update it
                if (this.CanUpdateSource(bindingExpression))
                {
                    // Update the source
                    bindingExpression.UpdateSource();
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        ///     Is called when content in this editing control changes.
        /// </summary>
        /// <param name="e">
        ///     The arguments that are associated with the
        ///     <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged" /> event.
        /// </param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            // When a binding is provided using the binding.
            BindingExpression bindingExpression = this.GetBindingExpression(TextProperty);
            if (this.CanUpdateSource(bindingExpression))
            {
                // Create a delegate method to do the binding update.
                _KeypressAction = bindingExpression.UpdateSource;
            }
            else
            {
                // Create a delegate method to just call the event.
                _KeypressAction = () => base.OnTextChanged(e);
            }

            if (this.DelayTime > 0)
            {
                _KeypressTimer.Interval = this.DelayTime;
                _KeypressTimer.Start();
            }
            else
            {
                this.OnTimeElapsed(this, null);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Detertmines if the bindings process can proceed.
        /// </summary>
        /// <param name="bindingExpression">The binding expression.</param>
        /// <returns>
        ///     <c>true</c> if the binding can be allowed, otherwise <c>false</c>
        /// </returns>
        private bool CanUpdateSource(BindingExpression bindingExpression)
        {
            // Cant update if there is no BindingExpression
            if (bindingExpression == null)
                return false;

            // Cant update if we have no data item
            if (bindingExpression.DataItem == null)
                return false;

            // Cant update if the binding is not active
            if (bindingExpression.Status != BindingStatus.Active)
                return false;

            // Cant update if the parent binding is null
            if (bindingExpression.ParentBinding == null)
                return false;

            // Dont need to update if the UpdateSourceTrigger is set to update every time the property changes
            if (bindingExpression.ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
                return false;

            return true;
        }

        /// <summary>
        ///     Called when <see cref="DelayedTextBox.DelayTime" /> property changes.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnDelayTimeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DelayedTextBox delayedTextBox = dependencyObject as DelayedTextBox;
            if (delayedTextBox != null)
                delayedTextBox.DelayTime = (int) e.NewValue;
        }

        /// <summary>
        ///     Called when the time has elapsed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        private void OnTimeElapsed(object source, ElapsedEventArgs e)
        {
            _KeypressTimer.Stop();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, _KeypressAction);
        }

        #endregion
    }
}