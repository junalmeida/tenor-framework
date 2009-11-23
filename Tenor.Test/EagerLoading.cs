using System;
using System.Text;
using System.Collections.Generic;
using Tenor.Data;
using SampleApp.Business.Entities;

#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
#endif

namespace Tenor.Test
{
    /// <summary>
    /// Summary description for EagerLoading
    /// </summary>
    [TestClass]
    public class EagerLoading : TestBase
    {
        [TestMethod]
        public void EagerLoadingTest()
        {
      
            SearchOptions so = new SearchOptions(typeof(Item));
            so.Conditions.Add("Name", "First category", "c");
            so.Conditions.Include("Category", "c");
            so.LoadAlso("PersonItemList");
            so.LoadAlso("Category");
            Item[] items = (Item[])so.Execute();
            //provide a way to check if it was loaded on eager mode.


            /*
            //Loads a person and adds 3 departments to its N:N relation.
            Person p = new Person(2);
            p.Name += "Changed";

            foreach (Department depto in p.DepartmentList)
            {
                //blah
            }
            p.DepartmentList.Clear();

            Department d = new Department();
            d.DepartmentId = 1;
            p.DepartmentList.Add(d);

            d = new Department();
            d.DepartmentId = 3;
            p.DepartmentList.Add(d);

            d = new Department();
            d.DepartmentId = 4;
            p.DepartmentList.Add(d);


            Transaction t = new Transaction();
            try
            {
                t.Include(p);
                p.Save(true);
                p.SaveList("DepartmentList");
            }
            catch
            {
                t.Rollback();
                return;
            }
            t.Commit();
            */
 


        }
    }
}
