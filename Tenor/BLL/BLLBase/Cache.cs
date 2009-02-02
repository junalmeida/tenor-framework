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

            FieldInfo[] chavesPrimaria = GetPrimaryKeys(this.GetType());
            if (chavesPrimaria.Length == 0)
            {
                throw (new MissingPrimaryKeyException());
            }
            string chavePrimaria = "";
            foreach (FieldInfo f in chavesPrimaria)
            {
                chavePrimaria += "," + f.PropertyValue(this).ToString();
            }


            if (chavePrimaria.Length == 0)
            {
                throw (new MissingPrimaryKeyException());
            }
            else
            {
                chavePrimaria = chavePrimaria.Substring(1);
            }
            return chavePrimaria;
        }


        /// <summary>
        /// Procura a instancia no cache e a inclue se não for encontrada.
        /// </summary>
        /// <returns>Verdadeiro se o item foi lido do cache. Falso se a intancia não foi encontrada. Caso retorne falso deverá ser carregado os dados da mesma.</returns>
        /// <remarks></remarks>
        private bool LoadFromCache()
        {

            string chavePrimaria = GetCacheKey();

            System.Web.Caching.Cache Cache = null;
            if (System.Web.HttpContext.Current != null)
            {
                Cache = System.Web.HttpContext.Current.Cache;
            }

            if (Cache != null)
            {
                Dictionary<string, BLLBase> obj = (Dictionary<string, BLLBase>)(Cache.Get(ChaveCache));

                if (obj == null)
                {
                    obj = new Dictionary<string, BLLBase>();
                    Cache.Add(ChaveCache, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 60, 0), System.Web.Caching.CacheItemPriority.Default, null);
                }


                object sync = new object();
                lock (sync)
                {
                    if (obj.ContainsKey(chavePrimaria) && (obj[chavePrimaria] != null))
                    {
                        BLLBase item = obj[chavePrimaria];
                        item.CopyTo(this);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SaveToCache()
        {
            System.Web.Caching.Cache Cache = null;
            if (System.Web.HttpContext.Current != null)
            {
                Cache = System.Web.HttpContext.Current.Cache;
            }
            if (Cache != null)
            {
                object sync = new object();
                lock (sync)
                {
                    Dictionary<string, BLLBase> obj = (Dictionary<string, BLLBase>)(Cache.Get(ChaveCache));
                    if (obj == null)
                    {
                        obj = new Dictionary<string, BLLBase>();
                        Cache.Add(ChaveCache, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 60, 0), System.Web.Caching.CacheItemPriority.Default, null);
                    }
                    string chavePrimaria = GetCacheKey();
                    if (obj.ContainsKey(chavePrimaria))
                    {
                        //A chave do cache existe, mas por algum motivo não está lá.
                        obj[chavePrimaria] = this;
                    }
                    else
                    {
                        //A chave do cache não existe
                        obj.Add(chavePrimaria, this);
                    }
                }
            }
        }




        /// <summary>
        /// Copia os campos private para o objeto especificado.
        /// </summary>
        /// <param name="obj"></param>
        /// <remarks></remarks>
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
