#region

using SharpFlame.Mapping.Tiles;
using SharpFlame.Painters;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyVertexTerrainInterpret : clsAction
    {
        private int BestCount;
        private int BestNum;
        private TileDirection OppositeDirection;
        private Painter Painter;
        private Terrain PainterTerrainA;
        private Terrain PainterTerrainB;
        private TileOrientationChance PainterTexture;
        private TileDirection ResultDirection;
        private clsTerrain Terrain;
        private int[] TerrainCount;
        private clsTerrain.Tile.sTexture Texture;
        private clsTerrain.Tile Tile;
        private TileDirection VertexDirection;

        private void ToolPerformTile()
        {
            var PainterBrushNum = 0;
            var A = 0;

            for ( PainterBrushNum = 0; PainterBrushNum <= Painter.TerrainCount - 1; PainterBrushNum++ )
            {
                PainterTerrainA = Painter.Terrains[PainterBrushNum];
                for ( A = 0; A <= PainterTerrainA.Tiles.TileCount - 1; A++ )
                {
                    PainterTexture = PainterTerrainA.Tiles.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TerrainCount[PainterTerrainA.Num]++;
                    }
                }
            }
            for ( PainterBrushNum = 0; PainterBrushNum <= Painter.TransitionBrushCount - 1; PainterBrushNum++ )
            {
                PainterTerrainA = Painter.TransitionBrushes[PainterBrushNum].TerrainInner;
                PainterTerrainB = Painter.TransitionBrushes[PainterBrushNum].TerrainOuter;
                for ( A = 0; A <= Painter.TransitionBrushes[PainterBrushNum].TilesStraight.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.TransitionBrushes[PainterBrushNum].TilesStraight.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.DirectionsOnSameSide(VertexDirection, ResultDirection) )
                        {
                            TerrainCount[PainterTerrainB.Num]++;
                        }
                        else
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                }
                for ( A = 0; A <= Painter.TransitionBrushes[PainterBrushNum].TilesCornerIn.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.TransitionBrushes[PainterBrushNum].TilesCornerIn.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.IdenticalTileDirections(VertexDirection, ResultDirection) )
                        {
                            TerrainCount[PainterTerrainB.Num]++;
                        }
                        else
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                }
                for ( A = 0; A <= Painter.TransitionBrushes[PainterBrushNum].TilesCornerOut.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.TransitionBrushes[PainterBrushNum].TilesCornerOut.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        OppositeDirection = PainterTexture.Direction;
                        OppositeDirection.FlipX();
                        OppositeDirection.FlipY();
                        TileUtil.RotateDirection(OppositeDirection, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.IdenticalTileDirections(VertexDirection, ResultDirection) )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                        else
                        {
                            TerrainCount[PainterTerrainB.Num]++;
                        }
                    }
                }
            }

            for ( PainterBrushNum = 0; PainterBrushNum <= Painter.CliffBrushCount - 1; PainterBrushNum++ )
            {
                PainterTerrainA = Painter.CliffBrushes[PainterBrushNum].Terrain_Inner;
                PainterTerrainB = Painter.CliffBrushes[PainterBrushNum].Terrain_Outer;
                for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Straight.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Straight.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.DirectionsOnSameSide(VertexDirection, ResultDirection) )
                        {
                            TerrainCount[PainterTerrainB.Num]++;
                        }
                        else
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                }
                for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_In.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_In.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.IdenticalTileDirections(VertexDirection, ResultDirection) )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                        else
                        {
                            TerrainCount[PainterTerrainB.Num]++;
                        }
                    }
                }
                for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_Out.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_Out.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        OppositeDirection = PainterTexture.Direction;
                        OppositeDirection.FlipX();
                        OppositeDirection.FlipY();
                        TileUtil.RotateDirection(OppositeDirection, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.IdenticalTileDirections(VertexDirection, ResultDirection) )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                        else
                        {
                            TerrainCount[PainterTerrainB.Num]++;
                        }
                    }
                }
            }

            for ( PainterBrushNum = 0; PainterBrushNum <= Painter.RoadBrushCount - 1; PainterBrushNum++ )
            {
                PainterTerrainA = Painter.RoadBrushes[PainterBrushNum].Terrain;
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_Corner_In.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_Corner_In.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TerrainCount[PainterTerrainA.Num]++;
                    }
                }
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_CrossIntersection.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_CrossIntersection.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TerrainCount[PainterTerrainA.Num]++;
                    }
                }
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_End.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_End.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TerrainCount[PainterTerrainA.Num]++;
                    }
                }
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_Straight.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_Straight.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TerrainCount[PainterTerrainA.Num]++;
                    }
                }
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_TIntersection.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_TIntersection.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TerrainCount[PainterTerrainA.Num]++;
                    }
                }
            }
        }

        public override void ActionPerform()
        {
            var A = 0;

            Terrain = Map.Terrain;

            Painter = Map.Painter;
            TerrainCount = new int[Painter.TerrainCount];

            if ( PosNum.Y > 0 )
            {
                if ( PosNum.X > 0 )
                {
                    VertexDirection = TileUtil.BottomRight;
                    Tile = Terrain.Tiles[PosNum.X - 1, PosNum.Y - 1];
                    Texture = Tile.Texture;
                    ToolPerformTile();
                }
                if ( PosNum.X < Terrain.TileSize.X )
                {
                    VertexDirection = TileUtil.BottomLeft;
                    Tile = Terrain.Tiles[PosNum.X, PosNum.Y - 1];
                    Texture = Tile.Texture;
                    ToolPerformTile();
                }
            }
            if ( PosNum.Y < Terrain.TileSize.Y )
            {
                if ( PosNum.X > 0 )
                {
                    VertexDirection = TileUtil.TopRight;
                    Tile = Terrain.Tiles[PosNum.X - 1, PosNum.Y];
                    Texture = Tile.Texture;
                    ToolPerformTile();
                }
                if ( PosNum.X < Terrain.TileSize.X )
                {
                    VertexDirection = TileUtil.TopLeft;
                    Tile = Terrain.Tiles[PosNum.X, PosNum.Y];
                    Texture = Tile.Texture;
                    ToolPerformTile();
                }
            }

            BestNum = -1;
            BestCount = 0;
            for ( A = 0; A <= Painter.TerrainCount - 1; A++ )
            {
                if ( TerrainCount[A] > BestCount )
                {
                    BestNum = A;
                    BestCount = TerrainCount[A];
                }
            }
            if ( BestCount > 0 )
            {
                Terrain.Vertices[PosNum.X, PosNum.Y].Terrain = Painter.Terrains[BestNum];
            }
            else
            {
                Terrain.Vertices[PosNum.X, PosNum.Y].Terrain = null;
            }

            Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
        }
    }
}