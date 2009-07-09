/*
using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Runtime.InteropServices;


namespace Tenor
{
	namespace Web
	{
		public partial class TenorModule
		{
			
			
			internal static string ChartToQueryString(System.Web.UI.Page Page, UI.WebControls.Chart Chart)
			{
				
				string url = string.Empty;
				System.Web.UI.WebControls.Chart with_1 = Chart;
				
				foreach (UI.WebControls.ChartItem ci in with_1.ChartItems)
				{
					if (with_1.Type == UI.WebControls.Chart.ChartType.Bar)
					{
						url += "," + ci.PrimaryValue.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ":" + ci.AlternateValue.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ":0";
					}
					else if (with_1.Type == UI.WebControls.Chart.ChartType.Pie)
					{
						url += "," + HttpUtility.UrlEncode(ci.PieLabel.Replace(",", "").Replace(":", "")) + ":" + ci.PrimaryValue.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ":" + (System.Convert.ToInt32(ci.Explode)).ToString();
						if (ci.PieColor.IsEmpty)
						{
							url += ":";
						}
						else
						{
							url += ":" + ColorTranslator.ToHtml(ci.PieColor).Replace("#", "");
						}
					}
				}
				if (url.Length > 0)
				{
					url = url.Substring(1);
					
					url = "c=" + url + "&t=" + (System.Convert.ToInt32(with_1.Type)).ToString();
					
					if (! with_1.ShowLegend)
					{
						url += "&sl=0";
					}
					if (! with_1.ShowTitle)
					{
						url += "&st=0";
					}
					if (with_1.ShowValues)
					{
						url += "&sv=1";
					}
					if (! string.IsNullOrEmpty(with_1.Title) && with_1.Title != "Chart Title")
					{
						url += "&ti=" + HttpUtility.UrlEncode(with_1.Title);
					}
					if (! with_1.OutlineBars)
					{
						url += "&ob=0";
					}
					if (with_1.Thickness != UI.WebControls.Chart.PieThickness.Medium)
					{
						url += "&pt=" + (System.Convert.ToInt32(with_1.Thickness)).ToString();
					}
					if (with_1.LegendValue != UI.WebControls.Chart.LegendValues.None)
					{
						url += "&lv=" + (System.Convert.ToInt32(with_1.LegendValue)).ToString();
					}
					if (! string.IsNullOrEmpty(with_1.LegendValueFormat))
					{
						url += "&lvf=" + HttpUtility.UrlEncode(with_1.LegendValueFormat);
					}
					if (with_1.GridLines != UI.WebControls.Chart.LineStyle.Both)
					{
						url += "&gl=" + (System.Convert.ToInt32(with_1.GridLines)).ToString();
					}
					if (! string.IsNullOrEmpty(with_1.LabelPrimary) && with_1.LabelPrimary != "Label A")
					{
						url += "&lp=" + HttpUtility.UrlEncode(with_1.LabelPrimary);
					}
					if (! string.IsNullOrEmpty(with_1.LabelAlternate) && with_1.LabelAlternate != "Label B")
					{
						url += "&la=" + HttpUtility.UrlEncode(with_1.LabelAlternate);
					}
					
					if (! with_1.ColorPrimary.IsEmpty && with_1.ColorPrimary != System.Drawing.Color.DarkBlue)
					{
						url += "&cp=" + ColorTranslator.ToHtml(with_1.ColorPrimary);
					}
					if (! with_1.ColorAlternate.IsEmpty && with_1.ColorAlternate != Color.CornflowerBlue)
					{
						url += "&ca=" + ColorTranslator.ToHtml(with_1.ColorAlternate);
					}
					
					if (with_1.Diameter != UI.WebControls.Chart.PieDiameter.Medium)
					{
						url += "&di=" + (System.Convert.ToInt32(with_1.Diameter)).ToString();
					}
					
					if (with_1.Rotate != 70)
					{
						url += "&ro=" + with_1.Rotate.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
					}
					if (with_1.Format != UI.WebControls.Chart.ChartFormat.Gif)
					{
						url += "&fo=" + (System.Convert.ToInt32(with_1.Format)).ToString();
					}
					
					
					if (with_1.BackColor != System.Drawing.Color.White)
					{
						url += "&bc=" + ColorTranslator.ToHtml(with_1.BackColor);
					}
					if (with_1.ForeColor != Color.Black)
					{
						url += "&fc=" + ColorTranslator.ToHtml(with_1.ForeColor);
					}
				}
				return url;
			}
			
			
			
			
			
			
			private void ChartRequest(HttpApplication app)
			{
				string[] valores = QueryString("c").Split(',');
				
				bool showlegend = true;
				bool showtitle = true;
				bool showvalues = false;
				bool outlinebars = true;
				string title = "Chart Title";
				string labelprimary = "Primary";
				string labelalternate = "Alternate";
				
				int iValue = 0;
				
				UI.WebControls.Chart.PieDiameter Diameter = UI.WebControls.Chart.PieDiameter.Medium;
				UI.WebControls.Chart.PieThickness Tickness = UI.WebControls.Chart.PieThickness.Medium;
				UI.WebControls.Chart.LegendValues LegendValues = UI.WebControls.Chart.LegendValues.None;
				string LegendValueFormat = null;
				UI.WebControls.Chart.ChartType Type = UI.WebControls.Chart.ChartType.Bar;
				UI.WebControls.Chart.LineStyle Gridlines = UI.WebControls.Chart.LineStyle.Both;
				UI.WebControls.Chart.ChartFormat Format = UI.WebControls.Chart.ChartFormat.Gif;
				
				Color colorPrimary = System.Drawing.Color.DarkBlue;
				Color colorAlternate = Color.CornflowerBlue;
				Color backColor = System.Drawing.Color.White;
				Color foreColor = System.Drawing.Color.Black;
				string sColor = string.Empty;
				
				
				if (QueryString("sl") == "0")
				{
					showlegend = false;
				}
				
				if (QueryString("st") == "0")
				{
					showtitle = false;
				}
				
				if (QueryString("sv") == "1")
				{
					showvalues = true;
				}
				
				if (QueryString("ob") == "0")
				{
					outlinebars = false;
				}
				
				title = QueryString("ti");
				labelprimary = QueryString("lp");
				labelalternate = QueryString("la");
				
				
				
				if (int.TryParse(QueryString("t"), ref iValue))
				{
					Type = (UI.WebControls.Chart.ChartType) iValue;
				}
				if (int.TryParse(QueryString("pt"), ref iValue))
				{
					Tickness = (UI.WebControls.Chart.PieThickness) iValue;
				}
				if (int.TryParse(QueryString("lv"), ref iValue))
				{
					LegendValues = (UI.WebControls.Chart.LegendValues) iValue;
				}
				if (! string.IsNullOrEmpty(QueryString("lvf")))
				{
					LegendValueFormat = QueryString("lvf");
				}
				if (int.TryParse(QueryString("gl"), ref iValue))
				{
					Gridlines = (UI.WebControls.Chart.LineStyle) iValue;
				}
				if (int.TryParse(QueryString("di"), ref iValue))
				{
					Diameter = (UI.WebControls.Chart.PieDiameter) iValue;
				}
				if (int.TryParse(QueryString("fo"), ref iValue))
				{
					Format = (UI.WebControls.Chart.ChartFormat) iValue;
				}
				
				sColor = QueryString("cp");
				if (! string.IsNullOrEmpty(sColor))
				{
					colorPrimary = ColorTranslator.FromHtml(sColor);
				}
				sColor = QueryString("ca");
				if (! string.IsNullOrEmpty(sColor))
				{
					colorAlternate = ColorTranslator.FromHtml(sColor);
				}
				
				sColor = QueryString("bc");
				if (! string.IsNullOrEmpty(sColor))
				{
					backColor = ColorTranslator.FromHtml(sColor);
				}
				sColor = QueryString("fc");
				if (! string.IsNullOrEmpty(sColor))
				{
					foreColor = ColorTranslator.FromHtml(sColor);
				}
				
				ChartRequest(app, valores, Type, Format, showlegend, showtitle, showvalues, outlinebars, title, labelprimary, labelalternate, Diameter, Gridlines, Tickness, LegendValues, LegendValueFormat, colorPrimary, colorAlternate, backColor, foreColor);
			}
			
			
			
			private void ChartRequest(HttpApplication app, string[] valores, UI.WebControls.Chart.ChartType Type, UI.WebControls.Chart.ChartFormat Format, bool showlegend, bool showtitle, bool showvalues, bool outlinebars, string title, string labelprimary, string labelalternate, UI.WebControls.Chart.PieDiameter Diameter, UI.WebControls.Chart.LineStyle Gridlines, UI.WebControls.Chart.PieThickness Tickness, UI.WebControls.Chart.LegendValues LegendValues, string LegendValueFormat, Color colorPrimary, Color colorAlternate, Color backColor, Color foreColor)
			{
				
				
				
				UI.WebControls.Chart chart = new UI.WebControls.Chart();
				
				
				
				chart.Format = Format;
				chart.Type = Type;
				
				
				chart.ShowLegend = showlegend;
				chart.ShowTitle = showtitle;
				chart.ShowValues = showvalues;
				chart.OutlineBars = outlinebars;
				if (! string.IsNullOrEmpty(title))
				{
					chart.Title = title;
				}
				if (! string.IsNullOrEmpty(labelprimary))
				{
					chart.LabelPrimary = labelprimary;
				}
				if (! string.IsNullOrEmpty(labelalternate))
				{
					chart.LabelAlternate = labelalternate;
				}
				chart.Thickness = Tickness;
				chart.LegendValue = LegendValues;
				chart.LegendValueFormat = LegendValueFormat;
				
				chart.ColorPrimary = colorPrimary;
				chart.ColorAlternate = colorAlternate;
				
				chart.GridLines = Gridlines;
				chart.Diameter = Diameter;
				
				chart.BackColor = backColor;
				chart.ForeColor = foreColor;
				
				foreach (string item in valores)
				{
					string[] itemDados = item.Split(':');
					
					if (itemDados.Length >= 3)
					{
						UI.WebControls.ChartItem ci = new UI.WebControls.ChartItem();
						float pValue = 0;
						if (float.TryParse(itemDados[0], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), ref pValue))
						{
							ci.PrimaryValue = pValue;
						}
						else
						{
							ci.PieLabel = itemDados[0];
						}
						
						if (float.TryParse(itemDados[1], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), ref pValue))
						{
							if (Type == UI.WebControls.Chart.ChartType.Pie)
							{
								ci.PrimaryValue = pValue;
							}
							else
							{
								ci.AlternateValue = pValue;
							}
						}
						else
						{
							throw (new ArgumentException("Invalid chart parameters", "c"));
						}
						
						int pBool = 0;
						if (int.TryParse(itemDados[2], ref pBool))
						{
							ci.Explode = System.Convert.ToBoolean(pBool);
						}
						else
						{
							throw (new ArgumentException("Invalid chart parameters", "c"));
						}
						
						Color pPieColor = System.Drawing.Color.Empty;
						if (itemDados.Length >= 4)
						{
							string strCor = itemDados[3];
							try
							{
								pPieColor = ColorTranslator.FromHtml("#" + strCor);
							}
							catch
							{
								
							}
							ci.PieColor = pPieColor;
						}
						
						chart.ChartItems.Add(ci);
					}
				}
				
				
				
				
				MemoryStream @out = (MemoryStream) (chart.WriteContent());
				
				Dados dados = new Dados();
				dados.ContentType = "image/jpeg";
				dados.FileName = "chart";
				dados.ForceDownload = false;
				dados.Object = @out;
				dados.Expires = 0;
				
				this.WriteHeaders(app, dados);
				this.WriteStream(((Stream) dados.Object), app);
				
			}
		}
	}
}
*/
