using System.Collections.Generic;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     An equality comparer that compares <see cref="IUID" /> interfaces.
    /// </summary>
    public class UIDEqualityComparer : IEqualityComparer<IUID>
    {
        #region IEqualityComparer<IUID> Members

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(IUID obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="x" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="y" /> to compare.</param>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public virtual bool Equals(IUID x, IUID y)
        {
            return Equals(x.Value, y.Value);
        }

        #endregion
    }
}