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







namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				internal class WindowPlasticStyle : WindowStyle
				{
					
					
					public WindowPlasticStyle(Window Window) : base(Window)
					{
					}
					
					
					public readonly Unit CloseButtonWidth = Unit.Parse("17px");
					public readonly Unit CloseButtonHeight = Unit.Parse("17px");
					
					public readonly Unit _BorderSize = Unit.Parse("3px");
					public readonly Unit _TitleBarSize = Unit.Parse("25px");
					
					public override System.Web.UI.WebControls.Unit BorderSize
					{
						get
						{
							return _BorderSize;
						}
					}
					
					public override System.Web.UI.WebControls.Unit TitleBarSize
					{
						get
						{
							return _TitleBarSize;
						}
					}
					
					
					
					protected override void CreateStyle(IStyleSheet StyleSheet, string RootCss)
					{
						IStyleSheet with_1 = StyleSheet;
						CustomStyle window = new CustomStyle();
						window.Style[HtmlTextWriterStyle.BorderCollapse] = "collapse";
						window.Style["border"] = "0";
						window.Style["padding"] = "0";
						window.Style["margin"] = "0";
						
						with_1.CreateStyleRule(window, null, RootCss);
						
						
						
						
						//--- TITLE ----
						CustomStyle title = new CustomStyle();
						title.Style[HtmlTextWriterStyle.Height] = TitleBarSize.ToString();
						title.Style["border"] = "0";
						title.Style["padding"] = "0";
						title.Style["margin"] = "0";
						
						with_1.CreateStyleRule(title, null, RootCss + " tr.title");
						
						
						//title left
						CustomStyle titleLeft = new CustomStyle();
						titleLeft.Style[HtmlTextWriterStyle.Width] = BorderSize.ToString();
						titleLeft.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowTitleLeftGif) + ")";
						titleLeft.Style["border"] = "0";
						titleLeft.Style["padding"] = "0";
						titleLeft.Style["margin"] = "0";
						with_1.CreateStyleRule(titleLeft, null, RootCss + " tr.title td.left");
						
						//title center
						CustomStyle titleCenter = new CustomStyle();
						titleCenter.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowTitleGif) + ")";
						titleCenter.Style["border"] = "0";
						titleCenter.Style["padding"] = "0";
						titleCenter.Style["margin"] = "0";
						with_1.CreateStyleRule(titleCenter, null, RootCss + " tr.title td.center");
						
						//title right
						CustomStyle titleRight = new CustomStyle();
						titleRight.Style[HtmlTextWriterStyle.Width] = BorderSize.ToString();
						titleRight.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowTitleRightGif) + ")";
						titleRight.Style["border"] = "0";
						titleRight.Style["padding"] = "0";
						titleRight.Style["margin"] = "0";
						with_1.CreateStyleRule(titleRight, null, RootCss + " tr.title td.right");
						
						//title text
						with_1.CreateStyleRule(MyWindow.TitleStyle, null, RootCss + " tr.title td.center div.titleText");
						
						//title close
						CustomStyle titleClose = new CustomStyle();
						titleClose.Style[HtmlTextWriterStyle.Overflow] = "hidden";
						
						titleClose.Style["float"] = "right";
						titleClose.Style[HtmlTextWriterStyle.Width] = CloseButtonWidth.ToString();
						titleClose.Style[HtmlTextWriterStyle.Height] = CloseButtonHeight.ToString();
						titleClose.Style[HtmlTextWriterStyle.MarginTop] = "3px";
						titleClose.Style[HtmlTextWriterStyle.MarginRight] = "2px";
						titleClose.Style["border"] = "0";
						titleClose.Style[HtmlTextWriterStyle.Cursor] = "pointer";
						titleClose.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowCloseGif) + ")";
						with_1.CreateStyleRule(titleClose, null, RootCss + " tr.title td.center div.close");
						CustomStyle titleCloseup = new CustomStyle();
						titleCloseup.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowCloseUpGif) + ")";
						with_1.CreateStyleRule(titleCloseup, null, RootCss + " tr.title td.center div.close:hover");
						
						
						
						//--- CONTENT ----
						CustomStyle content = new CustomStyle();
						content.Style["border"] = "0";
						content.Style["padding"] = "0";
						content.Style["margin"] = "0";
						with_1.CreateStyleRule(content, null, RootCss + " tr.content");
						
						
						//content left
						CustomStyle contentLeft = new CustomStyle();
						contentLeft.Style[HtmlTextWriterStyle.Width] = BorderSize.ToString();
						contentLeft.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowLeftGif) + ")";
						contentLeft.Style["border"] = "0";
						contentLeft.Style["padding"] = "0";
						contentLeft.Style["margin"] = "0";
						with_1.CreateStyleRule(contentLeft, null, RootCss + " tr.content td.left");
						
						//content center
						CustomStyle contentCenter = new CustomStyle();
						int newHeight = 100;
						if (! MyWindow.Height.IsEmpty)
						{
							newHeight = (int) MyWindow.Height.Value;
						}
						//.Style(HtmlTextWriterStyle.Height) = CStr(newHeight - TitleBarSize.Value - BorderSize.Value) & "px"
						contentCenter.Style["border"] = "0";
						contentCenter.Style["padding"] = "0";
						contentCenter.Style["margin"] = "0";
						with_1.CreateStyleRule(contentCenter, null, RootCss + " tr.content td.center");
						if (! MyWindow.ContentStyle.IsEmpty)
						{
							with_1.CreateStyleRule(MyWindow.ContentStyle, null, RootCss + " tr.content td.center");
						}
						
						//content center div
						CustomStyle contentCenterDiv = new CustomStyle();
						contentCenterDiv.Style[HtmlTextWriterStyle.Overflow] = "auto";
						contentCenterDiv.Style[HtmlTextWriterStyle.Height] = (newHeight - 50).ToString() + "px";
						with_1.CreateStyleRule(contentCenterDiv, null, RootCss + " tr.content td.center div.content");
						
						//content right
						CustomStyle contentRight = new CustomStyle();
						contentRight.Style[HtmlTextWriterStyle.Width] = BorderSize.ToString();
						contentRight.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowRightGif) + ")";
						contentRight.Style["border"] = "0";
						contentRight.Style["padding"] = "0";
						contentRight.Style["margin"] = "0";
						with_1.CreateStyleRule(contentRight, null, RootCss + " tr.content td.right");
						
						//--- FOOTER ---
						
						
						
						CustomStyle footer = new CustomStyle();
						footer.Style[HtmlTextWriterStyle.Height] = BorderSize.ToString();
						footer.Style["border"] = "0";
						footer.Style["padding"] = "0";
						footer.Style["margin"] = "0";
						with_1.CreateStyleRule(footer, null, RootCss + " tr.footer");
						
						//footer left
						CustomStyle footerLeft = new CustomStyle();
						footerLeft.Style[HtmlTextWriterStyle.Width] = BorderSize.ToString();
						footerLeft.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowBottomLeftGif) + ")";
						footerLeft.Style["border"] = "0";
						footerLeft.Style["padding"] = "0";
						footerLeft.Style["margin"] = "0";
						with_1.CreateStyleRule(footerLeft, null, RootCss + " tr.footer td.left");
						
						//footer center
						CustomStyle footerCenter = new CustomStyle();
						footerCenter.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowBottomGif) + ")";
						footerCenter.Style["border"] = "0";
						footerCenter.Style["padding"] = "0";
						footerCenter.Style["margin"] = "0";
						with_1.CreateStyleRule(footerCenter, null, RootCss + " tr.footer td.center");
						
						//footer right
						CustomStyle footerRight = new CustomStyle();
						footerRight.Style[HtmlTextWriterStyle.Width] = BorderSize.ToString();
						footerRight.Style[HtmlTextWriterStyle.BackgroundImage] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowBottomRightGif) + ")";
						footerRight.Style["border"] = "0";
						footerRight.Style["padding"] = "0";
						footerRight.Style["margin"] = "0";
						with_1.CreateStyleRule(footerRight, null, RootCss + " tr.footer td.right");
						
					}
					
				}
				
				internal abstract class WindowStyle
				{
					
					
					public WindowStyle(Window Window)
					{
						_MyWindow = Window;
						if (_MyWindow == null)
						{
							throw (new ArgumentNullException("Window"));
						}
					}
					
					public abstract Unit BorderSize{
						get;
					}
					public abstract Unit TitleBarSize{
						get;
					}
					
					
					protected abstract void CreateStyle(IStyleSheet StyleSheet, string RootCss);
					
					private Window _MyWindow;
					protected Window MyWindow
					{
						get
						{
							return _MyWindow;
						}
					}
					
					protected string ClientID
					{
						get
						{
							return MyWindow.ClientID;
						}
					}
					
					protected Page Page
					{
						get
						{
							return MyWindow.Page;
						}
					}
					
					
					private string GetRootCss()
					{
						return "#" + ClientID;
					}
					
					
					public void CreateStyle()
					{
						if (Page == null)
						{
							throw (new InvalidOperationException("Cannot access the Page class"));
						}
						if (Page.Header == null)
						{
							throw (new InvalidOperationException("The header tag must be a server control. Set runat attribute of header tag to \'server\'"));
						}
						
						CreateStyle(Page.Header.StyleSheet, GetRootCss());
						
						if (Page.Request.UserAgent.Contains("MSIE"))
						{
							CustomStyle Custom = new CustomStyle();
							Custom.Style["behavior"] = "url(" + Page.ClientScript.GetWebResourceUrl(this.GetType(), Configuration.Resources.WindowCssHoverHtc) + ")";
							Page.Header.StyleSheet.CreateStyleRule(Custom, null, "body");
						}
						
						
					}
				}
				
			}
		}
	}
	
}
