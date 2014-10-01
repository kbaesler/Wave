using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows
{
    /// <summary>
    ///     An abstract view model that is used to execute operations on a background thread using a
    ///     <see cref="BackgroundWorker" /> component.
    /// </summary>
    /// <typeparam name="T">The type of the arguments.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BackgroundViewModel : BaseViewModel
    {
        #region Fields

        private readonly BackgroundWorker _Worker;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BackgroundViewModel" /> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        protected BackgroundViewModel(string displayName)
            : base(displayName)
        {
            _Worker = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BackgroundViewModel" /> is busy.
        /// </summary>
        /// <value><c>true</c> if busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get { return _Worker.IsBusy; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets a value indicating whether the application has requested cancellation
        ///     of a background operation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the application has requested cancellation of a background operation;
        ///     otherwise, false. The default is <c>false</c>.
        /// </value>
        protected bool CancellationPending
        {
            get { return (_Worker.IsBusy && _Worker.CancellationPending); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Requests cancellation of a pending background operation.
        /// </summary>
        protected void CancelAsync()
        {
            if (_Worker.IsBusy)
            {
                _Worker.CancelAsync();
            }
        }

        /// <summary>
        ///     Raises the System.ComponentModel.BackgroundWorker.ProgressChanged event.
        /// </summary>
        /// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is complete.</param>
        /// <param name="userState">
        ///     The state object passed to
        ///     System.ComponentModel.BackgroundWorker.RunWorkerAsync(System.Object).
        /// </param>
        protected void ReportProgress(int percentProgress, object userState)
        {
            if (_Worker.IsBusy)
            {
                _Worker.ReportProgress(percentProgress, userState);
            }
        }

        /// <summary>
        /// Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        /// passed to the methods.
        /// </summary>
        /// <param name="execute">The delegate that handles the execution on the work.</param>
        /// <param name="completion">The delegate that handles when the work has completed.</param>
        protected void Run(Func<object, object> execute, Action<object, bool, Exception, object> completion)
        {
            this.Run(null, execute, completion);
        }

        /// <summary>
        /// Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        /// passed to the methods.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="execute">The delegate that handles the execution on the work.</param>
        /// <param name="completion">The delegate that handles when the work has completed.</param>
        /// <exception cref="System.ArgumentNullException">execute</exception>
        protected void Run(object arguments, Func<object, object> execute, Action<object, bool, Exception, object> completion)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            // The work event handler.
            _Worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                this.OnPropertyChanged("IsBusy");

                e.Result = execute(arguments);
            };

            // The complete event handler.
            _Worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (completion != null)
                {
                    object result = (e.Error == null) ? e.Result : null;
                    completion(result, e.Cancelled, e.Error, e.UserState);
                }

                this.OnPropertyChanged("IsBusy");
            };

            // Run asynchronously.
            _Worker.RunWorkerAsync(arguments);
        }

        #endregion
    }
}