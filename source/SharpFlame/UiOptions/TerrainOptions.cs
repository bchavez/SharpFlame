namespace SharpFlame.UiOptions
{
    public class TerrainOptions
    {
        public readonly clsBrush Brush;
        public readonly clsBrush CliffBrush;

        public TerrainOptions()
        {
            Brush = new clsBrush(2.0D, ShapeType.Circle);
            CliffBrush = new clsBrush(2.0D, ShapeType.Circle);
        }
    }
}

