using System.Windows;

using Wave.Extensions.Esri.Tests.UI.Control.TokenizedTextBox;
using Wave.Extensions.Esri.Tests.UI.Control.Watermark;

namespace Wave.Extensions.Esri.Tests.UI
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.TokenizedTextBoxCommand = new DelegateCommand((o) => this.ShowWindow(new TokenizedTextBoxView()));
            this.WatermarksCommand = new DelegateCommand((o) => this.ShowWindow(new WatermarkView()));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the tokenized text box command.
        /// </summary>
        /// <value>
        ///     The tokenized text box command.
        /// </value>
        public DelegateCommand TokenizedTextBoxCommand { get; private set; }

        /// <summary>
        /// Gets the watermarks command.
        /// </summary>
        /// <value>
        /// The watermarks command.
        /// </value>
        public DelegateCommand WatermarksCommand { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Shows the tokenized text box window.
        /// </summary>
        private void ShowWindow(Window window)
        {
            window.ShowDialog();
        }

        #endregion
    }
}