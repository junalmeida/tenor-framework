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
    /// Painél que mantém a posição das scrolls
    /// </summary>
    /// <remarks></remarks>
    public class ScrollPanel : System.Web.UI.WebControls.Panel, IPostBackDataHandler
    {





        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);

            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsScrollPanel);


            Page.RegisterRequiresPostBack(this);
            Page.ClientScript.RegisterHiddenField(ClientID + "_value", lastValue);

            string script = "var ScrollPanel_" + ClientID + " = new ScrollPanel(\"" + ClientID + "\");" + "\r\n";
            Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID, script, true);
        }




        private string lastValue = string.Empty;
        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            lastValue = postCollection[ClientID + "_value"];
            return true;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {

        }
    }

}