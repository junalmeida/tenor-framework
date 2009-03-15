using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TenorTemplate
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            throw e.Exception;
        }


        public class DriverData
        {
            public DriverData(MyMeta.dbDriver driver, string name, string conn)
            {
                this.driver = driver;
                this.name = name;
                this.conn = conn;
            }
            public MyMeta.dbDriver driver;
            public string name;
            public string conn;

            public override string ToString()
            {
                return name;
            }
        }
        public static DriverData[] CustomConnections
        {
            get
            {
                List<DriverData> list = new List<DriverData>();
                list.Add(new DriverData(MyMeta.dbDriver.SQL, "Microsoft Sql Server", "Provider=SQLNCLI.1;Data Source={0};Initial Catalog={1};User Id={2};Password={3};"));
                /*
                list.Add(MyMeta.dbDriver.Oracle, "Oracle");
                list.Add(MyMeta.dbDriver.MySql2, "MySql");
                list.Add(MyMeta.dbDriver.PostgreSQL8, "PostgreSql");
                list.Add(MyMeta.dbDriver.Firebird, "Firebird");
                list.Add(MyMeta.dbDriver.Interbase, "Interbase");
                list.Add(MyMeta.dbDriver.SQLite, "SQLite");
                list.Add(MyMeta.dbDriver.Access, "Microsoft Access");
                */
                return list.ToArray();
            }
        }
    }
}
