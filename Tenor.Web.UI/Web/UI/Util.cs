using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Web.Configuration;



namespace Tenor.Web
{

    /// <summary>
    /// Occurs when TenorModule is not defined on your configuraton file.
    /// </summary>
    public class ModuleNotFoundException : Exception
    {
        public override string Message
        {
            get
            {
                return "You must have Web.TenorModule running to use this resource. Add a reference to httpModules section on your web.config file. <httpModules><add name=\"Tenor\" type=\"Tenor.Web.TenorModule, Tenor\"/></httpModules>";
            }
        }
    }

}