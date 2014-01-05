namespace SharpFlame
{
	public sealed class modTools
	{
		
		public struct sEachTool
		{
			public clsTool ObjectSelect;
			public clsTool TextureBrush;
			public clsTool TerrainBrush;
			public clsTool TerrainFill;
			public clsTool RoadPlace;
			public clsTool RoadLines;
			public clsTool RoadRemove;
			public clsTool CliffTriangle;
			public clsTool CliffBrush;
			public clsTool CliffRemove;
			public clsTool HeightSetBrush;
			public clsTool HeightChangeBrush;
			public clsTool HeightSmoothBrush;
			public clsTool ObjectPlace;
			public clsTool ObjectLines;
			public clsTool TerrainSelect;
			public clsTool Gateways;
		}
		public static sEachTool Tools;
		
		public static void CreateTools()
		{
			
			clsTool newTool = default(clsTool);
			
			newTool = new clsTool();
			Tools.ObjectSelect = newTool;
			
			newTool = new clsTool();
			Tools.TextureBrush = newTool;
			
			newTool = new clsTool();
			Tools.TerrainBrush = newTool;
			
			newTool = new clsTool();
			Tools.TerrainFill = newTool;
			
			newTool = new clsTool();
			Tools.RoadPlace = newTool;
			
			newTool = new clsTool();
			Tools.RoadLines = newTool;
			
			newTool = new clsTool();
			Tools.RoadRemove = newTool;
			
			newTool = new clsTool();
			Tools.CliffTriangle = newTool;
			
			newTool = new clsTool();
			Tools.CliffBrush = newTool;
			
			newTool = new clsTool();
			Tools.CliffRemove = newTool;
			
			newTool = new clsTool();
			Tools.HeightSetBrush = newTool;
			
			newTool = new clsTool();
			Tools.HeightChangeBrush = newTool;
			
			newTool = new clsTool();
			Tools.HeightSmoothBrush = newTool;
			
			newTool = new clsTool();
			Tools.ObjectPlace = newTool;
			
			newTool = new clsTool();
			Tools.ObjectLines = newTool;
			
			newTool = new clsTool();
			Tools.TerrainSelect = newTool;
			
			newTool = new clsTool();
			Tools.Gateways = newTool;
			
			_Tool = Tools.TextureBrush;
			_PreviousTool = _Tool;
		}
		
		private static clsTool _Tool;
public static clsTool Tool
		{
			get
			{
				return _Tool;
			}
			set
			{
				_PreviousTool = _Tool;
				_Tool = value;
			}
		}
		private static clsTool _PreviousTool;
public static clsTool PreviousTool
		{
			get
			{
				return _PreviousTool;
			}
		}
	}
	
	public class clsTool
	{
		
	}
	
}
