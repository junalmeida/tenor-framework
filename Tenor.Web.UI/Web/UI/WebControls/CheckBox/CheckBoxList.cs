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
using System.Drawing;



namespace Tenor.Web.UI.WebControls
{


    //Designer("System.Web.UI.Design.WebControls.ListControlDesigner", "System.ComponentModel.Design.IDesigner") _
    /// <summary>
    /// Implementa uma CheckBoxList com ações especiais.
    /// Você pode usar este controle para fazer uma lista com funções de selecionar tudo.
    /// todo: Implementar seleções com shift do teclado.
    /// </summary>
    /// <remarks></remarks>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.CheckBoxList), "CheckBoxList.bmp"), ToolboxData("<{0}:CheckBoxList runat=server></{0}:CheckBoxList>"), ParseChildren(true, "Items"), PersistChildren(false, false)]
    public class CheckBoxList : System.Web.UI.WebControls.CheckBoxList
    {

        [Category("Behavior"), Description("Determines the action that special itens will perform"), DefaultValue(typeof(CheckBoxSpecialItemAction), "SelectAndUnSelectAll")]
        public CheckBoxSpecialItemAction SpecialItemAction
        {
            get
            {
                if (ViewState["SpecialItemAction"] == null)
                {
                    return CheckBoxSpecialItemAction.SelectAndUnSelectAll;
                }
                else
                {
                    return ((CheckBoxSpecialItemAction)(ViewState["SpecialItemAction"]));
                }
            }
            set
            {
                ViewState["SpecialItemAction"] = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Behavior"), Description("Determines whether to display the special item on the list"), DefaultValue(typeof(CheckBoxSpecialItemPosition), "Bottom")]
        public CheckBoxSpecialItemPosition SpecialItemPosition
        {
            get
            {

                if (ViewState["SpecialItemPosition"] == null)
                {
                    return CheckBoxSpecialItemPosition.Bottom;
                }
                else
                {
                    return ((CheckBoxSpecialItemPosition)(ViewState["SpecialItemPosition"]));
                }
            }
            set
            {
                value = CheckBoxSpecialItemPosition.Bottom;
                ViewState["SpecialItemPosition"] = value;
            }
        }

        [Category("Appearance"), Description("Gets or sets the text to show on special items"), DefaultValue(typeof(CheckBoxSpecialItemPosition), "(Un)Select All")]
        public string SpecialItemText
        {
            get
            {
                if (ViewState["SpecialItemText"] == null)
                {
                    return "(Un)Select All";
                }
                else
                {
                    return ViewState["SpecialItemText"].ToString();
                }
            }
            set
            {
                ViewState["SpecialItemText"] = value;
            }
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsCheckBoxList);

            base.OnPreRender(e);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (Items.Count <= 1)
            {
                base.Render(writer);
            }
            else
            {
                ListItem li1 = new ListItem(SpecialItemText, "");
                ListItem li2 = new ListItem(SpecialItemText, "");
                if ((SpecialItemPosition == CheckBoxSpecialItemPosition.Top) || (SpecialItemPosition == CheckBoxSpecialItemPosition.TopAndBottom))
                {
                    Items.Insert(0, li1);
                }
                if ((SpecialItemPosition == CheckBoxSpecialItemPosition.Bottom) || (SpecialItemPosition == CheckBoxSpecialItemPosition.TopAndBottom))
                {
                    Items.Add(li2);
                }
                switch (SpecialItemAction)
                {
                    case CheckBoxSpecialItemAction.SelectAndUnSelectAll:
                        li1.Attributes["onclick"] = "CheckBoxList_SetAll(\'" + ClientID + "\', " + Items.Count.ToString() + ", this.checked);";
                        li2.Attributes["onclick"] = li1.Attributes["onclick"];
                        break;
                }
                base.Render(writer);
                Items.Remove(li1);
                Items.Remove(li2);
            }

        }

        protected override void RenderItem(System.Web.UI.WebControls.ListItemType itemType, int repeatIndex, System.Web.UI.WebControls.RepeatInfo repeatInfo, System.Web.UI.HtmlTextWriter writer)
        {
            //If String.IsNullOrEmpty(Me.Items(repeatIndex).Value) Then
            //    writer.AddAttribute("onclick", "CheckBoxList_SetAll('" & ClientID & "', " & Items.Count.ToString() & ", this.checked);")
            //End If
            base.RenderItem(itemType, repeatIndex, repeatInfo, writer);
        }


        /// <summary>
        /// Gets a list of selected items
        /// </summary>
        /// <value></value>
        /// <returns>An Array of ListItem</returns>
        /// <remarks></remarks>
        public ListItem[] SelectedItems
        {
            get
            {
                List<ListItem> res = new List<ListItem>();
                foreach (ListItem i in this.Items)
                {
                    if (i.Selected)
                    {
                        res.Add(i);
                    }
                }
                return res.ToArray();
            }
        }

    }

    public enum CheckBoxSpecialItemPosition
    {
        None,
        Top,
        Bottom,
        TopAndBottom
    }

    public enum CheckBoxSpecialItemAction
    {
        SelectAndUnSelectAll
    }



}