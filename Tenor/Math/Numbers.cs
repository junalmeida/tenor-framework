using System;
using System.Collections.Generic;
using System.Text;

namespace Tenor.Math
{
    /// <summary>
    /// Contains a set of number manipulations.
    /// </summary>
    public static class Numbers
    {

        /*
        
/ -----------------------------------------------------------------*
 | CalculaDigitoMod11(Dado, NumDig, LimMult)                       |
 |    Retorna o(s) NumDig Dígitos de Controle Módulo 11 do         |
 |    Dado, limitando o Valor de Multiplicação em LimMult:         |
 |                                                                 |
 |          Números Comuns:            NumDig      LimMult         |
 |            CNPJ                        2            9           |
 |            CPF                         2           12           |
 |            PIS,C/C,Age                 1            9           |
 *----------------------------------------------------------------- /
function CalculaDigitoMod11(Dado, NumDig, LimMult)
  {
  var Mult, Soma, i, n;
    
  for(n=1; n<=NumDig; n++)
    {
    Soma = 0;
    Mult = 2;
    for(i=Dado.length-1; i>=0; i--)
      {
      Soma += (Mult * parseInt(Dado.charAt(i)));
      if(++Mult > LimMult) Mult = 2;
      }
    Dado += ((Soma * 10) % 11) % 10;
    }
  return Dado.substr(Dado.length-NumDig, NumDig);
  }
         */

        /// <summary>
        /// Calculates the check digit of a number, limiting power operations on powerLimit parameter.
        /// </summary>
        /// <param name="mode">One of <see cref="CheckDigitAlgorithm"/> values that determines witch algorithm will be used.</param>
        /// <param name="number">The desired number to calculate check digit.</param>
        /// <returns>An integer of the check digit.</returns>
        public static int CalculateCheckDigit(CheckDigitAlgorithm mode, long number)
        {
            return CalculateCheckDigit(mode, number, 1, 9);
        }

        /// <summary>
        /// Calculates the check digit of a number, limiting power operations on powerLimite parameter.
        /// </summary>
        /// <param name="mode">One of <see cref="CheckDigitAlgorithm"/> values that determines witch algorithm will be used.</param>
        /// <param name="number">The desired number to calculate check digit.</param>
        /// <param name="digitLength">The size of digits generated.</param>
        /// <param name="powerLimit">An integer limit of power operations.</param>
        /// <returns>An integer of the check digit.</returns>
        /// <remarks>
        /// </remarks>
        public static int CalculateCheckDigit(CheckDigitAlgorithm mode, long number, int digitLength, int powerLimit)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException("number");
            if (digitLength < 1 || digitLength > 8)
                throw new ArgumentOutOfRangeException("digitLength");

            string result = number.ToString();
            long power, amount;
            int i, n;

            for (n = 1; n <= digitLength; n++)
            {
                amount = 0;
                power = 2;
                for (i = result.Length - 1; i >= 0; i--)
                {
                    amount += (power * Convert.ToInt32(result[i].ToString()));
                    if (++power > powerLimit) power = 2;
                }
                result += (((amount * 10) % 11) % 10).ToString();
            }
            return Convert.ToInt32(result.Substring(result.Length - digitLength, digitLength));
        }
    }

    public enum CheckDigitAlgorithm
    {
        Modulus11
    }
}
