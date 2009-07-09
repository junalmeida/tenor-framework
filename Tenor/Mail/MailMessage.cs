using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;


namespace Tenor
{
	namespace Mail
	{
		
		/// <summary>
		/// Envia emails utilizando modelos em texto ou html.
		/// </summary>
		/// <remarks></remarks>
		public class MailMessage : System.Net.Mail.MailMessage
		{
			
			
			
			
            //public static MailAddress FromPadrao(string DisplayName)
            //{
            //    return new MailAddress(Configuration.MailMessage._FromPadrao, DisplayName);
            //}
			
			#region " Construtores "
			
			/// <summary>
			/// Inicializa uma nova instância da classe MailMessage
			/// </summary>
			/// <remarks></remarks>
			public MailMessage()
			{
			}
			
			/// <summary>
			/// Inicializa uma nova instância da classe MailMessage
			/// </summary>
			/// <param name="Template">Um Stream com o modelo desejado</param>
			/// <remarks><seealso cref="Template">Propriedade Template</seealso></remarks>
			public MailMessage(Stream Template)
			{
				byte[] buffer = new byte[System.Convert.ToInt32(Template.Length) + 1];
				Template.Read(buffer, 0, (int) Template.Length);
				_Template = System.Text.Encoding.UTF8.GetString(buffer);
			}
			
			/// <summary>
			/// Inicializa uma nova instância da classe MailMessage
			/// </summary>
			/// <param name="Template">Um String com o modelo desejado</param>
			/// <remarks><seealso cref="Template">Propriedade Template</seealso></remarks>
			public MailMessage(string Template)
			{
				_Template = Template;
			}
			
			/// <summary>
			/// Inicializa uma nova instância da classe MailMessage
			/// </summary>
			/// <param name="FileName">Especifíca o caminho completo do arquivo de modelo.</param>
			/// <param name="DetectEncoding">Indica se  deve tentar detectar o econding pela análise do arquivo.</param>
			/// <remarks><seealso cref="Template">Propriedade Template</seealso></remarks>
			public MailMessage(string FileName, bool DetectEncoding)
			{
				System.IO.StreamReader stream = new System.IO.StreamReader(FileName, DetectEncoding);
				
				this.BodyEncoding = stream.CurrentEncoding;
				_Template = stream.ReadToEnd();
				stream.Close();
			}
			
			/// <summary>
			/// Inicializa uma nova instância da classe MailMessage
			/// </summary>
			/// <param name="FileName">Especifíca o caminho completo do arquivo de modelo.</param>
			/// <param name="Encoding">Especifica o Encoding usado</param>
			/// <remarks><seealso cref="Template">Propriedade Template</seealso></remarks>
			public MailMessage(string FileName, System.Text.Encoding Encoding)
			{
				System.IO.StreamReader stream = new System.IO.StreamReader(FileName, Encoding);
				
				this.BodyEncoding = stream.CurrentEncoding;
				_Template = stream.ReadToEnd();
				stream.Close();
			}
			
			/// <summary>
			/// Inicializa uma nova instância da classe MailMessage
			/// </summary>
			/// <param name="Template">Um Stream com o modelo desejado</param>
			/// <param name="Values">Coleção de chaves e valores para substituição no modelo.</param>
			/// <remarks><seealso cref="Template">Propriedade Template</seealso></remarks>
			public MailMessage(Stream Template, System.Collections.Generic.Dictionary<string, string> Values) : this(Template)
			{
				_TemplateValues = Values;
			}
			
			/// <summary>
			/// Inicializa uma nova instância da classe MailMessage
			/// </summary>
			/// <param name="Template">Um String com o modelo desejado</param>
			/// <param name="Values">Coleção de chaves e valores para substituição no modelo.</param>
			/// <remarks></remarks>
			public MailMessage(string Template, System.Collections.Generic.Dictionary<string, string> Values) : this(Template)
			{
				_TemplateValues = Values;
			}
			
			/// <summary>
			/// Inicializa uma nova instância da classe MailMessage
			/// </summary>
			/// <param name="Template">Um String com o modelo desejado</param>
			/// <param name="Values">Coleção de chaves e valores para substituição no modelo.</param>
			/// <remarks></remarks>
			public MailMessage(string Template, System.Collections.Generic.Dictionary<string, string> Values, MailAddress From, MailAddress[] @To, MailAddress ReplyTo) : this(Template, Values)
			{
				this.To.Clear();
				foreach (MailAddress i in @To)
				{
					this.To.Add(i);
				}
				this.From = From;
				this.ReplyTo = ReplyTo;
			}
			
			#endregion
			
			
			
			private string _Template;
			
			/// <summary>
			/// Contém o modelo desejado para esta mensagem.
			/// </summary>
			/// <value>Um String</value>
			/// <returns>Um String</returns>
			/// <remarks>
			/// Deve conter um texto modelo para a mensagem desejada. Este texo pode ser do tipo html, xhtml ou texto plano.
			/// Você deve também fornecer os valores para os campos deste modelo na propriedade <see cref="TemplateValues">TemplateValues</see>. Defina os campos nesse modelo usando a seguinte notação: <code>[[[chave]]]</code>.
			/// Utilize campos com letras minúsculas.
			/// Veja o exemplo a seguir:
			/// <example>
			/// Olá [[[nome]]].
			/// Esta é uma mensagem de teste enviada em [[[data]]].
			/// </example>
			/// </remarks>
			public string Template
			{
				get
				{
					return _Template;
				}
				set
				{
					_Template = value;
				}
			}
			
