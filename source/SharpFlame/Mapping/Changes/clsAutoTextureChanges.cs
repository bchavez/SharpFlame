#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping.Changes
{
    public class clsAutoTextureChanges : clsMapTileChanges
    {
        public clsAutoTextureChanges(Map map) : base(map, map.Terrain.TileSize)
        {
        }

        public override void TileChanged(XYInt num)
        {
            Changed(num);
        }
    }
}