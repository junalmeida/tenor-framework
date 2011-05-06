using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Tenor.Data.Dialects.SQLite
{
    /// <summary>
    /// Represents the SQLite language.
    /// </summary>
    public class SQLite : GeneralDialect
    {
        private DbProviderFactory factory;
        public override System.Data.Common.DbProviderFactory Factory
        {
            get
            {
                if (factory == null)
                    factory = DbProviderFactories.GetFactory(this.ProviderInvariantName);
                return factory;
            }
        }

        private DbCommandBuilder builder;
        protected override System.Data.Common.DbCommandBuilder CommandBuilder
        {
            get
            {
                if (builder == null)
                {
                    builder = this.Factory.CreateCommandBuilder();
                    builder.QuotePrefix = "\"";
                    builder.QuoteSuffix = "\"";
                }
                return builder;
            }
        }

        public override string ProviderInvariantName
        {
            get
            {
#if MONO
                return "Mono.Data.Sqlite";
#else
                return "System.Data.SQLite";
#endif
            }
        }

        protected override string ParameterIdentifier
        {
            get { return "@"; }
        }

        public override string LineEnding
        {
            get { return ";"; }
        }

        public override string IdentityBeforeQuery
        {
            get { return null; }
        }

        public override string  IdentityDuringQuery
        {
            get { return null; }
        }

        public override string IdentityAfterQuery
        {
            get { return "SELECT LAST_INSERT_ROWID()"; }
        }

        public override bool GetIdentityOnSameCommand
        {
            get { return true; }
        }

        protected override string GetContainsInFlagsExpression(string field, string parameterName)
        {
            throw new NotImplementedException();
        }

        public override LimitType LimitAt
        {
            get { return LimitType.End; }
        }

        public override string CreateLimit(int limitValue)
        {
            return string.Format(" LIMIT {0}", limitValue);
        }

        public override string CreateSaveListSql(string tableNameExpression, string[] localFields, object[] localValues, string[] foreignFields, object[,] propertyValues, out TenorParameter[] parameters)
        {
            const string localParamPrefix = "local{0}";
            List<TenorParameter> parameterList = new List<TenorParameter>();

            for (int i = 0; i < localValues.Length; i++)
            {
                TenorParameter p = new TenorParameter(string.Format(localParamPrefix, i), localValues[i]);
                parameterList.Add(p);
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(string.Format("DELETE FROM {0} WHERE ", tableNameExpression));
            for (int i = 0; i < localFields.Length; i++)
            {
                if (i > 0)
                    sql.Append(this.GetOperator(LogicalOperator.And));
                sql.Append(this.CommandBuilder.QuoteIdentifier(localFields[i]));
                sql.Append(" = ");
                sql.Append(this.ParameterIdentifier + string.Format(localParamPrefix, i));
            }
            sql.AppendLine(LineEnding);
            sql.AppendLine(Helper.GoStatement);
            if (propertyValues.GetUpperBound(0) > -1) // if we have values on the list
            {

                sql.Append(string.Format("INSERT INTO {0} (", tableNameExpression));
                for (int i = 0; i < localFields.Length; i++)
                {
                    if (i > 0)
                        sql.Append(", ");
                    sql.Append(this.CommandBuilder.QuoteIdentifier(localFields[i]));
                }
                for (int i = 0; i < foreignFields.Length; i++)
                {
                    sql.Append(", ");
                    sql.Append(this.CommandBuilder.QuoteIdentifier(foreignFields[i]));
                }
                sql.AppendLine(") ");

                for (int i = 0; i <= propertyValues.GetUpperBound(0); i++)
                {
                    if (i > 0)
                        sql.Append(" UNION ALL ");

                    sql.Append(" SELECT ");
                    for (int j = 0; j < localFields.Length; j++)
                    {
                        if (j > 0)
                            sql.Append(", ");
                        sql.Append(this.ParameterIdentifier + string.Format(localParamPrefix, j));
                    }
                    for (int j = 0; j < foreignFields.Length; j++)
                    {
                        sql.Append(", ");

                        string value = string.Empty;
                        Type type = propertyValues[i, j].GetType();
                        if (type == typeof(string) || type == typeof(DateTime))
                        {
                            value = string.Format("'{0}'", propertyValues[i, j].ToString().Replace("'", "''"));
                        }
                        else if (type.IsEnum)
                        {
                            //TODO: Support converting enums to char and strings for legacy databases.
                            value = ((long)propertyValues[i, j]).ToString();
                        }
                        else
                        {
                            value = propertyValues[i, j].ToString();
                        }

                        sql.Append(value);
                    }
                    sql.AppendLine("");
                }

                sql.Append(LineEnding);
            }
            parameters = parameterList.ToArray();
            return sql.ToString();
        }

        protected override string GetLenExpression(string fieldExpression)
        {
            return string.Format("length({0})", fieldExpression);
        }

        internal override string CreateFullSql(Type baseClass, bool isDistinct, bool justCount, int limit, int? skip, int? take, string fieldsPart, string joinsPart, string sortPart, string wherePart)
        {
            string baseSQL = base.CreateFullSql(baseClass, isDistinct, justCount, limit, null, null, fieldsPart, joinsPart, sortPart, wherePart);
            
            if (skip.HasValue && take.HasValue)
            {
                if (justCount)
                    throw new InvalidOperationException("It is not possible to page a count result.");

                if (limit > 0)
                    throw new InvalidOperationException("It is not possible to use limit with a paged result.");

                return string.Format(@"{0} limit {1} offset {2}", baseSQL, take.Value, skip.Value);
            }
            else
            {
                return baseSQL;
            }
        }

        /// <summary>
        /// Command to enable foreign key constraints
        /// </summary>
        internal override string OnConnectCommand
        {
            get
            {
                return "PRAGMA foreign_keys = ON;";
            }
        }
    }
}
