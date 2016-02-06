using System;
using System.Collections.Generic;
using System.Linq;

using Miner.Geodatabase.Internal;
using Miner.Interop;

namespace Miner.Geodatabase
{
    /// <summary>
    /// Provides the ability to override the edit event enumeration.
    /// </summary>
    public class EventOverrides : IDisposable
    {
        #region Fields

        private readonly Dictionary<string, EventOverride> _EventOverrides = new Dictionary<string, EventOverride>();

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified key is overridden.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns <see cref="bool" /> </returns>
        public bool IsOverridden(string key)
        {
            if (_EventOverrides.ContainsKey(key))
                return _EventOverrides[key].IsOverridden;

            return false;
        }

        /// <summary>
        /// Sets the override for the specified edit event for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="editEvents">The edit events.</param>
        public void Set(string key, params mmEditEvent[] editEvents)
        {
            var eventOverride = new EventOverride();

            if (_EventOverrides.ContainsKey(key))
            {
                eventOverride = _EventOverrides[key];
            }
            else
            {
                _EventOverrides.Add(key, eventOverride);
            }

            foreach (var editEvent in editEvents)
            {
                eventOverride.Set(editEvent);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_EventOverrides.Any())
                {
                    var eventOverrides = _EventOverrides.Last();

                    var eventOverride = eventOverrides.Value;
                    eventOverride.Dispose();

                    _EventOverrides.Remove(eventOverrides.Key);
                }
            }
        }

        #endregion
    }
}