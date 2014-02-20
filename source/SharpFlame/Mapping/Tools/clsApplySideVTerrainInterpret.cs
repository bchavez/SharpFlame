#region

using SharpFlame.Mapping.Tiles;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsApplySideVTerrainInterpret : clsApplySideTerrainInterpret
    {
        public override void ActionPerform()
        {
            base.ActionPerform();

            var A = 0;

            if ( PosNum.X > 0 )
            {
                SideDirection = TileUtil.Right;
                Tile = Terrain.Tiles[PosNum.X - 1, PosNum.Y];
                Texture = Tile.Texture;
                ToolPerformTile();
            }
            if ( PosNum.X < Terrain.TileSize.X )
            {
                SideDirection = TileUtil.Left;
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
                Terrain.SideV[PosNum.X, PosNum.Y].Road = Painter.Roads[BestNum];
            }
            else
            {
                Terrain.SideV[PosNum.X, PosNum.Y].Road = null;
            }

            Map.SectorTerrainUndoChanges.SideVChanged(PosNum);
        }
    }
}