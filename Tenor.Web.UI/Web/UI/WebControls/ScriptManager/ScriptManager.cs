using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Permissions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Reflection;
using System.ComponentModel;


namespace Tenor.Web.UI.WebControls
{

    /// <summary>
    /// Contains event arguments used on <see cref="ScriptManager.Confirmation" /> event.
    /// </summary>
    public class ConfirmationEventArgs : EventArgs
    {
        public ConfirmationEventArgs(string commandName, bool response)
        {
            this._CommandName = commandName;
            this._Response = response;
        }

        private bool _Response;
        /// <summary>
        /// Gets the user response on the confirmation event.
        /// </summary>
        public bool Response
        {
            get
            {
                return _Response;
            }
        }

        private string _CommandName;
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string CommandName
        {
            get
            {
                return _CommandName;
            }
        }


    }

    public delegate void ConfirmationEventHandler(object sender, ConfirmationEventArgs e);

    /// <summary>
    /// This control can manipulate declared script features.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), PersistChildren(false), ParseChildren(ChildrenAsProperties = true, DefaultProperty = "Scripts"), ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:ScriptManager runat=\"server\" />"), Designer(typeof(Design.ScriptManagerDesigner)), ToolboxBitmapAttribute(typeof(ScriptManager), "ScriptManager.bmp")]
    public class ScriptManager : Control, IPostBackEventHandler
    {
        private ConfirmationEventHandler ConfirmationEvent;
        public event ConfirmationEventHandler Confirmation
        {
            add
            {
                ConfirmationEvent = (ConfirmationEventHandler)System.Delegate.Combine(ConfirmationEvent, value);
            }
            remove
            {
                ConfirmationEvent = (ConfirmationEventHandler)System.Delegate.Remove(ConfirmationEvent, value);
            }
        }

        protected void OnConfirmation(ConfirmationEventArgs e)
        {
            if (ConfirmationEvent != null)
                ConfirmationEvent(this, e);
        }

        private ScriptCollection _scripts;
        /*
        '<DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
        'NotifyParentProperty(True), _
        <Editor(GetType(Design.ScriptCollectionEditor), GetType(Drawing.Design.UITypeEditor))> _
        <EditorBrowsable(EditorBrowsableState.Always)> _
        <PersistenceMode(PersistenceMode.InnerDefaultProperty)> _
        Public ReadOnly Property Scripts() As ScriptCollection
            Get
                Return _scripts
            End Get
        End Property
        */

        /// <summary>
        /// Gets a collection of defined scripts.
        /// </summary>
        [NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor(typeof(Design.ScriptCollectionEditor), typeof(System.Drawing.Design.UITypeEditor)), PersistenceMode(PersistenceMode.InnerProperty), Description("Provides a collection of scripts to this page"), Browsable(false), EditorBrowsable(EditorBrowsableState.Always), Category("Misc")]
        public ScriptCollection Scripts
        {
            get
            {
                if (_scripts == null)
                {
                    _scripts = new ScriptCollection();
                }

                return _scripts;
            }
        }

        protected override void OnInit(System.EventArgs e)
        {

            /*
            AddHandler Page.Init, AddressOf Page_Init
             */
            base.OnInit(e);
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            //The previous code messed up the repeater, so we moved back to prerender. 
            //i guess that this call must be on INIT.
            base.OnPreRender(e);
            Page_Init(null, e);
        }


        private void Page_Init(object sender, EventArgs e)
        {
            //finds a list of scripts and adds each one once to the current page.

            List<Type> listaScripts = new List<Type>();
            foreach (Script i in Scripts)
            {
                if (!listaScripts.Contains(i.GetType()))
                {
                    listaScripts.Add(i.GetType());
                    i.Initialize(this.Page);
                }
            }
        }

        private string GetScriptTags(string script)
        {
            if (!script.StartsWith("\r\n"))
            {
                script = "\r\n" + script;
            }
            if (!script.EndsWith("\r\n"))
            {
                script += "\r\n";
            }

            return "<script type=\"text/javascript\">" + script + "</script>";
        }

