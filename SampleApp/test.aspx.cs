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
using Tenor.Drawing;

namespace SampleApp
{
    public partial class test : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "image/jpeg";
            //BarCode bcode = new BarCode(34191183400000292011090000107160253500375000M);
            BarCode bcode = new BarCode(34191183400000292);
            System.Drawing.Image img = bcode.Generate();
            img.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            Response.End();

            if (!IsPostBack)
            {
                GridView2.DataSource = Category.Search(new SearchOptions(typeof(Category)));
                //GridView1.DataSource = Helper.QueryData(ConfigurationManager.ConnectionStrings[0], "select * from \"Categories\"", null);
                GridView2.DataBind();

            }
        }
    }
}