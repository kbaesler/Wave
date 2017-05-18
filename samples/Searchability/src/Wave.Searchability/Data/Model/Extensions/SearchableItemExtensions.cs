using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Wave.Searchability.Data
{
    public static class SearchableItemExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates the searchable table using the table
        /// </summary>
        /// <param name="source">The layer.</param>
        /// <returns></returns>
        public static SearchableItem AsSearchableItem(this ITable source)
        {
            var ds = (IDataset) source;
            var item = new SearchableTable(ds.Name, ds.Name)
            {
                ItemType = SearchableItemType.Table
            };

            foreach (var field in source.Fields.AsEnumerable())
                item.Fields.Add(new SearchableField(field.Name) {AliasName = field.AliasName});

            return item;
        }

        /// <summary>
        ///     Creates the searchable layer using the layer
        /// </summary>
        /// <param name="source">The layer.</param>
        /// <returns></returns>
        public static SearchableItem AsSearchableItem(this IFeatureLayer source)
        {
            var item = new SearchableLayer(source.Name, source.FeatureClass.AliasName)
            {
                LayerDefinition = !string.IsNullOrEmpty(((IFeatureLayerDefinition) source).DefinitionExpression),
                ItemType = source.FeatureClass.GetSearchableType()
            };

            foreach (var field in source.FeatureClass.Fields.AsEnumerable())
                item.Fields.Add(new SearchableField(field.Name) {AliasName = field.AliasName});

            return item;
        }

        /// <summary>
        ///     Gets the type of the inventory.
        /// </summary>
        /// <param name="featureClass">The feature class.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableItemType" /> representing the type for the geometry.
        /// </returns>
        public static SearchableItemType GetSearchableType(this IFeatureClass featureClass)
        {
            var annoClass = featureClass.Extension is IAnnotationClassExtension;
            if (annoClass) return SearchableItemType.Annotation;

            var dimClass = featureClass.Extension is IDimensionClassExtension;
            if (dimClass) return SearchableItemType.Dimension;

            switch (featureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryLine:
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryPath:
                    return SearchableItemType.Line;

                case esriGeometryType.esriGeometryMultipoint:
                case esriGeometryType.esriGeometryPoint:
                    return SearchableItemType.Point;

                case esriGeometryType.esriGeometryPolygon:
                    return SearchableItemType.Polygon;
            }

            return SearchableItemType.Unknown;
        }

        #endregion
    }
}