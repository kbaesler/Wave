namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An exception that indicates a field is missing a model name.
    /// </summary>
    public class MissingFieldModelNameException : MissingModelNameException
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissingFieldModelNameException" /> class.
        /// </summary>
        /// <param name="oclass">The object class.</param>
        /// <param name="modelName">Name of the model.</param>
        public MissingFieldModelNameException(IObjectClass oclass, string modelName)
            : base(string.Format("The '{0}' is not assigned the '{1}' field model name.", ((IDataset)oclass).Name, modelName), modelName)
        {
        }

        #endregion
    }
}