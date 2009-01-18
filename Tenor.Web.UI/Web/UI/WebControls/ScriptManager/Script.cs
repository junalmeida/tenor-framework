using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
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



namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				
				[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]public class ScriptCollection : CollectionBase
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
							Add((Script) i);
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
							return ((Script) (List[Index]));
						}
						set
						{
							List[Index] = value;
						}
					}
					
					
					#endregion
				}
				
				/// <summary>
				/// Classe que representa um script que adicionará uma funcionalidade na página.
				/// </summary>
				/// <remarks></remarks>
				[TypeConverter(typeof(Design.ScriptTypeConverter))]public abstract class Script : object
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
				/// Adiciona à página scripts de máscaras.
				/// </summary>
				/// <remarks></remarks>
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
				
				
				///' <summary>
				///' Adiciona à página os scripts do SIOD+Tracking.
				///' Você pode usar esse script para obter os IDs gerados pelo SIOD.
				///' </summary>
				///' <remarks></remarks>
				//Public NotInheritable Class Tracking
				//    Inherits Script
				
				
				//    Public Sub New()
				//        MyBase.New()
				//    End Sub
				
				
				//    ''' <summary>
				//    ''' Método chamado na inicialização do componente.
				//    ''' </summary>
				//    ''' <param name="Page"></param>
				//    ''' <remarks></remarks>
				//    Public Overrides Sub Initialize(ByVal Page As Page)
				
				
				//        If Page.Header Is Nothing Then
				//            Throw New InvalidOperationException("Header tag must be a server control.")
				//        End If
				
				//        Dim trackingfound As Boolean = False
				//        For i As Integer = 0 To Page.Header.Controls.Count - 1
				//            Select Case True
				//                Case TypeOf Page.Header.Controls(i) Is LiteralControl
				//                    Dim lit As LiteralControl = CType(Page.Header.Controls(i), LiteralControl)
				//                    If lit.Text.Contains("""" & Configuration.Tracking.TrackingUrl & """") Then
				//                        trackingfound = True
				//                        Exit For
				//                    End If
				//            End Select
				//        Next
				//        Page.ClientScript.RegisterHiddenField(Configuration.Tracking.HiddenField, String.Empty)
				
				
				//        If Not trackingfound Then
				
				//            If Page.Header Is Nothing Then
				//                Throw New InvalidOperationException("Header tag must be a server control.")
				//            End If
				
				//            Dim script As New Literal()
				//            script.Text += Environment.NewLine + "<script src=""" + Configuration.Tracking.TrackingUrl + """ type=""text/javascript""></script>"
				//            Page.Header.Controls.Add(script)
				
				//            'Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "tracking", "<script src=""" & TrackingUrl & """></script>", False)
				//        End If
				//        Page.ClientScript.RegisterClientScriptResource(Me.GetType(), Configuration.Resources.JsTracking)
				//        Web.UI.WebControls.ScriptManager.Current.RegisterStartupScript("Tracking_Set", String.Format("Tracking_Set('{0}');" & vbCrLf, Configuration.Tracking.HiddenField))
				//        'Page.ClientScript.RegisterStartupScript(Me.GetType(), Me.GetType().Name, String.Format("Tracking_Set('{0}');" & vbCrLf, HiddenField), True)
				//    End Sub
				
				//    Private ReadOnly Property SIOD() As String()
				//        Get
				//            Dim context As HttpContext = HttpContext.Current
				//            Dim request As HttpRequest = Nothing
				//            If context IsNot Nothing Then
				//                request = context.Request
				//            End If
				//            If request IsNot Nothing Then
				//                Dim values As String() = (";" & request.Form(Configuration.Tracking.HiddenField) & ";").Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
				//                Return values
				
				//            End If
				//            Return New String() {}
				//        End Get
				//    End Property
				
				
				//    ''' <summary>
				//    ''' Retorna a origem desta visita.
				//    ''' </summary>
				//    ''' <value></value>
				//    ''' <returns></returns>
				//    ''' <remarks></remarks>
				//    Public ReadOnly Property Origem() As String
				//        Get
				//            If SIOD.Length >= 2 Then
				//                Return SIOD(0)
				//            Else
				//                Return String.Empty
				//            End If
				//        End Get
				//    End Property
				
				
				//    ''' <summary>
				//    ''' Retorna o número da visita
				//    ''' </summary>
				//    ''' <value></value>
				//    ''' <returns></returns>
				//    ''' <remarks></remarks>
				//    Public ReadOnly Property Visita() As Long
				//        Get
				//            If SIOD.Length >= 2 Then
				//                Dim res As Long = 0
				//                Long.TryParse(SIOD(1), res)
				//                Return res
				//            Else
				//                Return 0
				//            End If
				//        End Get
				//    End Property
				
				//End Class
				
				/// <summary>
				/// Adiciona à página scripts para bloquear a seleção de texto.
				/// </summary>
				/// <remarks></remarks>
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
				/// Adiciona scripts de bloqueio do click com o botão direito do mouse.
				/// </summary>
				/// <remarks></remarks>
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
				/// Adiciona à página scripts para consertar bugs de CSS do IE versões abaixo do 7.
				/// </summary>
				/// <remarks></remarks>
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
						
						//script.Text = Environment.NewLine + "<!--[if lt IE 7]>"
						//script.Text += Environment.NewLine + "<script src=""" + Page.ResolveUrl("~/" + Util.HandlerFileName) + "/iefix/IE7.js" + """ type=""text/javascript""></script>"
						//script.Text += Environment.NewLine + "<![endif]-->"
						
						Page.Header.Controls.Add(script);
					}
				}
				
				
			}
		}
	}
	
	
}
