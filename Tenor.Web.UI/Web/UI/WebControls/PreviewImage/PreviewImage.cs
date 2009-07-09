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
    /// Contém uma lista de opções para exibição da animação de carga.
    /// </summary>
    /// <remarks></remarks>
    public enum PreviewImageLoadingAnimation
    {
        /// <summary>
        /// Não exibir a animação.
        /// </summary>
        /// <remarks></remarks>
        None,
        /// <summary>
        /// Animação padrão cinza.
        /// </summary>
        /// <remarks></remarks>
        Gray,
        /// <summary>
        /// Animação de cor branca no fundo preto.
        /// </summary>
        /// <remarks></remarks>
        WhiteOnBlack,
        /// <summary>
        /// Animação de cor preta no fundo branco.
        /// </summary>
        /// <remarks></remarks>
        BlackOnWhite,
        /// <summary>
        /// Animação Engrenagem no fundo branco.
        /// </summary>
        /// <remarks></remarks>
        GearsOnWhite
    }


    /// <summary>
    /// Exibe um controle de imagem com capacidade de ampliação sem popups.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:PreviewImage runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.Image), "Image.bmp")]
    public class PreviewImage : System.Web.UI.WebControls.Image
    {



        #region "Properties"

        /// <summary>
        /// Nível de transparencia da visualização
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// Nível de transparencia da visualização
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// Url da imagem ampliada a ser exibida na página.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// Indica qual animação de carga usar.
        /// O padrão é cinza.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// Indica o texto descritivo da imagem.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// Determina se deve ocultar as tags 'object' ao exibir a visualização da imagem.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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
                case PreviewImageLoadingAnimation.GearsOnWhite:
                    loadingimg = "\'" + Page.ClientScript.GetWebResourceUrl(typeof(PreviewImage), Configuration.Resources.PreviewGearGif) + "\'";
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
        /// Retorna a referencia, com javascript: no inicio, da chamada da visualização da imagem.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetOnClickClientHyperlink()
        {
            return "javascript:" + PreviewImage.GetOnClickClientScript(Page, this.FullSizedImageUrl, this.LoadingAnimation, this.Description, this.TransparencyLevel, this.TransparencyColor, HideObjects);
            //Return "javascript:PreviewImage_Open(null, '" & ResolveClientUrl(Me.FullSizedImageUrl) & "', " & GetDesc() & ", " & GetLoadImg(Page, LoadingAnimation) & ", " & GetClose() & ", " & (100 - TransparencyLevel) & ", '" & ColorTranslator.ToHtml(TransparencyColor) & "')"
        }

        /// <summary>
        /// Retorna a referencia, sem javascript: no inicio, da chamada da visualização da imagem.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetOnClickClientScript(Page Page, string ImageUrl, PreviewImageLoadingAnimation LoadingAnimation, string Description, int TransparencyLevel, System.Drawing.Color TransparencyColor, bool HideObjects)
        {
            RegisterIncludes(Page);
            return "PreviewImage_Open(null, \'" + Page.ResolveClientUrl(ImageUrl) + "\', " + GetDesc(Description) + ", " + GetLoadImg(Page, LoadingAnimation) + ", " + GetClose(Page) + ", " + (100 - TransparencyLevel) + ", \'" + ColorTranslator.ToHtml(TransparencyColor) + "\', " + HideObjects.ToString().ToLower() + ");";
        }

        public static string GetOnClickClientScript(Page Page, string ImageUrl, PreviewImageLoadingAnimation LoadingAnimation, string Description, int TransparencyLevel, System.Drawing.Color TransparencyColor)
        {
            return GetOnClickClientScript(Page, ImageUrl, LoadingAnimation, Description, TransparencyLevel, TransparencyColor, false);
        }


    }
}