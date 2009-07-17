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
    /// This control shows a bulleted list that can handle html code on items.
    /// </summary>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:BulletedList runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.BulletedList), "BulletedList.bmp")]
    public class BulletedList : System.Web.UI.WebControls.BulletedList
    {
        protected override void RenderBulletText(System.Web.UI.WebControls.ListItem item, int index, System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(item.Text);
        }
    }

}