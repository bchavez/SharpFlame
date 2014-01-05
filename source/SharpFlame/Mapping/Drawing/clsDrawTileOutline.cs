using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawTileOutline : clsMap.clsAction
    {
        public sRGBA_sng Colour;

        private sXYZ_int Vertex0;
        private sXYZ_int Vertex1;
        private sXYZ_int Vertex2;
        private sXYZ_int Vertex3;

        public override void ActionPerform()
        {
            Vertex0.X = PosNum.X * App.TerrainGridSpacing;
            Vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y].Height * Map.HeightMultiplier);
            Vertex0.Z = - PosNum.Y * App.TerrainGridSpacing;
            Vertex1.X = (PosNum.X + 1) * App.TerrainGridSpacing;
            Vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height * Map.HeightMultiplier);
            Vertex1.Z = - PosNum.Y * App.TerrainGridSpacing;
            Vertex2.X = PosNum.X * App.TerrainGridSpacing;
            Vertex2.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height * Map.HeightMultiplier);
            Vertex2.Z = - (PosNum.Y + 1) * App.TerrainGridSpacing;
            Vertex3.X = (PosNum.X + 1) * App.TerrainGridSpacing;
            Vertex3.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height * Map.HeightMultiplier);
            Vertex3.Z = - (PosNum.Y + 1) * App.TerrainGridSpacing;
            GL.Begin(BeginMode.LineLoop);
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
            GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToInt32(- Vertex0.Z));
            GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToInt32(- Vertex1.Z));
            GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToInt32(- Vertex3.Z));
            GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToInt32(- Vertex2.Z));
            GL.End();
        }
    }
}