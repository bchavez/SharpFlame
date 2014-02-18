using System;
using System.Diagnostics;
using SharpFlame.Core.Domain;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyHeightSmoothing : clsAction
    {
        public double Ratio;
        public XYInt Offset;
        public XYInt AreaTileSize;

        private byte[,] NewHeight;
        private bool Started;
        private int TempHeight;
        private int Samples;
        private int LimitX;
        private int LimitY;
        private int XNum;
        private XYInt VertexNum;
        private clsTerrain Terrain;

        public void Start()
        {
            int X = 0;
            int Y = 0;

            Terrain = Map.Terrain;

            NewHeight = new byte[AreaTileSize.X + 1, AreaTileSize.Y + 1];
            for ( Y = 0; Y <= AreaTileSize.Y; Y++ )
            {
                for ( X = 0; X <= AreaTileSize.X; X++ )
                {
                    NewHeight[X, Y] = Terrain.Vertices[Offset.X + X, Offset.Y + Y].Height;
                }
            }

            Started = true;
        }

        public void Finish()
        {
            if ( !Started )
            {
                Debugger.Break();
                return;
            }

            int X = 0;
            int Y = 0;

            Terrain = Map.Terrain;

            for ( Y = 0; Y <= AreaTileSize.Y; Y++ )
            {
                VertexNum.Y = Offset.Y + Y;
                for ( X = 0; X <= AreaTileSize.X; X++ )
                {
                    VertexNum.X = Offset.X + X;
                    Terrain.Vertices[VertexNum.X, VertexNum.Y].Height = NewHeight[X, Y];

                    Map.SectorGraphicsChanges.VertexAndNormalsChanged(VertexNum);
                    Map.SectorUnitHeightsChanges.VertexChanged(VertexNum);
                    Map.SectorTerrainUndoChanges.VertexChanged(VertexNum);
                }
            }

            Started = false;
        }

        public override void ActionPerform()
        {
            if ( !Started )
            {
                Debugger.Break();
                return;
            }

            int X = 0;
            int Y = 0;
            int X2 = 0;
            int Y2 = 0;

            Terrain = Map.Terrain;

            LimitX = Terrain.TileSize.X;
            LimitY = Terrain.TileSize.Y;
            TempHeight = 0;
            Samples = 0;
            for ( Y = MathUtil.Clamp_int(App.SmoothRadius.Tiles.YMin + PosNum.Y, 0, LimitY) - PosNum.Y;
                Y <= MathUtil.Clamp_int(App.SmoothRadius.Tiles.YMax + PosNum.Y, 0, LimitY) - PosNum.Y;
                Y++ )
            {
                Y2 = PosNum.Y + Y;
                XNum = Y - App.SmoothRadius.Tiles.YMin;
                for ( X = MathUtil.Clamp_int(Convert.ToInt32(App.SmoothRadius.Tiles.XMin[XNum] + PosNum.X), 0, LimitX) - PosNum.X;
                    X <= MathUtil.Clamp_int(Convert.ToInt32(App.SmoothRadius.Tiles.XMax[XNum] + PosNum.X), 0, LimitX) - PosNum.X;
                    X++ )
                {
                    X2 = PosNum.X + X;
                    TempHeight += Terrain.Vertices[X2, Y2].Height;
                    Samples++;
                }
            }
            NewHeight[PosNum.X - Offset.X, PosNum.Y - Offset.Y] =
                Math.Min((byte)(Convert.ToInt32(Terrain.Vertices[PosNum.X, PosNum.Y].Height * (1.0D - Ratio) + TempHeight / (double)Samples * Ratio)), Byte.MaxValue);
        }
    }
}