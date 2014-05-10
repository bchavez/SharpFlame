

using SharpFlame.Core;
using SharpFlame.Core.Domain;


namespace SharpFlame.Mapping
{
    public class ShadowSector
    {
        public XYInt Num;
        public clsTerrain Terrain = new clsTerrain(new XYInt(Constants.SectorTileSize, Constants.SectorTileSize));
    }
}