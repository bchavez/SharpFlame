namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Runtime.InteropServices;

    [StandardModule]
    public sealed class modTools
    {
        private static clsTool _PreviousTool;
        private static clsTool _Tool;
        public static sEachTool Tools;

        public static void CreateTools()
        {
            clsTool tool = new clsTool();
            Tools.ObjectSelect = tool;
            tool = new clsTool();
            Tools.TextureBrush = tool;
            tool = new clsTool();
            Tools.TerrainBrush = tool;
            tool = new clsTool();
            Tools.TerrainFill = tool;
            tool = new clsTool();
            Tools.RoadPlace = tool;
            tool = new clsTool();
            Tools.RoadLines = tool;
            tool = new clsTool();
            Tools.RoadRemove = tool;
            tool = new clsTool();
            Tools.CliffTriangle = tool;
            tool = new clsTool();
            Tools.CliffBrush = tool;
            tool = new clsTool();
            Tools.CliffRemove = tool;
            tool = new clsTool();
            Tools.HeightSetBrush = tool;
            tool = new clsTool();
            Tools.HeightChangeBrush = tool;
            tool = new clsTool();
            Tools.HeightSmoothBrush = tool;
            tool = new clsTool();
            Tools.ObjectPlace = tool;
            tool = new clsTool();
            Tools.ObjectLines = tool;
            tool = new clsTool();
            Tools.TerrainSelect = tool;
            tool = new clsTool();
            Tools.Gateways = tool;
            _Tool = Tools.TextureBrush;
            _PreviousTool = _Tool;
        }

        public static clsTool PreviousTool
        {
            get
            {
                return _PreviousTool;
            }
        }

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

        [StructLayout(LayoutKind.Sequential)]
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
    }
}

