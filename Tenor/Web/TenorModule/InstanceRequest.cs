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
using Tenor.Drawing;

namespace Tenor
{
	namespace Web
	{
		public partial class TenorModule
		{
			
			
			
			private string _class;
			private bool FindAssembly(Assembly obj)
			{
				return (obj.GetType(_class, false, true) != null);
			}
			
			private bool FindProperty(PropertyInfo i)
			{
				ResponsePropertyAttribute[] atts = (ResponsePropertyAttribute[]) (i.GetCustomAttributes(typeof(ResponsePropertyAttribute), true));
				if (atts.Length > 0)
				{
					return (i.PropertyType.GetInterface(typeof(IResponseObject).FullName) != null);
				}
				else
				{
					return false;
				}
			}
			
			private void InstanceRequest(HttpApplication app)
			{
				System.Web.Caching.Cache Cache = app.Context.Cache;
				Dados item = null;
				
				object messagesLock = new object();
				if (app.Request[Tenor.Configuration.TenorModule.NoCache] == string.Empty)
				{
					lock(messagesLock)
					{
						item = Cache.Get("instance:" + app.Request.QueryString.ToString()) as Dados;
					}
				}
				
				if (item != null)
				{
					//os dados já estão no cache para a querystring atual
					WriteHeaders(app, item);
					WriteStream(((Stream) item.Object), app);
					
				}
				else
				{
					//os dados ainda não estão no cache
					
					
					_class = QueryString("cl");
					
					//Metodo novo - Hexadecimal
					try
					{
						_class = Tenor.Text.Strings.FromAscHex(_class);
					}
					catch (Exception)
					{
						try
						{
							//Metodo antigo
							_class = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_class));
						}
						catch (Exception)
						{
							throw (new ArgumentException("Invalid \'cl\' parameter. Value: " + _class));
						}
					}
					
					
					Type classeT = null;
					
					Assembly ass;
					
					lock(messagesLock)
					{
						try
						{
							ass = Array.Find<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), new Predicate<Assembly>(FindAssembly));
							if (ass != null)
							{
								classeT = ass.GetType(_class, false, true);
							}
						}
						catch (Exception)
						{
						}
					}
					
					if (classeT == null)
					{
						//Não encontrou a classe
						foreach (string file in System.IO.Directory.GetFiles(AppDomain.CurrentDomain.RelativeSearchPath, "*.dll"))
						{
							try
							{
								Assembly ass2 = Assembly.LoadFile(file);
								if (ass2 != null)
								{
									classeT = ass2.GetType(_class, false, true);
									if (classeT != null)
									{
										goto endOfForLoop;
									}
								}
							}
							catch (Exception)
							{
							}
						}
endOfForLoop:
						if (classeT == null)
						{
							throw (new Exception("Cannot find class \'" + _class + "\'."));
							//return; //404
						}
						//Else
						//    classeT = ass.GetType(_class, False, True)
					}
					
					//procura um construtor que receba um inteiro e um booleano pra desligar o lazy loading(!!!)
					
					ConstructorInfo classeC = classeT.GetConstructor(new Type[] {typeof(int)});
					