			private System.Collections.Generic.Dictionary<string, string> _TemplateValues;
			/// <summary>
			/// Contém uma lista de chaves e valores para o modelo desejado
			/// </summary>
			/// <value></value>
			/// <returns>
			/// </returns>
			/// <remarks>
			/// Esta propriedade só é usada quando um modelo foi definido na propriedade <see cref="Template">Template</see>.
			/// Você deve especificar as chaves sem os colchetes delimitadores do modelo.
			/// <example>
			/// obj.TemplateValues.Add("nome", "fulano")
			/// </example>
			/// </remarks>
			public System.Collections.Generic.Dictionary<string, string> TemplateValues
			{
				get
				{
					if (_TemplateValues == null)
					{
						_TemplateValues = new System.Collections.Generic.Dictionary<string, string>();
					}
					
					return _TemplateValues;
				}
			}
			
			/// <summary>
			/// Prepara a propriedade Body para conter o modelo mesclado com os valores.
			/// </summary>
			/// <remarks></remarks>
			protected virtual void PrepareTemplate()
			{
				if (! string.IsNullOrEmpty(Template))
				{
					this.Body = Template;
					this.IsBodyHtml = IsHtml(Body);
					foreach (string key in TemplateValues.Keys)
					{
						string valor = TemplateValues[key];
						if (this.IsBodyHtml)
						{
							valor = Text.Strings.EncodeAccentuation(valor);
						}
						this.Body = this.Body.Replace("[[[" + key + "]]]", valor);
					}
				}
				this.IsBodyHtml = IsHtml(Body);
			}
			
			protected bool IsHtml(string Content)
			{
				return Content.StartsWith("<html", StringComparison.OrdinalIgnoreCase) || Content.ToLower().Contains("\r\n" + "<html") || Content.ToLower().Contains("<body") || Text.Strings.RemoveHTML(Content.ToLower()).Length < Content.Length;
			}
			
			/// <summary>
			/// Envia a mensagem usando a configuração SMTP padrão.
			/// </summary>
			/// <remarks></remarks>
			public void Send()
			{
				Send(null, 0);
			}
			
			
			/// <summary>
			/// Envia a mensagem usando a configuração SMTP especificada.
			/// </summary>
			/// <param name="SmtpServer">Endereço do servidor SMTP</param>
			/// <param name="SmtpPort">Porta usada para a comunicação com o servidor.</param>
			/// <remarks>
			/// </remarks>
			public virtual void Send(string SmtpServer, int SmtpPort)
			{
				Send(SmtpServer, SmtpPort, null, null);
			}
			
			/// <summary>
			/// Envia a mensagem usando a configuração SMTP especificada.
			/// </summary>
			/// <param name="SmtpServer">Endereço do servidor SMTP</param>
			/// <param name="SmtpPort">Porta usada para a comunicação com o servidor.</param>
			/// <remarks>
			/// </remarks>
			public virtual void Send(string SmtpServer, int SmtpPort, string UserName, string Password)
			{
				Send(SmtpServer, SmtpPort, UserName, Password, false);
			}
			
			/// <summary>
			/// Envia a mensagem usando a configuração SMTP especificada.
			/// </summary>
			/// <param name="SmtpServer">Endereço do servidor SMTP</param>
			/// <param name="SmtpPort">Porta usada para a comunicação com o servidor.</param>
			/// <remarks>
			/// </remarks>
			public virtual void Send(string SmtpServer, int SmtpPort, string UserName, string Password, bool UseSSL)
			{
				
				System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
				//smtp.UseDefaultCredentials = False
				
				if (! string.IsNullOrEmpty(SmtpServer))
				{
					smtp.Host = SmtpServer;
				}
				
				if (SmtpPort > 0)
				{
					smtp.Port = SmtpPort;
				}
				
				if ((UserName != null)&& (Password != null))
				{
					smtp.Credentials = new System.Net.NetworkCredential(UserName, Password);
				}
				smtp.EnableSsl = UseSSL;
				/*
				this is not useful
				if (string.IsNullOrEmpty(smtp.Host) || smtp.Host.Equals("127.0.0.1"))
				{
					// Seta o padrão
					smtp.Host = Configuration.MailMessage._SmtpPadrao;
					smtp.Port = Configuration.MailMessage._SmtpPortaPadrao;
					smtp.EnableSsl = Configuration.MailMessage._SmtpUseSSL;
					smtp.Credentials = new System.Net.NetworkCredential(Configuration.MailMessage._UsuarioPadrao, Configuration.MailMessage._SenhaPadrao);
				}
				*/
				try
				{
					PrepareTemplate();
					this.Headers.Add("Precedence", "bulk");
					smtp.Send(this);
				}
				catch
				{
					throw;
				}
				finally
				{
					smtp = null;
				}
				
			}
			
			
			/// <summary>
			/// Converte uma lista de emails separados por ponto e virgula para um array de MailAddress
			/// </summary>
			/// <param name="source">Endereços de email separados por ponto e vírgula</param>
			/// <returns></returns>
			/// <remarks></remarks><exception cref="FormatException">Ocorre quando há um ou mais emails inválidos.</exception>
			public static System.Net.Mail.MailAddress[] ParseMailAddresses(string source)
			{
				List<MailAddress> res = new List<MailAddress>();
				foreach (string email in (source + ";").Split(';'))
				{
					if (email.Trim() != string.Empty)
					{
						res.Add(new MailAddress(email.Trim()));
					}
				}
				return res.ToArray();
			}
			
		}
		
	}
	
}
