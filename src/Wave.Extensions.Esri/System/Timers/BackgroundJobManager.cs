using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace System.Timers
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides access to a collection of <see cref="T:BackgroundJob" /> objects.
    /// </summary>
    public class BackgroundJobManager : IDisposable
    {
        #region Fields

        private readonly ConcurrentDictionary<string, BackgroundJob> _Jobs = new ConcurrentDictionary<string, BackgroundJob>();

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
        ///     Adds the <see cref="BackgroundJob" /> with the specified identifier when it doesn't exist, otherwise adds it to the
        ///     colleciton.
        /// </summary>
        /// <param name="job">The job.</param>
        public void Add(BackgroundJob job)
        {
            if (!_Jobs.ContainsKey(job.Id))
            {
                _Jobs.TryAdd(job.Id, job);
            }
            else
            {
                _Jobs.TryUpdate(job.Id, job, _Jobs[job.Id]);
            }
        }

        /// <summary>
        ///     Removes all of the jobs and stops them from recurring but will not terminate active jobs.
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
        ///     Removes the <see cref="BackgroundJob" /> with the specified identifier.
        /// </summary>
        /// <param name="job">The job.</param>
        public void Remove(BackgroundJob job)
        {
            BackgroundJob value;
            if (_Jobs.TryRemove(job.Id, out value))
            {
                value.Dispose();
            }
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
            WaitHandle[] waitHandles = _Jobs.Select(o => o.Value.Wait).Cast<WaitHandle>().ToArray();
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