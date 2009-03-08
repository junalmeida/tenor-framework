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
using SampleApp.Business.Entities;
using System.Collections.Generic;

public partial class PersonList : System.Web.UI.Page
{
    Business bp = new Business();
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            IList<Person> result = bp.ListPersons(txtName.Text, txtItemName.Text, txtCategory.Text);
            grdResults.DataSource = result;
            grdResults.DataBind();
            pnlResults.Visible = true;
        }
        catch (ApplicationException ex)
        {
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
        }
    }
}
