using System;
using System.Collections.Generic;
using System.Windows;

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
        ///     Gets or sets the tags.
        /// </summary>
        /// <value>
        ///     The tags.
        /// </value>
        public List<Tag> Tags { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the tags.
        /// </summary>
        /// <param name="data">The data.</param>
        private void CreateTags(object data)
        {
            this.Tags.Clear();

            char[] chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&".ToCharArray();
            Random r = new Random();
            for (int i = 0; i < r.Next(2, 20); i++)
            {
                int c = r.Next(chars.Length);
                char l = chars[c];
                this.Tags.Add(new Tag()
                {
                    Id = c,
                    Character = l
                });
            }
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