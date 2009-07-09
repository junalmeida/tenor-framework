using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.Data.Common;
using System.Text;


namespace Tenor.Diagnostics
{
    public class Debug
    {


        private Debug()
        {
        }

        /// <summary>
        /// Imprime na saída da depuração os tempos de inicio e final da execução da página. Útil para calcular performance.
        /// </summary>
        /// <param name="page"></param>
        /// <remarks></remarks>
        public static void PrintTimings(Page page)
        {
            page.PreInit += new EventHandler(PrintTimings_PreInit);
            page.PreRenderComplete += new EventHandler(PrintTimings_PreRenderComplete);
        }

        private static void PrintTimings_PreInit(object sender, EventArgs e)
        {
            Page p = sender as Page;
            if (p != null)
            {
                DateTime d = DateTime.Now;
                string res = p.AppRelativeVirtualPath + (" - StartTime:  " + d.ToString() + ":" + d.Millisecond.ToString());
                System.Diagnostics.Debug.WriteLine(res);
                p.Response.Write(res);
            }
        }

        private static void PrintTimings_PreRenderComplete(object sender, EventArgs e)
        {
            Page p = sender as Page;
            if (p != null)
            {
                DateTime d = DateTime.Now;
                string res = p.AppRelativeVirtualPath + (" - FinishTime: " + d.ToString() + ":" + d.Millisecond.ToString());
                System.Diagnostics.Debug.WriteLine(res);
                p.Response.Write("<br />" + res);
            }
        }


