#region

using System.Collections.Generic;

#endregion

namespace SharpFlame.Core.Collections
{
    public class SimpleList<TItemType> : List<TItemType>
    {
        public bool MaintainOrder = false;

        public void Insert(TItemType newItem, int position)
        {
            Insert(position, newItem);
        }

        public void PerformTool(ISimpleListTool<TItemType> tool)
        {
            for (var a = 0; a < Count; a++)
            {
                tool.SetItem(base[a]);
                tool.ActionPerform();
            }
        }

        protected virtual void AfterMoveAction(int position)
        {
        }
    }
}