					if (classeC == null)
					{
						//nenhum construtor
						//é necessário um contrutor que receba inteiro
						throw (new Exception("Cannot find any suitable constructor to call for class \'" + _class + "\'."));
						//return; //404
					}
					//no momento somente 1 parametro string é aceito
					string valorO = QueryString("p1");
					int valorI = 0;
					if (! int.TryParse(valorO, out valorI))
					{
						//parametro invalido
						throw (new Exception("Invalid parameter value."));
						//return; //404
					}
					else
					{
						try
						{
							
							
							
							object instancia = classeC.Invoke(new object[] {valorI});
							IResponseObject responseObject = null;
							
							if (classeT.GetInterface(typeof(IResponseObject).FullName) == null)
							{
								PropertyInfo prop = Array.Find<PropertyInfo>(classeT.GetProperties(), new Predicate<PropertyInfo>(FindProperty));
								if (prop != null)
								{
									responseObject = (IResponseObject) (prop.GetValue(instancia, null));
								}
							}
							else
							{
								responseObject = (IResponseObject) instancia;
							}
							if (responseObject == null)
							{
								//A classe não implementa IResponseObject
								//nem nenhuma propriedade que a retorne.
								throw (new Exception("Invalid response stream."));
								//return; //404
							}
							
							if (! string.IsNullOrEmpty(QueryString("w")) || ! string.IsNullOrEmpty(QueryString("h")) || ! string.IsNullOrEmpty(QueryString("l")))
							{
								//tratamento de imagem
								int l = 0;
								int.TryParse(QueryString("l"), out l);
								
								
								
								int w = 0;
								int.TryParse(QueryString("w"), out w);
								int h = 0;
								int.TryParse(QueryString("h"), out h);
								
								ResizeMode mode = ResizeMode.Proportional;
								
								if (! string.IsNullOrEmpty(QueryString("m")))
								{
									try
									{
										mode = (ResizeMode)(int.Parse(QueryString("m")));
									}
									catch (Exception)
									{
										throw (new HttpException("Invalid image parameters", 500));
									}
								}
								
								if (((object) responseObject).GetType().GetInterface(typeof(IImage).FullName) != null)
								{
									IImage img = (IImage) responseObject;
									if (w > 0 || h > 0)
									{
										img.Resize(w, h, mode);
									}
									if (l != 0)
									{
										img.LowQuality = true;
									}
									//If crop Then
									//    img.Resize(w, h, Drawing.ResizeMode.Crop)
									//Else
									//    If w = 0 Then
									//        img.ResizeByHeight(h)
									//    ElseIf h = 0 Then
									//        img.ResizeByWidth(w)
									//    Else
									//        img.Resize(w, h, Drawing.ResizeMode.Stretch)
									//    End If
									//End If
								}
							}
							
							
							Stream File = null;
							File = responseObject.WriteContent();
							
							Dados dados = new Dados();
                            dados.Expires = Tenor.Configuration.TenorModule.DefaultExpiresTime;
							dados.ContentType = responseObject.ContentType;
							dados.ContentLength = File.Length;
							
							
							if (! string.IsNullOrEmpty(QueryString("fn")))
							{
								dados.FileName = QueryString("fn");
								if (! dados.FileName.Contains("."))
								{
									dados.FileName += "." + IO.BinaryFile.GetExtension(dados.ContentType);
								}
							}
							else
							{
								dados.FileName = valorI.ToString() + "." + IO.BinaryFile.GetExtension(dados.ContentType);
							}
							
							
							if (! string.IsNullOrEmpty(QueryString("dl")))
							{
								try
								{
									dados.ForceDownload = Convert.ToBoolean(int.Parse(QueryString("dl")));
								}
								catch (Exception)
								{
								}
							}
							
							if (string.IsNullOrEmpty(dados.ContentType))
							{
								dados.ContentType = GetMimeType(File);
							}
							
							WriteHeaders(app, dados);
							WriteStream(File, app);
							
							
							dados.Object = File;
                            if (app.Request[Tenor.Configuration.TenorModule.NoCache] == string.Empty)
							{
								lock(messagesLock)
								{
									object obj;
									obj = Cache.Add("instance:" + app.Request.QueryString.ToString(), dados, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Default, new System.Web.Caching.CacheItemRemovedCallback(Cache_onItemRemoved));
								}
							}
							
						}
						catch (Exception)
						{
							//app.Context.ClearError()
							//app.Context.AddError(New HttpException(500, "server error", ex.InnerException))
							//app.Context.Response.StatusCode = 500
							throw;
						}
					}
				}
			}
			
			
			
			
		}
		
		
	}
}
