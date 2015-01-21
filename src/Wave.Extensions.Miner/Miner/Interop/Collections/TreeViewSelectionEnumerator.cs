using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop
{
    /// <summary>
    ///     An enumerator for the <see cref="IMMTreeViewSelection" /> interface.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TreeViewSelectionEnumerator : IMMTreeViewSelection, IDisposable
    {
        #region Fields

        private readonly int _Count;
        private readonly IEnumerator<IFeature> _Enumerator;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeViewSelectionEnumerator" /> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public TreeViewSelectionEnumerator(IEnumerable<IFeature> features)
        {
            var list = new List<IFeature>(features);

            _Enumerator = list.GetEnumerator();
            _Count = list.Count;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeViewSelectionEnumerator" /> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public TreeViewSelectionEnumerator(params IFeature[] features)
        {
            var list = new List<IFeature>(features);

            _Enumerator = list.GetEnumerator();
            _Count = list.Count;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region IMMTreeViewSelection Members

        /// <summary>
        ///     Gets a value indicating whether this <see cref="TreeViewSelectionEnumerator" /> is EOF.
        /// </summary>
        /// <value>
        ///     <c>true</c> if EOF; otherwise, <c>false</c>.
        /// </value>
        public bool EOF
        {
            get { return _Enumerator.Current == null; }
        }

        /// <summary>
        ///     Gets the next.
        /// </summary>
        public ID8ListItem Next
        {
            get
            {
                if (_Enumerator.MoveNext())
                {
                    IFeature current = _Enumerator.Current;
                    if (current == null) return null;

                    string tableName = ((IDataset) current.Table).Name;
                    IWorkspace workspace = ((IDataset) current.Table).Workspace;

                    ID8GeoAssoc geoAssoc = new D8FeatureClass();
                    geoAssoc.AssociatedGeoRow = current;
                    geoAssoc.OID = current.OID;
                    geoAssoc.TableName = tableName;
                    geoAssoc.Workspace = workspace;

                    return (ID8ListItem)geoAssoc;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the number of elements actually contained in the <see cref="T:System.Collections.Generic.List`1" />.
        /// </summary>
        /// <returns>
        ///     The number of elements actually contained in the <see cref="T:System.Collections.Generic.List`1" />.
        /// </returns>
        public int Count
        {
            get { return _Count; }
        }

        /// <summary>
        ///     Sets the enumerator to its initial position, which is before the first element
        ///     in the collection.
        /// </summary>
        public void Reset()
        {
            _Enumerator.Reset();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Enumerator.Dispose();
            }
        }

        #endregion
    }
}