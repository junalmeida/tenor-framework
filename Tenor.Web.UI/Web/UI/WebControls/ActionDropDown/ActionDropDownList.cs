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
    /// Contém uma lista de modos disponíveis para a ActionDropDownList
    /// </summary>
    /// <remarks></remarks>
    public enum ActionDropDownListMode
    {
        /// <summary>
        /// Adiciona um item novo à lista ao clicar no item especial.
        /// </summary>
        /// <remarks></remarks>
        AddNewItem
    }


    /// <summary>
    /// Exibe uma caixa de seleção em lista que possue um item especial que executa uma determinada ação.
    /// </summary>
    /// <remarks></remarks>
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
                //Uma lista especial não suporta o postback
                return base.AutoPostBack;
            }
            set
            {
                base.AutoPostBack = false;
                throw new NotSupportedException("Cannot have custom postback events. To use postback events use ASP.NET DropDownList instead.");
            }
        }

        /// <summary>
        /// Determina a ação para esta caixa de seleção especial.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// Define o rótulo que será exibido no item especial deste objeto.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// Define o texto exibido na caixa de entrada de dados exibida para o usuário.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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

            //Para forçar o registro do _doPostBack
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
                //adiciona o item para renderização pela classe base
                Items.Add(li);
                base.Render(writer);
                //remove o item do viewstate
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