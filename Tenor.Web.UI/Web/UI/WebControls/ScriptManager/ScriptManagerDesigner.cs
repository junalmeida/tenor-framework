using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Reflection;
using System.ComponentModel;


namespace Tenor.Web.UI.WebControls.Design
{

    [ToolboxItem(false)]
    public class ScriptManagerDesigner : Design.ControlDesigner
    {


        public override string GetDesignTimeHtml()
        {
            return this.CreatePlaceHolderDesignTimeHtml();
        }


    }
}
