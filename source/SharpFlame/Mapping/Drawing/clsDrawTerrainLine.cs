#region

using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawTerrainLine
    {
        //does not inherit action

        private readonly XYZInt vertex;
        public SRgba Colour;
        private XYInt FinishTile;
        public XYInt FinishXY;
        private XYInt Horizontal;
        private MathUtil.sIntersectPos IntersectX;
        private MathUtil.sIntersectPos IntersectY;
        private int LastXTile;
        public Map Map;
        private XYInt StartTile;
        public XYInt StartXY;
        private XYInt TileEdgeFinish;
        private XYInt TileEdgeStart;

        public clsDrawTerrainLine()
        {
            vertex = new XYZInt(0, 0, 0);
        }

        public void ActionPerform()
        {
            var X = 0;
            var Y = 0;

            GL.Begin(BeginMode.LineStrip);
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);

            StartTile.Y = StartXY.Y / Constants.TerrainGridSpacing;
            FinishTile.Y = FinishXY.Y / Constants.TerrainGridSpacing;
            LastXTile = StartXY.X / Constants.TerrainGridSpacing;

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
                    TileEdgeStart.Y = Y * Constants.TerrainGridSpacing;
                    TileEdgeFinish.X = Map.Terrain.TileSize.X * Constants.TerrainGridSpacing;
                    TileEdgeFinish.Y = Y * Constants.TerrainGridSpacing;
                    IntersectY = MathUtil.GetLinesIntersectBetween(StartXY, FinishXY, TileEdgeStart, TileEdgeFinish);
                    if ( IntersectY.Exists )
                    {
                        StartTile.X = LastXTile;
                        FinishTile.X = IntersectY.Pos.X / Constants.TerrainGridSpacing;

                        for ( X = StartTile.X + 1; X <= FinishTile.X; X++ )
                        {
                            TileEdgeStart.X = X * Constants.TerrainGridSpacing;
                            TileEdgeStart.Y = 0;
                            TileEdgeFinish.X = X * Constants.TerrainGridSpacing;
                            TileEdgeFinish.Y = Map.Terrain.TileSize.Y * Constants.TerrainGridSpacing;
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
                FinishTile.X = FinishXY.X / Constants.TerrainGridSpacing;
                for ( X = StartTile.X + 1; X <= FinishTile.X; X++ )
                {
                    TileEdgeStart.X = X * Constants.TerrainGridSpacing;
                    TileEdgeStart.Y = 0;
                    TileEdgeFinish.X = X * Constants.TerrainGridSpacing;
                    TileEdgeFinish.Y = Map.Terrain.TileSize.Y * Constants.TerrainGridSpacing;
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