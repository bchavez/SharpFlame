namespace SharpFlame.Old.Painters
{
    public class TransitionBrush
    {
        public string Name;
        public Terrain TerrainInner;
        public Terrain TerrainOuter;
        public TileList TilesCornerIn = new TileList();
        public TileList TilesCornerOut = new TileList();
        public TileList TilesStraight = new TileList();
    }
}