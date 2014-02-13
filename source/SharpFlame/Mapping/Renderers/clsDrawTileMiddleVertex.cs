using OpenTK.Graphics.OpenGL;
using SharpFlame.Mapping.Tiles;
using Matrix3D;

namespace SharpFlame.Mapping.Renderers
{
    public class clsDrawTileMiddleVertex : clsDrawTile
    {
        public override void Perform()
        {
            var terrain = Map.Terrain;
            var tileset = Map.Tileset;
            var tileTerrainHeight = new double[5];
            var vertices = new Position.XYZ_dbl[5]; //4 corners + center
            var normals = new Position.XYZ_dbl[5];
            var texCoords = new Position.XY_dbl[5];

            //Texture binding code copied from clsDrawTileOld
            if (terrain.Tiles[TileX, TileY].Texture.TextureNum < 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_NoTile);
            }
            else if (tileset == null)
            {
                GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_OverflowTile);
            }
            else if (terrain.Tiles[TileX, TileY].Texture.TextureNum < tileset.TileCount)
            {
                var viewGlTextureNum = tileset.Tiles[terrain.Tiles[TileX, TileY].Texture.TextureNum].MapViewGlTextureNum;
                if (viewGlTextureNum == 0)
                {
                    GL.BindTexture(TextureTarget.Texture2D, App.GLTexture_OverflowTile);
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, viewGlTextureNum);
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
            tileTerrainHeight[0] = terrain.Vertices[TileX, TileY].Height;
            tileTerrainHeight[1] = terrain.Vertices[TileX + 1, TileY].Height;
            tileTerrainHeight[2] = terrain.Vertices[TileX, TileY + 1].Height;
            tileTerrainHeight[3] = terrain.Vertices[TileX + 1, TileY + 1].Height;
            tileTerrainHeight[4] = (tileTerrainHeight[0] + tileTerrainHeight[1] + tileTerrainHeight[2] + tileTerrainHeight[3]) / 4; //middle height is average of the corners

            TileUtil.GetTileRotatedTexCoords(terrain.Tiles[TileX, TileY].Texture.Orientation, ref texCoords[0], ref texCoords[1], ref texCoords[2], ref texCoords[3]);

            //cowboy: don't forget the middle texture coordinate regardless of rotation.
            texCoords[4].X = 0.5f;
            texCoords[4].Y = 0.5f;

            vertices[0].X = TileX * App.TerrainGridSpacing;
            vertices[0].Y = (float)(tileTerrainHeight[0] * Map.HeightMultiplier);
            vertices[0].Z = -TileY * App.TerrainGridSpacing;

            vertices[1].X = (TileX + 1) * App.TerrainGridSpacing;
            vertices[1].Y = (float)(tileTerrainHeight[1] * Map.HeightMultiplier);
            vertices[1].Z = -TileY * App.TerrainGridSpacing;

            vertices[2].X = TileX * App.TerrainGridSpacing;
            vertices[2].Y = (float)(tileTerrainHeight[2] * Map.HeightMultiplier);
            vertices[2].Z = -(TileY + 1) * App.TerrainGridSpacing;

            vertices[3].X = (TileX + 1) * App.TerrainGridSpacing;
            vertices[3].Y = (float)(tileTerrainHeight[3] * Map.HeightMultiplier);
            vertices[3].Z = -(TileY + 1) * App.TerrainGridSpacing;

            vertices[4].X = ( TileX + 0.5f ) * App.TerrainGridSpacing;
            vertices[4].Y = (float)( tileTerrainHeight[4] * Map.HeightMultiplier );
            vertices[4].Z = -( TileY + 0.5f ) * App.TerrainGridSpacing;

            normals[0] = Map.TerrainVertexNormalCalc(TileX, TileY);
            normals[1] = Map.TerrainVertexNormalCalc(TileX + 1, TileY);
            normals[2] = Map.TerrainVertexNormalCalc(TileX, TileY + 1);
            normals[3] = Map.TerrainVertexNormalCalc(TileX + 1, TileY + 1);

            normals[4] = (normals[0] + normals[1] + normals[2] + normals[3])/4; //Linearly interpolate from corner vertices
            normals[4] /= normals[4].GetMagnitude(); //normalize vector length

            GL.Begin(BeginMode.Triangles);
            int[] indices = { 1, 0, 4, 3, 1, 4, 2, 3, 4, 0, 2, 4 };
            foreach ( int i in indices )
            {
                GL.Normal3(normals[i].X, normals[i].Y, -normals[i].Z);
                GL.TexCoord2(texCoords[i].X, texCoords[i].Y);
                GL.Vertex3(vertices[i].X, vertices[i].Y, -vertices[i].Z);
            }
            GL.End();
        }
    }
}