using System.Collections.Generic;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Compares two <see cref="IGeometry" /> objects based on the <see cref="IClone"/> equality.
    /// </summary>
    public class GeometryEqualityComparer : IEqualityComparer<IGeometry>
    {
        #region IEqualityComparer<IGeometry> Members


        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="IGeometry"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="IGeometry"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public virtual bool Equals(IGeometry x, IGeometry y)
        {
            IClone xc = x as IClone;
            IClone yc = y as IClone;

            return (xc != null && yc != null) && xc.IsEqual(yc);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(IGeometry obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}