        /// <summary>
        /// Imprime na saída como comentário a data e hora atual junto com o texto passado.
        /// </summary>
        /// <param name="Text"></param>
        /// <remarks></remarks>
        public static void PrintCurrentTime(string Text)
        {
            HttpContext context = HttpContext.Current;
            DateTime data = DateTime.Now;
            if ((context != null) && (context.Response != null))
            {
                HttpResponse response = context.Response;
                response.Write(string.Format("<!-- {0} - {1} -->" + "\r\n", Text, data.ToString() + ":" + data.Millisecond.ToString()));
            }
            else
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine(Text + " - " + (data.ToString() + ":" + data.Millisecond.ToString()));
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Envia a Exceção para os emails do ambiente de teste.
        /// </summary>
        /// <param name="exception"></param>
        /// <remarks></remarks>
        public static void HandleError(Exception exception)
        {
            if (HttpContext.Current == null)
            {
                HandleError(null, exception, true);
            }
            else
            {
                HandleError(HttpContext.Current.ApplicationInstance, exception, true);
            }
        }


        internal static void HandleError(HttpApplication app, Exception exception, bool HandledByUser)
        {
            try
            {


                string requestUrl = "";
                if (app != null)
                {
                    requestUrl = app.Request.Url.GetLeftPart(UriPartial.Authority) + app.Request.RawUrl;
                }


                //Dim Page As System.Web.UI.Page = Nothing

                //If app.Context.CurrentHandler IsNot Nothing AndAlso TypeOf (app.Context.CurrentHandler) Is System.Web.UI.Page Then
                //    Page = TryCast(app.Context.CurrentHandler, UI.Page)
                //End If

                Mail.MailMessage errmail = new Mail.MailMessage();
                //errmail.From = Mail.MailMessage.FromPadrao("Exception")

                foreach (Tenor.Configuration.EmailElement email in Tenor.Configuration.Tenor.Current.Exceptions.Emails)
                {
                    errmail.To.Add(new System.Net.Mail.MailAddress(email.Email, email.Name));
                }

                //string webconfigemails = System.Configuration.ConfigurationManager.AppSettings[Tenor.Configuration.Diagnostics.WebConfigKey];
                //if (!string.IsNullOrEmpty(webconfigemails))
                //{
                //    if (webconfigemails.ToLower() == "false")
                //    {
                //        return;
                //    }
                //    else
                //    {
                //        string[] emails = webconfigemails.Split(',');


                //        foreach (string email in emails)
                //        {
                //            try
                //            {
                //                errmail.To.Add(new System.Net.Mail.MailAddress(email));
                //            }
                //            catch
                //            {

                //            }
                //        }
                //        if (errmail.To.Count == 0)
                //        {
                //            foreach (string email in Tenor.Configuration.Diagnostics.DebugEmails)
                //            {
                //                errmail.To.Add(new System.Net.Mail.MailAddress(email));
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (string email in Tenor.Configuration.Diagnostics.DebugEmails)
                //    {
                //        errmail.To.Add(new System.Net.Mail.MailAddress(email));
                //    }
                //}
                if (errmail.To.Count == 0)
                {
                    errmail.Dispose();
                    return;
                }

                errmail.Subject = (HandledByUser ? "Handled" : "Unhandled").ToString() + " Exception: " + requestUrl + " (" + DateTime.Now.ToString() + ")";
                errmail.IsBodyHtml = true;
                errmail.BodyEncoding = System.Text.Encoding.UTF8;
                try
                {
                    HttpException htmlex = exception as HttpException;
                    if (htmlex != null)
                    {
                        errmail.Body = htmlex.GetHtmlErrorMessage();
                    }
                }
                catch
                {

                }

                if (errmail.Body.Trim().Length < 10)
                {
                    string serverError = string.Empty;
                    if (app != null)
                    {
                        serverError = "Server Error in \'" + app.Request.ApplicationPath + "\' Application.";
                    }
                    else
                    {
                        serverError = "Server Error";
                        System.Reflection.Assembly ass = System.Reflection.Assembly.GetCallingAssembly();
                        if (ass != null)
                        {
                            serverError = "Server Error in \'" + ass.GetName().Name + "\' Application.";
                        }
                    }

                    //TODO: No futuro passar esse modelo para um arquivo externo de template.

                    System.Text.StringBuilder except = new System.Text.StringBuilder();

                    except.AppendLine("<html>");
                    except.AppendLine("<head>");
                    except.Append("   <title>");
                    except.Append(exception.Message);
                    except.AppendLine("</title>");
                    except.AppendLine("   <style type=\"text/css\">");
                    except.AppendLine("body {font-family:\"Verdana\";font-weight:normal;font-size: .7em;color:black;} ");
                    except.AppendLine("p {font-family:\"Verdana\";font-weight:normal;color:black;margin-top: -5px} ");
                    except.AppendLine("b {font-family:\"Verdana\";font-weight:bold;color:black;margin-top: -5px} ");
                    except.AppendLine("H1 { font-family:\"Verdana\";font-weight:normal;font-size:18pt;color:red } ");
                    except.AppendLine("H2 { font-family:\"Verdana\";font-weight:normal;font-size:14pt;color:maroon } ");
                    except.AppendLine("pre {font-family:\"Lucida Console\";font-size: .9em} ");
                    except.AppendLine(".marker {font-weight: bold; color: black;text-decoration: none;} ");
                    except.AppendLine(".version {color: gray;} ");
                    except.AppendLine(".error {margin-bottom: 10px;} ");
                    except.AppendLine(".expandable { text-decoration:underline; font-weight:bold; color:navy; cursor:hand; } ");
                    except.AppendLine("   </style>");
                    except.AppendLine("</head>");
                    except.AppendLine("<body bgcolor=\"white\">");
                    except.Append("<h1>");
                    except.Append(serverError);
                    except.AppendLine("<hr  width=\"100%\" size=\"1\" color=\"silver\" /></h1>");

                    except.Append("<h2><i>");
                    except.Append(exception.Message);
                    except.AppendLine("</i></h2>");

                    except.AppendLine("<b>Detalhes:</b><br /><br />");
                    except.AppendLine("<table width=100% bgcolor=\"#ffffcc\">");
                    except.AppendLine("   <tr>");
                    except.AppendLine("      <td>");
                    except.AppendLine("         <code><pre>");
                    except.AppendLine(HttpUtility.HtmlEncode(exception.ToString()));
                    except.AppendLine("         </pre></code>");
                    except.AppendLine("      </td>");
                    except.AppendLine("   </tr>");
                    except.AppendLine("</table>");

                    string st = Environment.StackTrace;
                    string fimDaondeNaoImporta = "HandleError(Exception exception)" + Environment.NewLine;
                    int i = st.IndexOf(fimDaondeNaoImporta);
                    if (i > 0)
                    {
                        st = st.Substring(i + fimDaondeNaoImporta.Length);
                    }
                    else
                    {
                        fimDaondeNaoImporta = "get_StackTrace()" + Environment.NewLine;
                        i = st.IndexOf(fimDaondeNaoImporta);
                        if (i > 0)
                        {
                            st = st.Substring(i + fimDaondeNaoImporta.Length);
                        }
                    }

                    except.AppendLine("<br /><b>Stack Trace:</b> <br /><br />");
                    except.AppendLine("<table width=\"100%\" bgcolor=\"#ffffcc\">");
                    except.AppendLine("   <tr>");
                    except.AppendLine("      <td>");
                    except.AppendLine("          <code><pre>");
                    except.AppendLine(st);
                    except.AppendLine("          </pre></code>");
                    except.AppendLine("      </td>");
                    except.AppendLine("   </tr>");
                    except.AppendLine("</table>");

                    except.AppendLine("</body>");
                    except.AppendLine("</html>");

                    errmail.Body = except.ToString();
                }

                string extraInfo = string.Empty;

                Exception ex = exception;
                while (ex != null)
                {
                    if (ex.Data.Count > 0)
                    {

                        extraInfo += "<br /><br /><b>Informações Adicionais: </b>";
                        if (ex != exception && (exception.GetType() != typeof(HttpUnhandledException)))
                        {
                            extraInfo += ex.GetType().FullName + ": " + ex.Message;
                        }

                        extraInfo += "<br /><br />";

                        extraInfo += "<table width=100% bgcolor=\"#ffffcc\">" + "\r\n" + "   <tr>" + "\r\n" + "      <td>" + "\r\n" + "         <code><pre>" + "\r\n";
                        try
                        {
                            foreach (DictionaryEntry d in ex.Data)
                            {
                                extraInfo += HttpUtility.HtmlEncode(d.Key.ToString() + "\r\n" + "\t" + d.Value.ToString()) + "\r\n" + "\r\n";
                            }
                        }
                        catch
                        {

                        }

                        extraInfo += "         </pre></code>" + "\r\n" + "      </td>" + "\r\n" + "   </tr>" + "\r\n" + "</table>" + "\r\n";

                    }
                    ex = ex.InnerException;
                }



                if (app != null)
                {
                    extraInfo += GetExtraInfoHtml(app);
                }
                extraInfo += GetServerInfoHtml();
                extraInfo += GetLoadedAssembliesHtml();

                errmail.Body = errmail.Body.Replace("</body>", extraInfo + "</body>");

                errmail.Send();

            }
            catch
            {

            }
        }


        private static string GetExtraInfoHtml(HttpApplication app)
        {
            string extraInfo = "";

            try
            {
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Informa&ccedil;&otilde;es do Usu&aacute;rio</h2><br />";
                if ((app.User != null) && (app.User.Identity != null))
                {
                    extraInfo += "<b>Autenticado: </b>" + app.User.Identity.IsAuthenticated.ToString() + "<br />";
                    if (app.User.Identity.IsAuthenticated)
                    {
                        extraInfo += "<b>User: </b>" + app.User.Identity.Name.ToString() + "<br />";
                        extraInfo += "<b>Type: </b>" + app.User.Identity.AuthenticationType.ToString() + "<br />";
                    }
                }
            }
            catch (Exception)
            {
            }
            try
            {
                extraInfo += "<b>Endere&ccedil;o: </b>" + app.Request.UserHostName + " (" + app.Request.UserHostAddress + ")" + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                if (app.Request.UrlReferrer != null)
                {
                    extraInfo += "<b>Url Anterior: </b>" + app.Request.UrlReferrer.ToString() + "<br />";
                }
            }
            catch (Exception)
            {
            }
            try
            {
                extraInfo += "<b>Tipo de Chamada: </b>" + app.Request.RequestType + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {

                int tamanho = app.Request.TotalBytes;
                string tamanhoS = string.Empty;
                if (tamanho < 1024)
                {
                    tamanhoS = tamanho.ToString() + " bytes";
                }
                else if (tamanho >= 1024 && tamanho < 1024 * 1024)
                {
                    tamanho = tamanho / 1024;
                    tamanhoS = tamanho.ToString() + " kb";
                }
                else if (tamanho > 1024 * 1024)
                {
                    tamanho = tamanho / 1024 / 1024;
                    tamanhoS = tamanho.ToString() + " mb";
                }
                extraInfo += "<b>Tamanho: </b>" + tamanhoS + "<br />";
            }
            catch (Exception)
            {
            }


            try
            {
                extraInfo += "<br /><hr width=\"100%\" size=\"1\" color=\"silver\" /><h2>Valores de Formul&aacute;rio</h2><br />";
                foreach (string item in app.Request.Form.AllKeys)
                {
                    extraInfo += "<b>" + item + ": </b>" + HttpUtility.HtmlEncode(app.Request.Form[item]) + "<br />";
                }
            }
            catch (Exception)
            {
            }
            try
            {
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Valores de Url</h2><br />";
                foreach (string item in app.Request.QueryString.AllKeys)
                {
                    extraInfo += "<b>" + item + ": </b>" + HttpUtility.HtmlEncode(app.Request.QueryString[item]) + "<br />";
                }
            }
            catch (Exception)
            {
            }
            try
            {
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Valores de Cookies</h2><br />";
                foreach (string item in app.Request.Cookies.AllKeys)
                {
                    if (!app.Request.Cookies[item].HasKeys)
                    {
                        extraInfo += "<b>" + item + ": </b>" + HttpUtility.HtmlEncode(app.Request.Cookies[item].Value) + "<br />";
                    }
                    else
                    {
                        extraInfo += "<b>" + item + ": </b><br />";
                        foreach (string i in app.Request.Cookies[item].Values.AllKeys)
                        {
                            extraInfo += "&nbsp;&nbsp;&nbsp;&nbsp;&gt;&nbsp;" + i + ": " + HttpUtility.HtmlEncode(app.Request.Cookies[item].Values[i]) + "<br />";
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            try
            {
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Vari&aacute;veis de Servidor</h2><br />";
                foreach (string item in app.Request.ServerVariables.AllKeys)
                {
                    extraInfo += "<b>" + item + ": </b>" + HttpUtility.HtmlEncode(app.Request.ServerVariables[item]) + "<br />";
                }
            }
            catch (Exception)
            {
            }

            try
            {
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Cache</h2><br />";
                extraInfo += "<b>Count: </b>" + app.Context.Cache.Count.ToString() + "<br />";
#if !MONO
                extraInfo += "<b>EffectivePrivateBytesLimit: </b>" + app.Context.Cache.EffectivePrivateBytesLimit.ToString("N2") + "<br />";
#endif
                foreach (DictionaryEntry item in app.Context.Cache)
                {
                    if (item.Value != null)
                    {
                        extraInfo += "<b>" + item.Key.ToString() + ": </b>" + item.Value.ToString() + "<br />";
                        if (item.Value.GetType() == typeof(Dictionary<object, BLL.BLLBase>))
                        {
                            Dictionary<object, BLL.BLLBase> dict = (Dictionary<object, BLL.BLLBase>)item.Value;
                            foreach (object key in dict.Keys)
                            {
                                extraInfo += "&nbsp;&nbsp;&gt;" + key.ToString() + ": " + dict[key].ToString() + "<br />";
                            }
                        }
                    }
                }


            }
            catch (Exception)
            {
            }
            try
            {
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Valores de Sess&atilde;o</h2><br />";
                foreach (string item in app.Session.Keys)
                {
                    if (app.Session[item] != null)
                    {
                        extraInfo += "<b>" + item + ": </b>" + HttpUtility.HtmlEncode(app.Session[item].GetType().FullName + " - " + app.Session[item].ToString()) + "<br />";
                    }
                }
            }
            catch (Exception)
            {
            }
            try
            {
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Valores de Aplicativo</h2><br />";
                foreach (string item in app.Application.Keys)
                {
                    if (app.Application[item] != null)
                    {
                        extraInfo += "<b>" + item + ": </b>" + HttpUtility.HtmlEncode(app.Application[item].GetType().FullName + " - " + app.Application[item].ToString()) + "<br />";
                    }
                }
            }
            catch (Exception)
            {
            }
            return extraInfo;
        }

        private static string GetServerInfoHtml()
        {
            string extrainfo = string.Empty;

            extrainfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Informa&ccedil;&otilde;es do servidor</h2><br />";
            try
            {
                extrainfo += "<b>Vers&atilde;o do IIS instalado: </b>" + Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\InetStp", false).GetValue("MajorVersion").ToString() + "." + Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\InetStp", false).GetValue("MinorVersion").ToString() + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                string loc = typeof(char).Assembly.Location.Replace("mscorlib.dll", "webengine.dll");
                System.Diagnostics.FileVersionInfo versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(loc);

                string info = string.Format("{0}.{1}.{2}.{3}", new object[] { versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart });

                extrainfo += "<b>Vers&atilde;o do ASP.NET: </b>" + info + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                string loc = typeof(char).Assembly.Location.Replace("mscorlib.dll", "mscorwks.dll");
                System.Diagnostics.FileVersionInfo versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(loc);

                string info = string.Format("{0}.{1}.{2}.{3}", new object[] { versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart });

                extrainfo += "<b>Vers&atilde;o do .NET Framework: </b>" + info + "<br />";
            }
            catch (Exception)
            {
            }

            try
            {
                extrainfo += "<b>Informa&ccedil;&otilde;es do processo: </b>" + (System.Diagnostics.Process.GetCurrentProcess().MainModule.FileVersionInfo.ProductName + ", vers&atilde;o " + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileVersionInfo.ProductVersion) + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                string hostname = System.Net.Dns.GetHostName();
                string enderecos = string.Empty;
                foreach (System.Net.IPAddress ip in System.Net.Dns.GetHostAddresses(hostname))
                {
                    enderecos += ", " + ip.ToString();
                }
                if (enderecos != string.Empty)
                {
                    enderecos = enderecos.Substring(2);
                }
                extrainfo += "<b>Nome do servidor: </b>" + hostname + " (" + enderecos + ")" + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                extrainfo += "<b>Sistema Operacional: </b>" + Environment.OSVersion.VersionString + "<br />";
                extrainfo += "<b>Processadores: </b>" + System.Environment.ProcessorCount.ToString() + "<br />";
                /*
                extrainfo += "<b>Memória física: </b>" + (My.Computer.Info.TotalPhysicalMemory / 1024 / 1024).ToString("N2") + " mb (" + (My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024).ToString("N2") + " mb livres)" + "<br />";
                extrainfo += "<b>Memória virtual: </b>" + (My.Computer.Info.TotalVirtualMemory / 1024 / 1024).ToString("N2") + " mb (" + (My.Computer.Info.AvailableVirtualMemory / 1024 / 1024).ToString("N2") + " mb livres)" + "<br />";
                 */
                try
                {
                    extrainfo += "<b>Unidades de Disco: </b><br />";
                    foreach (System.IO.DriveInfo d in System.IO.DriveInfo.GetDrives())
                    {
                        if (d.DriveType == DriveType.Fixed)
                        {

                            string driveInfo = "";

                            driveInfo += " &gt; " + d.Name;
                            try
                            {
                                double dtotal = d.TotalSize / 1024 / 1024;
                                double dlivre = d.TotalFreeSpace / 1024 / 1024;

                                driveInfo += " (Total: " + dtotal.ToString("N2") + "mb";
                                if (dlivre < (5 * 1024))
                                {
                                    driveInfo += ", <span style=\"color:red\">Livre: " + dlivre.ToString("N2") + " mb</span>";
                                }
                                else
                                {
                                    driveInfo += ", Livre: " + dlivre.ToString("N2") + " mb";
                                }
                                driveInfo += ")";
                            }
                            catch
                            {

                            }
                            extrainfo += driveInfo + "<br/>";
                        }
                    }
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                extrainfo += "ComputerInfo:<br />" + ex.Message + "<br />";
            }

            return extrainfo;

        }

        private static string GetLoadedAssembliesHtml()
        {
            string extrainfo = string.Empty;

            try
            {
                extrainfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Assemblies Carregadas</h2><br />";
                System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Array.Sort<System.Reflection.Assembly>(assemblies, new SortAssembly());
                foreach (System.Reflection.Assembly Assembly in assemblies)
                {
                    try
                    {
                        extrainfo += "<p><b>" + Assembly.GetName().Name + "</b>" + " (" + new System.IO.FileInfo(Assembly.Location).LastWriteTime.ToString() + ")" + "<br />" + Assembly.FullName + "</p>";
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            return extrainfo;

        }


        private class SortAssembly : IComparer<System.Reflection.Assembly>
        {
            public int Compare(System.Reflection.Assembly x, System.Reflection.Assembly y)
            {
                return string.Compare(x.GetName().Name, y.GetName().Name);
            }
        }
        internal static void DebugSQL(string header, string sql, TenorParameter[] parameters, ConnectionStringSettings connection)
        {
            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {

                    DbProviderFactory factory = Helper.GetFactory(connection);
                    StringBuilder traceInfo = new StringBuilder();
                    traceInfo.AppendLine();
                    traceInfo.AppendLine(header);
                    traceInfo.AppendLine(" > " + connection.Name + " (" + connection.ProviderName + ")");

                    if (parameters != null)
                        foreach (TenorParameter p in parameters)
                        {
                            //TODO: Consider recieving the Dialect from a parameter.
                            traceInfo.AppendLine("DECLARE " + p.ParameterName + " " + Helper.GetDbTypeName(p.Value.GetType(), factory).ToLower());
                            if (p.Value == null)
                            {
                                traceInfo.AppendLine("SET " + p.ParameterName + " = NULL");
                            }
                            else
                            {
                                string value;
                                if (p.Value is bool)
                                {
                                    value = Math.Abs(System.Convert.ToInt32(p.Value)).ToString();
                                }
                                else if (p.Value is string)
                                {
                                    value = "\'" + ((string)p.Value).Replace("\'", "\'\'") + "\'";
                                }
                                else if (p.Value is DateTime)
                                {
                                    value = "\'" + ((DateTime)p.Value).ToString("yyyy-MM-dd hh:mm:ss") + "\'";
                                }
                                else
                                {
                                    value = p.Value.ToString();
                                }

                                traceInfo.AppendLine("SET " + p.ParameterName + " = " + value);
                            }
                        }
                    traceInfo.AppendLine(sql.ToString());


                    string st = Environment.StackTrace;
                    //string fimDaondeNaoImporta = "Tenor.Disgnostics.Debug.DebugSQL(String sql, TenorParameter[] parameters)" + Environment.NewLine;
                    //int i = st.IndexOf(fimDaondeNaoImporta);
                    //if (i > 0)
                    //{
                    //    st = st.Substring(i + fimDaondeNaoImporta.Length);
                    //}

                    traceInfo.AppendLine("> Stack Trace:");
                    traceInfo.AppendLine(st);
                    traceInfo.AppendLine("---------------------");

                    System.Diagnostics.Trace.TraceInformation(traceInfo.ToString());
                }
            }
            catch
            {
            }
        }
    }
}