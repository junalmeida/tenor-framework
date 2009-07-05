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

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Product p = new Product();
        p.ID = 11;
        p.Delete();

        gv.DataSource = Product.Search(new SearchOptions(typeof(Product)));
        gv.DataBind();
    }
}
