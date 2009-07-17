using System.Diagnostics;
using System.Data;
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



namespace Tenor.Web.UI.WebControls
{


    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public class ScriptCollection : CollectionBase
    {



        internal ScriptCollection()
        {
        }

        #region " IList Members"

        public int Add(Script value)
        {
            return List.Add(value);
        }

        public void AddRange(ICollection c)
        {
            foreach (object i in c)
            {
                Add((Script)i);
            }
        }

        public bool Contains(Script value)
        {
            return List.Contains(value);
        }

        public int IndexOf(Script value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, Script value)
        {
            List.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get
            {
                return List.IsFixedSize;
            }
        }


        public bool IsReadOnly
        {
            get
            {
                return List.IsReadOnly;
            }
        }

        public void Remove(Script value)
        {
            List.Remove(value);
        }

        public Script this[int Index]
        {
            get
            {
                return ((Script)(List[Index]));
            }
            set
            {
                List[Index] = value;
            }
        }


        #endregion
    }

    /// <summary>
    /// Base class that represents a script that will be rendered on the page.
    /// </summary>
    [TypeConverter(typeof(Design.ScriptTypeConverter))]
    public abstract class Script : object
    {


        protected Script()
        {
        }

        public abstract void Initialize(Page Page);


        public override string ToString()
        {
            return this.GetType().Name;
        }
    }



    /// <summary>
    /// Register mask scripts.
    /// </summary>
    public sealed class ScriptMasks : Script
    {
        public ScriptMasks()
        {
        }

        public override void Initialize(Page Page)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsMasks);
        }
    }

    /// <summary>
    /// Register a script that can block text selection on browser.
    /// </summary>
    public sealed class ScriptBlockSelection : Script
    {

        public ScriptBlockSelection()
        {
        }

        public override void Initialize(Page Page)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsSelect);

        }
    }

    /// <summary>
    /// Register a script that can block a right click on browser.
    /// </summary>
    public sealed class ScriptBlockRightClick : Script
    {
        public ScriptBlockRightClick()
        {
        }

        public override void Initialize(Page Page)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsRightClick);

        }
    }


    /// <summary>
    /// Adds a script with a set of bugfixes on IE version lower than 7.
    /// </summary>
    public sealed class IEFix : Script
    {
        public IEFix()
        {
        }

        public override void Initialize(Page Page)
        {

            if (Page.Header == null)
            {
                throw (new InvalidOperationException("Header tag must be a server control."));
            }

            Literal script = new Literal();
            script.Text = Environment.NewLine + "<!--[if lt IE 7]>";
            script.Text += Environment.NewLine + "<script src=\"" + Page.ResolveUrl("~/" + Tenor.Configuration.HttpModule.HandlerFileName) + "/iefix/" + "ie7-standard-p.js" + "\" type=\"text/javascript\"></script>";
            script.Text += Environment.NewLine + "<![endif]-->";

            /*
            script.Text = Environment.NewLine + "<!--[if lt IE 7]>"
            script.Text += Environment.NewLine + "<script src=""" + Page.ResolveUrl("~/" + Util.HandlerFileName) + "/iefix/IE7.js" + """ type=""text/javascript""></script>"
            script.Text += Environment.NewLine + "<![endif]-->"
            */
            Page.Header.Controls.Add(script);
        }
    }

}