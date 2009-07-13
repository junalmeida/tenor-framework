using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Tenor.Data.Dialects.PostgreSQL
{
    public class PostgreSQL : GeneralDialect
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
                    /*builder.QuotePrefix = "\"";
                    builder.QuoteSuffix = "\"";*/
                }
                return builder;
            }
        }

        public override string ProviderInvariantName
        {
            get { return "Npgsql"; }
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

        public override string IdentityDuringQuery
        {
            get { return null; }
        }

        public override string IdentityAfterQuery
        {
            get { return "SELECT CURRVAL('\"{0}\"')"; }
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
            get { throw new NotImplementedException(); }
        }

        public override string CreateLimit(int limitValue)
        {
            throw new NotImplementedException();
        }
    }
}
