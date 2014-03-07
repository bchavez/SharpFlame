namespace SharpFlame.Painters
{
    public class CliffBrush
    {
        public string Name;
        public Terrain Terrain_Inner;
        public Terrain Terrain_Outer;
        public TileList Tiles_Corner_In = new TileList();
        public TileList Tiles_Corner_Out = new TileList();
        public TileList Tiles_Straight = new TileList();
    }
}