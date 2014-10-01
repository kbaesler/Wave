using System.Runtime.InteropServices;

namespace System.Windows
{
    /// <summary>
    ///     A command that executes delegates to determine whether the command can execute, and to execute the command.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This command implementation is useful when the command simply needs to execute a method on a view model. The
    ///         delegate for
    ///         determining whether the command can execute is optional. If it is not provided, the command is considered
    ///         always eligible
    ///         to execute.
    ///     </para>
    /// </remarks>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class DelegateCommand : DelegateCommand<object>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="action">The delegate to invoke when the command is executed.</param>
        public DelegateCommand(Action<object> action)
            : base(action)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="action">The delegate to invoke when the command is executed.</param>
        /// <param name="predicate">The delegate to invoke to determine whether the command can execute.</param>
        public DelegateCommand(Action<object> action, Predicate<object> predicate)
            : base(action, predicate)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        protected DelegateCommand()
            : base(null)
        {
        }

        #endregion
    }

    /// <summary>
    ///     A command that executes delegates to determine whether the command can execute, and to execute the command.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This command implementation is useful when the command simply needs to execute a method on a view model. The
    ///         delegate for determining whether the command can execute is optional. If it is not provided, the command is
    ///         considered
    ///         always eligible to execute.
    ///     </para>
    /// </remarks>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class DelegateCommand<T> : Command
    {
        #region Fields

        private readonly Predicate<T> _CanExecute;
        private readonly Action<T> _Execute;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="action">The delegate to invoke when the command is executed.</param>
        public DelegateCommand(Action<T> action)
            : this(action, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="action">The delegate to invoke when the command is executed.</param>
        /// <param name="predicate">The delegate to invoke to determine whether the command can execute.</param>
        public DelegateCommand(Action<T> action, Predicate<T> predicate)
        {
            if (action == null)
                throw new ArgumentNullException("action", @"The action cannot be null.");

            _Execute = action;
            _CanExecute = predicate;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        /// <returns>
        ///     true if this command can be executed; otherwise, falSE.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            if (_CanExecute == null)
                return true;

            return _CanExecute((T) parameter);
        }

        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        public override void Execute(object parameter)
        {
            _Execute((T) parameter);
        }

        #endregion
    }
}