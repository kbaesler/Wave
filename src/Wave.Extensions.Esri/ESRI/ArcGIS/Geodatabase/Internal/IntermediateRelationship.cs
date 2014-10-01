using System;
using System.Collections.Generic;
using System.Globalization;

using ESRI.ArcGIS.ADF;

namespace ESRI.ArcGIS.Geodatabase.Internal
{
    /// <summary>
    ///     When a relationship class is created with many-to-many cardinality or with attributes an intermediate relationship
    ///     class table is created.
    ///     This table is used to map the associations between the origin and destination objects.
    ///     The intermediate table contains foreign key fields that are associated with the primary key values from the origin
    ///     and destination
    ///     feature classes and/or tables as well as any additional attributes.
    ///     Each row in the table associates one origin object with one destination object.
    /// </summary>
    internal class IntermediateRelationship : IIntermediateRelationship
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntermediateRelationship" /> class.
        /// </summary>
        /// <param name="relClass">The relationship class.</param>
        /// <exception cref="System.ArgumentException">
        ///     The relationship must either have attributes or many to many
        ///     cardinality.;relClass
        /// </exception>
        public IntermediateRelationship(IRelationshipClass relClass)
        {
            if (!CheckValidity(relClass))
                throw new ArgumentException(@"The relationship must either have attributes or many to many cardinality.", "relClass");

            this.RelationshipClass = relClass;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the relationship class.
        /// </summary>
        /// <value>
        ///     The relationship class.
        /// </value>
        protected IRelationshipClass RelationshipClass { get; private set; }

        #endregion

        #region IIntermediateRelationship Members

        /// <summary>
        ///     Creates record in the intermediate table for the relationship that is linked to the
        ///     <paramref name="originObject" /> and <paramref name="destinationObject" />.
        /// </summary>
        /// <param name="originObject">The origin object.</param>
        /// <param name="destinationObject">The destination object.</param>
        /// <returns>
        ///     Returns the <see cref="IntermediateRow" /> struct for the new row.
        /// </returns>
        public IIntermediateRow Create(IObject originObject, IObject destinationObject)
        {
            int originPrimaryIndex = originObject.Fields.FindField(this.RelationshipClass.OriginPrimaryKey);
            int destinationPrimaryIndex = destinationObject.Fields.FindField(this.RelationshipClass.DestinationPrimaryKey);

            string originForeignKey = TypeCast.Cast(originObject.get_Value(originPrimaryIndex), string.Empty);
            string destinationForeignKey = TypeCast.Cast(destinationObject.get_Value(destinationPrimaryIndex), string.Empty);

            ITable table = (ITable) this.RelationshipClass;
            int originForeignIndex = table.Fields.FindField(this.RelationshipClass.OriginForeignKey);
            int destinationForeignIndex = table.Fields.FindField(this.RelationshipClass.DestinationForeignKey);

            IRow row = table.CreateRow();
            row.set_Value(originForeignIndex, originForeignKey);
            row.set_Value(destinationForeignIndex, destinationForeignKey);

            return GetRow(row);
        }

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
        public int Delete(IObject relationshipObject, string fieldName, string fieldValue)
        {
            IQueryFilter filter = ConstructFilter(relationshipObject);
            if (filter == null) return 0;

            filter.WhereClause += string.Format(CultureInfo.InvariantCulture, " AND {0} = '{1}'", fieldName, fieldValue);

            return Delete(filter);
        }

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
        public int Delete(IObject originObject, IObject destinationObject, string fieldName, string fieldValue)
        {
            int originIndex = originObject.Fields.FindField(this.RelationshipClass.OriginPrimaryKey);
            int destinationIndex = destinationObject.Fields.FindField(this.RelationshipClass.OriginPrimaryKey);

            object destinationForeignKey = destinationObject.get_Value(destinationIndex);
            object originPrimaryKey = originObject.get_Value(originIndex);

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = string.Format(CultureInfo.InvariantCulture, "{0} = '{1}' AND {2} <> '{3}' AND {4} = '{5}'",
                this.RelationshipClass.DestinationForeignKey, destinationForeignKey,
                this.RelationshipClass.OriginForeignKey, originPrimaryKey,
                fieldName,
                fieldValue);
            return Delete(filter);
        }

        /// <summary>
        ///     Gets all of the rows that have associations with the <paramref name="relationshipObject" /> object.
        /// </summary>
        /// <param name="relationshipObject">The relationship object.</param>
        /// <returns>
        ///     A list of <see cref="IntermediateRow" /> structs.
        /// </returns>
        public IList<IIntermediateRow> GetRows(IObject relationshipObject)
        {
            return GetRows(relationshipObject, null, null);
        }

        /// <summary>
        ///     Gets all of the rows that have associations with the <paramref name="relationshipObject" /> object
        ///     that have a <paramref name="fieldName" /> with the <paramref name="fieldValue" />.
        /// </summary>
        /// <param name="relationshipObject">The relationship object.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>
        ///     A list of <see cref="IntermediateRow" /> structs.
        /// </returns>
        public IList<IIntermediateRow> GetRows(IObject relationshipObject, string fieldName, string fieldValue)
        {
            IList<IIntermediateRow> rows = new List<IIntermediateRow>();

            IQueryFilter filter = ConstructFilter(relationshipObject);
            if (filter == null) return rows;

            AppendFilter(filter, fieldName, fieldValue);

            return BuildList(filter);
        }

        /// <summary>
        ///     Gets all of the rows that have associations between the <paramref name="originObject" /> and
        ///     <paramref name="destinationObject" /> objects.
        /// </summary>
        /// <param name="originObject">The origin object.</param>
        /// <param name="destinationObject">The destination object.</param>
        /// <returns>
        ///     A list of <see cref="IntermediateRow" /> structs.
        /// </returns>
        public IList<IIntermediateRow> GetRows(IObject originObject, IObject destinationObject)
        {
            return GetRows(originObject, destinationObject, null, null);
        }

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
        ///     A list of <see cref="IntermediateRow" /> structs.
        /// </returns>
        public IList<IIntermediateRow> GetRows(IObject originObject, IObject destinationObject, string fieldName, string fieldValue)
        {
            IList<IIntermediateRow> rows = new List<IIntermediateRow>();

            IQueryFilter filter = ConstructFilter(originObject, destinationObject);
            if (filter == null) return rows;

            AppendFilter(filter, fieldName, fieldValue);

            return BuildList(filter);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Appends to the end of the filter an additional clause for an exact match for the <paramref name="fieldName" /> and
        ///     <paramref name="fieldValue" />.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        private void AppendFilter(IQueryFilter filter, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                if (string.IsNullOrEmpty(filter.WhereClause))
                    filter.WhereClause += string.Format(CultureInfo.InvariantCulture, "{0} = '{1}'", fieldName, fieldValue);
                else
                    filter.WhereClause += string.Format(CultureInfo.InvariantCulture, " AND {0} = '{1}'", fieldName, fieldValue);
            }
        }

