using System;
using System.Data;
using System.Configuration;
using System.Web;
using Tenor.Data;
namespace SampleApp.Business.Entities
{
    public partial class Department
    {
        public static Department[] List()
        {
            SortingCollection sc = new SortingCollection();
            sc.Add(Department.Properties.Name);

            return Department.Search(null, sc);
        }
    }
}