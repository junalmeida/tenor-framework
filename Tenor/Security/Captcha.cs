using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;




namespace Tenor
{
	namespace Security
	{
		
		
		/// <summary>
		/// Classe responsável por implementar formas de assegurar que somente seres humanos tenham acesso a determinado serviço.
		/// CAPTCHA é um acrônimo da expressão em língua inglesa "Completely Automated Public Turing test to tell Computers and Humans Apart" que pode ser traduzida como "Teste de Turing público e completamente automático para distinguir computadores e humanos.
		/// Esta ferramenta tem o objetivo de determinar, mediante uma ou mais perguntas, se o utilizador é um ser humano e não um computador ou, mais precisamente, um bot (robôs que executam tarefas pré-programadas). O teste captcha mais comum é o que solicita que o usuário escreva uma série de letras ou números que aparecem num quadro, geralmente um pouco distorcidos ou ofuscados para evitar o reconhecimento por máquinas.
		/// </summary>
		/// <remarks>
		/// Para a validação web, buscar o AccessCode no cache do aspnet numa key préviamente definida.
		/// Esta ação já está encapsulada no método <see cref="Captcha.AccessCode">AccessCode</see>.
		/// </remarks>
		public class Captcha
		{
			
			
			/// <summary>
			/// Instancia a classe gerando uma string aleatória para o <see cref="Captcha.AccessCode">AccessCode</see>.
			/// </summary>
			/// <remarks></remarks>
			public Captcha()
			{
				bgColors = new Color[] {Color.Violet, System.Drawing.Color.DarkBlue, System.Drawing.Color.DarkCyan, Color.DarkGoldenrod, System.Drawing.Color.IndianRed, System.Drawing.Color.Indigo};
				fgColors = new Color[] {Color.White, Color.Aquamarine, System.Drawing.Color.Gold, System.Drawing.Color.LightGreen, Color.LightYellow, System.Drawing.Color.LightGoldenrodYellow};
				
				_AccessCode = "";
				
				Random rnd = new Random();
				for (int i = 0; i <= rnd.Next(4, 7); i++)
				{
					char strChar = (char)(rnd.Next(65, 90));
					_AccessCode += strChar.ToString();
				}
			}
			
			/// <summary>
			/// Instancia a classe com um AccessCode pré-definido.
			/// </summary>
			/// <param name="AccessCode">Código de acesso desejado.</param>
			/// <remarks></remarks>
			public Captcha(string AccessCode)
			{
				bgColors = new Color[] {Color.Violet, System.Drawing.Color.DarkBlue, System.Drawing.Color.DarkCyan, Color.DarkGoldenrod, System.Drawing.Color.IndianRed, System.Drawing.Color.Indigo};
				fgColors = new Color[] {Color.White, Color.Aquamarine, System.Drawing.Color.Gold, System.Drawing.Color.LightGreen, Color.LightYellow, System.Drawing.Color.LightGoldenrodYellow};
				
				this.AccessCode = AccessCode;
			}
			
			
			private const int ImageWidth = 220;
			private const int ImageHeight = 70;
			
			
			private string _AccessCode;
			/// <summary>
			/// Mantém a string para ser gerada pela imagem. Retorna uma string aleatória caso nenhuma seja definida
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public string AccessCode
			{
				get
				{
					return _AccessCode;
				}
				set
				{
					_AccessCode = value;
				}
			}
			
			private string[] fontes = new string[] {"Arial", "Arial Black", "Tahoma", "Trebuchet MS", "Comic Sans MS", "Century Gothic"};
			private Font GenerateRandomFont()
			{
				Random rnd = new Random();
				Font font = new Font(fontes[rnd.Next(0, fontes.Length)], rnd.Next(28, 32), FontStyle.Bold, GraphicsUnit.Point);
				
				return font;
			}
			
			private Color[] bgColors;
			private Color[] fgColors;
			private Brush GenerateRandomBackBrush()
			{
				Random rnd = new Random();
				return new SolidBrush(bgColors[rnd.Next(bgColors.Length)]);
			}
			
			private Pen GenerateRandomForePen()
			{
				return new Pen(GenerateRandomForeColor());
			}
			
			private Color GenerateRandomForeColor()
			{
				Random rnd = new Random();
				return fgColors[rnd.Next(fgColors.Length)];
			}
			
			
			