        /// <summary>
        ///     Builds the collection of <see cref="IntermediateRow" /> objects using the <paramref name="filter" />.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        ///     A list of <see cref="IntermediateRow" /> structs.
        /// </returns>
        private IList<IIntermediateRow> BuildList(IQueryFilter filter)
        {
            IList<IIntermediateRow> rows = new List<IIntermediateRow>();
            using (ComReleaser cr = new ComReleaser())
            {
                ITable table = (ITable) this.RelationshipClass;
                ICursor cursor = table.Search(filter, false);
                cr.ManageLifetime(cursor);

                IRow row;
                while ((row = cursor.NextRow()) != null)
                {
                    IIntermediateRow intermediateRow = this.GetRow(row);
                    rows.Add(intermediateRow);
                }
            }

            return rows;
        }

        /// <summary>
        ///     Checks the validity of the <paramref name="relClass" /> is valid to have an intermediate table.
        /// </summary>
        /// <param name="relClass">The relationship class.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing if the relationship class is valid.
        /// </returns>
        private bool CheckValidity(IRelationshipClass relClass)
        {
            if (relClass.Cardinality != esriRelCardinality.esriRelCardinalityManyToMany || !relClass.IsAttributed)
                return false;

            return true;
        }

        /// <summary>
        ///     Constructs the filter used to query the intermediate table.
        /// </summary>
        /// <param name="originObject">The origin object.</param>
        /// <param name="destinationObject">The destination object.</param>
        /// <returns>
        ///     The <see cref="IQueryFilter" /> that is used to query the relationship.
        /// </returns>
        private IQueryFilter ConstructFilter(IObject originObject, IObject destinationObject)
        {
            int originIndex = originObject.Fields.FindField(this.RelationshipClass.OriginPrimaryKey);
            int destinationIndex = destinationObject.Fields.FindField(this.RelationshipClass.DestinationPrimaryKey);

            object originPrimaryKey = originObject.get_Value(originIndex);
            object destinationPrimaryKey = destinationObject.get_Value(destinationIndex);

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = string.Format(CultureInfo.InvariantCulture, "{0} = '{1}' AND {2} = '{3}'",
                this.RelationshipClass.OriginForeignKey, originPrimaryKey,
                this.RelationshipClass.DestinationForeignKey, destinationPrimaryKey);

            ITable table = (ITable) this.RelationshipClass;
            IQueryFilterDefinition queryDef = (IQueryFilterDefinition) filter;
            queryDef.PostfixClause = "ORDER BY " + table.OIDFieldName + " ASC";

            return filter;
        }

