using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp.Business.Entities;

namespace Tenor.Test
{
    /// <summary>
    /// Summary description for Projection
    /// </summary>
    [TestClass]
    public class Projection : TestBase
    {
        [TestMethod]
        public void SelectOnlyTwoFieldsOnBaseClass()
        {
            var query1 =
                (from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                 where person.Active == true && person.PersonItemList.Any(p => p.ItemId == 2)
                 select person);

            var list1 = query1.ToList();



            var query2 =
                (from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                 where person.Active == true && person.PersonItemList.Any(p => p.ItemId == 2)
                 select new { Name = person.Name, Email = person.Email });

            var list2 = query2.ToList();



            var query3 =
                (from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                 where person.Active == true && person.PersonItemList.Any(p => p.ItemId == 2)
                 select new TestClass { Name = person.Name, Email = person.Email });

            var list3 = query3.ToList();


            var query4 =
                (from item in Tenor.Linq.SearchOptions<Item>.CreateQuery()
                 where item.ItemId > 0
                 select new { ItemDescription = item.Description, ItemCategory = item.Category.Name });

            var list4 = query4.ToList();

            /*
            //next efforts:
            var query5 =
                (from item in Tenor.Linq.SearchOptions<Item>.CreateQuery()
                 where item.ItemId > 0
                 select new { ItemDescription = item.Description, ItemCategory = item.Category });

            var list5 = query5.ToList();
            */


        }

        [TestMethod]
        public void Grouping()
        {
            var query1 =
                (from item in Tenor.Linq.SearchOptions<Item>.CreateQuery()
                 group item by item.CategoryId into itemsGroup
                 select new { CategoryId = itemsGroup.Key, ItemCategory = itemsGroup.Count() });

            var list1 = query1.ToList();
        }

        private class TestClass
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
