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
    }
}
