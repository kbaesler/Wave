namespace ESRI.ArcGIS.Geometry
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGeometry" /> interface.
    /// </summary>
    public static class GeometryExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Constructs a polygon that is the locus of points at a distance less than or equal to a specified distance from this
        ///     geometry.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="distance">The distance.</param>
        /// <returns>
        ///     Returns a <see cref="IGeometry" /> representing the buffered shape.
        /// </returns>
        /// <remarks>
        ///     The buffer distance is in the same units as the source shape that is being buffered.
        ///     A negative distance can be specified to produce a buffer inside the original polygon.
        ///     This cannot be used with polyline and must be applied on top-level geometries only.
        ///     Top-Level geometries are point, multipoint, polyline and polygon.
        /// </remarks>
        public static IGeometry Buffer(this IGeometry source, double distance)
        {
            ITopologicalOperator topOp = (ITopologicalOperator) source;
            return topOp.Buffer(distance);
        }

        #endregion
    }
}