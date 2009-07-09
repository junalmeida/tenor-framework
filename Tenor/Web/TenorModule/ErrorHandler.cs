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

                Diagnostics.Debug.HandleError(app, exception, false);

            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// Verifica os HttpModules atuais na webconfig do projeto.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
                if (m.Type.StartsWith(httpModule.FullName + ", " + httpModule.Assembly.GetName().Name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Verifica se o módulo está definido na Web.Config.
        /// </summary>
        /// <remarks></remarks>
        public static void CheckHttpModule()
        {
            System.Web.HttpContext Context = System.Web.HttpContext.Current;
            if (Context != null)
            {
                if (!CheckHttpModules())
                {
                    throw new ModuleNotFoundException();
                }
            }
        }

    }


    /// <summary>
    /// Exceção gerada quando o módulo não está devidamente configurado.
    /// </summary>
    /// <remarks></remarks>
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