using System;
using System.Text;
using System.Collections.Generic;
using SampleApp.Business.Entities;
using Tenor.BLL;
using Tenor.Data;
#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using Assert = NUnit.Framework.Assert;
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


        [TestMethod]
        public void SaveManyToManyList()
        {

            //Loads a person and adds 3 departments to its N:N relation.
            Person p = new Person(2);
            p.Name += " Changed";

            p.DepartmentList.Clear();

            int[] departments = new int[] { 1, 3, 4 };

            foreach (int id in departments)
            {
                Department d = new Department();
                d.DepartmentId = id;
                p.DepartmentList.Add(d);
            }


            Transaction t = new Transaction();
            try
            {
                t.Include(p);
                p.Save(true);
                p.SaveList("DepartmentList");
                t.Commit();
            }
            catch (Exception ex)
            {
                t.Rollback();
                Assert.Fail("Cannot save person. " + ex.Message);
            }

            //Check if person 2 have the departments 1, 3 and 4
            p = new Person(2);

            foreach (int id in departments)
            {
                Department d = new Department();
                d.DepartmentId = id;
                if (!p.DepartmentList.Contains(d))
                    Assert.Fail(string.Format("Department {0} was not associated with person 2. ", id));
            }

        }

    }
}
