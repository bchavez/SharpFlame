using SharpFlame.Maths;

namespace SharpFlame.Mapping.Changes
{
    public class clsAutoTextureChanges : clsMapTileChanges
    {
        public clsAutoTextureChanges(clsMap Map) : base(Map, Map.Terrain.TileSize)
        {
        }

        public override void TileChanged(sXY_int Num)
        {
            Changed(Num);
        }
    }
}