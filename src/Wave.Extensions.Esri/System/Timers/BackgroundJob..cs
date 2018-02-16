using System.ComponentModel;
using System.Threading;

namespace System.Timers
{
    /// <summary>
    ///     Provides methods for creating <i>fire-and-forget</i> and <i>delayed</i> jobs
    ///     that are executed on <see cref="BackgroundWorker" /> threads.
    /// </summary>
    public class BackgroundJob : IDisposable, IEquatable<BackgroundJob>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BackgroundJob" /> class.
        /// </summary>
        /// <param name="method">The delegate function that is executed.</param>
        public BackgroundJob(Action method)
            : this(Guid.NewGuid().ToString("D"), method)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BackgroundJob" /> class.
        /// </summary>
        /// <param name="id">The unique identifier for the job.</param>
        /// <param name="method">The delegate function that is executed.</param>
        public BackgroundJob(string id, Action method)
        {
            this.Id = id;
            this.Method = method;
            this.Wait = new ManualResetEvent(false);
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
        public bool IsRunning => !this.Wait.WaitOne(TimeSpan.Zero);

        #endregion

        #region Protected Internal Properties

        /// <summary>
        ///     Gets or sets the wait.
        /// </summary>
        /// <value>
        ///     The wait.
        /// </value>
        protected internal ManualResetEvent Wait { get; set; }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the method.
        /// </summary>
        /// <value>
        ///     The method.
        /// </value>
        protected Action Method { get; set; }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region IEquatable<BackgroundJob> Members

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(BackgroundJob other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Id, other.Id);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a new fire-and-forget job based on a given method call expression.
        /// </summary>
        /// <param name="method">Instance method will executed.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing a unique identifier of a background job.
        /// </returns>
        public static string Run(Action method)
        {
            var job = new BackgroundJob(method);
            job.RunAsync();

            return job.Id;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != this.GetType()) return false;

            return Equals((BackgroundJob) obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        /// <summary>
        ///     Creates a new background job based on a specified method
        ///     call and schedules it to be trigger after a given delay.
        /// </summary>
        /// <param name="method">Instance method will executed.</param>
        /// <param name="delay">Delay, after which the job will be triggered.</param>
        /// <returns>
        ///     Returns a <see cref="BackgroundJob" /> representing the background job.
        /// </returns>
        public static BackgroundJob Schedule(Action method, TimeSpan delay)
        {
            var job = new BackgroundJob(method);
            job.Schedule(delay);

            return job;
        }


        /// <summary>
        ///     Schedules the job to be executed based on the a regular time interval.
        /// </summary>
        /// <param name="delay">Delay, after which the job will be triggered.</param>
        public virtual void Schedule(TimeSpan delay)
        {
            this.Schedule(Math.Abs(delay.TotalMilliseconds));
        }

        /// <summary>
        ///     Schedules the job to be executed based on the a regular time interval.
        /// </summary>
        /// <param name="interval">The interval in milliseconds of when the job should be executed.</param>
        public virtual void Schedule(double interval)
        {
            var timer = new Timer(interval);
            timer.Start();
            timer.Elapsed += (sender, args) =>
            {
                // The method can be executed simultaneously on two thread pool threads if the timer interval is 
                // less than the time required to execute the method.
                Timer ts = (Timer) sender;
                ts.Stop();

                Run();

                // Cleanup the timer.
                timer.Dispose();
                timer = null;
            };
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Wait.Set();
                Wait.Dispose();
            }
        }

        /// <summary>
        ///     Triggers the execution of the method delegate.
        /// </summary>
        protected void Run()
        {
            Wait.Reset();

            try
            {
                Method?.Invoke();
            }
            finally
            {
                Wait.Set();
            }
        }

        /// <summary>
        ///     Triggers the execution of the method delegate on STA thread, which will be executed on a
        ///     <see cref="BackgroundWorker" /> thread.
        /// </summary>
        protected void RunAsync()
        {
            Wait.Reset();

            try
            {
                Task.Run(Method);
            }
            finally
            {
                Wait.Set();
            }
        }

        #endregion
    }
}