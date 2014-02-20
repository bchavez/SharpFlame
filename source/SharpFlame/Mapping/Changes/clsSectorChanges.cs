#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping.Changes
{
    public class clsSectorChanges : clsMapTileChanges
    {
        public clsSectorChanges(clsMap Map) : base(Map, Map.SectorCount)
        {
        }

        public override void TileChanged(XYInt Num)
        {
            var SectorNum = Map.GetTileSectorNum(Num);
            Changed(SectorNum);
        }
    }
}