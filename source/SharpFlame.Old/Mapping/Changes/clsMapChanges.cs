#region

using SharpFlame.Old.Mapping.Changes;
using SharpFlame.Old.Mapping.Changes;

#endregion

namespace SharpFlame.Old.Mapping
{
    public partial class Map
    {
        public clsAutoTextureChanges AutoTextureChanges;
        public clsSectorChanges SectorGraphicsChanges;
        public clsSectorChanges SectorTerrainUndoChanges;
        public clsSectorChanges SectorUnitHeightsChanges;
        public clsTerrainUpdate TerrainInterpretChanges;
    }
}