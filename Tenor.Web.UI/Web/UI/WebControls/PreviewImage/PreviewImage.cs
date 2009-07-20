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


namespace Tenor.Web.UI.WebControls
{

    /// <summary>
    /// Defines how the behavior of the loading animation.
    /// </summary>
    public enum PreviewImageLoadingAnimation
    {
        /// <summary>
        /// No animation.
        /// </summary>
        None,
        /// <summary>
        /// The default gray animation.
        /// </summary>
        Gray,
        /// <summary>
        /// A white animation on black background.
        /// </summary>
        WhiteOnBlack,
        /// <summary>
        /// A black animation on white background.
        /// </summary>
        /// <remarks></remarks>
        BlackOnWhite
    }


    /// <summary>
    /// This control shows an image with zoom features without popups.
    /// </summary>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:PreviewImage runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.Image), "Image.bmp")]
    public class PreviewImage : System.Web.UI.WebControls.Image
    {



        #region "Properties"

        /// <summary>
        /// Gets or sets the transparency level of the viewport.
        /// </summary>
        [Themeable(true), DefaultValue(70), Category("Appearance"), Description("The transparency level. From 0 to 100 (percentage)")]
        public int TransparencyLevel
        {
            get
            {
                if (ViewState["TransparencyLevel"] == null)
                {
                    return 70;
                }
                else
                {
                    return System.Convert.ToInt32(ViewState["TransparencyLevel"]);
                }
            }
            set
            {
                if (value > 100 || value < 0)
                {
                    throw (new ArgumentOutOfRangeException("TransparencyLevel"));
                }
                ViewState["TransparencyLevel"] = value;
            }
        }


