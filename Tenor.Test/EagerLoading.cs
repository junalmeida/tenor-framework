using System.Collections.Generic;
using Tenor.Data;
using SampleApp.Business.Entities;

#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
#endif

namespace Tenor.Test
{
    /// <summary>
    /// Summary description for EagerLoading
    /// </summary>
    [TestClass]
    public class EagerLoading : TestBase
    {
        [TestMethod]
        public void EagerLoadingTest()
        {
#if DEBUG
            EntityBase.LastSearches.Clear();
#endif

            SearchOptions so = new SearchOptions(typeof(Item));
            so.Conditions.Add("Name", "First category", "c");
            so.Conditions.Include("Category", "c");
            so.LoadAlso("PersonItemList");
            so.LoadAlso("Category");
            Item[] items = (Item[])so.Execute();
            if (items.Length == 0)
                Assert.Inconclusive("No query results.");

            foreach (Item item in items)
            {
                Category cat = item.Category;
                IList<PersonItem> list = item.PersonItemList;

            }
#if DEBUG
            Assert.AreEqual(1, EntityBase.LastSearches.Count, "Tenor has generated more than one query.");
#else
            Assert.Inconclusive("Can only check generated queries on debug mode.");
         
#endif
        }
    }
}
