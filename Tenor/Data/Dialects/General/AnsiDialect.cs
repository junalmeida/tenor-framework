using System;
using System.Collections.Generic;
using System.Text;

namespace Tenor.Data.Dialects
{
    /// <summary>
    /// Represents the common code between all dialects.
    /// </summary>
    internal abstract class AnsiDialect
    {


        protected virtual string GetOperator(LogicalOperator logicalOperator)
        {
            switch (logicalOperator)
            {
                case Tenor.Data.LogicalOperator.And:
                    return (" AND ");
                case Tenor.Data.LogicalOperator.Or:
                    return (" OR ");
                default:
                    throw new ArgumentOutOfRangeException("logicalOperator");
            }

        }

        protected string CreateClassAlias(Type classType)
        {
            if (classType == null)
                throw new ArgumentNullException("classType");
            return classType.FullName.Replace(".", "").ToLower();
        }


    }
}
