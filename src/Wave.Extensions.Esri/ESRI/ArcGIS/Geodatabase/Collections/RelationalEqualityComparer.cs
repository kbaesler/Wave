using System.Collections.Generic;

using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Compares two <see cref="IGeometry" /> objects based on the <see cref="IRelationalOperator" /> equality.
    /// </summary>
    public class RelationalEqualityComparer : IEqualityComparer<IGeometry>
    {
        #region IEqualityComparer<IGeometry> Members

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="IGeometry"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="IGeometry"/> to compare.</param>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IGeometry x, IGeometry y)
        {
            IRelationalOperator xc = x as IRelationalOperator;
            IRelationalOperator yc = y as IRelationalOperator;

            return (xc != null && yc != null) && xc.Equals(yc);
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