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

    public static partial class Strings
    {

        /// <summary>
        /// Converts the desired value into a string representation of the current culture.
        /// </summary>
        /// <param name="value">The desired value.</param>
        public static string DoubleToWords(double value)
        {
            return DoubleToWords(value, System.Globalization.CultureInfo.CurrentCulture);
        }





        /// <summary>
        /// Converts the desired value into a string representation.
        /// </summary>
        /// <param name="value">The desired value.</param>
        /// <param name="culture">The desired culture.</param>
        public static string DoubleToWords(double value, System.Globalization.CultureInfo culture)
        {
            if (culture == null)
            {
                throw (new ArgumentNullException("Culture"));
            }
            switch (culture.IetfLanguageTag.ToLower())
            {
                case "pt-br":
                    return DoubleToWordsPTBR(value);
                case "en-us":
                case "en-gb":
                    return DoubleToWordsENUS(value);
                default:
                    throw (new NotImplementedException("This culture is not yet implemented. Please make a feature request."));
            }
        }

        /// <summary>
        /// Converts the double value to an string representation of it in portuguese.
        /// Based on Sérgio Eduardo Rodrigues algorithm (version 1.0 of jan-10-2001).
        /// Koders.org: Extenso.java
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <returns>An string with the description of the value.</returns>
        private static string DoubleToWordsPTBR(double value)
        {
            if (value < 0 || value > 1.0E+18)
            {
                throw (new ArgumentOutOfRangeException("value"));
            }
            else
            {
                /*
                 * Dim valor As String = (Value).ToString("F2").Replace(".", ",")
                 */
                return new NumeroPorExtensoPTBR((decimal)value).ToString();
            }
        }

        /// <summary>
        /// Converts the double value to an string representation of it in american english.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <returns>An string with the description of the value.</returns>
        private static string DoubleToWordsENUS(double value)
        {
            return new NumberToWordsENUS().changeCurrencyToWords(value);
        }
    }
}
