#region

using SharpFlame.Core.Domain;
using SharpFlame.Old.Domain;

#endregion

namespace SharpFlame.Old.Mapping.Objects
{
    public class clsUnitCreate
    {
        public bool AutoWalls = false;
        public XYInt Horizontal;
        public Map Map;
        public UnitTypeBase ObjectTypeBase;
        public bool RandomizeRotation = false;
        public int Rotation = 0;
        public clsUnitGroup UnitGroup;

        public Unit Perform()
        {
            if ( AutoWalls )
            {
                if ( ObjectTypeBase.Type == UnitType.PlayerStructure )
                {
                    var structureTypeBase = (StructureTypeBase)ObjectTypeBase;
                    if ( structureTypeBase.WallLink.IsConnected )
                    {
                        clsWallType AutoWallType = null;
                        AutoWallType = structureTypeBase.WallLink.Source;
                        Map.PerformTileWall(AutoWallType, Map.GetPosTileNum(Horizontal), true);
                        return null;
                    }
                }
            }
            var newUnit = new Unit();
            if ( RandomizeRotation )
            {
                newUnit.Rotation = (int)(App.Random.Next() * 360.0D);
            }
            else
            {
                newUnit.Rotation = Rotation;
            }
            newUnit.UnitGroup = UnitGroup;
            newUnit.Pos = Map.TileAlignedPosFromMapPos(Horizontal, ObjectTypeBase.GetGetFootprintSelected(newUnit.Rotation));
            newUnit.TypeBase = ObjectTypeBase;
            var UnitAdd = new clsUnitAdd();
            UnitAdd.Map = Map;
            UnitAdd.NewUnit = newUnit;
            UnitAdd.StoreChange = true;
            UnitAdd.Perform();
            return newUnit;
        }
    }
}