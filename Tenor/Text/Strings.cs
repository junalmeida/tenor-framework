using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using cript = System.Security.Cryptography;
using System.ComponentModel;
using SystemMath = System.Math;

namespace Tenor.Text
{
    /// <summary>
    /// Represents a set of string manipulation methods.
    /// </summary>
    public static partial class Strings
    {

        /// <summary>
        /// Removes HTML from text, converting BR, P and DIV tags to <see cref="System.Environment.NewLine" />.
        /// </summary>
        /// <param name="htmlText">The html text to be stripped.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveHTML(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return string.Empty;
            }
            else
            {

                htmlText = htmlText.Replace("\r\n", string.Empty);
                htmlText = htmlText.Replace("<br>", Environment.NewLine);
                htmlText = htmlText.Replace("<br/>", Environment.NewLine);
                htmlText = htmlText.Replace("<br />", Environment.NewLine);
                htmlText = htmlText.Replace("<BR>", Environment.NewLine);
                htmlText = htmlText.Replace("<BR/>", Environment.NewLine);
                htmlText = htmlText.Replace("<BR />", Environment.NewLine);
                htmlText = htmlText.Replace("<div>", Environment.NewLine);
                htmlText = htmlText.Replace("<DIV>", Environment.NewLine);
                htmlText = htmlText.Replace("<p>", Environment.NewLine);
                htmlText = htmlText.Replace("<P>", Environment.NewLine);



                System.Text.RegularExpressions.Regex RegEx = new System.Text.RegularExpressions.Regex("<[^>]*>");
                return RegEx.Replace(htmlText, "");
            }
        }




        /// <summary>
        /// Creates a text preview with no formatting.
        /// </summary>
        /// <param name="text">The original text string.</param>
        public static string Preview(string text)
        {
            return Preview(text, 50, true);
        }

        /// <summary>
        /// Creates a text preview with no formatting.
        /// </summary>
        /// <param name="text">The original text string.</param>
        /// <param name="charCount">Character limit.</param>
        /// <param name="stopOnSpace">Avoid to cut words.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Preview(string text, int charCount, bool stopOnSpace)
        {
            text = RemoveHTML(text);

            if (text.Length < charCount)
            {
                return text;
            }
            else
            {

                string textFinal;

                if (stopOnSpace)
                {
                    int pos = text.IndexOf(' ', charCount);
                    if (pos < 0)
                    {
                        textFinal = text.Substring(0, charCount);
                    }
                    else
                    {
                        textFinal = text.Substring(0, pos);
                    }
                }
                else
                {
                    textFinal = text.Substring(0, charCount);
                }


                if (textFinal.Length < text.Length)
                {
                    textFinal = textFinal.Trim() + "...";
                }
                return textFinal;
            }
        }

        /// <summary>
        /// Capitalizes each word, keeping 'and', 'dos', 'das', 'los', 'las', and words with 2 characters in lower case. 
        /// </summary>
        /// <param name="text">The original text string.</param>
        public static string ToMiddleCase(string text)
        {
            string[] words = (" " + text.ToLower() + " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i <= words.Length - 1; i++)
            {
                switch (words[i])
                {
                    case "and":
                    case "dos":
                    case "das":
                    case "los":
                    case "las":
                        break;
                    default:
                        if (words[i].Length > 2)
                        {
                            words[i] = words[i].Substring(0, 1).ToUpper() + words[i].Substring(1);
                        }
                        else if (words[i].EndsWith("."))
                        {
                            words[i] = words[i].ToUpper();
                        }
                        break;
                }
            }
            return string.Join(" ", words);
        }


        /// <summary>
        /// Creates a ramdom password with five alphanumeric characters.
        /// </summary>
        public static string GenerateRandomPassword()
        {
            return GenerateRandomPassword(PasswordStyle.FiveCharsOrNumbers);
        }

        /// <summary>
        /// Creates a ramdom password.
        /// </summary>
        /// <param name="passwordStyle">One of PasswordStyle values.</param>
        public static string GenerateRandomPassword(PasswordStyle passwordStyle)
        {
            int AscA = (int)('a');
            int AscZ = (int)('z');
            int Asc0 = (int)('0');
            int Asc9 = (int)('9');



            string password = string.Empty;
            Random randomchar = new Random();

            switch (passwordStyle)
            {
                case Text.PasswordStyle.FiveCharsOrNumbers:
                    for (int i = 1; i <= 5; i++)
                    {
                        int tipo = (int)(SystemMath.Round(randomchar.NextDouble() * 100));
                        if (tipo < 50)
                        {
                            password += (char)(randomchar.Next(AscA, AscZ));
                        }
                        else
                        {
                            password += (char)(randomchar.Next(Asc0, Asc9));
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                    break;
                default:
                    throw (new NotSupportedException("Invalid PasswordStyle value"));
            }

            return password;
        }



        /// <summary>
        /// Converts the text to an Hexadecimal representation.
        /// </summary>
        /// <param name="text">The original text string.</param>
        public static string ToAscHex(string text)
        {
            if (text == null)
            {
                return string.Empty;
            }

            string res = string.Empty;
            foreach (char ch in text)
            {
                res += ((int)(ch)).ToString("x2");
            }
            return res;
        }
        /// <summary>
        /// Converts the text from an Hexadecimal representation.
        /// </summary>
        /// <exception cref="FormatException" />
        public static string FromAscHex(string hexText)
        {
            if (hexText == null)
            {
                return string.Empty;
            }

            string res = string.Empty;
            for (int i = 0; i <= hexText.Length - 1; i += 2)
            {
                res += (char)(int.Parse(hexText.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
            }
            return res;
        }



        #region Enums

        /// <summary>
        /// Creates a list of enum items with its descriptions.
        /// </summary>
        public static Dictionary<int, string> EnumToDictionary(Type enumType)
        {
            return EnumToDictionaryGeneric<int>(enumType);
        }

        /// <summary>
        /// Creates a list of enum items with its descriptions.
        /// </summary>
        public static Dictionary<long, string> EnumToDictionaryLong(Type enumType)
        {
            return EnumToDictionaryGeneric<long>(enumType);
        }

        private static Dictionary<T, string> EnumToDictionaryGeneric<T>(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            if (!enumType.IsEnum)
                throw new ArgumentException("The parameter enumType must be an enum type (doh!).", "enumType");

            Dictionary<T, string> res = new Dictionary<T, string>();

            foreach (Enum value in Enum.GetValues(enumType))
            {
                res.Add((T)Convert.ChangeType(value, typeof(T)), GetEnumItemDescription((Enum)Convert.ChangeType(value, enumType)));
            }

            return res;
        }

        /// <summary>
        /// Gets the System.ComponentModel.DescriptionAttribute of the enum value. 
        /// </summary>
        /// <param name="value">An enum value</param>
        /// <returns>A string with the enum item description defined or the item's name when DescriptionAttribute is not found.</returns>
        public static string GetEnumItemDescription(Enum value)
        {
            string res = value.ToString();
            Type type = value.GetType();
            System.Reflection.FieldInfo fi = type.GetField(res);
            DescriptionAttribute[] attrs = (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[]);
            if (attrs.Length > 0)
            {
                res = attrs[0].Description;
            }
            return res;
        }
        #endregion






        /// <summary>
        /// Removes the accentuation of each word in a text.
        /// </summary>
        /// <param name="text">The original text string.</param>
        public static string RemoveAccentuation(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            else
            {

                string res = text;
                //' accent
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

                //' circunflex
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

                //' tilde
                res = res.Replace("ã", "a");
                res = res.Replace("õ", "o");
                res = res.Replace("Ã", "A");
                res = res.Replace("Õ", "O");
                res = res.Replace("Ñ", "N");
                res = res.Replace("ñ", "n");

                //' cedilla
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
        /// Encodes accentuation with HtmlEncoding.
        /// </summary>
        /// <param name="text">The original text string.</param>
        public static string EncodeAccentuation(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            else
            {

                string res = text;
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
    /// Defines how the password will be generated.
    /// </summary>
    /// <remarks></remarks>
    public enum PasswordStyle
    {
        /// <summary>
        /// Generates a password with five western characters or numbers.
        /// </summary>
        FiveCharsOrNumbers,
        /// <summary>
        /// Generates a password with five alphanumeric characters including symbols.
        /// </summary>
        FiveAlpha,
        /// <summary>
        /// Generates a password with five western alphabetic characters.
        /// </summary>
        FiveChars,
        /// <summary>
        /// Generates a password with two letters and two numbers.
        /// </summary>
        TwoCharsTwoNumbers,
        /// <summary>
        /// Generates a password with two numbers and two letters.
        /// </summary>
        TwoNumbersTwoChars
    }
}