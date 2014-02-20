#region

using SharpFlame.Collections.Specialized;
using SharpFlame.Painters;

#endregion

namespace SharpFlame.Util
{
    public class clsLayer
    {
        public bool[] AvoidLayers;
        public float Density;
        public float HeightMax;
        public float HeightMin;
        //for generator only
        public float Scale;
        public float SlopeMax;
        public float SlopeMin;
        public Terrain Terrain;
        public BooleanMap Terrainmap;
        public int WithinLayer;
    }
}