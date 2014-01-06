using Sprache;

namespace SharpFlame.Core.Parsers
{
    public class Numerics
    {
        //Parse.Char('-').Then( s => Parse.Number.Select(n => s+n)).Or( Parse.Number )
        public static readonly Parser<float> SignedFloat =
            from sign in Parse.Char('-').Optional()
            from number in Parse.Decimal.Token()
            select sign.IsDefined ? -float.Parse(number) : float.Parse(number);
    }
}