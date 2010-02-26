using System;
using System.Data;
using System.Configuration;
using System.Web;
using Tenor.BLL;
using System.Text;
using Tenor.Data;
using Tenor.Web;

#if SQLITE
// SQLite - We've found no way of having the special
// field return any numeric type other than Int64 :(
using DbInt = System.Int64;
#else
using DbInt = System.Int32;
#endif

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
                if (HasPhoto && PersonId <= int.MaxValue)
                    return TenorModule.GetInstanceUrl(this.GetType(), Convert.ToInt32(PersonId), Tenor.Drawing.ResizeMode.Proportional, 100, 100);
                else
                    return null;
            }
        }

        public string PhotoUrl
        {
            get
            {
                if (HasPhoto && PersonId <= int.MaxValue)
                    return TenorModule.GetInstanceUrl(this.GetType(), Convert.ToInt32(PersonId));
                else
                    return null;
            }
        }



        DbInt photoLength;

#if MYSQL
        // In MySql it's necessary to cast a bigint to tinyint in order for it to come as a bool
        // And, as there's no native function, we've created this one:
        /*
        CREATE DEFINER = 'root'@'%' FUNCTION `CastToTiny`(
                value BIGINT
            )
            RETURNS tinyint(1)
            DETERMINISTIC
            NO SQL
            SQL SECURITY DEFINER
            COMMENT ''
        BEGIN
          RETURN value;
        END;
        */
        //create the CastToTiny Function above.
        [SpecialField("CastToTiny(length(photo) > 0)")] 
#elif SQLITE
        [SpecialField("ifnull(length(photo), 0)")]
#elif MSSQL
        [SpecialField("isnull(len(photo), 0)")]
#elif ORACLE
        [SpecialField("nvl(dbms_lob.getlength(\"Photo\"), 0)")]
#elif POSTGRES        
        [SpecialField("COALESCE(length(\"Photo\"), 0)")]
#endif
        public DbInt PhotoLength
        {
            get { return photoLength; }
            set { photoLength = value; }
        }

        public bool HasPhoto
        {
            get { return PhotoLength > 0; }
        }
    }
}