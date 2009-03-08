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
    public partial class Item 
    {
        public static Item[] ListByCategory(int categoryId)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(Item.Properties.CategoryId, categoryId);
            SortingCollection sc = new SortingCollection();
            sc.Add(Item.Properties.Description);

            return Item.Search(cc, sc);
        }
    }
}
