using Sprache;

namespace SharpFlame.Core.Parsers
{
    //C and C++ scan wrappers
    public class Scan
    {
        public static readonly Parser<uint> U =
            from token in Parse.Regex( @"\S+" ).Token().Text()
            let scan = new ScanFormatted()
            let succeded = scan.Parse(token, "%u")
            select (uint)scan.Results[0];

        public static readonly Parser<int> I =
            from token in Parse.Regex( @"\S+" ).Token().Text()
            let scan = new ScanFormatted()
            let succeded = scan.Parse( token, "%i" )
            select (int)scan.Results[0];

        public static readonly Parser<int> D =
            from token in Parse.Regex( @"\S+" ).Token().Text()
            let scan = new ScanFormatted()
            let succeded = scan.Parse( token, "%d" )
            select (int)scan.Results[0];

        public static readonly Parser<uint> O =
            from token in Parse.Regex( @"\S+" ).Token().Text()
            let scan = new ScanFormatted()
            let succeded = scan.Parse(token, "%o")
            select (uint)scan.Results[0];

        public static readonly Parser<uint> X =
            from token in Parse.Regex(@"\S+").Token().Text()
            let scan = new ScanFormatted()
            let succeded = scan.Parse(token, "%x")
            select (uint)scan.Results[0];

        public static readonly Parser<float> F =
            from token in Parse.Regex( @"\S+" ).Token().Text()
            let scan = new ScanFormatted()
            let succeded = scan.Parse( token, "%f" )
            select (float)scan.Results[0];
    }
}