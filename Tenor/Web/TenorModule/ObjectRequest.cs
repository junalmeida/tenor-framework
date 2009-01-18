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
			
			
			private void ResponseObjectRequest(HttpApplication app)
			{
				//IResponseObject
				
				object messagesLock = new object();
				Dados obj;
				lock(messagesLock)
				{
                    obj = (Dados)(app.Context.Cache.Get(Tenor.Configuration.TenorModule.IdPrefix + QueryString("id")));
				}
				
				if (obj == null)
				{
					//404 - File Not Found
					
					//O cache do asp.net não contém o objeto esperado.
					//Atenção: Isso acontece quando compilamos diversas vezes
					//o projeto. Em alguns minutos o cache do asp.net se restabelece.
					//Pode ser necessário dar um restart ou recycle no application pool.
					throw (new Exception("Objeto não encontrado no cache."));
					
					//return;
					
				}
				else if ((obj.Object as Stream) == null)
				{
					//O campo Object contém uma Stream.
					//Isso é verdadeiro nas chamadas de cache.
					Stream memres = (Stream) obj.Object;
					//Envia os headers e a stream para o cliente
					WriteHeaders(app, obj);
					WriteStream(memres, app);
					
					return;
					
				}
				else if ((obj.Object as IResponseObject) == null)
				{
					//O conteúdo de obj.Object não era o esperado.
					//Uma programação correta não deve gerar este erro
					
					
					//retirar o objeto inválido do cache
					lock(messagesLock)
					{
                        app.Context.Cache.Remove(Tenor.Configuration.TenorModule.IdPrefix + QueryString("id"));
						
						app.Context.ClearError();
						app.Context.AddError(new HttpException(500, "server error", new InvalidCastException()));
						return;
					}
				}
				else
				{
					//Existe IResponseObject no cache.
					
					
					//retirar o objeto do cache para processamento.
					
					lock(messagesLock)
					{

                        app.Context.Cache.Remove(Tenor.Configuration.TenorModule.IdPrefix + QueryString("id"));
						
						Web.IResponseObject rObj = (IResponseObject) obj.Object;
						app.Response.Clear();
						Stream memres;
						//Chama o WriteContent implementado no objeto
						memres = rObj.WriteContent();
						
						if (memres == null || memres.Length <= 0)
						{
							//o objeto gerou uma resposta inválida
							app.Context.AddError(new HttpException(404, "file not found"));
							return; //404
						}
						//descobrir o tipo mime pelo conteudo do arquivo
						obj.ContentType = GetMimeType(memres);
						if (string.IsNullOrEmpty(obj.ContentType))
						{
							//usar o tipo mime definido pelo objeto
							obj.ContentType = rObj.ContentType;
						}
						
						//enviar para o cliente os headers e o stream
						WriteHeaders(app, obj);
						WriteStream(memres, app);
						
						//salvar no cache o stream gerado para posterior visualização
						obj.Object = memres;
                        app.Context.Cache.Insert(Tenor.Configuration.TenorModule.IdPrefix + QueryString("id"), obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, obj.Expires));
						//              app.Context.Cache.Insert(IdPrefix & app.Request.QueryString("id"), obj, Nothing, Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 0, obj.Expires), Caching.CacheItemPriority.Normal, AddressOf Cache_onItemRemoved)
						return;
					}
				}
			}
			
			
			
		}
		
		
	}
}
