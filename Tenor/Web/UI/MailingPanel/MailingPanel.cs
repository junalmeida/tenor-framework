using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tenor.Mail;


namespace Tenor.Web.UI.WebControls.Core
{
    /// <summary>
    /// The MailingPanel webcontrol makes easy to send forms to email without coding.
    /// </summary>
    /// <remarks></remarks>
    [ToolboxData("<{0}:MailingPanel runat=server></{0}:MailingPanel>")]
    public class MailingPanel : MultiView
    {



        [DefaultValue(true)]
        [Description("Indica se deve usar o Text das checkboxes como identificador de campo."), Category("Behavior")]
        public bool UseTextForCheckbox
        {
            get
            {
                if (ViewState["UseTextForCheckbox"] == null)
                {
                    return true;
                }
                else
                {
                    return System.Convert.ToBoolean(ViewState["UseTextForCheckbox"]);
                }
            }
            set
            {
                ViewState["UseTextForCheckbox"] = value;
            }
        }

        /// <summary>
        /// Contém a URL de um modelo para utilizar. Caso não seja especificado, um modelo padrão será adotado.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [
        UrlProperty("*.htm;*.html;*.txt"),
        Editor("System.Web.UI.Design.UrlEditor", "System.Drawing.Design.UITypeEditor"),
        Description("Contém a URL de um modelo para utilizar. Caso não seja especificado, um modelo padrão será adotado."),
        Category("Layout")
        ]
        public string TemplateUrl
        {
            get
            {
                if (ViewState["TemplateUrl"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["TemplateUrl"].ToString();
                }
            }
            set
            {
                ViewState["TemplateUrl"] = value;
            }
        }

        public string EmailTos
        {
            get
            {
                if (ViewState["EmailTos"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["EmailTos"].ToString();
                }
            }
            set
            {
                ViewState["EmailTos"] = value;
            }
        }

        public string EmailSubject
        {
            get
            {
                if (ViewState["EmailSubject"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["EmailSubject"].ToString();
                }
            }
            set
            {
                ViewState["EmailSubject"] = value;
            }
        }

        public string EmailFrom
        {
            get
            {
                if (ViewState["EmailFrom"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["EmailFrom"].ToString();
                }
            }
            set
            {
                ViewState["EmailFrom"] = value;
            }
        }

        /// <summary>
        /// Mensagem de erro a ser exibida quando o processo não tiver sucesso.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DefaultValue("Não foi possível enviar a mensagem. Por favor tente novamente mais tarde.")]
        [Description("Mensagem de erro a ser exibida quando o processo não tiver sucesso."), Category("Behavior")]
        public string ErrorMessage
        {
            get
            {
                if (ViewState["ErrorMessage"] == null)
                {
                    return "Não foi possível enviar a mensagem. Por favor tente novamente mais tarde.";
                }
                else
                {
                    return ViewState["ErrorMessage"].ToString();
                }
            }
            set
            {
                ViewState["ErrorMessage"] = value;
            }
        }


        /// <summary>
        /// Prefixo dos campos que serão utilizados no envio da mensagem. Caso esta propriedade não seja preenchida, todos os campos serão usados.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Description("Prefixo dos campos que serão utilizados no envio da mensagem. Caso esta propriedade não seja preenchida, todos os campos serão usados."), Category("Behavior")]
        public string FieldPrefix
        {
            get
            {
                if (ViewState["FieldPrefix"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["FieldPrefix"].ToString();
                }
            }
            set
            {
                ViewState["FieldPrefix"] = value;
            }
        }

        /// <summary>
        /// Nome de um IButtonControl para ativar a ação de envio.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [TypeConverter(typeof(AssociatedControlConverter)), IDReferenceProperty()]
        [Description("Nome de um IButtonControl para ativar a ação de envio."), Category("Behavior")]
        public string SendButton
        {
            get
            {
                if (ViewState["SendButton"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["SendButton"].ToString();
                }
            }
            set
            {
                ViewState["SendButton"] = value;
            }
        }

        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        Obsolete("Propriedade não disponível.", true)]
        private new int ActiveViewIndex
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        protected override System.Web.UI.ControlCollection CreateControlCollection()
        {
            return new MailingViewCollection(this);
        }

        public new MailingViewCollection Views
        {
            get
            {
                return ((MailingViewCollection)this.Controls);
            }
        }

        /// <summary>
        /// Referência à View do formulário.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(false)]
        public FormView FormView
        {
            get
            {
                FormView vw = null;
                if (Views.Count > 0)
                {
                    vw = (FormView)(Views[0]);
                }
                else
                {
                    throw (new InvalidOperationException("Cannot find FormView"));
                }
                return vw;
            }
        }

        /// <summary>
        /// Referência à View de resultado.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(false)]
        public ResultView ResultView
        {
            get
            {
                ResultView vw = null;
                if (Views.Count > 1)
                {
                    vw = (ResultView)(Views[1]);
                }
                else
                {
                    throw (new InvalidOperationException("Cannot find ResultView"));
                }
                return vw;
            }
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptResource(typeof(System.Web.UI.WebControls.Image), "WebForms.js");
            base.OnPreRender(e);
        }

        private object _Scripter;
        private object Scripter
        {
            get
            {
                if (_Scripter == null)
                {
                    _Scripter = GetScripter(Page.Controls);
                }
                if (_Scripter == null && (Page.Master != null))
                {
                    _Scripter = GetScripter(Page.Master);
                }
                return _Scripter;
            }
        }


        private object GetScripter(MasterPage Master)
        {
            object obj = GetScripter(Master.Controls);
            if (obj == null)
            {
                if (Master.Master != null)
                {
                    return GetScripter(Master.Master);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return obj;
            }
        }

        private object GetScripter(ControlCollection Controls)
        {
            foreach (Control c in Controls)
            {
                if (c.GetType().FullName == Configuration.Resources.ScriptManagerClass)
                {
                    return c;
                }
                else
                {
                    object obj = GetScripter(c.Controls);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

        protected override void CreateChildControls()
        {
            if (FormView == null || ResultView == null)
            {
                throw (new Exception("You must set a FormView and a ResultView."));
            }

            if (string.IsNullOrEmpty(EmailTos))
            {
                throw (new Exception("You must provide valid email destinations."));
            }

            Control ctrl = FormView.NamingContainer.FindControl(SendButton);

            if (ctrl == null || (ctrl.GetType() != typeof(Button) && ctrl.GetType() != typeof(ImageButton) && ctrl.GetType() != typeof(LinkButton)))
            {
                throw (new Exception("A send button must be set and must be of type Button, ImageButton or LinkButton."));
            }

            ctrl = FormView.NamingContainer.FindControl(FieldPrefix + "Nome");
            if (ctrl == null || (ctrl.GetType() != typeof(System.Web.UI.WebControls.TextBox) && ctrl.GetType() != typeof(HiddenField)))
            {
                throw (new Exception("There must be a TextBox or HiddenField whose ID is \"" + FieldPrefix + "Nome" + "\" for field Nome."));
            }

            ctrl = FormView.NamingContainer.FindControl(FieldPrefix + "Email");
            if (ctrl == null || (ctrl.GetType() != typeof(System.Web.UI.WebControls.TextBox) && ctrl.GetType() != typeof(HiddenField)))
            {
                throw (new Exception("There must be a TextBox or HiddenField whose ID is \"" + FieldPrefix + "Email" + "\" for field Email."));
            }

            if (base.ActiveViewIndex == -1)
            {
                base.ActiveViewIndex = 0;
            }

            base.CreateChildControls();
        }

        protected void RegisterButtonEvent()
        {
            Control ctrl = FormView.NamingContainer.FindControl(SendButton);
            Type tipoBotao = ctrl.GetType();

            if (tipoBotao == typeof(Button))
            {
                Button btn = (Button)ctrl;
                btn.Click += new System.EventHandler(SendButton_Click);
            }
            else if (tipoBotao == typeof(LinkButton))
            {
                LinkButton btn = (LinkButton)ctrl;
                btn.Click += new System.EventHandler(SendButton_Click);
            }
            else if (tipoBotao == typeof(ImageButton))
            {
                ImageButton btn = (ImageButton)ctrl;
                btn.Click += new System.Web.UI.ImageClickEventHandler(SendImageButton_Click);
            }
        }

        protected override void OnInit(System.EventArgs e)
        {
            RegisterButtonEvent();

            if (Scripter == null)
            {
                throw (new InvalidOperationException("Cannot find Web.UI.WebControls.ScriptManager."));
            }

            base.OnInit(e);
        }


        private void Alert(string Mensagem)
        {
            System.Reflection.MethodInfo alert = Scripter.GetType().GetMethod("Alert", new Type[] { typeof(string) });
            alert.Invoke(Scripter, new object[] { Mensagem });
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendImageButton_Click(object sender, ImageClickEventArgs e)
        {
            SendMessage();
        }

        protected void SendMessage()
        {
            try
            {
                string nome = string.Empty;
                string email = string.Empty;
                Control ctrlNome = FormView.NamingContainer.FindControl(FieldPrefix + "Nome");
                Control ctrlEmail = FormView.NamingContainer.FindControl(FieldPrefix + "Email");

                if (ctrlNome.GetType() == typeof(System.Web.UI.WebControls.TextBox))
                {
                    nome = ((System.Web.UI.WebControls.TextBox)ctrlNome).Text.Trim();
                }
                else if (ctrlNome.GetType() == typeof(TextBox))
                {
                    nome = ((TextBox)ctrlNome).Text.Trim();
                }
                else if (ctrlNome.GetType() == typeof(HiddenField))
                {
                    nome = ((HiddenField)ctrlNome).Value.Trim();
                }

                if (ctrlEmail.GetType() == typeof(System.Web.UI.WebControls.TextBox))
                {
                    email = ((System.Web.UI.WebControls.TextBox)ctrlEmail).Text.Trim();
                }
                else if (ctrlEmail.GetType() == typeof(TextBox))
                {
                    email = ((TextBox)ctrlEmail).Text.Trim();
                }
                else if (ctrlEmail.GetType() == typeof(HiddenField))
                {
                    email = ((HiddenField)ctrlEmail).Value.Trim();
                }

                Mail.MailMessage msg = new Mail.MailMessage();
                if (!string.IsNullOrEmpty(TemplateUrl))
                {
                    msg.Template = System.IO.File.ReadAllText(Page.MapPath(TemplateUrl));
                }

                msg.From = new System.Net.Mail.MailAddress(EmailFrom, nome);
                msg.ReplyTo = new System.Net.Mail.MailAddress(email, nome);

                string template = string.Empty;

                BuildTemplate(ref msg, ref template, FormView.Controls);

                string mensagem = string.Empty;
                System.Collections.Specialized.NameValueCollection dados = new System.Collections.Specialized.NameValueCollection();
                foreach (string key in msg.TemplateValues.Keys)
                {
                    dados.Add(key, msg.TemplateValues[key]);
                    if (msg.TemplateValues[key].Length > Tenor.Configuration.MailMessage.MaxLengthTemplateValue)
                    {
                        mensagem += "- O campo " + key + " só pode ter até " + Tenor.Configuration.MailMessage.MaxLengthTemplateValue.ToString() + " caracteres." + Environment.NewLine;
                    }
                }

                msg.To.Add(EmailTos);
                msg.Subject = EmailSubject;

                if (!string.IsNullOrEmpty(mensagem))
                {
                    Alert(mensagem);
                }
                else
                {
                    if (string.IsNullOrEmpty(msg.Template))
                    {
                        msg.Template = template;
                    }

                    msg.IsBodyHtml = true;

                    msg.Send();

                    SetActiveView(ResultView);
                }

            }
            catch (Exception ex)
            {
                Alert(ErrorMessage);
                Diagnostics.Debug.HandleError(ex);
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }

        protected void BuildTemplate(ref MailMessage Message, ref string Template, ControlCollection ControlCollection)
        {
            Template = string.Empty;
            foreach (Control ctrl in ControlCollection)
            {
                if ((ctrl.ID != null) && ctrl.ID.StartsWith(FieldPrefix))
                {
                    string key = string.Empty;
                    string value = string.Empty;

                    Type ctrlType = ctrl.GetType();

                    if ((ctrlType == typeof(TextBox)) || ctrlType.IsSubclassOf(typeof(TextBox)))
                    {
                        System.Web.UI.WebControls.TextBox txt = (System.Web.UI.WebControls.TextBox)ctrl;
                        key = txt.ID.Substring(FieldPrefix.Length, txt.ID.Length - FieldPrefix.Length);
                        value = txt.Text.Trim();
                    }
                    else if (ctrlType == typeof(HiddenField))
                    {
                        HiddenField hdn = (HiddenField)ctrl;
                        key = hdn.ID.Substring(FieldPrefix.Length, hdn.ID.Length - FieldPrefix.Length);
                        value = hdn.Value;
                    }
                    else if (ctrlType == typeof(DropDownList))
                    {
                        DropDownList ddl = (DropDownList)ctrl;
                        key = ddl.ID.Substring(FieldPrefix.Length, ddl.ID.Length - FieldPrefix.Length);
                        value = ddl.SelectedValue;
                    }
                    else if (ctrlType == typeof(RadioButtonList))
                    {
                        RadioButtonList rbl = (RadioButtonList)ctrl;
                        key = rbl.ID.Substring(FieldPrefix.Length, rbl.ID.Length - FieldPrefix.Length);
                        value = rbl.SelectedValue;
                    }
                    else if ((ctrlType == typeof(CheckBoxList)) || ctrlType.IsSubclassOf(typeof(CheckBoxList)))
                    {
                        System.Web.UI.WebControls.CheckBoxList chk = (System.Web.UI.WebControls.CheckBoxList)ctrl;
                        key = chk.ID.Substring(FieldPrefix.Length, chk.ID.Length - FieldPrefix.Length);
                        foreach (ListItem item in chk.Items)
                        {
                            if (item.Selected)
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    value += ", ";
                                }
                                value += item.Value;
                            }
                        }
                    }
                    else if (ctrlType == typeof(CheckBox))
                    {
                        CheckBox chk = (CheckBox)ctrl;
                        if (UseTextForCheckbox)
                        {
                            key = chk.Text;
                        }
                        else
                        {
                            key = chk.ID.Substring(FieldPrefix.Length, chk.ID.Length - FieldPrefix.Length);
                        }
                        value = "Não";
                        if (chk.Checked)
                        {
                            value = "Sim";
                        }
                    }
                    else if (ctrlType == typeof(ListBox))
                    {
                        ListBox lsb = (ListBox)ctrl;
                        key = lsb.ID.Substring(FieldPrefix.Length, lsb.ID.Length - FieldPrefix.Length);
                        foreach (ListItem item in lsb.Items)
                        {
                            if (item.Selected)
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    value += ", ";
                                }
                                value += item.Value;
                            }
                        }
                    }
                    else if (ctrlType == typeof(FileUpload))
                    {
                        FileUpload fup = (FileUpload)ctrl;
                        if (fup.HasFile)
                        {
                            Message.Attachments.Add(new System.Net.Mail.Attachment(fup.PostedFile.InputStream, System.IO.Path.GetFileName(fup.PostedFile.FileName)));
                        }
                    }

                    if (!string.IsNullOrEmpty(key))
                    {
                        string prepKey = Text.Strings.RemoveAccentuation(key).Replace(" ", "").ToLower();
                        Message.TemplateValues.Add(prepKey, value);
                        Template += "<p><strong>" + key + ":</strong> [[[" + prepKey + "]]]</p>" + Environment.NewLine;
                    }
                }

                if (ctrl.HasControls())
                {
                    BuildTemplate(ref Message, ref Template, ctrl.Controls);
                }
            }
        }
    }
}