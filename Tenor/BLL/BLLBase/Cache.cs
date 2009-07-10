﻿using System;
using System.Collections.Generic;
using System.Text;
using Tenor.Data;
using System.Data;

namespace Tenor.BLL
{
    public abstract partial class BLLBase
    {
        private string GetCacheKey()
        {

            FieldInfo[] primaryKeys = GetPrimaryKeys(this.GetType());
            if (primaryKeys.Length == 0)
            {
                throw (new Tenor.Data.MissingPrimaryKeyException(this.GetType()));
            }
            string primaryKey = "";
            foreach (FieldInfo f in primaryKeys)
            {
                primaryKey += "," + f.PropertyValue(this).ToString();
            }


            if (primaryKey.Length == 0)
            {
                throw (new Tenor.Data.MissingPrimaryKeyException(this.GetType()));
            }
            else
            {
                primaryKey = primaryKey.Substring(1);
            }
            return primaryKey;
        }


        /// <summary>
        /// Seeks for a cache instance. If not found, it will be cached.
        /// </summary>
        /// <returns>Returns true if the item was read from cache. In case of false, the instance must be read from the persistence medium.</returns>
        /// <remarks></remarks>
        private bool LoadFromCache()
        {

            string primaryKey = GetCacheKey();

            System.Web.Caching.Cache cache = null;
            if (System.Web.HttpContext.Current != null)
                cache = System.Web.HttpContext.Current.Cache;

            if (cache != null)
            {
                Dictionary<string, BLLBase> obj = (Dictionary<string, BLLBase>)(cache.Get(cacheKey));

                if (obj == null)
                {
                    obj = new Dictionary<string, BLLBase>();
                    cache.Add(cacheKey, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 60, 0), System.Web.Caching.CacheItemPriority.Default, null);
                }


                object sync = new object();
                lock (sync)
                {
                    if (obj.ContainsKey(primaryKey) && (obj[primaryKey] != null))
                    {
                        BLLBase item = obj[primaryKey];
                        item.CopyTo(this);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SaveToCache()
        {
            System.Web.Caching.Cache cache = null;
            if (System.Web.HttpContext.Current != null)
                cache = System.Web.HttpContext.Current.Cache;

            if (cache != null)
            {
                lock (this)
                {
                    Dictionary<string, BLLBase> obj = (Dictionary<string, BLLBase>)(cache.Get(cacheKey));
                    if (obj == null)
                    {
                        obj = new Dictionary<string, BLLBase>();
                        cache.Add(cacheKey, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 60, 0), System.Web.Caching.CacheItemPriority.Default, null);
                    }
                    string primaryKey = GetCacheKey();
                    if (obj.ContainsKey(primaryKey))
                    {
                        //The key already exists, but somehow it's not there.
                        obj[primaryKey] = this;
                    }
                    else
                    {
                        //The key does not exists.
                        obj.Add(primaryKey, this);
                    }
                }
            }
        }




        /// <summary>
        /// Copies all private fields to the specified object.
        /// </summary>
        /// <param name="obj">An instance of BLLBase to copy data.</param>
        /// <remarks>You can only copy data to instances of the same type.</remarks>
        private void CopyTo(BLLBase obj)
        {
            if (obj.GetType() != this.GetType())
            {
                throw (new InvalidCastException());
            }
            System.Reflection.FieldInfo[] fields = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(obj, field.GetValue(this));
            }
        }
    }
}
