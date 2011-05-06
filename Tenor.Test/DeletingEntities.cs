
using Tenor.Data;
using SampleApp.Business.Entities;


#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SQLite;
#else
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using Assert = NUnit.Framework.Assert;
#endif


namespace Tenor.Test
{
    /// <summary>
    /// Summary description for DeletingEntities
    /// </summary>
    [TestClass]
    public class DeletingEntities : TestBase
    {
        /// <summary>
        /// Deletes one entity.
        /// </summary>
        [TestMethod()]
        public void DeleteOne()
        {
            //Create a person, then deletes it

            Person p = new Person();
            p.Active = true;
            p.Name = "Test1";
            p.Save();

            p.Delete();
        }

        /// <summary>
        /// Deletes several records based on a criteria.
        /// </summary>
        [TestMethod()]
        public void DeleteSeveral()
        {
            string[] people = new string[] { "Test1", "Test2", "Test3", "Test4" };

            foreach (string person in people)
            {
                Person p = new Person();
                p.Active = true;
                p.Name = person;
                p.Save();

            }

            ConditionCollection cc = new ConditionCollection();
            cc.Add("Name", "Test%", CompareOperator.Like);

            EntityBase.Delete(typeof(Person), cc);

            Person[] dbPeople = Person.Search(cc, null);
            if (dbPeople.Length > 0)
            {
                Assert.Fail("Instances were not deleted.");
            }
        }

        /// <summary>
        /// Deletes an entity infringing its foreign key.
        /// </summary>
        [TestMethod()]
        public void DeleteInfringingFK()
        {
            // Creates a category
            Category cat = new Category();
            cat.Name = "cat";
            cat.Save(false);

            Item item = new Item();
            item.Description = "an item";
            item.CategoryId = cat.CategoryId;
            item.Save(false);

            try
            {
                cat.Delete();
            }
            catch (SQLiteException ex)
            {
                if (ex.ErrorCode == SQLiteErrorCode.Constraint)
                    return;
            }

            Assert.Fail();
        }

    }
}
