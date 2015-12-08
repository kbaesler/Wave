using System;
using System.Collections.Generic;

using Miner.Interop;

namespace Miner.Geodatabase.Internal
{
    /// <summary>
    ///     Overrides the edit event used in the execute method with the specified edit event until the
    ///     <see cref="IDisposable" /> return value has been disposed of.
    /// </summary>
    internal class EventOverride : IDisposable
    {
        #region Fields

        private readonly Stack<bool> _Continues = new Stack<bool>();
        private readonly Stack<mmEditEvent> _Overrides = new Stack<mmEditEvent>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="EventOverride" /> should continue.
        /// </summary>
        /// <value>
        ///     <c>true</c> if continue; otherwise, <c>false</c>.
        /// </value>
        public bool Continue
        {
            get { return _Continues.Peek(); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is overridden.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is overridden; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverridden
        {
            get { return _Overrides.Count > 0; }
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public mmEditEvent Value
        {
            get { return _Overrides.Peek(); }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _Overrides.Pop();
            _Continues.Pop();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Caches the edit event and increments the busy counter for the monitor.
        /// </summary>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="flag">if set to <c>true</c> if the overriden event effects the termination process.</param>
        public void Set(mmEditEvent editEvent, bool flag = false)
        {
            _Overrides.Push(editEvent);
            _Continues.Push(flag);
        }

        #endregion
    }
}