using System.Drawing;

using log4net.Core;
using log4net.Util;

namespace System.Diagnostics.Appenders
{
    /// <summary>
    ///     Provides the ability to control the style of the level.
    /// </summary>
    public class TextStyleLevel : LevelMappingEntry, IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextStyleLevel" /> class.
        /// </summary>
        /// <param name="level">The level.</param>
        public TextStyleLevel(Level level)
        {
            this.Level = level;
            this.FontStyle = FontStyle.Regular;
            this.SizeInPoints = 10.0f;
            this.TextColorName = SystemColors.WindowText.Name;
            this.BackColorName = SystemColors.Window.Name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the back color.
        /// </summary>
        /// <value>
        ///     The name of the back color.
        /// </value>
        public string BackColorName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="TextStyleLevel" /> is bold.
        /// </summary>
        /// <value>
        ///     <c>true</c> if bold; otherwise, <c>false</c>.
        /// </value>
        public bool Bold { get; set; }

        /// <summary>
        ///     Gets or sets the name of the font family.
        /// </summary>
        /// <value>
        ///     The name of the font family.
        /// </value>
        public string FontFamilyName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="TextStyleLevel" /> is italic.
        /// </summary>
        /// <value>
        ///     <c>true</c> if italic; otherwise, <c>false</c>.
        /// </value>
        public bool Italic { get; set; }

        /// <summary>
        ///     Gets or sets the size of the point.
        /// </summary>
        /// <value>
        ///     The size of the point.
        /// </value>
        public float SizeInPoints { get; set; }

        /// <summary>
        ///     Gets or sets the name of the text color.
        /// </summary>
        /// <value>
        ///     The name of the text color.
        /// </value>
        public string TextColorName { get; set; }

        #endregion

        #region Internal Properties

        /// <summary>
        ///     Gets or sets the color of the back.
        /// </summary>
        /// <value>
        ///     The color of the back.
        /// </value>
        internal Color BackColor { get; set; }

        /// <summary>
        ///     Gets or sets the font.
        /// </summary>
        /// <value>
        ///     The font.
        /// </value>
        internal Font Font { get; set; }

        /// <summary>
        ///     Gets or sets the font style.
        /// </summary>
        /// <value>
        ///     The font style.
        /// </value>
        internal FontStyle FontStyle { get; set; }

        /// <summary>
        ///     Gets or sets the color of the text.
        /// </summary>
        /// <value>
        ///     The color of the text.
        /// </value>
        internal Color TextColor { get; set; }

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

        #region Public Methods

        /// <summary>
        ///     Initialize the options for the object
        /// </summary>
        /// <remarks>Parse the properties</remarks>
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            if (this.Bold) this.FontStyle |= FontStyle.Bold;
            if (this.Italic) this.FontStyle |= FontStyle.Italic;

            if (this.FontFamilyName != null)
            {
                float size = this.SizeInPoints > 0.0f ? this.SizeInPoints : 8.25f;

                try
                {
                    this.Font = new Font(this.FontFamilyName, size, this.FontStyle);
                }
                catch (ArgumentException)
                {
                    this.Font = new Font("Arial", 8.25f, FontStyle.Regular);
                }
            }

            this.BackColor = Color.FromName(this.BackColorName);
            this.TextColor = Color.FromName(this.TextColorName);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Font != null)
                {
                    this.Font.Dispose();
                    this.Font = null;
                }
            }
        }

        #endregion
    }
}