using System;
using System.Collections.Generic;
using System.ComponentModel;

using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.ITable" /> interface.
    /// </summary>
    public static class TableAsyncExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns a list of the model names that are assigned to the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the class model names assigned to the object class.
        /// </returns>
        public static IEnumerable<string> GetClassModelNamesAsync(this ITable source)
        {
            return Task.Wait(() => source.GetClassModelNames());
        }


        /// <summary>
        ///     Gets a dictionary of the fields that are assigned the <paramref name="modelNames" /> organized by the model name
        ///     followed by the field indexes.
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        ///     Returns the  <see cref="Dictionary{Key, Value}" /> representing the field model name for the field indexes.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static Dictionary<string, List<int>> GetFieldIndexesAsync(this ITable source, params string[] modelNames)
        {
            return Task.Wait(() => source.GetFieldIndexes(modelNames));
        }

        /// <summary>
        ///     Gets the field manager for the specified <paramref name="source" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="subtypeCode">The subtype code.</param>
        /// <returns>
        ///     Returns the <see cref="IMMFieldManager" /> representing the properties for the class.
        /// </returns>
        public static IMMFieldManager GetFieldManagerAsync(this ITable source, int subtypeCode)
        {
            return Task.Wait(() => source.GetFieldManager(subtypeCode));
        }

        /// <summary>
        ///     Gets the field manager for the specified <paramref name="source" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="subtypeCode">The subtype code.</param>
        /// <param name="auxiliaryFieldBuilder">The auxiliary field builder.</param>
        /// <returns>
        ///     Returns the <see cref="IMMFieldManager" /> representing the properties for the class.
        /// </returns>
        public static IMMFieldManager GetFieldManagerAsync(this ITable source, int subtypeCode, IMMAuxiliaryFieldBuilder auxiliaryFieldBuilder)
        {
            return Task.Wait(() => source.GetFieldManager(subtypeCode, auxiliaryFieldBuilder));
        }

        /// <summary>
        ///     Returns a dictionary of the fields and model names that are assigned to the field for the specified
        ///     <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{Key, Value}" /> representing the model names assigned to the field.
        /// </returns>
        /// <exception cref="ArgumentNullException">field</exception>
        public static Dictionary<IField, List<string>> GetFieldModelNamesAsync(this ITable source)
        {
            return Task.Wait(() => source.GetFieldModelNames());
        }

        /// <summary>
        ///     Returns a list of the name of the fields that are assigned the specified <paramref name="modelName" /> on the
        ///     particiular <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="modelName">The field model name.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the name of the fields that are assigned the field model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static IEnumerable<string> GetFieldNamesAsync(this ITable source, string modelName)
        {
            return Task.Wait(() => source.GetFieldNames(modelName));
        }

        /// <summary>
        ///     Returns a list of the name of the fields that are assigned the specified <paramref name="modelNames" /> on the
        ///     particiular <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the name of the fields that are assigned the field model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static IEnumerable<string> GetFieldNamesAsync(this ITable source, params string[] modelNames)
        {
            return Task.Wait(() => source.GetFieldNames(modelNames));
        }

        /// <summary>
        ///     Gets all of the fields that has been assigned the <paramref name="modelNames" /> that is within the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class to check for model names</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        ///     Returns the  <see cref="IEnumerable{IField}" /> that has been assigned the model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static IEnumerable<IField> GetFieldsAsync(this ITable source, params string[] modelNames)
        {
            return Task.Wait(() => source.GetFields(modelNames));
        }

        /// <summary>
        ///     Finds the <see cref="IRelationshipClass" /> using the specified <paramref name="source" /> and
        ///     <paramref name="relationshipRole" /> that has been assigned the <paramref name="modelName" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="relationshipRole">The relationship role.</param>
        /// <param name="modelName">The class model name.</param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> representing the relationship assigned any of
        ///     the class model names, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IRelationshipClass GetRelationshipClassAsync(this ITable source, esriRelRole relationshipRole, string modelName)
        {
            return Task.Wait(() => source.GetRelationshipClass(relationshipRole, modelName));
        }

        /// <summary>
        ///     Finds the <see cref="IRelationshipClass" /> using the specified <paramref name="source" /> and
        ///     <paramref name="relationshipRole" /> that has been assigned the <paramref name="modelName" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="relationshipRole">The relationship role.</param>
        /// <param name="modelName">The class model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> representing the relationship assigned any of
        ///     the class model names, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IRelationshipClass GetRelationshipClassAsync(this ITable source, esriRelRole relationshipRole, string modelName, bool throwException)
        {
            return Task.Wait(() => source.GetRelationshipClass(relationshipRole, modelName, throwException));
        }

        /// <summary>
        ///     Finds the <see cref="IRelationshipClass" /> using the specified <paramref name="source" /> and
        ///     <paramref name="relationshipRole" /> that has been assigned any of the <paramref name="modelNames" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="relationshipRole">The relationship role.</param>
        /// <param name="modelNames">The class model names.</param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> representing the relationship assigned any of
        ///     the class model names, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IRelationshipClass GetRelationshipClassAsync(this ITable source, esriRelRole relationshipRole, string[] modelNames)
        {
            return Task.Wait(() => source.GetRelationshipClass(relationshipRole, modelNames));
        }

        /// <summary>
        ///     Finds the <see cref="IRelationshipClass" /> using the specified <paramref name="source" /> and
        ///     <paramref name="relationshipRole" /> that has been assigned any of the <paramref name="modelNames" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="relationshipRole">The relationship role.</param>
        /// <param name="modelNames">The class model names.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> representing the relationship assigned any of
        ///     the class model names, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IRelationshipClass GetRelationshipClassAsync(this ITable source, esriRelRole relationshipRole, string[] modelNames, bool throwException)
        {
            return Task.Wait(() => source.GetRelationshipClass(relationshipRole, modelNames, throwException));
        }

        /// <summary>
        ///     Finds all of the relationship classes using the specified <paramref name="source" /> and
        ///     <paramref name="relationshipRole" /> that has been assigned
        ///     any of the <paramref name="modelNames" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="relationshipRole">The relationship role.</param>
        /// <param name="modelNames">The class model names.</param>
        /// <returns>
        ///     Returns a
        ///     <see cref="T:IList{ESRI.ArcGIS.Geodatabase.IRelationshipClass}" />
        ///     representing those relationships that are assigned one or more of the class model names.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static IEnumerable<IRelationshipClass> GetRelationshipClassesAsync(this ITable source, esriRelRole relationshipRole, params string[] modelNames)
        {
            return Task.Wait(() => source.GetRelationshipClasses(relationshipRole, modelNames));
        }

        #endregion
    }
}
