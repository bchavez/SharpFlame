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

        public override void TileChanged(XYInt num)
        {
            var SectorNum = Map.GetTileSectorNum(num);
            Changed(SectorNum);
        }
    }
}