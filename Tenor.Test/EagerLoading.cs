using System;
using System.Text;
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
      
            SearchOptions so = new SearchOptions(typeof(Item));
            so.Conditions.Add("Name", "First category", "c");
            so.Conditions.Include("Category", "c");
            so.LoadAlso("PersonItemList");
            so.LoadAlso("Category");
            Item[] items = (Item[])so.Execute();

            foreach (Item item in items)
            {
                Category cat = item.Category;
                IList<PersonItem> list = item.PersonItemList;
                
            }

#if DEBUG
            Assert.AreEqual(1, Tenor.BLL.BLLBase.LastSearches.Count, "Tenor generated more than one query.");
#else
            Assert.Inconclusive("Can only check generated queries on debug mode.");
         
#endif
        }
    }
}
