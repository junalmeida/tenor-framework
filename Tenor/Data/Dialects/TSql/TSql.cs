using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data.Dialects;
using System.Reflection;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Specialized;
using System.Collections;
using Tenor.BLL;

namespace Tenor.Data.Dialects.TSql
{
    /// <summary>
    /// Represents the T-SQL language.
    /// </summary>
    public class TSql : GeneralDialect
    {
        private const string provider = "System.Data.SqlClient";
        public override string ProviderInvariantName
        {
            get { return provider; }
        }

        public TSql()
        {
            factory = DbProviderFactories.GetFactory(provider);
            commandBuilder = (SqlCommandBuilder)factory.CreateCommandBuilder();
        }

        private DbProviderFactory factory;
        private SqlCommandBuilder commandBuilder;

        protected override DbCommandBuilder CommandBuilder
        {
            get { return (DbCommandBuilder)commandBuilder; }
        }

        public override DbProviderFactory Factory
        {
            get { return factory; }
        }


        protected override string ParameterIdentifier
        {
            get { return "@"; }
        }

        public override string LineEnding
        {
            get { return null; }
        }

        protected override string GetContainsInFlagsExpression(string field, string parameterName)
        {
            return "(IsNull(" + field + ", 0) & " + parameterName + ") = " + parameterName;
        }

        public override string IdentityBeforeQuery
        {
            get { return null; }
        }

        public override string IdentityDuringQuery
        {
            get { return null; }
        }

        public override string IdentityAfterQuery
        {
            get { return "SELECT SCOPE_IDENTITY()"; }
        }

        public override bool GetIdentityOnSameCommand
        {
            get { return true; }
        }

        public override LimitType LimitAt
        {
            get { return LimitType.Start; }
        }

        public override string CreateLimit(int limitValue)
        {
            return "TOP " + limitValue.ToString();
        }
        
        internal override string CreateSaveList(TableInfo baseClass, ForeignKeyInfo fkInfo, Tenor.BLL.BLLBase baseInstance, out TenorParameter[] parameters, out System.Data.DataTable data)
        {
            IList values = (IList)fkInfo.PropertyValue(baseInstance);

            data = new System.Data.DataTable();

            for (int i = 0; i < fkInfo.LocalManyToManyFields.Length; i++)
            {
                data.Columns.Add(fkInfo.LocalManyToManyFields[i], fkInfo.LocalFields[i].RelatedProperty.PropertyType);
            }

            for (int i = 0; i < fkInfo.ForeignManyToManyFields.Length; i++)
            {
                data.Columns.Add(fkInfo.ForeignManyToManyFields[i], fkInfo.ForeignFields[i].RelatedProperty.PropertyType);
            }


            for (int i = 0; i < values.Count; i++)
            {
                System.Data.DataRow row = data.NewRow();
                for (int j = 0; j < fkInfo.LocalManyToManyFields.Length; j++)
                {
                    row[fkInfo.LocalManyToManyFields[j]] = fkInfo.LocalFields[j].PropertyValue(baseInstance);
                } 
                for (int j = 0; j < fkInfo.ForeignFields.Length; j++)
                {
                    row[fkInfo.ForeignManyToManyFields[j]] = fkInfo.ForeignFields[j].PropertyValue(values[i]);
                }
                data.Rows.Add(row);
            }
            const string localParamPrefix = "local{0}";

            List<TenorParameter> parame = new List<TenorParameter>();
            string tableExpression = GetPrefixAndTable(fkInfo.ManyToManyTablePrefix, fkInfo.ManyToManyTable);
            // generating delete statement
            StringBuilder sql = new StringBuilder();
            sql.Append(string.Format("DELETE FROM {0} WHERE ", tableExpression));
            for (int i = 0; i < fkInfo.LocalManyToManyFields.Length; i++)
            {
                if (i > 0)
                    sql.Append(this.GetOperator(LogicalOperator.And));
                sql.Append(this.CommandBuilder.QuoteIdentifier(fkInfo.LocalManyToManyFields[i]));
                sql.Append(" = ");
                sql.Append(this.ParameterIdentifier + string.Format(localParamPrefix, i));

                TenorParameter p = new TenorParameter(string.Format(localParamPrefix, i), fkInfo.LocalFields[i].PropertyValue(baseInstance));
                parame.Add(p);
            }
            parameters = parame.ToArray();
            sql.AppendLine();
            return sql.ToString();


        }

