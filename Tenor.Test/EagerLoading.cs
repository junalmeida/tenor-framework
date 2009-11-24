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
            //provide a way to check if it was loaded on eager mode.


        }
    }
}
