using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
	namespace Web
	{
		/// <summary>
		/// Interface para controles que respondem Ã  chamadas subseqÃ¼Ãªntes do navegador.
		/// </summary>
		/// <remarks></remarks>
		public interface IResponseObject
		{
			/// <summary>
			/// Propriedade que define o tipo de conteÃºdo da chamada
			/// </summary>
			/// <returns>MIMEType do conteÃºdo da chamada</returns>
			/// <remarks></remarks>
			string ContentType{
				get;
			}
			
			/// <summary>
			/// FunÃ§Ã£o chamada para retornar o conteÃºdo da chamada em formato de Stream
			/// </summary>
			/// <returns>Stream com o conteÃºdo da chamada</returns>
			/// <remarks></remarks>
			Stream WriteContent();
			
		}
		
		/// <summary>
		/// Determina que a propriedade contem um item IResponseObject usado para referencias do HttpModule.
		/// </summary>
		/// <remarks></remarks>
		[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]public class ResponsePropertyAttribute : Attribute
		{
			
			
			/// <summary>
			/// Inicializa uma instancia do ResponsePropertyAttribute.
			/// Esta classe define que esta propriedade deverÃ¡ retornar um iResponseObject para uso do HttpModule.
			/// </summary>
			/// <remarks></remarks>
			public ResponsePropertyAttribute()
			{
				
			}
		}
		
		/// <summary>
		/// Interface para controles modulares.
		/// </summary>
		/// <remarks></remarks>
		public interface IModule
		{
			
			
			void Init(System.Configuration.ConnectionStringSettings Connection);
			
			
			string Title{
				get;
			}
			
			bool DisplayLink{
				get;
			}
			
			
		}
		
		
		public class @Module
		{
			
			private @Module()
			{
			}
			
			public static string GetModuleUrl(Type AscxType)
			{
				string File = AscxType.Assembly.Location;
				
				string className = AscxType.FullName;
				if (! className.EndsWith("_ascx"))
				{
					className += "_ascx";
				}
				if (! className.StartsWith("ASP."))
				{
					className = "ASP." + className;
				}
				
				return string.Format("~/Publicador/Module.aspx?m={0}&t={1}", System.Web.HttpUtility.UrlEncode(new System.IO.FileInfo(File).Name), System.Web.HttpUtility.UrlEncode(className));
			}
		}
		
	}
	
}
