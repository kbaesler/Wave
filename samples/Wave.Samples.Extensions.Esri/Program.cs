using System;
using System.Linq;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

namespace Samples
{
    internal class Program
    {
        #region Public Methods

        /// <summary>
        ///     Clears the layer definitions.
        /// </summary>
        public void ClearLayerDefinitions()
        {
            IMap map = ArcMap.Application.GetActiveMap();
            foreach (var layerDefinition in map.Where(o => o.Visible).Select(o => (IFeatureLayerDefinition2) o))
            {
                layerDefinition.DefinitionExpression = null;
            }
        }

        /// <summary>
        ///     Updates all of the features 'TIMECREATED' field to the current date time for
        ///     those features that have NULLs.
        /// </summary>
        /// <param name="featureClass">The feature class.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of records updated.
        /// </returns>
        public int UpdateTimeCreated(IFeatureClass featureClass)
        {
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = "TIMECREATED IS NULL";

            int recordsAffected = featureClass.Fetch(filter, true, feature =>
            {
                feature.Update("TIMECREATED", DateTime.Now);
                feature.SaveChanges();

                return true;
            });

            return recordsAffected;
        }

        /// <summary>
        /// Creates a connection to the workspace using the sde connection file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// Returns a <see cref="IWorkspace" /> representing the connection to the database.
        /// </returns>
        public IWorkspace CreateWorkspace(string fileName)
        {
            IWorkspaceFactory factory = WorkspaceFactories.GetFactory(fileName);
            return factory.OpenFromFile(fileName, 0);
        }

        #endregion

        #region Private Methods

        private static void Main(string[] args)
        {
        }

        #endregion
    }
}