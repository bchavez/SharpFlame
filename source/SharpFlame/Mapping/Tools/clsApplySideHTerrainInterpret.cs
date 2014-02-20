#region

using SharpFlame.Mapping.Tiles;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsApplySideHTerrainInterpret : clsApplySideTerrainInterpret
    {
        public override void ActionPerform()
        {
            base.ActionPerform();

            var A = 0;

            if ( PosNum.Y > 0 )
            {
                SideDirection = TileUtil.Bottom;
                Tile = Terrain.Tiles[PosNum.X, PosNum.Y - 1];
                Texture = Tile.Texture;
                ToolPerformTile();
            }
            if ( PosNum.Y < Terrain.TileSize.Y )
            {
                SideDirection = TileUtil.Top;
                Tile = Terrain.Tiles[PosNum.X, PosNum.Y];
                Texture = Tile.Texture;
                ToolPerformTile();
            }

            BestNum = -1;
            BestCount = 0;
            for ( A = 0; A <= Painter.RoadCount - 1; A++ )
            {
                if ( RoadCount[A] > BestCount )
                {
                    BestNum = A;
                    BestCount = RoadCount[A];
                }
            }
            if ( BestCount > 0 )
            {
                Terrain.SideH[PosNum.X, PosNum.Y].Road = Painter.Roads[BestNum];
            }
            else
            {
                Terrain.SideH[PosNum.X, PosNum.Y].Road = null;
            }

            Map.SectorTerrainUndoChanges.SideHChanged(PosNum);
        }
    }
}