using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections;
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
    public class TestBase
    {

        //public string ConnectionString
        //{
        //    get
        //    {
        //        string value = ConfigurationManager.ConnectionStrings[0].ConnectionString;
        //        if (value.Contains("{0}"))
        //        {
        //            value = string.Format(value, Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent, "SampleApp" + Path.DirectorySeparatorChar + "App_Data"));
        //        }
        //        return value;
        //    }
        //}


        protected void ShouldListsBeEqual(IList listA, IList listB, bool checkOrder)
        {
            if (listA == null && listB == null)
                return;
            else if (listA != null && listB != null)
            {
                if (listA.Count != listB.Count)
                    Assert.Fail("First IList containing {0} elements and second IList containing {1} elements.", listA.Count, listB.Count);

                if (checkOrder)
                {
                    for (int i = 0; i < listA.Count; i++)
                        if (!object.Equals(listA[i], listB[i]))
                            Assert.Fail("Lists are different.");

                }
                else
                {
                    for (int i = 0; i < listA.Count; i++)
                        if (!listB.Contains(listA[i]))
                            Assert.Fail("Lists are different.");
                }
            }
            else
            {
                Assert.Fail("You passed just one list.");
            }

        }

    }
}
