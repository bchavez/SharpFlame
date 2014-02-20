#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping.Changes
{
    public class clsTerrainUpdate
    {
        public clsPointChanges SidesH;
        public clsPointChanges SidesV;
        public clsPointChanges Tiles;
        public clsPointChanges Vertices;

        public clsTerrainUpdate(XYInt TileSize)
        {
            Vertices = new clsPointChanges(new XYInt(TileSize.X + 1, TileSize.Y + 1));
            Tiles = new clsPointChanges(new XYInt(TileSize.X, TileSize.Y));
            SidesH = new clsPointChanges(new XYInt(TileSize.X, TileSize.Y + 1));
            SidesV = new clsPointChanges(new XYInt(TileSize.X + 1, TileSize.Y));
        }

        public void Deallocate()
        {
            Vertices = null;
            Tiles = null;
            SidesH = null;
            SidesV = null;
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