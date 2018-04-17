using System;
using System.Collections.Generic;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Carto
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IEnumLayer" /> enumeration.
    /// </summary>
    public static class LayerExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Sets a join based on the specified relationship class and join type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="table">The table.</param>
        /// <param name="foreignClass">The join.</param>
        /// <param name="layerKeyField">Name of the layer field.</param>
        /// <param name="foreignKeyField">Name of the join field.</param>
        /// <param name="cardinality">The cardinality.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <returns>
        /// Returns a <see cref="IRelQueryTable" /> represents the table and feature join.
        /// </returns>
        public static IRelQueryTable Add(this IFeatureLayer source, IRelQueryTable table, IObjectClass foreignClass, string layerKeyField, string foreignKeyField, esriRelCardinality cardinality, esriJoinType joinType)
        {
            if (source != null)
            {
                var factory = new RelQueryTableFactoryClass();
                var origin = factory.Open(table.RelationshipClass, true, null, null, "", true, ((IRelQueryTableInfo)table).JoinType == esriJoinType.esriLeftInnerJoin);

                var join = ((IObjectClass)origin).Join(foreignClass, string.Format("{0}.{1}", ((IDataset)table.SourceTable).Name, layerKeyField), foreignKeyField, cardinality, null);
                var result = factory.Open(join, true, null, null, "", true, joinType == esriJoinType.esriLeftInnerJoin);

                IDisplayRelationshipClass display = (IDisplayRelationshipClass)source;
                display.DisplayRelationshipClass(result.RelationshipClass, joinType);

                return result;
            }

            return null;
        }

        /// <summary>
        /// Sets a join based on the specified relationship class and join type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="foreignClass">The join.</param>
        /// <param name="layerKeyField">Name of the layer field.</param>
        /// <param name="foreignKeyField">Name of the join field.</param>
        /// <param name="cardinality">The cardinality.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <returns>
        /// Returns a <see cref="IRelQueryTable" /> represents the table and feature join.
        /// </returns>
        public static IRelQueryTable Add(this IFeatureLayer source, IObjectClass foreignClass, string layerKeyField, string foreignKeyField, esriRelCardinality cardinality, esriJoinType joinType)
        {
            IRelQueryTable result;
            IRelationshipClass relClass;
            var table = ((IDisplayTable)source).DisplayTable as IRelQueryTable;
            if (table != null)
            {
                result = source.Add(table, foreignClass, string.Format("{0}.{1}", ((IDataset)source.FeatureClass).Name, layerKeyField), foreignKeyField, cardinality, joinType);
                relClass = result.RelationshipClass;
            }
            else
            {
                var layer = (IFeatureClass)((IDisplayTable)source).DisplayTable;
                relClass = layer.Join(foreignClass, layerKeyField, foreignKeyField, cardinality, null);

                var factory = new RelQueryTableFactoryClass();
                result = factory.Open(relClass, true, null, null, "", true, joinType == esriJoinType.esriLeftInnerJoin);
            }

            IDisplayRelationshipClass display = (IDisplayRelationshipClass)source;
            display.DisplayRelationshipClass(relClass, joinType);

            return result;
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IElement" />
        /// </summary>
        /// <param name="source">An <see cref="IGroupElement" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the layers from the input source.</returns>
        public static IEnumerable<IElement> AsEnumerable(this IGroupElement source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.ElementCount; i++)
                {
                    yield return source.Element[i];
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IElement" />
        /// </summary>
        /// <param name="source">An <see cref="IGraphicsContainer" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the layers from the input source.</returns>
        public static IEnumerable<IElement> AsEnumerable(this IGraphicsContainer source)
        {
            if (source != null)
            {
                source.Reset();
                IElement element = source.Next();
                while (element != null)
                {
                    yield return element;
                    element = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ILayer" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumLayer" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the layers from the input source.</returns>
        public static IEnumerable<ILayer> AsEnumerable(this IEnumLayer source)
        {
            if (source != null)
            {
                source.Reset();
                ILayer layer = source.Next();
                while (layer != null)
                {
                    yield return layer;
                    layer = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ILayer" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumLayer" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the layers from the input source.
        /// </returns>
        public static IEnumerable<ILayer> AsEnumerable(this ICompositeLayer source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                yield return source.Layer[i];
            }
        }


        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IElement" />
        /// </summary>
        /// <param name="source">An <see cref="IElementCollection" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the layers from the input source.</returns>
        public static IEnumerable<KeyValuePair<IElement, int>> AsEnumerable(this IElementCollection source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Count - 1; i++)
                {
                    IElement element;
                    int linkedFeatureID;

                    source.QueryItem(i, out element, out linkedFeatureID);

                    yield return new KeyValuePair<IElement, int>(element, linkedFeatureID);
                }
            }
        }

        /// <summary>
        ///     Creates an enumeration of the <see cref="IRelQueryTable" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{IRelQueryTable}" /> representing the collection of tables.</returns>
        public static IEnumerable<IRelQueryTable> AsEnumerable(this IRelQueryTable source)
        {
            IRelQueryTable table = source;
            while (table != null)
            {
                yield return table;

                table = table.SourceTable as IRelQueryTable;
            }
        }

        /// <summary>
        ///     Gets the hierarchy of the layer and sibilings.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IHierarchy{ILayer}" /> representing the hierarchical structure of the layer.
        /// </returns>
        public static IHierarchy<ILayer> GetHierarchy(this ILayer source)
        {
            return GetHierarchyImpl(source, 0);
        }

        /// <summary>
        ///     Performs an identify operation with the provided geometry.
        ///     When identifying layers, typically a small envelope is passed in rather than a point to account for differences in
        ///     the precision of the display and the feature geometry.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="geometry">The geometry.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IFeatureIdentifyObj}" /> representing the features that have been identified.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">geometry</exception>
        /// <remarks>
        ///     On a FeatureIdentifyObject, you can do a QI to the IIdentifyObj interface to get more information about the
        ///     identified feature. The IIdentifyObj interface returns the window handle, layer, and name of the feature; it has
        ///     methods to flash the
        ///     feature in the display and to display a context menu at the Identify location.
        /// </remarks>
        public static IEnumerable<IFeatureIdentifyObj> Identify(this IFeatureLayer source, IGeometry geometry)
        {
            return source.Identify(geometry, null);
        }

        /// <summary>
        ///     Performs an identify operation with the provided geometry.
        ///     When identifying layers, typically a small envelope is passed in rather than a point to account for differences in
        ///     the precision of the display and the feature geometry.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="trackCancel">
        ///     The cancel tracker object to monitor the Esc key and to terminate processes at the request of
        ///     the user.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IFeatureIdentifyObj}" /> representing the features that have been identified.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">geometry</exception>
        /// <remarks>
        ///     On a FeatureIdentifyObject, you can do a QI to the IIdentifyObj interface to get more information about the
        ///     identified feature. The IIdentifyObj interface returns the window handle, layer, and name of the feature; it has
        ///     methods to flash the
        ///     feature in the display and to display a context menu at the Identify location.
        /// </remarks>
        public static IEnumerable<IFeatureIdentifyObj> Identify(this IFeatureLayer source, IGeometry geometry, ITrackCancel trackCancel)
        {
            if (geometry == null) throw new ArgumentNullException("geometry");

            if (source != null)
            {
                IIdentify2 identify = (IIdentify2)source;
                IArray array = identify.Identify(geometry, trackCancel);
                if (array != null)
                {
                    for (int i = 0; i < array.Count; i++)
                    {
                        if (trackCancel != null && !trackCancel.Continue())
                            break;

                        IFeatureIdentifyObj featureIdentify = array.Element[i] as IFeatureIdentifyObj;
                        yield return featureIdentify;
                    }
                }
            }
        }

        /// <summary>
        ///     Removes the relate to the specified foreign class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="foreignClass">The foreign class.</param>
        /// <param name="role">The role.</param>
        public static void Remove(this IFeatureLayer source, IObjectClass foreignClass, esriRelRole role)
        {
            IRelationshipClassCollectionEdit edit = (IRelationshipClassCollectionEdit)source;
            IRelationshipClassCollection collection = (IRelationshipClassCollection)edit;
            foreach (var relClass in collection.FindRelationshipClasses(foreignClass, role).AsEnumerable())
            {
                edit.RemoveRelationshipClass(relClass);
            }
        }

        /// <summary>
        ///     Removes all of the joins on the feature layer.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void RemoveAll(this IFeatureLayer source)
        {
            IDisplayRelationshipClass display = (IDisplayRelationshipClass)source;
            display.DisplayRelationshipClass(null, esriJoinType.esriLeftInnerJoin);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the hierarchy of the layer and sibilings.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>
        ///     Returns a <see cref="IHierarchy{ILayer}" /> representing the hierarchical structure of the layer.
        /// </returns>
        private static IHierarchy<ILayer> GetHierarchyImpl(ILayer source, int depth)
        {
            var children = new List<IHierarchy<ILayer>>();

            var node = new Hierarchy<ILayer>();
            node.Value = source;
            node.Children = children;
            node.Depth = depth++;

            var layer = source as ICompositeLayer;
            if (layer != null)
            {
                for (int i = 0; i < layer.Count; i++)
                {
                    var child = new Hierarchy<ILayer>();
                    child.Value = layer.Layer[i];
                    child.Parent = source;
                    child.Depth = depth;

                    children.Add(child);

                    var siblings = new List<IHierarchy<ILayer>>();
                    siblings.Add(GetHierarchyImpl(child.Value, depth));

                    child.Children = siblings;
                }
            }

            return node;
        }

        #endregion
    }
}