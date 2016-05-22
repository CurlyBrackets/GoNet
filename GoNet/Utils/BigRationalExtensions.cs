using Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.Utils
{
    static class BigRationalExtensions
    {
        public static BigRational Parse(string s)
        {
            string baseString, exponentString = null;
            if (s.Contains('e') || s.Contains('E'))
            {
                int index = s.IndexOf("e", StringComparison.InvariantCultureIgnoreCase);
                exponentString = s.Substring(index + 1);
                baseString = s.Substring(0, index);
            }
            else
                baseString = s;

            int decimalIndex = baseString.IndexOf('.');
            BigInteger denominator = default(BigInteger);
            if (decimalIndex != -1)
            {
                denominator = BigInteger.Pow(10, baseString.Length - decimalIndex - 1);
                baseString = baseString.Remove(decimalIndex, 1);
            }

            var numerator = BigInteger.Parse(baseString);

            if (!string.IsNullOrEmpty(exponentString))
                numerator *= BigInteger.Pow(10, int.Parse(exponentString));

            return new BigRational(numerator, denominator);
        }
    }
}
