using System;
using System.Collections.Generic;

using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.ITable" /> interface.
    /// </summary>
    public static class TableExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns a list of the model names that are assigned to the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the class model names assigned to the object class.
        /// </returns>
        public static IEnumerable<string> GetClassModelNames(this ITable source)
        {
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetClassModelNames();
        }

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
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetField(modelName, throwException);
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
            if (source == null) return -1;
            IObjectClass table = source as IObjectClass;
            if (table == null) return -1;

            return table.GetFieldIndex(modelName, throwException);
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
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetFieldIndexes(modelNames);
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
        public static IMMFieldManager GetFieldManager(this ITable source, int subtypeCode, IMMAuxiliaryFieldBuilder auxiliaryFieldBuilder = null)
        {
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetFieldManager(subtypeCode, auxiliaryFieldBuilder);
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
        public static Dictionary<IField, List<string>> GetFieldModelNames(this ITable source)
        {
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetFieldModelNames();
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
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetFieldName(modelName, throwException);
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
        public static IEnumerable<string> GetFieldNames(this ITable source, params string[] modelNames)
        {
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetFieldNames(modelNames);
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
        public static IEnumerable<string> GetFieldNames(this ITable source, string modelName)
        {
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetFieldNames(modelName);
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
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetFields(modelNames);
        }

        /// <summary>
        ///     Get the value of field that has been configured to be the primary display field.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IField" /> representing the field for the primary display.
        /// </returns>
        public static IField GetPrimaryDisplayField(this ITable source)
        {
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetPrimaryDisplayField();
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
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetRelationshipClass(relationshipRole, modelName, throwException);
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
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetRelationshipClass(relationshipRole, modelNames, throwException);
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
        public static IEnumerable<IRelationshipClass> GetRelationshipClasses(this ITable source, esriRelRole relationshipRole, params string[] modelNames)
        {
            if (source == null) return null;
            IObjectClass table = source as IObjectClass;
            if (table == null) return null;

            return table.GetRelationshipClasses(relationshipRole, modelNames);
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
            if (source == null) return false;
            IObjectClass table = source as IObjectClass;
            if (table == null) return false;

            return table.IsAssignedClassModelName(modelName);
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
            if (source == null) return false;
            IObjectClass table = source as IObjectClass;
            if (table == null) return false;

            return table.IsAssignedClassModelName(modelNames);
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
            if (source == null) return false;
            IObjectClass table = source as IObjectClass;
            if (table == null) return false;

            return table.IsAssignedFieldModelName(modelName);
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
            if (source == null) return false;
            IObjectClass table = source as IObjectClass;
            if (table == null) return false;

            return table.IsAssignedFieldModelName(modelNames);
        }

        #endregion
    }
}