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
			
			
			private void IEFixRequest(HttpApplication app)
			{
				//IEFix
				//Este trecho verifica se a requisição é específica ao ie fix
				
				//Carrega a assembly do ie fix
				Assembly webUI = @Assembly.Load(new System.Reflection.AssemblyName(Configuration.Resources.AssemblyWebUI));
				if (webUI == null)
				{
					return;
				}
				
				//pega o content type apartir da extensão do arquivo
				string contenttype = IO.BinaryFile.GetContentType(app.Request.Path);
				
				string pasta = "/iefix/";
				
				string filepath = app.Request.Path.Substring(app.Request.Path.ToLower().IndexOf(pasta) + pasta.Length);
				
				UnmanagedMemoryStream file = (UnmanagedMemoryStream) (webUI.GetManifestResourceStream(Configuration.Resources.AssemblyRoot + "." + filepath));
				if (file == null || file.Length == 0)
				{
					return; //404
				}
				Dados dados = new Dados();
				
				if (string.IsNullOrEmpty(contenttype))
				{
					dados.ContentType = GetMimeType(file);
					if (string.IsNullOrEmpty(app.Response.ContentType))
					{
						dados.ContentType = "text/plain";
					}
				}
				else
				{
					dados.ContentType = contenttype;
				}
				WriteHeaders(app, dados);
				
				WriteStream(file, app);
				file.Close();
			}
			
			
			
			
		}
		
		
	}
}
