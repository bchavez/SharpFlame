#region

using System;
using System.Diagnostics;
using SharpFlame.Core.Domain;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyHeightSmoothing : clsAction
    {
        public XYInt AreaTileSize;
        private int LimitX;
        private int LimitY;

        private byte[,] NewHeight;
        public XYInt Offset;
        public double Ratio;
        private int Samples;
        private bool Started;
        private int TempHeight;
        private clsTerrain Terrain;
        private XYInt VertexNum;
        private int XNum;

        public void Start()
        {
            var X = 0;
            var Y = 0;

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

            var X = 0;
            var Y = 0;

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

            var X = 0;
            var Y = 0;
            var X2 = 0;
            var Y2 = 0;

            Terrain = Map.Terrain;

            LimitX = Terrain.TileSize.X;
            LimitY = Terrain.TileSize.Y;
            TempHeight = 0;
            Samples = 0;
            for ( Y = MathUtil.ClampInt(App.SmoothRadius.Tiles.YMin + PosNum.Y, 0, LimitY) - PosNum.Y;
                Y <= MathUtil.ClampInt(App.SmoothRadius.Tiles.YMax + PosNum.Y, 0, LimitY) - PosNum.Y;
                Y++ )
            {
                Y2 = PosNum.Y + Y;
                XNum = Y - App.SmoothRadius.Tiles.YMin;
                for ( X = MathUtil.ClampInt(Convert.ToInt32(App.SmoothRadius.Tiles.XMin[XNum] + PosNum.X), 0, LimitX) - PosNum.X;
                    X <= MathUtil.ClampInt(Convert.ToInt32(App.SmoothRadius.Tiles.XMax[XNum] + PosNum.X), 0, LimitX) - PosNum.X;
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