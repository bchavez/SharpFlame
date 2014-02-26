#region

using SharpFlame.Old.Mapping.Objects;

#endregion

namespace SharpFlame.Old.Mapping.Tools
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