using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tenor.Web.UI.WebControls
{

    //PersistChildren(False), _


    /// <summary>
    /// Janela com funções de movimentação.
    /// </summary>
    /// <remarks></remarks>
    [Designer(typeof(Design.WindowDesigner)), ParseChildren(ChildrenAsProperties = true), ToolboxData("<{0}:Window runat=server></{0}:Window>")]
    public class Window : WebControl, IPostBackDataHandler, IPostBackEventHandler, INamingContainer
    {



        private WindowStyle _CurrentStyle;
        private WindowStyle CurrentStyle
        {
            get
            {
                if (_CurrentStyle == null)
                {
                    _CurrentStyle = new WindowPlasticStyle(this);
                }
                return _CurrentStyle;
            }
        }


        protected override string TagName
        {
            get
            {
                return "table";
            }
        }

        protected override System.Web.UI.HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Table;
            }
        }


        internal List<IAttributeAccessor> regions = new List<IAttributeAccessor>();

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            //Isto  está no load para sempre carregar independente do Visible
            Page.ClientScript.RegisterClientScriptResource(typeof(Page), "WebForms.js");
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsWindow);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), ClientID, "var Window_" + this.ClientID + " = null;", true);
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            CurrentStyle.CreateStyle();
            RegisterHiddenFields();

            string parameters = "";

            parameters += "\"" + this.ClientID + "\"";
            parameters += ", " + Movable.ToString().ToLower();
            parameters += ", " + Resizable.ToString().ToLower();
            parameters += ", " + CurrentStyle.BorderSize.Value.ToString();
            parameters += ", " + CurrentStyle.TitleBarSize.Value.ToString();
            if (Closable && AutoPostBack)
            {
                parameters += ", \"" + Page.ClientScript.GetPostBackEventReference(this, null) + "\"";
            }
            else
            {
                parameters += ", null";
            }


            string script = "\r\n" + "Window_" + this.ClientID + " = new WindowControl(" + parameters + ");" + "\r\n";

            ScriptManager.RegisterStartupScript(Page, script);

            base.OnPreRender(e);

            Style.Remove("display");
        }

        #region " Templates "


        private ITemplate _TitleTemplate;
        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(WindowItem)), TemplateInstance(TemplateInstance.Single)]
        public ITemplate TitleTemplate
        {
            get
            {
                return _TitleTemplate;
            }
            set
            {
                _TitleTemplate = value;
            }
        }

        private ITemplate _Content;
        [MergableProperty(false), TemplateContainer(typeof(Window)), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateInstance(TemplateInstance.Single)]
        public ITemplate ContentTemplate
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
            }
        }


        private Panel _Container;
        [Browsable(false)]
        public Control ContentTemplateContainer
        {
            get
            {
                return ((Control)_Container);
            }
        }

        #endregion

        #region " Styles "


        private TitleStyle _TitleStyle;
        [PersistenceMode(PersistenceMode.InnerProperty), Category("Style")]
        public TitleStyle TitleStyle
        {
            get
            {
                if (_TitleStyle == null)
                {
                    _TitleStyle = new TitleStyle();
                }
                return _TitleStyle;
            }
        }

        private Style _ContentStyle;
        [PersistenceMode(PersistenceMode.InnerProperty), Category("Style")]
        public Style ContentStyle
        {
            get
            {
                if (_ContentStyle == null)
                {
                    _ContentStyle = new Style();
                }
                return _ContentStyle;
            }
        }



        #endregion

        #region " Properties "
        [Description("Determines if this control will postback to server."), Themeable(false), DefaultValue(false), Category("Behavior")]
        public virtual bool AutoPostBack
        {
            get
            {
                object obj2 = this.ViewState["AutoPostBack"];
                if (obj2 != null)
                {
                    return System.Convert.ToBoolean(obj2);
                }
                return false;
            }
            set
            {
                this.ViewState["AutoPostBack"] = value;
            }
        }




        [DefaultValue(false), Category("Behavior"), Description("Determines if the window is positioned on center.")]
        public bool StartOnCenter
        {
            get
            {
                if (ViewState["StartOnCenter"] == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (value)
                {
                    ViewState["StartOnCenter"] = true;
                }
                else
                {
                    ViewState["StartOnCenter"] = null;
                }
            }
        }





        [Category("Behavior"), Description("Determines if the window is closed.")]
        public bool IsClosed
        {
            get
            {
                if (ViewState["IsClosed"] == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (value)
                {
                    ViewState["IsClosed"] = true;
                }
                else
                {
                    ViewState["IsClosed"] = null;
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Themeable(true), Description("Determines if the window can be closed.")]
        public bool Closable
        {
            get
            {
                if (ViewState["Closable"] == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    ViewState["Closable"] = null;
                }
                else
                {
                    ViewState["Closable"] = false;
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Themeable(true), Description("Determines if the window can be moved.")]
        public bool Movable
        {
            get
            {
                if (ViewState["Movable"] == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    ViewState["Movable"] = null;
                }
                else
                {
                    ViewState["Movable"] = false;
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Themeable(true), Description("Determines if the window can be resized.")]
        public bool Resizable
        {
            get
            {
                if (ViewState["Resizable"] == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    ViewState["Resizable"] = null;
                }
                else
                {
                    ViewState["Resizable"] = false;
                }
            }
        }

        [Bindable(true), Themeable(true), Category("Appearance"), Description("The title bar text"), DefaultValue(""), Localizable(true)]
        public string Text
        {
            get
            {
                string s = ViewState["Text"].ToString();
                if (s == null)
                {
                    return string.Empty;
                }
                else
                {
                    return s;
                }
            }

            set
            {
                ViewState["Text"] = value;
            }
        }
        #endregion

        #region " Rendering "
        private void CreateSubControls(System.Web.UI.HtmlControls.HtmlTableRow Parent)
        {
            System.Web.UI.HtmlControls.HtmlTableCell left = new System.Web.UI.HtmlControls.HtmlTableCell();
            left.Attributes["class"] = "left";
            System.Web.UI.HtmlControls.HtmlTableCell right = new System.Web.UI.HtmlControls.HtmlTableCell();
            right.Attributes["class"] = "right";
            System.Web.UI.HtmlControls.HtmlTableCell center = new System.Web.UI.HtmlControls.HtmlTableCell();
            center.Attributes["class"] = "center";

            Parent.Controls.Add(left);
            Parent.Controls.Add(center);
            Parent.Controls.Add(right);
        }



        private void CreateTitle(System.Web.UI.HtmlControls.HtmlTableCell Parent)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl titleText = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            titleText.Attributes["class"] = "titleText";
            System.Web.UI.HtmlControls.HtmlGenericControl close = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            close.Attributes["class"] = "close";

            Parent.Controls.Add(titleText);
            if (TitleTemplate == null)
            {
                if (Text == "")
                {
                    titleText.InnerHtml = "untitled";
                }
                else
                {
                    titleText.InnerHtml = Text;
                }
            }
            else
            {
                TitleTemplate.InstantiateIn(titleText);
            }


            if (Closable)
            {
                Parent.Controls.Add(close);
            }

        }

        private void CreateContents(System.Web.UI.HtmlControls.HtmlTableCell Parent)
        {
            if (ContentTemplate != null)
            {
                if (ContentTemplateContainer == null)
                {
                    _Container = new Panel();
                    ContentTemplate.InstantiateIn(ContentTemplateContainer);
                }
            }
            _Container.CssClass = "content";
            Parent.Controls.Clear();
            Parent.Controls.Add(ContentTemplateContainer);

            regions.Add((IAttributeAccessor)ContentTemplateContainer);
        }


        protected override void CreateChildControls()
        {
            Controls.Clear();

            System.Web.UI.HtmlControls.HtmlTableRow title = new System.Web.UI.HtmlControls.HtmlTableRow();
            title.Attributes["class"] = "title";
            System.Web.UI.HtmlControls.HtmlTableRow content = new System.Web.UI.HtmlControls.HtmlTableRow();
            content.Attributes["class"] = "content";
            System.Web.UI.HtmlControls.HtmlTableRow footer = new System.Web.UI.HtmlControls.HtmlTableRow();
            footer.Attributes["class"] = "footer";


            CreateSubControls(title);
            CreateSubControls(content);
            CreateSubControls(footer);

            CreateTitle((System.Web.UI.HtmlControls.HtmlTableCell)(title.Controls[1]));
            CreateContents((System.Web.UI.HtmlControls.HtmlTableCell)(content.Controls[1]));

            Controls.Add(title);
            Controls.Add(content);
            Controls.Add(footer);

        }

        protected override void OnInit(System.EventArgs e)
        {
            CreateChildControls();
            base.OnInit(e);

        }


        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {

            base.AddAttributesToRender(writer);
            writer.AddStyleAttribute("position", "absolute");
            if (Width.IsEmpty)
            {
                writer.AddStyleAttribute("width", "100px");
            }
            if (Height.IsEmpty)
            {
                writer.AddStyleAttribute("height", "100px");
            }
            if (IsClosed)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            }

        }
        #endregion

        #region " Post Back "
        private string lastValue = string.Empty;
        private string lastScrollValue = string.Empty;

        private void RegisterHiddenFields()
        {
            if (StartOnCenter && lastValue == string.Empty)
            {
                lastValue = "center;";
            }
            else
            {
                if (lastValue.EndsWith(";true;") && !IsClosed)
                {
                    lastValue = lastValue.Replace(";true;", ";false;");
                }
            }
            Page.ClientScript.RegisterHiddenField(this.ClientID + "_value", lastValue);
            Page.ClientScript.RegisterHiddenField(this.ClientID + "_scroll", lastScrollValue);
            Page.RegisterRequiresPostBack(this);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            lastValue = postCollection[this.ClientID + "_value"];
            lastScrollValue = postCollection[this.ClientID + "_scroll"];

            string[] value = postCollection[this.ClientID + "_value"].Split(';');
            if (value.Length >= 5)
            {
                this.Style[HtmlTextWriterStyle.Left] = value[0] + "px";
                this.Style[HtmlTextWriterStyle.Top] = value[1] + "px";
                this.Width = Unit.Parse(value[2] + "px");
                this.Height = Unit.Parse(value[3] + "px");
                this.IsClosed = bool.Parse(value[4]);
            }
            return true;

        }


        /// <summary>
        /// Ocorre quando a janela é fechada pelo botão fechar.
        /// </summary>
        /// <remarks></remarks>
        private EventHandler CloseEvent;
        public event EventHandler Close
        {
            add
            {
                CloseEvent = (EventHandler)System.Delegate.Combine(CloseEvent, value);
            }
            remove
            {
                CloseEvent = (EventHandler)System.Delegate.Remove(CloseEvent, value);
            }
        }

        protected void OnClose(EventArgs e)
        {
            if (CloseEvent != null)
                CloseEvent(this, e);
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            OnClose(new EventArgs());
        }
        #endregion
    }

    public class TitleStyle : Style
    {



        private Unit _PaddingTop;
        public Unit PaddingTop
        {
            get
            {
                return _PaddingTop;
            }
            set
            {
                _PaddingTop = value;
            }
        }

        private Unit _PaddingLeft;
        public Unit PaddingLeft
        {
            get
            {
                return _PaddingLeft;
            }
            set
            {
                _PaddingLeft = value;
            }
        }


        public override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer, System.Web.UI.WebControls.WebControl owner)
        {
            base.AddAttributesToRender(writer, owner);
            writer.AddStyleAttribute("float", "left");
            if (!PaddingTop.IsEmpty)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, PaddingTop.ToString());
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
            }
            if (!PaddingLeft.IsEmpty)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, PaddingLeft.ToString());
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "5px");
            }
        }

        protected override void FillStyleAttributes(System.Web.UI.CssStyleCollection attributes, System.Web.UI.IUrlResolutionService urlResolver)
        {
            base.FillStyleAttributes(attributes, urlResolver);
            attributes["float"] = "left";
            if (!PaddingTop.IsEmpty)
            {
                attributes[HtmlTextWriterStyle.PaddingTop] = PaddingTop.ToString();
            }
            else
            {
                attributes[HtmlTextWriterStyle.PaddingTop] = "5px";
            }
            if (!PaddingLeft.IsEmpty)
            {
                attributes[HtmlTextWriterStyle.PaddingLeft] = PaddingLeft.ToString();
            }
            else
            {
                attributes[HtmlTextWriterStyle.PaddingLeft] = "5px";
            }
        }
    }


    [Browsable(false)]
    public class WindowItem : System.Web.UI.WebControls.WebControl, INamingContainer
    { }
}