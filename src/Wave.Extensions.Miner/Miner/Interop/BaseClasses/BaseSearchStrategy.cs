using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Miner.Interop
{
    /// <summary>
    ///     An abstract class used to implement custom <see cref="IMMSearchStrategy" /> search strategies.
    /// </summary>
    /// <typeparam name="T">The type of search parameters.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseSearchStrategy<T> : IMMSearchStrategy
    {
        #region Protected Properties

        /// <summary>
        ///     Gets a value indicating whether a stop has been requested or the threshold has been reached.
        /// </summary>
        /// <value><c>true</c> if a stop has been requested or the threshold has been reached; otherwise, <c>false</c>.</value>
        protected virtual bool CancellationPending
        {
            get
            {
                // A stop has been requested or the threshold has been reached.
                return (this.Stopped || this.ThresholdReached);
            }
        }

        /// <summary>
        ///     Gets or sets the search control.
        /// </summary>
        /// <value>The search control.</value>
        protected virtual IMMSearchControl SearchControl { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BaseSearchStrategy&lt;T&gt;" /> is stopped.
        /// </summary>
        /// <value>
        ///     <c>true</c> if stopped; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool Stopped
        {
            get
            {
                if (this.SearchControl == null)
                    return false;

                return this.SearchControl.Stopped;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has reached it's threshold.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has reached it's threshold; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool ThresholdReached { get; set; }

        #endregion

        #region IMMSearchStrategy Members

        /// <summary>
        ///     Finds the results using the specified <paramref name="pSearchConfig" /> parameters and
        ///     allows halting the searching using the <paramref name="pSearchControl" />.
        /// </summary>
        /// <param name="pSearchConfig">The search config.</param>
        /// <param name="pSearchControl">The search control.</param>
        /// <returns>
        ///     Returns the results from the search.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pSearchConfig</exception>
        public virtual IMMSearchResults Find(IMMSearchConfiguration pSearchConfig, IMMSearchControl pSearchControl)
        {
            if (pSearchConfig == null) throw new ArgumentNullException("pSearchConfig");

            this.VerifyType(pSearchConfig.SearchParameters);

            T parameters = (T) pSearchConfig.SearchParameters;
            if (this.ValidateParameters(parameters))
            {
                this.ThresholdReached = false;
                this.SearchControl = pSearchControl;

                return this.Find(parameters);
            }

            return null;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Performs the search using the specified <paramref name="parameters" />.
        /// </summary>
        /// <param name="parameters">The search parameters.</param>
        /// <returns>Returns the <see cref="IMMSearchResults" /> from the search.</returns>
        protected abstract IMMSearchResults Find(T parameters);

        /// <summary>
        ///     Validates the parameters are correct for the given search strategy.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///     <c>true</c> if the parameters are valid; otherwise <c>false</c>.
        /// </returns>
        protected abstract bool ValidateParameters(T parameters);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Warns the developer if <paramref name="parameters" /> is not of the type specified in the declaration of the
        ///     object.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentNullException">parameters</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyType(object parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            if (!(parameters is T))
            {
                Debug.Fail(string.Format(CultureInfo.CurrentCulture, "The search parameters must be of type {0}.", typeof (T).Name));
            }
        }

        #endregion
    }
}