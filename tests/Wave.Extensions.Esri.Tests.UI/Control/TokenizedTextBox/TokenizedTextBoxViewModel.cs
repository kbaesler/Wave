using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace Wave.Extensions.Esri.Tests.UI.Control.TokenizedTextBox
{
    internal class TokenizedTextBoxViewModel : BaseViewModel
    {
        #region Fields

        private string _Text;
        private List<string> _Items;
        private List<Tag> _Tags;
 
        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenizedTextBoxViewModel" /> class.
        /// </summary>
        public TokenizedTextBoxViewModel()
        {
            this.Items = new List<string>();
            this.Tags = new List<Tag>();

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
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public List<string> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;

                OnPropertyChanged("Items");
            }
        }

        /// <summary>
        ///     Gets or sets the tags.
        /// </summary>
        /// <value>
        ///     The tags.
        /// </value>
        public List<Tag> Tags
        {
            get { return _Tags; }
            set
            {
                _Tags = value;

                OnPropertyChanged("Tags");
            }
        }
        
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

                OnPropertyChanged("Text");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the tags.
        /// </summary>
        /// <param name="data">The data.</param>
        private void CreateTags(object data)
        {
            var letters = new List<string>();
            var tags = new List<Tag>();

            var text = new StringBuilder();
            char[] chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&".ToCharArray();
            Random r = new Random();

            for (int i = 0; i < r.Next(2, chars.Length - 1); i++)
            {
                int c = r.Next(chars.Length);
                char l = chars[c];

                tags.Add(new Tag()
                {
                    Id = c,
                    Character = l
                });

                letters.Add(l.ToString(CultureInfo.InvariantCulture));
                text.AppendFormat("{0},", l);
            }

            this.Text = text.ToString();
            this.Tags = tags;
            this.Items = letters;
        }

        #endregion
    }

    public class Tag
    {
        #region Public Properties

        public char Character { get; set; }
        public int Id { get; set; }

        #endregion
    }
}