using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;


namespace Tenor.Mail
{

    /// <summary>
    /// The MailMessage class can be used to send emails using either text and html templates.
    /// </summary>
    public class MailMessage : System.Net.Mail.MailMessage
    {

        #region " Contructors "

        /// <summary>
        /// </summary>
        public MailMessage()
        {
        }

        /// <param name="template">A Stream with the desired template.</param>
        /// <seealso cref="Template" />
        public MailMessage(Stream template)
        {
            byte[] buffer = new byte[System.Convert.ToInt32(template.Length) + 1];
            template.Read(buffer, 0, (int)template.Length);
            _Template = System.Text.Encoding.UTF8.GetString(buffer);
        }

        /// <param name="template">A plain string with the template.</param>
        /// <seealso cref="Template" />
        public MailMessage(string template)
        {
            _Template = template;
        }

        /// <param name="fileName">A full path of a file that contains a template.</param>
        /// <param name="detectEncoding">If true, tries to detect encoding automatically.</param>
        /// <seealso cref="Template" />
        public MailMessage(string fileName, bool detectEncoding)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName, detectEncoding);

            this.BodyEncoding = stream.CurrentEncoding;
            _Template = stream.ReadToEnd();
            stream.Close();
        }

        /// <param name="fileName">A full path of a file that contains a template.</param>
        /// <param name="encoding">Defines the encoding used to open the template.</param>
        /// <seealso cref="Template" />
        public MailMessage(string fileName, System.Text.Encoding encoding)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName, encoding);

            this.BodyEncoding = stream.CurrentEncoding;
            _Template = stream.ReadToEnd();
            stream.Close();
        }

        /// <param name="template">A Stream with the desired template.</param>
        /// <param name="values">A dictionary with keys and values to be replaced on the template.</param>
        /// <seealso cref="Template" />
        public MailMessage(Stream template, System.Collections.Generic.Dictionary<string, string> values)
            : this(template)
        {
            _TemplateValues = values;
        }

        /// <param name="template">A plain string with the template.</param>
        /// <param name="values">A dictionary with keys and values to be replaced on the template.</param>
        /// <seealso cref="Template" />
        public MailMessage(string template, System.Collections.Generic.Dictionary<string, string> values)
            : this(template)
        {
            _TemplateValues = values;
        }


        /// <param name="template">A plain string with the template.</param>
        /// <param name="values">A dictionary with keys and values to be replaced on the template.</param>
        /// <param name="from">A MailAddress that identifies from.</param>
        /// <param name="replyTo">A MailAddress that this message could be replied.</param>
        /// <param name="to">A MailAddress with destination.</param>
        /// <seealso cref="Template" />
        public MailMessage(
            string template,
            System.Collections.Generic.Dictionary<string, string> values, 
            MailAddress from, MailAddress[] to, MailAddress replyTo)
            : this(template, values)
        {
            this.To.Clear();
            foreach (MailAddress i in to)
            {
                this.To.Add(i);
            }
            this.From = from;
            this.ReplyTo = replyTo;
        }

        #endregion



        private string _Template;

        /// <summary>
        /// Gets or sets the plain or html text template.
        /// </summary>
        /// <remarks>
        /// Must contains a template text with the desired message. This text can be and html, xhtml, or plain text.
        /// You can provide keys and values of this template on <see cref="TemplateValues"/>. 
        /// Set these keys on this template using the following notation: 
        /// <code>[[[keyname]]]</code>
        /// All key names must be in lower case.
        /// <example>
        /// Hello [[[name]]].
        /// This is a test message sent on [[[date]]];
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
        /// Gets a list of keys and values of the current template.
        /// </summary>
        /// <remarks>
        /// This property is useful when you have a <see cref="Template"/> set using keys on it.
        /// Provide key names without brackets.
        /// <example>
        /// obj.TemplateValues.Add("name", "John");
        /// obj.TemplateValues.Add("date", DateTime.Today.ToString());
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
        /// Prepare the message body using the current template merged with provided values.
        /// </summary>
        protected virtual void PrepareTemplate()
        {
            if (!string.IsNullOrEmpty(Template))
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

        /// <summary>
        /// Gets a boolean indicating if a content have html code.
        /// </summary>
        /// <param name="content">The original content.</param>
        /// <returns>True if content is html.</returns>
        protected bool IsHtml(string content)
        {
            //TODO: Use regular expressions to detect any html tags.
            return content.StartsWith("<html", StringComparison.OrdinalIgnoreCase) || content.ToLower().Contains("\r\n" + "<html") || content.ToLower().Contains("<body") || Text.Strings.RemoveHTML(content.ToLower()).Length < content.Length;
        }

        /// <summary>
        /// Sends this message using the default smtp.
        /// </summary>
        public void Send()
        {
            Send(null, 0);
        }


        /// <summary>
        /// Sends this message using provided smtp server.
        /// </summary>
        /// <param name="smtpServer">A smtp server.</param>
        /// <param name="smtpPort">The port to be used. 25 is the default port of unsecured smtp servers.</param>
        /// <remarks>
        /// </remarks>
        public virtual void Send(string smtpServer, int smtpPort)
        {
            Send(smtpServer, smtpPort, null, null);
        }

        /// <summary>
        /// Sends this message using provided smtp server.
        /// </summary>
        /// <param name="smtpServer">A smtp server.</param>
        /// <param name="smtpPort">The port to be used. 25 is the default port of unsecured smtp servers.</param>
        /// <param name="userName">The username used to authenticate on smtp server.</param>
        /// <param name="password">The password used to authenticate on smtp server.</param>
        public virtual void Send(string smtpServer, int smtpPort, string userName, string password)
        {
            Send(smtpServer, smtpPort, userName, password, false);
        }

        /// <summary>
        /// Sends this message using provided smtp server.
        /// </summary>
        /// <param name="smtpServer">A smtp server.</param>
        /// <param name="smtpPort">The port to be used. 25 is the default port of unsecured smtp servers.</param>
        /// <param name="userName">The username used to authenticate on smtp server.</param>
        /// <param name="password">The password used to authenticate on smtp server.</param>
        /// <param name="useSSL">Determines if an ssl attemp will be made.</param>
        public virtual void Send(string smtpServer, int smtpPort, string userName, string password, bool useSSL)
        {

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            //smtp.UseDefaultCredentials = False

            if (!string.IsNullOrEmpty(smtpServer))
            {
                smtp.Host = smtpServer;
            }

            if (smtpPort > 0)
            {
                smtp.Port = smtpPort;
            }

            if ((userName != null) && (password != null))
            {
                smtp.Credentials = new System.Net.NetworkCredential(userName, password);
            }
            smtp.EnableSsl = useSSL;
            try
            {
                PrepareTemplate();
                //To avoid this message to be moved to spam.
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
        /// Converts a list of semi-colon separated emails into an array of MailAddress.
        /// </summary>
        /// <param name="source">Semi-colon separated emails.</param>
        /// <exception cref="FormatException" />
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