using ESRI.ArcGIS.EditingTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods for the the editing geoprocessing tool.
    /// </summary>
    public static class EditingToolspExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds vertices along line or polygon features. Also replaces curve segments (Bezier, circular arcs, and elliptical
        ///     arcs) with line segments
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void Densify(this IFeatureClass source, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            Densify gp = new Densify();
            gp.in_features = source;
            gp.densification_method = "DISTANCE";
            gp.distance = 10;
            gp.max_angle = 10;
            gp.max_deviation = "";
            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        ///     Adds vertices along line or polygon features. Also replaces curve segments (Bezier, circular arcs, and elliptical
        ///     arcs) with line segments
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="distance">
        ///     The maximum linear distance between vertices. This distance will always be applied to line
        ///     segments and to simplify curves. The default value is a function of the data's X,Y tolerance.
        /// </param>
        /// <param name="deviation">
        ///     The maximum distance the output segment can be from the original. This parameter only affects
        ///     curves. The default value is a function of the data's X,Y tolerance.
        /// </param>
        /// <param name="densificationMethod">
        ///     The method selected to handle feature densification.
        ///     - DISTANCE —The tool will apply the value of the distance parameter to curves the same as it does to straight
        ///     lines. This is the default.
        ///     - OFFSET —The tool will apply the value of the max_deviation parameter to curves.
        ///     - ANGLE —The tool will apply the value of the max_angle parameter to curves.
        /// </param>
        /// <param name="angle">
        ///     The maximum angle that the output geometry can be from the input geometry. The valid range is from
        ///     0 to 90. The default value is 10. This parameter only affects curves.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void Densify(this IFeatureClass source, double distance, double deviation, string densificationMethod = "DISTANCE", double angle = 10, ITrackCancel trackCancel = null, IGeoProcessorEvents eventHandler = null)
        {
            Densify gp = new Densify();
            gp.in_features = source;
            gp.densification_method = densificationMethod;
            gp.distance = distance;
            gp.max_angle = angle;
            gp.max_deviation = deviation;
            gp.Run(trackCancel, eventHandler);
        }


        /// <summary>
        /// Moves points or vertices to coincide exactly with the vertices, edges, or end points of other features.
        /// Snapping rules can be specified to control whether the input vertices are snapped to the nearest vertex, edge, or
        /// endpoint within a specified distance
        /// </summary>
        /// <param name="source">The input features whose vertices will be snapped to the vertices, edges, or end points of other
        /// features. The input features can be points, multipoints, lines, or polygons.</param>
        /// <param name="snap">The features that the input features' vertices will be snapped to. These features can be points,
        /// multipoints, lines, or polygons.</param>
        /// <param name="snapType">The type of feature part that the input features' vertices can be snapped to (END | VERTEX |
        /// EDGE).</param>
        /// <param name="distance">The distance within which the input features' vertices will be snapped to the nearest vertex,
        /// edge, or end point.</param>
        /// <param name="units">The units.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void Snap(this IFeatureClass source, IFeatureClass snap, SnapType snapType, double distance, esriUnits units, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            IUnitConverter converter = new UnitConverterClass();
            object row = string.Format("{0} {1} '{2} {3}'", snap.GetAbsolutePath(), snapType.ToString().ToUpperInvariant(), distance, converter.EsriUnitsAsString(units, esriCaseAppearance.esriCaseAppearanceUpper, true));

            IGpValueTableObject table = new GpValueTableObjectClass();
            table.SetColumns(3);
            table.AddRow(ref row);

            source.Snap(table, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Moves points or vertices to coincide exactly with the vertices, edges, or end points of other features.
        ///     Snapping rules can be specified to control whether the input vertices are snapped to the nearest vertex, edge, or
        ///     endpoint within a specified distance
        /// </summary>
        /// <param name="source">
        ///     The input features whose vertices will be snapped to the vertices, edges, or end points of other
        ///     features. The input features can be points, multipoints, lines, or polygons.
        /// </param>
        /// <param name="snapEnvironments">The feature classes or feature layers containing the features you wish to snap to.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void Snap(this IFeatureClass source, IGpValueTableObject snapEnvironments, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            Snap gp = new Snap();
            gp.in_features = source;
            gp.snap_environment = snapEnvironments;
            gp.Run(trackCancel, eventHandler);
        }

        #endregion
    }

    #region Enumerations

    /// <summary>
    ///     Snapping types.
    /// </summary>
    public enum SnapType
    {
        /// <summary>
        ///     Snapping is applied to feature vertices.
        /// </summary>
        Vertex,

        /// <summary>
        ///     Snapping is applied to feature edges.
        /// </summary>
        Edge,

        /// <summary>
        ///     Snapping is applied to the end points of features.
        /// </summary>
        End
    }

    #endregion
}