

using SharpFlame.Core.Domain;


namespace SharpFlame.Mapping.Tools
{
    public class clsApplyRoadRemove : clsAction
    {
        private clsTerrain Terrain;

        private void ToolPerformSideH(XYInt SideNum)
        {
            Terrain = Map.Terrain;

            if ( Terrain.SideH[SideNum.X, SideNum.Y].Road != null )
            {
                Terrain.SideH[SideNum.X, SideNum.Y].Road = null;
                Map.AutoTextureChanges.SideHChanged(SideNum);
                Map.SectorGraphicsChanges.SideHChanged(SideNum);
                Map.SectorTerrainUndoChanges.SideHChanged(SideNum);
            }
        }

        private void ToolPerformSideV(XYInt SideNum)
        {
            Terrain = Map.Terrain;

            if ( Terrain.SideV[SideNum.X, SideNum.Y].Road != null )
            {
                Terrain.SideV[SideNum.X, SideNum.Y].Road = null;
                Map.AutoTextureChanges.SideVChanged(SideNum);
                Map.SectorGraphicsChanges.SideVChanged(SideNum);
                Map.SectorTerrainUndoChanges.SideVChanged(SideNum);
            }
        }

        public override void ActionPerform()
        {
            ToolPerformSideH(PosNum);
            ToolPerformSideH(new XYInt(PosNum.X, PosNum.Y + 1));
            ToolPerformSideV(PosNum);
            ToolPerformSideV(new XYInt(PosNum.X + 1, PosNum.Y));
        }
    }
}