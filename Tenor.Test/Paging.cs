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
            Type type = typeof(Person);

            SearchOptions so = new SearchOptions(type);
            so.Distinct = true;

            int pageSize = 2;

            PagingTestBase(pageSize, so, type);
        }

        [TestMethod]
        public void PagingWithConditionsTest()
        {
            Type type = typeof(Person);

            SearchOptions so = new SearchOptions(type);
            so.Distinct = true;

            so.Conditions.Add(new SearchCondition(string.Empty, Person.Properties.Name, "jane%", CompareOperator.Like));

            int pageSize = 2;

            PagingTestBase(pageSize, so, type);
        }

        [TestMethod]
        public void PagingWithJoinAndConditionsTest()
        {
            Type type = typeof(Person);

            SearchOptions so = new SearchOptions(type);
            so.Distinct = true;
            
            so.Conditions.Include("PersonItemList", "pi");
           
            so.Conditions.Add(new SearchCondition("pi", PersonItem.Properties.ItemId, 2));

            int pageSize = 2;

            PagingTestBase(pageSize, so, type);
        }

        [TestMethod]
        public void PagingWithJoinToManyAndConditionsTest()
        {
            Type type = typeof(Person);

            SearchOptions so = new SearchOptions(type);
            so.Distinct = true;

            so.Conditions.Include("PersonItemList", "pi");
            so.Conditions.Include("pi", "Item", "i", JoinMode.InnerJoin);

            so.Conditions.Add(new SearchCondition("i", Item.Properties.Description, "%item", CompareOperator.Like));

            int pageSize = 2;

            PagingTestBase(pageSize, so, type);
        }

        [TestMethod]
        public void PagingWithJoinToManyConditionsAndOtherTableSortingTest()
        {
            Type type = typeof(Item);
            
            SearchOptions so = new SearchOptions(type);
            so.Distinct = true;

            so.Conditions.Include("PersonItemList", "pi");
            so.Conditions.Include("pi", "Person", "p", JoinMode.InnerJoin);
            so.Conditions.Include(string.Empty, "Category", "c", JoinMode.LeftJoin);

            so.Conditions.Add(new SearchCondition("p", Person.Properties.Name, "%doe", CompareOperator.Like));

            so.Sorting.Add("c", Category.Properties.Name);
            so.Sorting.Add(Item.Properties.Description);

            int pageSize = 2;

            PagingTestBase(pageSize, so, type);
        }

        [TestMethod]
        public void PagingWithSortingToManyTest()
        {
            Type type = typeof(Item);

            SearchOptions so = new SearchOptions(type);
            so.Distinct = true;

            so.Conditions.Include("PersonItemList", "pi");
            so.Conditions.Include("pi", "Person", "p", JoinMode.InnerJoin);
            so.Conditions.Include(string.Empty, "Category", "c", JoinMode.LeftJoin);

            so.Sorting.Add("p", Person.Properties.Name);
            so.Sorting.Add("c", Category.Properties.Name);
            so.Sorting.Add(Item.Properties.Description);

            bool exceptionHappened = false;
            try
            {
                so.ExecutePaged(0, 2);
            }
            catch (InvalidSortException)
            {
                exceptionHappened = true;
            }

            if (!exceptionHappened)
                Assert.Fail("An InvalidSortException should have been thrown. Sorting by collection association fields is not allowed.");
        }
        /// <summary>
        /// Base to paging tests with person
        /// </summary>
        /// <param name="page">Desired page (zero-based)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="so">Search options</param>
        private void PagingTestBase(int pageSize, SearchOptions so, Type type)
        {
            BLLBase[] allItems = BLLBase.Search(so);

            int pages = (int)System.Math.Ceiling((double)allItems.Length / (double)pageSize);

            for (int page = 0; page < pages; page++)
            {
                BLLBase[] pagedInMemory = allItems.Skip(page * pageSize).Take(pageSize).ToArray();
                BLLBase[] items = so.ExecutePaged(page, pageSize);

                Assert.AreEqual(items.Length, pagedInMemory.Length, string.Format("Page {0} should have {1} items and has {2}.", page + 1, pagedInMemory.Length, items.Length));

                for (int i = 0; i < items.Length; i++)
                    PagingTestObjectComparison(items[i], pagedInMemory[i], type);
            }
        }

        /// <summary>
        /// Diferent ways to compare objects
        /// </summary>
        /// <param name="item1">First item</param>
        /// <param name="item2">Second item</param>
        /// <param name="type">Item type</param>
        private void PagingTestObjectComparison(BLLBase item1, BLLBase item2, Type type)
        {
            if (type == typeof(Person))
                Assert.AreEqual(((Person)item1).PersonId, ((Person)item2).PersonId);
            if (type == typeof(Item))
                Assert.AreEqual(((Item)item1).ItemId, ((Item)item2).ItemId);
        }
    }
}
