using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawTileAreaOutline
    {
        //does not inherit action

        public clsMap Map;
        public sRGBA_sng Colour;
        public XYInt StartXY;
        public XYInt FinishXY;

        private XYZInt Vertex0;
        private XYZInt Vertex1;

        public void ActionPerform()
        {
            int X = 0;
            int Y = 0;

            GL.Begin(BeginMode.Lines);
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
            for ( X = StartXY.X; X <= FinishXY.X - 1; X++ )
            {
                Vertex0.X = X * App.TerrainGridSpacing;
                Vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[X, StartXY.Y].Height * Map.HeightMultiplier);
                Vertex0.Z = - StartXY.Y * App.TerrainGridSpacing;
                Vertex1.X = (X + 1) * App.TerrainGridSpacing;
                Vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[X + 1, StartXY.Y].Height * Map.HeightMultiplier);
                Vertex1.Z = - StartXY.Y * App.TerrainGridSpacing;
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToInt32(- Vertex0.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToInt32(- Vertex1.Z));
            }
            for ( X = StartXY.X; X <= FinishXY.X - 1; X++ )
            {
                Vertex0.X = X * App.TerrainGridSpacing;
                Vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[X, FinishXY.Y].Height * Map.HeightMultiplier);
                Vertex0.Z = - FinishXY.Y * App.TerrainGridSpacing;
                Vertex1.X = (X + 1) * App.TerrainGridSpacing;
                Vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[X + 1, FinishXY.Y].Height * Map.HeightMultiplier);
                Vertex1.Z = - FinishXY.Y * App.TerrainGridSpacing;
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToInt32(- Vertex0.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToInt32(- Vertex1.Z));
            }
            for ( Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++ )
            {
                Vertex0.X = StartXY.X * App.TerrainGridSpacing;
                Vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[StartXY.X, Y].Height * Map.HeightMultiplier);
                Vertex0.Z = - Y * App.TerrainGridSpacing;
                Vertex1.X = StartXY.X * App.TerrainGridSpacing;
                Vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[StartXY.X, Y + 1].Height * Map.HeightMultiplier);
                Vertex1.Z = - (Y + 1) * App.TerrainGridSpacing;
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToInt32(- Vertex0.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToInt32(- Vertex1.Z));
            }
            for ( Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++ )
            {
                Vertex0.X = FinishXY.X * App.TerrainGridSpacing;
                Vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[FinishXY.X, Y].Height * Map.HeightMultiplier);
                Vertex0.Z = - Y * App.TerrainGridSpacing;
                Vertex1.X = FinishXY.X * App.TerrainGridSpacing;
                Vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[FinishXY.X, Y + 1].Height * Map.HeightMultiplier);
                Vertex1.Z = - (Y + 1) * App.TerrainGridSpacing;
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToInt32(- Vertex0.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToInt32(- Vertex1.Z));
            }
            GL.End();
        }
    }
}