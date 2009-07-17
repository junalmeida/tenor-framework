using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Data.Common;

namespace Tenor.Data.Dialects
{
    //We do not really need this.

    /*

    /// <summary>
    /// Represents a DBMS-based language dialect.
    /// </summary>
    internal interface IDialect
    {
        string CreateSelectSql(Type baseClass, FieldInfo[] fields, SpecialFieldInfo[] spFields);
        string CreateWhereSql(ConditionCollection conditions, Type baseClass, Join[] joins, out TenorParameter[] parameters);
        string CreateSortSql(SortingCollection sortCollection, Type baseClass, Join[] joins, bool isDistinct, out string appendToSelect);
        string CreateJoinsSql(Join[] joins);
        string CreateFullSql(Type baseClass, bool isDistinct, bool justCount, int limit, string fieldsPart, string joinsPart, string sortPart, string wherePart);


        string CreateSaveSql(Type baseClass, Dictionary<FieldInfo, object> data, NameValueCollection specialValues, ConditionCollection conditions, out TenorParameter[] parameters);
        string CreateConditionalSaveSql(string insertQuery, string updateQuery, string[] conditionalProperties, FieldInfo[] fieldsPrimary);

        string CreateDeleteSql(Type baseClass, ConditionCollection conditions, Join[] joins, out TenorParameter[] parameters);


        string IdentityBeforeQuery
        {
            get;
        }

        string IdentityAfterQuery
        {
            get;
        }

        string LineEnding
        {
            get;
        }

        bool GetIdentityOnSameCommand
        {
            get;
        }

        DbProviderFactory Factory
        {
            get;
        }

    }
      */
}
