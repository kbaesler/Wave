using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;

using Miner.Geodatabase;
using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IWorkspace" /> interface.
    /// </summary>
    public static class WorkspaceExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates a new version in the database.
        /// </summary>
        /// <param name="source">The workspace that creates the version.</param>
        /// <param name="name">The name of the version.</param>
        /// <param name="access">The access level of the version.</param>
        /// <param name="description">The description of the version.</param>
        /// <param name="deleteExisting">Flag to delete version if it exists</param>
        /// <returns>
        ///     Returns the <see cref="IVersion" /> representing the version that was created; otherwise <c>null</c>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     name
        ///     or
        ///     description
        /// </exception>
        public static IVersion CreateVersion(this IWorkspace source, string name, esriVersionAccess access, string description, bool deleteExisting)
        {
            if (source == null) return null;
            if (name == null) throw new ArgumentNullException("name");
            if (description == null) throw new ArgumentNullException("description");

            if (deleteExisting)
            {
                source.DeleteVersion(name);
            }

            // Create the version
            IMMVersioningUtils versioningUtils = new MMVersioningUtilsClass();
            return versioningUtils.CreateVersionFromBase(source, name, description, access);
        }

        /// <summary>
        ///     Deletes a version from the database.
        /// </summary>
        /// <param name="source">The workspace connection to the database.</param>
        /// <param name="name">The name of the version to delete.</param>
        /// <exception cref="ArgumentNullException">name</exception>
        public static void DeleteVersion(this IWorkspace source, string name)
        {
            try
            {
                if (source == null) return;
                if (name == null) throw new ArgumentNullException("name");

                IVersion version = source.FindVersion(name);
                if (version != null)
                {
                    version.Delete();
                }
            }
            catch (COMException ex)
            {
                // When the version has already been deleted.
                if (ex.ErrorCode != (int) fdoError.FDO_E_OBJECT_IS_DELETED)
                    throw;
            }
        }

        /// <summary>
        ///     Gets the version based on the name.
        /// </summary>
        /// <param name="source">The connection to the database that contains the version.</param>
        /// <param name="name">The name of the version to get.</param>
        /// <returns>
        ///     Returns a <see cref="IVersion" /> representing the version with the name; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">name</exception>
        /// <remarks>
        ///     The name must include the owner i.e. to get the DEFAULT version you would pass is SDE.DEFAULT.
        /// </remarks>
        public static IVersion FindVersion(this IWorkspace source, string name)
        {
            if (source == null) return null;
            if (name == null) throw new ArgumentNullException("name");

            IMMVersioningUtils versionUtils = new MMVersioningUtilsClass();
            return versionUtils.FindVersion(source, name);
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
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IFeatureClass GetFeatureClass(this IWorkspace source, string modelName, bool throwException = true)
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
        ///     Returns all of the feature classes that have been assigned the <paramref name="modelName" /> in the given
        ///     workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the feature classes from the input source.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<IFeatureClass> GetFeatureClasses(this IWorkspace source, string modelName)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            IEnumFeatureClass list = ModelNameManager.Instance.FeatureClassesFromModelNameWS(source, modelName);
            return list.AsEnumerable();
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
        public static IEnumerable<IFeatureClass> GetFeatureClasses(this IWorkspace source, params string[] modelNames)
        {
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            foreach (var modelName in modelNames)
            {
                IEnumFeatureClass list = ModelNameManager.Instance.FeatureClassesFromModelNameWS(source, modelName);
                foreach (var o in list.AsEnumerable())
                    yield return o;
            }
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
        public static IObjectClass GetObjectClass(this IWorkspace source, string modelName, bool throwException = true)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            var list = source.GetObjectClasses(modelName);
            var table = list.FirstOrDefault();

            if (table == null && throwException)
                throw new MissingClassModelNameException(modelName);

            return table;
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
        public static IEnumerable<IObjectClass> GetObjectClasses(this IWorkspace source, string modelName)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            IMMEnumObjectClass list = ModelNameManager.Instance.ObjectClassesFromModelNameWS(source, modelName);
            return list.AsEnumerable();
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
        public static IEnumerable<IObjectClass> GetObjectClasses(this IWorkspace source, params string[] modelNames)
        {
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            foreach (var modelName in modelNames)
            {
                IMMEnumObjectClass list = ModelNameManager.Instance.ObjectClassesFromModelNameWS(source, modelName);
                foreach (var o in list.AsEnumerable())
                    yield return o;
            }
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
        public static ITable GetTable(this IWorkspace source, string modelName, bool throwException = true)
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
        ///     Returns all of the tables that have been assigned the <paramref name="modelName" /> in the given workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelName">The model names.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the tables from the input source.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<ITable> GetTables(this IWorkspace source, string modelName)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            IMMEnumTable list = ModelNameManager.Instance.TablesFromModelNameWS(source, modelName);
            return list.AsEnumerable();
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
        public static IEnumerable<ITable> GetTables(this IWorkspace source, params string[] modelNames)
        {
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            foreach (var modelName in modelNames)
            {
                IMMEnumTable list = ModelNameManager.Instance.TablesFromModelNameWS(source, modelName);
                foreach (var o in list.AsEnumerable())
                    yield return o;
            }
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the database model names specified in the
        ///     <paramref name="modelNames" /> array.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <param name="modelNames">The database model names.</param>
        /// <returns>
        ///     Returns <c>true</c> if the workspace contains any of the database model names; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static bool IsAssignedDatabaseModelName(this IWorkspace source, params string[] modelNames)
        {
            if (source == null) return false;
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            IWorkspaceExtensionManager manager = source as IWorkspaceExtensionManager;
            if (manager != null)
            {
                UID uid = new UIDClass();
                uid.Value = "{54148E70-336D-11D5-9AB3-0001031AE963}"; // MMWorkspaceExtension

                IWorkspaceExtension ext = manager.FindExtension(uid);
                if (ext != null)
                {
                    IMMModelNameInfo modelNameInfo = (IMMModelNameInfo) ext;
                    return modelNames.Any(modelNameInfo.ModelNameExists);
                }
            }

            return false;
        }


        /// <summary>
        ///     Determines whether the workspaces are the same database.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <param name="other">The other workspace.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the workspaces point to the same database; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool IsEqual(this IWorkspace source, IWorkspace other)
        {
            if (source == null) return false;
            if (other == null) throw new ArgumentNullException("other");

            IMMWorkspaceManager manager = new MMWorkspaceManagerClass();
            return manager.IsSameDatabase(source, other);
        }

        #endregion
    }
}