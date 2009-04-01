using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SampleApp.Business.Entities
{
    /// <summary>
    /// Summary description for Enums
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
}