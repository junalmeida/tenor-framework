using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;



namespace Tenor.Web.UI.WebControls
{

    /// <summary>
    /// This control show a floating side-panel.
    /// </summary>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:FloatingPanel runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.Panel), "Panel.bmp")]
    public class FloatingPanel : System.Web.UI.WebControls.Panel
    {

        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);

            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsFloatingPanel);
            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "initFloater(\"" + this.ClientID + "\");" + "\r\n", true);
        }

    }
}