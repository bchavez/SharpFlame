

using SharpFlame.Core.Domain;


namespace SharpFlame.Mapping.Changes
{
    public class clsSectorChanges : clsMapTileChanges
    {
        public clsSectorChanges(Map Map) : base(Map, Map.SectorCount)
        {
        }

        public override void TileChanged(XYInt num)
        {
            var SectorNum = Map.GetTileSectorNum(num);
            Changed(SectorNum);
        }
    }
}