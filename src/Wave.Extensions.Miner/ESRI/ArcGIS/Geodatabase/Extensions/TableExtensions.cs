using System.Collections.Generic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.ITable" /> interface.
    /// </summary>
    public static class TableExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Finds the <see cref="IField" /> that has been assigned the <paramref name="modelName" /> that is within the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class to check for model names</param>
        /// <param name="modelName">The field model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="IField" /> representing the field that has been assigned the field model name.
        /// </returns>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static IField GetField(this ITable source, string modelName, bool throwException = true)
        {
            return ((IObjectClass) source).GetField(modelName, throwException);
        }

        /// <summary>
        ///     Finds index of the <see cref="IField" /> that has been assigned the <paramref name="modelName" /> that is within
        ///     the specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="modelName">The field modelname.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns the  <see cref="int" /> representing the index of the field assigned the model name.
        /// </returns>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static int GetFieldIndex(this ITable source, string modelName, bool throwException = true)
        {
            return ((IObjectClass) source).GetFieldIndex(modelName, throwException);
        }

        /// <summary>
        ///     Gets a dictionary of the fields that are assigned the <paramref name="modelNames" /> organized by the model name
        ///     followed by the field indexes.
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        ///     Returns the  <see cref="Dictionary{TKey,TValue}" /> representing the field model name for the field indexes.
        /// </returns>
        public static Dictionary<string, List<int>> GetFieldIndexes(this ITable source, params string[] modelNames)
        {
            return ((IObjectClass) source).GetFieldIndexes(modelNames);
        }

        /// <summary>
        ///     Finds the name of the field that has been assigned the <paramref name="modelName" /> that is within the specified
        ///     <paramref name="source" />.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="modelName">The field model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the field that is assigned the field model name.
        /// </returns>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static string GetFieldName(this ITable source, string modelName, bool throwException = true)
        {
            return ((IObjectClass) source).GetFieldName(modelName, throwException);
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
        public static IEnumerable<IField> GetFields(this ITable source, params string[] modelNames)
        {
            return ((IObjectClass) source).GetFields(modelNames);
        }

        /// <summary>
        ///     Finds the <see cref="IRelationshipClass" /> using the specified <paramref name="source" /> and
        ///     <paramref name="relationshipRole" /> that has been assigned
        ///     the <paramref name="modelName" />.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="relationshipRole">The relationship role.</param>
        /// <param name="modelName">The class model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> representing the relationship assigned any of
        ///     the class model names.
        /// </returns>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IRelationshipClass GetRelationshipClass(this ITable source, esriRelRole relationshipRole, string modelName, bool throwException = true)
        {
            return ((IObjectClass) source).GetRelationshipClass(relationshipRole, modelName, throwException);
        }

        /// <summary>
        ///     Finds the <see cref="IRelationshipClass" /> using the specified <paramref name="source" /> and
        ///     <paramref name="relationshipRole" /> that has been assigned
        ///     any of the <paramref name="modelNames" />.
        /// </summary>
        /// <param name="source">The table.</param>
        /// <param name="relationshipRole">The relationship role.</param>
        /// <param name="modelNames">The class model names.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> representing the relationship assigned any of
        ///     the class model names.
        /// </returns>
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IRelationshipClass GetRelationshipClass(this ITable source, esriRelRole relationshipRole, string[] modelNames, bool throwException = true)
        {
            return ((IObjectClass) source).GetRelationshipClass(relationshipRole, modelNames, throwException);
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the class model names specified in the
        ///     <paramref name="modelName" /> parameter.
        /// </summary>
        /// <param name="source">The table to check for model names</param>
        /// <param name="modelName">The class model name.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if table contains any of the class model name; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool IsAssignedClassModelName(this ITable source, string modelName)
        {
            return ((IObjectClass) source).IsAssignedClassModelName(modelName);
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the class model names specified in the
        ///     <paramref name="modelNames" /> array.
        /// </summary>
        /// <param name="source">The table to check for model names</param>
        /// <param name="modelNames">The class model name.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if table contains any of the class model name; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool IsAssignedClassModelName(this ITable source, params string[] modelNames)
        {
            return ((IObjectClass) source).IsAssignedClassModelName(modelNames);
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the field model names specified in the
        ///     <paramref name="modelName" /> parameter.
        /// </summary>
        /// <param name="source">The table to check for model names</param>
        /// <param name="modelName">The field model names.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if table contains any of the field model name; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool IsAssignedFieldModelName(this ITable source, string modelName)
        {
            return ((IObjectClass) source).IsAssignedFieldModelName(modelName);
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the field model names specified in the
        ///     <paramref name="modelNames" /> array.
        /// </summary>
        /// <param name="source">The table to check for model names</param>
        /// <param name="modelNames">The field model names.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if table contains any of the field model name; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool IsAssignedFieldModelName(this ITable source, params string[] modelNames)
        {
            return ((IObjectClass) source).IsAssignedFieldModelName(modelNames);
        }

        #endregion
    }
}