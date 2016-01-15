using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ESRI.ArcGIS.Geodatabase.Internal
{
    /// <summary>
    ///     A supporting class used to build SQL statements based on field inputs across multiple workspaces.
    /// </summary>
    internal class QueryBuilder
    {
        #region Fields

        private readonly ISubtypes _Subtypes;
        private readonly IWorkspace _Workspace;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryBuilder" /> class.
        /// </summary>
        /// <param name="buildClass">The build class.</param>
        public QueryBuilder(ITable buildClass)
        {
            _Workspace = ((IDataset) buildClass).Workspace;
            _Subtypes = (ISubtypes) buildClass;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryBuilder" /> class.
        /// </summary>
        /// <param name="buildClass">The build class.</param>
        public QueryBuilder(IObjectClass buildClass)
        {
            _Workspace = ((IDataset) buildClass).Workspace;
            _Subtypes = (ISubtypes) buildClass;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Builds the query based on the specified fields. When more then one field is available the query will use the
        ///     specified logical operator
        ///     to concatenate the statements.
        /// </summary>
        /// <param name="keyword">The criteria.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>
        ///     The SQL where clause for the specified field and workspace.
        /// </returns>
        /// <remarks>
        ///     The subtype values will be extracted for the field automatically.
        /// </remarks>
        public string Build(string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, IFields fields)
        {
            return this.Build(keyword, comparisonOperator, logicalOperator, fields.AsEnumerable().ToArray());
        }

        /// <summary>
        ///     Builds the query based on the specified fields. When more then one field is available the query will use the
        ///     specified logical operator
        ///     to concatenate the statements.
        /// </summary>
        /// <param name="keyword">The criteria.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>
        ///     The SQL where clause for the specified field and workspace.
        /// </returns>
        /// <remarks>
        ///     The subtype values will be extracted for the field automatically.
        /// </remarks>
        public string Build(string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, params IField[] fields)
        {
            StringBuilder builder = new StringBuilder();

            // Iterate through each field creating an sql statement for each field.
            foreach (var field in fields)
            {
                string statement = this.Build(keyword, comparisonOperator, logicalOperator, field);
                if (string.IsNullOrEmpty(statement)) continue;

                // Append the logical operator when the statement and expression length is longer then 0.
                if (builder.Length > 0 && statement.Length > 0)
                    builder.Append(string.Format(" {0} ", logicalOperator));

                builder.Append(statement);
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Builds the SQL statement based on the specified field using the logical operator to concatenate the statements.
        ///     The subtype values will be extracted for the field automatically.
        /// </summary>
        /// <param name="keyword">The criteria.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        ///     The SQL where clause for the specified field and workspace.
        /// </returns>
        /// <remarks>
        ///     The subtype values will be extracted for the field automatically.
        /// </remarks>
        public string Build(string keyword, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, IField field)
        {
            StringBuilder builder = new StringBuilder();

            // When the field is not supported exit out.
            if (!this.IsSupported(field.Type))
                return null;

            string lo = string.Empty;

            // When there are subtypes on the field we need to translate them into the coded values.
            IEnumerable<string> values = this.GetDomainValues(field, keyword, comparisonOperator);

            // Iterate through each domain value.
            foreach (string value in values)
            {
                // Append the logical operator and the expression.
                builder.Append(lo);

                // Format the expression into an SQL statement.
                string formattedExpression = this.FormatExpression(comparisonOperator, value, field);
                if (string.IsNullOrEmpty(formattedExpression)) continue;

                builder.Append(formattedExpression);

                // Set the logical operator.
                lo = string.Format(" {0} ", logicalOperator);
            }

            return builder.ToString();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Produce the actual SQL string for the specified <see cref="ComparisonOperator" />. This is the part
        ///     of the SQL string between the column name and the parameter value.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="isValueNull">
        ///     This parameter indicates whether the value of the parameter is null. This
        ///     is required because different operators must be used for null equality checks.
        /// </param>
        /// <returns>
        ///     The SQL string for the specified operator
        /// </returns>
        /// <exception cref="NotSupportedException">Unable to format query string for unknown operator.</exception>
        private string AsOperatorBegin(ComparisonOperator comparisonOperator, bool isValueNull)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals:
                    return isValueNull ? "Is" : "=";
                case ComparisonOperator.NotEquals:
                    return "Is Not";
                case ComparisonOperator.GreaterThan:
                    return ">";
                case ComparisonOperator.GreaterThanOrEquals:
                    return ">=";
                case ComparisonOperator.LessThan:
                    return "<";
                case ComparisonOperator.LessThanOrEquals:
                    return "<=";
                case ComparisonOperator.Like:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                case ComparisonOperator.Contains:
                    return "Like";
                case ComparisonOperator.NotLike:
                    return "Not Like";
                case ComparisonOperator.In:
                    return "In";
                case ComparisonOperator.NotIn:
                    return "Not In";
                default:
                    throw new NotSupportedException("Unable to format query string for unknown operator.");
            }
        }

        /// <summary>
        ///     Produce the actual SQL string for the specified <see cref="ComparisonOperator" />. This is the part
        ///     of the SQL string after the parameter value. This string is usually empty.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <returns>
        ///     The SQL string for the specified operator
        /// </returns>
        private string AsOperatorEnd(ComparisonOperator comparisonOperator)
        {
            if (comparisonOperator == ComparisonOperator.In || comparisonOperator == ComparisonOperator.NotIn)
                return ")";

            return "";
        }

        /// <summary>
        ///     Determines whether the specified keyword matches the value
        /// </summary>
        /// <param name="keyword">The criteria.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <returns>
        ///     <c>true</c> if the specified keyword matches the value; otherwise, <c>false</c>.
        /// </returns>
        private bool AsOperatorMatch(string keyword, string value, ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.StartsWith:
                    return value.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase);

                case ComparisonOperator.Contains:
                    return value.Contains(keyword);

                case ComparisonOperator.EndsWith:
                    return value.EndsWith(keyword, StringComparison.CurrentCultureIgnoreCase);

                case ComparisonOperator.Equals:
                    return value.Equals(keyword, StringComparison.CurrentCultureIgnoreCase);
            }

            return false;
        }

        /// <summary>
        ///     Creates the correct SQL formatted expression for the given parameters.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="keywords">The criteria.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        ///     The formatted SQL expression; otherwise <see cref="String.Empty" />.
        /// </returns>
        private string FormatExpression(ComparisonOperator comparisonOperator, string keywords, IField field)
        {
            StringBuilder expression = new StringBuilder();
            expression.Append("(");

            bool isValueNull = string.IsNullOrEmpty(keywords);
            string value = (isValueNull) ? "Null" : keywords;

            string formattedValue = this.GetFormattedValue(field, value, isValueNull);
            if (string.IsNullOrEmpty(formattedValue))
                return null;

            if (comparisonOperator == ComparisonOperator.Contains ||
                comparisonOperator == ComparisonOperator.EndsWith ||
                comparisonOperator == ComparisonOperator.StartsWith)
            {
                string likeExpression = this.FormatLikeExpression(formattedValue, comparisonOperator, field, isValueNull);
                if (likeExpression == null) return null;

                expression.Append(likeExpression);
            }
            else
            {
                expression.Append(field.Name);
                expression.Append(" ");
                expression.Append(this.AsOperatorBegin(comparisonOperator, isValueNull));
                expression.Append(" ");

                if (this.IsCharacter(field))
                {
                    if (isValueNull)
                        expression.Append(formattedValue);
                    else
                        expression.AppendFormat("'{0}'", formattedValue);
                }
                else
                {
                    expression.Append(formattedValue);
                }

                expression.Append(this.AsOperatorEnd(comparisonOperator));
            }

            expression.Append(")");

            return expression.ToString();
        }

        /// <summary>
        ///     Gets the formatted expression in regards to the <see cref="ComparisonOperator" /> enumeration
        ///     for wild card matching.
        /// </summary>
        /// <param name="formattedValue">The formatted value.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="field">The field.</param>
        /// <param name="isValueNull">if set to <c>true</c> [is value null].</param>
        /// <returns>
        ///     The formatted SQL expression for the value and field.
        /// </returns>
        /// <remarks>
        ///     There are performance considerations in regards to file geodatabase vs remote geodatabase. File geodatabases don't
        ///     support
        ///     function based indexes which can hurt performance when using UPPER or CAST because it requires a full table scan,
        ///     this can cause
        ///     extremely poor performance on large tables. Observed a 20 minute search on a table with 2 Million records for a
        ///     single match.
        /// </remarks>
        private string FormatLikeExpression(string formattedValue, ComparisonOperator comparisonOperator, IField field, bool isValueNull)
        {
            string specialCharacter = ((ISQLSyntax) _Workspace).GetSpecialCharacter(esriSQLSpecialCharacters.esriSQL_WildcardManyMatch);
            string comparisonEquality = this.AsOperatorBegin(comparisonOperator, isValueNull);
            
            if (this.IsCharacter(field))
            {
                if (isValueNull)
                    return string.Format("{0} {1} '{2}'", field.Name, comparisonEquality, specialCharacter);

                string upper = ((ISQLSyntax) _Workspace).GetFunctionName(esriSQLFunctionName.esriSQL_UPPER);

                switch (comparisonOperator)
                {
                    case ComparisonOperator.StartsWith:
                        return string.Format("{0}({1}) {2} '{3}{4}'", upper, field.Name, comparisonEquality, formattedValue.ToUpperInvariant(), specialCharacter);

                    case ComparisonOperator.Contains:
                        return string.Format("{0}({1}) {2} '{4}{3}{4}'", upper, field.Name, comparisonEquality, formattedValue.ToUpperInvariant(), specialCharacter);

                    case ComparisonOperator.EndsWith:
                        return string.Format("{0}({1}) {2} '{4}{3}'", upper, field.Name, comparisonEquality, formattedValue.ToUpperInvariant(), specialCharacter);
                }
            }
            else
            {
                // Microsoft Jet Driver doesn't support the same function casting as the other datatabase,
                // thus we need to reformat the non character statement.
                if (_Workspace.IsDBMS(DBMS.Access))
                {
                    // We use the the CSTR statement for Access appended with "" because if the value is null it will return an empty string instead avoiding
                    // an Invalid use of 'Null' error that will occur when the field value is null.
                    if (isValueNull)
                        return string.Format("CSTR({0} & \"\") {1} '{2}'", field.Name, comparisonEquality, specialCharacter);

                    switch (comparisonOperator)
                    {
                        case ComparisonOperator.StartsWith:
                            return string.Format("CSTR({0} & \"\") {1} '{2}{3}'", field.Name, comparisonEquality, formattedValue, specialCharacter);

                        case ComparisonOperator.Contains:
                            return string.Format("CSTR({0} & \"\") {1} '{3}{2}{3}'", field.Name, comparisonEquality, formattedValue, specialCharacter);

                        case ComparisonOperator.EndsWith:
                            return string.Format("CSTR({0} & \"\") {1} '{3}{2}'", field.Name, comparisonEquality, formattedValue, specialCharacter);
                    }
                }

                // Use the CHAR cast statement which supported in all databases except Access.
                string cast = ((ISQLSyntax) _Workspace).GetFunctionName(esriSQLFunctionName.esriSQL_CAST);

                if (isValueNull)
                    return string.Format("{0}({1} As CHAR({2})) {3} '{4}'", cast, field.Name, field.Name.Length, comparisonEquality, specialCharacter);

                switch (comparisonOperator)
                {
                    case ComparisonOperator.StartsWith:
                        return string.Format("{0}({1} As CHAR({2})) {3} '{4}{5}'", cast, field.Name, field.Name.Length, comparisonEquality, formattedValue, specialCharacter);

                    case ComparisonOperator.Contains:
                        return string.Format("{0}({1} As CHAR({2})) {3} '{5}{4}{5}'", cast, field.Name, field.Name.Length, comparisonEquality, formattedValue, specialCharacter);

                    case ComparisonOperator.EndsWith:
                        return string.Format("{0}({1} As CHAR({2})) {3} '{5}{4}'", cast, field.Name, field.Name.Length, comparisonEquality, formattedValue, specialCharacter);
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the corresponding domain value for the value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="keyword">The criteria.</param>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <returns>
        ///     The code value for the domain; otherwise the value parameter.
        /// </returns>
        private IEnumerable<string> GetDomainValues(IField field, string keyword, ComparisonOperator comparisonOperator)
        {
            List<string> items = new List<string>();

            if (_Subtypes.HasSubtype)
            {
                var values = _Subtypes.Subtypes.AsEnumerable();
                foreach (var subtype in values)
                {
                    if (_Subtypes.SubtypeFieldName.Equals(field.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (this.AsOperatorMatch(keyword, subtype.Value, comparisonOperator))
                        {
                            var subtypeCode = subtype.Key.ToString(CultureInfo.InvariantCulture);
                            if (!items.Contains(subtypeCode))
                                items.Add(subtypeCode);
                        }
                    }
                    else
                    {
                        ICodedValueDomain codedValueDomain = _Subtypes.Domain[subtype.Key, field.Name] as ICodedValueDomain;
                        if (codedValueDomain != null)
                        {
                            foreach (var domain in codedValueDomain.AsEnumerable())
                            {
                                if (this.AsOperatorMatch(keyword, domain.Key, comparisonOperator))
                                {
                                    if (!items.Contains(domain.Value))
                                        items.Add(domain.Value);
                                }
                            }
                        }
                    }
                }
            }

            if (items.Count == 0)
            {
                items.Add(keyword);
            }

            return items;
        }

        /// <summary>
        ///     Gets the formatted value for the <see cref="IField" />.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="isValueNull">if set to <c>true</c> is value null.</param>
        /// <returns>
        ///     The formatted value; otherwise <see cref="String.Empty" />
        /// </returns>
        private string GetFormattedValue(IField field, string value, bool isValueNull)
        {
            string formattedValue = string.Empty;

            // When the field is a character we can return the given value.
            if (this.IsCharacter(field))
            {
                formattedValue = isValueNull ? value : string.Format("{0}", ((ISQLSyntax) _Workspace).Escape(value));
            }
            else
            {
                // Depending on the field type perform different formatting.
                switch (field.Type)
                {
                    case esriFieldType.esriFieldTypeDate:

                        DateTime dateTime;
                        if (DateTime.TryParse(value, out dateTime))
                        {
                            formattedValue = _Workspace.GetFormattedDate(dateTime);
                        }
                        else if (isValueNull)
                        {
                            formattedValue = value;
                        }
                        else
                        {
                            formattedValue = string.Format("{0}", ((ISQLSyntax) _Workspace).Escape(value));
                        }
                        break;

                    case esriFieldType.esriFieldTypeDouble:

                        double result;
                        if (double.TryParse(value, out result))
                        {
                            formattedValue = string.Format("{0}", value);
                        }
                        break;

                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeOID:
                    case esriFieldType.esriFieldTypeSingle:
                    case esriFieldType.esriFieldTypeSmallInteger:

                        int number;
                        if (int.TryParse(value, out number))
                        {
                            formattedValue = string.Format("{0}", number);
                        }

                        break;
                }
            }

            return formattedValue;
        }

        /// <summary>
        ///     Determines whether the specified field is a character
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///     <c>true</c> if the specified field type is a character; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCharacter(IField field)
        {
            switch (field.Type)
            {
                case esriFieldType.esriFieldTypeGUID:
                case esriFieldType.esriFieldTypeGlobalID:
                case esriFieldType.esriFieldTypeString:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Determines whether the specified field type is supported.
        /// </summary>
        /// <param name="value">Type of the field.</param>
        /// <returns>
        ///     <c>true</c> if the specified field type is supported; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSupported(esriFieldType value)
        {
            switch (value)
            {
                case esriFieldType.esriFieldTypeBlob:
                case esriFieldType.esriFieldTypeGeometry:
                case esriFieldType.esriFieldTypeRaster:
                case esriFieldType.esriFieldTypeXML:
                    return false;
                default:
                    return true;
            }
        }

        #endregion
    }
}