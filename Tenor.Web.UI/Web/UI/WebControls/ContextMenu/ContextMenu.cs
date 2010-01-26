using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Tenor.Web.UI.WebControls
{


    /// <summary>
    /// This control renders a context menu.
    /// </summary>
    [ToolboxData("<{0}:ContextMenu runat=server></{0}:ContextMenu>")]
    public class ContextMenu : ListControl, IPostBackEventHandler
    {



        public ContextMenu()
        {
            //TODO: WTF?
            throw (new NotImplementedException());
        }

        #region " Rendering "

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            string estilo = string.Empty;
            for (HtmlTextWriterStyle i = HtmlTextWriterStyle.Padding; i <= HtmlTextWriterStyle.PaddingTop; i++)
            {
                if (string.IsNullOrEmpty(estilo))
                {
                    estilo = Style[i];
                }
            }
            if (string.IsNullOrEmpty(estilo))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0");
            }
            estilo = string.Empty;
            for (HtmlTextWriterStyle i = HtmlTextWriterStyle.Margin; i <= HtmlTextWriterStyle.MarginTop; i++)
            {
                if (string.IsNullOrEmpty(estilo))
                {
                    estilo = Style[i];
                }
            }
            if (string.IsNullOrEmpty(estilo))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "0");
            }

            //writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none")

            base.AddAttributesToRender(writer);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Items.Count != 0)
            {
                base.Render(writer);
            }
        }

        private void RenderAccessKey(HtmlTextWriter writer, string AccessKey)
        {
            string str = AccessKey;
            if (str.Length > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, str);
            }
        }

        protected virtual void RenderMenuText(ListItem item, int index, HtmlTextWriter writer)
        {
            if (!item.Enabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }

            if (string.IsNullOrEmpty(item.Value))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, Page.ClientScript.GetPostBackClientHyperlink(this, Items.IndexOf(item).ToString()));
            }
            else if (item.Value == "-")
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0)");
                if (item.Enabled)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                }
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, base.ResolveClientUrl(item.Value));
                /*
                If Not String.IsNullOrEmpty(Me.Target) Then
                    writer.AddAttribute(HtmlTextWriterAttribute.Target, Me.Target)
                End If
                 */
            }

            this.RenderAccessKey(writer, this.AccessKey);
            writer.RenderBeginTag(HtmlTextWriterTag.A);

            HttpUtility.HtmlEncode(item.Text, writer);

            writer.RenderEndTag();
        }


        protected override void RenderContents(HtmlTextWriter writer)
        {
            int i;
            for (i = 0; i <= this.Items.Count - 1; i++)
            {
                ListItem li = this.Items[i];

                foreach (string att in this.Items[i].Attributes.Keys)
                {
                    writer.AddAttribute(att, this.Items[i].Attributes[att]);
                }

                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                this.RenderMenuText(li, i, writer);
                writer.RenderEndTag();
            }
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            this.RegisterScript();
            base.OnPreRender(e);

        }
        #endregion

        /// <summary>
        /// Gets or sets the control id associated with this context menu.
        /// </summary>
        [Themeable(false), TypeConverter(typeof(AssociatedControlConverter)), Category("Behavior"), IDReferenceProperty(), DefaultValue(""), Description("The control that will handle this context menu.")]
        public virtual string AssociatedControlID
        {
            get
            {
                string str = this.ViewState["AssociatedControlID"].ToString();
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["AssociatedControlID"] = value;
            }
        }


        #region " Disabled properties "
        [Bindable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override int SelectedIndex
        {
            get
            {
                return base.SelectedIndex;
            }
            set
            {
                throw (new NotSupportedException());
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ListItem SelectedItem
        {
            get
            {
                return base.SelectedItem;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public override string SelectedValue
        {
            get
            {
                return base.SelectedValue;
            }
            set
            {
                throw (new NotSupportedException());
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
                throw (new NotSupportedException());
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoPostBack
        {
            get
            {
                return base.AutoPostBack;
            }
            set
            {
                throw (new NotSupportedException());
            }
        }

        #endregion

        #region " Configurations "

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Ul;
            }
        }
        #endregion

        #region " Eventos "
        [Description("Handles the click event"), Category("Action")]
        private ContextMenuEventHandler ClickEvent;
        public event ContextMenuEventHandler Click
        {
            add
            {
                ClickEvent = (ContextMenuEventHandler)System.Delegate.Combine(ClickEvent, value);
            }
            remove
            {
                ClickEvent = (ContextMenuEventHandler)System.Delegate.Remove(ClickEvent, value);
            }
        }


        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (this.CausesValidation)
            {
                this.Page.Validate(this.ValidationGroup);
            }
            OnClick(new ContextMenuEventArgs(this.Items[int.Parse(eventArgument, System.Globalization.CultureInfo.InvariantCulture)]));
        }

        protected virtual void OnClick(ContextMenuEventArgs e)
        {
            if (ClickEvent != null)
                ClickEvent(this, e);
        }
        #endregion

        #region " Postback "
		/*
        private string GetPostBackEventReference(string eventArgument)
        {
            /*
            If (Me.CausesValidation AndAlso (Me.Page.GetValidators(Me.ValidationGroup).Count > 0)) Then
                Return ("javascript:" & Util.GetClientValidatedPostback(Me, Me.ValidationGroup, eventArgument))
            End If
             *//*
            return this.Page.ClientScript.GetPostBackClientHyperlink(this, eventArgument, true);
        }
	    */

        #endregion

        #region " Script "
        /// <summary>
        /// Registers required javascripts
        /// </summary>
        private void RegisterScript()
        {
            StringBuilder script = new StringBuilder();

            string reference = "document";
            if (this.AssociatedControlID != string.Empty)
            {
                Control control = this.FindControl(AssociatedControlID);
                if (control == null)
                {
                    if (!base.DesignMode)
                    {
                        throw (new HttpException("AssociatedControlID was not found."));
                    }
                }
                else
                {
                    reference = "$(\'" + control.ClientID + "\')";
                }
            }

            script.AppendLine("window.addEvent(\"load\", function() {");
            script.AppendLine("     new DDMenu (\'" + ClientID + "\', " + reference + ", { ");
            script.AppendLine("         onOpen: function (e) {");
            script.AppendLine("             this.enableItems(true);");
            script.AppendLine("         },");
            script.AppendLine("         onItemSelect: function (act_id, act_el, menu_bindon) {");
            script.AppendLine("         }");
            script.AppendLine("     });");
            script.AppendLine("});");

            if (!MooTools.CheckMooTools(MooTools.MooVersion.Version_1_11, MooTools.MooModule.CoreEffectsDrag))
            {
                throw (new InvalidOperationException("ContextMenu requires MooTools CoreEffectsDrag or Full module. Check the ScriptManager."));
            }

            Page.ClientScript.RegisterClientScriptResource(typeof(ContextMenu), Configuration.Resources.JsContextMenooSrc);
            Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID, script.ToString(), true);
        }
        #endregion
    }

    #region " Other Stuff "
    public delegate void ContextMenuEventHandler(object sender, ContextMenuEventArgs e);

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ContextMenuEventArgs : EventArgs
    {


        public ContextMenuEventArgs(ListItem li)
        {
            this._index = li;
        }

        public string Key
        {
            get
            {
                return this._index.Value;
            }
        }

        public string Text
        {
            get
            {
                return this._index.Text;
            }
        }

        private ListItem _index;
    }
    #endregion
}