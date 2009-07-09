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
        /// Verifica os HttpModules atuais na webconfig do projeto.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool CheckHttpModules()
        {
            //Dim httpModule As Type = GetType(HttpModule)
            HttpModulesSection sec = (HttpModulesSection)(WebConfigurationManager.GetSection("system.web/httpModules"));
            if (sec == null)
            {
                return false;
            }

            foreach (HttpModuleAction m in sec.Modules)
            {
                if (m.Type.StartsWith("Tenor.Web.HttpModule, Tenor"))
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