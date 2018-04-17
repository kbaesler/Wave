using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ESRI.ArcGIS.esriSystem;

using Miner.Geodatabase;
using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IClass" /> and
    ///     <see cref="ESRI.ArcGIS.Geodatabase.IObjectClass" /> interfaces.
    /// </summary>
    public static class ClassExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns a list of the model names that are assigned to the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the class model names assigned to the object class.
        /// </returns>
        public static IEnumerable<string> GetClassModelNames(this IObjectClass source)
        {
            if (source == null) return null;

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                IEnumBSTR names = ModelNameManager.Instance.ClassModelNames(source);
                return names.AsEnumerable();
            }

            return null;
        }

        /// <summary>
        ///     Finds the <see cref="IField" /> that has been assigned the <paramref name="modelName" /> that is within the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class to check for model names</param>
        /// <param name="modelName">The field model name.</param>
        /// <returns>
        ///     Returns the  <see cref="ESRI.ArcGIS.Geodatabase.IField" /> that has been assigned the model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static IField GetField(this IObjectClass source, string modelName)
        {
            return source.GetField(modelName, true);
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
        ///     Returns the  <see cref="ESRI.ArcGIS.Geodatabase.IField" /> that has been assigned the model name.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static IField GetField(this IObjectClass source, string modelName, bool throwException)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                IField field = ModelNameManager.Instance.FieldFromModelName(source, modelName);

                if (field == null && throwException)
                    throw new MissingFieldModelNameException(source, modelName);

                return field;
            }

            return null;
        }

        /// <summary>
        ///     Finds index of the <see cref="IField" /> that has been assigned the <paramref name="modelName" /> that is within
        ///     the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class</param>
        /// <param name="modelName">The field model name.</param>
        /// <returns>
        ///     Returns the  <see cref="int" /> representing the index of the field assigned the model name.
        /// </returns>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static int GetFieldIndex(this IObjectClass source, string modelName)
        {
            return source.GetFieldIndex(modelName, true);
        }

        /// <summary>
        ///     Finds index of the <see cref="IField" /> that has been assigned the <paramref name="modelName" /> that is within
        ///     the
        ///     specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class</param>
        /// <param name="modelName">The field model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns the  <see cref="int" /> representing the index of the field assigned the model name.
        /// </returns>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static int GetFieldIndex(this IObjectClass source, string modelName, bool throwException)
        {
            if (source == null) return -1;
            if (modelName == null) throw new ArgumentNullException("modelName");

            IField field = source.GetField(modelName, throwException);
            return (field != null) ? source.FindField(field.Name) : -1;
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
        public static Dictionary<string, List<int>> GetFieldIndexes(this IObjectClass source, params string[] modelNames)
        {
            if (source == null) return null;
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            Dictionary<string, List<int>> indexes = new Dictionary<string, List<int>>(StringComparer.Create(CultureInfo.CurrentCulture, true));

            foreach (var modelName in modelNames)
            {
                var list = new List<int>();

                if (indexes.ContainsKey(modelName))
                    list = indexes[modelName];
                else
                    indexes.Add(modelName, list);

                list.AddRange(source.GetFields(modelName).Select(field => source.FindField(field.Name)));
            }

            return indexes;
        }

        /// <summary>
        ///     Gets the field manager for the specified <paramref name="source" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="subtypeCode">The subtype code.</param>
        /// <returns>
        ///     Returns the <see cref="IMMFieldManager" /> representing the properties for the class.
        /// </returns>
        public static IMMFieldManager GetFieldManager(this IObjectClass source, int subtypeCode)
        {
            return source.GetFieldManager(subtypeCode, null);
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
        public static IMMFieldManager GetFieldManager(this IObjectClass source, int subtypeCode, IMMAuxiliaryFieldBuilder auxiliaryFieldBuilder)
        {
            if (source == null) return null;

            IMMObjectClassBuilder builder = new MMObjectClassBuilderClass();
            builder.Build(source, subtypeCode);

            IMMFieldManager fieldManager = new MMFieldManagerClass();
            fieldManager.Build((IMMFieldBuilder) builder, auxiliaryFieldBuilder);

            return fieldManager;
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
        public static Dictionary<IField, List<string>> GetFieldModelNames(this IObjectClass source)
        {
            if (source == null) return null;

            Dictionary<IField, List<string>> list = new Dictionary<IField, List<string>>();

            foreach (var field in source.Fields.AsEnumerable())
            {
                var modelNames = source.GetFieldModelNames(field).ToList();
                if (modelNames.Any()) list.Add(field, modelNames);
            }

            return list;
        }

        /// <summary>
        ///     Returns a list of the model names that are assigned to the <paramref name="field" /> for the specified
        ///     <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the model names assigned to the field.
        /// </returns>
        /// <exception cref="ArgumentNullException">field</exception>
        public static IEnumerable<string> GetFieldModelNames(this IObjectClass source, IField field)
        {
            if (source == null) return null;
            if (field == null) throw new ArgumentNullException("field");

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                IEnumBSTR names = ModelNameManager.Instance.FieldModelNames(source, field);
                return names.AsEnumerable();
            }

            return null;
        }

        /// <summary>
        ///     Returns the name of the field that is assigned the <paramref name="modelName" /> on the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="modelName">The field model name.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the field, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static string GetFieldName(this IObjectClass source, string modelName)
        {
            return source.GetFieldName(modelName, true);
        }

        /// <summary>
        ///     Returns the name of the field that is assigned the <paramref name="modelName" /> on the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="modelName">The field model name.</param>
        /// <param name="throwException">
        ///     if set to <c>true</c> if an exception should be thrown when the model name is not
        ///     assigned.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the field, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static string GetFieldName(this IObjectClass source, string modelName, bool throwException)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            IField field = source.GetField(modelName, throwException);
            return (field != null) ? field.Name : null;
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
        public static IEnumerable<string> GetFieldNames(this IObjectClass source, string modelName)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                IEnumBSTR names = ModelNameManager.Instance.FieldNamesFromModelName(source, modelName);
                return names.AsEnumerable();
            }

            return null;
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
        public static IEnumerable<string> GetFieldNames(this IObjectClass source, params string[] modelNames)
        {
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                foreach (var modelName in modelNames)
                {
                    IEnumBSTR names = ModelNameManager.Instance.FieldNamesFromModelName(source, modelName);
                    foreach (var name in names.AsEnumerable())
                        yield return name;
                }
            }
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
        public static IEnumerable<IField> GetFields(this IObjectClass source, params string[] modelNames)
        {
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                foreach (var modelName in modelNames)
                {
                    var fields = ModelNameManager.Instance.FieldsFromModelName(source, modelName);
                    return fields.AsEnumerable();
                }
            }

            return new IField[] {};
        }

        /// <summary>
        ///     Get the value of field that has been configured to be the primary display field.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IField" /> representing the field for the primary display.
        /// </returns>
        public static IField GetPrimaryDisplayField(this IObjectClass source)
        {
            if (source == null) return null;

            IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
            if (configTopLevel == null) return null;

            return configTopLevel.GetPrimaryDisplayField(source);
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
        public static IRelationshipClass GetRelationshipClass(this IObjectClass source, esriRelRole relationshipRole, string modelName)
        {
            return source.GetRelationshipClass(relationshipRole, modelName, true);
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
        public static IRelationshipClass GetRelationshipClass(this IObjectClass source, esriRelRole relationshipRole, string modelName, bool throwException)
        {
            if (source == null) return null;
            if (modelName == null) throw new ArgumentNullException("modelName");

            return source.GetRelationshipClass(relationshipRole, new[] {modelName}, throwException);
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
        public static IRelationshipClass GetRelationshipClass(this IObjectClass source, esriRelRole relationshipRole, string[] modelNames)
        {
            return source.GetRelationshipClass(relationshipRole, modelNames, true);
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
        public static IRelationshipClass GetRelationshipClass(this IObjectClass source, esriRelRole relationshipRole, string[] modelNames, bool throwException)
        {
            if (source == null) return null;
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            IEnumerable<IRelationshipClass> list = source.GetRelationshipClasses(relationshipRole, modelNames);
            var relClass = list.FirstOrDefault();

            if (relClass == null && throwException)
                throw new MissingClassModelNameException(source, relationshipRole, modelNames);

            return relClass;
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
        public static IEnumerable<IRelationshipClass> GetRelationshipClasses(this IObjectClass source, esriRelRole relationshipRole, params string[] modelNames)
        {
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            IEnumRelationshipClass list = source.RelationshipClasses[relationshipRole];
            foreach (IRelationshipClass relClass in list.AsEnumerable())
            {
                switch (relationshipRole)
                {
                    case esriRelRole.esriRelRoleAny:
                        if (relClass.DestinationClass.IsAssignedClassModelName(modelNames) ||
                            relClass.OriginClass.IsAssignedClassModelName(modelNames))
                            yield return relClass;

                        break;

                    case esriRelRole.esriRelRoleDestination:
                        if (relClass.OriginClass.IsAssignedClassModelName(modelNames))
                            yield return relClass;

                        break;

                    default:
                        if (relClass.DestinationClass.IsAssignedClassModelName(modelNames))
                            yield return relClass;

                        break;
                }
            }
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the class model names specified in the
        ///     <paramref name="modelName" />.
        /// </summary>
        /// <param name="source">The object class to check for model names</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        ///     Returns <c>true</c> if object class contains any of the class model name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static bool IsAssignedClassModelName(this IObjectClass source, string modelName)
        {
            if (source == null) return false;
            if (modelName == null) throw new ArgumentNullException("modelName");

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                return ModelNameManager.Instance.ContainsClassModelName(source, modelName);
            }

            return false;
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the class model names specified in the
        ///     <paramref name="modelNames" /> array.
        /// </summary>
        /// <param name="source">The object class to check for model names</param>
        /// <param name="modelNames">The class model names.</param>
        /// <returns>
        ///     Returns <c>true</c> if object class contains any of the class model name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static bool IsAssignedClassModelName(this IObjectClass source, params string[] modelNames)
        {
            if (source == null) return false;
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                return modelNames.Any(name => ModelNameManager.Instance.ContainsClassModelName(source, name));
            }

            return false;
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the field model names specified in the
        ///     <paramref name="modelName" /> array.
        /// </summary>
        /// <param name="source">The object class to check for model names</param>
        /// <param name="modelName">The field model names.</param>
        /// <returns>
        ///     Returns <c>true</c> if object class contains any of the field model name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelName</exception>
        public static bool IsAssignedFieldModelName(this IObjectClass source, string modelName)
        {
            if (source == null) return false;
            if (modelName == null) throw new ArgumentNullException("modelName");

            if (ModelNameManager.Instance.CanReadModelNames(source))
            {
                return ModelNameManager.Instance.FieldFromModelName(source, modelName) != null;
            }

            return false;
        }

        /// <summary>
        ///     Determines if the <paramref name="field" /> on the <paramref name="source" /> contains any of the field model names
        ///     specified in the <paramref name="modelNames" /> array.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="field">The field.</param>
        /// <param name="modelNames">The model names.</param>
        /// <returns>
        ///     <c>true</c> if object class contains any of the field model name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static bool IsAssignedFieldModelName(this IObjectClass source, IField field, params string[] modelNames)
        {
            if (source == null) return false;
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            return modelNames.Any(name => ModelNameManager.Instance.ContainsFieldModelName(source, field, name));
        }

        /// <summary>
        ///     Determines if the <paramref name="source" /> contains any of the field model names specified in the
        ///     <paramref name="modelNames" /> array.
        /// </summary>
        /// <param name="source">The object class to check for model names</param>
        /// <param name="modelNames">The field model names.</param>
        /// <returns>
        ///     Returns <c>true</c> if object class contains any of the field model name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">modelNames</exception>
        public static bool IsAssignedFieldModelName(this IObjectClass source, params string[] modelNames)
        {
            if (source == null) return false;
            if (modelNames == null) throw new ArgumentNullException("modelNames");

            return modelNames.Any(name => ModelNameManager.Instance.FieldFromModelName(source, name) != null);
        }

        #endregion
    }
}