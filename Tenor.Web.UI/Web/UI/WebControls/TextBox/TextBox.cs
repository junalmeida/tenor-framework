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


    #region " ENUMs "

    /// <summary>
    /// Contains a list of possible styles for calendar button
    /// </summary>
    /// <remarks></remarks>
    public enum TextBoxCalendarButton
    {
        /// <summary>
        /// No button is shown
        /// </summary>
        None,
        /// <summary>
        /// A XP-like button is shown
        /// </summary>
        XPStyle
        /*
            /// <summary>
            /// A Win2000-like button is shown
            /// </summary>
            W2KStyle
         */
    }


    /// <summary>
    /// Contains a list of possible behaviors of a TextBox
    /// </summary>
    public enum TextBoxMode
    {
        /// <summary>
        /// Displays a SingleLine textbox for text input
        /// </summary>
        SingleLine,
        /// <summary>
        /// Displays a password field
        /// </summary>
        Password,
        /// <summary>
        /// Displays a textbox with a "CPF" mask
        /// </summary>
        CPF,
        /// <summary>
        /// Displays a textbox with a "CNPJ" mask
        /// </summary>
        CNPJ,
        /// <summary>
        /// Displays a textbox with a "CEP" mask
        /// </summary>
        CEP,
        /// <summary>
        /// Displays a textbox with PhoneNumber input
        /// </summary>
        PhoneNumber,
        /// <summary>
        /// Displays a textbox with PhoneNumber and AreaCode input
        /// </summary>
        PhoneNumberDDD,
        /// <summary>
        /// Displays a textbox with a "PABX" mask
        /// </summary>
        PABX,
        /// <summary>
        /// Displays a textbox with email input
        /// </summary>
        Email,
        /// <summary>
        /// Displays a textbox with Date (DD/MM/YYYY) input
        /// </summary>
        @DateDMY,
        /// <summary>
        /// Displays a textbox with Time (HH:mm) input
        /// </summary>
        TimeHM,
        /// <summary>
        /// Displays a textbox with Time (HH:mm:ss) input
        /// </summary>
        TimeHMS,
        /// <summary>
        /// Displays a textbox with an integer input
        /// </summary>
        @Integer,
        /// <summary>
        /// Displays a textbox with an integer input between 0 and 100
        /// </summary>
        Percent,
        /// <summary>
        /// Displays a textbox with a float input
        /// </summary>
        @Float,
        /// <summary>
        /// Displays a textbox with a currency input (2 decimal places, right aligned)
        /// </summary>
        @Currency,
        /// <summary>
        /// Displays a multiline textbox
        /// </summary>
        MultiLine,
        /// <summary>
        /// Displays a multiline richtext textbox
        /// </summary>
        RichText,
        /// <summary>
        /// Displays a textbox with a CustomMask
        /// </summary>
        CustomMask
    }
    #endregion

    /*
    Designer(GetType(Design.ControlDesigner)), _
     */
    /// <summary>
    /// This control is a customized textbox with a set of features described at <see cref="TextBox.TextBoxMode"/> property.
    /// </summary>
    [ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:TextBox runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.TextBox), "TextBox.bmp")]
    public class TextBox : System.Web.UI.WebControls.TextBox
    {



        #region "Properties "
        //Disabled the base TextMode.
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override System.Web.UI.WebControls.TextBoxMode TextMode
        {
            get
            {
                return base.TextMode;
            }
            set
            {
                throw new NotSupportedException("Use TextBoxMode instead.");
            }
        }


        /// <summary>
        /// Gets or sets a value that determines the behavior mode of the TextBox.
        /// </summary>
        /// <value>One of <see cref="Tenor.Web.UI.WebControls.TextBoxMode"/> values.</value>
        [Description("The behavior mode of the TextBox"), Category("Behavior"), DefaultValue(typeof(TextBoxMode), "SingleLine")]
        public TextBoxMode TextBoxMode
        {
            get
            {
                if (ViewState["TextBoxMode"] == null)
                {
                    return TextBoxMode.SingleLine;
                }
                return ((TextBoxMode)(ViewState["TextBoxMode"]));
            }
            set
            {
                ViewState["TextBoxMode"] = value;
                switch (value)
                {
                    case TextBoxMode.Password:
                        base.TextMode = System.Web.UI.WebControls.TextBoxMode.Password;
                        break;
                    case TextBoxMode.MultiLine:
                        base.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
                        break;
                    case WebControls.TextBoxMode.RichText:
                        base.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
                        break;
                    /*
                If DesignMode AndAlso Site IsNot Nothing Then
                    Dim webApp As System.Web.UI.Design.IWebApplication = CType(Site.GetService(GetType(System.Web.UI.Design.IWebApplication)), System.Web.UI.Design.IWebApplication)
                    Util.CheckWebConfig(webApp)
                End If
                     */
                    default:
                        base.TextMode = System.Web.UI.WebControls.TextBoxMode.SingleLine;
                        break;
                }
            }
        }



        /// <summary>
        /// Gets or sets the mask to apply when Mask is set to Custom.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>
        /// Custom Mask Formats
        /// <list>
        /// <item># - digit from 0 to 9</item>
        /// </list>
        /// </remarks>
        [DefaultValue(""), Description("Defines the mask to apply when Mask is set to Custom."), Category("Behavior")]
        public string CustomMask
        {
            get
            {
                return ViewState["CustomMask"].ToString();
            }
            set
            {
                ViewState["CustomMask"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the regular expression validation rule to apply when Mask is set to Custom.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DefaultValue(""), Description("Defines the regular expression validation rule to apply when Mask is set to Custom."), Category("Behavior")]
        public string CustomValidation
        {
            get
            {
                return ViewState["CustomValidation"].ToString();
            }
            set
            {
                ViewState["CustomValidation"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the error message to display in case of unsuccesful validation.
        /// </summary>
        [DefaultValue("Defines the error message to display in case of unsuccesful validation."), Description("Gets or sets the error message to display in case of unsuccesful validation."), Category("Behavior")]
        public string ErrorMessage
        {
            get
            {
                return ViewState["ErrorMessage"].ToString();
            }
            set
            {
                ViewState["ErrorMessage"] = value;
            }
        }


        /// <summary>
        /// Gets or sets a value that determines how the calendar is shown when this control behaves like a DateTime box.
        /// </summary>
        /// <value>One of the <see cref="TextBoxCalendarButton"/> values.</value>
        [Themeable(true), DefaultValue(typeof(TextBoxCalendarButton), "None"), Description("Indicates whether to show a glyph with a calendar pane. This property is valid only if TextBoxMode is set to any Date format."), Category("Appearance")]
        public TextBoxCalendarButton ShowCalendar
        {
            get
            {
                if (ViewState["ShowCalendar"] == null)
                {
                    return TextBoxCalendarButton.None;
                }
                else
                {
                    return ((TextBoxCalendarButton)(ViewState["ShowCalendar"]));
                }
            }
            set
            {
                ViewState["ShowCalendar"] = value;
            }
        }


        #endregion


        protected override void OnLoad(System.EventArgs e)
        {
            if (!DesignMode)
            {
                ScriptMasks mask = new ScriptMasks();
                //Registers the main js mask code.
                mask.Initialize(this.Page);


            }
            base.OnLoad(e);
        }

        #region "Masks "


        private void RegisterMaskScriptVersion1()
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsMasks);
        }

        private void RegisterMaskScriptVersion2()
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsJQuery);
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsMasks2);
        }

        #endregion

        protected override void OnPreRender(System.EventArgs e)
        {

            //Checks if we fave TinyMce only when TextBoxMode is set to RichText
            switch (TextBoxMode)
            {
                case WebControls.TextBoxMode.RichText:
                    //System.Reflection.Assembly tinymce = 
				    System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(Configuration.Resources.AssemblyTinyMCE));
                    Configuration.HttpModule.CheckHttpModule();
                    break;
                /*
                 MaskScriptVersion1()
                 SetMaskVersion1()
             Case WebControls.TextBoxMode.Integer, _
                  WebControls.TextBoxMode.Float, _
                  WebControls.TextBoxMode.Currency, _
                  WebControls.TextBoxMode.Percent, _
                  WebControls.TextBoxMode.Email

                 MaskScriptVersion1()
                 SetMaskVersion1()
                 */
                case WebControls.TextBoxMode.DateDMY:
                    /*
                    Page.ClientScript.RegisterClientScriptResource(GetType(System.Web.UI.WebControls.Image), "WebForms.js")
                     */
                    if (this.ShowCalendar != TextBoxCalendarButton.None)
                    {
                        RegisterCalendarScripts();
                    }
                    break;
                /*
                    MaskScriptVersion2()
                    SetMaskVersion2()

                Case Else
                    MaskScriptVersion2()
                    SetMaskVersion2()
                     */
            }
            //Executes a refresh to fix up MyBase.TextMode.
            TextBoxMode = TextBoxMode;
            SetMaskVersion1();


            base.OnPreRender(e);
        }


        /// <summary>
        /// Sets the mask based on textboxmode.
        /// </summary>
        private void SetMaskVersion2()
        {
            //Executes a refresh to fix up MyBase.TextMode.
            TextBoxMode = TextBoxMode;

            string mask = string.Empty;
            string validation = "";
            string placeholder = "_";

            if ((TextBoxMode) == TextBoxMode.DateDMY)
            {
                mask = "99/99/9999";
                validation = "__DateCheckValue(this[0], \'" + ErrorMessage + "\');";
            }
            else if ((TextBoxMode) == TextBoxMode.TimeHM)
            {
                mask = "99:99";
                validation = "Masks_checkValue(this[0], \'^(([1-9]{1})|([0-1][0-9])|([1-2][0-3])):([0-5][0-9])$\', \'" + ErrorMessage + "\');";
            }
            else if ((TextBoxMode) == TextBoxMode.TimeHMS)
            {
                mask = "99:99:99";
                validation = "Masks_checkValue(this[0], \'^((0-1)(0-9)|(2)(0-3)):((0-5)(0-9)):((0-5)(0-9))$\', \'" + ErrorMessage + "\');";
            }
            else if ((TextBoxMode) == TextBoxMode.CEP)
            {
                mask = "99999-999";
            }
            else if ((TextBoxMode) == TextBoxMode.CPF)
            {
                mask = "999.999.999-99";
                validation = "if (!Masks_validaCPF(this[0].value)) { this[0].value = \'\'; " + ((string.IsNullOrEmpty(ErrorMessage)) ? "" : "alert(\'" + ErrorMessage + "\');").ToString() + "} "; //+ Attributes("onblur")
            }
            else if ((TextBoxMode) == TextBoxMode.CNPJ)
            {
                mask = "99.999.999/9999-99";
                validation = "if (!Masks_validaCNPJ(this[0].value)) { this[0].value = \'\'; " + ((string.IsNullOrEmpty(ErrorMessage)) ? "" : "alert(\'" + ErrorMessage + "\');").ToString() + "} ";
            }
            else if ((TextBoxMode) == TextBoxMode.PhoneNumberDDD)
            {
                mask = "99 9999-9999";
            }
            else if ((TextBoxMode) == TextBoxMode.PhoneNumber)
            {
                mask = "9999-9999";
            }
            else if ((TextBoxMode) == TextBoxMode.CustomMask)
            {
                mask = CustomMask;
                if (!string.IsNullOrEmpty(CustomValidation))
                {
                    validation = "Masks_checkValue(this[0], \'" + CustomValidation + "\', \'" + ErrorMessage + "\');";
                }
            }
            if (!string.IsNullOrEmpty(validation))
            {
                validation = ",completed:function(){" + validation + "debugger;this.blur();this.val(\'\');}";
            }



            string script = string.Empty;
            script += "jQuery(function($){" + "\r\n";
            script += "  $(\"#" + ClientID + "\").mask(\"" + mask + "\",{placeholder:\"" + placeholder + "\"" + validation + "});" + "\r\n";
            script += "});" + "\r\n";

            Page.ClientScript.RegisterStartupScript(this.GetType(), ClientID, script, true);
        }


        /// <summary>
        /// Sets the mask based on textboxmode.
        /// </summary>
        private void SetMaskVersion1()
        {
            if (TextBoxMode != TextBoxMode.SingleLine && TextBoxMode != TextBoxMode.MultiLine && TextBoxMode != TextBoxMode.Password && TextBoxMode != TextBoxMode.RichText)
            {
                // fix up event attributes to avoid overriding user code.
                if (!string.IsNullOrEmpty(Attributes["onkeypress"]) && !Attributes["onkeypress"].EndsWith(";"))
                {
                    Attributes["onkeypress"] += ";";
                }
                if (!string.IsNullOrEmpty(Attributes["onkeydown"]) && !Attributes["onkeydown"].EndsWith(";"))
                {
                    Attributes["onkeydown"] += ";";
                }
                if (!string.IsNullOrEmpty(Attributes["onclick"]) && !Attributes["onclick"].EndsWith(";"))
                {
                    Attributes["onclick"] += ";";
                }
                /*
                 'If (Not String.IsNullOrEmpty(Attributes("onload")) AndAlso Not Attributes("onload").EndsWith(";")) Then Attributes("onload") += ";"
                 */
                if (!string.IsNullOrEmpty(Attributes["onblur"]) && !Attributes["onblur"].EndsWith(";"))
                {
                    Attributes["onblur"] += ";";
                }
                if (!string.IsNullOrEmpty(Attributes["onchange"]) && !Attributes["onchange"].EndsWith(";"))
                {
                    Attributes["onchange"] += ";";
                }
            }


            string onkeypress = string.Empty;
            string onkeydown = string.Empty;
            string onchange = string.Empty;
            string onblur = string.Empty;
            string onclick = string.Empty;



            if ((TextBoxMode) == TextBoxMode.Integer)
            {
                onkeypress = "return Masks_Integer(event);";
            }
            else if ((TextBoxMode) == TextBoxMode.Percent)
            {
                onkeypress = "return Masks_Integer(event);";
                onchange = "if (this.value && new Number(this.value) < 0) this.value=0;if (this.value && new Number(this.value) > 100) this.value=100;" + Attributes["onchange"];
            }
            else if ((TextBoxMode) == TextBoxMode.Float)
            {
                onkeypress = "return Masks_Float(event, this);";
                onblur = "Masks_FormatFloat(this,2);";
            }
            else if ((TextBoxMode) == TextBoxMode.Currency)
            {
                onkeydown = "return Masks_Currency(event, this);";
                onclick = onkeydown;
                /*
                 * Attributes("onload") += Attributes("onkeydown")
                 */
                Style[HtmlTextWriterStyle.TextAlign] = "right";
            }
            else if ((TextBoxMode) == TextBoxMode.DateDMY)
            {
                onkeypress = "return Masks_Format(event, this,\'##/##/####\');";
                onblur = "__DateCheckValue(this, \'" + ErrorMessage + "\');";
            }
            else if ((TextBoxMode) == TextBoxMode.TimeHM)
            {
                onkeypress = "return Masks_Format(event, this,\'##:##\');";
                /*
                 * Attributes("onblur") += "Masks_checkValue(this, '^((0-1)(0-9)|(2)(0-3)):((0-5)(0-9))$', '" + ErrorMessage + "');"
                 */
                onblur = "Masks_checkValue(this, \'^(([1-9]{1})|([0-1][0-9])|([1-2][0-3])):([0-5][0-9])$\', \'" + ErrorMessage + "\');";
            }
            else if ((TextBoxMode) == TextBoxMode.TimeHMS)
            {
                onkeypress = "return Masks_Format(event, this,\'##:##:##\');";
                onblur = "Masks_checkValue(this, \'^((0-1)(0-9)|(2)(0-3)):((0-5)(0-9)):((0-5)(0-9))$\', \'" + ErrorMessage + "\');";
            }
            else if ((TextBoxMode) == TextBoxMode.Email)
            {
                onblur = "Masks_checkValue(this, \'\\\\w+((-+.\\\\\\\')\\\\w+)*@\\\\w+((-.)\\\\w+)*\\\\.\\\\w+((-.)\\\\w+)*\', \'" + ErrorMessage + "\');";
            }
            else if ((TextBoxMode) == TextBoxMode.CEP)
            {
                onkeypress = "return Masks_Format(event, this,\'#####-###\');";
            }
            else if ((TextBoxMode) == TextBoxMode.CPF)
            {
                onkeypress = "return Masks_Format(event, this,\'###.###.###-##\');" + Attributes["onkeypress"];
                onchange = "if (this.value!= null && this.value != \'\' && !Masks_validaCPF(this.value)) { this.value = \'\'; " + ((string.IsNullOrEmpty(ErrorMessage)) ? "" : "alert(\'" + ErrorMessage + "\');").ToString() + "} "; //+ Attributes("onblur")
            }
            else if ((TextBoxMode) == TextBoxMode.CNPJ)
            {
                onkeypress = "return Masks_Format(event, this,\'##.###.###/####-##\');";
                onchange = "if (this.value!= null && this.value != \'\' && !Masks_validaCNPJ(this.value)) { this.value = \'\'; " + ((string.IsNullOrEmpty(ErrorMessage)) ? "" : "alert(\'" + ErrorMessage + "\');").ToString() + "} ";
            }
            else if ((TextBoxMode) == TextBoxMode.PhoneNumberDDD)
            {
                onkeypress = "return Masks_Format(event, this,\'## ####-####\');";
            }
            else if ((TextBoxMode) == TextBoxMode.PhoneNumber)
            {
                onkeypress = "return Masks_Format(event, this,\'####-####\');";
            }
            else if ((TextBoxMode) == TextBoxMode.CustomMask)
            {
                onkeypress = "return Masks_Format(event, this,\'" + CustomMask + "\');";
                onblur = "Masks_checkValue(this, \'" + CustomValidation + "\', \'" + ErrorMessage + "\');";
            }
            else if ((TextBoxMode) == TextBoxMode.RichText)
            {
                //Registering TinyMCE startup code.
                string script = "" + "\r\n" + "   TextBox_ID = \"" + ClientID + "\";" + "\r\n" + "	tinyMCE.init({" + "\r\n" + "		theme : \"advanced\"," + "\r\n" + "		language : \"pt_br\"," + "\r\n" + "       //editor_selector : \"TextArea\"," + "\r\n" + "		mode : \"exact\"," + "\r\n" + "       elements: \"" + this.ClientID + "\", " + "\r\n" + "		theme_advanced_toolbar_location : \"top\"," + "\r\n" + "		theme_advanced_toolbar_align : \"left\"," + "\r\n" + "		theme_advanced_disable : \"code\", " + "\r\n" + "       force_br_newlines : true, " + "\r\n" + "       force_p_newlines : false, " + "\r\n" + "		plugins : \"contextmenu,paste,preview,fullscreen,inlinepopups\"," + "\r\n" + "		theme_advanced_buttons1: \"fontsizeselect,fontselect,separator,bold,italic,underline,forecolor,separator,bullist,numlist\"," + "\r\n" + "		theme_advanced_buttons2: \"outdent,indent,separator,undo,redo,separator,link,unlink,separator,hr,separator,justifyleft,justifycenter,justifyright,justifyfull,separator\"," + "\r\n" + "		theme_advanced_buttons1_add : \"separator,pastetext,pasteword,separator,fullscreen,separator,cleanup,removeformat\", "
                + "\r\n" + "		theme_advanced_buttons3: \"\"," + "\r\n" + "       onchange_callback : \"TextBox_HandleChange\"," + "\r\n" + "		debug : false" + "\r\n" + "	});" + "\r\n" + "";

                /*
                 * "		//theme_advanced_styles : ""Parágrafo=parag;"", " + vbCrLf + _
                 * "		//content_css : ""arquivo.css""," + vbCrLf + _
                 * "		//theme_advanced_buttons3_add : ""separator, pastetext, pasteword, separator, tablecontrols"", " + vbCrLf + _
                 */

                //Registers the main tinymce code include
                Page.ClientScript.RegisterClientScriptInclude("TextArea_Include", ResolveUrl("~/" + Configuration.HttpModule.HandlerFileName) + "/tiny_mce/tiny_mce.js");
                //Registers the main tinymce startup code 
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TextArea_Startup" + this.ClientID, script, true);
            }
            else if ((((TextBoxMode) == TextBoxMode.SingleLine) || ((TextBoxMode) == TextBoxMode.MultiLine)) || ((TextBoxMode) == WebControls.TextBoxMode.Password))
            {
                //nothing todo here
            }


            if (!string.IsNullOrEmpty(onkeypress))
            {
                if (Attributes["onkeypress"] == null)
                {
                    Attributes["onkeypress"] = onkeypress;
                }
                else if (!Attributes["onkeypress"].Contains(onkeypress))
                {
                    Attributes["onkeypress"] += onkeypress;
                }
            }


            if (!string.IsNullOrEmpty(onkeydown))
            {
                if (Attributes["onkeydown"] == null)
                {
                    Attributes["onkeydown"] = onkeydown;
                }
                else if (!Attributes["onkeydown"].Contains(onkeydown))
                {
                    Attributes["onkeydown"] += onkeydown;
                }
            }


            if (!string.IsNullOrEmpty(onblur))
            {
                if (Attributes["onblur"] == null)
                {
                    Attributes["onblur"] = onblur;

                }
                else if (!Attributes["onblur"].Contains(onblur))
                {
                    Attributes["onblur"] = onblur + Attributes["onblur"];
                }
            }

            if (!string.IsNullOrEmpty(onclick))
            {
                if (Attributes["onclick"] == null)
                {
                    Attributes["onclick"] = onclick;
                }
                else if (!Attributes["onclick"].Contains(onclick))
                {
                    Attributes["onclick"] += onclick;
                }
            }

            if (!string.IsNullOrEmpty(onchange))
            {
                if (Attributes["onchange"] == null)
                {
                    Attributes["onchange"] = onchange;
                }
                else if (!Attributes["onchange"].Contains(onchange))
                {
                    Attributes["onchange"] += onchange;
                }
            }
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            switch (TextBoxMode)
            {
                case WebControls.TextBoxMode.DateDMY:
                    if (ShowCalendar != TextBoxCalendarButton.None)
                    {
                        RenderCalendarButton(writer);
                    }
                    else
                    {
                        base.Render(writer);
                    }
                    break;
                default:
                    base.Render(writer);
                    break;
            }
        }



        #region " Calendar "


        /// <summary>
        /// Register calendar scripts and css styles.
        /// </summary>
        private void RegisterCalendarScripts()
        {
            if (Page.Header == null)
            {
                throw (new Exception("The header tag must be a server control. Set runat attribute of header to server."));
            }

            string cssfile = "";
            switch (ShowCalendar)
            {
                case TextBoxCalendarButton.XPStyle:
                    cssfile = Configuration.Resources.CalendarSystemCss;
                    break;
            }
            System.Web.UI.HtmlControls.HtmlLink lnk = new System.Web.UI.HtmlControls.HtmlLink();
            lnk.Href = Page.ClientScript.GetWebResourceUrl(this.GetType(), cssfile);
            lnk.Attributes["type"] = "text/css";
            lnk.Attributes["rel"] = "stylesheet";
            lnk.Attributes["media"] = "all";
            lnk.Attributes["title"] = "system";
            Page.Header.Controls.Add(lnk);



            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsCalendar);
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsCalendarEn);
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsCalendarBr);
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsCalendarSetup);
            string script = "\r\n" + "var _" + ClientID + "_dropdown = false;" + "\r\n" + "Calendar.setup({" + "\r\n" + "    inputField     :    \"" + ClientID + "\",     // id of the input field" + "\r\n" + "    ifFormat       :    \"%d/%m/%Y\",      // format of the input field" + "\r\n" + "    button         :    \"" + ClientID + "_dropdown" + "\",  // trigger for the calendar (button ID)" + "\r\n" + "    align          :    \"Bl\",           // alignment (defaults to \"Bl\")" + "\r\n" + "    singleClick    :    true, " + "\r\n" + "    onClose        :    function(cal) {" + "cal.hide();_" + ClientID + "_dropdown=false;document.getElementById(\'" + ClientID + "_dropdown\').src=\'" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.CalendarCalNJpeg) + "\';return true;}" + "\r\n" + "});";
            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script, true);

        }


        /// <summary>
        /// Renders the HTML of calendar button.
        /// </summary>
        private void RenderCalendarButton(HtmlTextWriter writer)
        {

            switch (ShowCalendar)
            {
                case TextBoxCalendarButton.XPStyle:

                    string oldstyle = Style.Value;
                    Style.Value = "border: solid #7F9DB9 1px !important;background-color:white !important;";


                    System.IO.StringWriter baseCode = new System.IO.StringWriter();
                    HtmlTextWriter mywriter = new HtmlTextWriter(baseCode);

                    Unit oldheight = Height;
                    Unit oldwidth = Width;
                    Height = new Unit(21 - 4, UnitType.Pixel);
                    if (Width.IsEmpty || Width.Value < 100)
                    {
                        Width = Unit.Parse("100px");
                    }
                    Width = new Unit(Width.Value - 34 - 4, Width.Type);
                    base.Render(mywriter);
                    Height = oldheight;
                    Width = oldwidth;
                    Style.Value = oldstyle;
                    string input = baseCode.ToString();


                    string img = "<img id=\"" + ClientID + "_dropdown" + "\" src=\"" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.CalendarCalNJpeg) + "\" " + " onmouseover=\"if (!_" + ClientID + "_dropdown) this.src=\'" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.CalendarCalHJpeg) + "\';\" onmouseout=\"if (!_" + ClientID + "_dropdown) this.src=\'" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.CalendarCalNJpeg) + "\';\" onmousedown=\"_" + ClientID + "_dropdown=true;this.src=\'" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.CalendarCalDJpeg) + "\';\" onmouseup=\"this.src=\'" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.CalendarCalDJpeg) + "\';\" alt=\"\" align=\"top\" style=\"position:relative;left:-1px;" + (this.DesignMode ? "top:1px;" : "top:expression(\'1px\');").ToString() + "\" />";

                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");

                    div.Style.Value = oldstyle;

                    if (oldwidth.IsEmpty || oldwidth.Value < 100)
                    {
                        oldwidth = Unit.Parse("100px");
                    }
                    div.Style["width"] = oldwidth.ToString();
                    div.Style["height"] = new Unit(21, UnitType.Pixel).ToString();
                    if (string.IsNullOrEmpty(div.Style["display"]))
                    {
                        div.Style["display"] = "inline";
                    }

                    div.InnerHtml = input + img;

                    div.RenderControl(writer);
                    break;
            }

        }
        #endregion
    }
}