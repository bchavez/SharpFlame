using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;
using Matrix3D;

namespace SharpFlame.Mapping.Renderers
{
    public class clsDrawTileMiddleVertex : clsDrawTile
    {
        public override void Perform()
        {
            clsTerrain Terrain = Map.Terrain;
            clsTileset Tileset = Map.Tileset;
            double[] TileTerrainHeight = new double[5];
            Position.XYZ_dbl[] Vertices = new Position.XYZ_dbl[5]; //4 corners + center
            Position.XYZ_dbl[] Normals = new Position.XYZ_dbl[5];
            Position.XY_dbl[] TexCoords = new Position.XY_dbl[5];
            int A = 0;

            //Texture binding code copied from clsDrawTileOld
            if (Terrain.Tiles[TileX, TileY].Texture.TextureNum < 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_NoTile);
            }
            else if (Tileset == null)
            {
                GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_OverflowTile);
            }
            else if (Terrain.Tiles[TileX, TileY].Texture.TextureNum < Tileset.TileCount)
            {
                A = Tileset.Tiles[Terrain.Tiles[TileX, TileY].Texture.TextureNum].MapViewGlTextureNum;
                if (A == 0)
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

            //Vertex order:
            //0   1
            //  4
            //2   3
            TileTerrainHeight[0] = Terrain.Vertices[TileX, TileY].Height;
            TileTerrainHeight[1] = Terrain.Vertices[TileX + 1, TileY].Height;
            TileTerrainHeight[2] = Terrain.Vertices[TileX, TileY + 1].Height;
            TileTerrainHeight[3] = Terrain.Vertices[TileX + 1, TileY + 1].Height;
            TileTerrainHeight[4] = (TileTerrainHeight[0] + TileTerrainHeight[1] + TileTerrainHeight[2] + TileTerrainHeight[3]) / 4; //middle height is average of the corners

            TileUtil.GetTileRotatedTexCoords(Terrain.Tiles[TileX, TileY].Texture.Orientation, ref TexCoords[0], ref TexCoords[1], ref TexCoords[2], ref TexCoords[3]);

            //cowboy: don't forget the middle texture coordinate regardless of rotation.
            TexCoords[4].X = 0.5f;
            TexCoords[4].Y = 0.5f;

            Vertices[0].X = TileX * App.TerrainGridSpacing;
            Vertices[0].Y = (float)(TileTerrainHeight[0] * Map.HeightMultiplier);
            Vertices[0].Z = -TileY * App.TerrainGridSpacing;

            Vertices[1].X = (TileX + 1) * App.TerrainGridSpacing;
            Vertices[1].Y = (float)(TileTerrainHeight[1] * Map.HeightMultiplier);
            Vertices[1].Z = -TileY * App.TerrainGridSpacing;

            Vertices[2].X = TileX * App.TerrainGridSpacing;
            Vertices[2].Y = (float)(TileTerrainHeight[2] * Map.HeightMultiplier);
            Vertices[2].Z = -(TileY + 1) * App.TerrainGridSpacing;

            Vertices[3].X = (TileX + 1) * App.TerrainGridSpacing;
            Vertices[3].Y = (float)(TileTerrainHeight[3] * Map.HeightMultiplier);
            Vertices[3].Z = -(TileY + 1) * App.TerrainGridSpacing;

            Vertices[4].X = ( TileX + 0.5f ) * App.TerrainGridSpacing;
            Vertices[4].Y = (float)( TileTerrainHeight[4] * Map.HeightMultiplier );
            Vertices[4].Z = -( TileY + 0.5f ) * App.TerrainGridSpacing;

            Normals[0] = Map.TerrainVertexNormalCalc(TileX, TileY);
            Normals[1] = Map.TerrainVertexNormalCalc(TileX + 1, TileY);
            Normals[2] = Map.TerrainVertexNormalCalc(TileX, TileY + 1);
            Normals[3] = Map.TerrainVertexNormalCalc(TileX + 1, TileY + 1);

            Normals[4] = (Normals[0] + Normals[1] + Normals[2] + Normals[3])/4; //Linearly interpolate from corner vertices
            Normals[4] /= Normals[4].GetMagnitude(); //normalize vector length

            GL.Begin(BeginMode.Triangles);
            int[] indices = { 1, 0, 4, 3, 1, 4, 2, 3, 4, 0, 2, 4 };
            for( int i = 0; i < indices.Length; i++ )
            {
                GL.Normal3(Normals[indices[i]].X, Normals[indices[i]].Y, -Normals[indices[i]].Z);
                GL.TexCoord2(TexCoords[indices[i]].X, TexCoords[indices[i]].Y);
                GL.Vertex3(Vertices[indices[i]].X, Vertices[indices[i]].Y, -Vertices[indices[i]].Z);
            }
            GL.End();
        }
    }
}