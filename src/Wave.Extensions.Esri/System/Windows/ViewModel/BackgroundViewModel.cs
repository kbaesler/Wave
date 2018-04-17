using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace System.Windows
{
    /// <summary>
    ///     An abstract view model that is used to execute operations on a background thread using a
    ///     <see cref="BackgroundWorker" /> component.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BackgroundViewModel : BaseViewModel
    {
        #region Fields

        private BackgroundWorker _Worker;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundViewModel"/> class.
        /// </summary>
        protected BackgroundViewModel()
            : this("")
        {

        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BackgroundViewModel" /> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        protected BackgroundViewModel(string displayName)
            : base(displayName)
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BackgroundViewModel" /> is busy.
        /// </summary>
        /// <value><c>true</c> if busy; otherwise, <c>false</c>.</value>
        public virtual bool IsBusy
        {
            get { return _Worker != null && _Worker.IsBusy; }
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
            get { return (_Worker != null && _Worker.IsBusy && _Worker.CancellationPending); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Requests cancellation of a pending background operation.
        /// </summary>
        protected virtual void CancelAsync()
        {
            if (_Worker != null && _Worker.IsBusy)
            {
                _Worker.CancelAsync();
            }
        }

        /// <summary>
        /// Raises the System.ComponentModel.BackgroundWorker.ProgressChanged event.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void ReportProgress(string message)
        {
            this.ReportProgress(-1, message);
        }

        /// <summary>
        ///     Raises the System.ComponentModel.BackgroundWorker.ProgressChanged event.
        /// </summary>
        /// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is complete.</param>
        /// <param name="userState">
        ///     The state object passed to
        ///     System.ComponentModel.BackgroundWorker.RunWorkerAsync(System.Object).
        /// </param>
        protected virtual void ReportProgress<T>(int percentProgress, T userState)
        {
            if (_Worker != null && _Worker.IsBusy)
            {
                _Worker.ReportProgress(percentProgress, userState);
            }
        }

        /// <summary>
        /// Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        /// passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execute">The delegate that handles the execution on the work.</param>
        /// <param name="completion">The delegate that handles when the work has completed.</param>
        protected void Run<TResult>(Func<object, TResult> execute, Action<TResult, bool, Exception, object> completion)
        {
            this.Run(execute, null, completion);
        }

        /// <summary>
        /// Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        /// passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execute">The delegate that handles the execution on the work.</param>
        /// <param name="progress">The delegate that handles reporting the progress.</param>
        /// <param name="completion">The delegate that handles when the work has completed.</param>
        protected void Run<TResult>(Func<object, TResult> execute, Action<int, string> progress, Action<TResult, bool, Exception, object> completion)
        {
            this.Run(null, execute, progress, completion);
        }
        /// <summary>
        /// Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        /// passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TProgress">The type of the progress.</typeparam>
        /// <param name="execute">The delegate that handles the execution on the work.</param>
        /// <param name="progress">The delegate that handles reporting the progress.</param>
        /// <param name="completion">The delegate that handles when the work has completed.</param>
        protected void Run<TResult, TProgress>(Func<object, TResult> execute, Action<int, TProgress> progress, Action<TResult, bool, Exception, object> completion)
        {
            this.Run(null, execute, progress, completion);
        }

        /// <summary>
        /// Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        /// passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TProgress">The type of the progress.</typeparam>
        /// <param name="arguments">The arguments.</param>
        /// <param name="execute">The delegate that handles the execution on the work.</param>
        /// <param name="progress">The delegate that handles reporting the progress.</param>
        /// <param name="completion">The delegate that handles when the work has completed.</param>
        /// <exception cref="System.ArgumentNullException">execute</exception>
        protected void Run<TResult, TProgress>(object arguments, Func<object, TResult> execute, Action<int, TProgress> progress, Action<TResult, bool, Exception, object> completion)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _Worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            // The work event handler.
            _Worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                this.OnPropertyChanged("IsBusy");

                e.Result = execute(arguments);
            };

            // The complete event handler.
            _Worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                this.OnPropertyChanged("IsBusy");

                if (completion != null)
                {
                    TResult result = (e.Error == null) ? (TResult)e.Result : default(TResult);
                    completion(result, e.Cancelled, e.Error, e.UserState);
                }

                _Worker.Dispose();
            };

            // The progress event handler.
            _Worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                if (progress != null)
                {
                    progress(e.ProgressPercentage, (TProgress)e.UserState);
                }
            };

            // Run asynchronously.
            _Worker.RunWorkerAsync(arguments);
        }

        #endregion
    }
}