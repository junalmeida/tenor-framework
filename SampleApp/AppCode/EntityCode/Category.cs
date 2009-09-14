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
