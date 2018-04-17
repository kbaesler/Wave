using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extensions for the Analysis Geoprocessing Tools.
    /// </summary>
    public static class AnalysisToolsExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Reads a table and a set of fields and creates a new table containing unique field values and the number of
        ///     occurrences of each unique field value
        /// </summary>
        /// <param name="source">
        ///     The table containing the field(s) that will be used to calculate frequency statistics. This table
        ///     can be an INFO or OLE DB table, a dBASE or a VPF table, or a feature class table.
        /// </param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="frequencyFields">The attribute field or fields that will be used to calculate frequency statistics.</param>
        /// <param name="summaryFields">
        ///     The attribute field or fields to sum and add to the output table. Null values are excluded from this calculation.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <returns>
        ///     Return <see cref="ITable" /> representing the table that stores the calculated frequency statistics.
        /// </returns>
        public static ITable Frequency(this IFeatureClass source, string outputTableName, IWorkspace workspace, string[] frequencyFields, string[] summaryFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            Frequency gp = new Frequency();
            gp.in_table = source;
            gp.out_table = workspace.GetAbsolutePath(outputTableName);
            gp.summary_fields = (summaryFields != null) ? string.Join(";", summaryFields) : null;
            gp.frequency_fields = string.Join(";", frequencyFields);

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
                return workspace.GetTable(outputTableName);

            return null;
        }

       
        /// <summary>
        ///     Joins attributes from one feature to another based on the spatial relationship. The target features and the joined
        ///     attributes from the join features are written to the output feature class.
        /// </summary>
        /// <param name="source">
        ///     The source features and the attributes from the joined features are transferred to the output
        ///     feature class. However, a subset of attributes can be defined in the field map parameter.
        /// </param>
        /// <param name="join">The attributes from the join features are joined to the attributes of the source features.</param>
        /// <param name="tableName">A new feature class containing the attributes of the target and join features.</param>
        /// <param name="operation">
        ///     Determines how joins between the target features and join features will be handled in the
        ///     output feature class if multiple join features are found that have the same spatial relationship with a single
        ///     target feature.
        /// </param>
        /// <param name="isOuterJoin">
        ///     if set to <c>true</c> if all target features will be maintained in the output feature class
        ///     (known as outer join), or only those that have the specified spatial relationship with the join features (inner
        ///     join).
        /// </param>
        /// <param name="option">Defines the criteria used to match rows.</param>
        /// <param name="caseFields">The attribute fields will be in the output feature class.</param>
        /// <param name="searchRadius">
        ///     Join features within this distance of a target feature will be considered for the spatial
        ///     join.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>Returns a <see cref="IFeatureClass" /> representing the feature class of the joined data.</returns>
        public static IFeatureClass Join(this IFeatureClass source, IFeatureClass join, string tableName, JoinOperation operation, bool isOuterJoin, SpatialOperation option, string[] caseFields, double searchRadius, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            IGpFieldMappingsObject fieldMappings = new GpFieldMappingsObjectClass();
            fieldMappings.AddTable(source.GetAbsolutePath());
            fieldMappings.AddTable(join.GetAbsolutePath());

            int[] caseIndexes = caseFields.Select(fieldMappings.FindFieldMapIndex).ToArray();

            for (int i = fieldMappings.FieldCount - 1; i >= 0; i--)
            {
                if (caseIndexes.Contains(i)) continue;

                fieldMappings.RemoveFieldMap(i);
            }

            return source.Join(join, tableName, operation, isOuterJoin, option, fieldMappings, searchRadius, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Joins attributes from one feature to another based on the spatial relationship. The target features and the joined
        ///     attributes from the join features are written to the output feature class.
        /// </summary>
        /// <param name="source">
        ///     The source features and the attributes from the joined features are transferred to the output
        ///     feature class. However, a subset of attributes can be defined in the field map parameter.
        /// </param>
        /// <param name="join">The attributes from the join features are joined to the attributes of the source features.</param>
        /// <param name="tableName">A new feature class containing the attributes of the target and join features.</param>
        /// <param name="operation">
        ///     Determines how joins between the target features and join features will be handled in the
        ///     output feature class if multiple join features are found that have the same spatial relationship with a single
        ///     target feature.
        /// </param>
        /// <param name="isOuterJoin">
        ///     if set to <c>true</c> if all target features will be maintained in the output feature class
        ///     (known as outer join), or only those that have the specified spatial relationship with the join features (inner
        ///     join).
        /// </param>
        /// <param name="option">Defines the criteria used to match rows.</param>
        /// <param name="fieldMappings">The attribute fields will be in the output feature class.</param>
        /// <param name="searchRadius">
        ///     Join features within this distance of a target feature will be considered for the spatial
        ///     join.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>Returns a <see cref="IFeatureClass" /> representing the feature class of the joined data.</returns>
        public static IFeatureClass Join(this IFeatureClass source, IFeatureClass join, string tableName, JoinOperation operation, bool isOuterJoin, SpatialOperation option, IGpFieldMappingsObject fieldMappings, double searchRadius, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            var ds = (IDataset) source;

            SpatialJoin gp = new SpatialJoin();
            gp.join_operation = operation.ToDescription();
            gp.join_type = isOuterJoin ? "KEEP_ALL" : "KEEP_COMMON";
            gp.match_option = option.ToDescription();
            gp.field_mapping = fieldMappings;
            gp.search_radius = searchRadius;

            gp.target_features = source;
            gp.join_features = join;
            gp.out_feature_class = ds.Workspace.GetAbsolutePath(tableName);

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
            {
                return ds.Workspace.GetFeatureClass(tableName);
            }

            return null;
        }

        /// <summary>
        ///     Calculates summary statistics for field(s) in a table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="statisticFields">
        ///     The numeric field containing attribute values used to calculate the specified
        ///     statistic by the type of the statistic (SUM, MEAN, MIN, MAX, RANGE, STD, FIRST, LAST, COUNT).
        /// </param>
        /// <param name="caseFields">
        ///     The fields in the Input Table used to calculate statistics separately for each unique
        ///     attribute value (or combination of attributes values when multiple fields are specified).
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <returns>
        ///     Return <see cref="ITable" /> representing the table of statistic data.
        /// </returns>
        public static ITable Statistics(this IFeatureClass source, string outputTableName, IWorkspace workspace, List<KeyValuePair<string, StatisticType>> statisticFields, string[] caseFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            Statistics gp = new Statistics();
            gp.in_table = source;
            gp.out_table = workspace.GetAbsolutePath(outputTableName);

            string x = null;
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in statisticFields)
            {
                sb.Append(x);
                sb.AppendFormat("{0} {1}", kvp.Key, kvp.Value);
                x = ";";
            }

            gp.statistics_fields = sb.ToString().ToUpperInvariant();
            gp.case_field = string.Join(";", caseFields).ToUpperInvariant();

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
                return workspace.GetTable(outputTableName);

            return null;
        }

        /// <summary>
        ///     Calculates summary statistics for field(s) in a table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="statisticField">The numeric field containing attribute values used to calculate the specified statistic.</param>
        /// <param name="statisticType">Type of the statistic (SUM, MEAN, MIN, MAX, RANGE, STD, FIRST, LAST, COUNT).</param>
        /// <param name="caseFields">
        ///     The fields in the Input Table used to calculate statistics separately for each unique
        ///     attribute value (or combination of attributes values when multiple fields are specified).
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <returns>
        ///     Return <see cref="ITable" /> representing the table of statistic data.
        /// </returns>
        public static ITable Statistics(this IFeatureClass source, string outputTableName, IWorkspace workspace, string statisticField, StatisticType statisticType, string[] caseFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            var list = new List<KeyValuePair<string, StatisticType>>();
            list.Add(new KeyValuePair<string, StatisticType>(statisticField, statisticType));

            return source.Statistics(outputTableName, workspace, list, caseFields, trackCancel, eventHandler);
        }

        #endregion
    }

    /// <summary>
    ///     Determines how joins between the target features and join features will be handled in the output feature class if
    ///     multiple join features are found that have the same spatial relationship with a single target feature.
    /// </summary>
    public enum JoinOperation
    {
        /// <summary>
        ///     If multiple join features are found that have the same spatial relationship with a single target feature, the
        ///     attributes from the multiple join features will be aggregated using a field map merge rule. For example, if a point
        ///     target feature is found within two separate polygon join features, the attributes from the two polygons will be
        ///     aggregated before being transferred to the output point feature class. If one polygon has an attribute value of 3
        ///     and the other has a value of 7, and a Sum merge rule is specified, the aggregated value in the output feature class
        ///     will be 10. This is the default.
        /// </summary>
        OneToOne,

        /// <summary>
        ///     If multiple join features are found that have the same spatial relationship with a single target feature, the
        ///     output feature class will contain multiple copies (records) of the target feature. For example, if a single point
        ///     target feature is found within two separate polygon join features, the output feature class will contain two copies
        ///     of the target feature: one record with the attributes of one polygon, and another record with the attributes of the
        ///     other polygon.
        /// </summary>
        OneToMany
    }

    /// <summary>
    ///     Defines the criteria used to match rows.
    /// </summary>
    public enum SpatialOperation
    {
        /// <summary>
        ///     The features in the join features will be matched if they intersect a target feature.
        /// </summary>
        Intersect,

        /// <summary>
        ///     The features in the join features will be matched if they intersect a target feature in three-dimensional space (x,
        ///     y, and z).
        /// </summary>
        Intersect_3D,

        /// <summary>
        ///     The features in the join features will be matched if they are within a specified distance of a target feature.
        ///     Specify a distance in the search_radius parameter.
        /// </summary>
        WithinADistance,

        /// <summary>
        ///     Same as WITHIN_A_DISTANCE except that geodesic distance is used rather than planar distance. Choose this if your
        ///     data covers a large geographic extent or the coordinate system of the inputs is unsuitable for distance
        ///     calculations.
        /// </summary>
        WithinADistanceGeodesic,

        /// <summary>
        ///     The features in the join features will be matched if they are within a specified distance of a target feature in
        ///     three-dimensional space. Specify a distance in the search_radius parameter.
        /// </summary>
        WithinADistance_3D,

        /// <summary>
        ///     The features in the join features will be matched if a target feature contains them. The target features must be
        ///     polygons or polylines. For this option, the target features cannot be points, and the join features can only be
        ///     polygons when the target features are also polygons.
        /// </summary>
        Contains,

        /// <summary>
        ///     The features in the join features will be matched if a target feature completely contains them. Polygon can
        ///     completely contain any feature. Point cannot completely contain any feature, not even a point. Polyline can
        ///     completely contain only polyline and point.
        /// </summary>
        CompletelyContains,

        /// <summary>
        ///     This spatial relationship yields the same results as COMPLETELY_CONTAINS with the exception that if the join
        ///     feature is entirely on the boundary of the target feature (no part is properly inside or outside) the feature will
        ///     not be matched. Clementini defines the boundary polygon as the line separating inside and outside, the boundary of
        ///     a line is defined as its end points, and the boundary of a point is always empty.
        /// </summary>
        ContainsClementini,

        /// <summary>
        ///     The features in the join features will be matched if a target feature is within them. It is opposite to CONTAINS.
        ///     For this option, the target features can only be polygons when the join features are also polygons. Point can be
        ///     join feature only if point is target.
        /// </summary>
        Within,

        /// <summary>
        ///     The features in the join features will be matched if a target feature is completely within them. This is opposite
        ///     to COMPLETELY_CONTAINS.
        /// </summary>
        CompletelyWithin,

        /// <summary>
        ///     The result will be identical to WITHIN except if the entirety of the feature in the join features is on the
        ///     boundary of the target feature, the feature will not be matched. Clementini defines the boundary polygon as the
        ///     line separating inside and outside, the boundary of a line is defined as its end points, and the boundary of a
        ///     point is always empty.
        /// </summary>
        WithinClementini,

        /// <summary>
        ///     The features in the join features will be matched if they are identical to a target feature. Both join and target
        ///     feature must be of same shape type—point-to-point, line-to-line, and polygon-to-polygon.
        /// </summary>
        AreIdenticalTo,

        /// <summary>
        ///     The features in the join features will be matched if they have a boundary that touches a target feature. When the
        ///     target and join features are lines or polygons, the boundary of the join feature can only touch the boundary of the
        ///     target feature and no part of the join feature can cross the boundary of the target feature.
        /// </summary>
        BoundaryTouches,

        /// <summary>
        ///     The features in the join features will be matched if they share a line segment with a target feature. The join and
        ///     target features must be lines or polygons.
        /// </summary>
        ShareALineSegmentWith,

        /// <summary>
        ///     The features in the join features will be matched if a target feature is crossed by their outline. The join and
        ///     target features must be lines or polygons. If polygons are used for the join or target features, the polygon's
        ///     boundary (line) will be used. Lines that cross at a point will be matched, not lines that share a line segment.
        /// </summary>
        CrossedByTheOutlineOf,

        /// <summary>
        ///     The features in the join features will be matched if a target feature's center falls within them. The center of the
        ///     feature is calculated as follows: for polygon and multipoint the geometry's centroid is used, and for line input
        ///     the geometry's midpoint is used. Specify a distance in the search_radius parameter.
        /// </summary>
        HaveTheirCenterIn,

        /// <summary>
        ///     The feature in the join features that is closest to a target feature is matched. See the usage tip for more
        ///     information.
        /// </summary>
        Closest,

        /// <summary>
        ///     Same as CLOSEST except that geodesic distance is used rather than planar distance. Choose this if your data covers
        ///     a large geographic extent or the coordinate system of the inputs is unsuitable for distance calculations
        /// </summary>
        ClosestGeodesic
    }

    /// <summary>
    ///     The available statistic types
    /// </summary>
    public enum StatisticType
    {
        /// <summary>
        ///     Adds the total value for the specified field.
        /// </summary>
        Sum,

        /// <summary>
        ///     Calculates the average for the specified field.
        /// </summary>
        Mean,

        /// <summary>
        ///     Finds the smallest value for all records of the specified field.
        /// </summary>
        Min,

        /// <summary>
        ///     Finds the largest value for all records of the specified field.
        /// </summary>
        Max,

        /// <summary>
        ///     Finds the range of values (MAX – MIN) for the specified field.
        /// </summary>
        Range,

        /// <summary>
        ///     Finds the standard deviation on values in the specified field.
        /// </summary>
        Std,

        /// <summary>
        ///     Finds the first record in the Input Table and uses its specified field value.
        /// </summary>
        First,

        /// <summary>
        ///     Finds the last record in the Input Table and uses its specified field value.
        /// </summary>
        Last,

        /// <summary>
        ///     Finds the number of values included in statistical calculations. This counts each value except null values.
        /// </summary>
        Count
    }
}