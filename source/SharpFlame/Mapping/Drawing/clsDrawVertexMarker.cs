using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawVertexMarker : clsAction
    {
        public sRGBA_sng Colour;

        private sXYZ_int Vertex0;

        public override void ActionPerform()
        {
            Vertex0.X = PosNum.X * App.TerrainGridSpacing;
            Vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y].Height * Map.HeightMultiplier);
            Vertex0.Z = - PosNum.Y * App.TerrainGridSpacing;
            GL.Begin(BeginMode.Lines);
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
            GL.Vertex3(Vertex0.X - 8, Vertex0.Y, Convert.ToInt32(- Vertex0.Z));
            GL.Vertex3(Vertex0.X + 8, Vertex0.Y, Convert.ToInt32(- Vertex0.Z));
            GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToInt32(- Vertex0.Z - 8));
            GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z + 8);
            GL.End();
        }
    } 
}