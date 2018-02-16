namespace System.Timers
{
    /// <summary>
    ///     A recurring job that is scheduled based on the CRON expression.
    /// </summary>
    public class CronJob : RecurringJob
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CronJob" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="expression">The CRON expression.</param>
        /// <param name="method">The delegate function that will be executed.</param>
        public CronJob(string id, string expression, Action method)
            : base(id, method)
        {
            this.Expression = expression;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CronJob" /> class.
        /// </summary>
        /// <param name="expression">The CRON expression.</param>
        /// <param name="method">The delegate function that will be executed.</param>
        public CronJob(string expression, Action method)
            : this(Guid.NewGuid().ToString("D"), expression, method)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The CRON expression that is used to schedule the job.
        /// </summary>
        /// <value>
        ///     The expression.
        /// </value>
        public string Expression { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a new cron job based on a specified method
        ///     call and schedules it to be trigger based on the expression.
        /// </summary>
        /// <param name="expression">The CRON expression.</param>
        /// <param name="method">Instance method will executed.</param>
        /// <returns>
        ///     Returns a <see cref="CronJob" /> representing the CRON job.
        /// </returns>
        public static CronJob Schedule(string expression, Action method)
        {
            var job = new CronJob(expression, method);
            var delay = job.GetDelay();
            job.Schedule(delay);

            return job;
        }

        /// <summary>
        ///     Schedules the job to be executed based on the CRON expression.
        /// </summary>
        /// <param name="interval">The interval in milliseconds of when the job should be executed.</param>
        public override void Schedule(double interval)
        {
            Timer.Interval = interval;
            Timer.Start();
            Timer.Elapsed += (sender, args) =>
            {
                // The method can be executed simultaneously on two thread pool threads if the timer interval is 
                // less than the time required to execute the method.
                Timer ts = (Timer) sender;
                ts.Stop();

                Run();

                // Calculate the next time the method should be scheduled.
                var occurrence = GetDelay();
                if (occurrence != TimeSpan.Zero)
                {
                    ts.Interval = occurrence.TotalMilliseconds;
                    ts.Start();
                }
            };
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the delay until the next occurrence of the cron job.
        /// </summary>
        /// <returns>Returns a <see cref="TimeSpan" /> representing the delay until the next occurrence.</returns>
        protected virtual TimeSpan GetDelay()
        {
            var occurrence = GetNextOccurrence();
            var delay = !occurrence.HasValue ? TimeSpan.Zero : (occurrence.Value - DateTime.UtcNow);
            return delay;
        }

        /// <summary>
        ///     Gets the next occurrence of the job.
        /// </summary>
        /// <returns>Returns a <see cref="DateTime" /> representing the next occurrence.</returns>
        protected virtual DateTime? GetNextOccurrence()
        {
            return DateTime.UtcNow;
        }

        #endregion
    }
}