using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SampleApp.Business.Entities;
using Tenor.Data;

public partial class test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView1.DataSource = Category.Search(new SearchOptions(typeof(Category)));
            //GridView1.DataSource = Helper.QueryData(ConfigurationManager.ConnectionStrings[0], "select * from \"Categories\"", null);
            GridView1.DataBind();
        }
    }
}
