using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    ///     A container for the <see cref="RoutedCommand" /> commands.
    /// </summary>
    public static class TokenizedTextBoxCommands
    {
        #region Fields

        /// <summary>
        ///     The delete command
        /// </summary>
        private static readonly RoutedCommand DeleteCommand = new RoutedCommand();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the delete.
        /// </summary>
        /// <value>
        ///     The delete.
        /// </value>
        public static RoutedCommand Delete
        {
            get { return DeleteCommand; }
        }

        #endregion
    }
}