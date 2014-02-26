#region

using SharpFlame.Old.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Old.Mapping.Objects;

#endregion

namespace SharpFlame.Old.Mapping.Tools
{
    public class clsObjectSelect : ISimpleListTool<Unit>
    {
        private Unit Unit;

        public void ActionPerform()
        {
            Unit.MapSelect();
        }

        public void SetItem(Unit item)
        {
            Unit = item;
        }
    }
}