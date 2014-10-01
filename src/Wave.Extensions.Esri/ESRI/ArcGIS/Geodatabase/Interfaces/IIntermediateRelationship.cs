using System.Collections.Generic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides interaction with the intermediate relationship class table that created when a many-to-many
    ///     cardinality or attributed relationship class created.
    /// </summary>
    public interface IIntermediateRelationship
    {
        #region Public Methods

        /// <summary>
        ///     Creates record in the intermediate table for the relationship that is linked to the
        ///     <paramref name="originObject" /> and <paramref name="destinationObject" />.
        /// </summary>
        /// <param name="originObject">The origin object.</param>
        /// <param name="destinationObject">The destination object.</param>
        /// <returns>
        ///     Returns the <see cref="ESRI.ArcGIS.Geodatabase.IIntermediateRow" /> interface for the new row.
        /// </returns>
        IIntermediateRow Create(IObject originObject, IObject destinationObject);

        /// <summary>
        ///     Removes those rows from the intermediate relationship table that have a relationship with the
        ///     <paramref name="relationshipObject" />
        ///     have the <paramref name="fieldName" /> attribute value of <paramref name="fieldValue" />.
        /// </summary>
        /// <param name="relationshipObject">The relationship object.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value. (assumes a string character).</param>
        /// <returns>
        ///     Returns the <see cref="System.Int32" /> representing the number of records that have been deleted.
        /// </returns>
        int Delete(IObject relationshipObject, string fieldName, string fieldValue);

        /// <summary>
        ///     Removes those rows from the intermediate relationship table that have a relationship with the
        ///     <paramref name="destinationObject" />
        ///     have the <paramref name="fieldName" /> attribute value of <paramref name="fieldValue" /> but do not have a
        ///     relationship
        ///     with the <paramref name="originObject" />.
        /// </summary>
        /// <param name="originObject">The origin object.</param>
        /// <param name="destinationObject">The destination object.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value. (assumes a string character).</param>
        /// <returns>
        ///     Returns the <see cref="System.Int32" /> representing the number of records that have been deleted.
        /// </returns>
        int Delete(IObject originObject, IObject destinationObject, string fieldName, string fieldValue);

        /// <summary>
        ///     Gets all of the rows that have associations with the <paramref name="relationshipObject" /> object.
        /// </summary>
        /// <param name="relationshipObject">The relationship object.</param>
        /// <returns>
        ///     A list of <see cref="ESRI.ArcGIS.Geodatabase.IIntermediateRow" /> interfaces.
        /// </returns>
        IList<IIntermediateRow> GetRows(IObject relationshipObject);

        /// <summary>
        ///     Gets all of the rows that have associations with the <paramref name="relationshipObject" /> object
        ///     that have a <paramref name="fieldName" /> with the <paramref name="fieldValue" />.
        /// </summary>
        /// <param name="relationshipObject">The relationship object.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>
        ///     A list of <see cref="ESRI.ArcGIS.Geodatabase.IIntermediateRow" /> interfaces.
        /// </returns>
        IList<IIntermediateRow> GetRows(IObject relationshipObject, string fieldName, string fieldValue);

        /// <summary>
        ///     Gets all of the rows that have associations between the <paramref name="originObject" /> and
        ///     <paramref name="destinationObject" /> objects.
        /// </summary>
        /// <param name="originObject">The origin object.</param>
        /// <param name="destinationObject">The destination object.</param>
        /// <returns>
        ///     A list of <see cref="ESRI.ArcGIS.Geodatabase.IIntermediateRow" /> interfaces.
        /// </returns>
        IList<IIntermediateRow> GetRows(IObject originObject, IObject destinationObject);

        /// <summary>
        ///     Gets all of the rows that have associations between the <paramref name="originObject" /> and
        ///     <paramref name="destinationObject" /> objects
        ///     that have a <paramref name="fieldName" /> with the <paramref name="fieldValue" />.
        /// </summary>
        /// <param name="originObject">The origin object.</param>
        /// <param name="destinationObject">The destination object.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>
        ///     A list of <see cref="ESRI.ArcGIS.Geodatabase.IIntermediateRow" /> interfaces.
        /// </returns>
        IList<IIntermediateRow> GetRows(IObject originObject, IObject destinationObject, string fieldName, string fieldValue);

        #endregion
    }
}