#region

using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawVertexTerrain : clsAction
    {
        private SRgb _rgb;
        private SRgb RGB_sng2;
        private XYZDouble Vertex0;
        private XYZDouble Vertex1;
        private XYZDouble Vertex2;
        private XYZDouble Vertex3;
        public Matrix3DMath.Matrix3D ViewAngleMatrix;
        private XYZDouble XYZ_dbl;
        private XYZDouble XYZ_dbl2;
        private XYZDouble XYZ_dbl3;

        public override void ActionPerform()
        {
            var X = 0;
            var Y = 0;
            var A = 0;

            for ( Y = PosNum.Y * Constants.SectorTileSize;
                Y <= Math.Min(Convert.ToInt32((PosNum.Y + 1) * Constants.SectorTileSize - 1), Map.Terrain.TileSize.Y);
                Y++ )
            {
                for ( X = PosNum.X * Constants.SectorTileSize; X <= Math.Min((PosNum.X + 1) * Constants.SectorTileSize - 1, Map.Terrain.TileSize.X); X++ )
                {
                    if ( Map.Terrain.Vertices[X, Y].Terrain != null )
                    {
                        A = Map.Terrain.Vertices[X, Y].Terrain.Num;
                        if ( A < Map.Painter.TerrainCount )
                        {
                            if ( Map.Painter.Terrains[A].Tiles.TileCount >= 1 )
                            {
                                _rgb = Map.Tileset.Tiles[Map.Painter.Terrains[A].Tiles.Tiles[0].TextureNum].AverageColour;
                                if ( _rgb.Red + _rgb.Green + _rgb.Blue < 1.5F )
                                {
                                    RGB_sng2.Red = (_rgb.Red + 1.0F) / 2.0F;
                                    RGB_sng2.Green = (_rgb.Green + 1.0F) / 2.0F;
                                    RGB_sng2.Blue = (_rgb.Blue + 1.0F) / 2.0F;
                                }
                                else
                                {
                                    RGB_sng2.Red = _rgb.Red / 2.0F;
                                    RGB_sng2.Green = _rgb.Green / 2.0F;
                                    RGB_sng2.Blue = _rgb.Blue / 2.0F;
                                }
                                XYZ_dbl.X = X * Constants.TerrainGridSpacing;
                                XYZ_dbl.Y = Convert.ToDouble(Map.Terrain.Vertices[X, Y].Height * Map.HeightMultiplier);
                                XYZ_dbl.Z = - Y * Constants.TerrainGridSpacing;
                                XYZ_dbl2.X = 10.0D;
                                XYZ_dbl2.Y = 10.0D;
                                XYZ_dbl2.Z = 0.0D;
                                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, ref XYZ_dbl3);
                                Vertex0.X = XYZ_dbl.X + XYZ_dbl3.X;
                                Vertex0.Y = XYZ_dbl.Y + XYZ_dbl3.Y;
                                Vertex0.Z = XYZ_dbl.Z + XYZ_dbl3.Z;
                                XYZ_dbl2.X = -10.0D;
                                XYZ_dbl2.Y = 10.0D;
                                XYZ_dbl2.Z = 0.0D;
                                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, ref XYZ_dbl3);
                                Vertex1.X = XYZ_dbl.X + XYZ_dbl3.X;
                                Vertex1.Y = XYZ_dbl.Y + XYZ_dbl3.Y;
                                Vertex1.Z = XYZ_dbl.Z + XYZ_dbl3.Z;
                                XYZ_dbl2.X = -10.0D;
                                XYZ_dbl2.Y = -10.0D;
                                XYZ_dbl2.Z = 0.0D;
                                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, ref XYZ_dbl3);
                                Vertex2.X = XYZ_dbl.X + XYZ_dbl3.X;
                                Vertex2.Y = XYZ_dbl.Y + XYZ_dbl3.Y;
                                Vertex2.Z = XYZ_dbl.Z + XYZ_dbl3.Z;
                                XYZ_dbl2.X = 10.0D;
                                XYZ_dbl2.Y = -10.0D;
                                XYZ_dbl2.Z = 0.0D;
                                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, ref XYZ_dbl3);
                                Vertex3.X = XYZ_dbl.X + XYZ_dbl3.X;
                                Vertex3.Y = XYZ_dbl.Y + XYZ_dbl3.Y;
                                Vertex3.Z = XYZ_dbl.Z + XYZ_dbl3.Z;
                                GL.Begin(BeginMode.Quads);
                                GL.Color3(_rgb.Red, _rgb.Green, _rgb.Blue);
                                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                                GL.End();
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3(RGB_sng2.Red, RGB_sng2.Green, RGB_sng2.Blue);
                                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                                GL.End();
                            }
                        }
                    }
                }
            }
        }
    }
}