#region

using System;
using SharpFlame.Old.Mapping.Tiles;
using SharpFlame.Old.Mapping.Tiles;

#endregion

namespace SharpFlame.Old.Painters
{
    public struct TileOrientationChance
    {
        public UInt32 Chance;
        public TileDirection Direction;
        public int TextureNum;
    }
}