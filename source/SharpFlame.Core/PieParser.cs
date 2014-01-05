using System.Linq;
using Sprache;

namespace SharpFlame.Core
{
    public class Parsers
    {
        public static readonly Parser<float> SignedFloat =
            from sign in Parse.Char('-').Optional()
            from number in Parse.Decimal
            select sign.IsDefined ? -float.Parse(number) : float.Parse(number);
    }

    public class Pie
    {
        public static readonly Parser<int> Version =
            from pie in Parse.String("PIE")
            from number in Parse.Number.Token()
            let n = int.Parse(number)
            select n;

        public static readonly Parser<int> Type =
            from type in Parse.String("TYPE")
            from number in Parse.Number.Token()
            let n = int.Parse(number)
            select n;

        public static readonly Parser<TextureDirective> Texture =
            from texture in Parse.String("TEXTURE")
            from ignore in Parse.Number.Token()
            from path in Parse.AnyChar.Until(Parse.WhiteSpace).Text()
            from widthStr in Parse.Number.Token()
            from heightStr in Parse.Number.Token()
            let width = int.Parse(widthStr)
            let height = int.Parse(heightStr)
            select new TextureDirective
                {
                    Height = height,
                    Width = width,
                    Path = path
                };

        public static readonly Parser<int> Levels =
            from levels in Parse.String("LEVELS")
            from number in Parse.Number.Token()
            let n = int.Parse(number)
            select n;

        public static readonly Parser<int> Level =
            from levels in Parse.String( "LEVEL" )
            from number in Parse.Number.Token()
            let n = int.Parse( number )
            select n;

        public static readonly Parser<Xyz> XyzParser =
            from whitespace in Parse.WhiteSpace
            from x in Parsers.SignedFloat.Token()
            from y in Parsers.SignedFloat.Token()
            from z in Parsers.SignedFloat.Token()
            select new Xyz {X = x, Y = y, Z = z};

        public static readonly Parser<int> Points =
            from points in Parse.String("POINTS")
            from length in Parse.Number.Token()
            select int.Parse(length);

        public static readonly Parser<Xyz[]> PointsData =
            from points in XyzParser.Many()
            select points.ToArray();
    }

    public class TextureDirective
    {
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Xyz
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}