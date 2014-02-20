#region

using System.Collections.Generic;

#endregion

namespace SharpFlame.Collections
{
    public class SimpleList<ItemType> : List<ItemType>
    {
        public bool MaintainOrder = false;

        public void Insert(ItemType newItem, int position)
        {
            Insert (position, newItem);
        }

        public void PerformTool(SimpleListTool<ItemType> tool)
        {
            var A = 0;

            for ( A = 0; A < Count; A++ )
            {
                tool.SetItem(base[A]);
                tool.ActionPerform();
            }
        }

        protected virtual void AfterMoveAction(int position)
        {
        }
   }
}