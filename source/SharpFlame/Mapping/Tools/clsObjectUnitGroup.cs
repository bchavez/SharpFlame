#region

using SharpFlame.Mapping.Objects;

#endregion

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