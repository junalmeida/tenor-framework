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
                {
                    builder = this.Factory.CreateCommandBuilder();
                    //builder.QuotePrefix = "\"";
                    //builder.QuoteSuffix = "\"";
                }
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
    }
}
