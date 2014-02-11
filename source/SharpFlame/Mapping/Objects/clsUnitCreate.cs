using SharpFlame.Domain;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Objects
{
    public class clsUnitCreate
    {
        public clsMap Map;
        public UnitTypeBase ObjectTypeBase;
        public sXY_int Horizontal;
        public clsUnitGroup UnitGroup;
        public bool AutoWalls = false;
        public int Rotation = 0;
        public bool RandomizeRotation = false;

        public clsUnit Perform()
        {
            if ( AutoWalls )
            {
                if ( ObjectTypeBase.Type == UnitType.PlayerStructure )
                {
                    StructureTypeBase structureTypeBase = (StructureTypeBase)ObjectTypeBase;
                    if ( structureTypeBase.WallLink.IsConnected )
                    {
                        clsWallType AutoWallType = null;
                        AutoWallType = structureTypeBase.WallLink.Source;
                        Map.PerformTileWall(AutoWallType, Map.GetPosTileNum(Horizontal), true);
                        return null;
                    }
                }
            }
            clsUnit newUnit = new clsUnit();
            if ( RandomizeRotation )
            {
                newUnit.Rotation = (int)(App.Random.Next() * 360.0D);
            }
            else
            {
                newUnit.Rotation = Rotation;
            }
            newUnit.UnitGroup = UnitGroup;
            newUnit.Pos = Map.TileAlignedPosFromMapPos(Horizontal, ObjectTypeBase.get_GetFootprintSelected(newUnit.Rotation));
            newUnit.TypeBase = ObjectTypeBase;
            clsUnitAdd UnitAdd = new clsUnitAdd();
            UnitAdd.Map = Map;
            UnitAdd.NewUnit = newUnit;
            UnitAdd.StoreChange = true;
            UnitAdd.Perform();
            return newUnit;
        }
    }
}