        /// <summary>
        /// Checks whether there's any join that is part of a collection association
        /// </summary>
        /// <param name="join">Join</param>
        /// <param name="joins">All joins</param>
        /// <param name="alias">Property alias</param>
        /// <param name="propName">Property name</param>
        private void CheckJoin(Join join, Join[] joins, string alias, string propName)
        {
            if (join.ForeignKey.IsArray)
                throw new InvalidSortException(string.Format("You can't sort by a field that is in a collection association: alias {0} property {1}", alias, propName));
            
            if (!string.IsNullOrEmpty(join.ParentAlias))
            {
                foreach (Join j in joins)
                {
                    if (j.JoinAlias == join.ParentAlias)
                    {
                        CheckJoin(j, joins, alias, propName);
                        break;
                    }
                }
            }
        }

        internal override string CreateSortSql(SortingCollection sortCollection, Type baseClass, Join[] joins, bool isDistinct, bool isPaging, out string appendToSelect)
        {
            if (isPaging)
            {
                StringBuilder sqlSort = new StringBuilder();
                appendToSelect = string.Empty;
                foreach (SortingCriteria sort in sortCollection)
                {
                    Type table = baseClass;

                    // to be replaced in CreateFullSql
                    string alias = "[[[base_class_alias]]]";

                    string propAlias = string.Empty;

                    Join join = null;
                    
                    if (!string.IsNullOrEmpty(sort.JoinAlias))
                    {
                        int pos = Array.IndexOf(joins, new Join(sort.JoinAlias));
                        if (pos == -1)
                            throw new InvalidCollectionArgument(CollectionProblem.InvalidJoin, sort.JoinAlias);
                        
                        join = joins[pos];

                        CheckJoin(join, joins, sort.JoinAlias, sort.PropertyName);
                        
                        table = join.ForeignKey.ElementType;
                        alias = "[[[join_class_alias]]]";
                    }

                    PropertyInfo property = table.GetProperty(sort.PropertyName);
                    if (property == null)
                        throw new MissingFieldException(table, sort.PropertyName);
                    
                    FieldInfo fieldInfo = FieldInfo.Create(property);
                    
                    SpecialFieldInfo spInfo = SpecialFieldInfo.Create(property);
                    
                    if (fieldInfo == null && spInfo == null)
                        throw new MissingFieldException(table, property.Name, true);

                    if (join != null)
                        propAlias = join.JoinAlias + "||" + sort.PropertyName;

                    sqlSort.Append(", ");
                    sqlSort.Append(CreateOrderBy(table, alias, sort.PropertyName, propAlias, sort.SortOrder, sort.CastType));
                    
                    // -- isn't this useless? doesn't it always duplicate what "createorderby" has done?
                    ////fields that must come into sort part
                    //if (isDistinct && table != baseClass)
                    //{
                    //    StringBuilder fieldExpression = new StringBuilder();

                    //    if (fieldInfo != null)
                    //    {
                    //        fieldExpression.Append(alias + "." + CommandBuilder.QuoteIdentifier(fieldInfo.DataFieldName));
                    //    }
                    //    else if (spInfo != null)
                    //    {
                    //        fieldExpression.Append(GetSpecialFieldExpression(alias, spInfo));
                    //        fieldExpression.Append(" AS " + spInfo.Alias);
                    //    }

                    //    string field = ", " + fieldExpression;
                    //    if (!appendToSelect.Contains(field))
                    //    {
                    //        appendToSelect += (field);
                    //    }
                    //}
                }

                if (sqlSort.Length > 0)
                    sqlSort = sqlSort.Remove(0, 2);

                return sqlSort.ToString();
            }
            else
            {
                return base.CreateSortSql(sortCollection, baseClass, joins, isDistinct, isPaging, out appendToSelect);
            }
        }

