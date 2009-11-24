using System;
using System.Text;
using System.Collections.Generic;

#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp.Business.Entities;
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
    public class SelectingEntities
    {
        [TestMethod]
        public void SelectEverything()
        {
            Person[] persons = Person.Search(null, null);
           
        }
    }
}
