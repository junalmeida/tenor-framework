
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

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        Person p = new Person();
        p.EnableLazyLoading(true);
        foreach (PersonItem i in p.PersonItemList)
        {
            
        }
        foreach (Department d in p.DepartmentList)
        {
        }

    }

    protected void btnCheck_Click(object sender, EventArgs e)
    {
        Tenor.Security.Captcha cap = Session["captcha"] as Tenor.Security.Captcha;
        if (cap == null)
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert("Unknown error");
        else if (cap.ValidateCaptcha(txtCaptcha.Text))
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert("You are a human! (or not...)");
        else 
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert("You are a human! But you typed a wrong code.");
    }
}
