#region

using SharpFlame.Old.Collections.Specialized;
using SharpFlame.Old.Mapping.Tiles;

#endregion

namespace SharpFlame.Old.Mapping.Tools
{
    public struct sGenerateMasterTerrainArgs
    {
        public int LayerCount;
        public clsLayer[] Layers;
        public int LevelCount;
        public clsGeneratorTileset Tileset;
        public BooleanMap Watermap;

        public class clsLayer
        {
            public bool[] AvoidLayers;
            public float HeightMax;
            public float HeightMin;
            public bool IsCliff;
            public BooleanMap Terrainmap;
            public float TerrainmapDensity;
            public float TerrainmapScale;
            public int TileNum;
            public int WithinLayer;
        }
    }
}