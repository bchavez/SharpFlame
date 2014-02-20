#region

using SharpFlame.Collections;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectSelect : SimpleListTool<clsUnit>
    {
        private clsUnit Unit;

        public void ActionPerform()
        {
            Unit.MapSelect();
        }

        public void SetItem(clsUnit Item)
        {
            Unit = Item;
        }
    }
}