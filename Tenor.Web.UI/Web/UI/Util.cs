using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Web.Configuration;



namespace Tenor
{
	namespace Web
	{
		
		/// <summary>
		/// Exceção usada quando o módulo não estiver carregado.
		/// </summary>
		/// <remarks></remarks>
		public class ModuleNotFoundException : Exception
		{
			
			
			public override string Message
			{
				get
				{
					return "You must have Web.HttpModule running to use this resource. Add a reference to httpModules section on your web.config file. <httpModules><add name=\"Tenor\" type=\"Tenor.Web.HttpModule, Tenor\"/></httpModules>";
				}
			}
		}
		
	}
	
	
}
