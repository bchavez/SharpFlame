using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawTerrainLine
    {
        //does not inherit action

        public clsMap Map;
        public sRGBA_sng Colour;
        public XYInt StartXY;
        public XYInt FinishXY;

        private XYZInt vertex;
        private XYInt StartTile;
        private XYInt FinishTile;
        private MathUtil.sIntersectPos IntersectX;
        private MathUtil.sIntersectPos IntersectY;
        private XYInt TileEdgeStart;
        private XYInt TileEdgeFinish;
        private int LastXTile;
        private XYInt Horizontal;

		public clsDrawTerrainLine()
		{
			vertex = new XYZInt (0, 0, 0);
		}

        public void ActionPerform()
        {
            int X = 0;
            int Y = 0;

            GL.Begin(BeginMode.LineStrip);
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);

            StartTile.Y = (int)(StartXY.Y / App.TerrainGridSpacing);
            FinishTile.Y = FinishXY.Y / App.TerrainGridSpacing;
            LastXTile = StartXY.X / App.TerrainGridSpacing;

            Horizontal = StartXY;
            vertex.X = Horizontal.X;
            vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
            vertex.Z = Convert.ToInt32(- Horizontal.Y);
            GL.Vertex3(vertex.X, vertex.Y, Convert.ToInt32(- vertex.Z));

            if ( StartTile.Y + 1 <= FinishTile.Y )
            {
                for ( Y = StartTile.Y + 1; Y <= FinishTile.Y; Y++ )
                {
                    TileEdgeStart.X = 0;
                    TileEdgeStart.Y = Y * App.TerrainGridSpacing;
                    TileEdgeFinish.X = Map.Terrain.TileSize.X * App.TerrainGridSpacing;
                    TileEdgeFinish.Y = Y * App.TerrainGridSpacing;
                    IntersectY = MathUtil.GetLinesIntersectBetween(StartXY, FinishXY, TileEdgeStart, TileEdgeFinish);
                    if ( IntersectY.Exists )
                    {
                        StartTile.X = LastXTile;
                        FinishTile.X = (int)(IntersectY.Pos.X / App.TerrainGridSpacing);

                        for ( X = StartTile.X + 1; X <= FinishTile.X; X++ )
                        {
                            TileEdgeStart.X = X * App.TerrainGridSpacing;
                            TileEdgeStart.Y = 0;
                            TileEdgeFinish.X = X * App.TerrainGridSpacing;
                            TileEdgeFinish.Y = Map.Terrain.TileSize.Y * App.TerrainGridSpacing;
                            IntersectX = MathUtil.GetLinesIntersectBetween(StartXY, FinishXY, TileEdgeStart, TileEdgeFinish);
                            if ( IntersectX.Exists )
                            {
                                Horizontal = IntersectX.Pos;
                                vertex.X = Horizontal.X;
                                vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
                                vertex.Z = Convert.ToInt32(- Horizontal.Y);
                                GL.Vertex3(vertex.X, vertex.Y, Convert.ToInt32(- vertex.Z));
                            }
                        }

                        LastXTile = FinishTile.X;

                        Horizontal = IntersectY.Pos;
                        vertex.X = Horizontal.X;
                        vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
                        vertex.Z = Convert.ToInt32(- Horizontal.Y);
                        GL.Vertex3(vertex.X, vertex.Y, Convert.ToInt32(- vertex.Z));
                    }
                }
            }
            else
            {
                StartTile.X = LastXTile;
                FinishTile.X = FinishXY.X / App.TerrainGridSpacing;
                for ( X = StartTile.X + 1; X <= FinishTile.X; X++ )
                {
                    TileEdgeStart.X = X * App.TerrainGridSpacing;
                    TileEdgeStart.Y = 0;
                    TileEdgeFinish.X = X * App.TerrainGridSpacing;
                    TileEdgeFinish.Y = Map.Terrain.TileSize.Y * App.TerrainGridSpacing;
                    IntersectX = MathUtil.GetLinesIntersectBetween(StartXY, FinishXY, TileEdgeStart, TileEdgeFinish);
                    if ( IntersectX.Exists )
                    {
                        Horizontal = IntersectX.Pos;
                        vertex.X = Horizontal.X;
                        vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
                        vertex.Z = Convert.ToInt32(- Horizontal.Y);
                        GL.Vertex3(vertex.X, vertex.Y, Convert.ToInt32(- vertex.Z));
                    }
                }
            }

            Horizontal = FinishXY;
            vertex.X = Horizontal.X;
            vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
            vertex.Z = Convert.ToInt32(- Horizontal.Y);
            GL.Vertex3(vertex.X, vertex.Y, Convert.ToInt32(- vertex.Z));

            GL.End();
        }
    }
}