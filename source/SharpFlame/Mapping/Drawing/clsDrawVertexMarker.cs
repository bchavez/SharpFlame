#region

using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Tools;

#endregion

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawVertexMarker : clsAction
    {
        private readonly XYZInt vertex0;
        public sRGBA_sng Colour;

        public clsDrawVertexMarker()
        {
            vertex0 = new XYZInt(0, 0, 0);
        }

        public override void ActionPerform()
        {
            vertex0.X = PosNum.X * Constants.TerrainGridSpacing;
            vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y].Height * Map.HeightMultiplier);
            vertex0.Z = - PosNum.Y * Constants.TerrainGridSpacing;
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