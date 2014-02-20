#region

using SharpFlame.Mapping.Changes;

#endregion

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public clsAutoTextureChanges AutoTextureChanges;
        public clsSectorChanges SectorGraphicsChanges;
        public clsSectorChanges SectorTerrainUndoChanges;
        public clsSectorChanges SectorUnitHeightsChanges;
        public clsTerrainUpdate TerrainInterpretChanges;
    }
}