			/// <summary>
			/// Gera uma representação gráfica aleatória do código de acesso atual.
			/// </summary>
			/// <returns>Uma imagem JPEG com a representação gráfica.</returns>
			/// <remarks></remarks>
			public Image GenerateImage()
			{
				Random rnd = new Random();
				Font Font = GenerateRandomFont();
				SolidBrush brush = new SolidBrush(GenerateRandomForeColor());
				
				Bitmap bmp = new Bitmap(ImageWidth, ImageHeight);
				Graphics gr = Graphics.FromImage(bmp);
				
				//Fundo:
				gr.FillRectangle(GenerateRandomBackBrush(), new Rectangle(0, 0, bmp.Width, bmp.Height));
				
				GraphicsPath pth = new GraphicsPath();
				pth.AddString(AccessCode, Font.FontFamily, System.Convert.ToInt32(Font.Style), Font.Size, new PointF(rnd.Next(60), rnd.Next(5, 10)), StringFormat.GenericTypographic);
				Matrix m = new Matrix();
				
				m.Rotate(rnd.Next(- 4, 5));
				pth.Transform(m);
				
				
				gr.FillPath(brush, pth);
				gr.DrawPath(GenerateRandomForePen(), pth);
				
				gr.DrawLine(GenerateRandomForePen(), new Point(0, rnd.Next(bmp.Height)), new Point(bmp.Width, rnd.Next(bmp.Height)));
				gr.DrawLine(GenerateRandomForePen(), new Point(0, rnd.Next(bmp.Height)), new Point(bmp.Width, rnd.Next(bmp.Height)));
				gr.DrawLine(GenerateRandomForePen(), new Point(0, rnd.Next(bmp.Height)), new Point(bmp.Width, rnd.Next(bmp.Height)));
				
				for (int x = 0; x <= bmp.Width - 1; x++)
				{
					for (int y = 0; y <= bmp.Height - 1; y++)
					{
						if (rnd.Next(4) == 1)
						{
							bmp.SetPixel(x, y, GenerateRandomForeColor());
						}
					}
				}
				
				
				Font.Dispose();
				gr.Dispose();
				
				DistortImage(bmp, rnd.Next(5, 20) * System.Convert.ToInt32((rnd.Next(2) == 1) ? 1 : - 1));
				
				return bmp;
			}
			
			
			/// <summary>Distorts the image.</summary>
			/// <param name="b">The image to be transformed.</param>
			/// <param name="distortion">An amount of distortion.</param>
			private void DistortImage(Bitmap b, double distortion)
			{
				
				int width = b.Width;
				int height = b.Height;
				
				//' Copy the image so that we're always using the original for source color
				Bitmap copy = (Bitmap) (b.Clone());
				// Iterate over every pixel
				for (int y = 0; y <= height - 1; y++)
				{
					
					for (int x = 0; x <= width - 1; x++)
					{
						
						//' Adds a simple wave
						int newX = x + (int)(distortion * Math.Sin(Math.PI * y / 64.0));
						int newY = y + (int)(distortion * Math.Cos(Math.PI * x / 64.0));
						if (newX < 0 || newX >= width)
						{
							newX = 0;
						}
						if (newY < 0 || newY >= height)
						{
							newY = 0;
						}
						b.SetPixel(x, y, copy.GetPixel(newX, newY));
					}
				}
				
			}
			
			
			/// <summary>
			/// Cria uma representação falada do código de acesso atual.
			/// </summary>
			/// <returns>Array de bytes com audio no formato Wave PCM</returns>
			/// <remarks></remarks>
			public byte[] GenerateAudio()
			{
				throw new NotImplementedException();
				
				
				//Dim rnd As New Random
				//Dim toSpeak As String = ""
				//For i As Integer = 0 To AccessCode.Length - 1
				//    toSpeak += AccessCode.Chars(i).ToString & ". " & vbCrLf
				//Next
				//'toSpeak = toSpeak.Substring(1)
				//Dim tempPath As String = Environment.ExpandEnvironmentVariables("%TEMP%")
				
				
				//If Not tempPath.EndsWith("\") Then tempPath += "\"
				//Dim tempFile As String = tempPath & "sound" & rnd.Next(10000, 90000).ToString() & ".wav"
				
				
				//'usando o SpFileStream porque o memory stream nao da certo
				//Dim sound As New SpFileStream
				//Try
				//    sound.Open(tempFile, SpeechStreamFileMode.SSFMCreateForWrite)
				
				
				//    Dim speech As New SpVoice
				//    Dim voices As ISpeechObjectTokens = speech.GetVoices()
				//    speech.Voice = voices.Item(rnd.Next(voices.Count))
				//    'Dim format As New SpAudioFormat
				//    'format.Type = SpeechAudioFormatType.SAFTGSM610_11kHzMono
				//    'sound.Format = format
				
				//    speech.AudioOutputStream = sound
				//    speech.Rate = -5 'de -10 a +10
				
				//    'falar para o arquivo
				//    speech.Speak(toSpeak, SpeechVoiceSpeakFlags.SVSFlagsAsync)
				//    speech.WaitUntilDone(-1)
				
				//    speech = Nothing
				//    voices = Nothing
				//Catch ex As Exception
				//    Try
				//        sound.Close()
				//        sound = Nothing
				//    Catch ex2 As Exception
				//    End Try
				//    Try
				//        If System.IO.File.Exists(tempFile) Then
				//            System.IO.File.Delete(tempFile)
				//        End If
				//    Catch ex2 As Exception
				//    End Try
				//    Throw ex
				//Finally
				//    Try
				//        sound.Close()
				//        sound = Nothing
				
				//    Catch ex As Exception
				//    End Try
				//End Try
				
				//If System.IO.File.Exists(tempFile) Then
				
				
				//    Dim mem As New System.IO.FileStream(tempFile, System.IO.FileMode.Open)
				
				//    Dim bytes(CInt(mem.Length)) As Byte
				//    Dim offset As Integer = 0
				//    Dim remaining As Integer = CInt(mem.Length)
				
				//    Do While remaining > 0
				//        Dim read As Integer = mem.Read(bytes, offset, remaining)
				//        If read <= 0 Then Exit Do
				//        remaining -= read
				//        offset += read
				//    Loop
				//    mem.Close()
				//    mem.Dispose()
				
				//    Try
				//        System.IO.File.Delete(tempFile)
				//    Catch ex As Exception
				//    End Try
				//    Return bytes
				//Else
				//    Return Nothing
				//End If
			}
			
			
			
