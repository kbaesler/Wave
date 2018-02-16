namespace System.Threading.Tasks
{
    /// <summary>
    ///     Provides the ability to use the <see cref="Task" /> thread using the <see cref="TaskScheduler" />.
    /// </summary>
    public static class TaskFactoryExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Runs the background process as a <see cref="Threading.Tasks.Task" /> thread using the specified arguments that are
        ///     passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="creationOptions">
        ///     A TaskCreationOptions value that controls the behavior of the created
        ///     <see cref="T:System.Threading.Tasks.Task" />
        /// </param>
        /// <returns></returns>
        public static async Task<TResult> Run<TResult>(this TaskFactory source, Func<TResult> task, TaskScheduler scheduler, TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return await source.StartNew(task, CancellationToken.None, creationOptions, scheduler);
        }

        /// <summary>
        ///     Runs the background process as a <see cref="Threading.Tasks.Task" /> thread using the specified arguments that are
        ///     passed to the methods.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="creationOptions">
        ///     A TaskCreationOptions value that controls the behavior of the created
        ///     <see cref="T:System.Threading.Tasks.Task" />
        /// </param>
        /// <returns></returns>
        public static Task Run(this TaskFactory source, Action task, CancellationToken cancellationToken, TaskScheduler scheduler, TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return source.StartNew(task, cancellationToken, creationOptions, scheduler);
        }

        /// <summary>
        ///     Runs the background process as a <see cref="Threading.Tasks.Task" /> thread using the specified arguments that are
        ///     passed to the methods.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="task">The delegate that handles the execution on the work.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="creationOptions">
        ///     A TaskCreationOptions value that controls the behavior of the created
        ///     <see cref="T:System.Threading.Tasks.Task" />
        /// </param>
        /// <returns></returns>
        public static Task Run(this TaskFactory source, Action task, TaskScheduler scheduler, TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return Run(source, task, CancellationToken.None, scheduler, creationOptions);
        }

        #endregion
    }
}