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
		
		public class MailMessage
		{
			
			internal const string _FromPadrao = "disparo@";
			internal const string _SmtpPadrao = "smtp.gmail.com";
			internal const string _UsuarioPadrao = "disparo@";
			internal const string _SenhaPadrao = "";
			internal const int _SmtpPortaPadrao = 587;
			internal const bool _SmtpUseSSL = true;
			
			
			public const int MaxCharNome = 50;
			public const int MaxCharEmail = 80;
			public const int MaxCharTelefone = 50;
			public const int MaxCharCPF = 50;
			public const int MaxCharTemplateKey = 120;
			public const int MaxCharTemplateValue = 4000;
			
			
			public static readonly string[] Verifica = new string[] {"verifica@tenorframework.com"};
		}
	}
	
}