			/// <summary>
			/// Faz a validação do código digitado pelo usuário e remove do cache o valor.
			/// </summary>
			/// <param name="AspNetCacheID">ID usado para manter o código de acesso</param>
			/// <param name="Challenge">Código fornecido pelo usuário</param>
			/// <returns>Se o código é válido ou não</returns>
			/// <remarks></remarks>
			public static bool ValidateCaptcha(string AspNetCacheID, string Challenge)
			{
				System.Web.HttpContext Context = System.Web.HttpContext.Current;
				if (Context == null)
				{
					throw (new InvalidOperationException("Can\'t use this function while not in an ASP.NET context"));
				}
				if (Context.Cache[AspNetCacheID] == null)
				{
					return false;
				}
				else
				{
					bool res = Context.Cache[AspNetCacheID].ToString() == Challenge;
					if (res)
					{
						Context.Cache.Remove(AspNetCacheID);
					}
					return res;
				}
			}
			
			
			
			
			
			
			
			//Public Function TextToWav() As Byte()
			//    Dim rnd As New Random
			//    Dim b As Byte() = Nothing
			
			//    Try
			//        Dim spFlags As SpeechVoiceSpeakFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync
			
			
			//        Dim speech As New SpVoice
			//        Dim voices As ISpeechObjectTokens = speech.GetVoices()
			//        speech.Voice = voices.Item(rnd.Next(voices.Count))
			
			//        Dim spMemoryStream As New SpMemoryStream
			//        spMemoryStream.Format.Type = SpeechAudioFormatType.SAFT11kHz8BitMono
			//        speech.AudioOutputStream = spMemoryStream
			
			//        Dim r As Integer = speech.Speak(AccessCode, spFlags)
			//        speech.WaitUntilDone(-1)
			
			
			//        spMemoryStream.Seek(0, SpeechStreamSeekPositionType.SSSPTRelativeToStart)
			//        b = CType(spMemoryStream.GetData(), Byte())
			//        Return b
			//    Catch ex As Exception
			//        Throw ex
			//    End Try
			
			//End Function
			
			
			
		}
		
	}
	
}
