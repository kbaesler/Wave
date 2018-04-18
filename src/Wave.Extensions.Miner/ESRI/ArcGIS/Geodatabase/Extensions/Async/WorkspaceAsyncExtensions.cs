using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IWorkspace" /> interface.
    /// </summary>
    public static class WorkspaceAsyncExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Finds the <see cref="IFeatureClass" /> that has been assigned the <paramref name="modelName" /> that is within the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class that has been assigned the class model name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IFeatureClass GetFeatureClassAsync(this IWorkspace source, string modelName, bool throwException)
        {
            return Task.Wait(() => source.GetFeatureClass(modelName, throwException));
        }

        /// <summary>
        ///     Returns all of the feature classes that have been assigned the <paramref name="modelName" /> in the given
        ///     workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the feature classes from the input source.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<IFeatureClass> GetFeatureClassesAsync(this IWorkspace source, string modelName)
        {
            return Task.Wait(() => source.GetFeatureClasses(modelName));
        }

        /// <summary>
        ///     Returns all of the feature classes that have been assigned the <paramref name="modelNames" /> in the given
        ///     workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the feature classes from the input source.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static IEnumerable<IFeatureClass> GetFeatureClassesAsync(this IWorkspace source, params string[] modelNames)
        {
            return Task.Wait(() => source.GetFeatureClasses(modelNames));
        }

        /// <summary>
        ///     Finds the <see cref="IObjectClass" /> that has been assigned the <paramref name="modelName" /> that is within the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        ///     Returns a <see cref="IObjectClass" /> representing the object class that has been assigned the class model name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IObjectClass GetObjectClassAsync(this IWorkspace source, string modelName)
        {
            return Task.Wait(() => source.GetObjectClass(modelName));
        }

        /// <summary>
        ///     Finds the <see cref="IObjectClass" /> that has been assigned the <paramref name="modelName" /> that is within the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="IObjectClass" /> representing the object class that has been assigned the class model name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IObjectClass GetObjectClassAsync(this IWorkspace source, string modelName, bool throwException)
        {
            return Task.Wait(() => source.GetObjectClass(modelName, throwException));
        }

        /// <summary>
        ///     Returns all of the object classes that have been assigned the <paramref name="modelName" /> in the given
        ///     workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the object classes from the input source.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<IObjectClass> GetObjectClassesAsync(this IWorkspace source, string modelName)
        {
            return Task.Wait(() => source.GetObjectClasses(modelName));
        }

        /// <summary>
        ///     Returns all of the object classes that have been assigned the <paramref name="modelNames" /> in the given
        ///     workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the object classes from the input source.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<IObjectClass> GetObjectClassesAsync(this IWorkspace source, params string[] modelNames)
        {
            return Task.Wait(() => source.GetObjectClasses(modelNames));
        }
       
        /// <summary>
        ///     Finds the <see cref="ITable" /> that has been assigned the <paramref name="modelName" /> that is within the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the table that has been assigned the class model name, otherwise
        ///     <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static ITable GetTableAsync(this IWorkspace source, string modelName, bool throwException)
        {
            return Task.Wait(() => source.GetTable(modelName, throwException));
        }

        /// <summary>
        ///     Returns all of the tables that have been assigned the <paramref name="modelName" /> in the given workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">The model names.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the tables from the input source.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<ITable> GetTablesAsync(this IWorkspace source, string modelName)
        {
            return Task.Wait(() => source.GetTables(modelName));
        }

        /// <summary>
        ///     Returns all of the tables that have been assigned the <paramref name="modelNames" /> in the given workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the tables from the input source.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static IEnumerable<ITable> GetTablesAsync(this IWorkspace source, params string[] modelNames)
        {
            return Task.Wait(() => source.GetTables(modelNames));
        }

        #endregion
    }
}
