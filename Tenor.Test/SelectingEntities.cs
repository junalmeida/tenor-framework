using System.Linq;
using System;
using System.Collections.Generic;
using SampleApp.Business.Entities;
using Tenor.Data;
using Tenor.Linq;
#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
#endif

#if SQLITE
using DbInt = System.Int64;
using System.IO;

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
        public void SelectSingleOrDefault()
        {
            var person =
                (from p in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                 where p.PersonId == 1
                 select p).SingleOrDefault();

            Assert.IsNotNull(person);

            person =
                (from p in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                 where p.PersonId == 1
                 select p).Single();

            Assert.IsNotNull(person);

            try
            {
                person =
                    (from p in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                     where p.PersonId == -1
                     select p).Single();

            }
            catch (RecordNotFoundException)
            {
                // ok
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void SelectFirstOrDefault()
        {

            var person =
                (from p in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                 where p.PersonId > 0
                 select p).FirstOrDefault();

            Assert.IsNotNull(person);
        }

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
            DbInt id = 0;
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
            DbInt id = 0;
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


            DbInt id2 = 0;
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
        public void LinqSelectContainsIn()
        {
            var idArray = new DbInt[] { 1, 2, 3, 4, 5 };
            var idList = new List<DbInt>(idArray);

            var entities1 = (
                    from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                    where idArray.Contains(person.PersonId)
                    select person
                    ).ToArray();

            var entities2 = (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where idArray.Contains(person.PersonId)
                select person
                ).ToArray();

            CollectionAssert.AreEquivalent(entities1, entities2);
        }

        [TestMethod]
        public void LinqSelectContainsString()
        {
            string value = "a";
            var entities2 = (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where person.Name.Contains(value)
                select person
                ).ToArray();

            Assert.IsTrue(entities2.Length > 0);
        }



        [TestMethod]
        public void LinqSelectStartsWith()
        {
            string filterQuery = "j";

            //testing using linq lambda. 
            IQueryable<Person> so = Tenor.Linq.SearchOptions<Person>.CreateQuery();
            so = so.Where(p => p.Active && p.Email.StartsWith(filterQuery));
            //so = so.Where(p => p.DepartmentList.Any(e => e.Name == "blah"));
            //so = so.Where(p => p.DepartmentList.Any(e => e.Name == "bleh"));
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

            var valueInAEntity = new
            {
                Value = GetFilterTest(0, filterQuery)
            };

            //testing using linq query
            var query2 =
                (
                from person in Tenor.Linq.SearchOptions<Person>.CreateQuery()
                where person.Active && person.Email.StartsWith(valueInAEntity.Value)
                orderby person.Name, person.Active descending
                select person
                )
                .Distinct()
                .LoadAlso(p => p.DepartmentList);

            Person[] persons3 = query2.ToArray();


            //testing using classic way.
            SearchOptions search = new SearchOptions(typeof(Person));
            search.Conditions.Add(Person.Properties.Active, true);
            search.Conditions.And(Person.Properties.Email, filterQuery + "%", CompareOperator.Like);
            search.Sorting.Add(Person.Properties.Name);
            search.Sorting.Add(Person.Properties.Active, SortOrder.Descending);
            search.Distinct = true;
            search.LoadAlso("DepartmentList");

            Person[] persons4 = (Person[])search.Execute();

            CollectionAssert.AreEqual(persons2, persons);
            CollectionAssert.AreEqual(persons3, persons);
            CollectionAssert.AreEqual(persons4, persons);


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





        [TestMethod]
        public void SelectBinaryStream()
        {

            EntityBase.LastSearches.Clear();
            var p = new Person() { PersonId = 1 }; //without bind
            Assert.IsTrue(EntityBase.LastSearches.Count == 0);
            var stream = p.Photo;
            Assert.IsTrue(EntityBase.LastSearches.Count == 0);
            Assert.IsTrue(stream.Length == -1);


            var newStream = new MemoryStream();

            int bytesRead;
            byte[] buffer = new byte[1024 * 2];
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                    break;
                newStream.Write(buffer, 0, bytesRead);
            } while (true);

            Assert.IsTrue(EntityBase.LastSearches.Count == 1);


            EntityBase.LastSearches.Clear();
            p = new Person() { PersonId = 1 };
            p.Bind();

            var p3 = p.Photo3; //non-lazy binary

            Assert.IsTrue(EntityBase.LastSearches.Count == 1);
            stream = p.Photo;
            Assert.IsTrue(EntityBase.LastSearches.Count == 1);
            Assert.IsTrue(stream.Length > -1);


            newStream = new MemoryStream();
            buffer = new byte[1024 * 2];
            if (stream.Length > buffer.Length)
            {
                do
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead <= 0)
                        break;
                    newStream.Write(buffer, 0, bytesRead);
                } while (true);

                Assert.IsTrue(EntityBase.LastSearches.Count == 2);
            }
            else
                Assert.IsTrue(EntityBase.LastSearches.Count == 1);


            EntityBase.LastSearches.Clear();

            var lazyByte = p.Photo2;
            Assert.IsTrue(EntityBase.LastSearches.Count == 1);
        }
    }
}
