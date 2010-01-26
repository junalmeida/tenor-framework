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
           
        }

        [TestMethod]
        public void SelectWithConditions()
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add("Active", true);

            Person[] persons = Person.Search(cc, null);

            //lets use the same ConditionCollection again
            Person[] persons2 = Person.Search(cc, null);

            ShouldListsBeEqual(persons, persons2, false);
        }


        [TestMethod]
        public void LinqSelectWithConditions()
        {
            IQueryable<Person> so = Tenor.Linq.SearchOptions<Person>.CreateQuery();

            
            so = so.Where(p => p.Active && p.Email.StartsWith("j"));
            //so = so.Where(p => p.DepartmentList.Any(e => e.Name == "blah"));
            //so = so.Where(p => p.DepartmentList.Any(e => e.Name == "blleh"));

            so = so.OrderBy(p => p.Name);

            so = so.OrderByDescending(p => p.Active);
            
            so = so.Distinct();
            //so = so.Take(10);
            so = so.LoadAlso(p => p.DepartmentList);


            Person[] persons = so.ToArray();

            //int personCount = so.Count();
            //Assert.AreEqual(personCount, persons.Length);

            //so = Tenor.Linq.SearchOptions<Person>.CreateQuery();
            //Person[] persons2 = (from p in so
            //                     where p.Active && p.Email.StartsWith("j")
            //                     select p).ToArray();

            //ShouldListsBeEqual(persons, persons2, false);

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
            int countLowLevel = (int)countLowLevelDt[0][0];

            IQueryable<Person> so = Tenor.Linq.SearchOptions<Person>.CreateQuery();
            so = so.Where(p => p.DepartmentList.Any(e => e.Name != null));
            int count = so.Count();

            Assert.AreEqual(countLowLevel, count);
        }
    }
}