        /// <summary>
        /// Searches for an especific control.
        /// </summary>
        /// <param name="ControlType">FullName of the desired type.</param>
        private static Control SearchControl(string ControlType, ControlCollection Controls)
        {
            foreach (Control i in Controls)
            {
                if (i.GetType().FullName.Equals(ControlType))
                {
                    return i;
                }
                else
                {
                    Control obj = SearchControl(ControlType, i.Controls);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Prepares and sends a script via MagicAjax.
        /// </summary>
        /// <returns>True if this is a MagicAjax context.</returns>
        private static bool SendMagicAjaxScript(Page Page, string Script)
        {
            Control AjaxPanel = SearchControl("MagicAjax.UI.Controls.AjaxPanel", Page.Controls);
            if (AjaxPanel != null)
            {
                Type contexttype = AjaxPanel.GetType().Assembly.GetType("MagicAjax.MagicAjaxContext");
                PropertyInfo current = contexttype.GetProperty("Current");
                object context = current.GetValue(null, null);
                if (System.Convert.ToBoolean(contexttype.GetProperty("IsAjaxCall").GetValue(context, null)))
                {
                    Type helper = AjaxPanel.GetType().Assembly.GetType("MagicAjax.AjaxCallHelper");


                    try
                    {
                        MethodInfo write = helper.GetMethod("WriteOnEnd");
                        if (write == null)
                        {
                            write = helper.GetMethod("Write");
                        }
                        write.Invoke(null, new object[] { (Script) });
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Prepares and sends a script via MS AjaxNet (Atlas).
        /// </summary>
        /// <returns>True if this is an AjaxNet context.</returns>
        private static bool SendAjaxNetScript(Page Page, string Key, string Script)
        {
            Control ScriptManager = SearchControl("System.Web.UI.ScriptManager", Page.Controls);
            if (ScriptManager == null)
            {
                ScriptManager = SearchControl("Microsoft.Web.UI.ScriptManager", Page.Controls);
            }
            if (ScriptManager != null)
            {
                PropertyInfo isasync = ScriptManager.GetType().GetProperty("IsInAsyncPostBack");
                if (System.Convert.ToBoolean(isasync.GetValue(ScriptManager, null)))
                {
                    MethodInfo reg = ScriptManager.GetType().GetMethod("RegisterStartupScript", new Type[] { typeof(Page), typeof(Type), typeof(string), typeof(string), typeof(bool) });
                    object updPanel;

                    if (reg != null)
                    {
                        updPanel = Page;
                    }
                    else
                    {
                        reg = ScriptManager.GetType().GetMethod("RegisterStartupScript", new Type[] { typeof(Control), typeof(Type), typeof(string), typeof(string), typeof(bool) });
                        if (reg == null)
                        {
                            System.Diagnostics.Trace.TraceError("Unsuported Microsoft AjaxNet version.");
                            return false;
                        }
                        updPanel = SearchControl("Microsoft.Web.UI.UpdatePanel", Page.Controls);
                        if (updPanel == null)
                        {
                            updPanel = Page;
                        }

                    }
                    reg.Invoke(null, new object[] { updPanel, typeof(ScriptManager), Key, Script, true });
                }
                else
                {

                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a ramdom key to register scripts.
        /// </summary>
        private static string GetRandomKey()
        {
            string alertkey = "ScriptManager." + Guid.NewGuid().ToString();
            return alertkey;
        }

        /// <summary>
        /// Performs javascript encoding on the desired text.
        /// Escapes the following characters: ", \, tabs, carriage and line feed.
        /// </summary>
        private string EncodeMessage(string text)
        {

            text = text.Replace("\\", "\\\\");
            text = text.Replace("\"", "\\\"");
            //Message = Message.Replace("'", "\'")
            text = text.Replace("\t", "\\t");
            text = text.Replace("\r\n", "\\n");
            text = text.Replace("\r", "\\n");
            text = text.Replace("\n", "\\n");

            return text;
        }


        /// <summary>
        /// Pops up the browser alert window, and redirects to another address after user return.
        /// </summary>
        /// <param name="message">A string with the message shown on the alert window.</param>
        /// <param name="url">The url that will be redirected to.</param>
        public void AlertAndRedirect(string message, string url)
        {
            Alert(message, true);
            Redirect(url);
        }

        /// <summary>
        /// Pops up the browser alert window, and redirects to another address after user return.
        /// </summary>
        /// <param name="message">A string with the message shown on the alert window.</param>
        /// <param name="url">The url that will be redirected to.</param>
        /// <param name="endResponse">If true, the user code will be aborted during this call, and Page.Render will not be processed.</param>
        public void AlertAndRedirect(string message, string url, bool endResponse)
        {
            if (endResponse)
            {

                message = EncodeMessage(message);

                string script = this.GetScriptTags("alert(\"{0}\"); " + "\r\n" + "try {{ location.replace(\"{1}\"); }} catch(e) {{ }}" + "\r\n") + "\r\n" + "<a href=\"{2}\">Redirecionar</a>";


                script = string.Format(script, message, this.Page.ResolveClientUrl(url).Replace("\"", "\"\""), url);
                HttpResponse response = HttpContext.Current.Response;

                response.Write("<html><head><title>Redirecting</title></head><body>" + "\r\n" + script + "\r\n" + "</body></html>");
                response.End();
            }
            else
            {
                AlertAndRedirect(message, url);
            }
        }

        /// <summary>
        /// Redirects user to the address using client-side code.
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        public void Redirect(string url)
        {
            string key = GetRandomKey();

            string script = "location.replace(\'" + this.Page.ResolveClientUrl(url).Replace("\'", "\'\'") + "\');";
            if (!SendMagicAjaxScript(Page, script) && !SendAjaxNetScript(Page, key, script))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), key, script, true);
            }
        }

        /// <summary>
        /// Pops up the browser alert window.
        /// </summary>
        /// <param name="message">A string with the message shown on the alert window.</param>
        /// <remarks>The alert window will be shown after page rendering.</remarks>
        public void Alert(string message)
        {
            Alert(message, true);
        }

        /// <summary>
        /// Pops up the browser alert window.
        /// </summary>
        /// <param name="message">A string with the message shown on the alert window.</param>
        /// <param name="afterPage">If true, shows the alert window after page rendering. Otherwise, shows it before the page rendering.</param>
        /// <remarks>The afterPage parameter does not have any effect on ajax contexts.</remarks>
        public void Alert(string message, bool afterPage)
        {
            string alertkey = GetRandomKey();
            message = EncodeMessage(message);

            string script = "alert(\"" + message + "\");";
            if (!SendMagicAjaxScript(Page, script) && !SendAjaxNetScript(Page, alertkey, script))
            {
                if (afterPage)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), alertkey, script, true);
                }
                else
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), alertkey, script, true);
                }
            }

        }

        /// <summary>
        /// Opens a new browser window. 
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        /// <remarks>Note that actions of this method may be blocked by user popup blocker.</remarks>
        public void OpenNewWindow(string url)
        {
            OpenNewWindow(url, Unit.Empty, Unit.Empty, false, "_blank");
        }

        /// <summary>
        /// Opens a new browser window. 
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        /// <param name="width">An integer with the window's width in pixels.</param>
        /// <param name="height">An integer with the window's height in pixels.</param>
        /// <remarks>Note that actions of this method may be blocked by user popup blocker.</remarks>
        public void OpenNewWindow(string url, Unit width, Unit height)
        {
            OpenNewWindow(url, width, height, true, "_blank");
        }

        /// <summary>
        /// Opens a new browser window. 
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        /// <param name="width">An integer with the window's width in pixels.</param>
        /// <param name="height">An integer with the window's height in pixels.</param>
        /// <param name="isDialog">Defines if the new window will be a dialog.</param>
        /// <remarks>Note that actions of this method may be blocked by user popup blocker.</remarks>
        public void OpenNewWindow(string url, Unit width, Unit height, bool isDialog)
        {
            OpenNewWindow(url, width, height, isDialog, "_blank");
        }

        /// <summary>
        /// Opens a new browser window. 
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        /// <param name="target">A key to identify the new window.</param>
        /// <remarks>Note that actions of this method may be blocked by user popup blocker.</remarks>
        public void OpenNewWindow(string url, string target)
        {
            OpenNewWindow(url, Unit.Empty, Unit.Empty, false, target);
        }

        /// <summary>
        /// Opens a new browser window. 
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        /// <param name="width">An integer with the window's width in pixels.</param>
        /// <param name="height">An integer with the window's height in pixels.</param>
        /// <param name="isDialog">Defines if the new window will be a dialog.</param>
        /// <param name="target">A key to identify the new window.</param>
        /// <remarks>Note that actions of this method may be blocked by user popup blocker.</remarks>
        public void OpenNewWindow(string url, Unit width, Unit height, bool isDialog, string target)
        {
            string alertkey = GetRandomKey();
            string script = GetNewWindowScript(url, width, height, isDialog, target);
            if (!SendMagicAjaxScript(Page, script) && !SendAjaxNetScript(Page, alertkey, script))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), alertkey, script, true);
            }

        }

        /// <summary>
        /// Gets a javascript that can open a new browser window.
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        /// <param name="width">An integer with the window's width in pixels.</param>
        /// <param name="height">An integer with the window's height in pixels.</param>
        /// <param name="isDialog">Defines if the new window will be a dialog.</param>
        /// <remarks>Note that this script may be blocked by user popup blocker.</remarks>
        public string GetNewWindowScript(string url, Unit width, Unit height, bool isDialog)
        {
            return GetNewWindowScript(url, width, height, isDialog, "_blank");
        }


        /// <summary>
        /// Gets a javascript that can open a new browser window.
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        /// <param name="width">An integer with the window's width in pixels.</param>
        /// <param name="height">An integer with the window's height in pixels.</param>
        /// <param name="isDialog">Defines if the new window will be a dialog.</param>
        /// <param name="target">A key to identify the new window.</param>
        /// <remarks>Note that this script may be blocked by user popup blocker.</remarks>
        public string GetNewWindowScript(string url, Unit width, Unit height, bool isDialog, string target)
        {
            return GetNewWindowScript(url, Unit.Empty, Unit.Empty, width, height, isDialog, target);
        }


        /// <summary>
        /// Gets a javascript that can open a new browser window.
        /// </summary>
        /// <param name="url">A string with the destination url.</param>
        /// <param name="left">An integer with the window's left position in pixels.</param>
        /// <param name="top">An integer with the window's top position in pixels.</param>
        /// <param name="width">An integer with the window's width in pixels.</param>
        /// <param name="height">An integer with the window's height in pixels.</param>
        /// <param name="isDialog">Defines if the new window will be a dialog.</param>
        /// <param name="target">A key to identify the new window.</param>
        /// <remarks>Note that this script may be blocked by user popup blocker.</remarks>
        public string GetNewWindowScript(string url, Unit left, Unit top, Unit width, Unit height, bool isDialog, string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                target = "_blank";
            }
            if (left.IsEmpty)
            {
                left = Unit.Parse("90px");
            }
            if (top.IsEmpty)
            {
                top = Unit.Parse("90px");
            }

            string features = "";
            if (isDialog)
            {
                features += "resizable=0, status=0, toolbar=0, menubar=0, location=0, scrollbars=0";
            }
            else
            {
                features += "resizable=1, status=1, toolbar=0, menubar=0, location=0, scrollbars=1";
            }
            if (!width.IsEmpty)
            {
                if (!string.IsNullOrEmpty(features))
                {
                    features += ", ";
                }
                features += "width=" + width.Value.ToString();
            }
            if (!height.IsEmpty)
            {
                if (!string.IsNullOrEmpty(features))
                {
                    features += ", ";
                }
                features += "height=" + height.Value.ToString();
            }

            if (!string.IsNullOrEmpty(features))
            {
                features += ", ";
            }
            features += "left=" + left.Value.ToString();

            if (!string.IsNullOrEmpty(features))
            {
                features += ", ";
            }
            features += "top=" + top.Value.ToString();


            if (!string.IsNullOrEmpty(features))
            {
                features = ", \"" + features + "\"";
            }

            string script = "window.open(\"" + this.Page.ResolveClientUrl(url) + "\", \"" + target + "\"" + features + ");";
            return script;
        }


        /// <summary>
        /// Closes the current browser window.
        /// </summary>
        /// <remarks>Note that actions of this method may be blocked, specially if this window was not opened by another script code.</remarks>
        public void CloseWindow()
        {
            string alertkey = GetRandomKey();
            string script = GetCloseWindowScript();
            if (!SendMagicAjaxScript(Page, script) && !SendAjaxNetScript(Page, alertkey, script))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), alertkey, script, true);
            }
        }

        /// <summary>
        /// Gets a javascript that closes the current browser window.
        /// </summary>
        /// <remarks>
        /// Note that this script may be blocked, specially if this window was not opened by another script code.
        /// </remarks>
        public string GetCloseWindowScript()
        {
            return "window.close();";
        }

        /// <summary>
        /// Shows a browser confirm window, and returns user's response to the application using the <see cref="Confirmation"/> event.
        /// </summary>
        /// <param name="commandName">A string to identify the current action.</param>
        /// <param name="message">A string with the message shown on the confirmation window.</param>
        /// <remarks>To recieve user's response, check <see cref="Confirmation"/> event.</remarks>
        public void Confirm(string commandName, string message)
        {
            if (string.IsNullOrEmpty(commandName))
                throw new ArgumentNullException("commandName");

            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            commandName = EncodeMessage(commandName).Replace(":", ";");

            string alertkey = GetRandomKey();
            message = EncodeMessage(message);

            string Script = "var ScriptManager_Confirm = confirm(\"" + message + "\");" + "\r\n";

            //PostBackOptions opt = new PostBackOptions(this, "ScriptManager_Confirm.toString()", null, false, false, true, true, false, null);
            Script += " if (ScriptManager_Confirm) " + "\r\n";
            Script += Page.ClientScript.GetPostBackEventReference(this, "confirm:" + commandName + ":true") + ";" + "\r\n";
            Script += " else " + "\r\n";
            Script += Page.ClientScript.GetPostBackEventReference(this, "confirm:" + commandName + ":false") + ";" + "\r\n";
            Script += "" + "\r\n";

            if (!SendMagicAjaxScript(Page, Script) && !SendAjaxNetScript(Page, alertkey, Script))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), alertkey, Script, true);
            }
        }


        /// <summary>
        /// Processes event calls.
        /// </summary>
        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("confirm:"))
            {
                string[] valores = (eventArgument + ":").Split(':');
                ConfirmationEventArgs e = new ConfirmationEventArgs(valores[1], (valores[2].ToLower() == "true"));
                OnConfirmation(e);
            }
        }


        /// <summary>
        /// Gets the current ScriptManager instance. 
        /// </summary>
        /// <remarks>You can have only one ScriptManager declared.</remarks>
        public static ScriptManager Current
        {
            get
            {
                HttpContext Context = HttpContext.Current;
                if (Context == null)
                {
                    return null;
                }
                Page Page = Context.CurrentHandler as Page;
                if (Page == null)
                {
                    return null;
                }



                MasterPage master = Page.Master;
                if (master == null)
                {
                    return GetScriptManager(Page.Controls);
                }
                else
                {
                    do
                    {
                        if (master.Master == null)
                        {
                            break;
                        }
                        else
                        {
                            master = master.Master;
                        }
                    } while (true);
                    return GetScriptManager(master.Controls);
                }

            }
        }


        private static ScriptManager GetScriptManager(ControlCollection Controls)
        {
            if (Controls == null)
            {
                return null;
            }

            foreach (Control i in Controls)
            {
                if (i is ScriptManager)
                {
                    return ((ScriptManager)i);
                }
                else
                {
                    ScriptManager script = GetScriptManager(i.Controls);
                    if (script != null)
                    {
                        return script;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Registers the script by ajax context when available. 
        /// </summary>
        public void RegisterStartupScript(string script)
        {
            this.RegisterStartupScript(string.Empty, script);
        }


        /// <summary>
        /// Registers the script by ajax context when available.
        /// </summary>
        public void RegisterStartupScript(string key, string script)
        {
            RegisterStartupScript(Page, key, script);
        }
        /// <summary>
        /// Registers the script by ajax context when available.
        /// </summary>
        public static void RegisterStartupScript(Page Page, string script)
        {
            RegisterStartupScript(Page, null, script);
        }

        /// <summary>
        /// Registers the script by ajax context when available.
        /// </summary>
        public static void RegisterStartupScript(Page Page, string key, string script)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = GetRandomKey();
            }

            if (!SendMagicAjaxScript(Page, script) && !SendAjaxNetScript(Page, key, script))
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), key, script, true);
            }
        }
    }

}