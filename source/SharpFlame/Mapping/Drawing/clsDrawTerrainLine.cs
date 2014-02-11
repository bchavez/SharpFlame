using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawTerrainLine
    {
        //does not inherit action

        public clsMap Map;
        public sRGBA_sng Colour;
        public sXY_int StartXY;
        public sXY_int FinishXY;

        private sXYZ_int Vertex;
        private sXY_int StartTile;
        private sXY_int FinishTile;
        private MathUtil.sIntersectPos IntersectX;
        private MathUtil.sIntersectPos IntersectY;
        private sXY_int TileEdgeStart;
        private sXY_int TileEdgeFinish;
        private int LastXTile;
        private sXY_int Horizontal;

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
            Vertex.X = Horizontal.X;
            Vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
            Vertex.Z = Convert.ToInt32(- Horizontal.Y);
            GL.Vertex3(Vertex.X, Vertex.Y, Convert.ToInt32(- Vertex.Z));

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
                                Vertex.X = Horizontal.X;
                                Vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
                                Vertex.Z = Convert.ToInt32(- Horizontal.Y);
                                GL.Vertex3(Vertex.X, Vertex.Y, Convert.ToInt32(- Vertex.Z));
                            }
                        }

                        LastXTile = FinishTile.X;

                        Horizontal = IntersectY.Pos;
                        Vertex.X = Horizontal.X;
                        Vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
                        Vertex.Z = Convert.ToInt32(- Horizontal.Y);
                        GL.Vertex3(Vertex.X, Vertex.Y, Convert.ToInt32(- Vertex.Z));
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
                        Vertex.X = Horizontal.X;
                        Vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
                        Vertex.Z = Convert.ToInt32(- Horizontal.Y);
                        GL.Vertex3(Vertex.X, Vertex.Y, Convert.ToInt32(- Vertex.Z));
                    }
                }
            }

            Horizontal = FinishXY;
            Vertex.X = Horizontal.X;
            Vertex.Y = (int)(Map.GetTerrainHeight(Horizontal));
            Vertex.Z = Convert.ToInt32(- Horizontal.Y);
            GL.Vertex3(Vertex.X, Vertex.Y, Convert.ToInt32(- Vertex.Z));

            GL.End();
        }
    }
}