using FileHelpers;

namespace SharpFlame.Core.FileIO
{
    //PIE 2
    [DelimitedRecord( " " )]
    public class Header
    {
        [FieldTrim( TrimMode.Left, "PIE" )]
        public int Version;
    }

    [DelimitedRecord( " " )]
    public class TypeDirective
    {
        [FieldTrim( TrimMode.Left, "TYPE" )]
        public int Type;
    }

    [DelimitedRecord( " " )]
    public class TextureDirective
    {
        //[FieldIgnored]
        //[FieldTrim( TrimMode.Left, "TEXTURE" )]
        public string Ignore;
        public string Ignor2;
        public string Path;
        public int Width;
        public int Height;
    }
    [DelimitedRecord( " " )]
    public class LevelsDirective
    {
        [FieldTrim( TrimMode.Left, "LEVELS" )]
        public int Levels;
    }
    [DelimitedRecord( " " )]
    public class LevelDirective
    {
        [FieldTrim( TrimMode.Left, "LEVEL" )]
        public int Level;
    }
    [DelimitedRecord( " " )]
    public class PointsDirective
    {
        [FieldTrim( TrimMode.Left, "POINTS" )]
        public int Points;
    }

    [DelimitedRecord(" ")]
    public class PointRecord
    {
        public int X;
        public int Y;
        public int Z;
    }

    [DelimitedRecord(" ")]
    public class PolygonsDirective
    {
        [FieldTrim( TrimMode.Left, "POLYGONS" )]
        public int Polygons;
    }

    [DelimitedRecord( " " )]
    public class PolygonRecord
    {
        public uint Flags;
        public uint PointCount;
        public int P1;
        public int P2;
        public int P3;
        public int Frames;
        public int Rate;
        public int Width;
        public int Height;
        public int TexX;
        public int TexY;
    }


}