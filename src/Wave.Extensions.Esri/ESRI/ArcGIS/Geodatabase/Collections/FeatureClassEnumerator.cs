using System.Collections.Generic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    /// An iterator used to enumerate through a collection of <see cref="IFeatureClass"/> interfaces.
    /// </summary>
    public class FeatureClassEnumerator : IEnumFeatureClass
    {
        #region Fields

        private readonly IEnumerator<IFeatureClass> _Enumerator;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FeatureClassEnumerator" /> class.
        /// </summary>
        /// <param name="list">The feature classes.</param>
        public FeatureClassEnumerator(IEnumerable<IFeatureClass> list)
        {
            _Enumerator = new List<IFeatureClass>(list).GetEnumerator();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FeatureClassEnumerator" /> class.
        /// </summary>
        /// <param name="list">The feature classes.</param>
        public FeatureClassEnumerator(params IFeatureClass[] list)
        {
            _Enumerator = new List<IFeatureClass>(list).GetEnumerator();
        }

        #endregion

        #region IEnumFeatureClass Members

        /// <summary>
        ///    Sets the enumerator to its initial position, which is before the first element
        ///    in the collection.
        /// </summary>
        public void Reset()
        {
            _Enumerator.Reset();
        }

        /// <summary>
        ///     Gets the element in the collection at the next position of the enumerator.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the next feature class in the iterator; otherwise <c>null</c>
        /// </returns>
        public IFeatureClass Next()
        {
            return _Enumerator.MoveNext() ? _Enumerator.Current : null;
        }

        #endregion
    }
}