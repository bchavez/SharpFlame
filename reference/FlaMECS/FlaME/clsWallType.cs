namespace FlaME
{
    using System;

    public class clsWallType
    {
        public string Code;
        private const int d0 = 0;
        private const int d1 = 90;
        private const int d2 = 180;
        private const int d3 = 270;
        public string Name;
        public modLists.ConnectedList<clsStructureType, clsWallType> Segments;
        public int[] TileWalls_Direction;
        public int[] TileWalls_Segment;
        public modLists.ConnectedListLink<clsWallType, clsObjectData> WallType_ObjectDataLink;

        public clsWallType()
        {
            this.WallType_ObjectDataLink = new modLists.ConnectedListLink<clsWallType, clsObjectData>(this);
            this.Code = "";
            this.Name = "Unknown";
            this.TileWalls_Segment = new int[] { 0, 0, 0, 0, 0, 3, 3, 2, 0, 3, 3, 2, 0, 2, 2, 1 };
            this.TileWalls_Direction = new int[] { 0, 0, 180, 0, 270, 0, 270, 0, 90, 90, 180, 180, 270, 90, 270, 0 };
            this.Segments = new modLists.ConnectedList<clsStructureType, clsWallType>(this);
            this.Segments.MaintainOrder = true;
        }
    }
}

