#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace SharpFlame.Collections
{
    public class SimpleList<ItemType> : List<ItemType>
    {
        public bool MaintainOrder = false;

        public void Insert(ItemType newItem, int position)
        {
            base.Insert (position, newItem);
        }

        public void Deallocate()
        {
            Clear();
        }

        public void PerformTool(SimpleListTool<ItemType> tool)
        {
            var A = 0;

            for ( A = 0; A < base.Count; A++ )
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