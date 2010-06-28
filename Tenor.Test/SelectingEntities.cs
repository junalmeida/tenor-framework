using System.Linq;
using System;
using System.Text;
using System.Collections.Generic;

using SampleApp.Business.Entities;
using Tenor.Data;
using Tenor.Linq;
#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
#else
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
#endif

#if SQLITE
using DbInt = System.Int64;
#else
using DbInt = System.Int32;
#endif

namespace Tenor.Test
{
    /// <summary>
    /// Summary description for SelectingEntities
    /// </summary>
    [TestClass]
    public class SelectingEntities : TestBase
    {
        [TestMethod]
        public void SelectEverything()
        {
            Person[] persons = Person.Search(null, null);

            var query =
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                select person;

            Person[] persons2 = query.ToArray();

            CollectionAssert.AreEqual(persons, persons2);

        }

        [TestMethod]
        public void SelectWithConditions()
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add("Active", true);

            Person[] persons = Person.Search(cc, null);

            //lets use the same ConditionCollection again
            Person[] persons2 = Person.Search(cc, null);

            CollectionAssert.AreEqual(persons, persons2);
        }

        private string GetFilterTest()
        {
            return "j";
        }

        private string GetFilterTest(int a, string b)
        {
            return b;
        }

        [TestMethod]
        public void LinqSelectLogicalOperators()
        {
            long id = 0;
            var queryA =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where person.PersonId != null && person.MaritalStatus == MaritalStatus.Single
                select person
                ).ToList();

        }


        [TestMethod]
        public void LinqSelectComparisons()
        {
            long id = 0;
            var queryA =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where person.PersonId >= id
                orderby person.Name, person.Active descending
                select person
                ).ToList();


            
            var queryB =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where id <= person.PersonId
                orderby person.Name, person.Active descending
                select person
                ).ToList();


            CollectionAssert.AreEqual(queryA, queryB);


            int id2 = 0;
            queryA =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where person.PersonId >= id2
                orderby person.Name, person.Active descending
                select person
                ).ToList();



            queryB =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where id2 <= person.PersonId
                orderby person.Name, person.Active descending
                select person
                ).ToList();

            CollectionAssert.AreEqual(queryA, queryB);

            id2 = 2;
            queryA =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where person.PersonId == id2
                orderby person.Name, person.Active descending
                select person
                ).ToList();



            queryB =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where id2 == person.PersonId
                orderby person.Name, person.Active descending
                select person
                ).ToList();

            CollectionAssert.AreEqual(queryA, queryB);



            try
            {
                var query =
                    (
                    from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                    where person.ContractType == person.ContractType
                    orderby person.Name, person.Active descending
                    select person
                    ).ToList();

            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(NotImplementedException), "Comparing two fields is not yet implemented, but no or wrong exception was recieved.");
            }

        }

        


        [TestMethod]
        public void LinqSelectStartsWith()
        {
            string filterQuery = "j";

            //testing using linq lambda. 
            IQueryable<Person> so = Tenor.Linq.SearchOptions<Person>.CreateQuery();
            so = so.Where(p => p.Active && p.Email.StartsWith(filterQuery));
            //so = so.Where(p => p.DepartmentList.Any(e => e.Name == "blah"));
            //so = so.Where(p => p.DepartmentList.Any(e => e.Name == "blleh"));
            so = so.OrderBy(p => p.Name);
            so = so.OrderByDescending(p => p.Active);
            so = so.Distinct();
            //so = so.Take(10);
            so = so.LoadAlso(p => p.DepartmentList);

            Person[] persons = so.ToArray();

            //testing using linq query
            var query =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where person.Active && person.Email.StartsWith(GetFilterTest(0, filterQuery))
                orderby person.Name, person.Active descending
                select person
                )
                .Distinct()
                .LoadAlso(p => p.DepartmentList);

            Person[] persons2 = query.ToArray();


            //testing using classic way.
            SearchOptions search = new SearchOptions(typeof(Person));
            search.Conditions.Add(Person.Properties.Active, true);
            search.Conditions.And(Person.Properties.Email, filterQuery + "%", CompareOperator.Like);
            search.Sorting.Add(Person.Properties.Name);
            search.Sorting.Add(Person.Properties.Active, SortOrder.Descending);
            search.Distinct = true;
            search.LoadAlso("DepartmentList");

            Person[] persons3 = (Person[])search.Execute();

            CollectionAssert.AreEqual(persons2, persons);
            CollectionAssert.AreEqual(persons3, persons);


        }


        [TestMethod]
        public void Count()
        {
            DataTable countLowLevelDt = LowLevelExecuteQuery(@"
SELECT  COUNT(*) from (SELECT DISTINCT p.PersonId
 FROM Persons p
 LEFT OUTER JOIN person_department pd ON p.PersonId = pd.PersonId
 LEFT OUTER JOIN Departments d ON d.DepartmentId = pd.DepartmentId
 WHERE  (d.Name IS NOT NULL) 
 ) countQuery
");

            DbInt countLowLevel = (DbInt)countLowLevelDt[0][0];

            IQueryable<Person> so = Tenor.Linq.SearchOptions<Person>.CreateQuery();
            so = so.Where(p => p.DepartmentList.Any(e => e.Name != null));
            int count = so.Count();

            Assert.AreEqual(countLowLevel, count);
            if (countLowLevel <= 0)
                Assert.Fail("Invalid data.");
        }
    }
}
