using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Runtime.InteropServices;

namespace Tenor.Web
{
    public partial class TenorModule
    {

        private void Application_Error(object sender, EventArgs e)
        {
            try
            {
                HttpApplication app = (HttpApplication)sender;
                Exception exception = app.Server.GetLastError();
                if (exception != null)
                    Diagnostics.Debug.HandleError(app, exception, false);

            }
            catch (Exception)
            {
                //For god's sake, we cant have an exception on the exception handler!
                //Do nothing!
            }
        }


        /// <summary>
        /// Check if TenorModule is enabled on user-code webconfig.
        /// </summary>
        private static bool CheckHttpModules()
        {
            Type httpModule = typeof(TenorModule);
            HttpModulesSection sec = (HttpModulesSection)(WebConfigurationManager.GetSection("system.web/httpModules"));
            if (sec == null)
            {
                return false;
            }

            foreach (HttpModuleAction m in sec.Modules)
            {
                if (m.Type.StartsWith(httpModule.AssemblyQualifiedName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if TenorModule is enabled on user-code webconfig.
        /// </summary>
        /// <remarks></remarks>
        public static void CheckHttpModule()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context != null)
            {
                if (!CheckHttpModules())
                {
                    throw new ModuleNotFoundException();
                }
            }
        }

    }


    /// <summary>
    /// Occurs when the TenorModule is not defined on user-code webconfig.
    /// </summary>
    public class ModuleNotFoundException : TenorException
    {
        public override string Message
        {
            get
            {
                Type httpModule = typeof(TenorModule);
                return string.Format("You must have Web.TenorModule running to use this resource. Add a reference to httpModules section on your web.config file. <httpModules><add name=\"{0}\" type=\"{1}\"/></httpModules>", httpModule.Assembly.GetName().Name, httpModule.AssemblyQualifiedName);
            }
        }
    }
}