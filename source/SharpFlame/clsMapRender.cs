using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;

namespace SharpFlame
{
    public partial class clsMap
    {
        public abstract class clsDrawTile
        {
            public clsMap Map;
            public int TileX;
            public int TileY;

            public abstract void Perform();
        }

        public class clsDrawTileOld : clsDrawTile
        {
            public override void Perform()
            {
                clsTerrain Terrain = Map.Terrain;
                clsTileset Tileset = Map.Tileset;
                double[] TileTerrainHeight = new double[4];
                sXYZ_sng Vertex0 = new sXYZ_sng();
                sXYZ_sng Vertex1 = new sXYZ_sng();
                sXYZ_sng Vertex2 = new sXYZ_sng();
                sXYZ_sng Vertex3 = new sXYZ_sng();
                sXYZ_sng Normal0 = new sXYZ_sng();
                sXYZ_sng Normal1 = new sXYZ_sng();
                sXYZ_sng Normal2 = new sXYZ_sng();
                sXYZ_sng Normal3 = new sXYZ_sng();
                sXY_sng TexCoord0 = new sXY_sng();
                sXY_sng TexCoord1 = new sXY_sng();
                sXY_sng TexCoord2 = new sXY_sng();
                sXY_sng TexCoord3 = new sXY_sng();
                int A = 0;

                if ( Terrain.Tiles[TileX, TileY].Texture.TextureNum < 0 )
                {
                    GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_NoTile);
                }
                else if ( Tileset == null )
                {
                    GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_OverflowTile);
                }
                else if ( Terrain.Tiles[TileX, TileY].Texture.TextureNum < Tileset.TileCount )
                {
                    A = Tileset.Tiles[Terrain.Tiles[TileX, TileY].Texture.TextureNum].MapView_GL_Texture_Num;
                    if ( A == 0 )
                    {
                        GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_OverflowTile);
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, A);
                    }
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_OverflowTile);
                }
                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

                TileTerrainHeight[0] = Terrain.Vertices[TileX, TileY].Height;
                TileTerrainHeight[1] = Terrain.Vertices[TileX + 1, TileY].Height;
                TileTerrainHeight[2] = Terrain.Vertices[TileX, TileY + 1].Height;
                TileTerrainHeight[3] = Terrain.Vertices[TileX + 1, TileY + 1].Height;

                TileUtil.GetTileRotatedTexCoords(Terrain.Tiles[TileX, TileY].Texture.Orientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3);

                Vertex0.X = TileX * App.TerrainGridSpacing;
                Vertex0.Y = (float)(TileTerrainHeight[0] * Map.HeightMultiplier);
                Vertex0.Z = - TileY * App.TerrainGridSpacing;
                Vertex1.X = (TileX + 1) * App.TerrainGridSpacing;
                Vertex1.Y = (float)(TileTerrainHeight[1] * Map.HeightMultiplier);
                Vertex1.Z = - TileY * App.TerrainGridSpacing;
                Vertex2.X = TileX * App.TerrainGridSpacing;
                Vertex2.Y = (float)(TileTerrainHeight[2] * Map.HeightMultiplier);
                Vertex2.Z = - (TileY + 1) * App.TerrainGridSpacing;
                Vertex3.X = (TileX + 1) * App.TerrainGridSpacing;
                Vertex3.Y = (float)(TileTerrainHeight[3] * Map.HeightMultiplier);
                Vertex3.Z = - (TileY + 1) * App.TerrainGridSpacing;

                Normal0 = Map.TerrainVertexNormalCalc(TileX, TileY);
                Normal1 = Map.TerrainVertexNormalCalc(TileX + 1, TileY);
                Normal2 = Map.TerrainVertexNormalCalc(TileX, TileY + 1);
                Normal3 = Map.TerrainVertexNormalCalc(TileX + 1, TileY + 1);

                GL.Begin(BeginMode.Triangles);
                if ( Terrain.Tiles[TileX, TileY].Tri )
                {
                    GL.Normal3(Normal0.X, Normal0.Y, Convert.ToDouble(- Normal0.Z));
                    GL.TexCoord2(TexCoord0.X, TexCoord0.Y);
                    GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                    GL.Normal3(Normal2.X, Normal2.Y, Convert.ToDouble(- Normal2.Z));
                    GL.TexCoord2(TexCoord2.X, TexCoord2.Y);
                    GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                    GL.Normal3(Normal1.X, Normal1.Y, Convert.ToDouble(- Normal1.Z));
                    GL.TexCoord2(TexCoord1.X, TexCoord1.Y);
                    GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));

                    GL.Normal3(Normal1.X, Normal1.Y, Convert.ToDouble(- Normal1.Z));
                    GL.TexCoord2(TexCoord1.X, TexCoord1.Y);
                    GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                    GL.Normal3(Normal2.X, Normal2.Y, Convert.ToDouble(- Normal2.Z));
                    GL.TexCoord2(TexCoord2.X, TexCoord2.Y);
                    GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                    GL.Normal3(Normal3.X, Normal3.Y, Convert.ToDouble(- Normal3.Z));
                    GL.TexCoord2(TexCoord3.X, TexCoord3.Y);
                    GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                }
                else
                {
                    GL.Normal3(Normal0.X, Normal0.Y, Convert.ToDouble(- Normal0.Z));
                    GL.TexCoord2(TexCoord0.X, TexCoord0.Y);
                    GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                    GL.Normal3(Normal2.X, Normal2.Y, Convert.ToDouble(- Normal2.Z));
                    GL.TexCoord2(TexCoord2.X, TexCoord2.Y);
                    GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                    GL.Normal3(Normal3.X, Normal3.Y, Convert.ToDouble(- Normal3.Z));
                    GL.TexCoord2(TexCoord3.X, TexCoord3.Y);
                    GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));

                    GL.Normal3(Normal0.X, Normal0.Y, Convert.ToDouble(- Normal0.Z));
                    GL.TexCoord2(TexCoord0.X, TexCoord0.Y);
                    GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                    GL.Normal3(Normal3.X, Normal3.Y, Convert.ToDouble(- Normal3.Z));
                    GL.TexCoord2(TexCoord3.X, TexCoord3.Y);
                    GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                    GL.Normal3(Normal1.X, Normal1.Y, Convert.ToDouble(- Normal1.Z));
                    GL.TexCoord2(TexCoord1.X, TexCoord1.Y);
                    GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                }
                GL.End();
            }
        }

        //Public Class clsBufferData

        //    Public Structure sVertex
        //        Public Pos As sXYZ_sng
        //        Public Normal As sXYZ_sng
        //        Public TexCoord As sXY_sng
        //        Public RGBA As sRGBA_sng
        //        Private PaddingA As Integer
        //        Private PaddingB As Integer
        //        Private PaddingC As Integer
        //        Private PaddingD As Integer
        //    End Structure
        //    Public Vertices() As sVertex

        //    Public Position As Integer = 0

        //    Public Sub SendData(GLBufferNum As Integer)

        //        ReDim Preserve Vertices(Position - 1)

        //        GL.BindBuffer(BufferTarget.ArrayBuffer, GLBufferNum)
        //        GL.BufferData(Of sVertex)(BufferTarget.ArrayBuffer, CType(Position * 64, IntPtr), Vertices, BufferUsageHint.DynamicDraw)
        //        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)

        //        Vertices = Nothing
        //    End Sub
        //End Class
    }
}