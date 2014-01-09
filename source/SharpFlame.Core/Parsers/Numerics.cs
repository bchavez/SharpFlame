using System.Globalization;
using Sprache;

namespace SharpFlame.Core.Parsers
{
    public class Numerics
    {
        //public static readonly Parser<float> SignedFloat =
        //from sign in Parse.Char('-').Optional()
        //from number in Parse.Decimal.Token()
        //select sign.IsDefined ? -float.Parse(number) : float.Parse(number);

        public static readonly Parser<float> SignedFloat =
            from str in Parse.Char('-').Then(s => Parse.Decimal.Select(n => s + n))
                .Or(Parse.Decimal)
            select float.Parse(str, NumberStyles.AllowLeadingSign);

    }
}