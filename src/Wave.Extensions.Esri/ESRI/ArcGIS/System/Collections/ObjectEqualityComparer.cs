using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.System
{
    /// <summary>
    ///     Class used for comparing two <see cref="ESRI.ArcGIS.Geodatabase.IObject" /> objects based on the ObjectClassID and
    ///     OID of the object.
    /// </summary>
    public class ObjectEqualityComparer : IEqualityComparer<IObject>
    {
        #region IEqualityComparer<IObject> Members

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IObject x, IObject y)
        {
            return x.Class.ObjectClassID == y.Class.ObjectClassID &&
                   x.OID == y.OID;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(IObject obj)
        {
            int hCode = obj.Class.ObjectClassID ^ obj.OID;
            return hCode.GetHashCode();
        }

        #endregion
    }
}