using System;
using Microsoft.VisualBasic;

namespace SharpFlame.Painters
{
    public class TileList
    {
        public TileOrientationChance[] Tiles;
        public int TileCount;
        public int TileChanceTotal;

        public void Tile_Add(int TileNum, TileOrientation.sTileDirection TileOutwardOrientation, UInt32 Chance)
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
            TileOrientationChance ReturnResult = new TileOrientationChance();
            int A = 0;
            int intRandom = 0;
            int Total = 0;

            intRandom = (int)(Conversion.Int(VBMath.Rnd() * TileChanceTotal));
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
                ReturnResult.Direction = TileOrientation.TileDirection_None;
            }
            else
            {
                ReturnResult = Tiles[A];
            }
            return ReturnResult;
        }
    }
}