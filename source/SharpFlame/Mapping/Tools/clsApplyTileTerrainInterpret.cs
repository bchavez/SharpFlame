using SharpFlame.Mapping.Tiles;
using SharpFlame.Painters;

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyTileTerrainInterpret : clsAction
    {
        private Painter Painter;
        private Painters.Terrain PainterTerrainA;
        private Painters.Terrain PainterTerrainB;
        private clsTerrain.Tile.sTexture Texture;
        private TileDirection ResultDirection;
        private TileOrientationChance PainterTexture;
        private TileDirection OppositeDirection;
        private clsTerrain.Tile Tile;
        private clsTerrain Terrain;

        public override void ActionPerform()
        {
            int PainterBrushNum = 0;
            int A = 0;

            Terrain = Map.Terrain;

            Painter = Map.Painter;

            Tile = Terrain.Tiles[PosNum.X, PosNum.Y];
            Texture = Tile.Texture;

            Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
            Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;
            Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
            Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
            Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.None;

            for ( PainterBrushNum = 0; PainterBrushNum <= Painter.CliffBrushCount - 1; PainterBrushNum++ )
            {
                PainterTerrainA = Painter.CliffBrushes[PainterBrushNum].Terrain_Inner;
                PainterTerrainB = Painter.CliffBrushes[PainterBrushNum].Terrain_Outer;
                for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Straight.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Straight.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        if ( Tile.Tri )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                        }
                        else
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                        }
                        Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = ResultDirection;
                    }
                }
                for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_In.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_In.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( Tile.Tri )
                        {
                            if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.TopLeft) )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.BottomRight) )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                        else
                        {
                            if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.TopRight) )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.BottomLeft) )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
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
                        if ( Tile.Tri )
                        {
                            if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.TopLeft) )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.BottomRight) )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                        else
                        {
                            if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.TopRight) )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.BottomLeft) )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                    }
                }
            }

            Map.SectorTerrainUndoChanges.TileChanged(PosNum);
        }
    }
}