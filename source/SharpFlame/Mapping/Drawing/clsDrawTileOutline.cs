using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawTileOutline : clsAction
    {
        public sRGBA_sng Colour;

        private XYZInt vertex0;
        private XYZInt vertex1;
        private XYZInt vertex2;
        private XYZInt vertex3;

		public clsDrawTileOutline() {
			vertex0 = new XYZInt (0, 0, 0);
			vertex1 = new XYZInt (0, 0, 0);
			vertex2 = new XYZInt (0, 0, 0);
			vertex3 = new XYZInt (0, 0, 0);
		}

        public override void ActionPerform()
        {
            vertex0.X = PosNum.X * Constants.TerrainGridSpacing;
            vertex0.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y].Height * Map.HeightMultiplier);
            vertex0.Z = - PosNum.Y * Constants.TerrainGridSpacing;
            vertex1.X = (PosNum.X + 1) * Constants.TerrainGridSpacing;
            vertex1.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height * Map.HeightMultiplier);
            vertex1.Z = - PosNum.Y * Constants.TerrainGridSpacing;
            vertex2.X = PosNum.X * Constants.TerrainGridSpacing;
            vertex2.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height * Map.HeightMultiplier);
            vertex2.Z = - (PosNum.Y + 1) * Constants.TerrainGridSpacing;
            vertex3.X = (PosNum.X + 1) * Constants.TerrainGridSpacing;
            vertex3.Y = Convert.ToInt32(Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height * Map.HeightMultiplier);
            vertex3.Z = - (PosNum.Y + 1) * Constants.TerrainGridSpacing;
            GL.Begin(BeginMode.LineLoop);
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
            GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToInt32(- vertex0.Z));
            GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToInt32(- vertex1.Z));
            GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToInt32(- vertex3.Z));
            GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToInt32(- vertex2.Z));
            GL.End();
        }
    }
}