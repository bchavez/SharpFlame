using SharpFlame.Maths;

namespace SharpFlame.Mapping.Changes
{
    public class clsTerrainUpdate
    {
        public clsPointChanges Vertices;
        public clsPointChanges Tiles;
        public clsPointChanges SidesH;
        public clsPointChanges SidesV;

        public void Deallocate()
        {
        }

        public clsTerrainUpdate(sXY_int TileSize)
        {
            Vertices = new clsPointChanges(new sXY_int(TileSize.X + 1, TileSize.Y + 1));
            Tiles = new clsPointChanges(new sXY_int(TileSize.X, TileSize.Y));
            SidesH = new clsPointChanges(new sXY_int(TileSize.X, TileSize.Y + 1));
            SidesV = new clsPointChanges(new sXY_int(TileSize.X + 1, TileSize.Y));
        }

        public void SetAllChanged()
        {
            Vertices.SetAllChanged();
            Tiles.SetAllChanged();
            SidesH.SetAllChanged();
            SidesV.SetAllChanged();
        }

        public void ClearAll()
        {
            Vertices.Clear();
            Tiles.Clear();
            SidesH.Clear();
            SidesV.Clear();
        }
    }
}