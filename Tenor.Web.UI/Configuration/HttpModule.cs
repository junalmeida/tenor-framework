using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Web.Configuration;


namespace Tenor.Configuration
{
    internal class HttpModule
    {

        public const string HandlerFileName = "Tenor.axd";
        public const string IdPrefix = "__TENOR_";



        /// <summary>
        /// Check if TenorModule is defined on configuration file.
        /// </summary>
        /// <returns>True if the module is defined.
        /// </returns>
        private static bool CheckHttpModules()
        {
            HttpModulesSection sec = (HttpModulesSection)(WebConfigurationManager.GetSection("system.web/httpModules"));
            if (sec == null)
            {
                return false;
            }

            foreach (HttpModuleAction m in sec.Modules)
            {
                if (m.Type.StartsWith("Tenor.Web.TenorModule, Tenor"))
                {
                    return true;
                }
            }

            return false;
        }


        public static void CheckHttpModule()
		{
				System.Web.HttpContext Context = System.Web.HttpContext.Current;
				if (Context != null)
				{
					if (! CheckHttpModules())
					{
						throw (new Tenor.Web.ModuleNotFoundException());
					}
				}
				
        }
    }

}