#if NET45
using System;
using System.Threading.Tasks;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IDataset" /> and <see cref="IFeatureDataset" /> interfaces.
    /// </summary>
    public static class DatasetAsyncExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Finds the <see cref="IFeatureClass" /> that has been assigned the <paramref name="modelName" /> that is within the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class that has been assigned the class model name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static Task<IFeatureClass> GetFeatureClassAsync(this IFeatureDataset source, string modelName)
        {
            return Task.Run(() => source.GetFeatureClass(modelName));
        }

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
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static Task<IFeatureClass> GetFeatureClassAsync(this IFeatureDataset source, string modelName, bool throwException)
        {
            return Task.Run(() => source.GetFeatureClass(modelName, throwException));
        }

        #endregion
    }
}

#endif