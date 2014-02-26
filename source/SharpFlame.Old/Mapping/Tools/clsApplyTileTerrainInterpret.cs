#region

using SharpFlame.Old.Mapping.Tiles;
using SharpFlame.Old.Painters;

#endregion

namespace SharpFlame.Old.Mapping.Tools
{
    public class clsApplyTileTerrainInterpret : clsAction
    {
        private TileDirection oppositeDirection;
        private Painter painter;
        private TileOrientationChance painterTexture;
        private TileDirection resultDirection;
        private clsTerrain terrain;
        private clsTerrain.Tile.sTexture texture;
        private clsTerrain.Tile tile;

        public override void ActionPerform()
        {
            var painterBrushNum = 0;

            terrain = Map.Terrain;

            painter = Map.Painter;

            tile = terrain.Tiles[PosNum.X, PosNum.Y];
            texture = tile.Texture;

            terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
            terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;
            terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
            terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
            terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.None;

            for ( painterBrushNum = 0; painterBrushNum <= painter.CliffBrushCount - 1; painterBrushNum++ )
            {
                var a = 0;
                for ( a = 0; a <= painter.CliffBrushes[painterBrushNum].Tiles_Straight.TileCount - 1; a++ )
                {
                    painterTexture = painter.CliffBrushes[painterBrushNum].Tiles_Straight.Tiles[a];
                    if ( painterTexture.TextureNum == texture.TextureNum )
                    {
                        if ( tile.Tri )
                        {
                            terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                            terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                        }
                        else
                        {
                            terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                            terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                        }
                        terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                        TileUtil.RotateDirection(painterTexture.Direction, texture.Orientation, ref resultDirection);
                        terrain.Tiles[PosNum.X, PosNum.Y].DownSide = resultDirection;
                    }
                }
                for ( a = 0; a <= painter.CliffBrushes[painterBrushNum].Tiles_Corner_In.TileCount - 1; a++ )
                {
                    painterTexture = painter.CliffBrushes[painterBrushNum].Tiles_Corner_In.Tiles[a];
                    if ( painterTexture.TextureNum == texture.TextureNum )
                    {
                        TileUtil.RotateDirection(painterTexture.Direction, texture.Orientation, ref resultDirection);
                        if ( tile.Tri )
                        {
                            if ( TileUtil.IdenticalTileDirections(resultDirection, TileUtil.TopLeft) )
                            {
                                terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                                terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if ( TileUtil.IdenticalTileDirections(resultDirection, TileUtil.BottomRight) )
                            {
                                terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                                terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                        else
                        {
                            if ( TileUtil.IdenticalTileDirections(resultDirection, TileUtil.TopRight) )
                            {
                                terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                                terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if ( TileUtil.IdenticalTileDirections(resultDirection, TileUtil.BottomLeft) )
                            {
                                terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                                terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                    }
                }
                for ( a = 0; a <= painter.CliffBrushes[painterBrushNum].Tiles_Corner_Out.TileCount - 1; a++ )
                {
                    painterTexture = painter.CliffBrushes[painterBrushNum].Tiles_Corner_Out.Tiles[a];
                    if ( painterTexture.TextureNum == texture.TextureNum )
                    {
                        oppositeDirection = painterTexture.Direction;
                        oppositeDirection.FlipX();
                        oppositeDirection.FlipY();
                        TileUtil.RotateDirection(oppositeDirection, texture.Orientation, ref resultDirection);
                        if ( tile.Tri )
                        {
                            if ( TileUtil.IdenticalTileDirections(resultDirection, TileUtil.TopLeft) )
                            {
                                terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                                terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if ( TileUtil.IdenticalTileDirections(resultDirection, TileUtil.BottomRight) )
                            {
                                terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                                terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                        else
                        {
                            if ( TileUtil.IdenticalTileDirections(resultDirection, TileUtil.TopRight) )
                            {
                                terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                                terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if ( TileUtil.IdenticalTileDirections(resultDirection, TileUtil.BottomLeft) )
                            {
                                terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                                terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                    }
                }
            }

            Map.SectorTerrainUndoChanges.TileChanged(PosNum);
        }
    }
}