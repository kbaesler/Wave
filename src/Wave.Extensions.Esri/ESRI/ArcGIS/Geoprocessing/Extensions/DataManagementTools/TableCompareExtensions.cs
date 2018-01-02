using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extensions methods for the <see cref="TableCompare" /> Geoprocessing Tools.
    /// </summary>
    public static class TableCompareExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Compares two tables or table views and returns the comparison results. This tool can report differences and
        ///     similarities with tabular values and field definitions
        /// </summary>
        /// <param name="source">
        ///     The Input Base Table is compared with the Input Test Table. The Input Base Table refers to tabular
        ///     data that you have declared valid. This base data has the correct field definitions and attribute values.
        /// </param>
        /// <param name="tableName">
        ///     The Input Test Table is compared against the Input Base Table. The Input Test Table refers to data
        ///     that you have made changes to by editing or compiling new fields, new records, or new attribute values.
        /// </param>
        /// <param name="outputFile">
        ///     This file will contain all similarities and differences between the Input Base Features and
        ///     the Input Test Features. This file is a comma-delimited text file that can be viewed and used as a table in ArcGIS.
        /// </param>
        /// <param name="sortFields">
        ///     The field or fields used to sort records in the Input Base Table and the Input Test Table. The
        ///     records are sorted in ascending order.
        /// </param>
        /// <param name="omitFields">
        ///     The field or fields that will be omitted during comparison. The field definitions and the
        ///     tabular values for these fields will be ignored.
        /// </param>
        /// <param name="continueCompare">Indicates whether to compare all properties after encountering the first mismatch.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        public static void Compare(this ITable source, string tableName, string outputFile, string[] sortFields, string[] omitFields, bool continueCompare, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            var test = ((IDataset) source).Workspace.GetTable(tableName);
            CompareImpl(source, test, outputFile, sortFields, omitFields, "ATTRIBUTES_ONLY", "", continueCompare, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Compares two tables or table views and returns the comparison results. This tool can report differences and
        ///     similarities with tabular values and field definitions
        /// </summary>
        /// <param name="source">
        ///     The Input Base Table is compared with the Input Test Table. The Input Base Table refers to tabular
        ///     data that you have declared valid. This base data has the correct field definitions and attribute values.
        /// </param>
        /// <param name="test">
        ///     The Input Test Table is compared against the Input Base Table. The Input Test Table refers to data
        ///     that you have made changes to by editing or compiling new fields, new records, or new attribute values.
        /// </param>
        /// <param name="outputFile">
        ///     This file will contain all similarities and differences between the Input Base Features and
        ///     the Input Test Features. This file is a comma-delimited text file that can be viewed and used as a table in ArcGIS.
        /// </param>
        /// <param name="sortFields">
        ///     The field or fields used to sort records in the Input Base Table and the Input Test Table. The
        ///     records are sorted in ascending order.
        /// </param>
        /// <param name="omitFields">
        ///     The field or fields that will be omitted during comparison. The field definitions and the
        ///     tabular values for these fields will be ignored.
        /// </param>
        /// <param name="continueCompare">Indicates whether to compare all properties after encountering the first mismatch.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        public static void Compare(this ITable source, ITable test, string outputFile, string[] sortFields, string[] omitFields, bool continueCompare, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            CompareImpl(source, test, outputFile, sortFields, omitFields, "ATTRIBUTES_ONLY", "", continueCompare, trackCancel, eventHandler);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Compares two tables or table views and returns the comparison results. This tool can report differences and
        ///     similarities with tabular values and field definitions
        /// </summary>
        /// <param name="source">
        ///     The Input Base Table is compared with the Input Test Table. The Input Base Table refers to tabular data that you
        ///     have declared valid. This base data has the correct field definitions and attribute values.
        /// </param>
        /// <param name="test">
        ///     The Input Test Table is compared against the Input Base Table. The Input Test Table refers to data that you have
        ///     made changes to by editing or compiling new fields, new records, or new attribute values.
        /// </param>
        /// <param name="outputFile">
        ///     This file will contain all similarities and differences between the Input Base Features and
        ///     the Input Test Features. This file is a comma-delimited text file that can be viewed and used as a table in ArcGIS.
        /// </param>
        /// <param name="sortFields">
        ///     The field or fields used to sort records in the Input Base Table and the Input Test Table. The
        ///     records are sorted in ascending order.
        /// </param>
        /// <param name="omitFields">
        ///     The field or fields that will be omitted during comparison. The field definitions and the
        ///     tabular values for these fields will be ignored.
        /// </param>
        /// <param name="compareType">
        ///     The comparision type. The default is All, which will compare all properties of the rows being compared.
        ///     ALL —All properties of the tables will be compared. This is the default.
        ///     ATTRIBUTES_ONLY —Only the attributes and their values will be compared.
        ///     SCHEMA_ONLY —Only the schema of the tables will be compared.
        /// </param>
        /// <param name="ignoreOptions">
        ///     These properties will not be compared.
        ///     IGNORE_EXTENSION_PROPERTIES —Do not compare extension properties.
        ///     IGNORE_SUBTYPES —Do not compare subtypes.
        ///     IGNORE_RELATIONSHIPCLASSES —Do not compare Relationship classes.
        ///     IGNORE_FIELDALIAS —Do not compare field alias
        /// </param>
        /// <param name="continueCompare">Indicates whether to compare all properties after encountering the first mismatch.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>        
        private static void CompareImpl(ITable source, ITable test, string outputFile, string[] sortFields, string[] omitFields, string compareType, string ignoreOptions, bool continueCompare, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            TableCompare gp = new TableCompare();

            gp.in_base_table = source;
            gp.in_test_table = test;

            if (sortFields != null)
                gp.sort_field = string.Join(";", sortFields);
            else
                gp.sort_field = source.OIDFieldName;

            if(omitFields != null)
                gp.omit_field = string.Join(";", omitFields);

            gp.compare_type = compareType;
            gp.ignore_options = ignoreOptions;

            gp.continue_compare = continueCompare ? "CONTINUE_COMPARE" : "NO_CONTINUE_COMPARE";
            gp.out_compare_file = outputFile;

            gp.Run(trackCancel, eventHandler);            
        }

        #endregion
    }
}