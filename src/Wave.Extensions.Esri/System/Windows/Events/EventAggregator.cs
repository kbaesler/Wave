using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Windows
{
    /// <summary>
    ///     Implements <see cref="IEventAggregator" />.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        #region Fields

        private readonly Dictionary<Type, EventBase> _Events = new Dictionary<Type, EventBase>();
        private readonly SynchronizationContext _SyncContext = SynchronizationContext.Current;

        #endregion

        #region IEventAggregator Members

        /// <summary>
        ///     Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same
        ///     <typeparamref name="TEventType" /> returns the same event instance.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get. This must inherit from <see cref="EventBase" />.</typeparam>
        /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType" />.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            lock (_Events)
            {
                EventBase existingEvent = null;

                if (!_Events.TryGetValue(typeof (TEventType), out existingEvent))
                {
                    var newEvent = new TEventType();
                    newEvent.SynchronizationContext = _SyncContext;
                    _Events[typeof (TEventType)] = newEvent;

                    return newEvent;
                }
                return (TEventType) existingEvent;
            }
        }

        #endregion
    }
}