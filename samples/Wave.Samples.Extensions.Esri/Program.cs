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
        /// Clears the layer definitions.
        /// </summary>
        /// <param name="map">The map document within ArcMap.</param>
        public void ClearDefinitions(IMap map)
        {
            foreach (var layerDefinition in map.Where(o => o.Visible).Select(o => (IFeatureLayerDefinition2) o))
            {
                layerDefinition.DefinitionExpression = null;
            }
        }

        /// <summary>
        ///     Creates a connection to the workspace using the sde connection file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the connection to the database.
        /// </returns>
        public IWorkspace CreateWorkspace(string fileName)
        {
            return WorkspaceFactories.Open(fileName);
        }


        /// <summary>
        ///     Creates the workspace based on the connection properties.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="version">The version.</param>
        /// <param name="database">The database.</param>
        /// <param name="password">The password.</param>
        /// <param name="username">The username.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the connection to the database.
        /// </returns>
        public IWorkspace CreateWorkspace(string server, string instance, string version, string database, string password, string username)
        {
            return WorkspaceFactories.Open(server, instance, version, database, password, username);
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
        /// Validates the changes made within a version by performing 
        /// a version difference between the child and it's parent version.
        /// </summary>
        public void ValidateUpdates(IVersion childVersion, IVersion parentVersion)
        {
            var differences = childVersion.GetDifferences(parentVersion, null, (s, table) => table is IFeatureClass, esriDifferenceType.esriDifferenceTypeUpdateDelete, esriDifferenceType.esriDifferenceTypeUpdateNoChange, esriDifferenceType.esriDifferenceTypeUpdateUpdate);            
            foreach (var table in differences)
            {
                Console.WriteLine("Table: {0}", table.Key);

                foreach (var differenceRow in table.Value)
                {
                    Console.WriteLine("+++++++++++ {0} +++++++++++", differenceRow.OID);

                    foreach (var index in differenceRow.FieldIndices.AsEnumerable())
                    {
                        Console.WriteLine("Original: {0} -> Changed: {1}", differenceRow.Original.GetValue(index, DBNull.Value), differenceRow.Changed.GetValue(index, DBNull.Value));
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private static void Main(string[] args)
        {
        }

        #endregion
    }
}