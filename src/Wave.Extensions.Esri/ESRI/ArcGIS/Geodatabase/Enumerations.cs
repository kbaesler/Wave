namespace ESRI.ArcGIS.Geodatabase
{

    #region Enumerations

    /// <summary>
    ///     Enumeration of the comparison operators supported in SQL statements.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        ///     This is the equals operator for strict equality comparisons. The framework automatically
        ///     uses the correct syntax for null value comparisons.
        /// </summary>
        Equals,

        /// <summary>
        ///     This is the negated equals operator. The framework automatically uses the correct syntax
        ///     for null value comparisons.
        /// </summary>
        NotEquals,

        /// <summary>
        ///     This is the equals operator for partial equality comparisons. The parameter value
        ///     should be a string to match with percent characters used as wildcards.
        /// </summary>
        Like,

        /// <summary>
        ///     This is the equals operator for partial equality comparisons. The parameter value
        ///     should be a string to match with percent characters used as wildcards.
        /// </summary>
        Contains,

        /// <summary>
        ///     This is the equals operator for partial equality comparisons. The parameter value
        ///     should be a string to match with percent characters used as wildcards.
        /// </summary>
        StartsWith,

        /// <summary>
        ///     This is the equals operator for partial equality comparisons. The parameter value
        ///     should be a string to match with percent characters used as wildcards.
        /// </summary>
        EndsWith,

        /// <summary>
        ///     This is the negated equals operator for partial equality comparisons. The parameter value
        ///     should be a string to match with percent characters used as wildcards.
        /// </summary>
        NotLike,

        /// <summary>
        ///     This is the less than operator.
        /// </summary>
        LessThan,

        /// <summary>
        ///     This is the less than or equals operator.
        /// </summary>
        LessThanOrEquals,

        /// <summary>
        ///     This is the greater than operator.
        /// </summary>
        GreaterThan,

        /// <summary>
        ///     This is the greater than or equals operator.
        /// </summary>
        GreaterThanOrEquals,

        /// <summary>
        ///     This is the in operator which tests for set membership. Constraints using Operator.In can
        ///     only be added by specifying a list of elements (though the lists may contain 0 or 1 elements).
        /// </summary>
        In,

        /// <summary>
        ///     This is the negated in operator which tests for set non-membership.
        /// </summary>
        NotIn
    }

    /// <summary>
    ///     Enumeration of the logic operators supported in SQL statements.
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        ///     This is the AND operator. Combines two conditions together. Selects a record if both conditions are true.
        /// </summary>
        And,

        /// <summary>
        ///     This is the OR operator. Combines two conditions together. Selects a record if at least one condition is true.
        /// </summary>
        Or,

        /// <summary>
        ///     This is the NOT operator. Selects a record if it doesn't match the following expression.
        /// </summary>
        Not
    }

    #endregion
}