using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Tenor.Data.Dialects
{
    /// <summary>
    /// Represents a DBMS-based language dialect.
    /// </summary>
    interface IDialect
    {
        string CreateSelectSql(Type baseClass, FieldInfo[] fields, SpecialFieldInfo[] spFields);
        //string CreateCompareSql(Type @class, string classAlias, string propertyName, object value, CompareOperator compareOperator, Type castType, out string parameterName, Join[] joins);
        string CreateFiltersSql(ConditionCollection conditions, Type baseClass, Join[] joins, out TenorParameter[] parameters);
        string CreateSortSql(SortingCollection sortCollection, Type baseClass, Join[] joins, bool isDistinct, out string appendToSelect);
        string CreateJoinsSql(Join[] joins);
        string CreateFullSql(Type baseClass, bool isDistinct, bool justCount, int limit, string fieldsPart, string joinsPart, string sortPart, string wherePart);


        string CreateSaveSql(Type baseClass, Dictionary<FieldInfo, object> data, NameValueCollection specialValues, ConditionCollection conditions, out TenorParameter[] parameters, out string identityQuery, out bool runOnSameQuery);
        string CreateConditionalSaveSql(string insertQuery, string updateQuery, string[] conditionalProperties, FieldInfo[] fieldsPrimary);

        string CreateDeleteSql(Type baseClass, ConditionCollection conditions, out TenorParameter[] parameters);
    }
}
