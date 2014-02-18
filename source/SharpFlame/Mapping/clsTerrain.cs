using System.Diagnostics;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;
using SharpFlame.Painters;

namespace SharpFlame.Mapping
{
    public class clsTerrain
    {
        public struct Vertex
        {
            public byte Height;
            public Painters.Terrain Terrain;
        }

        public struct Tile
        {
            public struct sTexture
            {
                public int TextureNum;
                public TileOrientation Orientation;
            }

            public sTexture Texture;
            public bool Tri;
            public bool TriTopLeftIsCliff;
            public bool TriTopRightIsCliff;
            public bool TriBottomLeftIsCliff;
            public bool TriBottomRightIsCliff;
            public bool Terrain_IsCliff;
            public TileDirection DownSide;

            public void Copy(Tile TileToCopy)
            {
                Texture = TileToCopy.Texture;
                Tri = TileToCopy.Tri;
                TriTopLeftIsCliff = TileToCopy.TriTopLeftIsCliff;
                TriTopRightIsCliff = TileToCopy.TriTopRightIsCliff;
                TriBottomLeftIsCliff = TileToCopy.TriBottomLeftIsCliff;
                TriBottomRightIsCliff = TileToCopy.TriBottomRightIsCliff;
                Terrain_IsCliff = TileToCopy.Terrain_IsCliff;
                DownSide = TileToCopy.DownSide;
            }

            public void TriCliffAddDirection(TileDirection Direction)
            {
                if ( Direction.X == 0 )
                {
                    if ( Direction.Y == 0 )
                    {
                        TriTopLeftIsCliff = true;
                    }
                    else if ( Direction.Y == 2 )
                    {
                        TriBottomLeftIsCliff = true;
                    }
                    else
                    {
                        Debugger.Break();
                    }
                }
                else if ( Direction.X == 2 )
                {
                    if ( Direction.Y == 0 )
                    {
                        TriTopRightIsCliff = true;
                    }
                    else if ( Direction.Y == 2 )
                    {
                        TriBottomRightIsCliff = true;
                    }
                    else
                    {
                        Debugger.Break();
                    }
                }
                else
                {
                    Debugger.Break();
                }
            }
        }

        public struct Side
        {
            public Road Road;
        }

        public XYInt TileSize;

        public Vertex[,] Vertices;
        public Tile[,] Tiles;
        public Side[,] SideH;
        public Side[,] SideV;

        public clsTerrain(XYInt newSize)
        {
            TileSize = newSize;

            Vertices = new Vertex[TileSize.X + 1, TileSize.Y + 1];
            Tiles = new Tile[TileSize.X, TileSize.Y];
            SideH = new Side[TileSize.X, TileSize.Y + 1];
            SideV = new Side[TileSize.X + 1, TileSize.Y];
            int X = 0;
            int Y = 0;

            for ( Y = 0; Y <= TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= TileSize.X - 1; X++ )
                {
                    Tiles[X, Y].Texture.TextureNum = -1;
                    Tiles[X, Y].DownSide = TileUtil.None;
                }
            }
        }
    }
}