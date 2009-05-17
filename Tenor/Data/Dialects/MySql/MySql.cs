using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Tenor.Data.Dialects.MySql
{
    public class MySql : GeneralDialect, IDialect
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
                    builder = this.Factory.CreateCommandBuilder();
                return builder;
            }
        }

        public override string ProviderInvariantName
        {
            get { return "MySql.Data.MySqlClient"; }
        }

        protected override string ParameterIdentifier
        {
            get { return "@"; }
        }

        public override string IdentityBeforeQuery
        {
            get { return null; }
        }

        public override string IdentityAfterQuery
        {
            get { return ";SELECT LAST_INSERT_ID();"; }
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
            return "LIMIT " + limitValue.ToString();
        }
    }
    
}
