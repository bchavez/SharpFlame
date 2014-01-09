using SharpFlame.Maths;

namespace SharpFlame.Mapping
{
    public class clsShadowSector
    {
        public sXY_int Num;
        public clsTerrain Terrain = new clsTerrain(new sXY_int(Constants.SectorTileSize, Constants.SectorTileSize));
    }
}