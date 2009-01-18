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
	namespace Configuration
	{
		public class Diagnostics
		{
			
			
			public const string WebConfigKey = "exception";
			/// <summary>
			/// Contém uma lista de emails usados em ambiente de teste.
			/// Uso Interno.
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string[] DebugEmails
			{
				get
				{
					return new string[] {};
				}
			}
			
			
		}
	}
	
}
