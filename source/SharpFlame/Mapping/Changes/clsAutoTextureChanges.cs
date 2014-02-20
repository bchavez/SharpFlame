#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping.Changes
{
    public class clsAutoTextureChanges : clsMapTileChanges
    {
        public clsAutoTextureChanges(clsMap Map) : base(Map, Map.Terrain.TileSize)
        {
        }

        public override void TileChanged(XYInt Num)
        {
            Changed(Num);
        }
    }
}