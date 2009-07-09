using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor.Configuration
{
    /// <summary>
    /// Encapsulates all configuration constants of TenorModule.
    /// </summary>
    internal class TenorModule
    {
        /// <summary>
        /// Defines the default expiration time in seconds.
        /// </summary>
        /// <remarks></remarks>
        internal const int DefaultExpiresTime = 1 * 60 * 60;
        /// <summary>
        /// Defines the virtual file name that calls this module.
        /// </summary>
        /// <remarks></remarks>
        internal const string HandlerFileName = "Tenor.axd";
        /// <summary>
        /// Defines a base name of all keys.
        /// </summary>
        /// <remarks></remarks>
        internal const string IdPrefix = "__TENOR_";

        /// <summary>
        /// Defines the base name of the cache key.
        /// </summary>
        /// <remarks></remarks>
        internal const string CacheKeys = IdPrefix + "KEYS";

        /// <summary>
        /// Defines the query string directive to clear the cache. 
        /// </summary>
        /// <remarks></remarks>
        internal const string NoCache = "nocache";
    }
}