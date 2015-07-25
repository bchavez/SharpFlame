using System;
using System.Collections.Generic;
using SharpFlame.Core.Extensions;


namespace SharpFlame.Core.Parsers.Json
{
    public enum Passability
    {
        Ground = 'g',
        Road = 'r',
        Water = 'w',
        Cliff = 'c'
    }

    [Flags]
    public enum Orentation
    {
        XFlip = 0x8,
        YFlip = 0x4,
        Rotate0   = 0x00,
        Rotate90  = 0x01,
        Rotate180 = 0x02,
        Rotate270 = 0x03
    }

    public class Decal
    {
        //2 bytes
        public string Id { get; set; }
        //
        public Orentation RotateFlags { get; set; }

        public bool FlipX 
        {
            get { return this.RotateFlags.HasFlag(Orentation.XFlip); }
        }
        public bool FlipY
        {
            get { return this.RotateFlags.HasFlag(Orentation.YFlip); }
        }

        public int Rotation
        {
            get
            {
                if( RotateFlags.HasFlag(Orentation.Rotate0) )
                {
                    return 0;
                }
                return 0;
            }
        }

    }

    public class Ground : TexturePath
    {
        public decimal Scale { get; set; }
    }

    public class HeightMap
    {
        
    }

    public class TilesetJson
    {
        public Dictionary<string, TexturePath> Decals { get; set; }
        public Dictionary<string, Ground> Ground { get; set; }
    }

    public class TexturePath
    {
        public string Texture { get; set; }
    }

    public class GameJson
    {
        public Decal[][] Decals { get; set; }
        public Ground[][] Ground { get; set; }
        public HeightMap[][] HeightMap { get; set; }
        public MidpointMap[][] MidpointMap { get; set; }
        public TileType[][] TileTypes { get; set; }
    }


    public class TileType
    {
        
    }
    public class MidpointMap
    {
    }
}