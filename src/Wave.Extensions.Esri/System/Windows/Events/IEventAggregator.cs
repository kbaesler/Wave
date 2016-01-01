namespace System.Windows
{
    /// <summary>
    ///     A marker interface for classes that subscribe to messages.
    /// </summary>
    public interface IHandle
    {
    }

    /// <summary>
    ///     Denotes a class which can handle a particular type of message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to handle.</typeparam>
    public interface IHandle<TMessage> : IHandle
    {
        #region Public Methods

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Handle(TMessage message);

        #endregion
    }

    /// <summary>
    ///     Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public interface IEventAggregator
    {
        #region Public Methods

        /// <summary>
        ///     Searches the subscribed handlers to check if we have a handler for
        ///     the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        bool HandlerExistsFor(Type messageType);

        /// <summary>
        ///     Publishes a message.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        void Publish(object message, Action<Action> marshal);

        /// <summary>
        ///     Subscribes an instance to all events declared through implementations of <see cref="IHandle{T}" />
        /// </summary>
        /// <param name="subscriber">The instance to subscribe for event publication.</param>
        void Subscribe(object subscriber);

        /// <summary>
        ///     Unsubscribes the instance from all events.
        /// </summary>
        /// <param name="subscriber">The instance to unsubscribe.</param>
        void Unsubscribe(object subscriber);

        #endregion
    }
}