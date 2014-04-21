namespace SharpFlame.UiOptions
{
    public enum MouseTool {
        Default,
        CliffBrush,
        CliffRemove,
        CliffTriangle,
        Gateways,
        HeightChangeBrush,
        HeightSetBrush,
        HeightSmoothBrush,
        ObjectLines,
        ObjectPlace,
        ObjectSelect,
        RoadLines,
        RoadPlace,
        RoadRemove,
        TerrainBrush,
        TerrainFill,
        TerrainSelect,
        TextureBrush
    }

    public class Options
    {
        MouseTool mouseTool;
        public MouseTool MouseTool { 
            get { return mouseTool; }
            set { 
                if (mouseTool != value)
                {
                    mouseTool = value;
                }
            }
        }
     
        public readonly HeightOptions Height;
        public readonly TerrainOptions Terrain;
        public readonly Textures Textures;
        public readonly Minimap Minimap;

        public Options ()
        {
            Height = new HeightOptions();
            Terrain = new TerrainOptions();
            Textures = new Textures();
            Minimap = new Minimap();
        }
    }
}

