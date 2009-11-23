using System;
using System.Data;
using System.Configuration;
using System.Web;
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