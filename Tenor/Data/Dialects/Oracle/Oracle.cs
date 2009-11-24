using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Tenor.Data.Dialects.Oracle
{
    public class Oracle : GeneralDialect
    {
        private DbProviderFactory factory;
        public override DbProviderFactory Factory
        {
            get
            {
                if (factory == null)
                    factory = DbProviderFactories.GetFactory(this.ProviderInvariantName);
                return factory;
            }
        }

        private DbCommandBuilder builder;
        protected override DbCommandBuilder CommandBuilder
        {
            get
            {
                if (builder == null)
                    builder = this.Factory.CreateCommandBuilder();

                return builder;
            }
        }

        public override string ProviderInvariantName
        {
            get { return "System.Data.OracleClient"; }
        }

        protected override string ParameterIdentifier
        {
            get { return ":"; }
        }

        public override string LineEnding
        {
            get { return null; }
        }

        public override string IdentityBeforeQuery
        {
            get { return null; }
        }

        public override string IdentityDuringQuery
        {
            get { return "\"{0}\".NEXTVAL"; }
        }

        public override string IdentityAfterQuery
        {
            get { return "SELECT \"{0}\".CURRVAL FROM DUAL"; }
        }

        public override bool GetIdentityOnSameCommand
        {
            get { return false; }
        }

        protected override string GetContainsInFlagsExpression(string field, string parameterName)
        {
            throw new NotImplementedException();
        }

        public override LimitType LimitAt
        {
            get { throw new NotImplementedException(); }
        }

        public override string CreateLimit(int limitValue)
        {
            throw new NotImplementedException();
        }

        private const int AliasMaxChar = 30;

        private Dictionary<string, Type> aliases = new Dictionary<string, Type>();

        public override string CreateClassAlias(Type classType)
        {
            string alias = base.CreateClassAlias(classType);

            // in case the alias is too long, removes part of it
            if (alias.Length > AliasMaxChar)
            {
                int startIndex = alias.Length - AliasMaxChar;
                alias = alias.Substring(startIndex, alias.Length - startIndex);
            }

            // in case the alias is already being used for another class
            int index = 1;
            while (aliases.ContainsKey(alias) && aliases[alias] != classType)
            {
                index++;
                string ind = index.ToString();
                int lastIndexSize = (index - 1).ToString().Length;
                alias = alias.Substring(0, alias.Length - lastIndexSize) + ind;
            }

            aliases[alias] = classType;

            return alias;
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
                sql.AppendLine(" FROM DUAL ");
            }
            sql.Append(LineEnding);
            parameters = parameterList.ToArray();
            return sql.ToString();
        }
    }
}
