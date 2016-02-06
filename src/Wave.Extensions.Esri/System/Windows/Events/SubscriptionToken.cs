using System.Diagnostics.CodeAnalysis;

namespace System.Windows
{
    /// <summary>
    ///     Subscription token returned from <see cref="EventBase" /> on subscribe.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Should never have a need for a finalizer, hence no need for Dispole(bool)")]
    public class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
    {
        #region Fields

        private readonly Guid _Token;
        private Action<SubscriptionToken> _UnsubscribeAction;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of <see cref="SubscriptionToken" />.
        /// </summary>
        public SubscriptionToken(Action<SubscriptionToken> unsubscribeAction)
        {
            _UnsubscribeAction = unsubscribeAction;
            _Token = Guid.NewGuid();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Disposes the SubscriptionToken, removing the subscription from the corresponding <see cref="EventBase" />.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Should never have need for a finalizer, hence no need for Dispose(bool).")]
        public virtual void Dispose()
        {
            // While the SubsctiptionToken class implements IDisposable, in the case of weak subscriptions 
            // (i.e. keepSubscriberReferenceAlive set to false in the Subscribe method) it's not necessary to unsubscribe,
            // as no resources should be kept alive by the event subscription. 
            // In such cases, if a warning is issued, it could be suppressed.

            if (_UnsubscribeAction != null)
            {
                _UnsubscribeAction(this);
                _UnsubscribeAction = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region IEquatable<SubscriptionToken> Members

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(SubscriptionToken other)
        {
            if (other == null) return false;
            return Equals(_Token, other._Token);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />;
        ///     otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />. </param>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj" /> parameter is null.</exception>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as SubscriptionToken);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return _Token.GetHashCode();
        }

        #endregion
    }
}