using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extensions for the Data Management Geoprocessing Tools
    /// </summary>
    public static class DataManagementToolsExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the field to the table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="length">The length.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void AddField(this ITable source, string name, Type type, int length, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            AddField gp = new AddField();
            gp.in_table = source;
            gp.field_name = name;
            gp.field_type = type == typeof (string) ? "TEXT" : type.Name.ToUpperInvariant();
            gp.field_length = length;

            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        ///     Adds the field to the feature class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="length">The length.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void AddField(this IFeatureClass source, string name, Type type, int length, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            ((ITable) source).AddField(name, type, length, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Adds an attribute index to an existing table, feature class, shapefile, coverage, or attributed relationship class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name of the new index. An index name is necessary when adding an index to geodatabase feature
        ///     classes and tables. For other data types, such as shapefiles and coverage feature classes, index names cannot be
        ///     specified.
        /// </param>
        /// <param name="fields">
        ///     The list of fields that can participate in an attribute index. Any number of these fields can be
        ///     part of the index.
        /// </param>
        /// <param name="unique">if set to <c>true</c> when the values in the index are unique.</param>
        /// <param name="ascending">if set to <c>true</c> when the values are indexed in ascending order.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void AddIndex(this IFeatureClass source, string name, string[] fields, bool unique, bool ascending, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            ((ITable) source).AddIndex(name, fields, unique, ascending, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Adds an attribute index to an existing table, feature class, shapefile, coverage, or attributed relationship class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name of the new index. An index name is necessary when adding an index to geodatabase feature
        ///     classes and tables. For other data types, such as shapefiles and coverage feature classes, index names cannot be
        ///     specified.
        /// </param>
        /// <param name="fields">
        ///     The list of fields that can participate in an attribute index. Any number of these fields can be
        ///     part of the index.
        /// </param>
        /// <param name="unique">if set to <c>true</c> when the values in the index are unique.</param>
        /// <param name="ascending">if set to <c>true</c> when the values are indexed in ascending order.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void AddIndex(this ITable source, string name, string[] fields, bool unique, bool ascending, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            AddIndex gp = new AddIndex();
            gp.in_table = source;
            gp.index_name = name;
            gp.fields = string.Join(";", fields);
            gp.unique = unique ? "UNIQUE" : "NON_UNIQUE";
            gp.ascending = ascending ? "ASCENDING" : "NON_ASCENDING";
            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        ///     Adds the fields POINT_X and POINT_Y to the point input features and calculates their values. It also appends the
        ///     POINT_Z and POINT_M fields if the input features are Z- and M-enabled.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void AddXY(this IFeatureClass source, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            AddXY gp = new AddXY();
            gp.in_features = source;
            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        ///     Calculates the values of a field for a feature class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">The field that will be updated with the new calculation.</param>
        /// <param name="expression">The simple calculation expression used to create a value that will populate the selected rows.</param>
        /// <param name="python">A block of Python code for complex expresssions.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void Calculate(this IFeatureClass source, string fieldName, string expression, string python, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            ((ITable) source).Calculate(fieldName, expression, python, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Calculates the values of a field for a feature class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">The field that will be updated with the new calculation.</param>
        /// <param name="expression">The simple calculation expression used to create a value that will populate the selected rows.</param>
        /// <param name="python">A block of Python code for complex expresssions.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        public static void Calculate(this ITable source, string fieldName, string expression, string python, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            CalculateField gp = new CalculateField();
            gp.in_table = source;
            gp.field = fieldName;
            gp.expression = expression;
            gp.code_block = python;
            gp.expression_type = "PYTHON_9.3";
            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        /// Copies the rows from the source into the the target table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="visibleFields">Specifies which fields from the input table to rename and make visible in the output table
        /// view.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        public static void CopyTo(this ITable source, ITable target, IQueryFilter filter, Dictionary<string, string> visibleFields,ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            object input = source;

            if (filter != null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                var name = string.Format("{0}_V", ((IDataset) source).Name);
                input = source.MakeView(name, filter, visibleFields, trackCancel, eventHandler);
            }

            CopyRows gp = new CopyRows();
            gp.in_rows = input;
            gp.out_table = target;

            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        /// Copies the rows from the source into the the target table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        public static void Truncate(this ITable source, IQueryFilter filter, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            object input = source;

            if (filter != null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                var name = string.Format("{0}_V", ((IDataset)source).Name);
                input = source.MakeView(name, filter, null, trackCancel, eventHandler);
            }

            DeleteRows gp = new DeleteRows();
            gp.in_rows = input;

            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        ///     Copies the features from the source into the the target feature class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="visibleFields">
        ///     Specifies which fields from the input table to rename and make visible in the output table
        ///     view.
        /// </param>
        /// <param name="schemaMatchRequired">
        ///     if set to <c>true</c> if the schema must match between the two feature classes to
        ///     append the data.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        public static void CopyTo(this IFeatureClass source, IFeatureClass target, IQueryFilter filter, Dictionary<string, string> visibleFields, bool schemaMatchRequired, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            object input = source;

            if (filter != null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                var name = string.Format("{0}_V", ((IDataset) source).Name);
                input = source.MakeView(name, filter, visibleFields, trackCancel, eventHandler);
            }

            Append gp = new Append();
            gp.inputs = input;
            gp.target = target;
            gp.schema_type = schemaMatchRequired ? "TEST" : "NO_TEST";

            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        ///     Copies the features from the source into the the target feature class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="targetTableName">Name of the target table.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="visibleFields">
        ///     Specifies which fields from the input table to rename and make visible in the output table
        ///     view.
        /// </param>
        /// <param name="schemaMatchRequired">
        ///     if set to <c>true</c> if the schema must match between the two feature classes to
        ///     append the data.
        /// </param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        public static void CopyTo(this IFeatureClass source, string targetTableName, IQueryFilter filter, Dictionary<string, string> visibleFields, bool schemaMatchRequired, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            object input = source;

            if (filter != null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                var name = string.Format("{0}_V", ((IDataset) source).Name);
                input = source.MakeView(name, filter, visibleFields, trackCancel, eventHandler);
            }

            Append gp = new Append();
            gp.inputs = input;
            gp.target = workspace.GetAbsolutePath(targetTableName);
            gp.schema_type = schemaMatchRequired ? "TEST" : "NO_TEST";

            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        ///     Deletes records in a feature class or table which have identical values in a list of fields. If the field Shape is
        ///     selected, feature geometries are compared.s
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fields">The field or fields whose values will be compared to find identical records.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <remarks>
        ///     This tool finds identical records based on input field values, then deletes all but one of the identical
        ///     records from each set of identical records. The values from multiple fields in the input dataset can be compared.
        ///     If more than one field is specified, records are matched by the values in the first field, then by the values of
        ///     the second field, and so on.
        /// </remarks>
        public static void DeleteIdentical(this ITable source, string[] fields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            DeleteIdentical gp = new DeleteIdentical();
            gp.in_dataset = source;
            gp.fields = string.Join(";", fields);
            gp.Run(trackCancel, eventHandler);
        }

        /// <summary>
        ///     Aggregates features based on specified attributes.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="dissolveFields">The field or fields on which to aggregate features.</param>
        /// <param name="statisticFields">The fields and statistics with which to summarize attributes.</param>
        /// <param name="multiPart">if set to <c>true</c> when multipart features are allowed.</param>
        /// <param name="unsplitLines">
        ///     if set to <c>false</c> the lines are dissolved into a single feature. When UNSPLIT_LINES is
        ///     specified, only two lines that have a common endpoint (known as pseudonode) are merged into one continuous line.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>Returns a <see cref="IFeatureClass" /> representing the feature class that contains the aggregated features.</returns>
        public static IFeatureClass Dissolve(this IFeatureClass source, IQueryFilter filter, string tableName, IWorkspace workspace, string[] dissolveFields, List<KeyValuePair<string, StatisticType>> statisticFields, bool multiPart, bool unsplitLines, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            object input = source;

            if (filter != null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                var name = string.Format("{0}_V", ((IDataset) source).Name);
                input = source.MakeView(name, filter, null, trackCancel, eventHandler);
            }

            Dissolve gp = new Dissolve();
            gp.in_features = input;
            gp.out_feature_class = workspace.GetAbsolutePath(tableName);

            gp.dissolve_field = string.Join("; ", dissolveFields);

            if (statisticFields != null)
            {
                string x = null;
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in statisticFields)
                {
                    sb.Append(x);
                    sb.AppendFormat("{0} {1}", kvp.Key, kvp.Value);
                    x = ";";
                }

                gp.statistics_fields = sb.ToString().ToUpperInvariant();
            }

            gp.multi_part = multiPart ? "MULTI_PART" : "SINGLE_PART";
            gp.unsplit_lines = unsplitLines ? "UNSPLIT_LINES" : "DISSOLVE_LINES";

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
                return workspace.GetFeatureClass(tableName);

            return null;
        }

        /// <summary>
        ///     Writes the rows from an feature class to a new feature class.
        /// </summary>
        /// <param name="source">The rows from a feature class to be copied.</param>
        /// <param name="tableName">The name of the table to which the rows will be written.</param>
        /// <param name="workspace">The workspace that will contain the table.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the exported table.
        /// </returns>
        public static IFeatureClass Export(this IFeatureClass source, string tableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return source.Export(null, tableName, workspace, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Writes the rows from an feature class to a new feature class.
        /// </summary>
        /// <param name="source">The rows from a feature class to be copied.</param>
        /// <param name="filter">The filter used to create a subset of the data.</param>
        /// <param name="tableName">The name of the table to which the rows will be written.</param>
        /// <param name="workspace">The workspace that will contain the table.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the exported table.
        /// </returns>
        public static IFeatureClass Export(this IFeatureClass source, IQueryFilter filter, string tableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            object input = source;

            if (filter != null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                input = source.MakeView(string.Format("{0}_V", tableName), filter, null, trackCancel, eventHandler);
            }

            CopyFeatures gp = new CopyFeatures();
            gp.in_features = input;
            gp.out_feature_class = workspace.GetAbsolutePath(tableName);

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
            {
                var table = workspace.GetFeatureClass(tableName);
                return table;
            }

            return null;
        }

        /// <summary>
        ///     Writes the rows from an input table to a new table.
        /// </summary>
        /// <param name="source">The rows from a table to be copied.</param>
        /// <param name="filter">The filter used to create a subset of the data.</param>
        /// <param name="tableName">The name of the table to which the rows will be written.</param>
        /// <param name="workspace">The workspace that will contain the table.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the exported table.
        /// </returns>
        public static ITable Export(this ITable source, IQueryFilter filter, string tableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            object input = source;

            if (filter != null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                var name = string.Format("V_{0}", tableName);
                input = source.MakeView(name, filter, null, trackCancel, eventHandler);
            }

            CopyRows gp = new CopyRows();
            gp.in_rows = input;
            gp.out_table = tableName;

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
            {
                var table = workspace.GetTable(tableName);
                return table;
            }

            return null;
        }       

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Creates a table view from an input table or feature class. The table view that is created by the tool is temporary
        ///     and will not persist after the session ends unless the document is saved
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">The name of the table view to be created.</param>
        /// <param name="filter">An SQL expression used to select a subset of features.</param>
        /// <param name="visibleFields">
        ///     Specifies which fields from the input table to rename and make visible in the output table
        ///     view.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the path to the view.
        /// </returns>
        internal static string MakeView(this IFeatureClass source, string tableName, string filter, Dictionary<string, string> visibleFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return source.MakeView(tableName, new QueryFilterClass {WhereClause = filter}, visibleFields, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Creates a table view from an input table or feature class. The table view that is created by the tool is temporary
        ///     and will not persist after the session ends unless the document is saved
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">The name of the table view to be created.</param>
        /// <param name="filter">An SQL expression used to select a subset of features. </param>
        /// <param name="visibleFields">
        ///     Specifies which fields from the input table to rename and make visible in the output table
        ///     view.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>Returns a <see cref="string" /> representing the path to the view.</returns>
        internal static string MakeView(this IFeatureClass source, string tableName, IQueryFilter filter, Dictionary<string, string> visibleFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            var ds = (IDataset) source;
            var input = ds.Workspace.GetAbsolutePath(tableName);

            MakeFeatureLayer gp = new MakeFeatureLayer();
            gp.in_features = source;
            gp.where_clause = filter == null ? null : filter.WhereClause;
            gp.out_layer = input;

            if (visibleFields != null && visibleFields.Any())
            {
                IGpFieldInfoObject fieldInfo = new GpFieldInfoObjectClass();

                foreach (var field in visibleFields)
                {
                    fieldInfo.AddField(field.Key, field.Value, "true", "None");
                }

                gp.field_info = fieldInfo;
            }

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
                return input;

            return null;
        }

        /// <summary>
        ///     Creates a table view from an input table or feature class. The table view that is created by the tool is temporary
        ///     and will not persist after the session ends unless the document is saved
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableName">The name of the table view to be created.</param>
        /// <param name="filter">An SQL expression used to select a subset of features. </param>
        /// <param name="visibleFields">
        ///     Specifies which fields from the input table to rename and make visible in the output table
        ///     view.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>Returns a <see cref="string" /> representing the path to the view.</returns>
        internal static string MakeView(this ITable source, string tableName, IQueryFilter filter, Dictionary<string, string> visibleFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            var ds = (IDataset) source;
            var input = ds.Workspace.GetAbsolutePath(tableName);

            MakeTableView gp = new MakeTableView();
            gp.in_table = source;
            gp.where_clause = filter == null ? null : filter.WhereClause;
            gp.out_view = input;

            if (visibleFields != null && visibleFields.Any())
            {
                IGpFieldInfoObject fieldInfo = new GpFieldInfoObjectClass();
                foreach (var field in visibleFields)
                {
                    fieldInfo.AddField(field.Key, field.Value, "true", "");
                }

                gp.field_info = fieldInfo;
            }

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
                return input;

            return null;
        }

        #endregion
    }
}