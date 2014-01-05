namespace SharpFlame.Mapping.Renderers
{
    public abstract class clsDrawTile
    {
        public clsMap Map;
        public int TileX;
        public int TileY;

        public abstract void Perform();
    }
}