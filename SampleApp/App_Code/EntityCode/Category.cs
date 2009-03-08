using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Tenor.BLL;
using System.Text;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    public partial class Category 
    {
        public static Category[] List()
        {
            SortingCollection sc = new SortingCollection();
            sc.Add(Category.Properties.Name);

            return Category.Search(null, sc);
        }
    }
}
