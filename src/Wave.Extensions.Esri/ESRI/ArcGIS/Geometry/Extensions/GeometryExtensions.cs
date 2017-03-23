using System.Collections.Generic;

namespace ESRI.ArcGIS.Geometry
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGeometry" /> interface.
    /// </summary>
    public static class GeometryExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IGeometryCollection" />
        /// </summary>
        /// <param name="source">An <see cref="IGeometryCollection" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the geometries from the input source.</returns>
        public static IEnumerable<IGeometry> AsEnumerable(this IGeometryCollection source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.GeometryCount; i++)
                {
                    yield return source.Geometry[i];
                }
            }
        }

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
            if (source == null) return null;

            ITopologicalOperator topOp = (ITopologicalOperator) source;
            return topOp.Buffer(distance);
        }


        /// <summary>
        ///     Gets the units.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IUnit" /> representing the units.</returns>
        public static IUnit GetUnits(this ISpatialReference source)
        {
            var pcs = source as IProjectedCoordinateSystem;
            if (pcs != null) return pcs.CoordinateUnit;

            var gcs = source as IGeographicCoordinateSystem;
            if (gcs != null) return gcs.CoordinateUnit;

            return null;
        }

        #endregion
    }
}