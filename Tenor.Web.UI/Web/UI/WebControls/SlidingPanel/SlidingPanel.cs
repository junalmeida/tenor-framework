using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
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



namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				
				#region " Delegates And Events "
				public delegate void SlidingPanelItemEventHandler(object sender, SlidingPanelItemEventArgs e);
				
				public class SlidingPanelItemEventArgs : EventArgs
				{
					
					
					public SlidingPanelItemEventArgs(SlidingPanelItem Item)
					{
						_Item = Item;
					}
					
					
					private SlidingPanelItem _Item;
					public SlidingPanelItem Item
					{
						get
						{
							return _Item;
						}
					}
					
				}
				
				public delegate void SlidingPanelCommandEventHandler(object sender, SlidingPanelCommandEventArgs e);
				public class SlidingPanelCommandEventArgs : CommandEventArgs
				{
					
					// Methods
					public SlidingPanelCommandEventArgs(SlidingPanelItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
					{
						_item = item;
						_commandSource = commandSource;
					}
					
					
					// Properties
					public object CommandSource
					{
						get
						{
							return _commandSource;
						}
					}
					
					public SlidingPanelItem Item
					{
						get
						{
							return _item;
						}
					}
					
					
					// Fields
					private object _commandSource;
					private SlidingPanelItem _item;
					
				}
				#endregion
				
				/// <summary>
				/// Controle que exibe um painel com setas de rolagem.
				/// </summary>
				/// <remarks></remarks>
				[ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:SlidingPanel runat=\"server\" />"), ToolboxBitmapAttribute(typeof(System.Web.UI.WebControls.Panel), "Panel.bmp")]public class SlidingPanel : System.Web.UI.WebControls.CompositeDataBoundControl, IPostBackDataHandler
				{
					
					
					
					
					
					
					
					private ArrayList itemsArray = null;
					private SlidingPanelItemCollection itemsCollection = null;
					
					[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description(" Items")]public virtual SlidingPanelItemCollection Items
					{
						get
						{
							if (this.itemsCollection == null)
							{
								if (this.itemsArray == null)
								{
									this.EnsureChildControls();
								}
								this.itemsCollection = new SlidingPanelItemCollection(this.itemsArray);
							}
							return this.itemsCollection;
						}
					}
					
					
					protected override void OnDataBinding(EventArgs e)
					{
						base.OnDataBinding(e);
						this.Controls.Clear();
						base.ClearChildViewState();
						CreateChildControls();
						base.ChildControlsCreated = true;
					}
					
					
					
					protected override System.Web.UI.HtmlTextWriterTag TagKey
					{
						get
						{
							return HtmlTextWriterTag.Div;
						}
					}
					
					protected override string TagName
					{
						get
						{
							return "div";
						}
					}
					
					
					private int _Value;
					private int Value
					{
						get
						{
							return _Value;
						}
						set
						{
							_Value = Math.Abs(value);
						}
					}
					
					
					
					protected override void OnPreRender(System.EventArgs e)
					{
						this.EnsureDataBound();
						Page.RegisterRequiresPostBack(this);
						Page.ClientScript.RegisterHiddenField(this.ClientID + this.ClientIDSeparator + "Value", (Value * - 1).ToString());
						base.OnPreRender(e);
					}
					
					#region " Templates "
					private Style _AlternatingItemStyle;
					[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Description(""), Category("Style")]public Style AlternatingItemStyle
					{
						get
						{
							if (_AlternatingItemStyle == null)
							{
								_AlternatingItemStyle = new Style();
								//If (IsTrackingViewState) Then
								//    _AlternatingItemStyle.TrackViewState()
								//End If
							}
							return _AlternatingItemStyle;
							
						}
					}
					
					
					private ITemplate _AlternatingItemTemplate;
					[Browsable(false), TemplateContainer(typeof(SlidingPanelItem)), PersistenceMode(PersistenceMode.InnerProperty), Description(""), Category("Style")]public ITemplate AlternatingItemTemplate
					{
						get
						{
							return _AlternatingItemTemplate;
						}
						set
						{
							_AlternatingItemTemplate = value;
						}
					}
					
					
					private Style _ItemStyle;
					[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Description(""), Category("Style")]public Style ItemStyle
					{
						get
						{
							if (_ItemStyle == null)
							{
								_ItemStyle = new Style();
								//If (IsTrackingViewState) Then
								//    _ItemStyle.TrackViewState()
								//End If
							}
							return _ItemStyle;
							
						}
					}
					
					
					private ITemplate _ItemTemplate;
					[Browsable(false), TemplateContainer(typeof(SlidingPanelItem)), PersistenceMode(PersistenceMode.InnerProperty), Description(""), Category("Style")]public ITemplate ItemTemplate
					{
						get
						{
							return _ItemTemplate;
						}
						set
						{
							_ItemTemplate = value;
						}
					}
					
					
					
					
					private Style _LeftSlidingStyle;
					[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Description(""), Category("Style")]public Style LeftSlidingStyle
					{
						get
						{
							if (_LeftSlidingStyle == null)
							{
								_LeftSlidingStyle = new Style();
								//If (IsTrackingViewState) Then
								//    _LeftSlidingStyle.TrackViewState()
								//End If
							}
							return _LeftSlidingStyle;
							
						}
					}
					
					
					private ITemplate _LeftSlidingTemplate;
					[Browsable(false), TemplateContainer(typeof(SlidingPanelItem)), PersistenceMode(PersistenceMode.InnerProperty), Description(""), Category("Style")]public ITemplate LeftSlidingTemplate
					{
						get
						{
							return _LeftSlidingTemplate;
						}
						set
						{
							_LeftSlidingTemplate = value;
						}
					}
					
					private Style _RightSlidingStyle;
					[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Description(""), Category("Style")]public Style RightSlidingStyle
					{
						get
						{
							if (_RightSlidingStyle == null)
							{
								_RightSlidingStyle = new Style();
								//If (IsTrackingViewState) Then
								//    _RightSlidingStyle.TrackViewState()
								//End If
							}
							return _RightSlidingStyle;
							
						}
					}
					
					
					private ITemplate _RightSlidingTemplate;
					[Browsable(false), TemplateContainer(typeof(SlidingPanelItem)), PersistenceMode(PersistenceMode.InnerProperty), Description(""), Category("Style")]public ITemplate RightSlidingTemplate
					{
						get
						{
							return _RightSlidingTemplate;
						}
						set
						{
							_RightSlidingTemplate = value;
						}
					}
					#endregion
					
					#region " Propriedades "
					/// <summary>
					/// Controla a orientação deste controle.
					/// Atualmente somente o valor Horizontal é suportado.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Description("Determines the orientation of the SlidingPanel"), Category("Layout"), DefaultValue(typeof(Orientation), "Horizontal")]public Orientation Orientation
					{
						get
						{
							if (ViewState["Orientation"] == null)
							{
								return System.Web.UI.WebControls.Orientation.Horizontal;
							}
							else
							{
								return ((System.Web.UI.WebControls.Orientation) (ViewState["Orientation"]));
							}
						}
						set
						{
							ViewState["Orientation"] = null;
							//somente horizontal até o momento
						}
					}
					
					
					/// <summary>
					/// Ativa ou desativa o efeito de navegação ao usar as setas de rolagem.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Description("Determines if Slidinging animation is on"), Category("Behavior"), DefaultValue(true)]public bool Animation
					{
						get
						{
							if (ViewState["Animation"] == null)
							{
								return true;
							}
							else
							{
								return System.Convert.ToBoolean(ViewState["Animation"]);
							}
							
						}
						set
						{
							if (value)
							{
								ViewState["Animation"] = null;
							}
							else
							{
								ViewState["Animation"] = value;
							}
						}
					}
					
					/// <summary>
					/// Determina quantos pixels deverá rolar ao usar as setas de rolagem. O valor 0 (zero) assume a largura de cada item.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Description("Defines the pixel offset to Sliding when animation is false. Zeros assumes items width."), Category("Behavior"), DefaultValue(0)]public int MovingOffset
					{
						get
						{
							if (ViewState["MovingOffset"] == null)
							{
								return 0;
							}
							else
							{
								return System.Convert.ToInt32(ViewState["MovingOffset"]);
							}
							
						}
						set
						{
							if (value == 0)
							{
								ViewState["MovingOffset"] = null;
							}
							else
							{
								ViewState["MovingOffset"] = Math.Abs(value);
							}
						}
					}
					
					
					#endregion
					
					#region " Events "
					private SlidingPanelItemEventHandler ItemCreatedEvent;
					public event SlidingPanelItemEventHandler ItemCreated
					{
						add
						{
							ItemCreatedEvent = (SlidingPanelItemEventHandler) System.Delegate.Combine(ItemCreatedEvent, value);
						}
						remove
						{
							ItemCreatedEvent = (SlidingPanelItemEventHandler) System.Delegate.Remove(ItemCreatedEvent, value);
						}
					}
					
					
					protected void OnItemCreated(SlidingPanelItemEventArgs e)
					{
						if (ItemCreatedEvent != null)
							ItemCreatedEvent(this, e);
					}
					
					private SlidingPanelItemEventHandler ItemDataBoundEvent;
					public event SlidingPanelItemEventHandler ItemDataBound
					{
						add
						{
							ItemDataBoundEvent = (SlidingPanelItemEventHandler) System.Delegate.Combine(ItemDataBoundEvent, value);
						}
						remove
						{
							ItemDataBoundEvent = (SlidingPanelItemEventHandler) System.Delegate.Remove(ItemDataBoundEvent, value);
						}
					}
					
					
					protected void OnItemDataBound(SlidingPanelItemEventArgs e)
					{
						if (ItemDataBoundEvent != null)
							ItemDataBoundEvent(this, e);
					}
					
					
					private SlidingPanelCommandEventHandler ItemCommandEvent;
					public event SlidingPanelCommandEventHandler ItemCommand
					{
						add
						{
							ItemCommandEvent = (SlidingPanelCommandEventHandler) System.Delegate.Combine(ItemCommandEvent, value);
						}
						remove
						{
							ItemCommandEvent = (SlidingPanelCommandEventHandler) System.Delegate.Remove(ItemCommandEvent, value);
						}
					}
					
					
					protected void OnItemCommand(SlidingPanelCommandEventArgs e)
					{
						if (ItemCommandEvent != null)
							ItemCommandEvent(this, e);
					}
					
					
					protected override bool OnBubbleEvent(object source, System.EventArgs args)
					{
						bool flag = false;
						if (args is SlidingPanelCommandEventArgs)
						{
							this.OnItemCommand((SlidingPanelCommandEventArgs) args);
							flag = true;
						}
						return flag;
					}
					
					#endregion
					
					
					
					
					
					
					private const string DefaultWidth = "300px";
					private const string DefaultHeight = "100px";
					private const string DefaultLeftSlidingWidth = "15px";
					private const string DefaultRightSlidingWidth = "15px";
					
					
					private void SetAction(WebControl Control, int offSet, int max)
					{
						if (Animation)
						{
							Control.Attributes["onclick"] = "SlidingPanel_Move(\'" + ClientID + ClientIDSeparator + "container\', " + offSet.ToString() + ", " + max.ToString() + ", " + Animation.ToString().ToLower() + ")";
						}
						else
						{
							Control.Attributes["onmouseover"] = "SlidingPanel_MovingOn=true;SlidingPanel_Move(\'" + ClientID + ClientIDSeparator + "container\', " + offSet.ToString() + ", " + max.ToString() + ", " + Animation.ToString().ToLower() + ")";
							Control.Attributes["onmouseout"] = "SlidingPanel_MovingOn=false;";
						}
					}
					
					private Panel CreateLeftSliding()
					{
						
						Panel root = new Panel();
						root.ID = "left";
						
						root.ApplyStyle(LeftSlidingStyle);
						root.Height = Unit.Parse("100%");
						root.Style["float"] = "left";
						root.Style["text-align"] = "center";
						root.Style["vertical-align"] = "middle";
						
						if (root.Width.IsEmpty)
						{
							root.Width = Unit.Parse(DefaultLeftSlidingWidth);
						}
						
						if (LeftSlidingTemplate == null)
						{
							Literal lit = new Literal();
							lit.Text = "&lt;";
							root.Controls.Add(lit);
						}
						else
						{
							LeftSlidingTemplate.InstantiateIn(root);
						}
						
						return root;
					}
					
					private Panel CreateRightSliding()
					{
						
						
						Panel root = new Panel();
						root.ID = "right";
						
						root.ApplyStyle(RightSlidingStyle);
						root.Height = Unit.Parse("100%");
						root.Style["float"] = "left";
						root.Style["text-align"] = "center";
						root.Style["vertical-align"] = "middle";
						
						if (root.Width.IsEmpty)
						{
							root.Width = Unit.Parse(DefaultRightSlidingWidth);
						}
						
						if (RightSlidingTemplate == null)
						{
							Literal lit = new Literal();
							lit.Text = "&gt;";
							root.Controls.Add(lit);
						}
						else
						{
							RightSlidingTemplate.InstantiateIn(root);
						}
						
						return root;
					}
					
					
					
					private Panel CreateCenterPanel(Unit Width)
					{
						Panel root = new Panel();
						root.ID = "centerPanel";
						//root.ApplyStyle(LeftSlidingStyle)
						//root.Style(HtmlTextWriterStyle.Margin) = "auto"
						root.Style["float"] = "left";
						root.Style["overflow"] = "hidden";
						root.Height = Unit.Parse("100%");
						root.Width = Width;
						
						return root;
					}
					
					
					
					private void SendScripts()
					{
						System.Web.UI.HtmlControls.HtmlGenericControl obj = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
						obj.EnableViewState = false;
						obj.Attributes["type"] = "text/javascript";
						obj.InnerHtml = "SlidingPanel_SetContentWidth(\"" + ClientID + "\", \"" + ClientIDSeparator + "\");";
						Controls.Add(obj);
						
						Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsSlidingPanel);
						//ScriptManager.RegisterStartupScript(Page, "sliding" & Guid.NewGuid().ToString(), String.Format("SlidingPanel_SetContentWidth('{0}', '{1}');", ClientID, ClientIDSeparator))
					}


                    bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
					{
						string str = postCollection[ClientID + ClientIDSeparator + "Value"];
                        int value = 0;
						if (int.TryParse(str, out value))
						{
                            this.Value = value;
						}
                        return true;
					}
					
					void IPostBackDataHandler.RaisePostDataChangedEvent()
					{
						
					}
					
					
					
					
					protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
					{
						
						if (this.itemsArray != null)
						{
							this.itemsArray.Clear();
						}
						else
						{
							this.itemsArray = new ArrayList();
						}
						
						if (! dataBinding)
						{
							int dataItemCount = System.Convert.ToInt32(ViewState["_!ItemCount"]);
							dataSource = new Data.DummyDataSource(dataItemCount);
							this.itemsArray.Capacity = dataItemCount;
						}
						else if (dataSource == null)
						{
							return 0;
						}
						
						
						
						Panel leftSliding = CreateLeftSliding();
						Panel rightSliding = CreateRightSliding();
						
						Unit width = Unit.Parse("0px");
						if (leftSliding.Width.Type == UnitType.Pixel && rightSliding.Width.Type == UnitType.Pixel)
						{
							if (this.Width.IsEmpty)
							{
								width = Unit.Parse(DefaultWidth);
								width = new Unit(width.Value - leftSliding.Width.Value - rightSliding.Width.Value, UnitType.Pixel);
							}
							else if (width.Type == UnitType.Pixel)
							{
								width = new Unit(this.Width.Value - leftSliding.Width.Value - rightSliding.Width.Value, UnitType.Pixel);
							}
						}
						if (width.Value <= 0)
						{
							throw (new InvalidOperationException("Control, LeftSliding, and RightSliding styles must use the Pixel unit on its width"));
						}
						Panel centerPanel = CreateCenterPanel(width);
						
						Controls.Add(leftSliding);
						Controls.Add(centerPanel);
						Controls.Add(rightSliding);
						
						Panel content = new Panel();
						content.ID = "container";
						content.Height = Unit.Parse("100%");
						content.Style["position"] = "relative";
						content.Style["left"] = "0px";
						
						
						centerPanel.Style["position"] = "relative";
						centerPanel.Controls.Add(content);
						
						
						
						//adicionar os itens no centerpanel agora
						int indice = 0;
						
						
						
						//Dim itens As New List(Of SlidingPanelItem)
						
						
						foreach (object obj in dataSource)
						{
							ListItemType tipo = ListItemType.Item;
							if (indice % 2 != 0)
							{
								tipo = ListItemType.AlternatingItem;
							}
							
							SlidingPanelItem item = new SlidingPanelItem(indice, tipo);
							itemsArray.Add(item);
							Style style = new Style();
							if (tipo == ListItemType.AlternatingItem)
							{
								if (this.AlternatingItemTemplate != null)
								{
									AlternatingItemTemplate.InstantiateIn(item);
								}
								else if (this.ItemTemplate != null)
								{
									ItemTemplate.InstantiateIn(item);
								}
								
								if ((AlternatingItemStyle != null)&& ! AlternatingItemStyle.IsEmpty)
								{
									
									style = AlternatingItemStyle;
								}
								else
								{
									style = ItemStyle;
								}
								
							}
							else if (tipo == ListItemType.Item)
							{
								if (this.ItemTemplate != null)
								{
									ItemTemplate.InstantiateIn(item);
								}
								style = ItemStyle;
							}
							
							
							SlidingPanelItemEventArgs e = new SlidingPanelItemEventArgs(item);
							OnItemCreated(e);
							content.Controls.Add(item);
							
							if (dataBinding)
							{
								item.SetDataItem(obj);
								item.DataBind();
								OnItemDataBound(e);
								item.SetDataItem(null);
							}
							item.SetStyle(style);
							
							indice++;
						}
						int itensCount = itemsArray.Count;
						if (itensCount > 4)
						{
							itensCount = 4;
						}
						Unit defaultItemWidth = ItemStyle.Width;
						if (defaultItemWidth.IsEmpty)
						{
							defaultItemWidth = Unit.Parse(Math.Round(centerPanel.Width.Value / itensCount).ToString() + "px");
						}
						foreach (SlidingPanelItem i in itemsArray)
						{
							if (i.Width.IsEmpty)
							{
								i.Width = defaultItemWidth;
							}
							if (this.DesignMode)
							{
								int indiceItem = itemsArray.IndexOf(i) + 1;
								int rightPixel = (int) (defaultItemWidth.Value * indiceItem);
								if (rightPixel >= centerPanel.Width.Value)
								{
									content.Controls.Remove(i);
								}
							}
						}
						
						
						
						int offset = MovingOffset;
						if (offset == 0)
						{
							offset = (int) defaultItemWidth.Value;
						}
						
						
						
						SetAction(leftSliding, + offset, (int) centerPanel.Width.Value);
						SetAction(rightSliding, - offset, (int) centerPanel.Width.Value);
						
						
						if (dataBinding)
						{
							ViewState["_!ItemCount"] = itemsArray.Count;
						}
						
						
						SendScripts();
						
						return itemsArray.Count;
					}
					
					protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
					{
						
						base.AddAttributesToRender(writer);
						
						if (Width.IsEmpty)
						{
							writer.AddStyleAttribute("width", DefaultWidth);
						}
						if (Height.IsEmpty)
						{
							writer.AddStyleAttribute("height", DefaultHeight);
						}
						writer.AddStyleAttribute("overflow", "hidden");
					}
					
					
				}
				
				
				
			}
		}
	}
	
}
