using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace Tenor.Web.UI.WebControls
{
    /*
    PersistChildren(False), _
    ParseChildren(GetType(ListItem), ChildrenAsProperties:=False, DefaultProperty:="Items"), _

    Designer(GetType(System.Web.UI.Design.WebControls.ListControlDesigner)), _
    DefaultEvent("Click"), DefaultProperty("DisplayMode"), _
     */
    /// <summary>
    /// This control renders a tabbed display.
    /// </summary>
    [Themeable(true), AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class Tabs : ListControl, IPostBackEventHandler
    {
        private TabsEventHandler ClickEvent;
        public event TabsEventHandler Click
        {
            add
            {
                ClickEvent = (TabsEventHandler)System.Delegate.Combine(ClickEvent, value);
            }
            remove
            {
                ClickEvent = (TabsEventHandler)System.Delegate.Remove(ClickEvent, value);
            }
        }


        internal static string GetClientValidatedPostback(Control control, string validationGroup, string argument)
        {
            string text = control.Page.ClientScript.GetPostBackEventReference(control, argument, true);
            return (GetClientValidateEvent(validationGroup) + @text);
        }

        internal static string GetClientValidateEvent(string validationGroup)
        {
            if (validationGroup == null)
            {
                validationGroup = string.Empty;
            }
            return ("if (typeof(Page_ClientValidate) == \'function\') Page_ClientValidate(\'" + validationGroup + "\'); ");
        }


        protected override void OnPreRender(System.EventArgs e)
        {
            CreateStyle(this.Page, "#" + ClientID);
            base.OnPreRender(e);
        }


        private string GetPostBackEventReference(string eventArgument)
        {
            if (eventArgument == null)
            {
                eventArgument = string.Empty;
            }
            if (this.CausesValidation && (this.Page.GetValidators(this.ValidationGroup).Count > 0))
            {
                return ("javascript:" + GetClientValidatedPostback(this, this.ValidationGroup, eventArgument));
            }
            return this.Page.ClientScript.GetPostBackClientHyperlink(this, eventArgument, true);
        }

        protected virtual void OnClick(TabsEventArgs e)
        {
            if (ClickEvent != null)
                ClickEvent(this, e);
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (this.CausesValidation)
            {
                this.Page.Validate(this.ValidationGroup);
            }
            this.OnClick(new TabsEventArgs(int.Parse(eventArgument, System.Globalization.CultureInfo.InvariantCulture)));
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Items.Count != 0)
            {
                base.Render(writer);
            }
        }

        internal void RenderAccessKey(HtmlTextWriter writer, string AccessKey)
        {
            string text = AccessKey;
            if (@text.Length > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, @text);
            }
        }

        protected virtual void RenderTab(ListItem item, int index, HtmlTextWriter writer)
        {
            switch (this.DisplayMode)
            {
                case BulletedListDisplayMode.Text:
                    if (!item.Enabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                    }
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    HttpUtility.HtmlDecode(item.Text, writer);
                    writer.RenderEndTag();
                    return;
                case BulletedListDisplayMode.HyperLink:
                    if (!this._cachedIsEnabled || !item.Enabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                        break;
                    }
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, base.ResolveClientUrl(item.Value));
                    if (!string.IsNullOrEmpty(this.Target))
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Target, this.Target);
                    }
                    break;
                case BulletedListDisplayMode.LinkButton:
                    if (!this._cachedIsEnabled || !item.Enabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                    }
                    else
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, this.GetPostBackEventReference(index.ToString(System.Globalization.CultureInfo.InvariantCulture)));
                    }
                    this.RenderAccessKey(writer, this.AccessKey);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    HttpUtility.HtmlDecode(item.Text, writer);
                    writer.RenderEndTag();
                    return;
                default:
                    return;
            }
            this.RenderAccessKey(writer, this.AccessKey);
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            HttpUtility.HtmlDecode(item.Text, writer);
            writer.RenderEndTag();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            this._cachedIsEnabled = base.IsEnabled;
            if (this._itemCount == -1)
            {
                int i;
                for (i = 0; i <= this.Items.Count - 1; i++)
                {
                    if (this.Items[i].Selected)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "selected");
                    }
                    this.Items[i].Attributes.AddAttributes(writer);
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                    this.RenderTab(this.Items[i], i, writer);
                    writer.RenderEndTag();
                }
            }
            else
            {
                int j;
                for (j = this._firstItem; j <= (this._firstItem + this._itemCount) - 1; j++)
                {
                    if (this.Items[j].Selected)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "selected");
                    }
                    this.Items[j].Attributes.AddAttributes(writer);
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                    this.RenderTab(this.Items[j], j, writer);
                    writer.RenderEndTag();
                }
            }
        }
        /*
        					void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        					{
        						this.RaisePostBackEvent1(eventArgument);
        					}
         */


        #region " Propriedades Desativadas "

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoPostBack
        {
            get
            {
                return base.AutoPostBack;
            }
            set
            {
                throw (new NotSupportedException("Autopostback is not supported"));
            }
        }


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override short TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            set
            {
                throw (new NotSupportedException("TabIndex is not supported"));
            }
        }

        public override ControlCollection Controls
        {
            get
            {
                return new EmptyControlCollection(this);
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Ul;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string @Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                throw (new NotSupportedException("Text is not supported"));
            }
        }


        #endregion

        #region " Properties "


        internal void ApplyTabStyle(Style Style)
        {
            if (_TabStyle == null)
            {
                _TabStyle = new Style();
            }
            _TabStyle.CopyFrom(Style);
        }



        internal void ApplySelectedTabStyle(Style Style)
        {
            if (_SelectedTabStyle == null)
            {
                _SelectedTabStyle = new Style();
            }
            _SelectedTabStyle.CopyFrom(Style);
        }


        [Description(""), DefaultValue(0), Category("Behavior")]
        public virtual BulletedListDisplayMode DisplayMode
        {
            get
            {
                object obj2 = this.ViewState["DisplayMode"];
                if (obj2 != null)
                {
                    return ((BulletedListDisplayMode)obj2);
                }
                return BulletedListDisplayMode.Text;
            }
            set
            {
                if ((value < BulletedListDisplayMode.Text) || (value > BulletedListDisplayMode.LinkButton))
                {
                    throw (new ArgumentOutOfRangeException("value"));
                }
                this.ViewState["DisplayMode"] = value;
            }
        }

        [Description("Link target"), Category("Behavior"), DefaultValue(""), TypeConverter(typeof(TargetConverter))]
        public virtual string Target
        {
            get
            {
                object obj2 = this.ViewState["Target"];
                if (obj2 != null)
                {
                    return obj2.ToString();
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["Target"] = value;
            }
        }

        [TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), Category("Appearance"), Description("BackColor of a Tab Item")]
        public virtual Color TabBackColor
        {
            get
            {
                if (_TabStyle == null)
                {
                    _TabStyle = new Style();
                }
                return this._TabStyle.BackColor;
            }
            set
            {
                if (_TabStyle == null)
                {
                    _TabStyle = new Style();
                }
                this._TabStyle.BackColor = value;
            }
        }


        [Category("Appearance"), Description("Tab BorderColor"), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]
        public virtual System.Drawing.Color TabBorderColor
        {
            get
            {
                if (_TabStyle == null)
                {
                    _TabStyle = new Style();
                }
                return this._TabStyle.BorderColor;
            }
            set
            {
                if (_TabStyle == null)
                {
                    _TabStyle = new Style();
                }
                this._TabStyle.BorderColor = value;
            }
        }




        [DefaultValue(0), Category("Appearance"), Description("Tab BorderStyle")]
        public virtual BorderStyle TabBorderStyle
        {
            get
            {
                if (_TabStyle == null)
                {
                    _TabStyle = new Style();
                }
                return this._TabStyle.BorderStyle;
            }
            set
            {
                if (_TabStyle == null)
                {
                    _TabStyle = new Style();
                }
                this._TabStyle.BorderStyle = value;
            }
        }

        [Description("Tab BorderWidth"), Category("Appearance"), DefaultValue(typeof(Unit), "")]
        public virtual Unit TabBorderWidth
        {
            get
            {
                if (_TabStyle == null)
                {
                    _TabStyle = new Style();
                }
                return this._TabStyle.BorderWidth;
            }
            set
            {
                if (_TabStyle == null)
                {
                    _TabStyle = new Style();
                }
                this._TabStyle.BorderWidth = value;
            }
        }







        [TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), Category("Appearance"), Description("BackColor of a SelectedTab Item")]
        public virtual System.Drawing.Color SelectedTabBackColor
        {
            get
            {
                if (_SelectedTabStyle == null)
                {
                    _SelectedTabStyle = new Style();
                }
                return this._SelectedTabStyle.BackColor;
            }
            set
            {
                if (_SelectedTabStyle == null)
                {
                    _SelectedTabStyle = new Style();
                }
                this._SelectedTabStyle.BackColor = value;
            }
        }


        [Category("Appearance"), Description("SelectedTab BorderColor"), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]
        public virtual System.Drawing.Color SelectedTabBorderColor
        {
            get
            {
                if (_SelectedTabStyle == null)
                {
                    _SelectedTabStyle = new Style();
                }
                return this._SelectedTabStyle.BorderColor;
            }
            set
            {
                if (_SelectedTabStyle == null)
                {
                    _SelectedTabStyle = new Style();
                }
                this._SelectedTabStyle.BorderColor = value;
            }
        }




        [DefaultValue(0), Category("Appearance"), Description("SelectedTab BorderStyle")]
        public virtual BorderStyle SelectedTabBorderStyle
        {
            get
            {
                if (_SelectedTabStyle == null)
                {
                    _SelectedTabStyle = new Style();
                }
                return this._SelectedTabStyle.BorderStyle;
            }
            set
            {
                if (_SelectedTabStyle == null)
                {
                    _SelectedTabStyle = new Style();
                }
                this._SelectedTabStyle.BorderStyle = value;
            }
        }

        [Description("SelectedTab BorderWidth"), Category("Appearance"), DefaultValue(typeof(Unit), "")]
        public virtual Unit SelectedTabBorderWidth
        {
            get
            {
                if (_SelectedTabStyle == null)
                {
                    _SelectedTabStyle = new Style();
                }
                return this._SelectedTabStyle.BorderWidth;
            }
            set
            {
                if (_SelectedTabStyle == null)
                {
                    _SelectedTabStyle = new Style();
                }
                this._SelectedTabStyle.BorderWidth = value;
            }
        }


        [Description("Tab spacing"), Category("Appearance"), DefaultValue(typeof(Unit), "")]
        public virtual Unit TabSeparator
        {
            get
            {
                if (ViewState["TabSeparator"] == null)
                {
                    return Unit.Empty;
                }
                else
                {
                    return ((Unit)(ViewState["TabSeparator"]));
                }
            }
            set
            {
                if (value.IsEmpty)
                {
                    ViewState["TabSeparator"] = null;
                }
                else
                {
                    ViewState["TabSeparator"] = value;
                }
            }
        }

        #endregion




        private Style _TabStyle;
        private Style _SelectedTabStyle;
        protected override System.Web.UI.WebControls.Style CreateControlStyle()
        {
            return base.CreateControlStyle();
        }

        private bool _cachedIsEnabled;
        private int _firstItem = 0;
        private int _itemCount = -1;


        internal Unit RealHeight
        {
            get
            {
                if (Height.IsEmpty)
                {
                    if (this.BorderStyle == System.Web.UI.WebControls.BorderStyle.None || this.BorderStyle == System.Web.UI.WebControls.BorderStyle.NotSet || this.BorderWidth.IsEmpty)
                    {
                        return Unit.Parse("22px");
                    }
                    else
                    {
                        return new Unit(22 - BorderWidth.Value, UnitType.Pixel);
                    }
                }
                else
                {
                    return Height;
                }
            }
        }



        internal void CreateStyle(Page Page, string RootCSS)
        {

            if (Page == null)
            {
                throw (new InvalidOperationException("Cannot access the page class"));
            }
            else if (Page.Header == null)
            {
                throw (new InvalidOperationException("Cannot access the page header. Set runat attribute of head tag to server."));
            }


            CustomStyle ul = new CustomStyle();
            ul.Style["margin"] = "0";
            ul.Style["padding-bottom"] = RealHeight.ToString();
            ul.Style["padding-left"] = "0px";
            ul.Style["height"] = "auto !important";
            ul.Style["list-style-type"] = "none";

            if (BorderStyle != System.Web.UI.WebControls.BorderStyle.NotSet && BorderStyle != System.Web.UI.WebControls.BorderStyle.None)
            {
                ul.Style["border-top"] = "0 !important";
                ul.Style["border-left"] = "0 !important";
                ul.Style["border-right"] = "0 !important";
                ul.Style["border-bottom"] = BorderStyle.ToString().ToLower() + " " + BorderWidth.ToString() + " " + System.Drawing.ColorTranslator.ToHtml(BorderColor);
            }
            Page.Header.StyleSheet.CreateStyleRule(ul, null, RootCSS);

            CustomStyle li = new CustomStyle();
            li.Style["list-style-type"] = "none";
            li.Style["margin"] = "0";
            li.Style["padding"] = "0";
            li.Style["display"] = "inline";
            Page.Header.StyleSheet.CreateStyleRule(li, null, RootCSS + " li");

            CustomStyle liA = new CustomStyle();
            liA.Style["float"] = "left";
            liA.Style["line-height"] = "14px";
            liA.Style["padding"] = "2px 10px 2px 10px;";
            liA.Style["text-decoration"] = "none";
            liA.Style["margin-top"] = "2px";
            if (TabSeparator.IsEmpty)
            {
                liA.Style["margin-right"] = "2px";
            }
            else
            {
                liA.Style["margin-right"] = TabSeparator.ToString();
            }

            if (!ForeColor.IsEmpty)
            {
                liA.Style["color"] = ForeColor.ToString();
            }
            Page.Header.StyleSheet.CreateStyleRule(liA, null, RootCSS + " a:link, " + RootCSS + " a:visited, " + RootCSS + " span");
            if (!_TabStyle.IsEmpty)
            {
                Page.Header.StyleSheet.CreateStyleRule(_TabStyle, null, RootCSS + " a:link, " + RootCSS + " a:visited, " + RootCSS + " span");
            }


            CustomStyle liAselected = new CustomStyle();
            liAselected.Style["padding"] = "4px 10px 2px 10px;";
            liAselected.Style["margin-top"] = "0px";

            if (_SelectedTabStyle.BorderStyle == System.Web.UI.WebControls.BorderStyle.NotSet || _SelectedTabStyle.BorderStyle == System.Web.UI.WebControls.BorderStyle.None)
            {
                if (!SelectedTabBackColor.IsEmpty)
                {
                    liAselected.Style["border-bottom"] = "solid 1px " + System.Drawing.ColorTranslator.ToHtml(SelectedTabBackColor);
                }
                else
                {
                    liAselected.Style["border-bottom"] = "solid 1px white";
                }
                /*
                .Style("border-bottom") = _SelectedTabStyle.BorderStyle.ToString().ToLower() & " " & _SelectedTabStyle.BorderWidth.ToString() & " " & Drawing.ColorTranslator.ToHtml(_SelectedTabStyle.BorderColor)
                 */
            }
            Page.Header.StyleSheet.CreateStyleRule(liAselected, null, RootCSS + " li.selected a:link, " + RootCSS + " li.selected a:visited, " + RootCSS + " li.selected span");
            if (!_SelectedTabStyle.IsEmpty)
            {
                Page.Header.StyleSheet.CreateStyleRule(_SelectedTabStyle, null, RootCSS + " li.selected a:link, " + RootCSS + " li.selected a:visited, " + RootCSS + " li.selected span");
            }


        }


    }

    public delegate void TabsEventHandler(object sender, TabsEventArgs e);

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class TabsEventArgs : EventArgs
    {

        public TabsEventArgs(int index)
        {
            this._index = index;
        }


        public int Index
        {
            get
            {
                return this._index;
            }
        }

        private int _index;
    }
}