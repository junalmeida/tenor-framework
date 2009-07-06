using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Tenor.Data.Dialects
{
    public static class DialectFactory
    {
        /// <summary>
        /// Creates an instance of a dialect based on current connection.
        /// </summary>
        internal static IDialect CreateDialect(ConnectionStringSettings connection)
        {
            Dictionary<string, Type> dialects = GetAvailableDialectsAndTypes();
            if (dialects.ContainsKey(connection.ProviderName))
            {
                return (IDialect)Activator.CreateInstance(dialects[connection.ProviderName]);
            }
            else
            {
                throw new NotSupportedException("The provider '" + connection.ProviderName + "' is not supported yet. Please send a feature request.");
            }
        }

        /// <summary>
        /// Gets a list of all available dialects.
        /// </summary>
        /// <returns></returns>
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
                    if (Array.IndexOf<Type>(t.GetInterfaces(), typeof(IDialect)) == -1)
                    {
                        throw new System.Configuration.ConfigurationErrorsException(string.Format("The type '{0}' does not implement the '{1}' interface.", t.FullName, typeof(IDialect).FullName));
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

            if (!dialects.ContainsKey("System.Data.SQLite"))
                dialects.Add("System.Data.SQLite", typeof(SQLite.SQLite));

            if (!dialects.ContainsKey("System.Data.OracleClient"))
                dialects.Add("System.Data.OracleClient", typeof(Oracle.Oracle));

            

            return dialects;
        }

    }
}