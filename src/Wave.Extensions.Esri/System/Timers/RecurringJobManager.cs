using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace System.Timers
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides access to a collection of <see cref="T:RecurringJob" /> objects.
    /// </summary>
    public sealed class RecurringJobManager : IDisposable
    {
        #region Fields

        private readonly ConcurrentDictionary<string, RecurringJob> _Jobs;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RecurringJobManager" /> class.
        /// </summary>
        public RecurringJobManager()
        {
            _Jobs = new ConcurrentDictionary<string, RecurringJob>();
        }

        #endregion

        #region IDisposable Members

        /// <inheritdoc />
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
        ///     Adds the <see cref="RecurringJob" /> with the specified identifier when it doesn't exist, otherwise the schedule
        ///     is updated for the existing job.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="task">The action that will be executed.</param>
        /// <param name="dueTime">The signal time of when the job should be initially triggered.</param>
        /// <param name="signalTime">A function that calculates the next time the job should be triggered.</param>
        /// <returns>
        ///     Returns a <see cref="RecurringJob" /> representing the job that has been added or updated.
        /// </returns>
        public RecurringJob Add(string id, Action task, TimeSpan dueTime, Func<DateTime, DateTime> signalTime)
        {
            RecurringJob job;

            if (!_Jobs.ContainsKey(id))
            {
                job = new RecurringJob(id, task);
                job.Schedule(dueTime, signalTime);

                _Jobs.TryAdd(id, job);
            }
            else
            {
                job = RecurringJob.Schedule(id, task, dueTime, signalTime);

                _Jobs.TryUpdate(id, job, _Jobs[id]);
            }

            return job;
        }


        /// <summary>
        ///     Removes the <see cref="RecurringJob" /> with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id)
        {
            RecurringJob job;
            if (_Jobs.TryRemove(id, out job))
            {
                job.Dispose();
            }
        }

        /// <summary>
        ///     Removes and stops all of the jobs.
        /// </summary>
        public void Clear()
        {
            foreach (var job in _Jobs.Values)
            {
                job.Dispose();
            }

            _Jobs.Clear();
        }

        /// <summary>
        ///     Waits for all of the running jobs to recieve a signal.
        /// </summary>
        /// <param name="timeout">
        ///     A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a
        ///     <see cref="T:System.TimeSpan" /> that represents -1 milliseconds, to wait indefinitely.
        /// </param>
        /// <returns>Returns a <see cref="bool" /> representing <c>true</c> when every job has received a signal; otherwise, false.</returns>
        public bool WaitAll(TimeSpan timeout)
        {
            var waitHandles = _Jobs.Select(o => o.Value.Wait).ToArray();
            if (waitHandles.Any())
            {
                return WaitHandle.WaitAll(waitHandles, timeout);
            }

            return true;
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
                this.Clear();
            }
        }

        #endregion
    }
}