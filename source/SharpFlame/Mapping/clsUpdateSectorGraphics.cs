using SharpFlame.Mapping.Tools;

namespace SharpFlame.Mapping
{
    public class clsUpdateSectorGraphics : clsAction
    {
        public override void ActionPerform()
        {
            Map.Sector_GLList_Make(PosNum.X, PosNum.Y);
            Map.MinimapMakeLater();
        }
    }
}