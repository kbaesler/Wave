using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Windows
{
    /// <summary>
    ///     Provides access to the static event aggreator.
    /// </summary>
    public static class EventAggregator
    {
        #region Fields

        private static readonly SynchronizationContext Context = SynchronizationContext.Current;
        private static readonly Dictionary<Type, EventBase> Events = new Dictionary<Type, EventBase>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same
        ///     <typeparamref name="TEventType" /> returns the same event instance.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get. This must inherit from <see cref="EventBase" />.</typeparam>
        /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType" />.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            lock (Events)
            {
                EventBase existingEvent = null;

                if (!Events.TryGetValue(typeof (TEventType), out existingEvent))
                {
                    var newEvent = new TEventType();
                    newEvent.SynchronizationContext = Context;
                    Events[typeof (TEventType)] = newEvent;

                    return newEvent;
                }
                return (TEventType) existingEvent;
            }
        }

        #endregion
    }
}