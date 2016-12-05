using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace System.Windows
{
    /// <summary>
    ///     An abstract view model that is used to execute operations on a background thread using a
    ///     <see cref="Dispatcher" /> component.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class DispatcherViewModel : BaseViewModel
    {
        #region Fields

        private bool _CancellationPending;
        private bool _IsBusy;
        private bool _IsAborted;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DispatcherViewModel" /> class.
        /// </summary>
        protected DispatcherViewModel()
            : this("")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DispatcherViewModel" /> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        protected DispatcherViewModel(string displayName)
            : base(displayName)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="DispatcherViewModel" /> is busy.
        /// </summary>
        /// <value><c>true</c> if busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;

                OnPropertyChanged("IsBusy");
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="DispatcherViewModel" /> is aborted.
        /// </summary>
        /// <value><c>true</c> if aborted; otherwise, <c>false</c>.</value>
        public bool IsAborted
        {
            get { return _IsAborted; }
            set
            {
                _IsAborted = value;

                OnPropertyChanged("IsAborted");
            }
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
            get { return _CancellationPending; }
            set
            {
                _CancellationPending = value;

                OnPropertyChanged("CancellationPending");
            }
        }

        /// <summary>
        ///     Gets the dispatcher operation.
        /// </summary>
        /// <value>
        ///     The dispatcher operation.
        /// </value>
        protected DispatcherOperation DispatcherOperation { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Requests cancellation of a pending background operation.
        /// </summary>
        protected void CancelAsync()
        {
            if (this.IsBusy)
            {
                this.DispatcherOperation.Abort();
                this.CancellationPending = true;
            }
        }

        /// <summary>
        /// Runs the action on a <see cref="Dispatcher" /> thread using the delegates
        /// passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execute">The delegate that handles the execution on the work.</param>
        /// <param name="completion">The delegate that handles when the work has completed.</param>
        protected void Run<TResult>(Func<TResult> execute, Action<TResult, bool, DispatcherOperationStatus> completion)
        {
            this.Run(DispatcherPriority.Background, execute, completion);
        }

        /// <summary>
        /// Runs the action on a <see cref="Dispatcher"/> thread using the delegates
        /// passed to the methods.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="priority">The priority, relative to the other pending operations in the Dispatcher event queue, the
        /// specified method is invoked.</param>
        /// <param name="execute">The delegate that handles the execution on the work.</param>
        /// <param name="completion">The delegate that handles when the work has completed.</param>
        /// <exception cref="ArgumentNullException">execute</exception>
        protected void Run<TResult>(DispatcherPriority priority, Func<TResult> execute, Action<TResult, bool, DispatcherOperationStatus> completion)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.IsAborted = false;
            this.IsBusy = true;

            this.DispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, execute);
            this.DispatcherOperation.Aborted += (sender, args) => this.IsAborted = true;
            this.DispatcherOperation.Completed += (sender, args) =>
            {
                if (completion != null)
                {
                    completion((TResult)this.DispatcherOperation.Result, this.IsAborted, this.DispatcherOperation.Status);
                }

                this.IsBusy = false;
            };
        }

        #endregion
    }
}