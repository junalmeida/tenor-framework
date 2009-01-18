using System.Diagnostics;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;



namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				
				/// <summary>
				/// Controle de menu do ASP.NET.
				/// </summary>
				/// <remarks></remarks>
				[ToolboxData("<{0}:Menu runat=server></{0}:Menu>")]public class Menu : System.Web.UI.WebControls.Menu
				{
					
					
					private readonly Unit DefaultWidth = new Unit(130, UnitType.Pixel);
					private readonly Unit DefaultHeight = new Unit(20, UnitType.Pixel);
					
					private class CustomStyle : Style
					{
						
						
						public CustomStyle(Style FromStyle)
						{
							this.CopyFrom(FromStyle);
						}
						
						public override void CopyFrom(System.Web.UI.WebControls.Style s)
						{
							this.BackColor = s.BackColor;
							this.BorderColor = s.BorderColor;
							this.BorderStyle = s.BorderStyle;
							this.BorderWidth = s.BorderWidth;
							this.CssClass = s.CssClass;
							this.Font.CopyFrom(s.Font);
							this.ForeColor = s.ForeColor;
							this.Height = s.Height;
							this.Width = s.Width;
						}
						
						
						private Unit _Left;
						public Unit Left
						{
							get
							{
								return _Left;
							}
							set
							{
								_Left = value;
							}
						}
						
						
						private Unit _Top;
						public Unit Top
						{
							get
							{
								return _Top;
							}
							set
							{
								_Top = value;
							}
						}
						
						
						private string _Position;
						public string Position
						{
							get
							{
								return _Position;
							}
							set
							{
								_Position = value;
							}
						}
						
						private string _Visibility;
						public string Visibility
						{
							get
							{
								return _Visibility;
							}
							set
							{
								_Visibility = value;
							}
						}
						
						private string _Float;
						public string Float
						{
							get
							{
								return _Float;
							}
							set
							{
								_Float = value;
							}
						}
						
						private string _ListStyle;
						public string ListStyle
						{
							get
							{
								return _ListStyle;
							}
							set
							{
								_ListStyle = value;
							}
						}
						
						
						private string _Padding;
						public string Padding
						{
							get
							{
								return _Padding;
							}
							set
							{
								_Padding = value;
							}
						}
						
						private string _Margin;
						public string Margin
						{
							get
							{
								return _Margin;
							}
							set
							{
								_Margin = value;
							}
						}
						
						private string _Display;
						public string Display
						{
							get
							{
								return _Display;
							}
							set
							{
								_Display = value;
							}
						}
						
						
						private string _Cursor;
						public string Cursor
						{
							get
							{
								return _Cursor;
							}
							set
							{
								_Cursor = value;
							}
						}
						
						
						public override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer, System.Web.UI.WebControls.WebControl owner)
						{
							if (! string.IsNullOrEmpty(Position))
							{
								writer.AddStyleAttribute(HtmlTextWriterStyle.Position, Position);
							}
							if (! string.IsNullOrEmpty(Visibility))
							{
								writer.AddStyleAttribute(HtmlTextWriterStyle.Visibility, Visibility);
							}
							if (! Left.IsEmpty)
							{
								writer.AddStyleAttribute(HtmlTextWriterStyle.Left, Left.ToString());
							}
							if (! Top.IsEmpty)
							{
								writer.AddStyleAttribute(HtmlTextWriterStyle.Top, Top.ToString());
							}
							
							if (! string.IsNullOrEmpty(Margin))
							{
								writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, Margin.ToString());
							}
							if (! string.IsNullOrEmpty(Padding))
							{
								writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, Padding.ToString());
							}
							if (! string.IsNullOrEmpty(ListStyle))
							{
								writer.AddStyleAttribute("list-style", ListStyle.ToString());
							}
							if (! string.IsNullOrEmpty(Float))
							{
								writer.AddStyleAttribute("float", Float.ToString());
							}
							
							if (! string.IsNullOrEmpty(Display))
							{
								writer.AddStyleAttribute(HtmlTextWriterStyle.Display, Display.ToString());
							}
							
							if (! string.IsNullOrEmpty(Cursor))
							{
								writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, Cursor.ToString());
							}
							
							base.AddAttributesToRender(writer, owner);
						}
						
						protected override void FillStyleAttributes(System.Web.UI.CssStyleCollection attributes, System.Web.UI.IUrlResolutionService urlResolver)
						{
							if (! string.IsNullOrEmpty(Position))
							{
								attributes[HtmlTextWriterStyle.Position] = Position;
							}
							if (! string.IsNullOrEmpty(Visibility))
							{
								attributes[HtmlTextWriterStyle.Visibility] = Visibility;
							}
							if (! Left.IsEmpty)
							{
								attributes[HtmlTextWriterStyle.Left] = Left.ToString();
							}
							if (! Top.IsEmpty)
							{
								attributes[HtmlTextWriterStyle.Top] = Top.ToString();
							}
							
							if (! string.IsNullOrEmpty(Margin))
							{
								attributes[HtmlTextWriterStyle.Margin] = Margin.ToString();
							}
							if (! string.IsNullOrEmpty(Padding))
							{
								attributes[HtmlTextWriterStyle.Padding] = Padding.ToString();
							}
							if (! string.IsNullOrEmpty(ListStyle))
							{
								attributes["list-style"] = ListStyle.ToString();
							}
							if (! string.IsNullOrEmpty(Float))
							{
								attributes["float"] = Float.ToString();
							}
							if (! string.IsNullOrEmpty(Display))
							{
								attributes[HtmlTextWriterStyle.Display] = Display.ToString();
							}
							if (! string.IsNullOrEmpty(Cursor))
							{
								attributes[HtmlTextWriterStyle.Cursor] = Cursor.ToString();
							}
							
							
							
							base.FillStyleAttributes(attributes, urlResolver);
							
							string[] filter = new string[] {"left", "right", "top", "bottom", "width", "height", "margin-left", "margin-right", "margin-top", "margin-bottom"};
							foreach (string f in filter)
							{
								if (! string.IsNullOrEmpty(attributes[f]))
								{
									attributes[f] = attributes[f].Replace(",", ".");
								}
							}
							
						}
						
					}
					
					
					
					protected override void OnPreRender(System.EventArgs e)
					{
						base.OnPreRender(e);
						if (Page.Header == null)
						{
							throw (new Exception("Html Header element not found on page. Set runat attribut of <head> to \'server\'"));
						}
						
						//Control Style
						Style controlstyle = this.CreateControlStyle();
						if (! controlstyle.IsEmpty)
						{
							Page.Header.StyleSheet.CreateStyleRule(controlstyle, null, "#" + ClientID);
						}
						
						
						if (! this.StaticMenuStyle.IsEmpty)
						{
							CustomStyle cssstatic = new CustomStyle(this.StaticMenuStyle);
							
							Page.Header.StyleSheet.CreateStyleRule(cssstatic, null, "#" + ClientID + " .StaticMenuStyle");
						}
						
						
						
						CustomStyle listyle = new CustomStyle(new Style());
						listyle.ListStyle = "none";
						listyle.Position = "relative";
						listyle.Float = "left";
						listyle.Cursor = "default";
						Page.Header.StyleSheet.CreateStyleRule(listyle, null, "#" + ClientID + " .StaticMenuStyle li");
						
						if (Orientation == System.Web.UI.WebControls.Orientation.Vertical)
						{
							CustomStyle vstyle = new CustomStyle(new Style());
							vstyle.Float = "none";
							
							Page.Header.StyleSheet.CreateStyleRule(listyle, null, "#" + ClientID + " .StaticMenuStyle .StaticMenuItemStyle");
						}
						
						CustomStyle ulstyle = new CustomStyle(new Style());
						ulstyle.Display = "block";
						ulstyle.Padding = "0";
						ulstyle.Margin = "0";
						ulstyle.ListStyle = "none";
						ulstyle.Position = "absolute";
						ulstyle.Visibility = "hidden";
						Page.Header.StyleSheet.CreateStyleRule(ulstyle, null, "#" + ClientID + " .StaticMenuStyle ul");
						
						
						
						CustomStyle statmenu = new CustomStyle(this.StaticMenuItemStyle);
						//If statmenu.Width.IsEmpty Then
						//    statmenu.Width = Me.DefaultWidth
						//End If
						if (statmenu.Height.IsEmpty)
						{
							statmenu.Height = this.DefaultHeight;
						}
						Page.Header.StyleSheet.CreateStyleRule(statmenu, null, "#" + ClientID + " .StaticMenuStyle .StaticMenuItemStyle");
						
						if (! StaticHoverStyle.IsEmpty)
						{
							Page.Header.StyleSheet.CreateStyleRule(this.StaticHoverStyle, null, "#" + ClientID + " .StaticMenuStyle .StaticHoverStyle");
						}
						
						
						CustomStyle ul = new CustomStyle(new Style());
						ul.Visibility = "hidden";
						Page.Header.StyleSheet.CreateStyleRule(ul, null, "#" + ClientID + " .StaticMenuStyle .StaticHoverStyle ul ul");
						Page.Header.StyleSheet.CreateStyleRule(ul, null, "#" + ClientID + " .StaticMenuStyle .DynamicHoverStyle ul ul");
						
						
						ul = new CustomStyle(new Style());
						ul.Visibility = "visible";
						string lis = "";
						Page.Header.StyleSheet.CreateStyleRule(ul, null, "#" + ClientID + " .StaticMenuStyle .StaticHoverStyle ul");
						for (int i = 0; i <= 7; i++)
						{
							Page.Header.StyleSheet.CreateStyleRule(ul, null, "#" + ClientID + " .StaticMenuStyle" + lis + " .DynamicHoverStyle ul");
							lis += " li";
						}
						
						
						if (! StaticSelectedStyle.IsEmpty)
						{
							Page.Header.StyleSheet.CreateStyleRule(this.StaticSelectedStyle, null, "#" + ClientID + " .StaticSelectedStyle");
						}
						
						if (! DynamicMenuStyle.IsEmpty)
						{
							Page.Header.StyleSheet.CreateStyleRule(this.DynamicMenuStyle, null, "#" + ClientID + " .DynamicMenuStyle");
						}
						
						CustomStyle dynmenu = new CustomStyle(this.DynamicMenuItemStyle);
						if (dynmenu.Width.IsEmpty)
						{
							dynmenu.Width = statmenu.Width;
						}
						if (dynmenu.Width.IsEmpty)
						{
							dynmenu.Width = DefaultWidth;
						}
						if (dynmenu.Height.IsEmpty)
						{
							dynmenu.Height = statmenu.Height;
						}
						Page.Header.StyleSheet.CreateStyleRule(dynmenu, null, "#" + ClientID + " .DynamicMenuItemStyle");
						
						if (! DynamicHoverStyle.IsEmpty)
						{
							Page.Header.StyleSheet.CreateStyleRule(this.DynamicHoverStyle, null, "#" + ClientID + " .DynamicMenuItemStyle:hover");
							Page.Header.StyleSheet.CreateStyleRule(this.DynamicHoverStyle, null, "#" + ClientID + " .DynamicHoverStyle");
						}
						if (! DynamicSelectedStyle.IsEmpty)
						{
							Page.Header.StyleSheet.CreateStyleRule(this.DynamicSelectedStyle, null, "#" + ClientID + " .DynamicSelectedStyle");
						}
						
						for (int i = 0; i <= LevelMenuItemStyles.Count - 1; i++)
						{
							Page.Header.StyleSheet.CreateStyleRule(this.LevelMenuItemStyles[i], null, "#" + ClientID + " .LevelMenuItemStyles_" + i.ToString());
						}
						for (int i = 0; i <= LevelSelectedStyles.Count - 1; i++)
						{
							Page.Header.StyleSheet.CreateStyleRule(this.LevelSelectedStyles[i], null, "#" + ClientID + " .LevelSelectedStyles_" + i.ToString());
						}
						for (int i = 0; i <= LevelSubMenuStyles.Count - 1; i++)
						{
							Page.Header.StyleSheet.CreateStyleRule(this.LevelSubMenuStyles[i], null, "#" + ClientID + " .LevelSubMenuStyles_" + i.ToString());
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
							
							//TOPMOST DIV
							writer.WriteBeginTag("div");
							writer.WriteAttribute("id", this.ClientID);
							if (! string.IsNullOrEmpty(CssClass))
							{
								writer.WriteAttribute("class", this.CssClass);
							}
							foreach (string key in this.Style.Keys)
							{
								writer.WriteStyleAttribute(key, this.Style[key]);
							}
							foreach (string key in this.Attributes.Keys)
							{
								if (! string.IsNullOrEmpty(this.Attributes[key]))
								{
									writer.WriteAttribute(key, this.Attributes[key]);
								}
							}
							writer.Write(HtmlTextWriter.TagRightChar);
							writer.Indent++;
							writer.WriteLine();
							
							//DIV
							
							writer.WriteBeginTag("ul");
							writer.WriteAttribute("class", "StaticMenuStyle");
							writer.Write(HtmlTextWriter.TagRightChar);
							writer.Indent++;
							writer.WriteLine();
							
							BuildItems(writer, this.Items);
							
							writer.Indent--;
							writer.WriteEndTag("ul");
							writer.WriteLine();
							
							
							//END DIV
							writer.Indent--;
							writer.WriteEndTag("div");
							writer.WriteLine();
							
						}
					}
					
					private void BuildItems(HtmlTextWriter writer, MenuItemCollection Menus)
					{
						foreach (MenuItem item in Menus)
						{
							
							
							//If (item.Depth = 0) Then
							
							//    writer.WriteBeginTag("div")
							//    writer.WriteAttribute("class", "StaticMenuItemStyle")
							
							//    writer.Write(HtmlTextWriter.SpaceChar & "style" & HtmlTextWriter.EqualsChar & HtmlTextWriter.DoubleQuoteChar)
							//    If Not StaticMenuItemStyle.Width.IsEmpty Then
							//        writer.WriteStyleAttribute("width", DefaultWidth.ToString())
							//    End If
							//    writer.Write(HtmlTextWriter.DoubleQuoteChar)
							
							//    writer.Write(HtmlTextWriter.TagRightChar)
							//    writer.Indent += 1
							//    writer.WriteLine()
							
							//Else
							
							writer.WriteBeginTag("li");
							if (item.Depth == 0)
							{
								writer.WriteAttribute("class", "StaticMenuItemStyle");
								
								writer.WriteAttribute("onmouseover", "this.className+=\' StaticHoverStyle\'");
								writer.WriteAttribute("onmouseout", "this.className=this.className.replace(/\\ StaticHoverStyle/ig, \'\');");
							}
							else
							{
								writer.WriteAttribute("class", "DynamicMenuItemStyle LevelMenuItemStyles_" + (item.Depth - 1).ToString());
								
								
								writer.WriteAttribute("onmouseover", "this.className+=\' DynamicHoverStyle\'");
								writer.WriteAttribute("onmouseout", "this.className=this.className.replace(/\\ DynamicHoverStyle/ig, \'\');");
							}
							if (Orientation == System.Web.UI.WebControls.Orientation.Horizontal && item.Depth == 0)
							{
								writer.WriteAttribute("style", "float:left");
							}
							writer.Write(HtmlTextWriter.TagRightChar);
							writer.Indent++;
							writer.WriteLine();
							
							
							
							//End If
							
							if ((item.Depth < StaticDisplayLevels) && (StaticItemTemplate != null))
							{
								MenuItemTemplateContainer container = new MenuItemTemplateContainer(this.Items.IndexOf(item), item);
								StaticItemTemplate.InstantiateIn(container);
								container.DataBind();
								container.RenderControl(writer);
								
							}
							else if (DynamicItemTemplate != null)
							{
								MenuItemTemplateContainer container = new MenuItemTemplateContainer(this.Items.IndexOf(item), item);
								DynamicItemTemplate.InstantiateIn(container);
								container.DataBind();
								container.RenderControl(writer);
							}
							else
							{
								BuildInnerItem(writer, item);
							}
							
							if (item.ChildItems.Count > 0)
							{
								writer.WriteBeginTag("ul");
								writer.WriteAttribute("class", "DynamicMenuStyle");
								
								
								Unit tamanhoH = Unit.Empty;
								if (LevelMenuItemStyles.Count > item.Depth)
								{
									tamanhoH = LevelMenuItemStyles[item.Depth].Height;
								}
								if (tamanhoH.IsEmpty)
								{
									tamanhoH = DynamicMenuItemStyle.Height;
								}
								if (tamanhoH.IsEmpty)
								{
									tamanhoH = StaticMenuItemStyle.Height;
								}
								if (tamanhoH.IsEmpty)
								{
									tamanhoH = DefaultHeight;
								}
								
								string topstyle = "";
								if (item.Depth > 0 || Orientation == System.Web.UI.WebControls.Orientation.Vertical)
								{
									topstyle = "; top:0px"; //& tamanhoH.ToString()
								}
								
								
								
								Unit tamanhoW = Unit.Empty;
								if (LevelMenuItemStyles.Count > item.Depth)
								{
									tamanhoW = LevelMenuItemStyles[item.Depth].Width;
								}
								if (tamanhoW.IsEmpty)
								{
									tamanhoW = DynamicMenuItemStyle.Width;
								}
								if (tamanhoW.IsEmpty)
								{
									tamanhoW = StaticMenuItemStyle.Width;
								}
								if (tamanhoW.IsEmpty)
								{
									tamanhoW = DefaultWidth;
								}
								
								
								//Dim fatorAmpliacao As Integer = item.Depth
								if (Orientation == System.Web.UI.WebControls.Orientation.Horizontal && item.Depth == 0)
								{
									tamanhoW = new Unit(0, tamanhoW.Type);
									
								}
								//fatorAmpliacao += 1
								
								
								writer.WriteAttribute("style", "left:" + tamanhoW.ToString() + topstyle);
								writer.Write(HtmlTextWriter.TagRightChar);
								
								BuildItems(writer, item.ChildItems);
								writer.WriteEndTag("ul");
							}
							
							
							writer.Indent--;
							//If (item.Depth = 0) Then
							//    writer.WriteEndTag("div")
							//Else
							writer.WriteEndTag("li");
							//End If
							writer.WriteLine();
							
							
						}
						
					}
					
					
					
					private void BuildInnerItem(HtmlTextWriter writer, MenuItem Item)
					{
						if (IsLink(Item))
						{
							writer.WriteBeginTag("a");
							if (! string.IsNullOrEmpty(Item.NavigateUrl))
							{
								writer.WriteAttribute("href", Page.Server.HtmlEncode(ResolveClientUrl(Item.NavigateUrl)));
							}
							else
							{
								writer.WriteAttribute("href", Page.ClientScript.GetPostBackClientHyperlink(this, "b" + Item.ValuePath.Replace(PathSeparator.ToString(), "\\"), true));
							}
							
							
							//writer.WriteAttribute("class", GetItemClass(Menu, Item))
							if (! string.IsNullOrEmpty(Item.Target))
							{
								writer.WriteAttribute("target", Item.Target);
							}
							
							if (! string.IsNullOrEmpty(Item.ToolTip))
							{
								writer.WriteAttribute("title", Item.ToolTip);
							}
							else if (! string.IsNullOrEmpty(ToolTip))
							{
								writer.WriteAttribute("title", ToolTip);
							}
							writer.WriteAttribute("style", "display: block");
							
							writer.Write(HtmlTextWriter.TagRightChar);
							writer.Indent++;
							writer.WriteLine();
						}
						else
						{
							writer.WriteBeginTag("span");
							writer.WriteAttribute("style", "display: block");
							//writer.WriteAttribute("class", GetItemClass(Menu, Item))
							writer.Write(HtmlTextWriter.TagRightChar);
							writer.Indent++;
							writer.WriteLine();
						}
						
						
						if (! string.IsNullOrEmpty(Item.ImageUrl))
						{
							writer.WriteBeginTag("img");
							writer.WriteAttribute("src", ResolveClientUrl(Item.ImageUrl));
							writer.WriteAttribute("alt", ((! string.IsNullOrEmpty(Item.ToolTip)) ? Item.ToolTip : ((! string.IsNullOrEmpty(ToolTip)) ? ToolTip : Item.Text)).ToString());
							writer.Write(HtmlTextWriter.SelfClosingTagEnd);
						}
						
						writer.Write(Item.Text);
						
						writer.Indent--;
						if (IsLink(Item))
						{
							writer.WriteEndTag("a");
						}
						else
						{
							writer.WriteEndTag("span");
						}
						
					}
					
					
					private bool IsLink(MenuItem item)
					{
						return (item != null) && item.Enabled && ((! string.IsNullOrEmpty(item.NavigateUrl)) || item.Selectable);
					}
					
				}
			}
		}
	}
	
}
