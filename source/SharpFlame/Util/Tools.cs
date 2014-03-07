namespace SharpFlame.Util
{
    public sealed class modTools
    {
        public static sEachTool Tools;

        private static clsTool tool;

        private static clsTool previousTool;

        public static clsTool Tool
        {
            get { return tool; }
            set
            {
                previousTool = tool;
                tool = value;
            }
        }

        public static clsTool PreviousTool
        {
            get { return previousTool; }
        }

        public static void CreateTools()
        {
            var newTool = default(clsTool);

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

            tool = Tools.TextureBrush;
            previousTool = tool;
        }

        public struct sEachTool
        {
            public clsTool CliffBrush;
            public clsTool CliffRemove;
            public clsTool CliffTriangle;
            public clsTool Gateways;
            public clsTool HeightChangeBrush;
            public clsTool HeightSetBrush;
            public clsTool HeightSmoothBrush;
            public clsTool ObjectLines;
            public clsTool ObjectPlace;
            public clsTool ObjectSelect;
            public clsTool RoadLines;
            public clsTool RoadPlace;
            public clsTool RoadRemove;
            public clsTool TerrainBrush;
            public clsTool TerrainFill;
            public clsTool TerrainSelect;
            public clsTool TextureBrush;
        }
    }

    public class clsTool
    {
    }
}