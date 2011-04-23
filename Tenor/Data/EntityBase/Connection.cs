/*
 * Licensed under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */
using System.Configuration;

namespace Tenor.Data
{
    public abstract partial class EntityBase
    {
        private static ConnectionStringSettings _SystemConnection;

        /// <summary>
        /// Gets or sets the connection that will be used globally.
        /// </summary>
        public static ConnectionStringSettings SystemConnection
        {
            get
            {
                //TODO: Consider moving this to another layer??
                if (_SystemConnection == null)
                {
                    _SystemConnection = GetDefaultConnection();
                }
                return _SystemConnection;
            }
            set
            {
                _SystemConnection = value;
            }
        }


        /// <summary>
        /// Gets the default connection to be used.
        /// </summary>
        private static ConnectionStringSettings GetDefaultConnection()
        {
            if (ConfigurationManager.ConnectionStrings.Count == 0)
            {
                throw (new ConfigurationErrorsException("Cannot find any usable connection string."));
            }
            else
            {
                int i = 0;
                do
                {
                    if (!ConfigurationManager.ConnectionStrings[i].ConnectionString.ToLower().Contains("|datafile|") && !ConfigurationManager.ConnectionStrings[i].ConnectionString.ToLower().Contains("aspnetdb."))
                    {
                        break;
                    }

                    i++;
                    if (i > ConfigurationManager.ConnectionStrings.Count - 1)
                    {
                        throw (new ConfigurationErrorsException("Cannot find any usable connection string."));
                    }
                } while (true);
                return ConfigurationManager.ConnectionStrings[i];
            }

        }




    }
}
