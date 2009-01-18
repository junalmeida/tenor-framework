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
using System.Threading;

namespace Tenor
{
	namespace Web
	{
		public partial class TenorModule
		{
			
			
			
			
			/// <summary>Registra um controle para posterior chamada</summary>
			/// <param name="Control">Controle para registrar</param>
			/// <param name="Expires">Tempo em segundos que a informação irá expirar após ser acessada pela primeira vez. O padrão é 1 hora.</param>
			/// <returns>An Uri to complete this request</returns>
			/// <remarks>O Controle precisa implementar a interface IResponseObject</remarks>
			public static string RegisterControlForRequest(System.Web.UI.Control Control, int Expires)
			{
				return RegisterObjectForRequest(Control, Expires);
			}
			
			/// <summary>Registra um controle para posterior chamada</summary>
			/// <param name="Control">Controle para registrar</param>
			/// <returns>An Uri to complete this request</returns>
			/// <remarks>O Controle precisa implementar a interface IResponseObject</remarks>
			public static string RegisterControlForRequest(System.Web.UI.Control Control)
			{
				return RegisterObjectForRequest(Control);
			}
			
			/// <summary>
			/// Registra um objeto para posterior chamada
			/// </summary>
			/// <param name="Object">Objeto para registrar</param>
			/// <param name="Context">HttpContext atual</param>
			/// <param name="Expires">Tempo em segundos que a informação irá expirar após ser acessada pela primeira vez. O padrão é 1 hora.</param>
			/// <param name="ForceDownload">Determina se esta requisição deverá ou não forçar com que o arquivo seja 'baixado' pelo navegador do cliente. O padrão é falso</param>
			/// <param name="FileName">Determina o nome do arquivo que será exibido pelo navegador. O padrão é nulo</param>
			/// <returns>An Uri to complete this request</returns>
			/// <remarks>O Objeto precisa implementar a interface IResponseObject</remarks>
			public static string RegisterObjectForRequest(object @object, System.Web.HttpContext Context, int Expires, bool ForceDownload, string FileName)
			{
				CheckHttpModule();
				
				
				if (@object as IResponseObject == null)
				{
					throw (new InvalidCastException("This instance must implement \'" + typeof(IResponseObject).FullName + "\' interface"));
				}
				
				object messagesLock = new object();
				lock(messagesLock)
				{
					
					
					string sControlName = System.Guid.NewGuid().ToString();
					//Page.Application(HttpModule.IdPrefix & sControlName) = [Object]
					Dados dados = new Dados();
					dados.Object = @object;
					dados.Expires = Expires;
					dados.FileName = FileName;
					dados.ForceDownload = ForceDownload;

                    Context.Cache.Insert(Tenor.Configuration.TenorModule.IdPrefix + sControlName, dados, null, DateTime.UtcNow.AddMinutes(2), System.Web.Caching.Cache.NoSlidingExpiration);

                    string uri = Tenor.Configuration.TenorModule.HandlerFileName + "?id=" + sControlName;
					
					if (Context.Request.ApplicationPath.EndsWith("/"))
					{
						uri = Context.Request.ApplicationPath + uri;
					}
					else
					{
						uri = Context.Request.ApplicationPath + "/" + uri;
					}
					return uri;
					
				}
				
			}
			
			/// <summary>
			/// Registra um objeto para posterior chamada
			/// </summary>
			/// <param name="Object">Objeto para registrar</param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string RegisterObjectForRequest(object @object)
			{
				return RegisterObjectForRequest(@object, System.Web.HttpContext.Current, Tenor.Configuration.TenorModule.DefaultExpiresTime, false, null);
			}
			
			/// <summary>
			/// Registra um objeto para posterior chamada
			/// </summary>
			/// <param name="Object">Objeto para registrar</param>
			/// <param name="Expires">Tempo em segundos que a informação irá expirar após ser acessada pela primeira vez. O padrão é 1 hora.</param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string RegisterObjectForRequest(object @object, int Expires)
			{
				return RegisterObjectForRequest(@object, System.Web.HttpContext.Current, Expires, false, null);
			}
			
			
			
			
			
			
			
		}
		
		
	}
}
