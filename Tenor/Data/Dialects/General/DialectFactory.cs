using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.Common;

namespace Tenor.Data.Dialects
{
    public static class DialectFactory
    {
        /// <summary>
        /// Creates an instance of a dialect based on current connection string settings.
        /// </summary>
        /// <param name="connectionStringSettings">Connection string settings</param>
        internal static GeneralDialect CreateDialect(ConnectionStringSettings connectionStringSettings)
        {
            Dictionary<string, Type> dialects = GetAvailableDialectsAndTypes();
            if (dialects.ContainsKey(connectionStringSettings.ProviderName))
            {
                return (GeneralDialect)Activator.CreateInstance(dialects[connectionStringSettings.ProviderName]);
            }
            else
            {
                throw new NotSupportedException("The provider '" + connectionStringSettings.ProviderName + "' is not supported yet. Please send a feature request.");
            }
        }

        /// <summary>
        /// Creates an instance of a dialect based on current connection instance.
        /// </summary>
        /// <param name="connection">Connection instance</param>
        internal static GeneralDialect CreateDialect(DbConnection connection)
        {
            Dictionary<string, Type> dialects = GetAvailableDialectsAndTypes();
            Type connectionType = connection.GetType();

            Exception notSupportedEx = null;
            foreach (Type dialectType in dialects.Values)
            {
                GeneralDialect dialect = (GeneralDialect)Activator.CreateInstance(dialectType);
                try
                {
                    if (dialect.ConnectionType.Equals(connectionType))
                        return dialect;
                }
                catch (NotSupportedException ex)
                {
                    notSupportedEx = ex;
                }
            }

            throw new NotSupportedException("The connection type '" + connectionType.FullName + "' is not supported yet or its provider is not correctly installed.", notSupportedEx);
        }

        /// <summary>
        /// Gets a list of all available dialects.
        /// </summary>
        public static string[] GetAvailableDialects()
        {
            return new List<string>(GetAvailableDialectsAndTypes().Keys).ToArray();
        }

        internal static Dictionary<string, Type> GetAvailableDialectsAndTypes()
        {
            Dictionary<string, Type> dialects = new Dictionary<string, Type>();

            foreach (Tenor.Configuration.DialectElement d in Tenor.Configuration.Tenor.Current.Dialects)
            {
                try
                {
                    Type t = Type.GetType(d.Type, true);
                    if (!t.IsSubclassOf(typeof(GeneralDialect)))
                    {
                        throw new System.Configuration.ConfigurationErrorsException(string.Format("The type '{0}' does not derive from '{1}'.", t.FullName, typeof(GeneralDialect).FullName));
                    }
                    dialects.Add(d.ProviderName, t);
                }
                catch (ConfigurationErrorsException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new System.Configuration.ConfigurationErrorsException("Could not load the dialect type.", ex);
                }
            }


            if (!dialects.ContainsKey("System.Data.SqlClient"))
                dialects.Add("System.Data.SqlClient", typeof(TSql.TSql));

            if (!dialects.ContainsKey("MySql.Data.MySqlClient"))
                dialects.Add("MySql.Data.MySqlClient", typeof(MySql.MySql));

#if MONO
            if (!dialects.ContainsKey("Mono.Data.Sqlite"))
                dialects.Add("Mono.Data.Sqlite", typeof(SQLite.SQLite));
#endif            
            if (!dialects.ContainsKey("System.Data.SQLite"))
                dialects.Add("System.Data.SQLite", typeof(SQLite.SQLite));


            if (!dialects.ContainsKey("System.Data.OracleClient"))
                dialects.Add("System.Data.OracleClient", typeof(Oracle.Oracle));

            if (!dialects.ContainsKey("Npgsql"))
                dialects.Add("Npgsql", typeof(PostgreSQL.PostgreSQL));
  

            return dialects;
        }
    }
}