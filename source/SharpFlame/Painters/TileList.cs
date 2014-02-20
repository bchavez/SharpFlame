#region

using System;
using SharpFlame.Mapping.Tiles;

#endregion

namespace SharpFlame.Painters
{
    public class TileList
    {
        public int TileChanceTotal;
        public int TileCount;
        public TileOrientationChance[] Tiles;

        public void Tile_Add(int TileNum, TileDirection TileOutwardOrientation, UInt32 Chance)
        {
            Array.Resize(ref Tiles, TileCount + 1);
            Tiles[TileCount].TextureNum = TileNum;
            Tiles[TileCount].Direction = TileOutwardOrientation;
            Tiles[TileCount].Chance = Chance;
            TileCount++;

            TileChanceTotal += Convert.ToInt32(Chance);
        }

        public void Tile_Remove(int Num)
        {
            TileChanceTotal -= Convert.ToInt32(Tiles[Num].Chance);

            TileCount--;
            if ( Num != TileCount )
            {
                Tiles[Num] = Tiles[TileCount];
            }
            Array.Resize(ref Tiles, TileCount);
        }

        public TileOrientationChance GetRandom()
        {
            var ReturnResult = new TileOrientationChance();
            var A = 0;
            var intRandom = 0;
            var Total = 0;

            var rnd = new Random();
            intRandom = rnd.Next() * TileChanceTotal;
            for ( A = 0; A <= TileCount - 1; A++ )
            {
                Total += Convert.ToInt32(Tiles[A].Chance);
                if ( intRandom < Total )
                {
                    break;
                }
            }
            if ( A == TileCount )
            {
                ReturnResult.TextureNum = -1;
                ReturnResult.Direction = TileUtil.None;
            }
            else
            {
                ReturnResult = Tiles[A];
            }
            return ReturnResult;
        }
    }
}