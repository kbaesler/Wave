using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows
{
    /// <summary>
    ///     An abstract view model that is used to execute operations on a background thread using a
    ///     <see cref="BackgroundWorker" /> component.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BackgroundViewModel : BackgroundViewModel<object>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BackgroundViewModel" /> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        protected BackgroundViewModel(string displayName)
            : base(displayName)
        {
        }

        #endregion
    }

    /// <summary>
    ///     An abstract view model that is used to execute operations on a background thread using a
    ///     <see cref="BackgroundWorker" /> component.
    /// </summary>
    /// <typeparam name="T">The type of the arguments.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BackgroundViewModel<T> : BaseViewModel
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
            _Worker = new BackgroundWorker {WorkerSupportsCancellation = true, WorkerReportsProgress = true};
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
        ///     Handles when the DoWork event of the BackgroundWorker thread.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs" /> instance containing the event data.</param>
        protected abstract void OnRun(DoWorkEventArgs e);

        /// <summary>
        ///     Handles when the RunWorkerCompleted event of the BackgroundWorker thread.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        protected abstract void OnRunCompleted(RunWorkerCompletedEventArgs e);

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
        ///     Runs the background process on a <see cref="BackgroundWorker" /> thread using the specified arguments that are
        ///     passed to the methods.
        ///     You must override the <see cref="OnRun" /> and <see cref="OnRunCompleted" /> methods.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        protected void Run(T arguments)
        {
            // The work event handler.
            _Worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                this.OnPropertyChanged("IsBusy");
                this.OnRun(e);
            };

            // The complete event handler.
            _Worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                this.OnRunCompleted(e);
                this.OnPropertyChanged("IsBusy");
            };

            // Run asynchronously.
            _Worker.RunWorkerAsync(arguments);
        }

        #endregion
    }
}