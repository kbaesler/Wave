using System.Collections.Generic;
using System.Linq;

namespace ESRI.ArcGIS.Geometry
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMSegmentation" /> interface.
    /// </summary>
    public static class SegmentationExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns M values at the distance along the polyline. An array of one or two Ms is returned. Two Ms can be returned
        ///     if the given distance is exactly at the beginning or ending of a part.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="point">The point.</param>
        /// <returns>Returns a <see cref="double" /> array of the m values.</returns>
        /// <remarks>
        ///     Does not assume that the point is on top of the polyline. If the point is equally distant from more that one
        ///     location along the polyline, then returns the all the M values.
        /// </remarks>
        public static double[] GetMsAtPoint(this IPolyline source, IPoint point)
        {
            IMAware aware = (IMAware) source;
            if (aware.MAware)
            {
                double distanceAlongCurve = 0.0;
                double distanceFromCurve = 0.0;
                bool rightSide = false;
                IPoint queryPoint = new PointClass();

                source.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, point, false, queryPoint, ref distanceAlongCurve, ref distanceFromCurve, ref rightSide);

                IMSegmentation segmentation = (IMSegmentation) source;
                var values = segmentation.GetMsAtDistance(distanceAlongCurve, false) as double[];
                return values;
            }

            return null;
        }

        /// <summary>
        ///     Gets the points on the geometry where the first and last M value occurs.
        /// </summary>
        /// <param name="source">The geometry.</param>
        /// <returns>Returns a <see cref="IEnumerable{T}" /> representing the points of M values.</returns>
        public static IEnumerable<IPoint> GetPointsAtMs(this IMSegmentation3 source)
        {
            double firstM, lastM;
            source.QueryFirstLastM(out firstM, out lastM);

            var collection = source.GetPointsAtM(firstM, 0);
            if (collection.GeometryCount == 1)
                yield return (IPoint) collection.Geometry[0];

            collection = source.GetPointsAtM(lastM, 0);
            if (collection.GeometryCount == 1)
                yield return (IPoint) collection.Geometry[0];
        }

        /// <summary>
        /// Returns the points along the polyline using the first and last M values of the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>
        /// Returns a <see cref="IEnumerable{IPoint}" /> representing the points along the other polyline.
        /// </returns>
        public static IEnumerable<IPoint> GetPointsAtMs(this IPolyline source, IPolyline other)
        {
            var points = ((IMSegmentation3)source).GetPointsAtMs();
            IMSegmentation segmentation = (IMSegmentation)other;

            foreach (var point in points)
            {
                var ms = other.GetMsAtPoint(point);
                if (ms != null && ms.Any())
                {
                    foreach (var m in ms)
                    {
                        var collection = segmentation.GetPointsAtM(m, 0);
                        foreach (var geometry in collection.AsEnumerable())
                        {
                            yield return geometry as IPoint;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Returns the points along the other using the first and last M values of the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IPoint}" /> representing the points along the other polyline.
        /// </returns>
        public static IEnumerable<IPoint> GetPointsAtMs(this IMSegmentation3 source, IMSegmentation3 other)
        {
            double firstM, lastM;
            source.QueryFirstLastM(out firstM, out lastM);

            var collection = other.GetPointsAtM(firstM, 0);
            if (collection.GeometryCount == 1)
                yield return (IPoint)collection.Geometry[0];

            collection = other.GetPointsAtM(lastM, 0);
            if (collection.GeometryCount == 1)
                yield return (IPoint)collection.Geometry[0];
        }

        /// <summary>
        ///     Gets polyline geometry corresponding to the subcurve(s) between the fromM and the toM.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fromM">From m.</param>
        /// <param name="toM">To m.</param>
        /// <returns></returns>
        public static IPolyline GetPolylineByMs(this IMSegmentation4 source, double fromM, double toM)
        {
            IMAware aware = source as IMAware;
            if (aware != null && aware.MAware)
            {
                var collection = source.GetSubcurveBetweenMs(fromM, toM);
                return collection as IPolyline;
            }

            return null;
        }

        /// <summary>
        ///     Gets polyline geometry corresponding to the subcurve(s) between the fromM and the toM.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fromM">From measurement.</param>
        /// <param name="toM">To measurement.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public static IPolyline GetPolylineByMs(this IMSegmentation4 source, double fromM, double toM, double offset)
        {
            var polyline = source.GetPolylineByMs(fromM, toM);

            var curve = new PolylineClass();
            curve.ConstructOffset(polyline, offset);

            return curve;
        }

        #endregion
    }
}