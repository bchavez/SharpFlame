using SharpFlame.Maths;

namespace SharpFlame.Mapping.Changes
{
    public class clsSectorChanges : clsMapTileChanges
    {
        public clsSectorChanges(clsMap Map) : base(Map, Map.SectorCount)
        {
        }

        public override void TileChanged(sXY_int Num)
        {
            sXY_int SectorNum = new sXY_int();

            SectorNum = Map.GetTileSectorNum(Num);
            Changed(SectorNum);
        }
    }
}