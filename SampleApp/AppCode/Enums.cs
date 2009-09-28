using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum MaritalStatus
    {
        Single,
        Married,
        Separated,
        Divorced,
        Widowed,
        Engaged,
        Annulled,
        Cohabitating,
        Deceased
    }


    /// <summary>
    /// 
    /// </summary>
    public enum ContractType
    {
        [EnumDatabaseValue("E")]
        Employee,
        [EnumDatabaseValue("S")]
        Outsourced,
        [EnumDatabaseValue("O")]
        Other
    }
}