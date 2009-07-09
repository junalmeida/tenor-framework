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
    /// Hyperlink dinâmico com Hover.
    /// </summary>
    /// <remarks></remarks>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:HoverLink runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.ImageButton), "ImageButton.bmp"), DefaultEvent("Click")]
    public class HoverLink : System.Web.UI.WebControls.HyperLink, IPostBackEventHandler
    {




        private EventHandler ClickEvent;
        public event EventHandler Click
        {
            add
            {
                ClickEvent = (EventHandler)System.Delegate.Combine(ClickEvent, value);
            }
            remove
            {
                ClickEvent = (EventHandler)System.Delegate.Remove(ClickEvent, value);
            }
        }




        [Themeable(true), DefaultValue(true), Category("Behavior"), Description("Indicates whether to preload images on page load")]
        public bool PreloadImages
        {
            get
            {
                if (ViewState["PreloadImage"] == null)
                {
                    return true;
                }
                else
                {
                    return System.Convert.ToBoolean(ViewState["PreloadImage"]);
                }
            }
            set
            {
                ViewState["PreloadImage"] = value;
            }
        }



        [Themeable(true), UrlProperty("*.bmp;*.gif.*.jpg;*.png"), Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), DefaultValue(""), Category("Appearance"), Description("The URL of the image to be shown when mouse hovers.")]
        public string HoverImageUrl
        {
            get
            {
                if (ViewState["HoverImageUrl"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["HoverImageUrl"].ToString();
                }
            }
            set
            {
                ViewState["HoverImageUrl"] = value;
            }
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);
            if (!this.DesignMode)
            {
                if (PreloadImages && !string.IsNullOrEmpty(HoverImageUrl))
                {
                    Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsPreviewImage);
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "PreviewImage_Preload(\'" + ResolveClientUrl(HoverImageUrl) + "\');", true);
                }

            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.DesignMode)
            {
                base.Render(writer);
            }
            else
            {
                bool resetnavigate = false;

                if (string.IsNullOrEmpty(this.NavigateUrl))
                {
                    resetnavigate = true;
                    this.NavigateUrl = "javascript:" + Page.ClientScript.GetPostBackEventReference(this, "");
                    //writer.AddAttribute("onclick", Page.ClientScript.GetPostBackEventReference(Me, ""))
                }
                if (!string.IsNullOrEmpty(ImageUrl) && !string.IsNullOrEmpty(HoverImageUrl))
                {
                    writer.AddAttribute("onmouseover", ("this.firstChild.src=\'" + ResolveUrl(HoverImageUrl) + "\';"));
                    writer.AddAttribute("onmouseout", ("this.firstChild.src=\'" + ResolveUrl(ImageUrl) + "\';"));
                }

                base.Render(writer);

                if (resetnavigate)
                {
                    this.NavigateUrl = string.Empty;
                }

            }
        }


        protected virtual void OnClick(EventArgs e)
        {
            if (ClickEvent != null)
                ClickEvent(this, e);
        }


        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            OnClick(new EventArgs());
        }
    }
}
