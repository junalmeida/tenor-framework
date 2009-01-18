using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Runtime.InteropServices;

namespace Tenor
{
	namespace Web
	{
		public partial class TenorModule
		{
			
			
			/// <summary>
			/// Limpa o cache desta aplicação
			/// </summary>
			/// <remarks></remarks>
			public static void ClearCache()
			{
				ClearCache(null, 0);
			}
			
			/// <summary>
			/// Limpa o cache de uma instancia específica.
			/// </summary>
			/// <param name="Type"></param>
			/// <param name="p1"></param>
			/// <remarks></remarks>
			public static void ClearCache(Type Type, int p1)
			{
				HttpContext context = HttpContext.Current;
				if (Type != null)
				{
					string anamespace = Tenor.Text.Strings.ToAscHex(Type.FullName);
					ClearCache(context.ApplicationInstance, anamespace, Convert.ToString(p1), false);
				}
				else
				{
					ClearCache(context.ApplicationInstance, "all", string.Empty, false);
				}
			}
			
			
			/// <summary>
			/// Função genérica para limpar o cache.
			///
			/// </summary>
			/// <param name="app"></param>
			/// <param name="ClassName">Nome da classe em string ou 'all' para limpeza geral.</param>
			/// <param name="p1">Parâmetro de limpeza.</param>
			/// <param name="OutputStatus">Se verdadeiro exibe na tela o estado da limpeza.</param>
			/// <remarks></remarks>
			private static void ClearCache(HttpApplication app, string ClassName, string p1, bool OutputStatus)
			{
				System.Text.StringBuilder lista = new System.Text.StringBuilder();
				
				System.Web.Caching.Cache Cache = app.Context.Cache;
				if (ClassName == "none")
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
				else if (ClassName == "all")
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
						if (key.Contains("cl=" + ClassName) && key.Contains("p1=" + p1))
						{
							Cache.Remove(key);
						}
					}
				}
				
				if (OutputStatus)
				{
					try
					{
						app.Context.ClearError();
						app.Response.ContentType = "text/plain";
						app.Response.Write("Cache Count: " + app.Context.Cache.Count + "\r\n");
						app.Response.Write("EffectivePrivateBytesLimit: " + (app.Context.Cache.EffectivePrivateBytesLimit / 1024 / 1024).ToString("N2") + " MB" + "\r\n");
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
}
