using System;
using Microsoft.VisualBasic;
using SharpFlame.Mapping.Tiles;

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyCliffTriangle : clsAction
    {
        public bool Triangle;

        private clsTerrain Terrain;
        private bool CliffChanged;

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;

            CliffChanged = false;
            if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri )
            {
                if ( Triangle )
                {
                    if ( !Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                        CliffChanged = true;
                    }
                }
                else
                {
                    if ( !Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                        CliffChanged = true;
                    }
                }
            }
            else
            {
                if ( Triangle )
                {
                    if ( !Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                        CliffChanged = true;
                    }
                }
                else
                {
                    if ( !Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                        CliffChanged = true;
                    }
                }
            }

            if ( !CliffChanged )
            {
                return;
            }

            double HeightA = 0;
            double HeightB = 0;
            double difA = 0;
            double difB = 0;
            int A;

            Map.AutoTextureChanges.TileChanged(PosNum);
            Map.SectorGraphicsChanges.TileChanged(PosNum);
            Map.SectorTerrainUndoChanges.TileChanged(PosNum);

            HeightA = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height) / 2.0D);
            HeightB = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) / 2.0D);
            difA = HeightB - HeightA;
            HeightA = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) / 2.0D);
            HeightB = Convert.ToDouble(((Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) / 2.0D);
            difB = HeightB - HeightA;
            if ( Math.Abs(difA) == Math.Abs(difB) )
            {
                A = (int)((App.Random.Next() * 4.0F));
                if ( A == 0 )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Top;
                }
                else if ( A == 1 )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Right;
                }
                else if ( A == 2 )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Bottom;
                }
                else
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Left;
                }
            }
            else if ( Math.Abs(difA) > Math.Abs(difB) )
            {
                if ( difA < 0.0D )
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
                if ( difB < 0.0D )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Right;
                }
                else
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Left;
                }
            }
        }
    }
}