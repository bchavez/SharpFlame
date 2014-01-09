using SharpFlame.Mapping.Tiles;
using SharpFlame.Painters;

namespace SharpFlame.Mapping.Tools
{
    public abstract class clsApplySideTerrainInterpret : clsAction
    {
        protected Painter Painter;
        protected Painters.Terrain PainterTerrain;
        protected Road PainterRoad;
        protected clsTerrain.Tile.sTexture Texture;
        protected TileDirection ResultDirection;
        protected TileOrientationChance PainterTexture;
        protected TileDirection OppositeDirection;
        protected clsTerrain.Tile Tile;
        protected int[] RoadCount;
        protected TileDirection SideDirection;
        protected int BestNum;
        protected int BestCount;
        protected clsTerrain Terrain;

        protected void ToolPerformTile()
        {
            int PainterBrushNum = 0;
            int A = 0;

            for ( PainterBrushNum = 0; PainterBrushNum <= Painter.RoadBrushCount - 1; PainterBrushNum++ )
            {
                PainterRoad = Painter.RoadBrushes[PainterBrushNum].Road;
                PainterTerrain = Painter.RoadBrushes[PainterBrushNum].Terrain;
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_Corner_In.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_Corner_In.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.DirectionsOnSameSide(SideDirection, ResultDirection) )
                        {
                            RoadCount[PainterRoad.Num]++;
                        }
                    }
                }
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_CrossIntersection.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_CrossIntersection.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        RoadCount[PainterRoad.Num]++;
                    }
                }
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_End.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_End.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.IdenticalTileDirections(SideDirection, ResultDirection) )
                        {
                            RoadCount[PainterRoad.Num]++;
                        }
                    }
                }
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_Straight.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_Straight.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( TileUtil.DirectionsAreInLine(SideDirection, ResultDirection) )
                        {
                            RoadCount[PainterRoad.Num]++;
                        }
                    }
                }
                for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_TIntersection.TileCount - 1; A++ )
                {
                    PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_TIntersection.Tiles[A];
                    if ( PainterTexture.TextureNum == Texture.TextureNum )
                    {
                        TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                        if ( !TileUtil.DirectionsOnSameSide(SideDirection, ResultDirection) )
                        {
                            RoadCount[PainterRoad.Num]++;
                        }
                    }
                }
            }
        }

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            Painter = Map.Painter;
            RoadCount = new int[Painter.RoadCount];
        }
    }
}