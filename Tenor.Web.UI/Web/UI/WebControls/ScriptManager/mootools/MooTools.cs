using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;


namespace Tenor.Web.UI.WebControls
{

    /// <summary>
    /// Manage MooTools scripts.
    /// </summary>
    public sealed class MooTools : Script
    {

        /// <summary>
        /// Check if MooTools is already registered.
        /// </summary>
        public static bool CheckMooTools(MooModule module)
        {
            return CheckMooTools(MooVersion.Version_1_11, module);
        }

        /// <summary>
        /// Check if MooTools is already registered.
        /// </summary>
        public static bool CheckMooTools(MooVersion version, MooModule module)
        {
            if (HttpContext.Current == null)
            {
                throw (new InvalidOperationException("Invalid HttpContext."));
            }
            Page Page = HttpContext.Current.CurrentHandler as Page;
            if (Page == null)
            {
                throw (new InvalidOperationException("This request must be a Page request."));
            }
            if (Page.Header == null)
            {
                throw (new InvalidOperationException("Header tag must be a server control."));
            }
            foreach (Control c in Page.Header.Controls)
            {
                if (c is Literal)
                {


                    Literal lit = (Literal)c;
                    if (module == MooModule.Core)
                    {
                        //Any module defined contains the core.
                        for (MooModule i = MooModule.Core; i <= MooModule.Full; i++)
                        {
                            string script = GetScript(Page, version, i);
                            if (lit.Text.Contains(script))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        //Only the self module or the full one.
                        string script = GetScript(Page, version, module);
                        string scriptFull = GetScript(Page, version, MooModule.Full);

                        if (lit.Text.Contains(script) || lit.Text.Contains(scriptFull))
                        {
                            return true;
                        }

                    }
                }
            }
            return false;
        }
        //TODO: Implement newer mootools version.

        public enum MooVersion
        {
            /// <summary>
            /// Uses the version 1.11.
            /// </summary>
            Version_1_11,
            /// <summary>
            /// Uses the version 1.2.
            /// </summary>
            Version_1_2
        }

        public enum MooModule
        {
            Core,
            CoreTips,
            CoreSortables,
            CoreSmoothscroll,
            CoreRemote,
            CoreEffectsDrag,
            Full
        }


        private MooVersion _Version;
        /// <summary>
        /// Gets or sets a value with MooTools version.
        /// </summary>
        public MooVersion Version
        {
            get
            {
                return _Version;
            }
            set
            {
                _Version = value;
            }
        }


        private MooModule _Module;
        /// <summary>
        /// Gets or sets desired MooTools modules.
        /// </summary>
        public MooModule @Module
        {
            get
            {
                return _Module;
            }
            set
            {
                _Module = value;
            }
        }



        public MooTools()
        {
        }

        public override void Initialize(Page page)
        {

            if (page.Header == null)
            {
                throw (new InvalidOperationException("Header tag must be a server control."));
            }

            //string modulo = this.Module.ToString();


            Literal script = new Literal();
            script.Text += GetScript(page, this.Version, this.Module);
            page.Header.Controls.AddAt(0, script);

            if (this.Version == MooVersion.Version_1_11)
            {
                StringBuilder scriptText = new StringBuilder();
                Literal script2 = new Literal();
                switch (this.Module)
                {
                    case MooModule.CoreSortables:
                        IncludeSortables(page, scriptText);
                        break;
                    case MooModule.CoreEffectsDrag:
                        if (Slimbox)
                        {
                            IncludeSlimbox(page, scriptText);
                        }
                        if (Squeezebox)
                        {
                            IncludeSqueezebox(page, scriptText);
                        }
                        break;
                    case MooModule.Full:
                        IncludeSortables(page, scriptText);
                        if (Slimbox)
                        {
                            IncludeSlimbox(page, scriptText);
                        }
                        if (Squeezebox)
                        {
                            IncludeSqueezebox(page, scriptText);
                        }
                        break;
                }
                script2.Text = scriptText.ToString();
                page.Header.Controls.AddAt(1, script2);
            }
        }

        private static void IncludeSortables(Page Page, StringBuilder sb)
        {
            sb.AppendLine("<script src=\"" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.JsMooTools1_11_SortableOrder) + "\" type=\"text/javascript\"></script>");
        }
        private static void IncludeSlimbox(Page Page, StringBuilder sb)
        {
            sb.AppendLine("<script src=\"" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.JsMooSlimbox) + "\" type=\"text/javascript\"></script>");
        }
        private static void IncludeSqueezebox(Page Page, StringBuilder sb)
        {
            sb.AppendLine("<script src=\"" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.JsMooSqueezeboxSrc) + "\" type=\"text/javascript\"></script>");
            sb.AppendLine("<link rel=\"Stylesheet\" href=\"" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.MooSqueezeboxCss) + "\" type=\"text/css\" />");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("     .sbox-window-ie6 #sbox-btn-close {");
            sb.AppendLine("         background-image:		url(" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.MooSqueezeboxClosebox) + ");");
            sb.AppendLine("     }");
            sb.AppendLine("     #sbox-btn-close {");
            sb.AppendLine("         background:		url(" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.MooSqueezeboxClosebox) + ")  no-repeat center;");
            sb.AppendLine("     }");
            sb.AppendLine("     .sbox-loading #sbox-content {");
            sb.AppendLine("         background-image:		url(" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.MooSqueezeboxSpinner) + ");");
            sb.AppendLine("     }");
            sb.AppendLine("</style>");
        }


        private bool _Slimbox;
        /// <summary>
        /// Gets or sets whether to load Slimbox.
        /// </summary>
        public bool Slimbox
        {
            get
            {
                if (_Slimbox && @Module != MooModule.CoreEffectsDrag && @Module != MooModule.Full)
                {
                    throw (new InvalidOperationException("Please choose CoreEffectsDrag Or Full to use Slimbox"));
                }
                return _Slimbox;
            }
            set
            {
                _Slimbox = value;
            }
        }


        private bool _Squeezebox;
        /// <summary>
        /// Gets or sets whether to load Squeezebox.
        /// </summary>
        public bool Squeezebox
        {
            get
            {
                if (_Squeezebox && @Module != MooModule.CoreEffectsDrag && @Module != MooModule.Full)
                {
                    throw (new InvalidOperationException("Please choose CoreEffectsDrag Or Full to use Squeezebox"));
                }
                return _Squeezebox;
            }
            set
            {
                _Squeezebox = value;
            }
        }



        /// <summary>
        /// Gets a script tag that references the choosed mootools module. 
        /// </summary>
        public static string GetScript(Page Page, MooModule @Module)
        {
            return GetScript(Page, MooVersion.Version_1_11, @Module);
        }

        /// <summary>
        /// Gets a script tag that references the choosed mootools module. 
        /// </summary>
        public static string GetScript(Page Page, MooVersion Version, MooModule @Module)
        {
            switch (Version)
            {
                case MooVersion.Version_1_11:
                    return Environment.NewLine + "<script src=\"" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.MooRoot111 + @Module.ToString() + ".js") + "\" type=\"text/javascript\"></script>";
                case MooVersion.Version_1_2:
                    string script = "";
                    if (@Module != MooModule.Core)
                    {
                        script += Environment.NewLine + GetScript(Page, Version, MooModule.Core);
                    }
                    script += Environment.NewLine + "<script src=\"" + Page.ClientScript.GetWebResourceUrl(typeof(MooTools), Configuration.Resources.MooRoot12 + @Module.ToString() + ".js") + "\" type=\"text/javascript\"></script>";
                    return script;
                default:
                    return null;
            }
        }


        public static void RegisterSqueezeBoxScript(Control Control, Control Link, int Width, int height)
        {
            StringBuilder script = new StringBuilder();
            script.AppendLine("window.addEvent(\"domready\", function() {");
            script.AppendLine("   sq = SqueezeBox.initialize();");
            script.AppendLine("   var obj = $(\"" + Control.ClientID + "\");");
            script.AppendLine("   if (obj) {");
            script.AppendLine("         sq.assign($(\"" + Link.ClientID + "\"), {size:{x:" + Width.ToString() + ", y:" + height.ToString() + "}, handler:\"adopt\", adopt:obj});");
            script.AppendLine("   } else { ");
            script.AppendLine("         alert(\"RegisterSqueezeBoxScript failed.\"); ");
            script.AppendLine("   } ");
            script.AppendLine("});");
            ScriptManager smanager = ScriptManager.Current;

            if (smanager == null)
            {
                throw (new InvalidOperationException("Cannot find ScriptManager."));
            }
            else
            {
                smanager.RegisterStartupScript(script.ToString());
            }

        }
    }
}