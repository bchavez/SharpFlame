using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawHorizontalPosOnTerrain
    {
        //does not inherit action

        public clsMap Map;

        public sXY_int Horizontal;
        public sRGBA_sng Colour;

        private sXYZ_int Vertex0;

        public void ActionPerform()
        {
            Vertex0.X = Horizontal.X;
            Vertex0.Y = (int)(Map.GetTerrainHeight(Horizontal));
            Vertex0.Z = Convert.ToInt32(- Horizontal.Y);
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