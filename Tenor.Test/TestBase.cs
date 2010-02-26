﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections;
using Tenor.Data;
#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
#else
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using Assert = NUnit.Framework.Assert;
#endif

namespace Tenor.Test
{
    public abstract class TestBase
    {

        public TestBase()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        [TestInitialize()]
        public void TestInitialize()
        {
            string cn = ConnectionString;
#if SQLITE
            Tenor.BLL.BLLBase.SystemConnection = new ConnectionStringSettings(Tenor.BLL.BLLBase.SystemConnection.Name, cn, Tenor.BLL.BLLBase.SystemConnection.ProviderName);
#endif
            System.Diagnostics.Trace.WriteLine("Initializing tests. We will recreate the database.", TestContext.TestName);
            System.Diagnostics.Trace.WriteLine(string.Format("Connection: {0}", cn), TestContext.TestName);

#if SQLITE
            const string dbType = "sqlite";
#elif MSSQL
            const string dbType = "mssql";
#elif ORACLE
            const string dbType = "oracle";
#elif MYSQL
            const string dbType = "mysql";
#elif POSTGRES
            const string dbType = "postgres";
#endif

            Stream stream = this.GetType().Assembly.GetManifestResourceStream(string.Format("Tenor.Test.Scripts.tables.{0}.sql", dbType));
            using (StreamReader reader = new StreamReader(stream))
            {
                LowLevelExecuteNonQuery(reader.ReadToEnd());
            }

            System.Diagnostics.Trace.WriteLine("The database was reset.", TestContext.TestName);
            System.Diagnostics.Trace.WriteLine("Executing test", TestContext.TestName);
            System.Diagnostics.Trace.Indent();
        }

        [TestCleanup()]
        public void TestFinalize()
        {
            System.Diagnostics.Trace.Unindent();
            System.Diagnostics.Trace.WriteLine("Test done.", TestContext.TestName);
        }

        public string ConnectionString
        {
            get
            {
                string value = Tenor.BLL.BLLBase.SystemConnection.ConnectionString;
                if (value.Contains("{0}"))
                {
                    value = string.Format(value, new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName);
                    value = value.Replace("/", Path.DirectorySeparatorChar.ToString());
                }
                return value;
            }
        }


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

        protected void LowLevelExecuteNonQuery(string query, params DbParameter[] parameters)
        {
            DbProviderFactory fac = DbProviderFactories.GetFactory(Tenor.BLL.BLLBase.SystemConnection.ProviderName);
            using (DbConnection con = fac.CreateConnection())
            {
                con.ConnectionString = ConnectionString;
                try
                {
                    con.Open();

                    foreach (string q in query.Split(new string[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
#if MSSQL
                        if (q.Contains("DROP DATABASE"))
                            con.ChangeDatabase("master");
#endif
                        using (DbCommand cmd = con.CreateCommand())
                        {
                            cmd.CommandText = q;
                            if (parameters != null && parameters.Length > 0)
                                cmd.Parameters.AddRange(parameters);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                finally
                {
                    if (con.State != System.Data.ConnectionState.Closed)
                        con.Close();
                }
            }

        }

        protected DataTable LowLevelExecuteQuery(string query, params DbParameter[] parameters)
        {
            DbProviderFactory fac = DbProviderFactories.GetFactory(Tenor.BLL.BLLBase.SystemConnection.ProviderName);
            DbConnection con = fac.CreateConnection();
            con.ConnectionString = ConnectionString;
            try
            {
                con.Open();
                DbCommand cmd = con.CreateCommand();
                cmd.CommandText = query;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                DbDataAdapter adapt = fac.CreateDataAdapter();
                adapt.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapt.Fill(dt);
                return dt;
            }
            finally
            {
                if (con.State != System.Data.ConnectionState.Closed)
                    con.Close();
            }
        }
    }
}
