using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     The interface for entities that database rows can be mapped to.
    /// </summary>
    public interface IEntity<in TContext>
    {
        #region Public Properties

        /// <summary>
        ///     The ObjectID field.
        /// </summary>
        int OID { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Binds the entity to specified object from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="oid">The OBJECTID of the row to bind.</param>
        void Bind(TContext context, int oid);

        /// <summary>
        ///     Binds the entity to specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        void Bind(IRow row);

        /// <summary>
        ///     Deletes this item from the context.
        /// </summary>
        void Delete();

        /// <summary>
        ///     Inserts the entity into specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Returns a <see cref="int" /> representing the OID.</returns>
        int Insert(TContext context);

        /// <summary>
        ///     Commits changes to the context.
        /// </summary>
        void Update();

        #endregion
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <typeparam name="TGeometry">The type of the geometry.</typeparam>
    public interface IEntity<in TContext, out TGeometry> : IEntity<IFeatureClass>
        where TGeometry : IGeometry
    {
        #region Public Properties

        /// <summary>
        ///     The geometry of this item.
        /// </summary>
        /// <value>
        ///     The shape.
        /// </value>
        TGeometry Shape { get; }

        #endregion
    }
}