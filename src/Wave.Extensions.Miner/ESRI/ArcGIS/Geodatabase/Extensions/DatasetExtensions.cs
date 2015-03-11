using System;
using System.Linq;

using Miner.Geodatabase;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    /// Provides extension methods for the <see cref="IDataset"/> and <see cref="IFeatureDataset"/> interfaces.
    /// </summary>
    public static class DatasetExtensions
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
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IFeatureClass GetFeatureClass(this IFeatureDataset source, string modelName, bool throwException = true)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            var list = ModelNameManager.Instance.FeatureClassesFromModelNameWS(source.Workspace, modelName);
            var table = list.AsEnumerable().FirstOrDefault();
            
            if (table == null && throwException)
                throw new MissingClassModelNameException(modelName);

            return table;
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any feature class assigned the class model name.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <param name="modelName">The class model name.</param>
        /// <returns>
        ///     Returns <c>true</c> if the dataset contains a feature class assigned the class model name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static bool IsAssignedClassModelName(this IDataset source, string modelName)
        {
            if (source == null) return false;
            if (modelName == null) throw new ArgumentNullException("modelName");

            var list = ModelNameManager.Instance.FeatureClassesFromModelNameDS(source, modelName);
            return list.AsEnumerable().Any();
        }

        #endregion
    }
}