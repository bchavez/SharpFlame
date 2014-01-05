using System;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Tiles;

namespace SharpFlame.Painters
{
    public struct TileOrientationChance
    {
        public int TextureNum;
        public TileDirection Direction;
        public UInt32 Chance;
    }
}