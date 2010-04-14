
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Teste.TESTE;
using Tenor.Data;
using SampleApp.Business.Entities;

namespace SampleApp
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] teste = { 1, 2, 3 };

            grdRelatorio.DataSource = teste;
            grdRelatorio.DataBind();
        }


        //protected void btnCheck_Click(object sender, EventArgs e)
        //{
        //    Tenor.Security.Captcha cap = Session["captcha"] as Tenor.Security.Captcha;
        //    if (cap == null)
        //        Tenor.Web.UI.WebControls.ScriptManager.Current.Alert("Unknown error");
        //    else if (cap.ValidateCaptcha(txtCaptcha.Text))
        //        Tenor.Web.UI.WebControls.ScriptManager.Current.Alert("You are a human! (or not...)");
        //    else
        //        Tenor.Web.UI.WebControls.ScriptManager.Current.Alert("You are a human! But you typed a wrong code.");
        //}

        
        protected void grdRelatorio_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Adiociona o evento a textbox
                TextBox txtRetirada = (TextBox)e.Row.FindControl("txtRetirada");
                txtRetirada.TextChanged += new EventHandler(txtRetirada_TextChanged);
                txtRetirada.AutoPostBack = true;
            }
        }

        protected void txtRetirada_TextChanged(object sender, EventArgs e)
        {            
            TextBox txtRetirada = (TextBox)sender;
            txtRetirada.Text = "bla";

        }
    }
}