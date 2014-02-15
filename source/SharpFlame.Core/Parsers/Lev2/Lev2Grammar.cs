using Sprache;

namespace SharpFlame.Core.Parsers.Lev2
{
    public class Lev2Grammar
    {
        //campaign	MULTI_CAM_1
        public static readonly Parser<string> CampaingDirective =
            from directive in Parse.String("campaign")
            from whitespace in Parse.WhiteSpace.Until(Parse.WhiteSpace.Not())
            from name in Parse.AnyChar.Until(Parse.WhiteSpace).Token().Text()
            select name;

        //data		"wrf/basic.wrf"
        public static readonly Parser<string> DataDirective =
            from directive in Parse.String("data")
            from whitespace in Parse.WhiteSpace.Until(Parse.WhiteSpace.Not())
            from openquote in Parse.Char('"')
            from datapath in Parse.AnyChar.Until(Parse.Char('"')).Token().Text()
            from closequote in Parse.Char('"')
            select datapath;
    }
}