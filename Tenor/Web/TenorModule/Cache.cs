using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Runtime.InteropServices;

namespace Tenor.Web
{
    public partial class TenorModule
    {
        /// <summary>
        /// Clears all the TenorModule cache.
        /// </summary>
        /// <remarks></remarks>
        public static void ClearCache()
        {
            ClearCache(null, 0);
        }

        /// <summary>
        /// Removes an instance from the cache.
        /// </summary>
        public static void ClearCache(Type type, int p1)
        {
            HttpContext context = HttpContext.Current;
            if (type != null)
            {
                string anamespace = Tenor.Text.Strings.ToAscHex(type.FullName);
                ClearCache(context.ApplicationInstance, anamespace, Convert.ToString(p1), false);
            }
            else
            {
                ClearCache(context.ApplicationInstance, "all", string.Empty, false);
            }
        }


        /// <summary>
        /// Method that holds all cache logic.
        /// </summary>
        /// <param name="app">The application instance.</param>
        /// <param name="className">A string with a class name or 'all' to clear all the cache.</param>
        /// <param name="p1">The first parameter..</param>
        /// <param name="outputStatus">If true, shows up on response the resume of this operation.</param>
        /// <remarks></remarks>
        private static void ClearCache(HttpApplication app, string className, string p1, bool outputStatus)
        {
            System.Text.StringBuilder lista = new System.Text.StringBuilder();

            System.Web.Caching.Cache Cache = app.Context.Cache;
            if (className == "none")
            {
                foreach (System.Collections.DictionaryEntry item in Cache)
                {
                    try
                    {
                        lista.AppendLine(item.Key.ToString() + ": " + item.Value.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else if (className == "all")
            {
                foreach (System.Collections.DictionaryEntry item in Cache)
                {
                    Cache.Remove(item.Key.ToString());
                }
            }
            else
            {
                foreach (System.Collections.DictionaryEntry item in Cache)
                {
                    string key = item.Key.ToString();
                    if (key.Contains("cl=" + className) && key.Contains("p1=" + p1))
                    {
                        Cache.Remove(key);
                    }
                }
            }

            if (outputStatus)
            {
                try
                {
                    app.Context.ClearError();
                    app.Response.ContentType = "text/plain";
                    app.Response.Write("Cache Count: " + app.Context.Cache.Count + "\r\n");
#if !MONO
                    app.Response.Write("EffectivePrivateBytesLimit: " + (app.Context.Cache.EffectivePrivateBytesLimit / 1024 / 1024).ToString("N2") + " MB" + "\r\n");
#endif
                    app.Response.Write("\r\n" + "\r\n" + lista.ToString());
                }
                catch (Exception)
                {
                }
            }
        }


        private static void Cache_onItemRemoved(string Key, object Value, System.Web.Caching.CacheItemRemovedReason Reason)
        {
            Debug.WriteLine("Key: " + Key + "; Reason=" + Reason.ToString(), "Cache");
        }
    }
}