        internal override string CreateFullSql(Type baseClass, bool isDistinct, bool justCount, int limit, int? pagingStart, int? pagingEnd, string fieldsPart, string joinsPart, string sortPart, string wherePart)
        {
            if (pagingStart.HasValue && pagingEnd.HasValue)
            {
                if (justCount)
                    throw new InvalidOperationException("It is not possible to page a count result.");

                if (limit > 0)
                    throw new InvalidOperationException("It is not possible to use limit with a paged result.");


                TableInfo table = TableInfo.CreateTableInfo(baseClass);
                string baseAlias = CreateClassAlias(baseClass);

                string baseSQL = @"SELECT distinct * FROM
(SELECT ROW_NUMBER() OVER (ORDER BY [[[sort_for_row_number]]]
) AS ROW, [[[PKS_for_row_number]]]
FROM (
SELECT DISTINCT [[[without_to_many_relations_regular_query]]]
) DistinctQuery
) AS ListWithRowNumbers
INNER JOIN 
(
SELECT [[[regular_query]]]
) AllDataQuery
ON [[[joins_between_row_num_and_data]]]
WHERE  ListWithRowNumbers.Row >= {0} AND ListWithRowNumbers.Row <= {1}
ORDER BY [[[sort_without_alias]]]";


                string sortRowNumber = sortPart.Replace("[[[base_class_alias]]]", "DistinctQuery").Replace("[[[join_class_alias]]]", "DistinctQuery").Replace("||", "_");

                FieldInfo[] pks = BLLBase.GetFields(baseClass, true);
                string pksRowNumber = string.Empty;
                
                foreach (FieldInfo pk in pks)
                    pksRowNumber += ", DistinctQuery." + pk.DataFieldName;

                if (pksRowNumber.Length > 0)
                    pksRowNumber = pksRowNumber.Substring(2);

                string froms = GetPrefixAndTable(table.Prefix, table.TableName);

                froms += " " + baseAlias;

                StringBuilder innerQuery = new StringBuilder();

                string sortSeparator = "||";

                StringBuilder sortingExtraFields = new StringBuilder();

                // TODO: also get sort fields
                innerQuery.Append(CreateSelectSql(baseClass, string.Empty, BLLBase.GetFields(baseClass, null), new SpecialFieldInfo[0]));
                foreach (string part in sortPart.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (part.Contains(sortSeparator))
                    {
                        string inPart = part.Trim();
                        if (inPart.EndsWith(" ASC"))
                            inPart = inPart.Substring(0, inPart.Length - " ASC".Length);
                        else if (inPart.EndsWith(" DESC"))
                            inPart = inPart.Substring(0, inPart.Length - " DESC".Length);

                        string[] parts = inPart.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                        Array.Reverse(parts);

                        string fieldAlias = parts[0].Trim();

                        int separatorIndex = fieldAlias.IndexOf(sortSeparator);
                        sortingExtraFields.AppendFormat(", {0}.{1} {2}", fieldAlias.Substring(0, separatorIndex), fieldAlias.Substring(separatorIndex + sortSeparator.Length), fieldAlias.Replace(sortSeparator, "_"));
                    }
                }

                innerQuery.Append(sortingExtraFields.ToString());

                innerQuery.AppendLine(" FROM " + froms);

                innerQuery.AppendLine(joinsPart);

                if (!string.IsNullOrEmpty(wherePart))
                {
                    innerQuery.Append(" WHERE ");
                    innerQuery.AppendLine(wherePart);
                }

                StringBuilder regularQuery = new StringBuilder();
                
                regularQuery.AppendLine(fieldsPart);

                regularQuery.Append(sortingExtraFields.ToString());

                regularQuery.AppendLine(" FROM " + froms);

                regularQuery.AppendLine(joinsPart);

                if (!string.IsNullOrEmpty(wherePart))
                {
                    regularQuery.Append(" WHERE ");
                    regularQuery.AppendLine(wherePart);
                }

                string joinsRowNum = "";

                foreach (FieldInfo pk in pks)
                    joinsRowNum += " AND ListWithRowNumbers." + pk.DataFieldName + " = AllDataQuery." + pk.DataFieldName + Environment.NewLine;

                if (joinsRowNum.Length > 0)
                    joinsRowNum = joinsRowNum.Substring(5);

                string finalSort = sortPart.Replace("[[[base_class_alias]]]", "AllDataQuery").Replace("[[[join_class_alias]]]", "AllDataQuery").Replace("||", "_");

                string sql = baseSQL.Replace("[[[sort_for_row_number]]]", sortRowNumber)
                                .Replace("[[[PKS_for_row_number]]]", pksRowNumber)
                                .Replace("[[[without_to_many_relations_regular_query]]]", innerQuery.ToString())
                                .Replace("[[[regular_query]]]", regularQuery.ToString())
                                .Replace("[[[joins_between_row_num_and_data]]]", joinsRowNum)
                                .Replace("[[[sort_without_alias]]]", finalSort);

                sql = string.Format(sql, pagingStart.Value, pagingEnd.Value);

                return sql;
            }
            else
            {
                return base.CreateFullSql(baseClass, isDistinct, justCount, limit, null, null, fieldsPart, joinsPart, sortPart, wherePart);
            }
        }
    }
}
