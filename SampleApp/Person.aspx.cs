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

public partial class _Person : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadPerson();
        }
    }

    private int PersonId
    {
        get
        {
            int id = 0;
            int.TryParse(Request.QueryString["id"], out id);
            return id;
        }
    }
    Business bp = new Business();
    private void LoadPerson()
    {
        try
        {
            Person p = (PersonId > 0 ? bp.LoadPerson(PersonId) : new Person());
            txtName.Text = p.Name;
            txtEmail.Text = p.Email;
        }
        catch (ApplicationException ex)
        {
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            Person p = new Person();
            p.PersonId = PersonId;
            p.Name = txtName.Text;
            p.Email = txtEmail.Text;
            bp.Save(p);
        }
        catch (ApplicationException ex)
        {
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
        }
    }
}
