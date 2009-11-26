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
using Tenor.Configuration;


namespace Tenor.Diagnostics
{
    /// <summary>
    /// Represents a set of common methods with debugging code.
    /// </summary>
    public static class Debug
    {

        /// <summary>
        /// Prints on page output execution timings. This is useful on performance calculations.
        /// Time values will be printed on the output between html comments. These time values will also be printed on debug output if debug mode is activated.
        /// </summary>
        /// <param name="page">A Page.</param>
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
                p.Response.Write(string.Format("<!-- {0} -->", res));
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
                p.Response.Write(string.Format("<!-- {0} -->", res));
            }
        }


        /// <summary>
        /// Prints on page output the current system time. 
        /// Time values and descritive text will be printed between html comments, if we are on a web context, otherwise, it will be printed on debug output.
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
        /// Tries to send an exception to defined emails.
        /// </summary>
        /// <param name="exception">A System.Exception.</param>
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
                Tenor.Configuration.Tenor config = Tenor.Configuration.Tenor.Current;

                if (exception == null)
                    throw new ArgumentNullException("exception");

                if (config.Exceptions.LogMode == LogMode.None
                    || (config.Exceptions.LogMode == LogMode.Email && config.Exceptions.Emails.Count == 0)
                    || (config.Exceptions.LogMode == LogMode.File && 
                        (string.IsNullOrEmpty(config.Exceptions.FilePath) || !Directory.Exists(config.Exceptions.FilePath))))
                    return;

                string requestUrl = string.Empty;
                if (app != null)
                    requestUrl = app.Request.Url.GetLeftPart(UriPartial.Authority) + app.Request.RawUrl;

                string title = (HandledByUser ? "Handled" : "Unhandled").ToString() + " Exception: " + requestUrl + " (" + DateTime.Now.ToString() + ")";

                string body = string.Empty;

                try
                {
                    HttpException htmlex = exception as HttpException;
                    if (htmlex != null)
                    {
                        body = htmlex.GetHtmlErrorMessage();
                    }
                }
                catch { }

                if (body.Trim().Length < 10)
                    body = BuildExceptionDetails(app, exception);

                body = BuildExtraInfo(app, exception, body);

                if (config.Exceptions.LogMode == LogMode.Email)
                {
                    Mail.MailMessage errmail = new Mail.MailMessage();

                    foreach (EmailElement email in config.Exceptions.Emails)
                    {
                        errmail.To.Add(new System.Net.Mail.MailAddress(email.Email, email.Name));
                    }

                    if (errmail.To.Count == 0)
                    {
                        errmail.Dispose();
                        return;
                    }

                    errmail.Subject = title;
                    errmail.IsBodyHtml = true;
                    errmail.BodyEncoding = System.Text.Encoding.UTF8;
                    errmail.Body = body;

                    errmail.Send();
                }
                else if(config.Exceptions.LogMode == LogMode.File)
                {
                    string path = config.Exceptions.FilePath;
                    if (!path.EndsWith("\\"))
                        path += "\\";
                    File.WriteAllText(string.Format("{1}\\{0:yyyy-MM-dd-HH-mm-ss}.html", DateTime.Now, path), body, System.Text.Encoding.UTF8);
                }
            }
            catch { }
        }

        private static string BuildExceptionDetails(HttpApplication app, Exception exception)
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

            //TODO: Move this email template to an external xml file on an assembly resource.

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
            string endOfDoesNotMatter = "HandleError(Exception exception)" + Environment.NewLine;
            int i = st.IndexOf(endOfDoesNotMatter);
            if (i > 0)
            {
                st = st.Substring(i + endOfDoesNotMatter.Length);
            }
            else
            {
                endOfDoesNotMatter = "get_StackTrace()" + Environment.NewLine;
                i = st.IndexOf(endOfDoesNotMatter);
                if (i > 0)
                {
                    st = st.Substring(i + endOfDoesNotMatter.Length);
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

            return except.ToString();
        }

        private static string BuildExtraInfo(HttpApplication app, Exception exception, string body)
        {
            string extraInfo = string.Empty;

            Exception ex = exception;
            while (ex != null)
            {
                if (ex.Data.Count > 0)
                {

                    extraInfo += "<br /><br /><b>Additional Information: </b>";
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

            return body.Replace("</body>", extraInfo + "</body>");
        }

        private static string GetExtraInfoHtml(HttpApplication app)
        {
            string extraInfo = "";

            try
            {
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>User Information</h2><br />";
                if ((app.User != null) && (app.User.Identity != null))
                {
                    extraInfo += "<b>Authenticated: </b>" + app.User.Identity.IsAuthenticated.ToString() + "<br />";
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
                extraInfo += "<b>Address: </b>" + app.Request.UserHostName + " (" + app.Request.UserHostAddress + ")" + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                if (app.Request.UrlReferrer != null)
                {
                    extraInfo += "<b>Url Referrer: </b>" + app.Request.UrlReferrer.ToString() + "<br />";
                }
            }
            catch (Exception)
            {
            }
            try
            {
                extraInfo += "<b>Request Type: </b>" + app.Request.RequestType + "<br />";
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
                extraInfo += "<b>Size: </b>" + tamanhoS + "<br />";
            }
            catch (Exception)
            {
            }


            try
            {
                extraInfo += "<br /><hr width=\"100%\" size=\"1\" color=\"silver\" /><h2>Form Values</h2><br />";
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
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Url Values</h2><br />";
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
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Cookie Values</h2><br />";
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
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Server Variables</h2><br />";
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
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Session Values</h2><br />";
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
                extraInfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Application Values</h2><br />";
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

            extrainfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Server Information</h2><br />";
            try
            {
                extrainfo += "<b>Current IIS Version: </b>" + Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\InetStp", false).GetValue("MajorVersion").ToString() + "." + Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\InetStp", false).GetValue("MinorVersion").ToString() + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                string loc = typeof(char).Assembly.Location.Replace("mscorlib.dll", "webengine.dll");
                System.Diagnostics.FileVersionInfo versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(loc);

                string info = string.Format("{0}.{1}.{2}.{3}", new object[] { versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart });

                extrainfo += "<b>ASP.NET Version: </b>" + info + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                string loc = typeof(char).Assembly.Location.Replace("mscorlib.dll", "mscorwks.dll");
                System.Diagnostics.FileVersionInfo versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(loc);

                string info = string.Format("{0}.{1}.{2}.{3}", new object[] { versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart });

                extrainfo += "<b>.NET Framework Runtime Version: </b>" + info + "<br />";
            }
            catch (Exception)
            {
            }

            try
            {
                extrainfo += "<b>Process Information: </b>" + (System.Diagnostics.Process.GetCurrentProcess().MainModule.FileVersionInfo.ProductName + ", version " + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileVersionInfo.ProductVersion) + "<br />";
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
                extrainfo += "<b>Host Name: </b>" + hostname + " (" + enderecos + ")" + "<br />";
            }
            catch (Exception)
            {
            }
            try
            {
                extrainfo += "<b>Operating System: </b>" + Environment.OSVersion.VersionString + "<br />";
                extrainfo += "<b>Processors: </b>" + System.Environment.ProcessorCount.ToString() + "<br />";

                try
                {
                    MemoryStatus stat = new MemoryStatus();
                    GlobalMemoryStatus(out stat);
                    long totalPhysical = (long)stat.TotalPhysical;
                    long availablePhysical = (long)stat.AvailablePhysical;
                    long totalVirtual = (long)stat.TotalVirtual;
                    long availableVirtual = (long)stat.AvailableVirtual;

                    extrainfo += "<b>Physical Memory: </b>" + (totalPhysical / 1024 / 1024).ToString("N2") + " mb (" + (availablePhysical / 1024 / 1024).ToString("N2") + " mb livres)" + "<br />";
                    extrainfo += "<b>Virtual Memory: </b>" + (totalVirtual / 1024 / 1024).ToString("N2") + " mb (" + (availableVirtual / 1024 / 1024).ToString("N2") + " mb livres)" + "<br />";
                }
                catch { }


                try
                {
                    extrainfo += "<b>Disk Usage: </b><br />";
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
                                    driveInfo += ", Free: " + dlivre.ToString("N2") + " mb";
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
                extrainfo += "<br /><hr  width=\"100%\" size=\"1\" color=\"silver\" /><h2>Loaded Assemblies</h2><br />";
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

                    DbProviderFactory factory = Tenor.Data.Dialects.DialectFactory.CreateDialect(connection).Factory;
                    StringBuilder traceInfo = new StringBuilder();
                    traceInfo.AppendLine();
                    traceInfo.AppendLine(header);
                    traceInfo.AppendLine(" > " + connection.Name + " (" + connection.ProviderName + ")");

                    if (parameters != null)
                        foreach (TenorParameter p in parameters)
                        {
                            //TODO: Consider recieving the Dialect from a parameter or create one to generate specific code.
                            Type valueType = p.Value.GetType();
                            if (valueType.IsEnum)
                                valueType = valueType.GetFields()[0].FieldType;
                            traceInfo.AppendLine("DECLARE " + p.ParameterName + " " + Helper.GetDbTypeName(valueType, factory).ToLower());
                            if (p.Value == null)
                            {
                                traceInfo.AppendLine("SET " + p.ParameterName + " = NULL");
                            }
                            else
                            {
                                string value;
                                if (p.Value is bool)
                                {
                                    value = System.Math.Abs(System.Convert.ToInt32(p.Value)).ToString();
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
                    /*
                    string fimDaondeNaoImporta = "Tenor.Diagnostics.Debug.DebugSQL(String sql, TenorParameter[] parameters)" + Environment.NewLine;
                    int i = st.IndexOf(fimDaondeNaoImporta);
                    if (i > 0)
                    {
                        st = st.Substring(i + fimDaondeNaoImporta.Length);
                    }
                     */

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

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern void GlobalMemoryStatus(out MemoryStatus stat);

    }

    public struct MemoryStatus
    {
        public uint Length;
        public uint MemoryLoad;
        public uint TotalPhysical;
        public uint AvailablePhysical;
        public uint TotalPageFile;
        public uint AvailablePageFile;
        public uint TotalVirtual;
        public uint AvailableVirtual;
    }

}