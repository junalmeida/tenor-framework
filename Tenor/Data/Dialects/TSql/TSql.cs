using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data.Dialects;
using System.Reflection;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Specialized;
using System.Collections;

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
                StringBuilder sql = new StringBuilder();

                sql.Append("SELECT * FROM (");
                sql.Append("SELECT");
                
                // TODO: test if distinct works with paging
                if (isDistinct)
                    sql.Append(" DISTINCT");
                
                sql.Append(" ROW_NUMBER() OVER (ORDER BY ");
                sql.AppendLine(sortPart.ToString());
                sql.Append(") AS ROW, ");
                sql.AppendLine(fieldsPart);

                string froms = GetPrefixAndTable(table.Prefix, table.TableName);

                froms += " " + baseAlias;
                sql.AppendLine(" FROM " + froms);

                sql.AppendLine(joinsPart);

                if (!string.IsNullOrEmpty(wherePart))
                {
                    sql.Append(" WHERE ");
                    sql.AppendLine(wherePart);
                }

                sql.AppendLine(") AS ListWithRowNumbers");
                sql.AppendLine(string.Format("WHERE  Row >= {0} AND Row <= {1}", pagingStart.Value, pagingEnd.Value));

                return sql.ToString();
            }
            else
            {
                return base.CreateFullSql(baseClass, isDistinct, justCount, limit, null, null, fieldsPart, joinsPart, sortPart, wherePart);
            }
        }
    }
}
