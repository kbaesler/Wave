using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace System.Threading.Tasks
{
    /// <summary>
    ///     Provides a task scheduler that uses STA (short for Single-Threaded apartment) threads in order to support the
    ///     execution of tasks. It is necessary to set the maximum number of threads desired for this scheduler.
    /// </summary>
    /// <seealso cref="System.Threading.Tasks.TaskScheduler" />
    /// <seealso cref="System.IDisposable" />
    public sealed class STATaskScheduler : TaskScheduler, IDisposable
    {
        #region Fields

        /// <summary>
        ///     The STA threads used by the scheduler.
        /// </summary>
        private readonly List<Thread> _Threads;

        /// <summary>
        ///     The default <see cref="STATaskScheduler" /> that limits the concurrency to 1 thread.
        /// </summary>
        public new static readonly STATaskScheduler Default = new STATaskScheduler(1);

        /// <summary>
        ///     Stores the queued tasks to be executed by our pool of STA threads.
        /// </summary>
        private BlockingCollection<Task> _Tasks;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="STATaskScheduler" /> class.
        /// </summary>
        /// <param name="degreeOfParallelism">The number of threads that should be created and used by this scheduler.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">degreeOfParallelism</exception>
        public STATaskScheduler(int degreeOfParallelism)
        {
            // Validate arguments 
            if (degreeOfParallelism < 1) throw new ArgumentOutOfRangeException(nameof(degreeOfParallelism));

            // Initialize the tasks collection 
            _Tasks = new BlockingCollection<Task>();

            // Create the threads to be used by this scheduler 
            _Threads = Enumerable.Range(0, degreeOfParallelism).Select(i =>
            {
                var thread = new Thread(() =>
                {
                    // Continually get the next task and try to execute it. 
                    // This will continue until the scheduler is disposed and no more tasks remain. 
                    foreach (var t in _Tasks.GetConsumingEnumerable())
                    {
                        TryExecuteTask(t);
                    }
                });
                thread.IsBackground = true;
                thread.SetApartmentState(ApartmentState.STA);
                return thread;
            }).ToList();

            // Start all of the threads 
            _Threads.ForEach(t => t.Start());
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the maximum concurrency level supported by this scheduler.
        /// </summary>
        /// <value>
        ///     The maximum concurrency level.
        /// </value>
        public override int MaximumConcurrencyLevel => _Threads.Count;

        #endregion

        #region IDisposable Members

        /// <inheritdoc />
        /// <summary>
        ///     Cleans up the scheduler by indicating that no more tasks will be queued.
        ///     This method blocks until all threads successfully shutdown.
        /// </summary>
        public void Dispose()
        {
            if (_Tasks != null)
            {
                // Indicate that no new tasks will be coming in 
                _Tasks.CompleteAdding();

                // Wait for all threads to finish processing tasks 
                foreach (var thread in _Threads) thread.Join();

                // Cleanup 
                _Tasks.Dispose();
                _Tasks = null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Waits for the action to complete and cancels the task when the delay is reached
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="delay">The time interval until the task is cancelled.</param>
        /// <returns></returns>
        public bool Wait(Action action, TimeSpan delay)
        {
            var task = Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this);
            return task.Wait(delay);
        }

        /// <summary>
        ///     A handy wrapper around Task.Factory.StartNew
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public Task Run(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this);
        }

        /// <summary>
        ///     A handy wrapper around Task.Factory.StartNew
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public Task Run(Action action, CancellationToken token)
        {
            return Task.Factory.StartNew(action, token, TaskCreationOptions.None, this);
        }

        /// <summary>
        ///     A handy wrapper around Task.Factory.StartNew
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public Task Run(Func<Task> action, CancellationToken token)
        {
            return Task.Factory.StartNew(action, token, TaskCreationOptions.None, this).Unwrap();
        }

        /// <summary>
        ///     A handy wrapper around Task.Factory.StartNew
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public Task<TResult> Run<TResult>(Func<Task<TResult>> action, CancellationToken token)
        {
            return Task.Factory.StartNew(action, token, TaskCreationOptions.None, this).Unwrap();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Provides a list of the scheduled tasks for the debugger to consume.
        /// </summary>
        /// <returns>
        ///     An enumerable of all tasks currently scheduled.
        /// </returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // Serialize the contents of the blocking collection of tasks for the debugger 
            return _Tasks.ToArray();
        }

        /// <summary>
        ///     Queues a Task to be executed by this scheduler.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        protected override void QueueTask(Task task)
        {
            // Push it into the blocking collection of tasks 
            _Tasks.Add(task);
        }

        /// <summary>
        ///     Determines whether a Task may be inlined.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">Whether the task was previously queued.</param>
        /// <returns>
        ///     true if the task was successfully inlined; otherwise, false.
        /// </returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // Try to inline if the current thread is STA 
            return
                Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
                this.TryExecuteTask(task);
        }

        #endregion
    }
}