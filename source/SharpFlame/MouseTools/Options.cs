using System.Collections.Generic;
using System.Linq;
using SharpFlame.Domain;

namespace SharpFlame.MouseTools
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

    public class ToolOptions
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
        public readonly MinimapOpts MinimapOpts;


        public ToolOptions ()
        {
            Height = new HeightOptions();
            Terrain = new TerrainOptions();
            Textures = new Textures();
            MinimapOpts = new MinimapOpts();
	        PlaceObject = new PlaceObjectOptions();
        }

	    public PlaceObjectOptions PlaceObject { get; set; }
    }

	public class PlaceObjectOptions
	{
		public double Rotation { get; set; }
		public IEnumerable<UnitTypeBase> SelectedObjectTypes { get; set; }
		public bool AutoWalls { get; set; }
		public bool RotationRandom { get; set; }
		public bool RotateFootprints { get; set; }

		public UnitTypeBase SingleSelectedObjectTypeBase
		{
			get
			{
				if( SelectedObjectTypes.Count() == 1 )
				{
					return SelectedObjectTypes.First();
				}
				return null;
			}
		}

	}
}

