namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Runtime.InteropServices;

    public class clsPainter
    {
        public int CliffBrushCount;
        public clsCliff_Brush[] CliffBrushes;
        public int RoadBrushCount;
        public clsRoad_Brush[] RoadBrushes;
        public int RoadCount;
        public clsRoad[] Roads;
        public int TerrainCount;
        public clsTerrain[] Terrains;
        public int TransitionBrushCount;
        public clsTransition_Brush[] TransitionBrushes;

        public void CliffBrush_Add(clsCliff_Brush NewBrush)
        {
            this.CliffBrushes = (clsCliff_Brush[]) Utils.CopyArray((Array) this.CliffBrushes, new clsCliff_Brush[this.CliffBrushCount + 1]);
            this.CliffBrushes[this.CliffBrushCount] = NewBrush;
            this.CliffBrushCount++;
        }

        public void CliffBrush_Remove(int Num)
        {
            this.CliffBrushCount--;
            if (Num != this.CliffBrushCount)
            {
                this.CliffBrushes[Num] = this.CliffBrushes[this.CliffBrushCount];
            }
            this.CliffBrushes = (clsCliff_Brush[]) Utils.CopyArray((Array) this.CliffBrushes, new clsCliff_Brush[(this.CliffBrushCount - 1) + 1]);
        }

        public void Road_Add(clsRoad NewRoad)
        {
            NewRoad.Num = this.RoadCount;
            this.Roads = (clsRoad[]) Utils.CopyArray((Array) this.Roads, new clsRoad[this.RoadCount + 1]);
            this.Roads[this.RoadCount] = NewRoad;
            this.RoadCount++;
        }

        public void Road_Remove(int Num)
        {
            this.Roads[Num].Num = -1;
            this.RoadCount--;
            if (Num != this.RoadCount)
            {
                this.Roads[Num] = this.Roads[this.RoadCount];
                this.Roads[Num].Num = Num;
            }
            this.Roads = (clsRoad[]) Utils.CopyArray((Array) this.Roads, new clsRoad[(this.RoadCount - 1) + 1]);
        }

        public void RoadBrush_Add(clsRoad_Brush NewRoadBrush)
        {
            this.RoadBrushes = (clsRoad_Brush[]) Utils.CopyArray((Array) this.RoadBrushes, new clsRoad_Brush[this.RoadBrushCount + 1]);
            this.RoadBrushes[this.RoadBrushCount] = NewRoadBrush;
            this.RoadBrushCount++;
        }

        public void RoadBrush_Remove(int Num)
        {
            this.RoadBrushCount--;
            if (Num != this.RoadBrushCount)
            {
                this.RoadBrushes[Num] = this.RoadBrushes[this.RoadBrushCount];
            }
            this.RoadBrushes = (clsRoad_Brush[]) Utils.CopyArray((Array) this.RoadBrushes, new clsRoad_Brush[(this.RoadBrushCount - 1) + 1]);
        }

        public void Terrain_Add(clsTerrain NewTerrain)
        {
            NewTerrain.Num = this.TerrainCount;
            this.Terrains = (clsTerrain[]) Utils.CopyArray((Array) this.Terrains, new clsTerrain[this.TerrainCount + 1]);
            this.Terrains[this.TerrainCount] = NewTerrain;
            this.TerrainCount++;
        }

        public void Terrain_Remove(int Num)
        {
            this.Terrains[Num].Num = -1;
            this.TerrainCount--;
            if (Num != this.TerrainCount)
            {
                this.Terrains[Num] = this.Terrains[this.TerrainCount];
                this.Terrains[Num].Num = Num;
            }
            this.Terrains = (clsTerrain[]) Utils.CopyArray((Array) this.Terrains, new clsTerrain[(this.TerrainCount - 1) + 1]);
        }

        public void TransitionBrush_Add(clsTransition_Brush NewBrush)
        {
            this.TransitionBrushes = (clsTransition_Brush[]) Utils.CopyArray((Array) this.TransitionBrushes, new clsTransition_Brush[this.TransitionBrushCount + 1]);
            this.TransitionBrushes[this.TransitionBrushCount] = NewBrush;
            this.TransitionBrushCount++;
        }

        public void TransitionBrush_Remove(int Num)
        {
            this.TransitionBrushCount--;
            if (Num != this.TransitionBrushCount)
            {
                this.TransitionBrushes[Num] = this.TransitionBrushes[this.TransitionBrushCount];
            }
            this.TransitionBrushes = (clsTransition_Brush[]) Utils.CopyArray((Array) this.TransitionBrushes, new clsTransition_Brush[(this.TransitionBrushCount - 1) + 1]);
        }

        public class clsCliff_Brush
        {
            public string Name;
            public clsPainter.clsTerrain Terrain_Inner;
            public clsPainter.clsTerrain Terrain_Outer;
            public clsPainter.clsTileList Tiles_Corner_In = new clsPainter.clsTileList();
            public clsPainter.clsTileList Tiles_Corner_Out = new clsPainter.clsTileList();
            public clsPainter.clsTileList Tiles_Straight = new clsPainter.clsTileList();
        }

        public class clsRoad
        {
            public string Name;
            public int Num;
        }

        public class clsRoad_Brush
        {
            public clsPainter.clsRoad Road;
            public clsPainter.clsTerrain Terrain;
            public clsPainter.clsTileList Tile_Corner_In = new clsPainter.clsTileList();
            public clsPainter.clsTileList Tile_CrossIntersection = new clsPainter.clsTileList();
            public clsPainter.clsTileList Tile_End = new clsPainter.clsTileList();
            public clsPainter.clsTileList Tile_Straight = new clsPainter.clsTileList();
            public clsPainter.clsTileList Tile_TIntersection = new clsPainter.clsTileList();
        }

        public class clsTerrain
        {
            public string Name;
            public int Num;
            public clsPainter.clsTileList Tiles = new clsPainter.clsTileList();
        }

        public class clsTileList
        {
            public int TileChanceTotal;
            public int TileCount;
            public sTileOrientationChance[] Tiles;

            public sTileOrientationChance GetRandom()
            {
                int num2 = (int) Math.Round((double) ((float) (App.Random.Next() * this.TileChanceTotal)));
                int num4 = this.TileCount - 1;
                int index = 0;
                while (index <= num4)
                {
                    int num3;
                    num3 += (int) this.Tiles[index].Chance;
                    if (num2 < num3)
                    {
                        break;
                    }
                    index++;
                }
                if (index == this.TileCount)
                {
                    sTileOrientationChance chance2;
                    chance2.TextureNum = -1;
                    chance2.Direction = TileOrientation.TileDirection_None;
                    return chance2;
                }
                return this.Tiles[index];
            }

            public void Tile_Add(int TileNum, TileOrientation.sTileDirection TileOutwardOrientation, uint Chance)
            {
                this.Tiles = (sTileOrientationChance[]) Utils.CopyArray((Array) this.Tiles, new sTileOrientationChance[this.TileCount + 1]);
                this.Tiles[this.TileCount].TextureNum = TileNum;
                this.Tiles[this.TileCount].Direction = TileOutwardOrientation;
                this.Tiles[this.TileCount].Chance = Chance;
                this.TileCount++;
                this.TileChanceTotal += (int) Chance;
            }

            public void Tile_Remove(int Num)
            {
                this.TileChanceTotal -= (int) this.Tiles[Num].Chance;
                this.TileCount--;
                if (Num != this.TileCount)
                {
                    this.Tiles[Num] = this.Tiles[this.TileCount];
                }
                this.Tiles = (sTileOrientationChance[]) Utils.CopyArray((Array) this.Tiles, new sTileOrientationChance[(this.TileCount - 1) + 1]);
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct sTileOrientationChance
            {
                public int TextureNum;
                public TileOrientation.sTileDirection Direction;
                public uint Chance;
            }
        }

        public class clsTransition_Brush
        {
            public string Name;
            public clsPainter.clsTerrain Terrain_Inner;
            public clsPainter.clsTerrain Terrain_Outer;
            public clsPainter.clsTileList Tiles_Corner_In = new clsPainter.clsTileList();
            public clsPainter.clsTileList Tiles_Corner_Out = new clsPainter.clsTileList();
            public clsPainter.clsTileList Tiles_Straight = new clsPainter.clsTileList();
        }
    }
}

