#region

using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Old.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;

#endregion

namespace SharpFlame.Old.Mapping.Drawing
{
    public class clsDrawHorizontalPosOnTerrain
    {
        //does not inherit action

        private readonly XYZInt vertex0;
        public SRgba Colour;
        public XYInt Horizontal;
        public Map Map;

        public clsDrawHorizontalPosOnTerrain()
        {
            vertex0 = new XYZInt(0, 0, 0);
        }

        public void ActionPerform()
        {
            vertex0.X = Horizontal.X;
            vertex0.Y = (int)(Map.GetTerrainHeight(Horizontal));
            vertex0.Z = Convert.ToInt32(- Horizontal.Y);
            GL.Begin(BeginMode.Lines);
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
            GL.Vertex3(vertex0.X - 8, vertex0.Y, Convert.ToInt32(- vertex0.Z));
            GL.Vertex3(vertex0.X + 8, vertex0.Y, Convert.ToInt32(- vertex0.Z));
            GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToInt32(- vertex0.Z - 8));
            GL.Vertex3(vertex0.X, vertex0.Y, - vertex0.Z + 8);
            GL.End();
        }
    }
}