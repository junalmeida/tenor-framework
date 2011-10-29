
namespace Tenor.Data
{
    /// <summary>
    /// Specifies the comparison mode on search.
    /// </summary>
    public enum CompareOperator
    {
        /// <summary>
        /// Equality operator.
        /// </summary>
        Equal,
        /// <summary>
        /// Inequality operator.
        /// </summary>
        NotEqual,
        /// <summary>
        /// Less than operator.
        /// </summary>
        LessThan,
        /// <summary>
        /// Greater than operator.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Less than or equal operator.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// Greater than or equal operator.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// Like operator for strings.
        /// </summary>
        @Like,
        /// <summary>
        /// Not like operator for strings.
        /// </summary>
        NotLike,
        /// <summary>
        /// Checks if your value is in the flags field.
        /// </summary>
        ContainsInFlags,
        EqualLower,
        EqualUpper,
        NotEqualUpper,
        NotEqualLower
    }


    /// <summary>
    /// Specifies the logical operator.
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        /// AND logical operator.
        /// </summary>
        @And,
        /// <summary>
        /// OR logical operator.
        /// </summary>
        @Or
    }

    /// <summary>
    /// Specifies the sort direction.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Sorts the item in an ascending order.
        /// </summary>
        Ascending,
        /// <summary>
        /// Sorts the item in a descending order.
        /// </summary>
        Descending
    }
}