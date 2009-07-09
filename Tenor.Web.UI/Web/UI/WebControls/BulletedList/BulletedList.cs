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
    /// Exibe uma lista com marcadores que aceita código html em seus itens.
    /// </summary>
    /// <remarks></remarks>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:BulletedList runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.BulletedList), "BulletedList.bmp")]
    public class BulletedList : System.Web.UI.WebControls.BulletedList
    {



        //Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        //    Dim sb As New StringBuilder()
        //    Dim sw As New StringWriter(sb)
        //    Dim htmlWriter As New HtmlTextWriter(sw)
        //    Dim strRendered As String

        //    MyBase.Render(htmlWriter)
        //    strRendered = Regex.Replace(sb.ToString(), "(?<!&lt;)&lt;(?!&lt;)", "<")
        //    strRendered = Regex.Replace(strRendered, "(?<!&gt;)&gt;(?!&gt;)", ">")
        //    strRendered = Regex.Replace(strRendered, "(?<!&amp;)&amp;(?!&amp;)", "&")
        //    strRendered = Regex.Replace(strRendered, "(?<!&quot;)&quot;(?!&quot;)", """")

        //    strRendered = strRendered.Replace("&lt;&lt;", "&lt;").Replace("&gt;&gt;", "&gt;").Replace("&quot;&quot;", "&quot;")
        //    writer.Write(strRendered)
        //End Sub


        protected override void RenderBulletText(System.Web.UI.WebControls.ListItem item, int index, System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(item.Text);
        }
    }

}