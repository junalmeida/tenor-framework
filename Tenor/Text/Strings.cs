using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using cript = System.Security.Cryptography;


namespace Tenor
{
	namespace Text
	{
		/// <summary>
		/// Contém funções auxiliares para manipulação de Strings
		/// </summary>
		/// <remarks></remarks>
		public partial class Strings
		{
			
			private Strings()
			{
			}
			
			
			/// <summary>
			/// Remove tags HTML de todo o texto, convertendo tags BR, P e DIV para CrLf.
			/// </summary>
			/// <param name="HtmlText"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string RemoveHTML(string HtmlText)
			{
				if (string.IsNullOrEmpty(HtmlText))
				{
					return string.Empty;
				}
				else
				{
					
					HtmlText = HtmlText.Replace("\r\n", string.Empty);
					HtmlText = HtmlText.Replace("<br>", "\r\n");
					HtmlText = HtmlText.Replace("<br/>", "\r\n");
					HtmlText = HtmlText.Replace("<br />", "\r\n");
					HtmlText = HtmlText.Replace("<BR>", "\r\n");
					HtmlText = HtmlText.Replace("<BR/>", "\r\n");
					HtmlText = HtmlText.Replace("<BR />", "\r\n");
					HtmlText = HtmlText.Replace("<div>", "\r\n");
					HtmlText = HtmlText.Replace("<DIV>", "\r\n");
					HtmlText = HtmlText.Replace("<p>", "\r\n");
					HtmlText = HtmlText.Replace("<P>", "\r\n");
					
					
					
					System.Text.RegularExpressions.Regex RegEx = new System.Text.RegularExpressions.Regex("<[^>]*>");
					return RegEx.Replace(HtmlText, "");
				}
			}
			
			
			
			
			/// <summary>
			/// Retorna uma prévia sem formatação do texto.
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string Preview(string Text)
			{
				return Preview(Text, 50, true);
			}
			
			/// <summary>
			/// Retorna uma prévia sem formatação do texto.
			/// </summary>
			/// <param name="CharCount">Número de caracteres para exibir.</param>
			/// <param name="StopOnSpace">Se ativo, evita cortar palavras na metade.</param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string Preview(string Text, int CharCount, bool StopOnSpace)
			{
				Text = RemoveHTML(Text);
				
				if (Text.Length < CharCount)
				{
					return Text;
				}
				else
				{
					
					string textFinal;
					
					if (StopOnSpace)
					{
						int pos = Text.IndexOf(' ', CharCount);
						if (pos < 0)
						{
							textFinal = Text.Substring(0, CharCount);
						}
						else
						{
							textFinal = Text.Substring(0, pos);
						}
					}
					else
					{
						textFinal = Text.Substring(0, CharCount);
					}
					
					
					if (textFinal.Length < Text.Length)
					{
						textFinal = textFinal.Trim() + "...";
					}
					return textFinal;
				}
			}
			
			/// <summary>
			/// Muda o 'case' de cada palavra, para a regra de nomes próprios.
			/// </summary>
			/// <param name="Text"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string ToMiddleCase(string Text)
			{
				string[] palavras = (" " + Text.ToLower() + " ").Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				
				for (int i = 0; i <= palavras.Length - 1; i++)
				{
					switch (palavras[i])
					{
						case "dos":
						case "das":
						case "los":
						case "las":
							break;
						default:
							if (palavras[i].Length > 2)
							{
								palavras[i] = palavras[i].Substring(0, 1).ToUpper() + palavras[i].Substring(1);
							}
							else if (palavras[i].EndsWith("."))
							{
								palavras[i] = palavras[i].ToUpper();
							}
							break;
					}
				}
				return string.Join(" ", palavras);
			}
			
			
			/// <summary>
			/// Gera uma senha aleatória de cinco caracteres aphanuméricos
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string GenerateRandomPassword()
			{
				return GenerateRandomPassword(PasswordStyle.FiveCharsOrNumbers);
			}
			
