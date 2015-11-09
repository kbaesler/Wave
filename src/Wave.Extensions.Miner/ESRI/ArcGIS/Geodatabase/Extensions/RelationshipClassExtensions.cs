using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> interface.
    /// </summary>
    public static class RelationshipClassExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates a new relationship between the two specified objects.
        /// </summary>
        /// <param name="source">The relationship class that participates in a many to many relationship.</param>
        /// <param name="originObject">The origin object.</param>
        /// <param name="destinationObject">The destination object.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>
        ///     Returns a <see cref="IRelationship" /> representing the relationship between the two objects.
        /// </returns>
        public static IRelationship CreateRelationship(this IRelationshipClass source, IObject originObject, IObject destinationObject, mmAutoUpdaterMode mode)
        {
            if (source == null) return null;

            using (new AutoUpdaterModeReverter(mode))
            {
                return source.CreateRelationship(originObject, destinationObject);
            }
        }

        #endregion
    }
}