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
            dialects.Add("System.Data.SqlClient", typeof(TSql.TSql));
            return dialects;
        }

    }
}