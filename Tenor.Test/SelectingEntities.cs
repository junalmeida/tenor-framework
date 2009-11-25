using System.Linq;
using System;
using System.Text;
using System.Collections.Generic;

using SampleApp.Business.Entities;
using Tenor.Data;
#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            IQueryable<Person> so = new Tenor.Linq.SearchOptions<Person>();

            so = so.Where(p => p.Active && (p.Email == null || !p.Email.Contains("@")));

            so = so.OrderBy(p => p.Name);

            so = so.OrderByDescending(p => p.Active);


            so = so.Distinct();

            //we cannot test this on sqlite.
            //so = so.Take(10);


            Person[] persons = so.ToArray();

            //int personCount = so.Count();
            //Assert.AreEqual(personCount, persons.Length);

            Person[] persons2 = so.ToArray();

            ShouldListsBeEqual(persons, persons2, false);

        }
    }
}
