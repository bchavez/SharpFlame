#region

using SharpFlame.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping.Tools
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