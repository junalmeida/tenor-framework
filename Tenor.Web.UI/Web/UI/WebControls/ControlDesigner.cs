using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Reflection;
using System.ComponentModel;


namespace Tenor.Web.UI.WebControls.Design
{
    [ToolboxItem(false)]
    public class ControlDesigner : System.Web.UI.Design.ControlDesigner
    {


        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            //Dim webApp As IWebApplication = CType(GetService(GetType(IWebApplication)), IWebApplication)
            //Util.CheckWebConfig(webApp)
        }

    }

}