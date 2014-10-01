using System;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An abstract exception used to indiciate missing model names.
    /// </summary>
    public abstract class MissingModelNameException : Exception
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissingModelNameException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="modelNames">The model names.</param>
        protected MissingModelNameException(string message, params string[] modelNames)
            : base(message)
        {
            this.ModelNames = modelNames;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the model names.
        /// </summary>
        /// <value>
        ///     The model names.
        /// </value>
        public string[] ModelNames { get; private set; }

        #endregion
    }
}