using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop
{
    /// <summary>
    ///     A collection that implements the <see cref="IMMTreeViewSelection" /> interface.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TreeViewCollection : List<IFeature>, IMMTreeViewSelection
    {
        #region Fields

        private int _Position = -1;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeViewCollection" /> class.
        /// </summary>
        public TreeViewCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeViewCollection" /> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public TreeViewCollection(IEnumerable<IFeature> features)
            : base(features)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeViewCollection" /> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public TreeViewCollection(params IFeature[] features)
            : base(features)
        {
        }

        #endregion

        #region IMMTreeViewSelection Members

        /// <summary>
        ///     Gets a value indicating whether this <see cref="TreeViewCollection" /> is EOF.
        /// </summary>
        /// <value>
        ///     <c>true</c> if EOF; otherwise, <c>false</c>.
        /// </value>
        public bool EOF
        {
            get { return (_Position >= this.Count || _Position == -1); }
        }

        /// <summary>
        ///     Gets the next.
        /// </summary>
        public ID8ListItem Next
        {
            get
            {
                _Position++;

                IFeature feature = this.ElementAtOrDefault(_Position);
                if (feature == null) return null;

                string tableName = ((IDataset) feature.Table).Name;
                IWorkspace workspace = ((IDataset) feature.Table).Workspace;

                ID8Feature d8f = new D8FeatureClass();
                ID8GeoAssoc d8ga = (ID8GeoAssoc) d8f;
                d8ga.AssociatedGeoRow = feature;
                d8ga.OID = feature.OID;
                d8ga.TableName = tableName;
                d8ga.Workspace = workspace;

                return (ID8ListItem) d8f;
            }
        }

        /// <summary>
        ///     Gets the number of elements actually contained in the <see cref="T:System.Collections.Generic.List`1" />.
        /// </summary>
        /// <returns>
        ///     The number of elements actually contained in the <see cref="T:System.Collections.Generic.List`1" />.
        /// </returns>
        public new int Count
        {
            get { return base.Count; }
        }

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        public void Reset()
        {
            _Position = -1;
        }

        #endregion
    }
}