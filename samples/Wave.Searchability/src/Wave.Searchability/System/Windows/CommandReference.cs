using System.Windows.Input;

namespace System.Windows
{
    /// <summary>
    ///     This class facilitates associating a key binding in XAML markup to a command
    ///     defined in a View Model by exposing a Command dependency property.
    ///     The class derives from Freezable to work around a limitation in WPF when data-binding from XAML.
    /// </summary>
    public class CommandReference : Freezable, ICommand
    {
        #region Fields

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand), typeof (CommandReference), new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));

        #endregion

        #region Constructors

        public CommandReference()
        {
            // Blank
        }

        #endregion

        #region Public Properties

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (Command != null)
                return Command.CanExecute(parameter);
            return false;
        }

        public void Execute(object parameter)
        {
            Command.Execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Protected Methods

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandReference commandReference = d as CommandReference;
            ICommand oldCommand = e.OldValue as ICommand;
            ICommand newCommand = e.NewValue as ICommand;

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= commandReference.CanExecuteChanged;
            }
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += commandReference.CanExecuteChanged;
            }
        }

        #endregion
    }
}