using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Tenor.Data.Dialects.Oracle
{
    public class Oracle : GeneralDialect, IDialect
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

        protected override string CreateClassAlias(Type classType)
        {
            if (classType == null)
                throw new ArgumentNullException("classType");

            string alias = classType.FullName.Replace(".", "").ToLower();

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
    }
}
