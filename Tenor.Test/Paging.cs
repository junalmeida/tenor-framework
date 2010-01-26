using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tenor.BLL;
using Tenor.Data;
using SampleApp.Business.Entities;

namespace Tenor.Test
{
    /// <summary>
    /// Summary description for Paging
    /// </summary>
    [TestClass]
    public class Paging
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void PagingTest()
        {
            SearchOptions so = new SearchOptions(typeof(Person));
            so.Conditions.Include("PersonItemList", "pi");
            so.Conditions.Include("pi", "Item", "i", JoinMode.InnerJoin);
            so.Distinct = true;

            //so.Conditions.Add(new SearchCondition(string.Empty, Person.Properties.Name, "jane%", CompareOperator.Like));
            so.Conditions.Add(new SearchCondition("i", Item.Properties.Description, "%item", CompareOperator.Like));

            so.Sorting.Add(Person.Properties.PersonId, SortOrder.Descending);

            // zero-based
            int page = 0;
            int pageSize = 2;

            Person[] items = (Person[])so.ExecutePaged(page, pageSize);

            Person[] allItems = Person.Search(so.Conditions, so.Sorting, true);

            Person[] pagedInMemory = allItems.Skip(page * pageSize).Take(pageSize).ToArray();

            Assert.AreEqual(items.Length, pagedInMemory.Length);

            for (int i = 0; i < items.Length; i++)
            {
                Assert.AreEqual(items[i].PersonId, pagedInMemory[i].PersonId);
            }
        }
    }
}
