using System.Globalization;
using System.Reflection;
using System.Windows.Input;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Defines the command behavior binding
    /// </summary>
    public class CommandBehaviorBinding : IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     The command to execute when the specified event is raised
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command { get; set; }

        /// <summary>
        ///     Gets or sets a CommandParameter
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter { get; set; }

        /// <summary>
        ///     The event info of the event
        /// </summary>
        /// <value>The event.</value>
        public EventInfo Event { get; private set; }

        /// <summary>
        ///     Gets the EventHandler for the binding with the event
        /// </summary>
        /// <value>The event handler.</value>
        public Delegate EventHandler { get; private set; }

        /// <summary>
        ///     The event name to hook up to
        ///     This property can only be set from the BindEvent Method
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName { get; private set; }

        /// <summary>
        ///     Get the owner of the CommandBinding ex: a Button
        ///     This property can only be set from the BindEvent Method
        /// </summary>
        /// <value>The owner.</value>
        public DependencyObject Owner { get; private set; }

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

        #region Public Methods

        /// <summary>
        ///     Creates an EventHandler on runtime and registers that handler to the Event specified
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="eventName">Name of the event.</param>
        public void BindEvent(DependencyObject owner, string eventName)
        {
            this.EventName = eventName;
            this.Owner = owner;
            this.Event = this.Owner.GetType().GetEvent(this.EventName, BindingFlags.Public | BindingFlags.Instance);
            if (this.Event == null)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not resolve event name {0}", EventName));

            // Create an event handler for the event that will call the ExecuteCommand method
            this.EventHandler = BehaviorEventHandler.CreateDelegate(
                Event.EventHandlerType, typeof (CommandBehaviorBinding).GetMethod("ExecuteCommand", BindingFlags.Public | BindingFlags.Instance), this);

            // Register the handler to the Event
            this.Event.AddEventHandler(this.Owner, this.EventHandler);
        }

        /// <summary>
        ///     Executes the command that has been assigned to the <see cref="Command" /> property.
        /// </summary>
        public void ExecuteCommand()
        {
            if (this.Command == null)
                return;

            if (this.Command.CanExecute(this.CommandParameter))
                this.Command.Execute(this.CommandParameter);
        }

        #endregion

        #region Protected Methods

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
                if (this.Event != null)
                    this.Event.RemoveEventHandler(this.Owner, this.EventHandler);
            }
        }

        #endregion
    }
}