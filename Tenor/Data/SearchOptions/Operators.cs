using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor.Data
{
    /// <summary>
    /// Keeps a list of compare operators.
    /// </summary>
    /// <remarks></remarks>
    public enum CompareOperator
    {
        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <remarks></remarks>
        Equal,
        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <remarks></remarks>
        NotEqual,
        /// <summary>
        /// Less than operator.
        /// </summary>
        /// <remarks></remarks>
        LessThan,
        /// <summary>
        /// Greater than operator.
        /// </summary>
        /// <remarks></remarks>
        GreaterThan,
        /// <summary>
        /// Less than or equal operator.
        /// </summary>
        /// <remarks></remarks>
        LessThanOrEqual,
        /// <summary>
        /// Greater than or equal operator.
        /// </summary>
        /// <remarks></remarks>
        GreaterThanOrEqual,
        /// <summary>
        /// Like operator for strings.
        /// </summary>
        /// <remarks></remarks>
        @Like,
        /// <summary>
        /// Not like operator for strings.
        /// </summary>
        /// <remarks></remarks>
        NotLike,
        /// <summary>
        /// Checks if your value is in the flags field.
        /// </summary>
        /// <remarks></remarks>
        ContainsInFlags
    }


    /// <summary>
    /// Logical operators.
    /// </summary>
    /// <remarks></remarks>
    public enum LogicalOperator
    {
        /// <summary>
        /// AND logical operator.
        /// </summary>
        /// <remarks></remarks>
        @And,
        /// <summary>
        /// OR logical operator.
        /// </summary>
        /// <remarks></remarks>
        @Or
    }

    /// <summary>
    /// Sort directions.
    /// </summary>
    /// <remarks></remarks>
    public enum SortOrder
    {
        /// <summary>
        /// Sorts the item in an ascending order.
        /// </summary>
        /// <remarks></remarks>
        Ascending,
        /// <summary>
        /// Sorts the item in a descending order.
        /// </summary>
        /// <remarks></remarks>
        Descending
    }
}