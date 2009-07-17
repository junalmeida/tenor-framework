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
using System.ComponentModel.Design;
using System.Drawing;


namespace Tenor.Web.UI.WebControls
{


    /// <summary>
    /// Defines the drop down list behavior.
    /// </summary>
    public enum ActionDropDownListMode
    {
        /// <summary>
        /// Adds a new item to the list when a special item is activated by the user.
        /// </summary>
        AddNewItem
    }


    /// <summary>
    /// This control shows a combobox with a special item that performs a defined action.
    /// </summary>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:ActionDropDownList runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.DropDownList), "DropDownList.bmp")]
    public class ActionDropDownList : System.Web.UI.WebControls.DropDownList, IPostBackEventHandler
    {
        private AddingNewEventHandler AddingNewEvent;
        public event AddingNewEventHandler AddingNew
        {
            add
            {
                AddingNewEvent = (AddingNewEventHandler)System.Delegate.Combine(AddingNewEvent, value);
            }
            remove
            {
                AddingNewEvent = (AddingNewEventHandler)System.Delegate.Remove(AddingNewEvent, value);
            }
        }



        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override bool AutoPostBack
        {
            get
            {
                //We cant support autopostback
                return base.AutoPostBack;
            }
            set
            {
                base.AutoPostBack = false;
                throw new NotSupportedException("Cannot have custom postback events. To use postback events use ASP.NET DropDownList instead.");
            }
        }

        /// <summary>
        /// Gets or sets the action of the special item.
        /// </summary>
        [Description("The behavior mode of the DropDownList"), Category("Behavior"), DefaultValue(typeof(ActionDropDownListMode), "AddNewItem")]
        public ActionDropDownListMode DropDownMode
        {
            get
            {
                return ((ActionDropDownListMode)(ViewState["Action"]));
            }
            set
            {
                ViewState["Action"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the special item's label.
        /// </summary>
        [Description("Defines the label of the custom item"), Category("Appearance"), DefaultValue("New Label")]
        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(ViewState["ActionLabel"].ToString()))
                {
                    return "New Label";
                }
                return ViewState["ActionLabel"].ToString();
            }
            set
            {
                ViewState["ActionLabel"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the text shown on the inputbox prompt. 
        /// </summary>
        [Description("Defines the text to show on the prompt"), Category("Appearance"), DefaultValue("Type below the new label")]
        public string ActionText
        {
            get
            {
                if (string.IsNullOrEmpty(ViewState["ActionText"].ToString()))
                {
                    return "Type below the new label";
                }
                return ViewState["ActionText"].ToString();
            }
            set
            {
                ViewState["ActionText"] = value;
            }
        }


        protected override void OnPreRender(System.EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsDropdown);

            //forcing _doPostPack declarations.
            Page.RegisterRequiresPostBack(this);

            Page.ClientScript.GetPostBackClientHyperlink(this, string.Empty);

            base.OnPreRender(e);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.DesignMode)
            {
                base.Render(writer);
            }
            else
            {

                ListItem li = new ListItem(ActionLabel, "");
                li.Attributes["id"] = this.ClientID + this.ClientIDSeparator + "AddNewItem";

                //TODO: Oh god, may we have a better way to do this:
                //adds the special item.
                Items.Add(li);
                base.Render(writer);
                //removes the item from viewstate and user-lists.
                Items.Remove(li);

            }
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            writer.AddAttribute("onchange", "ActionDropDownList_Check(this, \'" + ActionText.Replace("\r\n", "\\n").Replace("\'", "\\").Replace("\t", "\\t") + "\')");
            base.AddAttributesToRender(writer);
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (AddingNewEvent != null)
                AddingNewEvent(this, new AddingNewEventArgs(eventArgument));
        }
    }
}