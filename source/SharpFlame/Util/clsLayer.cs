using SharpFlame.Collections.Specialized;
using SharpFlame.Painters;

namespace SharpFlame.Util
{
    public class clsLayer
    {
        public int WithinLayer;
        public bool[] AvoidLayers;
        public Terrain Terrain;
        public BooleanMap Terrainmap;
        public float HeightMin;
        public float HeightMax;
        public float SlopeMin;
        public float SlopeMax;
        //for generator only
        public float Scale;
        public float Density;
    }
}