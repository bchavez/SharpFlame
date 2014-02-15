using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sprache;

namespace SharpFlame.Core.Parsers
{
    public class Numerics
    {
        private static readonly Parser<char> decimalParse = 
            Parse.Digit.Or (Parse.Char ('.'));

        public static readonly Parser<double> Double = 
            from result in decimalParse.AtLeastOnce ().Text ()
            select double.Parse (result, CultureInfo.InvariantCulture);

        public static readonly Parser<int> Int =
            from result in Parse.Digit.AtLeastOnce().Text()
            select int.Parse(result);


        //public static readonly Parser<float> SignedFloat =
        //from sign in Parse.Char('-').Optional()
        //from number in Parse.Decimal.Token()
        //select sign.IsDefined ? -float.Parse(number) : float.Parse(number);       
        public static readonly Parser<float> Float =
            from str in Parse.Char('-').Then(s => decimalParse.AtLeastOnce().Text().Select(n => s + n))
                .Or(decimalParse.AtLeastOnce().Text())
            select float.Parse(str, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

    }
}