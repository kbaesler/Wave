using System.Collections.Generic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Compares two <see cref="IRow" /> objects based on name and object id.
    /// </summary>
    public class RowEqualityComparer : IEqualityComparer<IRow>
    {
        #region IEqualityComparer<IRow> Members

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="IRow" /> to compare.</param>
        /// <param name="y">The second object of type <see cref="IRow" /> to compare.</param>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IRow x, IRow y)
        {
            return (((IDataset) x.Table).Name == ((IDataset) y.Table).Name && x.OID == y.OID);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(IRow obj)
        {
            return new {A = ((IDataset) obj.Table).Name, B = obj.OID}.GetHashCode();
        }

        #endregion
    }
}