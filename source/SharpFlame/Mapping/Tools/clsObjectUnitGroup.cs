using SharpFlame.Mapping.Objects;

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectUnitGroup : clsObjectAction
    {
        public clsUnitGroup UnitGroup;

        protected override void _ActionPerform()
        {
            ResultUnit.UnitGroup = UnitGroup;
        }
    }
}