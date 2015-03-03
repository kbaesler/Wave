using ESRI.ArcGIS.Geodatabase.Internal;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> interface.
    /// </summary>
    public static class RelationshipClassExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns the many to many relationship (or intermediate table) that resides in between the origin and destination
        ///     classes.
        /// </summary>
        /// <param name="source">The relationship class that participates in a many to many relationship.</param>
        /// <returns>
        ///     Returns a <see cref="IIntermediateRelationship" /> representing the intermediate table (or many to many
        ///     relationship) that resides between
        ///     the many to many relationship.
        /// </returns>
        public static IIntermediateRelationship GetIntermediateRelationship(this IRelationshipClass source)
        {
            if (source == null) return null;

            IIntermediateRelationship o = new IntermediateRelationship(source);
            return o;
        }

        #endregion
    }
}