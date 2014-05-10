

using System;
using SharpFlame.Mapping.Tiles;


namespace SharpFlame.Painters
{
    public class TileList
    {
        public int TileChanceTotal;
        public int TileCount;
        public TileOrientationChance[] Tiles;

        public void TileAdd(int tileNum, TileDirection tileOutwardOrientation, UInt32 chance)
        {
            Array.Resize(ref Tiles, TileCount + 1);
            Tiles[TileCount].TextureNum = tileNum;
            Tiles[TileCount].Direction = tileOutwardOrientation;
            Tiles[TileCount].Chance = chance;
            TileCount++;

            TileChanceTotal += Convert.ToInt32(chance);
        }

        public void TileRemove(int Num)
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
            var returnResult = new TileOrientationChance();
            var a = 0;
            var intRandom = 0;
            var total = 0;

            var rnd = new Random();
            intRandom = rnd.Next() * TileChanceTotal;
            for ( a = 0; a <= TileCount - 1; a++ )
            {
                total += Convert.ToInt32(Tiles[a].Chance);
                if ( intRandom < total )
                {
                    break;
                }
            }
            if ( a == TileCount )
            {
                returnResult.TextureNum = -1;
                returnResult.Direction = TileUtil.None;
            }
            else
            {
                returnResult = Tiles[a];
            }
            return returnResult;
        }
    }
}