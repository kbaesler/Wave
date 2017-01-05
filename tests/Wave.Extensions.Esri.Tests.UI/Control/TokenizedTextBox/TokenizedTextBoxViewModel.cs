using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Wave.Extensions.Esri.Tests.UI.Control.TokenizedTextBox
{
    internal class TokenizedTextBoxViewModel : BaseViewModel
    {
        #region Fields

        private string _Text;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenizedTextBoxViewModel" /> class.
        /// </summary>
        public TokenizedTextBoxViewModel()
        {
            this.CreateTagsCommand = new DelegateCommand(this.CreateTags);
            this.CreateTags(null);
        }

        #endregion

        #region Public Properties        
        /// <summary>
        ///     Gets or sets the create tags command.
        /// </summary>
        /// <value>
        ///     The create tags command.
        /// </value>
        public DelegateCommand CreateTagsCommand { get; set; }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;

                base.OnPropertyChanged("Text");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the tags.
        /// </summary>
        /// <param name="data">The data.</param>
        private void CreateTags(object data)
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < random.Next(2, 20); i++)
            {
                sb.Append(random.Next(i, i+1*2));
                sb.Append(",");
            }

            this.Text = sb.ToString();
        }

        #endregion
    }
}