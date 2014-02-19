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

        private XYZInt vertex0;
        private XYZInt vertex1;

		public clsDrawTileAreaOutline() {
			vertex0 = new XYZInt (0, 0, 0);
			vertex1 = new XYZInt (0, 0, 0);
		}

        public void ActionPerform()
        {
            int X = 0;
            int Y = 0;

            GL.Begin(BeginMode.Lines);
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
            for ( X = StartXY.X; X <= FinishXY.X - 1; X++ )
            {
                vertex0.X = X * Constants.TerrainGridSpacing;
                vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[X, StartXY.Y].Height * Map.HeightMultiplier);
                vertex0.Z = - StartXY.Y * Constants.TerrainGridSpacing;
                vertex1.X = (X + 1) * Constants.TerrainGridSpacing;
                vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[X + 1, StartXY.Y].Height * Map.HeightMultiplier);
                vertex1.Z = - StartXY.Y * Constants.TerrainGridSpacing;
                GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToInt32(- vertex0.Z));
                GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToInt32(- vertex1.Z));
            }
            for ( X = StartXY.X; X <= FinishXY.X - 1; X++ )
            {
                vertex0.X = X * Constants.TerrainGridSpacing;
                vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[X, FinishXY.Y].Height * Map.HeightMultiplier);
                vertex0.Z = - FinishXY.Y * Constants.TerrainGridSpacing;
                vertex1.X = (X + 1) * Constants.TerrainGridSpacing;
                vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[X + 1, FinishXY.Y].Height * Map.HeightMultiplier);
                vertex1.Z = - FinishXY.Y * Constants.TerrainGridSpacing;
                GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToInt32(- vertex0.Z));
                GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToInt32(- vertex1.Z));
            }
            for ( Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++ )
            {
                vertex0.X = StartXY.X * Constants.TerrainGridSpacing;
                vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[StartXY.X, Y].Height * Map.HeightMultiplier);
                vertex0.Z = - Y * Constants.TerrainGridSpacing;
                vertex1.X = StartXY.X * Constants.TerrainGridSpacing;
                vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[StartXY.X, Y + 1].Height * Map.HeightMultiplier);
                vertex1.Z = - (Y + 1) * Constants.TerrainGridSpacing;
                GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToInt32(- vertex0.Z));
                GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToInt32(- vertex1.Z));
            }
            for ( Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++ )
            {
                vertex0.X = FinishXY.X * Constants.TerrainGridSpacing;
                vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[FinishXY.X, Y].Height * Map.HeightMultiplier);
                vertex0.Z = - Y * Constants.TerrainGridSpacing;
                vertex1.X = FinishXY.X * Constants.TerrainGridSpacing;
                vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[FinishXY.X, Y + 1].Height * Map.HeightMultiplier);
                vertex1.Z = - (Y + 1) * Constants.TerrainGridSpacing;
                GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToInt32(- vertex0.Z));
                GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToInt32(- vertex1.Z));
            }
            GL.End();
        }
    }
}