			/// <summary>
			/// Gera uma senha aleatória
			/// </summary>
			/// <param name="PasswordStyle">Tipo de senha</param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string GenerateRandomPassword(PasswordStyle PasswordStyle)
			{
				int AscA = (int)('a');
                int AscZ = (int)('z');
                int Asc0 = (int)('0');
                int Asc9 = (int)('9');
				
				
				
				
				string senha = string.Empty;
				Random randomchar = new Random();
				
				switch (PasswordStyle)
				{
					case Text.PasswordStyle.FiveCharsOrNumbers:
						for (int i = 1; i <= 5; i++)
						{
							int tipo = (int) (Math.Round(randomchar.NextDouble() * 100));
							if (tipo < 50)
							{
								senha += (char)(randomchar.Next(AscA, AscZ));
							}
							else
							{
								senha += (char)(randomchar.Next(Asc0, Asc9));
							}
							System.Threading.Thread.Sleep(1);
						}
						break;
					default:
						throw (new NotSupportedException("Invalid PasswordStyle value"));
				}
				
				return senha;
			}
			
			
			
			/// <summary>
			/// Criptografa o texto passado para Hexadecimal usando um algoritmo de criptografia.
			/// </summary>
			/// <param name="Text"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string ToAscHex(string Text)
			{
				if (Text == null)
				{
					return string.Empty;
				}
				
				string res = string.Empty;
				foreach (char ch in Text)
				{
					res += ((int)(ch)).ToString("x2");
				}
				return res;
			}
			/// <summary>
			/// Criptografa o texto passado para Hexadecimal usando um algoritmo de criptografia.
			///
			/// </summary>
			/// <param name="AscText">Um texto codificado com a função ToAscHex</param>
			/// <returns></returns>
			/// <remarks></remarks>
			/// <exception cref="FormatException"></exception>
			public static string FromAscHex(string AscText)
			{
				if (AscText == null)
				{
					return string.Empty;
				}
				
				string res = string.Empty;
				for (int i = 0; i <= AscText.Length - 1; i += 2)
				{
					res += (char)(int.Parse(AscText.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
				}
				return res;
			}
			
			
			
			
			
			
			
			
			
			/// <summary>
			/// Remove a acentuação do texto informado.
			/// </summary>
			/// <param name="Text"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string RemoveAccentuation(string Text)
			{
				if (string.IsNullOrEmpty(Text))
				{
					return string.Empty;
				}
				else
				{
					
					string res = Text;
					//' acento agudo
					res = res.Replace("á", "a");
					res = res.Replace("é", "e");
					res = res.Replace("í", "i");
					res = res.Replace("ó", "o");
					res = res.Replace("ú", "u");
					res = res.Replace("Á", "A");
					res = res.Replace("É", "E");
					res = res.Replace("Í", "I");
					res = res.Replace("Ó", "O");
					res = res.Replace("Ú", "U");
					res = res.Replace("Ý", "Y");
					res = res.Replace("ý", "y");
					
					//' acento circunflexo
					res = res.Replace("â", "a");
					res = res.Replace("ê", "e");
					res = res.Replace("î", "i");
					res = res.Replace("ô", "o");
					res = res.Replace("û", "u");
					res = res.Replace("Â", "A");
					res = res.Replace("Ê", "E");
					res = res.Replace("Î", "I");
					res = res.Replace("Ô", "O");
					res = res.Replace("Û", "U");
					
					//' til
					res = res.Replace("ã", "a");
					res = res.Replace("õ", "o");
					res = res.Replace("Ã", "A");
					res = res.Replace("Õ", "O");
					res = res.Replace("Ñ", "N");
					res = res.Replace("ñ", "n");
					
					//' cedilha
					res = res.Replace("ç", "c");
					res = res.Replace("Ç", "C");
					
					//' trema
					res = res.Replace("ü", "u");
					res = res.Replace("Ü", "U");
					res = res.Replace("ä", "a");
					res = res.Replace("Ä", "a");
					
					//' crase
					res = res.Replace("à", "a");
					res = res.Replace("è", "e");
					res = res.Replace("ì", "i");
					res = res.Replace("ò", "o");
					res = res.Replace("ù", "u");
					res = res.Replace("À", "A");
					res = res.Replace("È", "E");
					res = res.Replace("Ì", "I");
					res = res.Replace("Ò", "O");
					res = res.Replace("Ù", "U");
					
					
					
					return res;
				}
			}
			
			
			
			
			
			/// <summary>
			/// Codifica a acentuação do texto para HTML.
			/// </summary>
			/// <param name="Text"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public static string EncodeAccentuation(string Text)
			{
				if (string.IsNullOrEmpty(Text))
				{
					return string.Empty;
				}
				else
				{
					
					string res = Text;
					res = res.Replace("&", "&amp;");
					//' acento agudo
					res = res.Replace("á", "&aacute;");
					res = res.Replace("é", "&eacute;");
					res = res.Replace("í", "&iacute;");
					res = res.Replace("ó", "&oacute;");
					res = res.Replace("ú", "&uacute;");
					res = res.Replace("Á", "&Aacute;");
					res = res.Replace("É", "&Eacute;");
					res = res.Replace("Í", "&Iacute;");
					res = res.Replace("Ó", "&Oacute;");
					res = res.Replace("Ú", "&Uacute;");
					res = res.Replace("Ý", "&Yacute;");
					res = res.Replace("ý", "&yacute;");
					
					//' acento circunflexo
					res = res.Replace("â", "&acirc;");
					res = res.Replace("ê", "&ecirc;");
					res = res.Replace("î", "&icirc;");
					res = res.Replace("ô", "&ocirc;");
					res = res.Replace("û", "&ucirc;");
					res = res.Replace("Â", "&Acirc;");
					res = res.Replace("Ê", "&Ecirc;");
					res = res.Replace("Î", "&Icirc;");
					res = res.Replace("Ô", "&Ocirc;");
					res = res.Replace("Û", "&Ucirc;");
					
					//' til
					res = res.Replace("ã", "&atilde;");
					res = res.Replace("õ", "&otilde;");
					res = res.Replace("Ã", "&Atilde;");
					res = res.Replace("Õ", "&Otilde;");
					res = res.Replace("Ñ", "&Ntilde;");
					res = res.Replace("ñ", "&ntilde;");
					
					//' cedilha
					res = res.Replace("ç", "&ccedil;");
					res = res.Replace("Ç", "&Ccedil;");
					
					//' trema
					res = res.Replace("ä", "&auml;");
					res = res.Replace("Ä", "&Auml;");
					res = res.Replace("ü", "&uuml;");
					res = res.Replace("Ü", "&Uuml;");
					
					//' crase
					res = res.Replace("à", "&agrave;");
					res = res.Replace("è", "&egrave;");
					res = res.Replace("ì", "&igrave;");
					res = res.Replace("ò", "&ograve;");
					res = res.Replace("ù", "&ugrave;");
					res = res.Replace("À", "&Agrave;");
					res = res.Replace("È", "&Egrave;");
					res = res.Replace("Ì", "&Igrave;");
					res = res.Replace("Ò", "&Ograve;");
					res = res.Replace("Ù", "&Ugrave;");
					
					
					
					return res;
				}
			}
			
		}
		
		/// <summary>
		/// Contém uma lista de tipos de senhas disponíveis
		/// </summary>
		/// <remarks></remarks>
		public enum PasswordStyle
		{
			/// <summary>
			/// Cinco caracteres entre letras e números
			/// </summary>
			/// <remarks></remarks>
			FiveCharsOrNumbers,
			/// <summary>
			/// Cinco caracteres alphanuméricos incluindo símbolos.
			/// </summary>
			/// <remarks></remarks>
			FiveAlpha,
			/// <summary>
			/// Cinco letras
			/// </summary>
			/// <remarks></remarks>
			FiveChars,
			/// <summary>
			/// Duas letras e dois números
			/// </summary>
			/// <remarks></remarks>
			TwoCharsTwoNumbers,
			/// <summary>
			/// Dois números e duas letras
			/// </summary>
			/// <remarks></remarks>
			TwoNumbersTwoChars
		}
		
		
		
		
		
	}
}
