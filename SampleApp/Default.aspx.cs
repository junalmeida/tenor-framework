
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
            int[] testValuesToBind = { 1, 2, 3 };

            gridReport.DataSource = testValuesToBind;
            gridReport.DataBind();
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


        protected void gridReport_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Adiociona o evento a textbox
                TextBox txtValue = (TextBox)e.Row.FindControl("txtValue");
                txtValue.TextChanged += new EventHandler(txtValue_TextChanged);
                txtValue.AutoPostBack = true;
            }
        }

        protected void txtValue_TextChanged(object sender, EventArgs e)
        {            
            TextBox txtValue = (TextBox)sender;
            txtValue.Text = "bla";

        }
    }
}