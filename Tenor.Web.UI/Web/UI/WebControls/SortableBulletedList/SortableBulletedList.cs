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
using System.IO;
using System.Text.RegularExpressions;


namespace Tenor.Web.UI.WebControls
{



    /// <summary>
    /// This control renders an orderable bulleted list that can contain html on its items.
    /// </summary>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:SortableBulletedList runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.BulletedList), "BulletedList.bmp")]
    public class SortableBulletedList : System.Web.UI.WebControls.BulletedList, IPostBackDataHandler
    {


        private void SortableBulletedList_Load(object sender, System.EventArgs e)
        {
            Page.RegisterRequiresPostBack(this);
        }


        [DefaultValue(typeof(Orientation), "Vertical"), Category("Layout"), Description("Indica a orientação da lista.")]
        public Orientation Orientation
        {
            get
            {
                if (ViewState["Orientation"] == null)
                {
                    return System.Web.UI.WebControls.Orientation.Vertical;
                }
                return ((System.Web.UI.WebControls.Orientation)(ViewState["Orientation"]));
            }
            set
            {
                ViewState["Orientation"] = value;
            }
        }



        [Category("Layout"), Description("Define uma classe CSS para cada item da lista.")]
        public string ItemCssClass
        {
            get
            {
                return ViewState["ItemCssClass"].ToString();
            }
            set
            {
                ViewState["ItemCssClass"] = value;
            }
        }


        [Category("Behavior"), Description("Define um função para ser chamada ao completar uma mudança de ordem.")]
        public string OnCompleteScript
        {
            get
            {
                return ViewState["OnCompleteScript"].ToString();
            }
            set
            {
                ViewState["OnCompleteScript"] = value;
            }
        }


        private void SortableBulletedList_PreRender(object sender, System.EventArgs e)
        {
            if (!MooTools.CheckMooTools(MooTools.MooModule.CoreSortables))
            {
                throw (new InvalidOperationException("SortableBulletedList requires MooTools Sortables or Full module. Check the ScriptManager."));
            }

            Page.ClientScript.RegisterHiddenField(Configuration.HttpModule.IdPrefix + ClientID, string.Empty);
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsSortable);

            List<string> values = new List<string>();

            foreach (ListItem li in this.Items)
            {
                if (string.IsNullOrEmpty(li.Value))
                {
                    throw (new InvalidOperationException("SortableBulletedList items must have a value."));
                }
                else
                {

                    if (values.Contains(li.Value))
                    {
                        throw (new InvalidOperationException("SortableBulletedList items must have unique values."));
                    }
                    else
                    {
                        values.Add(li.Value);
                    }

                    if (!string.IsNullOrEmpty(ItemCssClass) && string.IsNullOrEmpty(li.Attributes["class"]))
                    {
                        li.Attributes["class"] = ItemCssClass;
                    }
                }
            }
            string script = OnCompleteScript;
            if (string.IsNullOrEmpty(script))
            {
                script = "null";
            }
            else if (script.Contains(" ") || script.Contains(";"))
            {
                script = "function () {" + script + "}";
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), ClientID, "SortableBulletedList(\'" + ClientID + "\', Array(\'" + string.Join("\',\'", values.ToArray()) + ("\'), \'" + Orientation.ToString().ToLower() + "\', " + script + ", \'" + Configuration.HttpModule.IdPrefix + "\')") + Environment.NewLine, true);
        }

        protected override void RenderBulletText(System.Web.UI.WebControls.ListItem item, int index, System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(item.Text);
        }



        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            string str = postCollection[Configuration.HttpModule.IdPrefix + ClientID];
            if (!string.IsNullOrEmpty(str))
            {
                ListItemCollection ListaVelha = new ListItemCollection();

                ListItem[] arrTemp = new ListItem[this.Items.Count];
                this.Items.CopyTo(arrTemp, 0);
                this.Items.Clear();

                ListaVelha.AddRange(arrTemp);

                string[] ordens = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string i in ordens)
                {
                    this.Items.Add(ListaVelha.FindByValue(i));
                }

                OnOrderChanged();
            }
            return true;

        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
        }


        #region "Events"

        private EventHandler OrderChangedEvent;
        public event EventHandler OrderChanged
        {
            add
            {
                OrderChangedEvent = (EventHandler)System.Delegate.Combine(OrderChangedEvent, value);
            }
            remove
            {
                OrderChangedEvent = (EventHandler)System.Delegate.Remove(OrderChangedEvent, value);
            }
        }

        protected void OnOrderChanged()
        {
            if (OrderChangedEvent != null)
                OrderChangedEvent(this, new EventArgs());
        }
        #endregion


    }
}