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
