
namespace System.Windows
{
    /// <summary>
    ///     Provides access to the static event aggreator.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets an instance of an event type.
        /// </summary>
        /// <typeparam name="TEventType">The type of the event type.</typeparam>
        /// <returns>An instance of an event object of type TEventType.</returns>
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }
}
