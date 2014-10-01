namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An exception that indicates a database is missing a model name.
    /// </summary>
    public class MissingDatabaseModelNameException : MissingModelNameException
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissingDatabaseModelNameException" /> class.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        public MissingDatabaseModelNameException(string modelName)
            : base(string.Format("The '{0}' database model name is not assgined.", modelName), modelName)
        {
        }

        #endregion
    }
}