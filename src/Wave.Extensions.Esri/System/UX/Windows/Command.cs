using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace System.Windows
{
    /// <summary>
    ///     An abstract implementation of the <see cref="ICommand" /> interface used for actions in WPF controls.
    /// </summary>
    public abstract class Command : ICommand
    {
        #region Fields

        private readonly Dispatcher _Dispatcher;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class.
        /// </summary>
        protected Command()
        {
            if (Application.Current != null)
            {
                _Dispatcher = Application.Current.Dispatcher;
            }
            else
            {
                _Dispatcher = Dispatcher.CurrentDispatcher;
            }
        }

        #endregion

        #region ICommand Members

        /// <summary>
        ///     Occurs whenever the state of the application changes such that the result of a call to <see cref="CanExecute" />
        ///     may return a different value.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        ///     Determines whether this command can execute.
        /// </summary>
        /// <param name="parameter">
        ///     The command parameter.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the command can execute, otherwise <see langword="false" />.
        /// </returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        ///     Executes this command.
        /// </summary>
        /// <param name="parameter">
        ///     The command parameter.
        /// </param>
        public abstract void Execute(object parameter);

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            if (!_Dispatcher.CheckAccess())
            {
                _Dispatcher.Invoke((ThreadStart) OnCanExecuteChanged, DispatcherPriority.Normal);
            }
            else
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion
    }
}