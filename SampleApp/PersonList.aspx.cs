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
namespace SampleApp
{

    public partial class PersonList : System.Web.UI.Page
    {
        BusinessProcess bp = new BusinessProcess();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            try
            {
                IList<Person> result = bp.ListPersons(txtName.Text, txtItemName.Text, txtCategory.Text);
                grdResults.DataKeyNames = new string[] { "PersonId" };
                grdResults.DataSource = result;
                grdResults.DataBind();
                pnlResults.Visible = true;
            }
            catch (ApplicationException ex)
            {
                throw;
                //Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
            }
        }
        protected void grdResults_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                Person person = new Person();
                person.PersonId = Convert.ToInt32(grdResults.DataKeys[e.RowIndex].Value);
                bp.Delete(person);
            }
            catch (ApplicationException ex)
            {
                Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
                return;
            }
            Search();
        }
    }
}