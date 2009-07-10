using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data.Dialects;
using System.Reflection;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Specialized;

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
    }
}
