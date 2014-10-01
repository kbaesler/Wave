namespace ESRI.ArcGIS.Geodatabase.Internal
{

    #region Enumerations

    /// <summary>
    ///     Enumeration of class (object) types.
    /// </summary>
    internal enum ClassType
    {
        /// <summary>
        ///     Annotation Feature Class
        /// </summary>
        Annotation,

        /// <summary>
        ///     Line Feature Class
        /// </summary>
        Line,

        /// <summary>
        ///     Relationship Class
        /// </summary>
        Relationship,

        /// <summary>
        ///     Point Feature Class
        /// </summary>
        Point,

        /// <summary>
        ///     General Table.
        /// </summary>
        Table
    }

    #endregion

    /// <summary>
    ///     A lightweight class used to categorize the conflict class.
    /// </summary>
    internal class ConflictClass
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConflictClass" /> class.
        /// </summary>
        /// <param name="conflictClass">The conflict class.</param>
        public ConflictClass(IConflictClass conflictClass)
        {
            this.Class = conflictClass;
            this.Type = ClassType.Table;

            IFeatureClass oclass = conflictClass as IFeatureClass;
            if ((oclass != null))
            {
                this.IsFeatureClass = true;
                this.IsNetworkClass = (conflictClass is INetworkClass);

                switch (oclass.FeatureType)
                {
                    case esriFeatureType.esriFTAnnotation:
                    case esriFeatureType.esriFTCoverageAnnotation:

                        this.Type = ClassType.Annotation;
                        break;

                    case esriFeatureType.esriFTSimpleEdge:
                    case esriFeatureType.esriFTComplexEdge:
                        this.Type = ClassType.Line;
                        break;

                    case esriFeatureType.esriFTComplexJunction:
                    case esriFeatureType.esriFTSimpleJunction:
                        this.Type = ClassType.Point;
                        break;
                }
            }
            else if (conflictClass is IRelationshipClass)
            {
                this.Type = ClassType.Relationship;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the class.
        /// </summary>
        public IConflictClass Class { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is a feature class.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is a feature class; otherwise, <c>false</c>.
        /// </value>
        public bool IsFeatureClass { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is a network feature class.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is a network feature class; otherwise, <c>false</c>.
        /// </value>
        public bool IsNetworkClass { get; private set; }

        /// <summary>
        ///     Gets or sets the type of the class.
        /// </summary>
        /// <value>The type of the class.</value>
        public ClassType Type { get; private set; }

        #endregion
    }
}