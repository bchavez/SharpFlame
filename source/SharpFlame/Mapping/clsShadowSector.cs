using SharpFlame.Core.Domain;
using SharpFlame.Maths;

namespace SharpFlame.Mapping
{
    public class clsShadowSector
    {
        public XYInt Num;
        public clsTerrain Terrain = new clsTerrain(new XYInt(Constants.SectorTileSize, Constants.SectorTileSize));
    }
}