namespace SharpFlame.UiOptions
{
    public class HeightOptions
    {
        public readonly clsBrush Brush;
        public double LmbHeight { get; set; }
        public double RmbHeight { get; set; }

        public double ChangeRate { get; set; }
        public bool ChangeFade { get; set; }

        public double SmoothRate { get; set; }

        public HeightOptions()
        {
            Brush = new clsBrush(2.0D, ShapeType.Circle);
            LmbHeight = 85D;
            RmbHeight = 0D;

            ChangeRate = 16D;
            ChangeFade = true;

            SmoothRate = 3D;
        }
    }
}

