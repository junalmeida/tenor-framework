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


    /// <summary>
    /// Contém uma lista de direções.
    /// </summary>
    /// <remarks></remarks>
    public enum Direction
    {
        /// <summary>
        /// Acima.
        /// </summary>
        /// <remarks></remarks>
        Up,
        /// <summary>
        /// Abaixo.
        /// </summary>
        /// <remarks></remarks>
        Down
    }
    //System.ComponentModel.DesignerAttribute(GetType(design.ImageButtonDesigner)), _

    /// <summary>
    /// Disponibiliza a função de reordenar itens em uma gridview.
    /// Este controle deve estar em um template field de uma gridview do ASP.NET.
    /// A ordem final pode ser obtida através das funções <see cref="ReorderImageButton.GetIndexesForGridView">GetIndexesForGridView</see>,
    /// <see cref="ReorderImageButton.GetDataKeysValueForGridView">GetDataKeysValueForGridView</see>, ou <see cref="ReorderImageButton.GetDataKeysValuesForGridView">GetDataKeysValuesForGridView</see>.
    /// </summary>
    /// <remarks></remarks>
    [System.ComponentModel.DefaultPropertyAttribute("ImageUrl"), ToolboxData("<{0}:ReorderImageButton runat=server></{0}:ReorderImageButton>")]
    public class ReorderImageButton : Image
    {


        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);
            if (IsInsideGridView)
            {
                AttachJsAndHidden();
            }
        }


        private bool IsInsideGridView
        {
            get
            {
                return (this.NamingContainer != null && this.NamingContainer.GetType() == typeof(GridViewRow));
            }
        }


        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            if (IsInsideGridView)
            {
                GridViewRow row = (GridViewRow)this.NamingContainer;
                GridView gridView = (System.Web.UI.WebControls.GridView)row.NamingContainer;
                objVar = "ReorderButton_" + gridView.ClientID;
            }
        }


        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.DesignMode || IsInsideGridView)
            {
                base.Render(writer);
            }

        }



        /// <summary>
        /// Controla a ação deste ReorderImageButton.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DefaultValue(typeof(Direction), "Up"), Description("Determines wich action this image button will perform"), Category("Behavior")]
        public Direction Direction
        {
            get
            {
                if (ViewState["Direction"] == null)
                {
                    return WebControls.Direction.Up;
                }
                else
                {
                    return ((WebControls.Direction)(ViewState["Direction"]));
                }
            }
            set
            {
                if (value == WebControls.Direction.Up)
                {
                    ViewState.Remove("Direction");
                }
                else
                {
                    ViewState["Direction"] = value;
                }
            }
        }

        [DefaultValue(""), Description("Gets or sets the client-side script that executes when a ReorderImageButton control\'s Click event is raised."), Category("Behavior")]
        public string OnClientClick
        {
            get
            {
                if (ViewState["OnClientClick"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return System.Convert.ToString(ViewState["OnClientClick"]);
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    ViewState.Remove("OnClientClick");
                }
                else
                {
                    ViewState["OnClientClick"] = value;
                }
            }
        }



        private static bool handlerAttatched = false;
        public override void DataBind()
        {
            base.DataBind();
            AttachHandler();
        }

        protected override void DataBind(bool raiseOnDataBinding)
        {
            base.DataBind(raiseOnDataBinding);
            AttachHandler();
        }

        private void AttachHandler()
        {
            if (!handlerAttatched && IsInsideGridView)
            {

                GridViewRow row = (GridViewRow)this.NamingContainer;
                GridView gridView = (System.Web.UI.WebControls.GridView)row.NamingContainer;
                gridView.DataBound += new System.EventHandler(DataBound);

                handlerAttatched = true;
            }

        }



        private string objVar;
        private void AttachJsAndHidden()
        {


            GridViewRow row = (GridViewRow)this.NamingContainer;
            GridView gridView = (System.Web.UI.WebControls.GridView)row.NamingContainer;

            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsReorderButton);

            Page.ClientScript.RegisterHiddenField(gridView.ClientID + "_order", arrayInicial);
            Page.ClientScript.RegisterStartupScript(this.GetType(), gridView.ClientID, "\r\n" + " var " + objVar + " = new ReorderButton(\'" + gridView.ClientID + "\');" + "\r\n", true);
            if (!string.IsNullOrEmpty(arrayInicial))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), gridView.ClientID + "_array", "\r\n" + objVar + ".arrOrdem = new Array(" + arrayInicial + ");", true);
            }
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            if (IsInsideGridView)
            {
                GridViewRow row = (GridViewRow)this.NamingContainer;

                writer.AddStyleAttribute("cursor", "pointer");
                writer.AddAttribute("onclick", objVar + ".ReOrder(this, " + row.DataItemIndex + ", \'" + Direction.ToString().ToLower() + "\');" + OnClientClick);
            }
        }

        private string arrayInicial
        {
            get
            {
                if (ViewState["arrayInicial"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["arrayInicial"].ToString();
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    ViewState.Remove("arrayInicial");
                }
                else
                {
                    ViewState["arrayInicial"] = value;
                }
            }
        }


        private void DataBound(object sender, EventArgs e)
        {
            handlerAttatched = false;
            GridView gridView = (System.Web.UI.WebControls.GridView)sender;
            for (int i = 0; i <= gridView.Rows.Count - 1; i++)
            {
                if (arrayInicial.Length > 0)
                {
                    arrayInicial += ",";
                }
                arrayInicial += i.ToString();
            }

            gridView.DataBound -= new System.EventHandler(DataBound);
        }


        /// <summary>
        /// Retorna uma lista de indices na ordem definidos pelo usuário.
        /// </summary>
        /// <param name="gridView"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int[] GetIndexesForGridView(GridView gridView)
        {
            string hf = gridView.ClientID + "_order";
            string hfdados = HttpContext.Current.Request.Form[hf];
            List<int> array = new List<int>();
            if (!string.IsNullOrEmpty(hfdados))
            {
                foreach (string i in (hfdados + ",").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int indice = int.Parse(i);
                    array.Add(indice);

                }
            }
            return array.ToArray();
        }

        /// <summary>
        /// Retorna uma lista de datakeys na ordem definidos pelo usuário.
        /// Retorna somente o primeiro valor do datakey
        /// </summary>
        /// <param name="gridView"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object[] GetDataKeysValueForGridView(GridView gridView)
        {
            string hf = gridView.ClientID + "_order";
            string hfdados = HttpContext.Current.Request.Form[hf];
            List<object> array = new List<object>();
            if (!string.IsNullOrEmpty(hfdados))
            {
                foreach (string i in (hfdados + ",").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int indice = int.Parse(i);

                    array.Add(gridView.DataKeys[indice].Value);

                }
            }
            return array.ToArray();
        }

        /// <summary>
        /// Retorna uma lista de datakeys na ordem definidos pelo usuário.
        /// </summary>
        /// <param name="gridView"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static System.Collections.Specialized.IOrderedDictionary[] GetDataKeysValuesForGridView(GridView gridView)
        {
            string hf = gridView.ClientID + "_order";
            string hfdados = HttpContext.Current.Request.Form[hf];
            List<System.Collections.Specialized.IOrderedDictionary> array = new List<System.Collections.Specialized.IOrderedDictionary>();
            if (!string.IsNullOrEmpty(hfdados))
            {
                foreach (string i in (hfdados + ",").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int indice = int.Parse(i);

                    array.Add(gridView.DataKeys[indice].Values);

                }
            }
            return array.ToArray();
        }
    }


}
