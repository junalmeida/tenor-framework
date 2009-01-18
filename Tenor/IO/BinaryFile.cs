using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
	namespace IO
	{
		public class BinaryFile : Web.IResponseObject
		{
			
			
			/// <summary>
			/// Determina o Mime Type através da extensão do arquivo
			/// </summary>
			/// <param name="path">Nome do arquivo para determinar o tipo.</param>
			/// <returns>Uma String com o mime type sugerido</returns>
			/// <remarks>
			/// Utiliza o arquivo mime.xml para determinar o mimetype através da extensão do arquivo.
			/// </remarks>
			public static string GetContentType(string path)
			{
				string ext = new System.IO.FileInfo(path).Extension.ToLower().Substring(1);
				
				Stream file = (Stream) (typeof(BinaryFile).Assembly.GetManifestResourceStream(Tenor.Configuration.Resources.MimeXML));
				System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
				xml.Load(file);
				
				System.Xml.XmlNodeList nodes = xml.SelectNodes("MimeTypes/mimetype[@ext=\'" + ext + "\']");
				if (nodes.Count > 0)
				{
					return nodes[0].InnerText;
				}
				else
				{
					return "application/octet-stream";
				}
				
			}
			
			/// <summary>
			/// Retorna a extensão associada ao mime type.
			/// </summary>
			/// <param name="MimeType">Mime Type desejado</param>
			/// <returns>Extensão sem o ponto.</returns>
			/// <remarks></remarks>
			public static string GetExtension(string MimeType)
			{
				Stream file = (Stream) (typeof(BinaryFile).Assembly.GetManifestResourceStream(Configuration.Resources.MimeXML));
				System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
				xml.Load(file);
				
				System.Xml.XmlNodeList nodes = xml.SelectNodes("MimeTypes/mimetype[text() = \'" + MimeType + "\']");
				if (nodes.Count > 0)
				{
					return nodes[0].Attributes["ext"].Value;
				}
				else
				{
					return string.Empty;
				}
				
			}
			
			
			
			/// <summary>
			/// Converte uma Stream de qualquer tipo em bytes de forma segura e eficiente.
			/// </summary>
			/// <param name="stream">Stream que deseja converter</param>
			/// <returns>Array de Byte</returns>
			/// <remarks></remarks>
			public static byte[] StreamToBytes(Stream stream)
			{
				
				stream.Seek(0, SeekOrigin.Begin);
				
				byte[] data = new byte[System.Convert.ToInt32(stream.Length)];
				int offset = 0;
				int remaining = (int) stream.Length;
				while (remaining > 0)
				{
					
					int read = stream.Read(data, offset, remaining);
					if (read <= 0)
					{
						break;
					}
					remaining -= read;
					offset += read;
				}
				
				return data;
			}
			
			/// <summary>
			/// Retorna a stream representada por este BinaryFile
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public virtual Stream GetStream()
			{
				MemoryStream obj = new MemoryStream(_buffer);
				return obj;
			}
			
			
			public BinaryFile(byte[] Buffer, string ContentType)
			{
				_buffer = Buffer;
				_ContentType = ContentType;
			}
			
			//Private _Expires As Integer
			///' <summary>
			///' Determina em quanto tempo a informação deste objeto expira
			///' </summary>
			///' <value></value>
			///' <returns></returns>
			///' <remarks></remarks>
			//Public Property Expires() As Integer
			//    Get
			//        Return _Expires
			//    End Get
			//    Set(ByVal value As Integer)
			//        _Expires = value
			//    End Set
			//End Property
			
			
			protected byte[] _buffer;
			protected string _ContentType = "application/octect-stream";
			
			public string ContentType
			{
				get
				{
					return _ContentType;
				}
				set
				{
					_ContentType = value;
				}
			}
			
			//			public string ContentType
			//			{
				//				get
				//				{
					//					return this.IContentType;
					//				}
					//			}
					
					public string IContentType
					{
						get
						{
							return _ContentType;
						}
					}
					
					virtual public System.IO.Stream WriteContent()
					{
						MemoryStream mem = new MemoryStream(_buffer);
						return mem;
					}
					
					
					/// <summary>
					/// Registra uma url para exibição do arquivo na web
					/// </summary>
					/// <param name="Page">Página atual</param>
					/// <returns></returns>
					/// <remarks>Registra a imagem com 1 hora de uso</remarks>
					public string GetFileUrl(System.Web.UI.Page Page)
					{
						return GetFileUrl(Page, Tenor.Configuration.TenorModule.DefaultExpiresTime);
					}
					
					/// <summary>
					/// Registra uma url para exibição do arquivo na web
					/// </summary>
					/// <param name="Page">Página atual</param>
					/// <param name="Expires">Tempo para expirar em segundos</param>
					/// <returns></returns>
					/// <remarks></remarks>
					public string GetFileUrl(System.Web.UI.Page Page, int Expires)
					{
                        return Tenor.Web.TenorModule.RegisterObjectForRequest(this, Expires);
					}
					
					/// <summary>
					/// Registra uma url para exibição do arquivo na web
					/// </summary>
					/// <param name="Context">HttpContext atual</param>
					/// <returns></returns>
					/// <remarks></remarks>
					public string GetFileUrl(System.Web.HttpContext Context)
					{
                        return GetFileUrl(Context, Tenor.Configuration.TenorModule.DefaultExpiresTime);
					}
					
					/// <summary>
					/// Registra uma url para exibição do arquivo na web
					/// </summary>
					/// <param name="Context">HttpContext atual</param>
					/// <param name="Expires">Tempo para expirar em segundos</param>
					/// <returns></returns>
					/// <remarks></remarks>
					public string GetFileUrl(System.Web.HttpContext Context, int Expires)
					{
                        return Tenor.Web.TenorModule.RegisterObjectForRequest(this, Context, Expires, false, null);
					}
					
					/// <summary>
					/// Registra uma url para exibição do arquivo na web
					/// </summary>
					/// <param name="Context">HttpContext atual</param>
					/// <param name="Expires">Tempo para expirar em segundos</param>
					/// <param name="ForceDownload">Determina se deve ou não forçar o download do conteúdo pelo navegador do cliente.</param>
					/// <param name="FileName">Determina o nome do arquivo exibido pelo navegador</param>
					/// <returns></returns>
					/// <remarks></remarks>
					public string GetFileUrl(System.Web.HttpContext Context, int Expires, bool ForceDownload, string FileName)
					{
                        return Tenor.Web.TenorModule.RegisterObjectForRequest(this, Context, Expires, ForceDownload, FileName);
					}
					
					/// <summary>
					/// Registra uma url para exibição do arquivo na web
					/// </summary>
					/// <param name="Context">HttpContext atual</param>
					/// <param name="ForceDownload">Determina se deve ou não forçar o download do conteúdo pelo navegador do cliente.</param>
					/// <param name="FileName">Determina o nome do arquivo exibido pelo navegador</param>
					/// <returns></returns>
					/// <remarks></remarks>
					public string GetFileUrl(System.Web.HttpContext Context, bool ForceDownload, string FileName)
					{
                        return Tenor.Web.TenorModule.RegisterObjectForRequest(this, Context, Tenor.Configuration.TenorModule.DefaultExpiresTime, ForceDownload, FileName);
					}
				}
				
			}
			
		}
