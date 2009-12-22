using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;

namespace Tenor.Configuration
{
    /// <summary>
    /// Encapsulates all configuration constants of TenorModule.
    /// </summary>
    public class TenorModuleSection :  ConfigurationElement
    {
        /// <summary>
        /// Defines the default expiration time in seconds.
        /// </summary>
        public const int DefaultExpiresTime = 1 * 60 * 60;
        private const string handlerFileName = "Tenor.axd";

        /// <summary>
        /// Defines the virtual file name that calls this module.
        /// </summary>
        [ConfigurationProperty("handlerFileName", DefaultValue = "")]
        public string HandlerFileName
        {
            get
            {
                string ret = (string)this[this.Properties["handlerFileName"]];
                if (string.IsNullOrEmpty(ret))
                    return handlerFileName;
                else
                    return ret;
            }
        }

        /// <summary>
        /// Defines a base name of all keys.
        /// </summary>
        internal const string IdPrefix = "__TENOR_";

        /// <summary>
        /// Defines the base name of the cache key.
        /// </summary>
        internal const string CacheKeys = IdPrefix + "KEYS";

        /// <summary>
        /// Defines the query string directive to clear the cache. 
        /// </summary>
        internal const string NoCache = "nocache";
    }
}