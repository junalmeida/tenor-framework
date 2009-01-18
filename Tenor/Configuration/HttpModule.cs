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
		internal class TenorModule
		{
			
			
			/// <summary>
			/// ContÃ©m o tempo padrÃ£o para expirar a informaÃ§Ã£o
			/// </summary>
			/// <remarks></remarks>
			public const int DefaultExpiresTime = 1 * 60 * 60;
			/// <summary>
			/// ContÃ©m o nome do falso arquivo que irÃ¡ ativar este HttpModule
			/// </summary>
			/// <remarks></remarks>
			public const string HandlerFileName = "Tenor.axd";
			/// <summary>
			/// ContÃ©m o prefixo das chaves geradas.
			/// </summary>
			/// <remarks></remarks>
			public const string IdPrefix = "__TENOR_";
			
			/// <summary>
			/// ContÃ©m o prefixo que armazena as chaves.
			/// </summary>
			/// <remarks></remarks>
			public const string CacheKeys = IdPrefix + "KEYS";
			
			/// <summary>
			/// ContÃ©m o nome NoCache da queryString.
			/// </summary>
			/// <remarks></remarks>
			public const string NoCache = "nocache";
			
		}
	}
}
