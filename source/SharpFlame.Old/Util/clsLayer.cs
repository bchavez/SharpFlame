#region

using SharpFlame.Old.Collections.Specialized;
using SharpFlame.Old.Collections.Specialized;
using SharpFlame.Old.Painters;

#endregion

namespace SharpFlame.Old.Util
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