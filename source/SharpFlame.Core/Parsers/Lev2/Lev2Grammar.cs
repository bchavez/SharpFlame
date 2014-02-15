using Sprache;

namespace SharpFlame.Core.Parsers.Lev2
{
    public class Lev2Grammar
    {
        //campaign	MULTI_CAM_1
        public static readonly Parser<string> CampaingDirective =
            from directive in Parse.String("campaign")
            from whitespace in Parse.WhiteSpace.AtLeastOnce()
            from name in Parse.AnyChar.Until(Parse.WhiteSpace).Token().Text()
            select name;
    }
}