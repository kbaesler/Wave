#if NET45
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Carto
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMap" /> interface.
    /// </summary>
    public static class MapAsyncExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns the feature class that is assigned the <paramref name="modelName" /> that is in the map.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="modelName">The class model name.</param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IFeatureClass" /> representing the feature class that is assigned
        ///     the model name.
        /// </returns>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static Task<IFeatureClass> GetFeatureClassAsync(this IMap source, string modelName)
        {
            return Task.Run(() => source.GetFeatureClass(modelName));
        }

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
        public static Task<IFeatureClass> GetFeatureClassAsync(this IMap source, string modelName, bool throwException)
        {
            return Task.Run(() => source.GetFeatureClass(modelName, throwException));
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
        public static Task<IEnumerable<IFeatureClass>> GetFeatureClassesAsync(this IMap source, string modelName)
        {
            return Task.Run(() => source.GetFeatureClasses(modelName));
        }

        /// <summary>
        ///     Returns the layer that is assigned the <paramref name="modelName" /> that resides the map.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="modelName">The class model name.</param>
        /// <returns>
        ///     Returns the <see cref="ESRI.ArcGIS.Carto.IFeatureLayer" /> representing the layer that is assigned the model
        ///     name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static Task<IFeatureLayer> GetFeatureLayerAsync(this IMap source, string modelName)
        {
            return Task.Run(() => source.GetFeatureLayer(modelName));
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
        public static Task<IFeatureLayer> GetFeatureLayerAsync(this IMap source, string modelName, bool throwException)
        {
            return Task.Run(() => source.GetFeatureLayer(modelName, throwException));
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
        public static Task<IEnumerable<IFeatureLayer>> GetFeatureLayersAsync(this IMap source, string modelName)
        {
            return Task.Run(() => source.GetFeatureLayers(modelName));
        }

        /// <summary>
        ///     Returns the table that is assigned the <paramref name="modelName" /> that resides within map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">The name of the class model name.</param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.ITable" /> representing the table that is assigned the model
        ///     name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static Task<ITable> GetTableAsync(this IMap source, string modelName)
        {
            return Task.Run(() => source.GetTable(modelName));
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
        public static Task<ITable> GetTableAsync(this IMap source, string modelName, bool throwException)
        {
            return Task.Run(() => source.GetTable(modelName, throwException));
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
        public static Task<IEnumerable<ITable>> GetTablesAsync(this IMap source, string modelName)
        {
            return Task.Run(() => source.GetTables(modelName));
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
        public static Task<IWorkspace> GetWorkspaceAsync(this IMap source, Predicate<IWorkspace> predicate)
        {
            return Task.Run(() => source.GetWorkspace(predicate));
        }

        /// <summary>
        ///     Gets the workspace that is assigned the <paramref name="modelName" /> to the database level that resides within the
        ///     map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">The name of the database model.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace in the map that is assigned the database model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingDatabaseModelNameException"></exception>
        public static Task<IWorkspace> GetWorkspaceAsync(this IMap source, string modelName)
        {
            return Task.Run(() => source.GetWorkspace(modelName));
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
        public static Task<IWorkspace> GetWorkspaceAsync(this IMap source, string modelName, bool throwException)
        {
            return Task.Run(() => source.GetWorkspace(modelName, throwException));
        }

        #endregion
    }
}
#endif