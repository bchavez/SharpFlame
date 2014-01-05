using SharpFlame.Collections;

namespace SharpFlame.Mapping.Wz
{
    public struct sCreateWZObjectsArgs
    {
        public SimpleClassList<clsWZBJOUnit> BJOUnits;
        public IniStructures INIStructures;
        public IniDroids INIDroids;
        public IniFeatures INIFeatures;
    }
}