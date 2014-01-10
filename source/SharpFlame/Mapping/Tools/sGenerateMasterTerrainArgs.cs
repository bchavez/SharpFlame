using SharpFlame.Collections.Specialized;
using SharpFlame.Mapping.Tiles;

namespace SharpFlame.Mapping.Tools
{
    public struct sGenerateMasterTerrainArgs
    {
        public clsGeneratorTileset Tileset;
        public int LevelCount;

        public class clsLayer
        {
            public int WithinLayer;
            public bool[] AvoidLayers;
            public int TileNum;
            public BooleanMap Terrainmap;
            public float TerrainmapScale;
            public float TerrainmapDensity;
            public float HeightMin;
            public float HeightMax;
            public bool IsCliff;
        }

        public clsLayer[] Layers;
        public int LayerCount;
        public BooleanMap Watermap;
    }
}