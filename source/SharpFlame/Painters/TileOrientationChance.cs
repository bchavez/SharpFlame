#region

using System;
using SharpFlame.Mapping.Tiles;

#endregion

namespace SharpFlame.Painters
{
    public struct TileOrientationChance
    {
        public UInt32 Chance;
        public TileDirection Direction;
        public int TextureNum;
    }
}