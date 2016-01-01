using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Windows
{
    /// <summary>
    ///     Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        #region Fields

        private readonly List<Handler> _Handlers = new List<Handler>();

        /// <summary>
        ///     Processing of handler results on publication thread.
        /// </summary>
        public static Action<object, object> PublicationHandler = (target, result) => { };

        #endregion

        #region IEventAggregator Members

        /// <summary>
        ///     Searches the subscribed handlers to check if we have a handler for
        ///     the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        public bool HandlerExistsFor(Type messageType)
        {
            return _Handlers.Any(handler => handler.Handles(messageType) & !handler.IsDead);
        }

        /// <summary>
        ///     Publishes a message.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        public virtual void Publish(object message, Action<Action> marshal)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (marshal == null)
            {
                throw new ArgumentNullException("marshal");
            }

            Handler[] toNotify;
            lock (_Handlers)
            {
                toNotify = _Handlers.ToArray();
            }

            marshal(() =>
            {
                var messageType = message.GetType();

                var dead = toNotify
                    .Where(handler => !handler.Handle(messageType, message))
                    .ToList();

                if (dead.Any())
                {
                    lock (_Handlers)
                    {
                        dead.ForEach(x => _Handlers.Remove(x));
                    }
                }
            });
        }

        /// <summary>
        ///     Subscribes an instance to all events declared through implementations of <see cref="IHandle{T}" />
        /// </summary>
        /// <param name="subscriber">The instance to subscribe for event publication.</param>
        public virtual void Subscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            lock (_Handlers)
            {
                if (_Handlers.Any(x => x.Matches(subscriber)))
                {
                    return;
                }

                _Handlers.Add(new Handler(subscriber));
            }
        }

        /// <summary>
        ///     Unsubscribes the instance from all events.
        /// </summary>
        /// <param name="subscriber">The instance to unsubscribe.</param>
        public virtual void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            lock (_Handlers)
            {
                var found = _Handlers.FirstOrDefault(x => x.Matches(subscriber));

                if (found != null)
                {
                    _Handlers.Remove(found);
                }
            }
        }

        #endregion

        #region Nested Type: Handler

        private class Handler
        {
            #region Fields

            private readonly WeakReference _Reference;
            private readonly Dictionary<Type, MethodInfo> _SupportedHandlers = new Dictionary<Type, MethodInfo>();

            #endregion

            #region Constructors

            public Handler(object handler)
            {
                _Reference = new WeakReference(handler);

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof (IHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach (var @interface in interfaces)
                {
                    var type = @interface.GetGenericArguments()[0];
                    var method = @interface.GetMethod("Handle");
                    _SupportedHandlers[type] = method;
                }
            }

            #endregion

            #region Public Properties

            public bool IsDead
            {
                get { return _Reference.Target == null; }
            }

            #endregion

            #region Public Methods

            public bool Handle(Type messageType, object message)
            {
                var target = _Reference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var pair in _SupportedHandlers)
                {
                    if (pair.Key.IsAssignableFrom(messageType))
                    {
                        var result = pair.Value.Invoke(target, new[] {message});
                        if (result != null)
                        {
                            PublicationHandler(target, result);
                        }
                    }
                }

                return true;
            }

            public bool Handles(Type messageType)
            {
                return _SupportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType));
            }

            public bool Matches(object instance)
            {
                return _Reference.Target == instance;
            }

            #endregion
        }

        #endregion
    }
}