        /// <summary>
        /// Gets or sets the transparency color of the viewport.
        /// </summary>
        [Themeable(true), DefaultValue(typeof(Color), "Black"), Category("Appearance"), Description("The transparency color.")]
        public System.Drawing.Color TransparencyColor
        {
            get
            {
                if (ViewState["TransparencyColor"] == null)
                {
                    return System.Drawing.Color.Black;
                }
                else
                {
                    return ((System.Drawing.Color)(ViewState["TransparencyColor"]));
                }
            }
            set
            {
                ViewState["TransparencyColor"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the url of the original (full sized) image.
        /// </summary>
        [Themeable(true), UrlProperty("*.bmp;*.jpg;*.png;*.tif;*.gif"), Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), DefaultValue(""), Category("Appearance"), Description("The URL of a Full Sized image.")]
        public string FullSizedImageUrl
        {
            get
            {
                if (ViewState["FullSizedImageUrl"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["FullSizedImageUrl"].ToString();
                }
            }
            set
            {
                ViewState["FullSizedImageUrl"] = value;
            }
        }


        /// <summary>
        /// Gets or sets a value that specifies which loading animation will be shown.
        /// </summary>
        [Themeable(true), DefaultValue(typeof(PreviewImageLoadingAnimation), "Gray"), Category("Appearance"), Description("Specifies one of the LoadingAnimation to show.")]
        public PreviewImageLoadingAnimation LoadingAnimation
        {
            get
            {
                if (ViewState["LoadingAnimation"] == null)
                {
                    return PreviewImageLoadingAnimation.Gray;
                }
                else
                {
                    return ((PreviewImageLoadingAnimation)(ViewState["LoadingAnimation"]));
                }
            }
            set
            {
                ViewState["LoadingAnimation"] = value;
            }
        }


        /// <summary>
        /// Gets or sets a text of the image description.
        /// </summary>
        [Themeable(true), DefaultValue(""), Category("Appearance"), Description("The text to show below the image.")]
        public string Description
        {
            get
            {
                if (ViewState["Description"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return System.Convert.ToString(ViewState["Description"]);
                }
            }
            set
            {
                ViewState["Description"] = value;
            }
        }



        /// <summary>
        /// Gets or sets a value that determines whether to hide windowed controls when showing an image. 
        /// </summary>
        [Themeable(true), DefaultValue(false), Category("Behavior"), Description("Determines whether to hide \'object\' tags when showing image.")]
        public bool HideObjects
        {
            get
            {
                if (ViewState["HideObjects"] == null)
                {
                    return false;
                }
                else
                {
                    return System.Convert.ToBoolean(ViewState["HideObjects"]);
                }
            }
            set
            {
                ViewState["HideObjects"] = value;
            }
        }

        #endregion
        private static string GetDesc(string Description)
        {
            if (Description == string.Empty)
            {
                return "null";
            }
            else
            {
                return "\'" + Description.Replace("\r\n", "\\n").Replace("\'", "\\\'") + "\'";
            }
        }

        private static string GetClose(Page Page)
        {
            string closeimg = "null";
            closeimg = "\'" + Page.ClientScript.GetWebResourceUrl(typeof(PreviewImage), Configuration.Resources.PreviewPrevCloseGif) + "\'";
            return closeimg;
        }

        private static string GetLoadImg(Page Page, PreviewImageLoadingAnimation LoadingAnimation)
        {
            string loadingimg = "null";
            switch (LoadingAnimation)
            {
                case PreviewImageLoadingAnimation.None:
                    loadingimg = "false";
                    break;
                case PreviewImageLoadingAnimation.Gray:
                    loadingimg = "\'" + Page.ClientScript.GetWebResourceUrl(typeof(PreviewImage), Configuration.Resources.PreviewGrayGif) + "\'";
                    break;
                case PreviewImageLoadingAnimation.BlackOnWhite:
                    loadingimg = "\'" + Page.ClientScript.GetWebResourceUrl(typeof(PreviewImage), Configuration.Resources.PreviewBlackWhiteGif) + "\'";
                    break;
                case PreviewImageLoadingAnimation.WhiteOnBlack:
                    loadingimg = "\'" + Page.ClientScript.GetWebResourceUrl(typeof(PreviewImage), Configuration.Resources.PreviewWhiteBlackGif) + "\'";
                    break;
            }
            return loadingimg;
        }

        private static void RegisterIncludes(Page Page)
        {
            Page.ClientScript.RegisterClientScriptResource(typeof(System.Web.UI.WebControls.Image), "WebForms.js");
            Page.ClientScript.RegisterClientScriptResource(typeof(PreviewImage), Configuration.Resources.JsPreviewImage);

        }

        protected override void OnPreRender(System.EventArgs e)
        {
            RegisterIncludes(Page);
            string loadimg = GetLoadImg(Page, LoadingAnimation);
            if (loadimg != "false")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "PreviewImage_Preload(" + loadimg + ");", true);
            }

            base.OnPreRender(e);
        }


        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            writer.AddAttribute("onclick", PreviewImage.GetOnClickClientScript(Page, this.FullSizedImageUrl, this.LoadingAnimation, this.Description, this.TransparencyLevel, this.TransparencyColor, HideObjects));
            writer.AddStyleAttribute("cursor", "pointer");
        }

        /// <summary>
        /// Gets the statement (starting with javascript:) that can startup this control.
        /// </summary>
        public string GetOnClickClientHyperlink()
        {
            return "javascript:" + PreviewImage.GetOnClickClientScript(Page, this.FullSizedImageUrl, this.LoadingAnimation, this.Description, this.TransparencyLevel, this.TransparencyColor, HideObjects);
            //Return "javascript:PreviewImage_Open(null, '" & ResolveClientUrl(Me.FullSizedImageUrl) & "', " & GetDesc() & ", " & GetLoadImg(Page, LoadingAnimation) & ", " & GetClose() & ", " & (100 - TransparencyLevel) & ", '" & ColorTranslator.ToHtml(TransparencyColor) & "')"
        }

        /// <summary>
        /// Gets the statement (without javascript:) that can startup this control.
        /// </summary>
        public static string GetOnClickClientScript(Page page, string imageUrl, PreviewImageLoadingAnimation loadingAnimation, string description, int transparencyLevel, System.Drawing.Color transparencyColor, bool hideObjects)
        {
            RegisterIncludes(page);
            return "PreviewImage_Open(null, \'" + page.ResolveClientUrl(imageUrl) + "\', " + GetDesc(description) + ", " + GetLoadImg(page, loadingAnimation) + ", " + GetClose(page) + ", " + (100 - transparencyLevel) + ", \'" + ColorTranslator.ToHtml(transparencyColor) + "\', " + hideObjects.ToString().ToLower() + ");";
        }

        /// <summary>
        /// Gets the statement (without javascript:) that can startup this control.
        /// </summary>
        public static string GetOnClickClientScript(Page page, string imageUrl, PreviewImageLoadingAnimation loadingAnimation, string description, int transparencyLevel, System.Drawing.Color transparencyColor)
        {
            return GetOnClickClientScript(page, imageUrl, loadingAnimation, description, transparencyLevel, transparencyColor, false);
        }


    }
}