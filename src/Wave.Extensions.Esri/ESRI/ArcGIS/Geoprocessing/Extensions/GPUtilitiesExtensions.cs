using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGPUtilities3" /> interface.
    /// </summary>
    public static class GPUtilitiesExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the workspace specified by the given geoprocessing value object.
        /// </summary>
        /// <param name="utilities">The utilities.</param>
        /// <param name="value">The geoprocessing value object.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace object.
        /// </returns>
        public static IWorkspace GetWorkspace(this IGPUtilities2 utilities, IGPValue value)
        {
            if (!value.IsEmpty())
            {
                IDataset dataset = utilities.OpenDataset(value);
                if (dataset != null)
                {
                    return dataset.Workspace;
                }
            }

            return null;
        }

        /// <summary>
        ///     Opens the relationship class specified by the given geoprocessing value object.
        /// </summary>
        /// <param name="utilities">The utilities.</param>
        /// <param name="value">The geoprocessing value object.</param>
        /// <returns>
        ///     Returns a <see cref="IRelationshipClass" /> representing the table object.
        /// </returns>
        public static IRelationshipClass OpenRelationshipClass(this IGPUtilities2 utilities, IGPValue value)
        {
            if (!value.IsEmpty())
            {
                IDataset dataset = utilities.OpenDataset(value);
                if (dataset != null)
                {
                    return dataset as IRelationshipClass;
                }
            }

            return null;
        }

        /// <summary>
        ///     Opens the table object specified by the given geoprocessing value object.
        /// </summary>
        /// <param name="utilities">The utilities.</param>
        /// <param name="value">The geoprocessing value object.</param>
        /// <returns>
        ///     Returns a <see cref="IObjectClass" /> representing the table object.
        /// </returns>
        public static IObjectClass OpenTable(this IGPUtilities2 utilities, IGPValue value)
        {
            if (!value.IsEmpty())
            {
                IDataset dataset = utilities.OpenDataset(value);
                if (dataset != null)
                {
                    return dataset as IObjectClass;
                }
            }

            return null;
        }

        #endregion
    }
}