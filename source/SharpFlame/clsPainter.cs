using System;
using Microsoft.VisualBasic;

namespace SharpFlame
{
    public class clsPainter
    {
        public class clsTerrain
        {
            public int Num;
            public string Name;

            public clsTileList Tiles = new clsTileList();
        }

        public clsTerrain[] Terrains;
        public int TerrainCount;

        public class clsTileList
        {
            public struct sTileOrientationChance
            {
                public int TextureNum;
                public TileOrientation.sTileDirection Direction;
                public UInt32 Chance;
            }

            public sTileOrientationChance[] Tiles;
            public int TileCount;
            public int TileChanceTotal;

            public void Tile_Add(int TileNum, TileOrientation.sTileDirection TileOutwardOrientation, UInt32 Chance)
            {
                Array.Resize(ref Tiles, TileCount + 1);
                Tiles[TileCount].TextureNum = TileNum;
                Tiles[TileCount].Direction = TileOutwardOrientation;
                Tiles[TileCount].Chance = Chance;
                TileCount++;

                TileChanceTotal += System.Convert.ToInt32(Chance);
            }

            public void Tile_Remove(int Num)
            {
                TileChanceTotal -= System.Convert.ToInt32(Tiles[Num].Chance);

                TileCount--;
                if ( Num != TileCount )
                {
                    Tiles[Num] = Tiles[TileCount];
                }
                Array.Resize(ref Tiles, TileCount);
            }

            public sTileOrientationChance GetRandom()
            {
                sTileOrientationChance ReturnResult = new sTileOrientationChance();
                int A = 0;
                int intRandom = 0;
                int Total = 0;

                intRandom = (int)(Conversion.Int(VBMath.Rnd() * TileChanceTotal));
                for ( A = 0; A <= TileCount - 1; A++ )
                {
                    Total += System.Convert.ToInt32(Tiles[A].Chance);
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

        public class clsTransition_Brush
        {
            public string Name;
            public clsTerrain Terrain_Inner;
            public clsTerrain Terrain_Outer;
            public clsTileList Tiles_Straight = new clsTileList();
            public clsTileList Tiles_Corner_In = new clsTileList();
            public clsTileList Tiles_Corner_Out = new clsTileList();
        }

        public clsTransition_Brush[] TransitionBrushes;
        public int TransitionBrushCount;

        public class clsCliff_Brush
        {
            public string Name;
            public clsTerrain Terrain_Inner;
            public clsTerrain Terrain_Outer;
            public clsTileList Tiles_Straight = new clsTileList();
            public clsTileList Tiles_Corner_In = new clsTileList();
            public clsTileList Tiles_Corner_Out = new clsTileList();
        }

        public clsCliff_Brush[] CliffBrushes;
        public int CliffBrushCount;

        public class clsRoad
        {
            public int Num;
            public string Name;
        }

        public clsRoad[] Roads;
        public int RoadCount;

        public class clsRoad_Brush
        {
            public clsRoad Road;
            public clsTerrain Terrain;
            public clsTileList Tile_Straight = new clsTileList();
            public clsTileList Tile_Corner_In = new clsTileList();
            public clsTileList Tile_TIntersection = new clsTileList();
            public clsTileList Tile_CrossIntersection = new clsTileList();
            public clsTileList Tile_End = new clsTileList();
        }

        public clsRoad_Brush[] RoadBrushes;
        public int RoadBrushCount;

        public void TransitionBrush_Add(clsTransition_Brush NewBrush)
        {
            Array.Resize(ref TransitionBrushes, TransitionBrushCount + 1);
            TransitionBrushes[TransitionBrushCount] = NewBrush;
            TransitionBrushCount++;
        }

        public void TransitionBrush_Remove(int Num)
        {
            TransitionBrushCount--;
            if ( Num != TransitionBrushCount )
            {
                TransitionBrushes[Num] = TransitionBrushes[TransitionBrushCount];
            }
            Array.Resize(ref TransitionBrushes, TransitionBrushCount);
        }

        public void CliffBrush_Add(clsCliff_Brush NewBrush)
        {
            Array.Resize(ref CliffBrushes, CliffBrushCount + 1);
            CliffBrushes[CliffBrushCount] = NewBrush;
            CliffBrushCount++;
        }

        public void CliffBrush_Remove(int Num)
        {
            CliffBrushCount--;
            if ( Num != CliffBrushCount )
            {
                CliffBrushes[Num] = CliffBrushes[CliffBrushCount];
            }
            Array.Resize(ref CliffBrushes, CliffBrushCount);
        }

        public void Terrain_Add(clsTerrain NewTerrain)
        {
            NewTerrain.Num = TerrainCount;
            Array.Resize(ref Terrains, TerrainCount + 1);
            Terrains[TerrainCount] = NewTerrain;
            TerrainCount++;
        }

        public void Terrain_Remove(int Num)
        {
            Terrains[Num].Num = -1;
            TerrainCount--;
            if ( Num != TerrainCount )
            {
                Terrains[Num] = Terrains[TerrainCount];
                Terrains[Num].Num = Num;
            }
            Array.Resize(ref Terrains, TerrainCount);
        }

        public void RoadBrush_Add(clsRoad_Brush NewRoadBrush)
        {
            Array.Resize(ref RoadBrushes, RoadBrushCount + 1);
            RoadBrushes[RoadBrushCount] = NewRoadBrush;
            RoadBrushCount++;
        }

        public void RoadBrush_Remove(int Num)
        {
            RoadBrushCount--;
            if ( Num != RoadBrushCount )
            {
                RoadBrushes[Num] = RoadBrushes[RoadBrushCount];
            }
            Array.Resize(ref RoadBrushes, RoadBrushCount);
        }

        public void Road_Add(clsRoad NewRoad)
        {
            NewRoad.Num = RoadCount;
            Array.Resize(ref Roads, RoadCount + 1);
            Roads[RoadCount] = NewRoad;
            RoadCount++;
        }

        public void Road_Remove(int Num)
        {
            Roads[Num].Num = -1;
            RoadCount--;
            if ( Num != RoadCount )
            {
                Roads[Num] = Roads[RoadCount];
                Roads[Num].Num = Num;
            }
            Array.Resize(ref Roads, RoadCount);
        }
    }
}