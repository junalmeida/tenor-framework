using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using cript = System.Security.Cryptography;
using System.Text;


namespace Tenor.Text
{

    public partial class Strings
    {

        /// <summary>
        /// Retorna o valor passado por extenso usando o idioma atual.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string DoubleToWords(double Value)
        {
            return DoubleToWords(Value, System.Globalization.CultureInfo.CurrentCulture);
        }





        /// <summary>
        /// Retorna o valor passado por extenso usando o idioma especificado.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string DoubleToWords(double Value, System.Globalization.CultureInfo Culture)
        {
            if (Culture == null)
            {
                throw (new ArgumentNullException("Culture"));
            }
            switch (Culture.IetfLanguageTag.ToLower())
            {
                case "pt-br":
                    return DoubleToWordsPTBR(Value);
                case "en-us":
                case "en-gb":
                    return DoubleToWordsENUS(Value);
                default:
                    throw (new NotImplementedException("This culture is not yet supported"));
            }
        }

        /// <summary>
        /// Baseado o algoritmo de Sérgio Eduardo Rodrigues (versão 1.0 de 10 de janeiro de 2001).
        /// Fonte: Koders.org: classe Extenso.java
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string DoubleToWordsPTBR(double value)
        {
            if (value < 0 || value > 1.0E+18)
            {
                throw (new ArgumentOutOfRangeException("value"));
            }
            else
            {
                //Dim valor As String = (Value).ToString("F2").Replace(".", ",")
                return new NumeroPorExtensoPTBR((decimal)value).ToString();
            }
        }

        private static string DoubleToWordsENUS(double value)
        {
            return new NumberToWordsENUS().changeCurrencyToWords(value);
        }
    }
}
