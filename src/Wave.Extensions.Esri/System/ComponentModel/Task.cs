using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace System.ComponentModel
{
    /// <summary>
    ///     Provides a STA (Single Thread Apartment) approach to running long tasks in the background using a
    ///     <see cref="BackgroundWorker" />.
    /// </summary>
    public static class Task
    {
        #region Fields

        /// <summary>
        ///     The current single apartment synchronization context.
        /// </summary>
        private static readonly SynchronizationContext Context = new STASynchronizationContext();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        ///     passed to the methods.
        /// </summary>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <returns>
        ///     Returns a <see cref="BackgroundWorker" /> representing the executing task.
        /// </returns>
        public static BackgroundWorker Run(Action task)
        {
            return Run(task, Context);
        }

        /// <summary>
        ///     Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        ///     passed to the methods.
        /// </summary>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>
        ///     Returns a <see cref="BackgroundWorker" /> representing the executing task.
        /// </returns>
        public static BackgroundWorker Run(Action task, SynchronizationContext synchronizationContext)
        {
            Func<bool> execute = () =>
            {
                task();

                return true;
            };

            return Run(execute, synchronizationContext);
        }

        /// <summary>
        ///     Waits for the action that is issued as a background process on a <see cref="BackgroundWorker" /> thread to
        ///     complete.
        /// </summary>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <returns>Returns a <see cref="TimeSpan" /> representing the time waited for the task to complete.</returns>
        public static TimeSpan Wait(Action task)
        {
            return Wait(task, Context);
        }

        /// <summary>
        ///     Waits for the action that is issued as a background process on a <see cref="BackgroundWorker" /> thread to
        ///     complete.
        /// </summary>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>Returns a <see cref="TimeSpan" /> representing the time waited for the task to complete.</returns>
        public static TimeSpan Wait(Action task, SynchronizationContext synchronizationContext)
        {
            Func<bool> execute = () =>
            {
                task();

                return true;
            };

            Stopwatch timer = Stopwatch.StartNew();

            using (var done = new AutoResetEvent(false))
            {
                Run(execute, done, synchronizationContext);

                done.WaitOne();
            }

            return timer.Elapsed;
        }


        /// <summary>
        ///     Waits for all of the actions (that are issued as a background process on a <see cref="BackgroundWorker" /> thread)
        ///     to complete.
        /// </summary>
        /// <param name="tasks">The delegates that handles the execution on the work.</param>
        /// <returns>Returns a <see cref="TimeSpan" /> representing the time waited for the tasks to complete.</returns>
        public static TimeSpan WaitAll(IEnumerable<Action> tasks)
        {
            return WaitAll(tasks, Context);
        }

        /// <summary>
        ///     Waits for all of the actions (that are issued as a background process on a <see cref="BackgroundWorker" /> thread)
        ///     to complete.
        /// </summary>
        /// <param name="tasks">The delegates that handles the execution on the work.</param>
        /// <returns>Returns a <see cref="TimeSpan" /> representing the time waited for the tasks to complete.</returns>
        public static TimeSpan WaitAll(IEnumerable<BackgroundWorker> tasks)
        {
            // Calling the WaitHandle.WaitAll() method must be done from a multithreaded apartment (MTA) thread. 
            // To launch multiple synchronized tasks, you first have to launch a worker thread that, in turn, will run the multiple threads.
            return Wait(() =>
            {
                var done = new List<WaitHandle>();

                foreach (var t in tasks)
                {
                    AutoResetEvent wait = new AutoResetEvent(false);

                    if (t.IsBusy)
                    {
                        t.RunWorkerCompleted += null;
                        t.RunWorkerCompleted += (sender, e) => wait.Set();
                    }
                    else
                    {
                        wait.Set();
                    }

                    done.Add(wait);
                }

                WaitHandle.WaitAll(done.ToArray());
            },
                new SynchronizationContext());
        }

        /// <summary>
        ///     Waits for all of the actions (that are issued as a background process on a <see cref="BackgroundWorker" /> thread)
        ///     to complete.
        /// </summary>
        /// <param name="tasks">The delegates that handles the execution on the work.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>Returns a <see cref="TimeSpan" /> representing the time waited for the tasks to complete.</returns>
        public static TimeSpan WaitAll(IEnumerable<Action> tasks, SynchronizationContext synchronizationContext)
        {
            // Calling the WaitHandle.WaitAll() method must be done from a multithreaded apartment (MTA) thread. 
            // To launch multiple synchronized tasks, you first have to launch a worker thread that, in turn, will run the multiple threads.
            return Wait(() =>
            {
                var done = new List<WaitHandle>();

                foreach (var t in tasks)
                {
                    AutoResetEvent wait = new AutoResetEvent(false);
                    Action task = t;

                    Func<bool> execute = () =>
                    {
                        task();

                        return true;
                    };

                    done.Add(wait);

                    Run(execute, wait, synchronizationContext);
                }

                WaitHandle.WaitAll(done.ToArray());
            },
                new SynchronizationContext());
        }

        /// <summary>
        ///     Waits for any of the actions (that are issued as a background process on a <see cref="BackgroundWorker" /> thread)
        ///     to complete.
        /// </summary>
        /// <param name="tasks">The delegates that handles the execution on the work.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>Returns a <see cref="TimeSpan" /> representing the time waited for the tasks to complete.</returns>
        public static TimeSpan WaitAny(IEnumerable<Action> tasks, SynchronizationContext synchronizationContext)
        {
            // Calling the WaitHandle.WaitAll() method must be done from a multithreaded apartment (MTA) thread. 
            // To launch multiple synchronized tasks, you first have to launch a worker thread that, in turn, will run the multiple threads.
            return Wait(() =>
            {
                var done = new List<WaitHandle>();

                foreach (var t in tasks)
                {
                    AutoResetEvent wait = new AutoResetEvent(false);
                    Action task = t;

                    Func<bool> execute = () =>
                    {
                        task();

                        return true;
                    };

                    done.Add(wait);

                    Run(execute, wait, synchronizationContext);
                }

                WaitHandle.WaitAny(done.ToArray());
            },
                new SynchronizationContext());
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        ///     passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>
        ///     Returns a <see cref="BackgroundWorker" /> representing the executing task.
        /// </returns>
        /// <exception cref="ArgumentNullException">task</exception>
        private static BackgroundWorker Run<TResult>(Func<TResult> task, SynchronizationContext synchronizationContext)
        {
            return Run(task, null, synchronizationContext);
        }

        /// <summary>
        ///     Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        ///     passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <param name="done">The handle that notifies a waiting thread that an event has occurred.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>
        ///     Returns a <see cref="BackgroundWorker" /> representing the executing task.
        /// </returns>
        /// <exception cref="ArgumentNullException">task</exception>
        private static BackgroundWorker Run<TResult>(Func<TResult> task, AutoResetEvent done, SynchronizationContext synchronizationContext)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            AsyncOperationManager.SynchronizationContext = synchronizationContext ?? SynchronizationContext.Current;

            var worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            worker.DoWork += (sender, e) => { e.Result = task(); };

            if (done != null)
            {
                worker.RunWorkerCompleted += (sender, e) => { done.Set(); };
            }

            worker.RunWorkerAsync();

            return worker;
        }

        #endregion
    }

    /// <summary>
    ///     A single threaded apartment synchornization context.
    /// </summary>
    /// <seealso cref="System.Threading.SynchronizationContext" />
    public class STASynchronizationContext : SynchronizationContext
    {
        #region Public Methods

        /// <summary>
        ///     When overridden in a derived class, dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            Thread t = new Thread(d.Invoke);
            t.SetApartmentState(ApartmentState.STA);
            t.Start(state);
        }

        #endregion
    }
}