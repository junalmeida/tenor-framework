using System;
using System.Text;
using System.Collections.Generic;

#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp.Business.Entities;
using Tenor.BLL;
#else
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
#endif

namespace Tenor.Test
{
    /// <summary>
    /// Summary description for SavingEntities
    /// </summary>
    [TestClass]
    public class SavingEntities
    {
        public static string RandomString(int size, bool lowerCase)
        {
            Random randomSeed = new Random();

            StringBuilder randStr = new StringBuilder(size);
            // Ascii start position (65 = A / 97 = a)
            int Start = (lowerCase) ? 97 : 65;

            // Add random chars
            for (int i = 0; i < size; i++)
                randStr.Append((char)(26 * randomSeed.NextDouble() + Start));

            return randStr.ToString();
        }

        public static Person CreateNewPerson()
        {   
            Person p = new Person();
            p.Active = true;
            p.Name = "Test " + RandomString(8, true);
            p.Save();
            return p;        
        }

        [TestMethod]
        public void CreateNewEntity()
        {
            Person p = CreateNewPerson();

            p = new Person(p.PersonId);

        }

    }
}