        /// <summary>
        ///     Constructs the filter used to query the intermediate table.
        /// </summary>
        /// <param name="relationshipObject">The relationship object.</param>
        /// <returns>
        ///     The <see cref="IQueryFilter" /> that is used to query the relationship.
        /// </returns>
        private IQueryFilter ConstructFilter(IObject relationshipObject)
        {
            IQueryFilter filter = new QueryFilterClass();
            if (relationshipObject.Class.AliasName == this.RelationshipClass.OriginClass.AliasName)
            {
                int originIndex = relationshipObject.Fields.FindField(this.RelationshipClass.OriginPrimaryKey);
                if (originIndex == -1) return null;

                object originPrimaryKey = relationshipObject.get_Value(originIndex);
                filter.WhereClause = string.Format(CultureInfo.InvariantCulture, "{0} = '{1}'", this.RelationshipClass.OriginForeignKey, originPrimaryKey);
            }
            else
            {
                int destinationIndex = relationshipObject.Fields.FindField(this.RelationshipClass.DestinationPrimaryKey);
                if (destinationIndex == -1) return null;

                object destinationPrimaryKey = relationshipObject.get_Value(destinationIndex);
                filter.WhereClause = string.Format(CultureInfo.InvariantCulture, "{0} = '{1}'", this.RelationshipClass.DestinationForeignKey, destinationPrimaryKey);
            }

            ITable table = (ITable) this.RelationshipClass;
            IQueryFilterDefinition queryDef = (IQueryFilterDefinition) filter;
            queryDef.PostfixClause = "ORDER BY " + table.OIDFieldName + " ASC";

            return filter;
        }

        /// <summary>
        ///     Deletes those records that satisfy the <paramref name="filter" />
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of records deleted.
        /// </returns>
        private int Delete(IQueryFilter filter)
        {
            ITable table = (ITable) this.RelationshipClass;
            int deleteCount = table.RowCount(filter);
            if (deleteCount > 0) table.DeleteSearchedRows(filter);
            return deleteCount;
        }

        /// <summary>
        ///     Builds the <see cref="IIntermediateRow" /> struct for the specified <paramref name="row" />.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        ///     Returns the <see cref="IIntermediateRow" /> struct.
        /// </returns>
        private IIntermediateRow GetRow(IRow row)
        {
            IIntermediateRow intermediateRow = new IntermediateRow(row, this.RelationshipClass);

            ITable table = (ITable) this.RelationshipClass;
            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                IField field = table.Fields.get_Field(i);
                if (IsReadOnly(field))
                    continue;

                object value = row.get_Value(i);
                intermediateRow.Items.Add(field.Name, value);
            }

            return intermediateRow;
        }

        /// <summary>
        ///     Determines whether the specified field is read only.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///     <c>true</c> if the specified field is read only; otherwise, <c>false</c>.
        /// </returns>
        private bool IsReadOnly(IField field)
        {
            if (!field.Editable
                || field.Name.Equals(this.RelationshipClass.OriginForeignKey, StringComparison.CurrentCultureIgnoreCase)
                || field.Name.Equals(this.RelationshipClass.DestinationForeignKey, StringComparison.CurrentCultureIgnoreCase))
                return true;

            return false;
        }

        #endregion
    }
}