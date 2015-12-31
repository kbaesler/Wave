using System.Collections.Generic;
using System.IO;

namespace Wave.Searchability.Data.Configuration
{
    public abstract class ConfigurationFile<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFile{T}"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        protected ConfigurationFile(string fileName)
        {
            this.Deseralize(fileName);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the contents.
        /// </summary>
        /// <value>
        /// The contents.
        /// </value>
        public T Contents { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the contents.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        protected abstract T GetContents(StreamReader stream);

        #endregion

        #region Private Methods

        /// <summary>
        /// Deseralizes the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void Deseralize(string fileName)
        {
            using(var stream = new StreamReader(fileName))
                this.Contents = this.GetContents(stream);
        }

        #endregion
    }
}