using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
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
			
			
			private string[] Fonts = new string[] {"Brush Script MT", "Comic Sans MS", "Times New Roman"};
			private int[] FontSize = new int[] {14, 10, 10};
			private const string vTestes = "Versão de Testes";
			
			private void CorAleatoria(ref Color ForeColor, ref Color BackColor)
			{
				
				
				List<int> seeds = new List<int>();
				for (int i = 0; i <= 2; i++)
				{
					string str = Guid.NewGuid().ToString();
					if (str.Length > 10)
					{
						str = str.Substring(0, 10);
					}
					foreach (char c in str.ToCharArray())
					{
						if (c == '-' || ((int)c >= (int)'a' && (int)c <= (int)'z') || ((int)c >= (int)'A' && (int)c <= (int)'Z'))
						{
							str = str.Replace(c.ToString(), "");
						}
					}
					seeds.Add(int.Parse(str));
				}
				
				
				Random rnd = new Random(seeds[0]);
				int r = rnd.Next(0, 255);
				rnd = new Random(seeds[1]);
				int g = rnd.Next(0, 255);
				rnd = new Random(seeds[2]);
				int b = rnd.Next(0, 255);
				
				
				ForeColor = Color.FromArgb(255, r, g, b);
				BackColor = Color.FromArgb(127, 255 - r, 255 - g, 255 - b);
			}
			
			private void DynamicImageRequest(HttpApplication App, string ButtonText)
			{
				
				if (App == null)
				{
					throw (new ArgumentNullException("App"));
				}
				
				int width = 0;
				int height = 0;
				bool active = false;
				int.TryParse(QueryString("w"), out width);
				int.TryParse(QueryString("h"), out height);
				if (true == QueryString("st").Contains("on"))
				{
					active = true;
				}
				else
				{
					active = false;
				}
				
				
				
				Font font = null;
				for (int i = 0; i <= Fonts.Length - 1; i++)
				{
					try
					{
						FontFamily ff = new FontFamily(Fonts[i]);
						font = new Font(ff, FontSize[i], FontStyle.Italic, GraphicsUnit.Point);
						
						goto endOfForLoop;
					}
					catch (Exception)
					{
					}
				}
endOfForLoop:
				if (font == null)
				{
					throw (new InvalidOperationException("Invalid font."));
				}
				
				
				
				Bitmap img;
				Graphics gr;
				img = new Bitmap(115, 30);
				gr = Graphics.FromImage(img);
				SizeF textSize = gr.MeasureString(ButtonText, font);
				gr.Dispose();
				img.Dispose();
				
				if (width <= 0 || height <= 0)
				{
					if (width <= 0)
					{
						width = (int) (textSize.Width + 30);
						if (width < 104)
						{
							width = 104;
						}
					}
					if (height <= 0)
					{
						height = (int) (textSize.Height + 5 + 5);
					}
				}
				
				
				img = new Bitmap(width, height);
				gr = Graphics.FromImage(img);
				
				
				gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
				
				RectangleF rect = new RectangleF(0, 0, width, height);
				Pen pen = new Pen(ColorTranslator.FromHtml("#999999"));
				System.Drawing.Drawing2D.LinearGradientBrush brush;
				
				
				string[] colorsStart = new string[] {"#FF00FF", "#00FF00", "#00FFFF"};
				string[] colorsEnd = new string[] {"#00FF00", "#FF00FF", "#FF0000"};
				
				Random rnd = new Random();
				int indice = rnd.Next(0, 2);
				
				if (active)
				{
					brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, ColorTranslator.FromHtml(colorsStart[indice]), ColorTranslator.FromHtml(colorsEnd[indice]), System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
				}
				else
				{
					brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, ColorTranslator.FromHtml(colorsEnd[indice]), ColorTranslator.FromHtml(colorsStart[indice]), System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
				}
				gr.FillRectangle(brush, rect);
				gr.DrawRectangle(pen, 0, 0, width, height);
				
				
				PointF point = new PointF();
				point.X = System.Convert.ToSingle((width / 2) - (textSize.Width / 2));
				point.Y = System.Convert.ToSingle(((height - 5) / 2) - (textSize.Height / 2));
				
				VersaoTestes(gr, height);
				
				gr.DrawString(ButtonText, font, Brushes.White, point);
				gr.DrawString(ButtonText, font, Brushes.Black, point.X - 1, point.Y - 1);
				
				gr.DrawRectangle(new Pen(System.Drawing.Color.Cyan, 3), new Rectangle(0, 0, width - 1, height - 1));
				
				
				gr.Dispose();
				MemoryStream mem = new MemoryStream();
				
				img.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
				img.Dispose();
				
				
				
				Dados dados = new Dados();
				dados.ContentType = "image/png";
				dados.ContentLength = mem.Length;
				WriteHeaders(App, dados);
				WriteStream(mem, App);
			}
			
			private void VersaoTestes(Graphics gr, int height)
			{
				
				Font font = new Font(new FontFamily("Arial"), 9, FontStyle.Bold, GraphicsUnit.Point);
				
				SizeF tam = gr.MeasureString(vTestes, font);
				
				PointF point = new PointF(0, height - tam.Height);
				
				Color backColor = System.Drawing.Color.White;
				Color foreColor = System.Drawing.Color.Black;
				CorAleatoria(ref foreColor, ref backColor);
				
				gr.FillRectangle(new SolidBrush(backColor), new RectangleF(point, tam));
				
				gr.DrawString("Versão de Testes", font, new SolidBrush(foreColor), point);
				
			}
			
			
			
			
			
		}
		
		
	}
}
