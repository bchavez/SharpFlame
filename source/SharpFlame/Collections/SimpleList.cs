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

        public void PerformTool(ISimpleListTool<ItemType> tool)
        {
            var a = 0;

            for ( a = 0; a < Count; a++ )
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