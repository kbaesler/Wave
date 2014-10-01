namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An exception that indicates a table is missing a model name.
    /// </summary>
    public class MissingClassModelNameException : MissingModelNameException
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissingClassModelNameException" /> class.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        public MissingClassModelNameException(string modelName)
            : base(string.Format("The '{0}' class model name is not assgined.", modelName), modelName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissingClassModelNameException" /> class.
        /// </summary>
        /// <param name="oclass">The object class.</param>
        /// <param name="modelName">Name of the model.</param>
        public MissingClassModelNameException(IObjectClass oclass, string modelName)
            : base(string.Format("The '{0}' is not assigned the '{1}' class model name.", ((IDataset)oclass).Name, modelName), modelName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissingClassModelNameException" /> class.
        /// </summary>
        /// <param name="oclass">The object class.</param>
        /// <param name="modelNames">The model names.</param>
        public MissingClassModelNameException(IObjectClass oclass, params string[] modelNames)
            : base(string.Format("The '{0}' is not assigned the '{1}' class model name.", ((IDataset)oclass).Name, string.Join(" or ", modelNames)), modelNames)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissingClassModelNameException" /> class.
        /// </summary>
        /// <param name="oclass">The oclass.</param>
        /// <param name="relationshipRole">The relationship role.</param>
        /// <param name="modelNames">The model names.</param>
        public MissingClassModelNameException(IObjectClass oclass, esriRelRole relationshipRole, params string[] modelNames)
            : base(string.Format("There are no '{0}' relationships with the '{1}' that are assigned the '{2}' class model name.", relationshipRole, ((IDataset)oclass).Name, string.Join(" or ", modelNames)), modelNames)
        {
        }

        #endregion
    }
}