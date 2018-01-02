using System;
using System.Collections.Generic;
using System.Diagnostics;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods when working with geoprocessors.
    /// </summary>
    public static class GeoprocessingExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IGPMessages" />
        /// </summary>
        /// <param name="source">An <see cref="IGPMessages" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the individual messages.</returns>
        public static IEnumerable<IGPMessage> AsEnumerable(this IGPMessages source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                yield return source.GetMessage(i);
            }
        }

        /// <summary>
        ///     Exports the specified table or feature class name to the specified worksapce.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the exported table or feature class.
        /// </returns>
        public static ITable Export(this IWorkspace source, string tableName, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return source.Export(tableName, null, outputTableName, workspace, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Exports the specified table or feature class name to the specified worksapce.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the exported table or feature class.
        /// </returns>
        public static ITable Export(this IWorkspace source, string tableName, IQueryFilter filter, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            var table = source.GetTable(tableName);

            if (table.IsFeatureClass())
            {
                return ((IFeatureClass) table).Export(filter, outputTableName, workspace, trackCancel, eventHandler) as ITable;
            }

            return table.Export(filter, outputTableName, workspace, trackCancel, eventHandler);
        }


        /// <summary>
        ///     Gets the absolute path to the feature class in the data source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="string" /> representing the absolute path.</returns>
        public static string GetAbsolutePath(this IFeatureClass source)
        {
            IDataset ds = (IDataset) source;
            return ds.Workspace.GetAbsolutePath(ds.Name);
        }

        /// <summary>
        ///     Gets the absolute path to the table in the data source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="string" /> representing the absolute path.</returns>
        public static string GetAbsolutePath(this ITable source)
        {
            IDataset ds = (IDataset) source;
            return ds.Workspace.GetAbsolutePath(ds.Name);
        }

        /// <summary>
        ///     Gets the absolute path.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns a <see cref="string" /> representing the absolute path</returns>
        public static string GetAbsolutePath(this IWorkspace source, string name)
        {
            if (string.IsNullOrEmpty(source.PathName))
                throw new ArgumentOutOfRangeException("source", "The Path on the workspace is null");

            return string.Format("{0}\\{1}", source.PathName, name);
        }


        /// <summary>
        ///     Gets the messages that are captured by the geoprocessor
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{String}" /> representing the messages in the geoprocessor.</returns>
        public static IEnumerable<string> GetMessages(this Geoprocessor.Geoprocessor source)
        {
            if (source.MessageCount > 0)
            {
                for (int i = 0; i <= source.MessageCount - 1; i++)
                {
                    yield return source.GetMessage(i);
                }
            }
        }

        /// <summary>
        ///     Runs the specified process for the geoprocessor.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <returns>
        ///     The status of the job.
        /// </returns>
        public static esriJobStatus Run(this IGPProcess process, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return process.Run(null, true, false, trackCancel, eventHandler);
        }

        /// <summary>
        /// Runs the specified process for the geoprocessor.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <returns>
        /// The status of the job.
        /// </returns>
        public static esriJobStatus Run(this IGPProcess process, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return process.Run(workspace, true, false, trackCancel, eventHandler);
        }

        /// <summary>
        /// Runs the specified process for the geoprocessor.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="overwriteOutput">if set to <c>true</c> the tools automatically overwrite any existing output when run.
        /// When it is <c>true</c>, you receive a warning before tool execution that the output exists, but the tool executes
        /// and overwrites the output dataset. With this <c>false</c>, existing outputs are not overwritten, and the tool
        /// displays an error, preventing you from executing the tool.</param>
        /// <param name="addOutputsToMap">if set to <c>true</c> when the resulting output datasets should be added to the application display.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <returns>
        /// The status of the job.
        /// </returns>
        public static esriJobStatus Run(this IGPProcess process, IWorkspace workspace, bool overwriteOutput, bool addOutputsToMap, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            Geoprocessor.Geoprocessor gp = new Geoprocessor.Geoprocessor();
            gp.OverwriteOutput = overwriteOutput;
            gp.AddOutputsToMap = addOutputsToMap;
            
            if(workspace != null)
                gp.SetEnvironmentValue("workspace", workspace.PathName);            

            if (eventHandler != null)
                gp.RegisterGeoProcessorEvents(eventHandler);

            IGeoProcessorResult result;

            try
            {
                result = gp.Execute(process, trackCancel) as IGeoProcessorResult;
            }
            catch (Exception e)
            {
                Log.Error(gp.GetType(), e);

                foreach (var msg in gp.GetMessages())
                    Log.Error(gp.GetType(), msg);

                throw;
            }
            finally
            {
                if (eventHandler != null)
                    gp.UnRegisterGeoProcessorEvents(eventHandler);
            }

            return result != null ? result.Status : esriJobStatus.esriJobFailed;
        }
        
        /// <summary>
        ///     Gets the description of the error.
        /// </summary>
        /// <param name="source">The enumeration value.</param>
        /// <returns>Returns a <see cref="string" /> representing the error description.</returns>
        public static string ToDescription(this esriLocatingError source)
        {
            switch (source)
            {
                case esriLocatingError.LOCATING_E_CANT_FIND_EXTENT:
                    return "Could not find route location's shape, the from-measure and the to-measure are outside of the route measures.";
                case esriLocatingError.LOCATING_E_CANT_FIND_LOCATION:
                    return "Could not find route location's shape (the route has no M's or the route location's measures don't exist on the route).";
                case esriLocatingError.LOCATING_E_CANT_FIND_ROUTE:
                    return "The route does not exist.";
                case esriLocatingError.LOCATING_E_FROM_PARTIAL_MATCH:
                    return "Could not find the entire route location's shape, the from-measure was outside of the route measure range.";
                case esriLocatingError.LOCATING_E_FROM_TO_PARTIAL_MATCH:
                    return "Could not find the entire route location's shape, the from-measure and the to-measure were outside of the route measure range.";
                case esriLocatingError.LOCATING_E_INVALIDMEASURE:
                    return "At least one of the route location's measure values is invalid.";
                case esriLocatingError.LOCATING_E_INVALIDRID:
                    return "The route location's route ID is invalid (NULL, empty or invalid value).";
                case esriLocatingError.LOCATING_E_MULTIPLE_LOCATION:
                    return "More than one point location were found.";
                case esriLocatingError.LOCATING_E_NULL_EXTENT:
                    return "The from-measure is equal to the to-measure.";
                case esriLocatingError.LOCATING_E_ROUTE_MS_NULL:
                    return "The route does not have Ms or Ms are null.";
                case esriLocatingError.LOCATING_E_ROUTE_NOT_MAWARE:
                    return "The route is not a polyline M aware.";
                case esriLocatingError.LOCATING_E_ROUTE_SHAPE_EMPTY:
                    return "The route does not have a shape or the shape is empty.";
                case esriLocatingError.LOCATING_E_TO_PARTIAL_MATCH:
                    return "Could not find the entire route location's shape, the to-measure was outside of the route measure range.";
                case esriLocatingError.LOCATING_OK:
                    return "Locating was successful.";
                default:
                    return "Unknown.";
            }
        }


        /// <summary>
        ///     Gets the description of the enumeration.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="string" /> representing the description.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source</exception>
        public static string ToDescription(this JoinOperation source)
        {
            switch (source)
            {
                case JoinOperation.OneToOne:
                    return "JOIN_ONE_TO_ONE";
                case JoinOperation.OneToMany:
                    return "JOIN_ONE_TO_MANY";
                default:
                    throw new ArgumentOutOfRangeException("source");
            }
        }

        /// <summary>
        ///     Gets the description of the enumeration.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="string" /> representing the description.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source</exception>
        public static string ToDescription(this SpatialOperation source)
        {
            switch (source)
            {
                case SpatialOperation.Intersect:
                    return "INTERSECT";
                case SpatialOperation.Intersect_3D:
                    return "INTERSECT_3D";
                case SpatialOperation.WithinADistance:
                    return "WITHIN_A_DISTANCE";
                case SpatialOperation.WithinADistanceGeodesic:
                    return "WITHIN_A_DISTANCE_GEODESIC";
                case SpatialOperation.WithinADistance_3D:
                    return "WITHIN_A_DISTANCE_3D";
                case SpatialOperation.Contains:
                    return "CONTAINS";
                case SpatialOperation.CompletelyContains:
                    return "COMPLETELY_CONTAINS";
                case SpatialOperation.ContainsClementini:
                    return "CONTAINS_CLEMENTINI";
                case SpatialOperation.Within:
                    return "WITHIN";
                case SpatialOperation.CompletelyWithin:
                    return "COMPLETELY_WITHIN";
                case SpatialOperation.WithinClementini:
                    return "WITHIN_CLEMENTINI";
                case SpatialOperation.AreIdenticalTo:
                    return "ARE_IDENTICAL_TO";
                case SpatialOperation.BoundaryTouches:
                    return "BOUNDARY_TOUCHES";
                case SpatialOperation.ShareALineSegmentWith:
                    return "SHARE_A_LINE_SEGMENT_WITH";
                case SpatialOperation.CrossedByTheOutlineOf:
                    return "CROSSED_BY_THE_OUTLINE_OF";
                case SpatialOperation.HaveTheirCenterIn:
                    return "HAVE_THEIR_CENTER_IN";
                case SpatialOperation.Closest:
                    return "CLOSEST";
                case SpatialOperation.ClosestGeodesic:
                    return "CLOSEST_GEODESIC";
                default:
                    throw new ArgumentOutOfRangeException("source");
            }
        }

        #endregion
    }
}