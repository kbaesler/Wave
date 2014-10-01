using System.Collections.Generic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Compares two <see cref="IObject" /> objects based on object class id and object id.
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
            return x.Class.ObjectClassID == y.Class.ObjectClassID && x.OID == y.OID;
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
            return new {A = obj.Class.ObjectClassID, B = obj.OID}.GetHashCode();
        }

        #endregion
    }
}