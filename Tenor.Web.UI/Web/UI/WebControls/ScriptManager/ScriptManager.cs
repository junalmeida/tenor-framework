using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Security.Permissions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Reflection;
using System.ComponentModel;


namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				
				/// <summary>
				/// Objeto que contem informação para o evento <see cref="ScriptManager.Confirmation">ScriptManager.Confirmation</see>
				/// </summary>
				/// <remarks></remarks>
				public class ConfirmationEventArgs : EventArgs
				{
					
					
					public ConfirmationEventArgs(string CommandName, bool Response)
					{
						this.CommandName = CommandName;
						this.Response = Response;
					}
					
					private bool _Response;
					/// <summary>
					/// Contém a resposta do usuário no evento Confirmation.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					public bool Response
					{
						get
						{
							return _Response;
						}
						set
						{
							_Response = value;
						}
					}
					private string _CommandName;
					/// <summary>
					/// Contém o nome do comando.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					public string CommandName
					{
						get
						{
							return _CommandName;
						}
						set
						{
							_CommandName = value;
						}
					}
					
					
				}
				
				public delegate void ConfirmationEventHandler(object sender, ConfirmationEventArgs e);
				
				/// <summary>
				/// Controle com diversas funcionalidades de JavaScript 1.2.
				/// </summary>
				/// <remarks></remarks>
				[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), PersistChildren(false), ParseChildren(ChildrenAsProperties = true, DefaultProperty = "Scripts"), ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:ScriptManager runat=\"server\" />"), Designer(typeof(Design.ScriptManagerDesigner)), ToolboxBitmapAttribute(typeof(ScriptManager), "ScriptManager.bmp")]public class ScriptManager : Control, IPostBackEventHandler
				{
					
					
					
					private ConfirmationEventHandler ConfirmationEvent;
					public event ConfirmationEventHandler Confirmation
					{
						add
						{
							ConfirmationEvent = (ConfirmationEventHandler) System.Delegate.Combine(ConfirmationEvent, value);
						}
						remove
						{
							ConfirmationEvent = (ConfirmationEventHandler) System.Delegate.Remove(ConfirmationEvent, value);
						}
					}
					
					protected void OnConfirmation(ConfirmationEventArgs e)
					{
						if (ConfirmationEvent != null)
							ConfirmationEvent(this, e);
					}
					
					private ScriptCollection _scripts;
					//'<DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
					//'NotifyParentProperty(True), _
					//<Editor(GetType(Design.ScriptCollectionEditor), GetType(Drawing.Design.UITypeEditor))> _
					//<EditorBrowsable(EditorBrowsableState.Always)> _
					//<PersistenceMode(PersistenceMode.InnerDefaultProperty)> _
					//Public ReadOnly Property Scripts() As ScriptCollection
					//    Get
					//        Return _scripts
					//    End Get
					//End Property
					
					
					/// <summary>
					/// Armazena uma coleção de scripts pré definidos
					/// </summary>
					/// <remarks></remarks>
					[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor(typeof(Design.ScriptCollectionEditor), typeof(System.Drawing.Design.UITypeEditor)), PersistenceMode(PersistenceMode.InnerProperty), Description("Provides a collection of scripts to this page"), Browsable(false), EditorBrowsable(EditorBrowsableState.Always), Category("Misc")]public ScriptCollection Scripts
					{
						get
						{
							if (_scripts == null)
							{
								_scripts = new ScriptCollection();
							}
							
							return _scripts;
						}
					}
					
					protected override void OnInit(System.EventArgs e)
					{
						
						//AddHandler Page.Init, AddressOf Page_Init
						base.OnInit(e);
						
						
					}
					
					protected override void OnPreRender(System.EventArgs e)
					{
						//Cagou  o repeater e por iss ovoltei pro prerender.. Se der problema, deveria ser no init
						base.OnPreRender(e);
						
						Page_Init(null, e);
					}
					
					
					private void Page_Init(object sender, EventArgs e)
					{
						//procura uma lista de script e adiciona exclusivamente cada um à página atual
						
						List<Type> listaScripts = new List<Type>();
						foreach (Script i in Scripts)
						{
							if (! listaScripts.Contains(i.GetType()))
							{
								listaScripts.Add(i.GetType());
								i.Initialize(this.Page);
							}
						}
					}
					
					///' <summary>
					///' Retorna o objeto Tracking Tipado para uso.
					///' É necessário que o ScriptManager contenha um Script Tracking.
					///' </summary>
					///' <returns></returns>
					///' <remarks></remarks>
					//Public Function GetTracking() As Tracking
					//    For Each i As Script In Me.Scripts
					//        If TypeOf i Is Tracking Then
					//            Return CType(i, WebControls.Tracking)
					//        End If
					//    Next
					//    Return Nothing
					//End Function
					
					
					private string GetScriptTags(string script)
					{
						if (! script.StartsWith("\r\n"))
						{
							script = "\r\n" + script;
						}
						if (! script.EndsWith("\r\n"))
						{
							script += "\r\n";
						}
						
						return "<script type=\"text/javascript\">" + script + "</script>";
					}
					
					/// <summary>
					/// Procura um controle do tipo específico
					/// </summary>
					/// <param name="ControlType">Nome completo do tipo desejado</param>
					/// <param name="Controls"></param>
					/// <returns></returns>
					/// <remarks></remarks>
					private static Control SearchControl(string ControlType, ControlCollection Controls)
					{
						foreach (Control i in Controls)
						{
							if (i.GetType().FullName.Equals(ControlType))
							{
								return i;
							}
							else
							{
								Control obj = SearchControl(ControlType, i.Controls);
								if (obj != null)
								{
									return obj;
								}
							}
						}
						return null;
					}
					/// <summary>
					/// Prepara e envia um script por MagicAjax.
					/// </summary>
					/// <param name="Script"></param>
					/// <returns>Se o contexto é uma chamada ajax ou não.</returns>
					/// <remarks></remarks>
					private static bool SendMagicAjaxScript(Page Page, string Script)
					{
						
						
						Control AjaxPanel = SearchControl("MagicAjax.UI.Controls.AjaxPanel", Page.Controls);
						if (AjaxPanel != null)
						{
							Type contexttype = AjaxPanel.GetType().Assembly.GetType("MagicAjax.MagicAjaxContext");
							PropertyInfo current = contexttype.GetProperty("Current");
							object context = current.GetValue(null, null);
							if (System.Convert.ToBoolean(contexttype.GetProperty("IsAjaxCall").GetValue(context, null)))
							{
								Type helper = AjaxPanel.GetType().Assembly.GetType("MagicAjax.AjaxCallHelper");
								
								
								try
								{
									MethodInfo write = helper.GetMethod("WriteOnEnd");
									if (write == null)
									{
										write = helper.GetMethod("Write");
									}
									write.Invoke(null, new object[] {(Script)});
								}
								catch (Exception)
								{
									return false;
								}
								return true;
							}
							else
							{
								return false;
							}
						}
						else
						{
							return false;
						}
					}
					
					/// <summary>
					/// Prepara e envia um script por AjaxNet (Atlas)
					/// </summary>
					/// <param name="Key"></param>
					/// <param name="Script"></param>
					/// <returns></returns>
					/// <remarks></remarks>
					private static bool SendAjaxNetScript(Page Page, string Key, string Script)
					{
						Control ScriptManager = SearchControl("System.Web.UI.ScriptManager", Page.Controls);
						if (ScriptManager == null)
						{
							ScriptManager = SearchControl("Microsoft.Web.UI.ScriptManager", Page.Controls);
						}
						if (ScriptManager != null)
						{
							PropertyInfo isasync = ScriptManager.GetType().GetProperty("IsInAsyncPostBack");
							if (System.Convert.ToBoolean(isasync.GetValue(ScriptManager, null)))
							{
								MethodInfo reg = ScriptManager.GetType().GetMethod("RegisterStartupScript", new Type[] {typeof(Page), typeof(Type), typeof(string), typeof(string), typeof(bool)});
								object updPanel;
								
								if (reg != null)
								{
									updPanel = Page;
								}
								else
								{
									reg = ScriptManager.GetType().GetMethod("RegisterStartupScript", new Type[] {typeof(Control), typeof(Type), typeof(string), typeof(string), typeof(bool)});
									if (reg == null)
									{
										System.Diagnostics.Trace.TraceError("Unsuported Microsoft AjaxNet version.");
										return false;
									}
									updPanel = SearchControl("Microsoft.Web.UI.UpdatePanel", Page.Controls);
									if (updPanel == null)
									{
										updPanel = Page;
									}
									
								}
								reg.Invoke(null, new object[] {updPanel, typeof(ScriptManager), Key, Script, true});
							}
							else
							{
								
								return false;
							}
							return true;
						}
						else
						{
							return false;
						}
					}
					
					/// <summary>
					/// Gera uma chave aleatória para enviar o script para o cliente.
					/// </summary>
					/// <returns></returns>
					/// <remarks></remarks>
					private static string GetRandomKey()
					{
						string alertkey = "ScriptManager." + Guid.NewGuid().ToString();
						return alertkey;
					}
					
					/// <summary>
					/// Faz o Encoding para javascript
					/// </summary>
					/// <param name="Message"></param>
					/// <returns></returns>
					/// <remarks></remarks>
					private string EncodeMessage(string text)
					{
						
						text = text.Replace("\\", "\\\\");
						text = text.Replace("\"", "\\\"");
						//Message = Message.Replace("'", "\'")
						text = text.Replace("\t", "\\t");
						text = text.Replace("\r\n", "\\n");
						text = text.Replace("\r", "\\n");
						text = text.Replace("\n", "\\n");
						
						return text;
					}
					
					
					/// <summary>
					/// Mostra uma janela de alerta e redireciona a página para outro endereço
					/// </summary>
					/// <param name="message">Mensagem de alerta</param>
					/// <param name="url">Url da pagina</param>
					/// <remarks></remarks>
					public void AlertAndRedirect(string message, string url)
					{
						Alert(message, true);
						Redirect(url);
					}
					
					/// <summary>
					/// Mostra uma janela de alerta e redireciona a página para outro endereço
					/// </summary>
					/// <param name="message">Mensagem de alerta</param>
					/// <param name="url">Url da pagina</param>
					/// <remarks></remarks>
					public void AlertAndRedirect(string message, string url, bool endResponse)
					{
						if (endResponse)
						{
							
							message = EncodeMessage(message);
							
							string script = this.GetScriptTags("alert(\"{0}\"); " + "\r\n" + "try {{ location.replace(\"{1}\"); }} catch(e) {{ }}" + "\r\n") + "\r\n" + "<a href=\"{2}\">Redirecionar</a>";
							
							
							script = string.Format(script, message, this.Page.ResolveClientUrl(url).Replace("\"", "\"\""), url);
							HttpResponse response = HttpContext.Current.Response;
							
							response.Write("<html><head><title>Redirecionar</title></head><body>" + "\r\n" + script + "\r\n" + "</body></html>");
							response.End();
						}
						else
						{
							AlertAndRedirect(message, url);
						}
					}
					
					/// <summary>
					/// Redireciona para o endereço passado.
					/// </summary>
					/// <param name="url">Endereço de destino</param>
					/// <remarks></remarks>
					public void Redirect(string url)
					{
						string key = GetRandomKey();
						
						string script = "location.replace(\'" + this.Page.ResolveClientUrl(url).Replace("\'", "\'\'") + "\');";
						if (! SendMagicAjaxScript(Page, script) && ! SendAjaxNetScript(Page, key, script))
						{
							Page.ClientScript.RegisterStartupScript(this.GetType(), key, script, true);
						}
					}
					
					/// <summary>
					/// Mostra uma janela de alerta no navegador do cliente
					/// </summary>
					/// <param name="message">Mensagem a ser mostrada</param>
					/// <remarks>Mostra uma janela de alerta ao final do processamento da página.</remarks>
					public void Alert(string message)
					{
						Alert(message, true);
					}
					
					/// <summary>
					/// Mostra uma janela de alerta no navegador do cliente
					/// </summary>
					/// <param name="message">Mensagem a ser mostrada</param>
					/// <param name="AfterPage">Verdadeiro para mostrar a mensagem após a página ser carregada. Padrão é verdadeiro.</param>
					/// <remarks></remarks>
					public void Alert(string message, bool AfterPage)
					{
						string alertkey = GetRandomKey();
						message = EncodeMessage(message);
						
						string script = "alert(\"" + message + "\");";
						if (! SendMagicAjaxScript(Page, script) && ! SendAjaxNetScript(Page, alertkey, script))
						{
							if (AfterPage)
							{
								Page.ClientScript.RegisterStartupScript(this.GetType(), alertkey, script, true);
							}
							else
							{
								Page.ClientScript.RegisterClientScriptBlock(this.GetType(), alertkey, script, true);
							}
						}
						
					}
					
					/// <summary>
					/// Abre uma nova janela no navegador do cliente.
					/// </summary>
					/// <param name="url">Url de destino</param>
					/// <remarks></remarks>
					public void OpenNewWindow(string url)
					{
						OpenNewWindow(url, Unit.Empty, Unit.Empty, false, "_blank");
					}
					
					/// <summary>
					/// Abre uma nova janela no navegador do cliente.
					/// </summary>
					/// <param name="url">Url de destino</param>
					/// <param name="Width">Largura em pixels</param>
					/// <param name="Height">Altura em pixels</param>
					/// <remarks></remarks>
					public void OpenNewWindow(string url, Unit Width, Unit Height)
					{
						OpenNewWindow(url, Width, Height, true, "_blank");
					}
					
					/// <summary>
					/// Abre uma nova janela no navegador do cliente.
					/// </summary>
					/// <param name="url">Url de destino</param>
					/// <param name="Width">Largura em pixels</param>
					/// <param name="Height">Altura em pixels</param>
					/// <param name="isDialog">Define se a janela será um diálogo</param>
					/// <remarks></remarks>
					public void OpenNewWindow(string url, Unit Width, Unit Height, bool isDialog)
					{
						OpenNewWindow(url, Width, Height, isDialog, "_blank");
					}
					
					/// <summary>
					/// Abre uma nova janela no navegador do cliente.
					/// </summary>
					/// <param name="url">Url de destino</param>
					/// <param name="target">Nome da janela</param>
					/// <remarks></remarks>
					public void OpenNewWindow(string url, string target)
					{
						OpenNewWindow(url, Unit.Empty, Unit.Empty, false, target);
					}
					
					/// <summary>
					/// Abre uma nova janela no navegador do cliente.
					/// </summary>
					/// <param name="url">Url de destino</param>
					/// <param name="target">Nome da janela</param>
					/// <param name="isDialog">Define se a janela será um diálogo</param>
					/// <param name="Width">Largura em pixels</param>
					/// <param name="Height">Altura em pixels</param>
					/// <remarks></remarks>
					public void OpenNewWindow(string url, Unit Width, Unit Height, bool isDialog, string target)
					{
						string alertkey = GetRandomKey();
						string script = GetNewWindowScript(url, Width, Height, isDialog, target);
						if (! SendMagicAjaxScript(Page, script) && ! SendAjaxNetScript(Page, alertkey, script))
						{
							Page.ClientScript.RegisterStartupScript(this.GetType(), alertkey, script, true);
						}
						
					}
					
					/// <summary>
					/// Monta um script para abertura de janelas do navegador do cliente
					/// </summary>
					/// <param name="url">Url de destino</param>
					/// <param name="Width">Largura da janela</param>
					/// <param name="Height">Altura da janela</param>
					/// <param name="isDialog">Define se a janela será um diálogo</param>
					/// <returns>Uma string com código JavaScript 1.2</returns>
					/// <remarks></remarks>
					public string GetNewWindowScript(string url, Unit Width, Unit Height, bool isDialog)
					{
						return GetNewWindowScript(url, Width, Height, isDialog, "_blank");
					}
					
					
					/// <summary>
					/// Monta um script para abertura de janelas do navegador do cliente
					/// </summary>
					/// <param name="url">Url de destino</param>
					/// <param name="Width">Largura da janela</param>
					/// <param name="Height">Altura da janela</param>
					/// <param name="isDialog">Define se a janela será um diálogo</param>
					/// <param name="target">Nome da janela</param>
					/// <returns>Uma string com código JavaScript 1.2</returns>
					/// <remarks></remarks>
					public string GetNewWindowScript(string url, Unit Width, Unit Height, bool isDialog, string target)
					{
						return GetNewWindowScript(url, Unit.Empty, Unit.Empty, Width, Height, isDialog, target);
					}
					
					
					/// <summary>
					/// Monta um script para abertura de janelas do navegador do cliente
					/// </summary>
					/// <param name="url">Url de destino</param>
					/// <param name="Width">Largura da janela</param>
					/// <param name="Height">Altura da janela</param>
					/// <param name="isDialog">Define se a janela será um diálogo</param>
					/// <param name="target">Nome da janela</param>
					/// <returns>Uma string com código JavaScript 1.2</returns>
					/// <remarks></remarks>
					public string GetNewWindowScript(string url, Unit Left, Unit Top, Unit Width, Unit Height, bool isDialog, string target)
					{
						if (string.IsNullOrEmpty(target))
						{
							target = "_blank";
						}
						if (Left.IsEmpty)
						{
							Left = Unit.Parse("90px");
						}
						if (Top.IsEmpty)
						{
							Top = Unit.Parse("90px");
						}
						
						string features = "";
						if (isDialog)
						{
							features += "resizable=0, status=0, toolbar=0, menubar=0, location=0, scrollbars=0";
						}
						else
						{
							features += "resizable=1, status=1, toolbar=0, menubar=0, location=0, scrollbars=1";
						}
						if (! Width.IsEmpty)
						{
							if (! string.IsNullOrEmpty(features))
							{
								features += ", ";
							}
							features += "width=" + Width.Value.ToString();
						}
						if (! Height.IsEmpty)
						{
							if (! string.IsNullOrEmpty(features))
							{
								features += ", ";
							}
							features += "height=" + Height.Value.ToString();
						}
						
						if (! string.IsNullOrEmpty(features))
						{
							features += ", ";
						}
						features += "left=" + Left.Value.ToString();
						
						if (! string.IsNullOrEmpty(features))
						{
							features += ", ";
						}
						features += "top=" + Top.Value.ToString();
						
						
						if (! string.IsNullOrEmpty(features))
						{
							features = ", \"" + features + "\"";
						}
						
						string script = "window.open(\"" + this.Page.ResolveClientUrl(url) + "\", \"" + target + "\"" + features + ");";
						return script;
					}
					
					
					/// <summary>
					/// Tenta fechar a janela atual do browser. Note que alguns navegadores bloqueiam esta ação, principalmente se a janela não tiver sido aberta por um script.
					/// </summary>
					/// <remarks></remarks>
					public void CloseWindow()
					{
						string alertkey = GetRandomKey();
						string script = GetCloseWindowScript();
						if (! SendMagicAjaxScript(Page, script) && ! SendAjaxNetScript(Page, alertkey, script))
						{
							Page.ClientScript.RegisterStartupScript(this.GetType(), alertkey, script, true);
						}
					}
					
					/// <summary>
					/// Retorna um script para fechar a janela.
					/// </summary>
					/// <remarks></remarks>
					public string GetCloseWindowScript()
					{
						return "window.close();";
					}
					
					/// <summary>
					/// Mostra uma janela de confirmação e retorna a resposta do usuário para o servidor.
					/// Para manipular a resposta do usuário, Utilize o evento <see cref="Confirmation">Confirmation</see>.
					/// </summary>
					/// <param name="CommandName">Nome do comando. Utilizado para manipular a resposta.</param>
					/// <param name="Message">Mensagem de confirmação</param>
					/// <remarks></remarks>
					public void Confirm(string CommandName, string Message)
					{
						CommandName = EncodeMessage(CommandName).Replace(":", ";");
						
						string alertkey = GetRandomKey();
						Message = EncodeMessage(Message);
						
						string Script = "var ScriptManager_Confirm = confirm(\"" + Message + "\");" + "\r\n";
						
						PostBackOptions opt = new PostBackOptions(this, "ScriptManager_Confirm.toString()", null, false, false, true, true, false, null);
						Script += " if (ScriptManager_Confirm) " + "\r\n";
						Script += Page.ClientScript.GetPostBackEventReference(this, "confirm:" + CommandName + ":true") + ";" + "\r\n";
						Script += " else " + "\r\n";
						Script += Page.ClientScript.GetPostBackEventReference(this, "confirm:" + CommandName + ":false") + ";" + "\r\n";
						Script += "" + "\r\n";
						
						if (! SendMagicAjaxScript(Page, Script) && ! SendAjaxNetScript(Page, alertkey, Script))
						{
							Page.ClientScript.RegisterStartupScript(this.GetType(), alertkey, Script, true);
						}
					}
					
					
					/// <summary>
					/// Chama um dos eventos definidos na página
					/// </summary>
					/// <param name="eventArgument"></param>
					/// <remarks></remarks>
					void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
					{
						if (eventArgument.StartsWith("confirm:"))
						{
							string[] valores = (eventArgument + ":").Split(':');
							ConfirmationEventArgs e = new ConfirmationEventArgs(valores[1], (valores[2].ToLower() == "true"));
							OnConfirmation(e);
						}
					}
					
					
					/// <summary>
					/// Procura uma instância do scriptManager na página.
					/// </summary>
					/// <returns></returns>
					/// <remarks></remarks>
					[Obsolete(), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]public static ScriptManager GetScriptManager()
					{
						return Current;
					}
					
					/// <summary>
					/// Retorna a instância atual de um ScriptManager
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					public static ScriptManager Current
					{
						get
						{
							HttpContext Context = HttpContext.Current;
							if (Context == null)
							{
								return null;
							}
							Page Page = Context.CurrentHandler as Page;
							if (Page == null)
							{
								return null;
							}
							
							
							
							MasterPage master = Page.Master;
							if (master == null)
							{
								return GetScriptManager(Page.Controls);
							}
							else
							{
								do
								{
									if (master.Master == null)
									{
										break;
									}
									else
									{
										master = master.Master;
									}
								} while (true);
								return GetScriptManager(master.Controls);
							}
							
						}
					}
					
					
					
					
					//Private Shared Function GetScriptManager(ByVal MasterPage As MasterPage) As ScriptManager
					//    If MasterPage Is Nothing Then Return Nothing
					
					//    Dim script As ScriptManager = GetScriptManager(MasterPage.Controls)
					//    If script Is Nothing Then
					//        Return GetScriptManager(MasterPage.Master)
					//    Else
					//        Return script
					//    End If
					
					//End Function
					
					
					private static ScriptManager GetScriptManager(ControlCollection Controls)
					{
						if (Controls == null)
						{
							return null;
						}
						
						foreach (Control i in Controls)
						{
							if (i is ScriptManager)
							{
								return ((ScriptManager) i);
							}
							else
							{
								ScriptManager script = GetScriptManager(i.Controls);
								if (script != null)
								{
									return script;
								}
							}
						}
						return null;
					}
					
					/// <summary>
					/// Envia o script desejado por componentes ajax ou pela página.
					/// </summary>
					/// <param name="script"></param>
					/// <remarks></remarks>
					public void RegisterStartupScript(string script)
					{
						this.RegisterStartupScript(string.Empty, script);
						
					}
					
					
					/// <summary>
					/// Envia o script desejado por componentes ajax ou pela página.
					/// </summary>
					/// <param name="key"></param>
					/// <param name="script"></param>
					/// <remarks></remarks>
					public void RegisterStartupScript(string key, string script)
					{
						RegisterStartupScript(Page, key, script);
					}
					/// <summary>
					/// Envia o script desejado por componentes ajax ou pela página.
					/// </summary>
					/// <param name="script"></param>
					/// <remarks></remarks>
					public static void RegisterStartupScript(Page Page, string script)
					{
						RegisterStartupScript(Page, null, script);
					}
					
					/// <summary>
					/// Envia o script desejado por componentes ajax ou pela página.
					/// </summary>
					/// <param name="key"></param>
					/// <param name="script"></param>
					/// <remarks></remarks>
					public static void RegisterStartupScript(Page Page, string key, string script)
					{
						if (string.IsNullOrEmpty(key))
						{
							key = GetRandomKey();
						}
						
						if (! SendMagicAjaxScript(Page, script) && ! SendAjaxNetScript(Page, key, script))
						{
							Page.ClientScript.RegisterStartupScript(Page.GetType(), key, script, true);
						}
					}
				}
				
			}
		}
	}
	
}
