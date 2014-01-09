using System;
using System.Linq;
using FluentValidation.Attributes;
using SharpFlame.Core.Parsers.Validators;
using Sprache;

namespace SharpFlame.Core.Parsers
{
    [Flags]
    public enum PolygonFlags : uint
    {
        Texture,
        TextureAnimation
    }

    public class PieGrammar
    {
        //PIE 2
        public static readonly Parser<int> Version =
            from pie in Parse.String("PIE")
            from number in Parse.Number.Token()
            let n = 
            int.Parse(number)
            select n;

        //TYPE 200
        public static readonly Parser<int> Type =
            from type in Parse.String("TYPE")
            from number in Parse.Number.Token()
            let n = int.Parse(number)
            select n;

        //TEXTURE 0 page-16-droid-drives.png 256 256
        public static readonly Parser<Texture> Texture =
            from texture in Parse.String("TEXTURE")
            from ignore in Parse.Number.Token()
            from path in Parse.AnyChar.Until(Parse.WhiteSpace).Text()
            from widthStr in Parse.Number.Token()
            from heightStr in Parse.Number.Token()
            let width = int.Parse(widthStr)
            let height = int.Parse(heightStr)
            select new Texture
                {
                    Height = height,
                    Width = width,
                    Path = path
                };

        //LEVELS 3
        public static readonly Parser<int> Levels =
            from levels in Parse.String("LEVELS")
            from number in Parse.Number.Token()
            let n =
            int.Parse(number)
            select n;

        //LEVEL 1
        public static readonly Parser<int> Level =
            from levels in Parse.String( "LEVEL" )
            from number in Parse.Number.Token()
            let n = 
            int.Parse( number )
            select n;

        //	5 20 17 
        //  x, y, z
        public static readonly Parser<Point> PointLine =
            from x in Numerics.SignedFloat.Token()
            from y in Numerics.SignedFloat.Token()
            from z in Numerics.SignedFloat.Token()
            select new Point
                {
                    X = x, Y = y, Z = z
                };

        //POINTS 41 
        public static readonly Parser<int> Points =
            from points in Parse.String("POINTS")
            from length in Parse.Number.Token()
            select 
            int.Parse(length);

        //	0 23 20 
        //  5 23 17 
	    //  0 20 11 
        //  x, y, z
        //public static readonly Parser<Point[]> PointsData =
        //    from points in PointLine.Many()
        //    select points.ToArray();

        //POLYGONS 68
        public static readonly Parser<int> Polygons =
            from levels in Parse.String( "POLYGONS" )
            from number in Parse.Number.Token()
            let 
            n = int.Parse( number )
            select n;

        //data:
        //	200 3  3  2  1  237 220 239 220 239 222
        //scan codes:
        //  %x  %u %d %d %d %d  %d  %f  %f  %f  %f
        // flag, numpoints, p1, p2, p3, [nFrames, pbRate, tWidth, tHeight], texcoord[]xy
        //  hex, -any.number, 
        public static readonly Parser<Polygon> PolygonLine =
            from flags in Scan.X.Token()
            from numpoints in Scan.U.Token()
            from p1 in Scan.D.Token()
            from p2 in Scan.D.Token()
            from p3 in Scan.D.Token()
            from nFrames in Scan.D.Token()
            from pbRate in Scan.D.Token()
            from tWidth in Scan.F.Token()
            from tHeight in Scan.F.Token()
            from xtex in Scan.F.Token()
            from ytex in Scan.F.Token()
            select new Polygon
                {
                    Flags = (PolygonFlags)flags,
                    PointCount = numpoints,
                    P1 = p1,
                    P2 = p2,
                    P3 = p3,
                    TexX = xtex,
                    TexY = ytex
                };

        //CONNECTORS 2
        public static readonly Parser<int> Connectors =
            from levels in Parse.String( "CONNECTORS" )
            from number in Parse.Number.Token()
            let 
            n = int.Parse( number )
            select n;

        //	0 11 22
        //  x, y, z
        public static readonly Parser<Connector> ConnectorLine =
            from x in Scan.D.Token()
            from y in Scan.D.Token()
            from z in Scan.D.Token()
            select new Connector
                {
                    X = x,
                    Y = y,
                    Z = z,
                };

        //Parses the entire file.
        public static readonly Parser<Pie> Pie =
            from version in Version
            from type in Type
            from texture in Texture
            from levels in Levels
            from levelArray in
                (from level in Level
                    from points in Points
                    from pointsData in PointLine.Repeat(points)
                    from polygons in Polygons
                    from polygonsData in PolygonLine.Repeat(polygons)
                    from connectors in Connectors.Optional()
                    from connectorData in ConnectorLine.Repeat(connectors.GetOrDefault())
                 select new Level
                        {
                            LevelNumber = level,
                            PointsCount = points,
                            Points = pointsData.ToArray(),
                            PolygonsCount = polygons,
                            Polygons = polygonsData.ToArray(),
                            ConnectorCount = connectors.IsDefined ? connectors.Get() : default(int?),
                            Connectors = connectorData.ToArray()
                        })
                    .Repeat(levels)
            select new Pie
                {
                    Version = version,
                    Texture = texture,
                    Type = type,
                    LevelCount = levels,
                    Levels = levelArray.ToArray()
                };
    }

    public class Polygon
    {
        public PolygonFlags Flags { get; set; }
        public uint PointCount { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }
        public float TexX { get; set; }
        public float TexY { get; set; }
    }

    public class Texture
    {
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public class Connector
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    [Validator(typeof(LevelValidator))]
    public class Level
    {
        public int LevelNumber { get; set; }
        public Point[] Points { get; set; }
        public Polygon[] Polygons { get; set; }
        internal int PointsCount { get; set; }
        internal int PolygonsCount { get; set; }
        internal int? ConnectorCount { get; set; }
        internal Connector[] Connectors { get; set; }
    }

    [Validator(typeof(PieValidator))]
    public class Pie
    {
        public string FileName { get; set; }
        public int Version { get; set; }
        public int Type { get; set; }
        public Texture Texture { get; set; }
        public Level[] Levels { get; set; }
        internal int LevelCount { get; set; }
    }
}