using System.Windows.Input;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Defines a Command Binding
    ///     This inherits from freezable so that it gets inheritance context for DataBinding to work
    /// </summary>
    public class BehaviorBinding : Freezable, IDisposable
    {
        #region Fields

        /// <summary>
        ///     CommandParameter Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof (object), typeof (BehaviorBinding),
                new FrameworkPropertyMetadata(null,
                    OnCommandParameterChanged));

        /// <summary>
        ///     Command Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof (ICommand), typeof (BehaviorBinding),
                new FrameworkPropertyMetadata(null,
                    OnCommandChanged));

        /// <summary>
        ///     Event Dependency Property
        /// </summary>
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof (string), typeof (BehaviorBinding),
                new FrameworkPropertyMetadata(null,
                    OnEventChanged));

        private CommandBehaviorBinding _Behavior;
        private DependencyObject _Owner;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Command property.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the CommandParameter property.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the Event property.
        /// </summary>
        public string Event
        {
            get { return (string) GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the Owner of the binding
        /// </summary>
        public DependencyObject Owner
        {
            get { return _Owner; }
            set
            {
                _Owner = value;
                ResetEventBinding();
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        ///     Stores the Command Behavior Binding
        /// </summary>
        internal CommandBehaviorBinding Behavior
        {
            get
            {
                if (_Behavior == null)
                    _Behavior = new CommandBehaviorBinding();
                return _Behavior;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     This is not actually used. This is just a trick so that this object gets WPF Inheritance Context
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Behavior != null)
                {
                    _Behavior.Dispose();
                    _Behavior = null;
                }
            }
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the Command property.
        /// </summary>
        protected virtual void OnCommandChanged(DependencyPropertyChangedEventArgs e)
        {
            Behavior.Command = Command;
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the CommandParameter property.
        /// </summary>
        protected virtual void OnCommandParameterChanged(DependencyPropertyChangedEventArgs e)
        {
            Behavior.CommandParameter = CommandParameter;
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the Event property.
        /// </summary>
        protected virtual void OnEventChanged(DependencyPropertyChangedEventArgs e)
        {
            ResetEventBinding();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles changes to the Command property.
        /// </summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BehaviorBinding) d).OnCommandChanged(e);
        }

        /// <summary>
        ///     Handles changes to the CommandParameter property.
        /// </summary>
        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BehaviorBinding) d).OnCommandParameterChanged(e);
        }

        /// <summary>
        ///     Handles changes to the Event property.
        /// </summary>
        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BehaviorBinding) d).OnEventChanged(e);
        }

        /// <summary>
        ///     Resets the event binding.
        /// </summary>
        private void ResetEventBinding()
        {
            if (Owner != null) //only do this when the Owner is set
            {
                //check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
                if (Behavior.Event != null && Behavior.Owner != null)
                    Behavior.Dispose();

                //bind the new event to the command
                Behavior.BindEvent(Owner, Event);
            }
        }

        #endregion
    }
}