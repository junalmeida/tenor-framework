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
using Tenor.Web;

namespace SampleApp.Business.Entities
{
    public partial class Person : BLLBase
    {
        protected override bool Validate()
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(this.Name))
                sb.AppendLine("You must type a name.");
            if (!string.IsNullOrEmpty(this.Email) && !this.Email.Contains("@"))
                sb.AppendLine("You must type a valid email address.");

            if (sb.Length > 0)
                throw new ApplicationException("Could not save. Please check the following:\r\n" + sb.ToString()); 
            //If you don't need to throw exception, just return false.

            return true;
        }

        public static Person[] List(string name, string itemName, string categoryName)
        {
            ConditionCollection cc = new ConditionCollection();
            if (!string.IsNullOrEmpty(name))
            {
                cc.Add(Person.Properties.Name, name, CompareOperator.Like);
            }

            cc.Include(null, "PersonItemList", "pl", JoinMode.LeftJoin);
            cc.Include("pl", "Item", "i", JoinMode.LeftJoin);
            cc.Include("i", "Category", "c", JoinMode.LeftJoin);

            if (!string.IsNullOrEmpty(itemName))
            {
                if (cc.Count > 0)
                    cc.Add(LogicalOperator.And);

                cc.Add(Item.Properties.Description, itemName, CompareOperator.Like, "i");
            }
            if (!string.IsNullOrEmpty(categoryName))
            {
                if (cc.Count > 0)
                    cc.Add(LogicalOperator.And);
                cc.Add(Category.Properties.Name, categoryName, CompareOperator.Like, "c");
            }

            SortingCollection sc = new SortingCollection();
            sc.Add(Person.Properties.Name, SortOrder.Ascending);
            return Person.Search(cc, sc, true);
        }



        private Tenor.Drawing.Image image;
        [ResponseProperty()]
        public Tenor.Drawing.Image Image
        {
            get
            {
                if (image == null)
                    image = new Tenor.Drawing.Image(Photo);
                return image;
            }
        }

        public string ThumbPhotoUrl
        {
            get
            {
                if (HasPhoto)
                    return TenorModule.GetInstanceUrl(this.GetType(), PersonId, Tenor.Drawing.ResizeMode.Proportional, 100, 100);
                else
                    return null;
            }
        }

        private bool hasPhoto;


        [SpecialField("CastToTiny(length(photo) > 0)")]
        public bool HasPhoto
        {
            get { return hasPhoto; }
            set { hasPhoto = value; }
        }
	
    }
}
