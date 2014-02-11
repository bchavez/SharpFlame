using System;
using Microsoft.VisualBasic;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyCliff : clsAction
    {
        public double Angle;
        public bool SetTris;

        private int RandomNum;
        private double DifA;
        private double DifB;
        private double HeightA;
        private double HeightB;
        private double TriTopLeftMaxSlope;
        private double TriTopRightMaxSlope;
        private double TriBottomLeftMaxSlope;
        private double TriBottomRightMaxSlope;
        private bool CliffChanged;
        private bool TriChanged;
        private bool NewVal;
        private clsTerrain Terrain;
        private sXY_int Pos;

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            HeightA = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height) / 2.0D);
            HeightB = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) / 2.0D);
            DifA = HeightB - HeightA;
            HeightA = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) / 2.0D);
            HeightB = Convert.ToDouble(((Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) / 2.0D);
            DifB = HeightB - HeightA;
            if ( Math.Abs(DifA) == Math.Abs(DifB) )
            {
                RandomNum = (int)(Conversion.Int(App.Random.Next() * 4.0F));
                if ( RandomNum == 0 )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Top;
                }
                else if ( RandomNum == 1 )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Right;
                }
                else if ( RandomNum == 2 )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Bottom;
                }
                else
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Left;
                }
            }
            else if ( Math.Abs(DifA) > Math.Abs(DifB) )
            {
                if ( DifA < 0.0D )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Bottom;
                }
                else
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Top;
                }
            }
            else
            {
                if ( DifB < 0.0D )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Right;
                }
                else
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Left;
                }
            }

            CliffChanged = false;
            TriChanged = false;

            if ( SetTris )
            {
                DifA = Math.Abs((Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) - Terrain.Vertices[PosNum.X, PosNum.Y].Height);
                DifB = Math.Abs((Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) - Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height);
                if ( DifA == DifB )
                {
                    if ( App.Random.Next() >= 0.5F )
                    {
                        NewVal = false;
                    }
                    else
                    {
                        NewVal = true;
                    }
                }
                else if ( DifA < DifB )
                {
                    NewVal = false;
                }
                else
                {
                    NewVal = true;
                }
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri != NewVal )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Tri = NewVal;
                    TriChanged = true;
                }
            }

            if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri )
            {
                Pos.X = (int)((PosNum.X + 0.25D) * App.TerrainGridSpacing);
                Pos.Y = (int)((PosNum.Y + 0.25D) * App.TerrainGridSpacing);
                TriTopLeftMaxSlope = Map.GetTerrainSlopeAngle(Pos);
                Pos.X = (int)((PosNum.X + 0.75D) * App.TerrainGridSpacing);
                Pos.Y = (int)((PosNum.Y + 0.75D) * App.TerrainGridSpacing);
                TriBottomRightMaxSlope = Map.GetTerrainSlopeAngle(Pos);

                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;
                    CliffChanged = true;
                }
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
                    CliffChanged = true;
                }

                NewVal = TriTopLeftMaxSlope >= Angle;
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff != NewVal )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = NewVal;
                    CliffChanged = true;
                }

                NewVal = TriBottomRightMaxSlope >= Angle;
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff != NewVal )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = NewVal;
                    CliffChanged = true;
                }

                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff || Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                }
                else
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = false;
                }
            }
            else
            {
                Pos.X = (int)((PosNum.X + 0.75D) * App.TerrainGridSpacing);
                Pos.Y = (int)((PosNum.Y + 0.25D) * App.TerrainGridSpacing);
                TriTopRightMaxSlope = Map.GetTerrainSlopeAngle(Pos);
                Pos.X = (int)((PosNum.X + 0.25D) * App.TerrainGridSpacing);
                Pos.Y = (int)((PosNum.Y + 0.75D) * App.TerrainGridSpacing);
                TriBottomLeftMaxSlope = Map.GetTerrainSlopeAngle(Pos);

                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
                    CliffChanged = true;
                }
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
                    CliffChanged = true;
                }

                NewVal = TriTopRightMaxSlope >= Angle;
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff != NewVal )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = NewVal;
                    CliffChanged = true;
                }

                NewVal = TriBottomLeftMaxSlope >= Angle;
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff != NewVal )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = NewVal;
                    CliffChanged = true;
                }

                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff || Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                }
                else
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = false;
                }
            }

            if ( CliffChanged )
            {
                Map.AutoTextureChanges.TileChanged(PosNum);
            }
            if ( TriChanged || CliffChanged )
            {
                Map.SectorGraphicsChanges.TileChanged(PosNum);
                Map.SectorTerrainUndoChanges.TileChanged(PosNum);
            }
        }
    }
}