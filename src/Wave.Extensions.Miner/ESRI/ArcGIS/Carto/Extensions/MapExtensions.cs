using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

namespace ESRI.ArcGIS.Carto
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMap" /> interface.
    /// </summary>
    public static class MapExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns the feature class that is assigned the <paramref name="modelName" /> that is in the map.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="modelName">The class model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IFeatureClass" /> representing the feature class that is assigned
        ///     the model name.
        /// </returns>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IFeatureClass GetFeatureClass(this IMap source, string modelName, bool throwException = true)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            var list = source.GetFeatureClasses(modelName);
            var table = list.FirstOrDefault();

            if (table == null && throwException)
                throw new MissingClassModelNameException(modelName);

            return table;
        }

        /// <summary>
        ///     Returns the feature class that is assigned the <paramref name="modelName" /> that is in the map.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="modelName">The class model name.</param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IFeatureClass" /> representing the feature class that is assigned
        ///     the model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<IFeatureClass> GetFeatureClasses(this IMap source, string modelName)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            return source.Where<IFeatureLayer>(o => o.Valid && o.FeatureClass.IsAssignedClassModelName(modelName)).Select(o => o.FeatureClass);
        }

        /// <summary>
        ///     Returns the layer that is assigned the <paramref name="modelName" /> that resides the map.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="modelName">The class model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="ESRI.ArcGIS.Carto.IFeatureLayer" /> representing the layer that is assigned the model
        ///     name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IFeatureLayer GetFeatureLayer(this IMap source, string modelName, bool throwException = true)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            var list = source.GetFeatureLayers(modelName);
            var layer = list.FirstOrDefault();

            if (layer == null && throwException)
                throw new MissingClassModelNameException(modelName);

            return layer;
        }
      
        /// <summary>
        ///     Returns the layers that are assigned the <paramref name="modelName" /> that resides the map.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="modelName">The class model name.</param>
        /// <returns>
        ///     Returns the <see cref="IEnumerable{IFeatureLayer}" /> representing the layers that are assigned the model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<IFeatureLayer> GetFeatureLayers(this IMap source, string modelName)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            return source.Where<IFeatureLayer>(o => o.Valid && o.FeatureClass.IsAssignedClassModelName(modelName));
        }

        /// <summary>
        ///     Returns the table that is assigned the <paramref name="modelName" /> that resides within map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">The name of the class model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.ITable" /> representing the table that is assigned the model
        ///     name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static ITable GetTable(this IMap source, string modelName, bool throwException = true)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            var list = source.GetTables(modelName);
            var table = list.FirstOrDefault();

            if (table == null && throwException)
                throw new MissingClassModelNameException(modelName);

            return table;
        }

        /// <summary>
        ///     Returns the tables that are assigned the <paramref name="modelName" /> that resides within map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">The name of the class model name.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{ITable}" /> representing the tables that are assigned the model
        ///     name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<ITable> GetTables(this IMap source, string modelName)
        {
            if (modelName == null) throw new ArgumentNullException("modelName");

            var standaloneTableCollection = source as IStandaloneTableCollection;
            if (standaloneTableCollection != null)
            {
                for (int i = 0; i < standaloneTableCollection.StandaloneTableCount; i++)
                {
                    IStandaloneTable standaloneTable = standaloneTableCollection.StandaloneTable[i];
                    if (standaloneTable != null && standaloneTable.Valid)
                    {
                        if (standaloneTable.Table.IsAssignedClassModelName(modelName))
                            yield return standaloneTable.Table;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the workspace that satisifies the <paramref name="predicate" /> criteria.
        ///     map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace in the map that is being edited.
        /// </returns>
        /// <exception cref="ArgumentNullException">predicate</exception>
        public static IWorkspace GetWorkspace(this IMap source, Predicate<IWorkspace> predicate)
        {
            if (source == null) return null;
            if (predicate == null) throw new ArgumentNullException("predicate");

            IMMWorkspaceManagerMap manager = new MMWorkspaceManagerClass();
            IEnumWorkspaceEx workspaces = manager.GetMapWorkspaces(source);

            return workspaces.AsEnumerable().FirstOrDefault(workspace => predicate(workspace));
        }

        /// <summary>
        ///     Gets the workspace that is assigned the <paramref name="modelName" /> to the database level that resides within the
        ///     map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">The name of the database model.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace in the map that is assigned the database model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingDatabaseModelNameException"></exception>
        public static IWorkspace GetWorkspace(this IMap source, string modelName, bool throwException = true)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            IWorkspace workspace = source.GetWorkspace(o => o.IsAssignedDatabaseModelName(modelName));

            if (workspace == null && throwException)
                throw new MissingDatabaseModelNameException(modelName);

            return workspace;
        }

        #endregion
    }
}