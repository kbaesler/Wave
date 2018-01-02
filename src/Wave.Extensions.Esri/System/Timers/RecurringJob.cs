using System.Threading;

using Timer = System.Timers.Timer;

namespace System
{
    /// <summary>
    ///     A schedule task that executes an action every time the timer elapses.
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public sealed class RecurringJob : IDisposable, IEquatable<RecurringJob>
    {
        #region Fields

        private readonly ManualResetEvent _WaitHandle = new ManualResetEvent(false);

        private Timer _Timer;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RecurringJob" /> class.
        /// </summary>
        /// <param name="id">The unique identifier for the job.</param>
        /// <param name="task">The delegate function that is executed.</param>
        public RecurringJob(string id, Action task)
        {
            Id = id;
            Task = task;
        }


        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.ComponentModel.RecurringJob" /> class.
        /// </summary>
        /// <param name="task">The delgate function that is executed.</param>
        public RecurringJob(Action task)
            : this(Guid.NewGuid().ToString("D"), task)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the unique identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public string Id { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get { return !_WaitHandle.WaitOne(TimeSpan.Zero); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is scheduled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is scheduled; otherwise, <c>false</c>.
        /// </value>
        public bool IsScheduled
        {
            get { return (_Timer != null && _Timer.Interval > 0 && _Timer.Enabled); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        ///     Gets the wait handle.
        /// </summary>
        /// <value>
        ///     The wait.
        /// </value>
        internal WaitHandle Wait
        {
            get { return _WaitHandle; }
        }

        #endregion

        #region Private Properties

        /// <summary>
        ///     Gets the task.
        /// </summary>
        /// <value>
        ///     The task.
        /// </value>
        private Action Task { get; }

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

        #region IEquatable<RecurringJob> Members

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(RecurringJob other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as RecurringJob;
            if (other == null) return false;

            return string.Equals(other.Id, this.Id);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return (Id != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Id) : 0);
        }

        /// <summary>
        ///     Schedules the frequency of the specified delegate to be executed.
        /// </summary>
        /// <param name="task">The delegate function that is executed.</param>
        /// <param name="dueTime">The signal time of when the job should be initially triggered.</param>
        /// <param name="signalTime">A function that calculates the next time the job should be triggered.</param>
        /// <returns>Returns a <see cref="RecurringJob" /> representing the recurring job instance.</returns>
        public static RecurringJob Schedule(Action task, TimeSpan dueTime, Func<DateTime, DateTime> signalTime)
        {
            var job = new RecurringJob(task);
            job.Schedule(dueTime, signalTime);
            return job;
        }

        /// <summary>
        ///     Schedules the frequency of the specified delegate to be executed.
        /// </summary>
        /// <param name="id">The unique identifier for the job.</param>
        /// <param name="task">The delegate function that is executed.</param>
        /// <param name="dueTime">The signal time of when the job should be initially triggered.</param>
        /// <param name="signalTime">A function that calculates the next time the job should be triggered.</param>
        /// <returns>Returns a <see cref="RecurringJob" /> representing the recurring job instance.</returns>
        public static RecurringJob Schedule(string id, Action task, TimeSpan dueTime, Func<DateTime, DateTime> signalTime)
        {
            var job = new RecurringJob(id, task);
            job.Schedule(dueTime, signalTime);
            return job;
        }

        /// <summary>
        ///     Schedules the frequency of the job.
        /// </summary>
        /// <param name="dueTime">The signal time of when the job should be initially triggered.</param>
        /// <param name="signalTime">A function that calculates the next time the job should be triggered.</param>
        public void Schedule(TimeSpan dueTime, Func<DateTime, DateTime> signalTime)
        {
            _Timer = new Timer(dueTime.TotalMilliseconds);
            _Timer.Start();
            _Timer.Elapsed += (sender, args) =>
            {
                // The method can be executed simultaneously on two thread pool threads if the timer interval is 
                // less than the time required to execute the method.
                Timer ts = (Timer) sender;
                ts.Stop();

                Execute();

                // The next signal time will be scheduled based on the current time. 
                var nextSignalTime = signalTime(DateTime.Now);
                var milliseconds = (nextSignalTime - DateTime.Now).TotalMilliseconds;
                if (milliseconds > 0)
                {
                    ts.Interval = milliseconds;
                    ts.Start();
                }
            };
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
                if (_Timer != null)
                    _Timer.Dispose();

                _WaitHandle.Set();
                _WaitHandle.Dispose();
            }
        }

        /// <summary>
        ///     Executes the job immediately.
        /// </summary>
        private void Execute()
        {
            _WaitHandle.Reset();

            try
            {
                if (Task != null)
                    Task.Invoke();
            }
            finally
            {
                _WaitHandle.Set();
            }
        }

        #endregion
    }
}