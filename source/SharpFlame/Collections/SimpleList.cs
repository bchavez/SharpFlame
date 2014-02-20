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

        public virtual void AddList(IList<ItemType> newItems)
        {
            base.AddRange (newItems);
        }

        public virtual void AddSimpleList(SimpleList<ItemType> newItems)
        {
            base.AddRange (newItems);        }

        public void Insert(ItemType newItem, int position)
        {
            base.Insert (position, newItem);
        }

        public void Remove(int position)
        {
            base.RemoveAt (position);
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