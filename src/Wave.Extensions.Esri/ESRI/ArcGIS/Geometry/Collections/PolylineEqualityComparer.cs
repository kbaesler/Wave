using System;

using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Geometry
{
    /// <summary>
    ///     Compares two <see cref="IGeometry" /> objects based on the <see cref="IClone" /> equality.
    /// </summary>
    public class PolylineEqualityComparer : GeometryEqualityComparer
    {
        #region Fields

        private readonly double _Tolerance;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PolylineEqualityComparer" /> class.
        /// </summary>
        /// <param name="tolerance">
        ///     The tolerance in the distance allowed between the two geometries for the changes in length and
        ///     x/y location for the from and to points.
        /// </param>
        public PolylineEqualityComparer(double tolerance)
        {
            _Tolerance = tolerance;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="IGeometry"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="IGeometry"/> to compare.</param>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public override bool Equals(IGeometry x, IGeometry y)
        {
            if (!base.Equals(x, y))
            {
                if (x is IPolyline && y is IPolyline)
                {
                    IPolyline oldValue = (IPolyline) x;
                    IPolyline newValue = (IPolyline) y;

                    double length = Math.Abs(oldValue.Length - newValue.Length);
                    if (length > _Tolerance) return true;

                    double toX = Math.Abs(oldValue.ToPoint.X - newValue.ToPoint.X);
                    if (toX > _Tolerance) return true;

                    double toY = Math.Abs(oldValue.ToPoint.Y - newValue.ToPoint.Y);
                    if (toY > _Tolerance) return true;

                    double fromX = Math.Abs(oldValue.FromPoint.X - newValue.FromPoint.X);
                    if (fromX > _Tolerance) return true;

                    double fromY = Math.Abs(oldValue.FromPoint.Y - newValue.FromPoint.Y);
                    if (fromY > _Tolerance) return true;
                }

                return false;
            }

            return true;
        }
        
        #endregion
    }
}