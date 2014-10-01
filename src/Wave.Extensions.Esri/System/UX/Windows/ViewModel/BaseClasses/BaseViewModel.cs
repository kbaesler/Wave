using System.Windows.Input;

namespace System.Windows
{
    /// <summary>
    ///     An abstract view model pattern class use for notification binding in WPF applications.
    /// </summary>
    public abstract class BaseViewModel : Observable, IDisposable
    {
        #region Fields

        private string _DisplayName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewModel" /> class.
        /// </summary>
        protected BaseViewModel()
            : this(string.Empty)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewModel" /> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        protected BaseViewModel(string displayName)
        {
            this.DisplayName = displayName;
            this.CloseCommand = new DelegateCommand(param => this.OnClose());
        }

        #endregion

        #region Events

        /// <summary>
        ///     Raised when invoked, attempts to remove this view model from the user interface.
        /// </summary>
        public event EventHandler Close;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the command that, when invoked, attempts to remove this view model from the user interface.
        /// </summary>
        /// <value>The close command.</value>
        public ICommand CloseCommand { get; private set; }

        /// <summary>
        ///     Gets or sets the display name for the view model.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return _DisplayName; }
            set
            {
                _DisplayName = value;

                base.OnPropertyChanged("DisplayName");
            }
        }

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

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.OnClose();
            }
        }

        /// <summary>
        ///     Raises the <see cref="Close" /> event.
        /// </summary>
        protected virtual void OnClose()
        {
            EventHandler handler = this.Close;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion
    }
}