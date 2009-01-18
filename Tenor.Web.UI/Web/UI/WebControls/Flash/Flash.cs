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
using System.IO;


namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				
				/// <summary>
				/// Exibe um Adobe Flash Player na página para exibição de um filme flash.
				/// </summary>
				/// <remarks></remarks>
				[ToolboxItem(typeof(System.Web.UI.Design.WebControlToolboxItem)), ToolboxData("<{0}:Flash runat=\"server\" />"), ToolboxBitmapAttribute(typeof(Flash), Configuration.Resources.FlashBmp)]public class Flash : System.Web.UI.WebControls.WebControl
				{
					
					
					
					public Flash()
					{
					}
					
					#region "TagKey"
					
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
							return TagKey.ToString().ToLower();
						}
					}
					
					
					#endregion
					
					#region "properties"
					
					/// <summary>
					/// Specifies the location (URL) of the movie to be loaded.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Themeable(true), DefaultValue(typeof(FlashRenderMode), "ScriptTag"), Category("Behavior"), Description("Controls how the Flash object is rendered.")]public FlashRenderMode RenderMode
					{
						get
						{
							if (ViewState["FlashMode"] == null)
							{
								return FlashRenderMode.ScriptTag;
							}
							else
							{
								return ((FlashRenderMode) (ViewState["FlashMode"]));
							}
						}
						set
						{
							ViewState["FlashMode"] = value;
						}
					}
					
					/// <summary>
					/// Specifies the location (URL) of the movie to be loaded.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Themeable(true), UrlProperty("*.swf"), Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), DefaultValue(""), Category("Appearance"), Description("Specifies the location (URL) of the movie to be loaded")]public string MovieUrl
					{
						get
						{
							
							if (ViewState["MovieUrl"] == null)
							{
								return string.Empty;
							}
							else
							{
								return ViewState["MovieUrl"].ToString();
							}
						}
						set
						{
							ViewState["MovieUrl"] = value;
						}
					}
					
					/// <summary>
					/// Determines whether to show the flash context menu, allowing the user a variety of options to enhance or control playback.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[BrowsableAttribute(false), Themeable(true), DefaultValue(true), Category("Behavior"), Description("Determines whether to show the flash context menu, allowing the user a variety of options to enhance or control playback.")]public bool ShowMenu
					{
						get
						{
							if (ViewState["ShowMenu"] == null)
							{
								return true;
							}
							else
							{
								return System.Convert.ToBoolean(ViewState["ShowMenu"]);
							}
							
						}
						set
						{
							ViewState["ShowMenu"] = value;
						}
					}
					/// <summary>
					/// Determines whether to allow native full screen mode.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[BrowsableAttribute(false), Themeable(true), DefaultValue(true), Category("Behavior"), Description("Determines whether to allow native full screen mode.")]public bool AllowFullScreen
					{
						get
						{
							if (ViewState["AllowFullScreen"] == null)
							{
								return false;
							}
							else
							{
								return System.Convert.ToBoolean(ViewState["AllowFullScreen"]);
							}
							
						}
						set
						{
							ViewState["AllowFullScreen"] = value;
						}
					}
					
					/// <summary>
					/// Used to send root level variables to the movie. The format of the string is a set of name=value combinations separated by '&amp;'.
					/// </summary>
					/// <value>A System.String</value>
					/// <returns>System.String</returns>
					/// <remarks>For more information on FlashVars, please refer to <a href="http://www.adobe.com/cfusion/knowledgebase/index.cfm?id=tn_16417">FlashVars to pass variables to a SWF</a></remarks>
					[BrowsableAttribute(false), Themeable(true), DefaultValue(""), Category("Data"), Description("Used to send root level variables to the movie. The format of the string is a set of name=value combinations separated by \'&\'.")]public string FlashVars
					{
						get
						{
							if (ViewState["FlashVars"] == null)
							{
								return string.Empty;
							}
							else
							{
								return ViewState["FlashVars"].ToString();
							}
						}
						set
						{
							ViewState["FlashVars"] = value;
						}
					}
					
					/// <summary>
					/// Specifies whether the browser should start Java when loading the Flash Player for the first time. The default value is false if this attribute is omitted. If you use JavaScript and Flash on the same page, Java must be running for the FSCommand to work.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[BrowsableAttribute(false), Themeable(true), DefaultValue(false), Category("Behavior"), Description("Specifies whether the browser should start Java when loading the Flash Player for the first time. The default value is false if this attribute is omitted. If you use JavaScript and Flash on the same page, Java must be running for the FSCommand to work.")]public bool SwLiveConnect
					{
						get
						{
							if (ViewState["SwLiveConnect"] == null)
							{
								return false;
							}
							else
							{
								return System.Convert.ToBoolean(ViewState["SwLiveConnect"]);
							}
						}
						set
						{
							ViewState["SwLiveConnect"] = value;
						}
					}
					
					/// <summary>
					/// Specifies whether the movie begins playing immediately on loading in the browser. The default value is true.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[BrowsableAttribute(false), Themeable(true), DefaultValue(true), Category("Behavior"), Description("Specifies whether the movie begins playing immediately on loading in the browser.")]public bool Play
					{
						get
						{
							if (ViewState["Play"] == null)
							{
								return true;
							}
							else
							{
								return System.Convert.ToBoolean(ViewState["Play"]);
							}
						}
						set
						{
							ViewState["Play"] = value;
						}
					}
					
					
					/// <summary>
					/// Specifies whether the movie repeats indefinitely or stops when it reaches the last frame. The default value is true.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[BrowsableAttribute(false), Themeable(true), DefaultValue(true), Category("Behavior"), Description("Specifies whether the movie repeats indefinitely or stops when it reaches the last frame. The default value is true.")]public bool @Loop
					{
						get
						{
							if (ViewState["Loop"] == null)
							{
								return true;
							}
							else
							{
								return System.Convert.ToBoolean(ViewState["Loop"]);
							}
						}
						set
						{
							ViewState["Loop"] = value;
						}
					}
					
					
					/// <summary>
					/// Specifies the quality mode used to play the flash movie.
					/// </summary>
					/// <value>One of the FlashQuality constants</value>
					/// <returns></returns>
					/// <remarks></remarks>
					[BrowsableAttribute(false), Themeable(true), DefaultValue(typeof(FlashQuality), "Best"), Category("Behavior"), Description("Specifies the quality mode used to play the flash movie.")]public FlashQuality Quality
					{
						get
						{
							if (ViewState["Quality"] == null)
							{
								return FlashQuality.Best;
							}
							else
							{
								return ((FlashQuality) (ViewState["Quality"]));
							}
						}
						set
						{
							ViewState["Quality"] = value;
						}
					}
					
					
					/// <summary>
					/// Specifies the scale mode used to display the flash movie.
					/// </summary>
					/// <value>One of the FlashScale constants</value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Themeable(true), DefaultValue(typeof(FlashScale), "ShowAll"), Category("Behavior"), Description("Specifies the scale mode used to display the flash movie.")]public FlashScale Scale
					{
						get
						{
							if (ViewState["Scale"] == null)
							{
								return FlashScale.ShowAll;
							}
							else
							{
								return ((FlashScale) (ViewState["Scale"]));
							}
						}
						set
						{
							ViewState["Scale"] = value;
						}
					}
					
					/// <summary>
					/// Specifies the alignment used to display the flash movie.
					/// </summary>
					/// <value>One of the FlashScale constants</value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Themeable(true), DefaultValue(typeof(FlashAlign), "Center"), Category("Appearance"), Description("Specifies the alignment used to display the flash movie.")]public FlashAlign Alignment
					{
						get
						{
							if (ViewState["Alignment"] == null)
							{
								return FlashAlign.Center;
							}
							else
							{
								return ((FlashAlign) (ViewState["Alignment"]));
							}
						}
						set
						{
							ViewState["Alignment"] = value;
						}
					}
					
					/// <summary>
					/// Determines whether to generate code to solve transparent issues for expansible banners.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Themeable(true), DefaultValue(false), Category("Behavior"), Description("Determines whether to generate code to solve transparent issues for expansible banners.")]public bool ActAsExpansibleBanner
					{
						get
						{
							if (ViewState["ActAsExpansibleBanner"] == null)
							{
								return false;
							}
							else
							{
								return System.Convert.ToBoolean(ViewState["ActAsExpansibleBanner"]);
							}
						}
						set
						{
							ViewState["ActAsExpansibleBanner"] = value;
						}
					}
					
					/// <summary>
					/// Gets or sets the Width when this control is expanded.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks>When this property has a value, some piece of code id generated to solve browser issues on expansible banners</remarks>
					[Themeable(true), Category("Layout"), Description("Gets or sets the Width when this control is expanded.")]public Unit ExpandedWidth
					{
						get
						{
							if (ViewState["ExpandedWidth"] == null)
							{
								return Unit.Empty;
							}
							else
							{
								return ((Unit) (ViewState["ExpandedWidth"]));
							}
						}
						set
						{
							ViewState["ExpandedWidth"] = value;
						}
					}
					/// <summary>
					/// Gets or sets the Height when this control is expanded.
					/// </summary>
					/// <value></value>
					/// <returns></returns>
					/// <remarks>When this property has a value, some piece of code id generated to solve browser issues on expansible banners</remarks>
					[Themeable(true), Category("Layout"), Description("Gets or sets the Height when this control is expanded.")]public Unit ExpandedHeight
					{
						get
						{
							if (ViewState["ExpandedHeight"] == null)
							{
								return Unit.Empty;
							}
							else
							{
								return ((Unit) (ViewState["ExpandedHeight"]));
							}
						}
						set
						{
							ViewState["ExpandedHeight"] = value;
						}
					}
					
					
					/// <summary>
					/// Determines whether the javascript can access to this component.
					/// </summary>
					/// <value>One of the FlashScriptAccess constants</value>
					/// <returns></returns>
					/// <remarks></remarks>
					[Themeable(true), DefaultValue(typeof(FlashScriptAccess), "Default"), Category("Behavior"), Description("Determines whether the javascript can access to this component.")]public FlashScriptAccess ScriptAccess
					{
						get
						{
							if (ViewState["ScriptAccess"] == null)
							{
								return FlashScriptAccess.Default;
							}
							else
							{
								return ((FlashScriptAccess) (ViewState["ScriptAccess"]));
							}
						}
						set
						{
							ViewState["ScriptAccess"] = value;
						}
					}
					
					
					/// <summary>
					/// Specifies the minimum flash version required
					/// </summary>
					/// <value>One of the Flash Versions</value>
					/// <returns></returns>
					/// <remarks></remarks>
					[BrowsableAttribute(false), Themeable(true), DefaultValue("8.0.0.0"), Category("Behavior"), Description("Specifies the minimum flash version required."), TypeConverter(typeof(Design.FlashVersionConverter))]public string FlashVersion
					{
						get
						{
							if (ViewState["FlashVersion"] == null)
							{
								return "8.0.0.0";
							}
							else
							{
								return System.Convert.ToString(ViewState["FlashVersion"]);
							}
						}
						set
						{
							string res = "";
							string[] newvalue = (value + ".").Split('.');
							foreach (string i in newvalue)
							{
								if (! string.IsNullOrEmpty(i))
								{
									
									int num = 0;
									if (! int.TryParse(i, out num))
									{
										throw (new InvalidExpressionException());
									}
									res += "." + num.ToString();
								}
							}
							
							ViewState["FlashVersion"] = res.Substring(1);
						}
					}
					
					#endregion
					protected override void OnInit(System.EventArgs e)
					{
						Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsFlash);
						
						base.OnInit(e);
					}
					
					
					#region " Rendering "
					
					private string _ScriptLine = string.Empty;
					
					protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
					{
						base.AddAttributesToRender(writer);
					}
					
					
					
					private void WriteParam(HtmlTextWriter writer, string name, string value)
					{
						//escreve uma tag Param dentro do object
						//xhtml compatible
						
						writer.Write(HtmlTextWriter.TagLeftChar + "param");
						writer.WriteAttribute("name", name);
						writer.WriteAttribute("value", value);
						writer.Write(HtmlTextWriter.SelfClosingTagEnd);
						writer.WriteLine();
						
						
						//para a tag embed
						if (name == "movie")
						{
							name = "src";
						}
						writer.AddAttribute(name, value, false);
					}
					
					
					protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
					{
						if (this.DesignMode)
						{
							//Nada aki
						}
						else
						{
							WriteParam(writer, "movie", ResolveClientUrl(MovieUrl));
							// controga wmode e bgcolor de acordo com BackColor
							if (BackColor == System.Drawing.Color.Transparent)
							{
								WriteParam(writer, "wmode", "Transparent");
							}
							else if (BackColor.Equals(System.Drawing.Color.Empty))
							{
								//fazer nada
							}
							else
							{
                                //Conversion.Hex
								string color = this.BackColor.ToArgb().ToString("x2");
								if (! color.Equals("0"))
								{
									WriteParam(writer, "bgcolor", "#" + color.Substring(2));
								}
							}
							
							WriteParam(writer, "menu", ShowMenu.ToString().ToLower());
							
							if (! string.IsNullOrEmpty(FlashVars))
							{
								WriteParam(writer, "flashVars", FlashVars);
							}
							
							if (this.Scale != FlashScale.ShowAll)
							{
								WriteParam(writer, "scale", Scale.ToString().ToLower());
							}
							
							if (! Play)
							{
								WriteParam(writer, "play", "false");
							}
							if (ScriptAccess != FlashScriptAccess.Default)
							{
								WriteParam(writer, "allowScriptAccess", ScriptAccess.ToString().ToLower());
							}
							if (AllowFullScreen)
							{
								WriteParam(writer, "allowFullscreen", AllowFullScreen.ToString().ToLower());
							}
							
							if (! @Loop)
							{
								WriteParam(writer, "loop", "false");
							}
							
							WriteParam(writer, "quality", Quality.ToString().ToLower());
							
							if (this.Alignment != FlashAlign.Center)
							{
								string align = "";
								switch (Alignment)
								{
									case FlashAlign.Bottom:
										align = "b";
										break;
									case FlashAlign.BottomLeft:
										align = "bl";
										break;
									case FlashAlign.BottomRight:
										align = "br";
										break;
									case FlashAlign.Left:
										align = "l";
										break;
									case FlashAlign.Right:
										align = "r";
										break;
									case FlashAlign.Top:
										align = "t";
										break;
									case FlashAlign.TopLeft:
										align = "tl";
										break;
									case FlashAlign.TopRight:
										align = "tr";
										break;
								}
								WriteParam(writer, "salign", align);
							}
							
							if (! Unit.Equals(ExpandedWidth, Unit.Empty))
							{
								writer.AddAttribute("width", this.ExpandedWidth.ToString());
							}
							else
							{
								writer.AddAttribute("width", this.Width.ToString());
							}
							if (! Unit.Equals(ExpandedHeight, Unit.Empty))
							{
								writer.AddAttribute("height", this.ExpandedHeight.ToString());
							}
							else
							{
								writer.AddAttribute("height", this.Height.ToString());
							}
							
							
							writer.AddAttribute(HtmlTextWriterAttribute.Type, "application/x-shockwave-flash");
							writer.AddAttribute("pluginspage", "http://www.macromedia.com/go/getflashplayer");
							writer.RenderBeginTag(HtmlTextWriterTag.Embed);
							writer.RenderEndTag();
							
							
						}
					}
					
					private void RenderFlashObject(HtmlTextWriter writer)
					{
						
						switch (RenderMode)
						{
							case FlashRenderMode.ObjectTag:
								writer.AddAttribute("data", ResolveClientUrl(MovieUrl));
								
								writer.AddAttribute("classid", "clsid:d27cdb6e-ae6d-11cf-96b8-444553540000");
								writer.AddAttribute("codebase", "http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=" + FlashVersion.Replace(".", ","));
								if (! Unit.Equals(ExpandedWidth, Unit.Empty))
								{
									writer.AddAttribute("width", this.ExpandedWidth.ToString());
								}
								else
								{
									writer.AddAttribute("width", this.Width.ToString());
								}
								if (! Unit.Equals(ExpandedHeight, Unit.Empty))
								{
									writer.AddAttribute("height", this.ExpandedHeight.ToString());
								}
								else
								{
									writer.AddAttribute("height", this.Height.ToString());
								}
								
								writer.AddAttribute("type", "application/x-shockwave-flash");
								writer.AddAttribute("viewastext", "viewastext");
								writer.RenderBeginTag(HtmlTextWriterTag.Object);
								RenderContents(writer);
								writer.RenderEndTag();
								break;
								
								
								
							case FlashRenderMode.ScriptTag:
								
								
								
								//Adiciona os atributos da tag object ja gerada pela classe base
								
								if (! Unit.Equals(ExpandedWidth, Unit.Empty))
								{
									_ScriptLine += ", \'" + this.ExpandedWidth.ToString() + "\'";
								}
								else
								{
									_ScriptLine += ", \'" + this.Width.ToString() + "\'";
								}
								if (! Unit.Equals(ExpandedHeight, Unit.Empty))
								{
									_ScriptLine += ", \'" + this.ExpandedHeight.ToString() + "\'";
								}
								else
								{
									_ScriptLine += ", \'" + this.Height.ToString() + "\'";
								}
								
								_ScriptLine += ", \'" + ResolveClientUrl(MovieUrl) + "\'";
								// controga wmode e bgcolor de acordo com BackColor
								if (BackColor == System.Drawing.Color.Transparent)
								{
									_ScriptLine += ", \'#ffffff\'";
									_ScriptLine += ", \'Transparent\'";
								}
								else if (BackColor.Equals(System.Drawing.Color.Empty))
								{
									//fazer nada
									_ScriptLine += ", \'#ffffff\'";
									_ScriptLine += ", \'\'";
								}
								else
								{
                                    //string color = Conversion.Hex(this.BackColor.ToArgb());
                                    string color = this.BackColor.ToArgb().ToString("x2");
                                    if (!color.Equals("0"))
									{
										_ScriptLine += ", \'#" + color.Substring(2) + "\'";
										_ScriptLine += ", \'\'";
									}
								}
								
								_ScriptLine += ", \'" + FlashVars + "\'";
								
								if (this.ScriptAccess == FlashScriptAccess.Default)
								{
									_ScriptLine += ", null";
								}
								else
								{
									_ScriptLine += ", \'" + ScriptAccess.ToString().ToLower() + "\'";
								}
								
								if (AllowFullScreen)
								{
									_ScriptLine += ", true";
								}
								else
								{
									_ScriptLine += ", false";
								}
								
								string scale = "null";
								if (this.Scale != FlashScale.ShowAll)
								{
									scale = "\'" + this.Scale.ToString().ToLower() + "\'";
								}
								_ScriptLine += ", " + scale;
								
								
								
								string align = "null";
								
								if (this.Alignment != FlashAlign.Center)
								{
									switch (Alignment)
									{
										case FlashAlign.Bottom:
											align = "\'b\'";
											break;
										case FlashAlign.BottomLeft:
											align = "\'bl\'";
											break;
										case FlashAlign.BottomRight:
											align = "\'br\'";
											break;
										case FlashAlign.Left:
											align = "\'l\'";
											break;
										case FlashAlign.Right:
											align = "\'r\'";
											break;
										case FlashAlign.Top:
											align = "\'t\'";
											break;
										case FlashAlign.TopLeft:
											align = "\'tl\'";
											break;
										case FlashAlign.TopRight:
											align = "\'tr\'";
											break;
									}
								}
								_ScriptLine += ", " + align;
								
								
								
								_ScriptLine += ");";
								writer.AddAttribute("type", "text/javascript");
								writer.RenderBeginTag(HtmlTextWriterTag.Script);
								writer.WriteLine(_ScriptLine);
								writer.RenderEndTag();
								break;
						}
					}
					
					protected override void Render(System.Web.UI.HtmlTextWriter writer)
					{
						if (! DesignMode)
						{
							System.IO.StringWriter str = new System.IO.StringWriter();
							HtmlTextWriter mwriter = new HtmlTextWriter(str);
							
							
							
							mwriter.AddAttribute("id", ClientID);
							if (! string.IsNullOrEmpty(CssClass))
							{
								mwriter.AddAttribute("class", CssClass);
							}
							if (! string.IsNullOrEmpty(this.Style.Value))
							{
								mwriter.AddAttribute("style", this.Style.Value);
							}
							mwriter.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
							mwriter.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
							
							
							
							_ScriptLine = "writeFlash(\'" + ClientID + "\'";
							
							
							
							if (Unit.Equals(ExpandedWidth, Unit.Empty) && Unit.Equals(ExpandedHeight, Unit.Empty))
							{
								RenderFlashObject(mwriter);
							}
							else
							{
								string w;
								string h;
								if (Unit.Equals(Width, Unit.Empty))
								{
									w = "auto";
								}
								else
								{
									w = Width.ToString();
								}
								if (Unit.Equals(Height, Unit.Empty))
								{
									h = "auto";
								}
								else
								{
									h = Height.ToString();
								}
								
								
								
								
								string exw;
								string exh;
								if (Unit.Equals(ExpandedWidth, Unit.Empty))
								{
									exw = w;
								}
								else
								{
									exw = ExpandedWidth.ToString();
								}
								if (Unit.Equals(ExpandedHeight, Unit.Empty))
								{
									exh = h;
								}
								else
								{
									exh = ExpandedHeight.ToString();
								}
								
								mwriter.AddAttribute("onmouseover", "this.style.clip = \'rect(0px " + exw + " " + exh + " 0px)\';");
								mwriter.AddAttribute("onmouseout", "this.style.clip = \'rect(0px " + w + " " + h + " 0px)\';");
								mwriter.AddStyleAttribute("clip", "rect(0px " + w + " " + h + " 0px)");
								mwriter.RenderBeginTag(HtmlTextWriterTag.Div);
								RenderFlashObject(mwriter);
								mwriter.RenderEndTag();
								
								
								
							}
							writer.Write(str.ToString());
							
						}
						else //DesignMode
						{
							//adiciona os parametro da tag div ja gerada pela classe base
							//este trecho é usado pelo designer
							writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
							writer.AddStyleAttribute("border", "solid 1px gray");
							writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(BackColor));

                            string url = Page.ClientScript.GetWebResourceUrl(typeof(Configuration.Resources), Configuration.Resources.FlashJpg);
							writer.AddAttribute("src", url);
							writer.AddStyleAttribute("background-image", "url(" + url + ")");
							writer.AddStyleAttribute("background-repeat", "no-repeat");
							switch (Alignment)
							{
								case FlashAlign.TopLeft:
									writer.AddStyleAttribute("background-position", "left top");
									break;
								case FlashAlign.TopRight:
									writer.AddStyleAttribute("background-position", "right top");
									break;
								case FlashAlign.Top:
									writer.AddStyleAttribute("background-position", "center top");
									break;
								case FlashAlign.BottomLeft:
									writer.AddStyleAttribute("background-position", "left bottom");
									break;
								case FlashAlign.BottomRight:
									writer.AddStyleAttribute("background-position", "right bottom");
									break;
								case FlashAlign.Bottom:
									writer.AddStyleAttribute("background-position", "center bottom");
									break;
								case FlashAlign.Left:
									writer.AddStyleAttribute("background-position", "left center");
									break;
								case FlashAlign.Right:
									writer.AddStyleAttribute("background-position", "right center");
									break;
								default:
									writer.AddStyleAttribute("background-position", "center");
									break;
							}
							
							base.Render(writer);
						}
						
					}
					
					/// <summary>
					/// Em BETA
					/// </summary>
					/// <returns></returns>
					/// <remarks></remarks>
					private string GetACOEScript()
					{
						string w;
						string h;
						if (Unit.Equals(Width, Unit.Empty))
						{
							w = "auto";
						}
						else
						{
							w = Width.ToString();
						}
						if (Unit.Equals(Height, Unit.Empty))
						{
							h = "auto";
						}
						else
						{
							h = Height.ToString();
						}
						string exw;
						string exh;
						if (Unit.Equals(ExpandedWidth, Unit.Empty))
						{
							exw = w;
						}
						else
						{
							exw = ExpandedWidth.ToString();
						}
						if (Unit.Equals(ExpandedHeight, Unit.Empty))
						{
							exh = h;
						}
						else
						{
							exh = ExpandedHeight.ToString();
						}
						
						
						System.Text.StringBuilder res = new System.Text.StringBuilder();
						res.AppendLine("AC_FL_RunContent(");
						res.AppendLine("    \"src\", \"" + ResolveClientUrl(MovieUrl) + "\",");
						res.AppendLine("    \"width\", \"" + exw + "\",");
						res.AppendLine("    \"height\", \"" + exh + "\",");
						res.AppendLine("    \"id\", \"" + ClientID + "\",");
						res.AppendLine("    \"quality\", \"example\",");
						res.AppendLine("    \"bgcolor\", \"example\",");
						res.AppendLine("    \"wmode\", \"example\",");
						res.AppendLine("    \"allowScriptAccess\", \"example\",");
						res.AppendLine("    \"type\", \"application/x-shockwave-flash\",");
						res.AppendLine("    \"codebase\", \"http://fpdownload.macromedia.com/get/flashplayer/current/swflash.cab\",");
						res.AppendLine("    \"pluginspage\", \"http://www.adobe.com/go/getflashplayer\",");
						res.AppendLine(");");
						
						return res.ToString();
					}
					
					protected override void OnPreRender(System.EventArgs e)
					{
						base.OnPreRender(e);
					}
					
					#endregion
					
					#region " disabled properties "
					
					[BrowsableAttribute(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is not available on Flash objects", true)]public override System.Drawing.Color BorderColor
					{
						get
						{
							return base.BorderColor;
						}
						set
						{
							base.BorderColor = System.Drawing.Color.Empty;
						}
					}
					
					[BrowsableAttribute(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is not available on Flash objects", true)]public override System.Web.UI.WebControls.BorderStyle BorderStyle
					{
						get
						{
							return base.BorderStyle;
						}
						set
						{
							base.BorderStyle = System.Web.UI.WebControls.BorderStyle.NotSet;
						}
					}
					
					[BrowsableAttribute(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is not available on Flash objects", true)]
                    public override System.Web.UI.WebControls.Unit BorderWidth
					{
						get
						{
							return base.BorderWidth;
						}
						set
						{
							base.BorderWidth = Unit.Empty;
						}
					}
					
					
					[BrowsableAttribute(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is not available on Flash objects", true)]
                    public override System.Web.UI.WebControls.FontInfo Font
					{
						get
						{
							return base.Font;
						}
					}
					
					[BrowsableAttribute(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is not available on Flash objects", true)]
                    public override System.Drawing.Color ForeColor
					{
						get
						{
							return base.ForeColor;
						}
						set
						{
							base.ForeColor = System.Drawing.Color.Empty;
						}
					}
					
					#endregion
					
				}
				
				public enum FlashScriptAccess
				{
					@Default,
					Always,
					SameDomain
				}
				
				public enum FlashAlign
				{
					/// <summary>
					/// Align the movie on the center of the browser window.
					/// </summary>
					/// <remarks></remarks>
					Center,
					/// <summary>
					/// Align the movie along the left edge of the browser window and crop the remaining three sides as needed.
					/// </summary>
					/// <remarks></remarks>
					Left,
					/// <summary>
					/// Align the movie along the top edge of the browser window and crop the remaining three sides as needed.
					/// </summary>
					/// <remarks></remarks>
					Top,
					/// <summary>
					/// Align the movie along the right edge of the browser window and crop the remaining three sides as needed.
					/// </summary>
					/// <remarks></remarks>
					Right,
					/// <summary>
					/// Align the movie along the bottom edge of the browser window and crop the remaining three sides as needed.
					/// </summary>
					/// <remarks></remarks>
					Bottom,
					/// <summary>
					/// Align the movie to the top left corner of the browser window and crop the bottom and remaining right and bottom side as needed.
					/// </summary>
					/// <remarks></remarks>
					TopLeft,
					/// <summary>
					/// Align the movie to the top right corner of the browser window and crop the bottom and remaining left and bottom side as needed.
					/// </summary>
					/// <remarks></remarks>
					TopRight,
					/// <summary>
					/// Align the movie to the bottom left corner of the browser window and crop the bottom and remaining right and top side as needed.
					/// </summary>
					/// <remarks></remarks>
					BottomLeft,
					/// <summary>
					/// Align the movie to the bottom right corner of the browser window and crop the bottom and remaining left and top side as needed.
					/// </summary>
					/// <remarks></remarks>
					BottomRight
				}
				
				public enum FlashScale
				{
					/// <summary>
					/// Makes the entire movie visible in the specified area without distortion, while maintaining the original aspect ratio of the movie. Borders may appear on two sides of the movie.
					/// </summary>
					/// <remarks></remarks>
					ShowAll,
					/// <summary>
					/// Scales the movie to fill the specified area, without distortion but possibly with some cropping, while maintaining the original aspect ratio of the movie.
					/// </summary>
					/// <remarks></remarks>
					NoBorder,
					/// <summary>
					/// Makes the entire movie visible in the specified area without trying to preserve the original aspect ratio. Distortion may occur.
					/// </summary>
					/// <remarks></remarks>
					ExactFit,
					/// <summary>
					/// Shows the movie "as is"
					/// </summary>
					/// <remarks></remarks>
					NoScale
				}
				
				public enum FlashQuality
				{
					/// <summary>
					/// Favors playback speed over appearance and never uses anti-aliasing.
					/// </summary>
					/// <remarks></remarks>
					Low,
					/// <summary>
					/// Emphasizes speed at first but improves appearance whenever possible. Playback begins with anti-aliasing turned off. If the Flash Player detects that the processor can handle it, anti-aliasing is turned on.
					/// </summary>
					/// <remarks></remarks>
					Autolow,
					/// <summary>
					/// Emphasizes playback speed and appearance equally at first but sacrifices appearance for playback speed if necessary. Playback begins with anti-aliasing turned on. If the actual frame rate drops below the specified frame rate, anti-aliasing is turned off to improve playback speed. Use this setting to emulate the View > Antialias setting in Flash.
					/// </summary>
					/// <remarks></remarks>
					Autohigh,
					/// <summary>
					/// Applies some anti-aliasing and does not smooth bitmaps. It produces a better quality than the Low setting, but lower quality than the High setting.
					/// </summary>
					/// <remarks></remarks>
					Medium,
					/// <summary>
					/// Favors appearance over playback speed and always applies anti-aliasing. If the movie does not contain animation, bitmaps are smoothed; if the movie has animation, bitmaps are not smoothed.
					/// </summary>
					/// <remarks></remarks>
					High,
					/// <summary>
					/// Provides the best display quality and does not consider playback speed. All output is anti-aliased and all bitmaps are smoothed.
					/// </summary>
					/// <remarks></remarks>
					Best
				}
				
				public enum FlashRenderMode
				{
					/// <summary>
					/// Uses javascript to render the Flash object. This is the default behavior.
					/// </summary>
					/// <remarks></remarks>
					ScriptTag,
					/// <summary>
					/// Uses standard object tag.
					/// </summary>
					/// <remarks></remarks>
					ObjectTag
				}
			}
		}
	}
	
}
