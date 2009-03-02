using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;



namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				
				/// <summary>
				/// Implementa um controle de exibição de seções em Abas, usando os conceitos da MultiView.
				/// </summary>
				/// <remarks></remarks>
#if !MONO
				[Themeable(true), ControlBuilder(typeof(MultiViewControlBuilder)), DefaultEvent("ActiveViewChanged"), Designer("System.Web.UI.Design.WebControls.MultiViewDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ParseChildren(typeof(TabView)), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), ToolboxData("<{0}:TabsMultiView runat=server></{0}:TabsMultiView>")]
#endif
				public class TabsMultiView : MultiView
				{
					
					
					#region " Default Style Implementation "
					[Browsable(true)]public override string SkinID
					{
						get
						{
							return base.SkinID;
						}
						set
						{
							base.SkinID = value;
						}
					}
					
					
					[TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), Category("Appearance"), Description("WebControl_BackColor")]public virtual System.Drawing.Color BackColor
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return System.Drawing.Color.Empty;
							}
							return this.ControlStyle.BackColor;
						}
						set
						{
							this.ControlStyle.BackColor = value;
						}
					}
					
					[Category("Appearance"), Description("WebControl_BorderColor"), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]public virtual System.Drawing.Color BorderColor
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return System.Drawing.Color.Empty;
							}
							return this.ControlStyle.BorderColor;
						}
						set
						{
							this.ControlStyle.BorderColor = value;
						}
					}
					
					[DefaultValue(0), Category("Appearance"), Description("WebControl_BorderStyle")]public virtual BorderStyle BorderStyle
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return BorderStyle.NotSet;
							}
							return this.ControlStyle.BorderStyle;
						}
						set
						{
							this.ControlStyle.BorderStyle = value;
						}
					}
					
					[Description("WebControl_BorderWidth"), Category("Appearance"), DefaultValue(typeof(Unit), "")]public virtual Unit BorderWidth
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return Unit.Empty;
							}
							return this.ControlStyle.BorderWidth;
						}
						set
						{
							this.ControlStyle.BorderWidth = value;
						}
					}
					
					private Style _ControlStyle;
					[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("WebControl_ControlStyle")]public Style ControlStyle
					{
						get
						{
							if (this._ControlStyle == null)
							{
								this._ControlStyle = this.CreateControlStyle();
								//If MyBase.IsTrackingViewState Then
								//    Me.ControlStyle.TrackViewState()
								//End If
								//If Me._webControlFlags.Item(1) Then
								//    Me._webControlFlags.Clear(1)
								//    Me.ControlStyle.LoadViewState(Nothing)
								//End If
							}
							return this._ControlStyle;
						}
					}
					
					[EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false), Description("WebControl_ControlStyleCreated"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]public bool ControlStyleCreated
					{
						get
						{
							return (this.ControlStyle != null);
						}
					}
					
					[Description("WebControl_CSSClassName"), DefaultValue(""), Category("Appearance")]public string CssClass
					{
						get
						{
							if (ViewState["CssClass"] == null)
							{
								return string.Empty;
							}
							else
							{
								return ViewState["CssClass"].ToString();
							}
						}
						set
						{
							if (string.IsNullOrEmpty(value))
							{
								ViewState["CssClass"] = null;
							}
							else
							{
								ViewState["CssClass"] = value;
							}
						}
					}
					
					
					public void ApplyStyle(Style s)
					{
						if ((s != null) && ! s.IsEmpty)
						{
							this.ControlStyle.CopyFrom(s);
						}
					}
					
					
					protected virtual Style CreateControlStyle()
					{
						return new Style(this.ViewState);
					}
					
					[Description("WebControl_Font"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), Category("Appearance")]public virtual FontInfo Font
					{
						get
						{
							return this.ControlStyle.Font;
						}
					}
					
					[Description("WebControl_ForeColor"), TypeConverter(typeof(WebColorConverter)), Category("Appearance"), DefaultValue(typeof(Color), "")]public virtual System.Drawing.Color ForeColor
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return System.Drawing.Color.Empty;
							}
							return this.ControlStyle.ForeColor;
						}
						set
						{
							this.ControlStyle.ForeColor = value;
						}
					}
					
					
					[Category("Layout"), DefaultValue(typeof(Unit), ""), Description("WebControl_Width")]public virtual Unit Width
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return Unit.Empty;
							}
							return this.ControlStyle.Width;
						}
						set
						{
							this.ControlStyle.Width = value;
						}
					}
					
					[Description("WebControl_Height"), Category("Layout"), DefaultValue(typeof(Unit), "")]public virtual Unit Height
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return Unit.Empty;
							}
							return this.ControlStyle.Height;
						}
						set
						{
							this.ControlStyle.Height = value;
						}
					}
					
					#endregion
					
					#region " Style Properties "
					
					
					
					public virtual Unit ContainerPadding
					{
						get
						{
							if (Unit.Equals(ContainerPaddingLeft, ContainerPaddingRight) && Unit.Equals(ContainerPaddingLeft, ContainerPaddingBottom) && Unit.Equals(ContainerPaddingLeft, ContainerPaddingTop))
							{
								
								return ContainerPaddingLeft;
							}
							else
							{
								return Unit.Empty;
							}
							
						}
						set
						{
							ContainerPaddingLeft = value;
							ContainerPaddingTop = value;
							ContainerPaddingBottom = value;
							ContainerPaddingRight = value;
						}
					}
					
					public virtual Unit ContainerPaddingLeft
					{
						get
						{
							if (ViewState["ContainerPaddingLeft"] == null)
							{
								return Unit.Empty;
							}
							else
							{
								return ((Unit) (ViewState["ContainerPaddingLeft"]));
							}
						}
						set
						{
							if (value.IsEmpty)
							{
								ViewState["ContainerPaddingLeft"] = null;
							}
							else
							{
								ViewState["ContainerPaddingLeft"] = value;
							}
						}
					}
					
					
					public virtual Unit ContainerPaddingTop
					{
						get
						{
							if (ViewState["ContainerPaddingTop"] == null)
							{
								return Unit.Empty;
							}
							else
							{
								return ((Unit) (ViewState["ContainerPaddingTop"]));
							}
						}
						set
						{
							if (value.IsEmpty)
							{
								ViewState["ContainerPaddingTop"] = null;
							}
							else
							{
								ViewState["ContainerPaddingTop"] = value;
							}
						}
					}
					
					
					public virtual Unit ContainerPaddingBottom
					{
						get
						{
							if (ViewState["ContainerPaddingBottom"] == null)
							{
								return Unit.Empty;
							}
							else
							{
								return ((Unit) (ViewState["ContainerPaddingBottom"]));
							}
						}
						set
						{
							if (value.IsEmpty)
							{
								ViewState["ContainerPaddingBottom"] = null;
							}
							else
							{
								ViewState["ContainerPaddingBottom"] = value;
							}
						}
					}
					
					public virtual Unit ContainerPaddingRight
					{
						get
						{
							if (ViewState["ContainerPaddingRight"] == null)
							{
								return Unit.Empty;
							}
							else
							{
								return ((Unit) (ViewState["ContainerPaddingRight"]));
							}
						}
						set
						{
							if (value.IsEmpty)
							{
								ViewState["ContainerPaddingRight"] = null;
							}
							else
							{
								ViewState["ContainerPaddingRight"] = value;
							}
						}
					}
					
					
					
					
					
					
					
					
					/// <summary>
					/// Define qual chave de query string usar para manipular a aba atual do controle.
					/// O padrão é "TabIndex".
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Description("Define qual chave de query string usar para manipular a aba atual do controle."), DefaultValue("TabIndex"), Category("Behavior")]public virtual string QueryStringKey
					{
						get
						{
							string obj2 = this.ViewState["QueryStringKey"].ToString();
							if (! string.IsNullOrEmpty(obj2))
							{
								return obj2;
							}
							else
							{
								return "TabIndex";
							}
						}
						set
						{
							if (string.IsNullOrEmpty(value) || value == "TabIndex")
							{
								this.ViewState["QueryStringKey"] = null;
							}
							else if (value.Contains(" ") || value.Contains("\r") || value.Contains("\n"))
							{
								throw (new ArgumentException("Expression cannot contains spaces or new line characters"));
							}
							else
							{
								this.ViewState["QueryStringKey"] = value;
							}
						}
					}
					
					#endregion
					
					#region " Behavior "
					/// <summary>
					/// Determina se o controle irá definir automáticamente a aba atual pela QueryString da página.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Description("Handle active index from QueryString"), Themeable(false), DefaultValue(true), Category("Behavior")]public virtual bool AutomaticTabChange
					{
						get
						{
							object obj2 = this.ViewState["AutomaticTabChange"];
							if (obj2 != null)
							{
								return System.Convert.ToBoolean(obj2);
							}
							return true;
						}
						set
						{
							this.ViewState["AutomaticTabChange"] = value;
						}
					}
					
					/// <summary>
					/// Retorna ou define o índice da aba atual.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					public override int ActiveViewIndex
					{
						get
						{
							if (! this.DesignMode && AutomaticTabChange)
							{
								int index = 0;
								int.TryParse(Page.Request.QueryString[this.QueryStringKey], out index);
								if (index < 0)
								{
									index = 0;
								}
								if (index > Views.Count - 1)
								{
									index = Views.Count - 1;
								}
								if (index != - 1)
								{
									if (! ((TabView) (Views[index])).TabVisible)
									{
										index = - 1;
									}
								}
								base.ActiveViewIndex = index;
								return index;
								
							}
							else
							{
								return base.ActiveViewIndex;
							}
						}
						set
						{
							base.ActiveViewIndex = value;
						}
					}
					
					#endregion
					
					#region " Tab Style "
					
					private Style _TabStyle;
					
					protected virtual Style CreateTabStyle()
					{
						return new Style();
					}
					
					
					protected virtual Style CreateSelectedTabStyle()
					{
						return new Style();
					}
					
					//Protected Overridable Function CreateContainerStyle() As Style
					//    Return New Style
					//End Function
					
					
					
					//Private _ContainerStyle As Style
					
					//<Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("WebControl_ControlStyle")> _
					//Public ReadOnly Property ContainerStyle() As Style
					//    Get
					//        If (Me._ContainerStyle Is Nothing) Then
					//            Me._ContainerStyle = Me.CreateContainerStyle
					//        End If
					//        Return Me._ContainerStyle
					//    End Get
					//End Property
					
					
					//<TypeConverter(GetType(WebColorConverter)), DefaultValue(GetType(Drawing.Color), ""), Category("Appearance"), Description("BackColor of a Tab Item")> _
					//Public Overridable Property ContainerBackColor() As Drawing.Color
					//    Get
					//        Return Me.ContainerStyle.BackColor
					//    End Get
					//    Set(ByVal value As Color)
					//        Me.ContainerStyle.BackColor = value
					//    End Set
					//End Property
					
					
					
					//<Category("Appearance"), Description("Container BorderColor"), DefaultValue(GetType(Color), ""), TypeConverter(GetType(WebColorConverter))> _
					//Public Overridable Property ContainerBorderColor() As Color
					//    Get
					//        Return Me.ContainerStyle.BorderColor
					//    End Get
					//    Set(ByVal value As Color)
					//        Me.ContainerStyle.BorderColor = value
					//    End Set
					//End Property
					
					
					
					
					//<DefaultValue(0), Category("Appearance"), Description("Container BorderStyle")> _
					//Public Overridable Property ContainerBorderStyle() As BorderStyle
					//    Get
					//        Return Me.ContainerStyle.BorderStyle
					//    End Get
					//    Set(ByVal value As BorderStyle)
					//        Me.ContainerStyle.BorderStyle = value
					//    End Set
					//End Property
					
					//<Description("Container BorderWidth"), Category("Appearance"), DefaultValue(GetType(Unit), "")> _
					//Public Overridable Property ContainerBorderWidth() As Unit
					//    Get
					//        Return Me.ContainerStyle.BorderWidth
					//    End Get
					//    Set(ByVal value As Unit)
					//        Me.ContainerStyle.BorderWidth = value
					//    End Set
					//End Property
					
					
					
					
					
					[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("WebControl_ControlStyle")]public Style TabStyle
					{
						get
						{
							if (this._TabStyle == null)
							{
								this._TabStyle = this.CreateTabStyle();
							}
							return this._TabStyle;
						}
					}
					
					private Style _SelectedTabStyle;
					
					[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("WebControl_ControlStyle")]public Style SelectedTabStyle
					{
						get
						{
							if (this._SelectedTabStyle == null)
							{
								this._SelectedTabStyle = this.CreateSelectedTabStyle();
							}
							return this._SelectedTabStyle;
						}
					}
					
					
					
					[Description(""), DefaultValue(1), Category("Behavior")]public virtual BulletedListDisplayMode TabDisplayMode
					{
						get
						{
							object obj2 = this.ViewState["DisplayMode"];
							if (obj2 != null)
							{
								return ((BulletedListDisplayMode) obj2);
							}
							return BulletedListDisplayMode.HyperLink;
						}
						set
						{
							if ((value < BulletedListDisplayMode.Text) || (value > BulletedListDisplayMode.LinkButton))
							{
								throw (new ArgumentOutOfRangeException("value"));
							}
							this.ViewState["DisplayMode"] = value;
						}
					}
					
					
					[TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), Category("Appearance"), Description("BackColor of a Tab Item")]public virtual System.Drawing.Color TabBackColor
					{
						get
						{
							return this.TabStyle.BackColor;
						}
						set
						{
							this.TabStyle.BackColor = value;
						}
					}
					
					
					[Category("Appearance"), Description("Tab BorderColor"), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]public virtual System.Drawing.Color TabBorderColor
					{
						get
						{
							return this.TabStyle.BorderColor;
						}
						set
						{
							this.TabStyle.BorderColor = value;
						}
					}
					
					
					
					
					[DefaultValue(0), Category("Appearance"), Description("Tab BorderStyle")]public virtual BorderStyle TabBorderStyle
					{
						get
						{
							return this.TabStyle.BorderStyle;
						}
						set
						{
							this.TabStyle.BorderStyle = value;
						}
					}
					
					[Description("Tab BorderWidth"), Category("Appearance"), DefaultValue(typeof(Unit), "")]public virtual Unit TabBorderWidth
					{
						get
						{
							return this.TabStyle.BorderWidth;
						}
						set
						{
							this.TabStyle.BorderWidth = value;
						}
					}
					
					
					
					
					
					
					
					[TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), Category("Appearance"), Description("BackColor of a SelectedTab Item")]public virtual System.Drawing.Color SelectedTabBackColor
					{
						get
						{
							return this.SelectedTabStyle.BackColor;
						}
						set
						{
							this.SelectedTabStyle.BackColor = value;
						}
					}
					
					
					[Category("Appearance"), Description("SelectedTab BorderColor"), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]public virtual System.Drawing.Color SelectedTabBorderColor
					{
						get
						{
							return this.SelectedTabStyle.BorderColor;
						}
						set
						{
							this.SelectedTabStyle.BorderColor = value;
						}
					}
					
					
					
					
					[DefaultValue(0), Category("Appearance"), Description("SelectedTab BorderStyle")]public virtual BorderStyle SelectedTabBorderStyle
					{
						get
						{
							return this.SelectedTabStyle.BorderStyle;
						}
						set
						{
							this.SelectedTabStyle.BorderStyle = value;
						}
					}
					
					[Description("SelectedTab BorderWidth"), Category("Appearance"), DefaultValue(typeof(Unit), "")]public virtual Unit SelectedTabBorderWidth
					{
						get
						{
							return this.SelectedTabStyle.BorderWidth;
						}
						set
						{
							this.SelectedTabStyle.BorderWidth = value;
						}
					}
					
					
					[Description("Tab spacing"), Category("Appearance"), DefaultValue(typeof(Unit), "")]public virtual Unit TabSeparator
					{
						get
						{
							if (ViewState["TabSeparator"] == null)
							{
								return Unit.Empty;
							}
							else
							{
								return ((Unit) (ViewState["TabSeparator"]));
							}
						}
						set
						{
							if (value.IsEmpty)
							{
								ViewState["TabSeparator"] = null;
							}
							else
							{
								ViewState["TabSeparator"] = value;
							}
						}
					}
					
					
					
					
					#endregion
					
					#region " Create Tabs "
					
					//private Tabs _Tabs;
					
					protected virtual Tabs CreateTabs()
					{
						Tabs tabs = new Tabs();
						
						
						tabs.CssClass = ClientID;
						tabs.DisplayMode = TabDisplayMode;
						tabs.ApplyStyle(this.ControlStyle);
						
						tabs.Width = Unit.Empty;
						tabs.Height = Unit.Empty;
						
						tabs.ApplyTabStyle(TabStyle);
						tabs.ApplySelectedTabStyle(SelectedTabStyle);
						tabs.TabSeparator = this.TabSeparator;
						
						TabView activeView = GetCurrentView();
						if (activeView != null)
						{
							if (activeView.BorderStyle != BorderStyle.None && activeView.BorderStyle != BorderStyle.NotSet)
							{
								tabs.BorderColor = activeView.BorderColor;
								tabs.BorderStyle = activeView.BorderStyle;
								tabs.BorderWidth = activeView.BorderWidth;
							}
							else
							{
								tabs.BorderColor = this.TabBorderColor;
								tabs.BorderStyle = this.TabBorderStyle;
								tabs.BorderWidth = this.TabBorderWidth;
							}
						}
						
						
						int i = 1;
						foreach (View view in this.Views)
						{
							TabView tab = view as TabView;
							if (tab.TabVisible)
							{
								
								ListItem item = new ListItem();
								item.Selected = tab == activeView;
								
								
								if (tab != null)
								{
									if (string.IsNullOrEmpty(tab.Title))
									{
										item.Text = "Tab " + i.ToString();
									}
									else
									{
										item.Text = tab.Title;
									}
								}
								else
								{
									item.Text = "Tab " + i.ToString();
								}
								
								switch (TabDisplayMode)
								{
									case BulletedListDisplayMode.HyperLink:
										bool added = false;
										item.Value = Page.Request.AppRelativeCurrentExecutionFilePath + "";
										foreach (string qs in Page.Request.QueryString.Keys)
										{
											if (! item.Value.EndsWith(""))
											{
												item.Value += "&";
											}
											if (qs.Equals(QueryStringKey, StringComparison.OrdinalIgnoreCase))
											{
												item.Value += QueryStringKey + "=" + (i - 1).ToString();
												added = true;
											}
											else
											{
												item.Value += qs + "=" + HttpUtility.UrlEncode(Page.Request.QueryString[qs]);
											}
										}
										if (! added)
										{
											if (! item.Value.EndsWith(""))
											{
												item.Value += "&";
											}
											item.Value += QueryStringKey + "=" + (i - 1).ToString();
										}
										break;
									case BulletedListDisplayMode.LinkButton:
										item.Value = (i - 1).ToString();
										break;
								}
								
								tabs.Items.Add(item);
							}
							
							i++;
						}
						
						return tabs;
					}
					
					#endregion
					
					#region " Rendering "
					
					
					private TabView GetCurrentView()
					{
						if (this.ActiveViewIndex >= 0 && this.ActiveViewIndex < this.Views.Count)
						{
							TabView view = (TabView) (this.Views[this.ActiveViewIndex]);
							return view;
						}
						else
						{
							return null;
						}
					}
					
					private Tabs tabs;
					protected override void OnPreRender(System.EventArgs e)
					{
						base.OnPreRender(e);
						tabs = CreateTabs();
						tabs.CreateStyle(Page, "#" + ClientID + " ul." + ClientID);
					}
					
					public override void RenderControl(System.Web.UI.HtmlTextWriter writer)
					{
						if (this.DesignMode)
						{
							base.RenderControl(writer);
						}
						else
						{
							
							Panel outerDiv = new Panel();
							outerDiv.ID = ClientID;
							outerDiv.CssClass = this.CssClass;
							
							outerDiv.ApplyStyle(ControlStyle);
							if (tabs == null)
							{
								tabs = CreateTabs();
								tabs.CreateStyle(Page, "#" + ClientID + " ul." + ClientID);
							}
							outerDiv.Controls.Add(tabs);
							
							//Dim lClear As New Literal
							//lClear.Text = "<div style=""clear:both""></div>"
							//outerDiv.Controls.Add(lClear)
							
							
							Panel innerDiv = new Panel();
							innerDiv.CssClass = "container";
							
							TabView view = GetCurrentView();
							if (view != null)
							{
								innerDiv.ApplyStyle(view.ControlStyle);
								innerDiv.Style["border-top"] = "0 !important";
								if (view.ControlStyle.BorderStyle == BorderStyle.None || view.ControlStyle.BorderStyle == BorderStyle.NotSet)
								{
									innerDiv.BorderStyle = TabBorderStyle;
									innerDiv.BorderWidth = TabBorderWidth;
									innerDiv.BorderColor = TabBorderColor;
									
								}
								
								//padding: top right bottom left
								if (! ContainerPaddingLeft.IsEmpty)
								{
									innerDiv.Style["padding-left"] = ContainerPaddingLeft.ToString();
								}
								if (! ContainerPaddingRight.IsEmpty)
								{
									innerDiv.Style["padding-right"] = ContainerPaddingLeft.ToString();
								}
								if (! ContainerPaddingTop.IsEmpty)
								{
									innerDiv.Style["padding-top"] = ContainerPaddingLeft.ToString();
								}
								if (! ContainerPaddingBottom.IsEmpty)
								{
									innerDiv.Style["padding-bottom"] = ContainerPaddingLeft.ToString();
								}
							}
							
							innerDiv.ScrollBars = ScrollBars.Auto;
							if (! outerDiv.Height.IsEmpty)
							{
								innerDiv.Height = new Unit(outerDiv.Height.Value - tabs.RealHeight.Value, UnitType.Pixel);
								outerDiv.Height = Unit.Empty;
							}
							//If outerDiv.Height.IsEmpty Then
							//    If innerDiv.Height.IsEmpty Then
							//        innerDiv.Height = New Unit(200 - tabs.RealHeight.Value, UnitType.Pixel)
							//    End If
							//Else
							//    innerDiv.Height = New Unit(outerDiv.Height.Value - tabs.RealHeight.Value, UnitType.Pixel)
							//End If
							
							
							Literal l = new Literal();
							l.Text = "{0}";
							
							innerDiv.Controls.Add(l);
							
							outerDiv.Controls.Add(innerDiv);
							
							
							System.IO.StringWriter htmlSW = new System.IO.StringWriter();
							HtmlTextWriter html32 = new HtmlTextWriter(htmlSW);
							outerDiv.RenderControl(html32);
							string html = htmlSW.ToString();
							
							writer.WriteLine(html.Substring(0, html.IndexOf("{0}")));
							base.RenderControl(writer);
							writer.WriteLine(html.Substring(html.IndexOf("{0}") + 3));
							
							
							
						}
					}
					
					
					#endregion
					
					
				}
				
				#region " Tab View "
				
				public class TabView : View
				{
					
					
					
					#region " Default Style Implementation "
					
					
					[TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), Category("Appearance"), Description("WebControl_BackColor")]public virtual System.Drawing.Color BackColor
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return System.Drawing.Color.Empty;
							}
							return this.ControlStyle.BackColor;
						}
						set
						{
							this.ControlStyle.BackColor = value;
						}
					}
					
					[Category("Appearance"), Description("WebControl_BorderColor"), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]public virtual System.Drawing.Color BorderColor
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return System.Drawing.Color.Empty;
							}
							return this.ControlStyle.BorderColor;
						}
						set
						{
							this.ControlStyle.BorderColor = value;
						}
					}
					
					[DefaultValue(0), Category("Appearance"), Description("WebControl_BorderStyle")]public virtual BorderStyle BorderStyle
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return BorderStyle.NotSet;
							}
							return this.ControlStyle.BorderStyle;
						}
						set
						{
							this.ControlStyle.BorderStyle = value;
						}
					}
					
					[Description("WebControl_BorderWidth"), Category("Appearance"), DefaultValue(typeof(Unit), "")]public virtual Unit BorderWidth
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return Unit.Empty;
							}
							return this.ControlStyle.BorderWidth;
						}
						set
						{
							this.ControlStyle.BorderWidth = value;
						}
					}
					
					private Style _ControlStyle;
					[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("WebControl_ControlStyle")]public Style ControlStyle
					{
						get
						{
							if (this._ControlStyle == null)
							{
								this._ControlStyle = this.CreateControlStyle();
								//If MyBase.IsTrackingViewState Then
								//    Me.ControlStyle.TrackViewState()
								//End If
								//If Me._webControlFlags.Item(1) Then
								//    Me._webControlFlags.Clear(1)
								//    Me.ControlStyle.LoadViewState(Nothing)
								//End If
							}
							return this._ControlStyle;
						}
					}
					
					[EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false), Description("WebControl_ControlStyleCreated"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]public bool ControlStyleCreated
					{
						get
						{
							return (this.ControlStyle != null);
						}
					}
					
					[Description("WebControl_CSSClassName"), DefaultValue(""), Category("Appearance")]public string CssClass
					{
						get
						{
							if (ViewState["CssClass"] == null)
							{
								return string.Empty;
							}
							else
							{
								return ViewState["CssClass"].ToString();
							}
						}
						set
						{
							if (string.IsNullOrEmpty(value))
							{
								ViewState["CssClass"] = null;
							}
							else
							{
								ViewState["CssClass"] = value;
							}
						}
					}
					
					
					public void ApplyStyle(Style s)
					{
						if ((s != null) && ! s.IsEmpty)
						{
							this.ControlStyle.CopyFrom(s);
						}
					}
					
					
					protected virtual Style CreateControlStyle()
					{
						return new Style(this.ViewState);
					}
					
					[Description("WebControl_Font"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), Category("Appearance")]public virtual FontInfo Font
					{
						get
						{
							return this.ControlStyle.Font;
						}
					}
					
					[Description("WebControl_ForeColor"), TypeConverter(typeof(WebColorConverter)), Category("Appearance"), DefaultValue(typeof(Color), "")]public virtual System.Drawing.Color ForeColor
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return System.Drawing.Color.Empty;
							}
							return this.ControlStyle.ForeColor;
						}
						set
						{
							this.ControlStyle.ForeColor = value;
						}
					}
					
					
					[Category("Layout"), DefaultValue(typeof(Unit), ""), Description("WebControl_Width")]public virtual Unit Width
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return Unit.Empty;
							}
							return this.ControlStyle.Width;
						}
						set
						{
							this.ControlStyle.Width = value;
						}
					}
					
					[Description("WebControl_Height"), Category("Layout"), DefaultValue(typeof(Unit), "")]public virtual Unit Height
					{
						get
						{
							if (! this.ControlStyleCreated)
							{
								return Unit.Empty;
							}
							return this.ControlStyle.Height;
						}
						set
						{
							this.ControlStyle.Height = value;
						}
					}
					
					#endregion
					
					
					
					
					
					
					[Description("The tab title of this view"), Category("Appearance")]public string Title
					{
						get
						{
							if (ViewState["Title"] == null)
							{
								return string.Empty;
							}
							else
							{
								return ViewState["Title"].ToString();
							}
						}
						set
						{
							if (string.IsNullOrEmpty(value))
							{
								ViewState["Title"] = null;
							}
							else
							{
								ViewState["Title"] = value;
							}
						}
					}
					
					[Description("Controls when to show this TabView"), Category("Behavior"), DefaultValue(true)]public bool TabVisible
					{
						get
						{
							if (ViewState["TabVisible"] == null)
							{
								return true;
							}
							else
							{
								return System.Convert.ToBoolean(ViewState["TabVisible"]);
							}
						}
						set
						{
							if (value == true)
							{
								ViewState["TabVisible"] = null;
							}
							else
							{
								ViewState["TabVisible"] = value;
							}
						}
					}
					
				}
				
				#endregion
				
				
			}
		}
	}
	
}
