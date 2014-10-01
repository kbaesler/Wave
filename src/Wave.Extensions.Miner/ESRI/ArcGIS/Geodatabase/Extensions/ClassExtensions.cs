using System.Collections.Generic;
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
        ///     Assigns the class model names to the specified <paramref name="source" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelNames">The model names.</param>
        public static void AddClassModelName(this IObjectClass source, params string[] modelNames)
        {
            foreach (var modelName in modelNames)
                ModelNameManager.Instance.AddClassModelName(source, modelName);
        }

        /// <summary>
        ///     Assigns the field model names to the specified <paramref name="field" /> on the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="field">The field.</param>
        /// <param name="modelNames">The model names.</param>
        public static void AddFieldModelName(this IObjectClass source, IField field, params string[] modelNames)
        {
            foreach (var modelName in modelNames)
                ModelNameManager.Instance.AddFieldModelName(source, field, modelName);
        }


        /// <summary>
        ///     Returns a list of the model names that are assigned to the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the class model names assigned to the object class.
        /// </returns>
        public static IEnumerable<string> GetClassModelNames(this IObjectClass source)
        {
            IEnumBSTR names = ModelNameManager.Instance.ClassModelNames(source);
            return names.AsEnumerable();
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
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static IField GetField(this IObjectClass source, string modelName, bool throwException = true)
        {
            IField field = ModelNameManager.Instance.FieldFromModelName(source, modelName);

            if (field == null && throwException)
                throw new MissingFieldModelNameException(source, modelName);

            return field;
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
        public static int GetFieldIndex(this IObjectClass source, string modelName, bool throwException = true)
        {
            IField field = source.GetField(modelName, throwException);
            return (field != null) ? source.FindField(field.Name) : -1;
        }

        /// <summary>
        ///     Gets the field manager for the specified <paramref name="source" />
        /// </summary>
        /// <param name="source">The class.</param>
        /// <param name="subtype">The subtype.</param>
        /// <returns>
        ///     Returns the <see cref="IMMFieldManager" /> representing the properties for the class.
        /// </returns>
        public static IMMFieldManager GetFieldManager(this IObjectClass source, int subtype)
        {
            IMMObjectClassBuilder builder = new MMObjectClassBuilderClass();
            builder.Build(source, subtype);

            IMMFieldManager fieldManager = new MMFieldManagerClass();
            fieldManager.Build((IMMFieldBuilder)builder, null);

            return fieldManager;
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
        public static IEnumerable<string> GetFieldModelNames(this IObjectClass source, IField field)
        {
            IEnumBSTR names = ModelNameManager.Instance.FieldModelNames(source, field);
            return names.AsEnumerable();
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
        /// <exception cref="MissingFieldModelNameException"></exception>
        public static string GetFieldName(this IObjectClass source, string modelName, bool throwException = true)
        {
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
        public static IEnumerable<string> GetFieldNames(this IObjectClass source, string modelName)
        {
            IEnumBSTR names = ModelNameManager.Instance.FieldNamesFromModelName(source, modelName);
            return names.AsEnumerable();
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
        public static IEnumerable<string> GetFieldNames(this IObjectClass source, params string[] modelNames)
        {
            foreach (var modelName in modelNames)
            {
                IEnumBSTR names = ModelNameManager.Instance.FieldNamesFromModelName(source, modelName);
                foreach (var name in names.AsEnumerable())
                    yield return name;
            }
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
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IRelationshipClass GetRelationshipClass(this IObjectClass source, esriRelRole relationshipRole, string modelName, bool throwException = true)
        {
            return source.GetRelationshipClass(relationshipRole, new[] { modelName }, throwException);
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
        /// <exception cref="MissingClassModelNameException"></exception>
        public static IRelationshipClass GetRelationshipClass(this IObjectClass source, esriRelRole relationshipRole, string[] modelNames, bool throwException = true)
        {
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
        ///     <see>
        ///         <cref>T:IList{ESRI.ArcGIS.Geodatabase.IRelationshipClass}</cref>
        ///     </see>
        ///     representing those relationships that are assigned one or more of the class model names.
        /// </returns>
        public static IEnumerable<IRelationshipClass> GetRelationshipClasses(this IObjectClass source, esriRelRole relationshipRole, params string[] modelNames)
        {
            IEnumRelationshipClass enumClasses = source.RelationshipClasses[relationshipRole];
            foreach (IRelationshipClass relClass in enumClasses.AsEnumerable())
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

                    case esriRelRole.esriRelRoleOrigin:
                        if (relClass.DestinationClass.IsAssignedClassModelName(modelNames))
                            yield return relClass;

                        break;
                }
            }
        }


        /// <summary>
        ///     Gets the name of the owner or schema name of the table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the owner.
        /// </returns>
        public static string GetSchemaName(this IObjectClass source)
        {
            return ((ITable)source).GetSchemaName();
        }

        /// <summary>
        ///     Finds the code of the subtype that has the specified <paramref name="subtypeName" />.
        /// </summary>
        /// <param name="source">The object class.</param>
        /// <param name="subtypeName">Name of the subtype.</param>
        /// <returns>Returns a <see cref="int" /> representing the code of the subtype; otherwise <c>-1</c>.</returns>
        public static int GetSubtypeCode(this IObjectClass source, string subtypeName)
        {
            return ((ITable)source).GetSubtypeCode(subtypeName);
        }

        /// <summary>
        ///     Gets the name of the table (without the owner or schema name).
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the table.
        /// </returns>
        public static string GetTableName(this IObjectClass source)
        {
            return ((ITable)source).GetTableName();
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
        public static bool IsAssignedClassModelName(this IObjectClass source, string modelName)
        {
            return ModelNameManager.Instance.ContainsClassModelName(source, modelName);
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
        public static bool IsAssignedClassModelName(this IObjectClass source, params string[] modelNames)
        {
            if (modelNames == null || source == null || modelNames.Length == 0) return false;

            return modelNames.Any(name => ModelNameManager.Instance.ContainsClassModelName(source, name));
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
        public static bool IsAssignedFieldModelName(this IObjectClass source, string modelName)
        {
            return ModelNameManager.Instance.FieldFromModelName(source, modelName) != null;
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
        public static bool IsAssignedFieldModelName(this IObjectClass source, params string[] modelNames)
        {
            if (modelNames == null || source == null || modelNames.Length == 0) return false;

            return modelNames.Any(name => ModelNameManager.Instance.FieldFromModelName(source, name) != null);
        }

        /// <summary>
        ///     Removes the class model names from the specified <paramref name="source" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="modelNames">The model names.</param>
        public static void RemoveClassModelName(this IObjectClass source, params string[] modelNames)
        {
            foreach (var modelName in modelNames)
                ModelNameManager.Instance.RemoveClassModelName(source, modelName);
        }

        /// <summary>
        ///     Removes the field model names from the specified <paramref name="field" /> on the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="field">The field.</param>
        /// <param name="modelNames">The model names.</param>
        public static void RemoveFieldModelName(this IObjectClass source, IField field, params string[] modelNames)
        {
            foreach (var modelName in modelNames)
                ModelNameManager.Instance.RemoveFieldModelName(source, field, modelName);
        }

        #endregion
    }
}