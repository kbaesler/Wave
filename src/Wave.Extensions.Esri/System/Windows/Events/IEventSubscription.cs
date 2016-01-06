using System.Diagnostics.CodeAnalysis;

namespace System.Windows
{
    /// <summary>
    ///     Defines a contract for an event subscription to be used by <see cref="EventBase" />.
    /// </summary>
    public interface IEventSubscription
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a <see cref="SubscriptionToken" /> that identifies this <see cref="IEventSubscription" />.
        /// </summary>
        /// <value>A token that identifies this <see cref="IEventSubscription" />.</value>
        SubscriptionToken SubscriptionToken { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the execution strategy to publish this event.
        /// </summary>
        /// <returns>
        ///     An <see cref="Action{T}" /> with the execution strategy, or <see langword="null" /> if the
        ///     <see cref="IEventSubscription" /> is no longer valid.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Action<object[]> GetExecutionStrategy();

        #endregion
    }
}