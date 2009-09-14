using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Tenor.Data;
namespace SampleApp.Business.Entities
{
    public partial class Department
    {
        public static Department[] List()
        {
            SortingCollection sc = new SortingCollection();
            sc.Add(Department.Properties.Name);

            return Department.Search(null, sc);
        }
    }
}