using System;
using System.Collections.Generic;
using System.ComponentModel;

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
        public static IFeatureClass GetFeatureClassAsync(this IMap source, string modelName)
        {
            return Task.Wait(() => source.GetFeatureClass(modelName, true));
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
        public static IFeatureClass GetFeatureClassAsync(this IMap source, string modelName, bool throwException)
        {
            return Task.Wait(() => source.GetFeatureClass(modelName, throwException));
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
        public static IEnumerable<IFeatureClass> GetFeatureClassesAsync(this IMap source, string modelName)
        {
            return Task.Wait(() => source.GetFeatureClasses(modelName));
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
        public static IFeatureLayer GetFeatureLayerAsync(this IMap source, string modelName)
        {
            return Task.Wait(() => source.GetFeatureLayer(modelName, true));
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
        public static IFeatureLayer GetFeatureLayerAsync(this IMap source, string modelName, bool throwException)
        {
            return Task.Wait(() => source.GetFeatureLayer(modelName, throwException));
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
        public static IEnumerable<IFeatureLayer> GetFeatureLayersAsync(this IMap source, string modelName)
        {
            return Task.Wait(() => source.GetFeatureLayers(modelName));
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
        public static ITable GetTableAsync(this IMap source, string modelName)
        {
            return Task.Wait(() => source.GetTable(modelName));
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
        public static ITable GetTableAsync(this IMap source, string modelName, bool throwException)
        {
            return Task.Wait(() => source.GetTable(modelName, throwException));
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
        public static IEnumerable<ITable> GetTablesAsync(this IMap source, string modelName)
        {
            return Task.Wait(() => source.GetTables(modelName));
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
        public static IWorkspace GetWorkspaceAsync(this IMap source, Predicate<IWorkspace> predicate)
        {
            return Task.Wait(() => source.GetWorkspace(predicate));
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
        public static IWorkspace GetWorkspaceAsync(this IMap source, string modelName)
        {
            return Task.Wait(() => source.GetWorkspace(modelName));
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
        public static IWorkspace GetWorkspaceAsync(this IMap source, string modelName, bool throwException)
        {
            return Task.Wait(() => source.GetWorkspace(modelName, throwException));
        }

        #endregion
    }
}