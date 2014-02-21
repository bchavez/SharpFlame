#region

using System.Diagnostics;

#endregion

namespace SharpFlame.Collections
{
    public class SimpleClassList<ItemType> : SimpleList<ItemType> where ItemType : class
    {
        public AddNullItemBehavior AddNullItemBehavior = AddNullItemBehavior.Allow;

        public new void Add(ItemType NewItem)
        {
            switch ( AddNullItemBehavior )
            {
                case AddNullItemBehavior.Allow:
                    base.Add(NewItem);
                    break;
                case AddNullItemBehavior.DisallowIgnore:
                    if ( NewItem != null )
                    {
                        base.Add(NewItem);
                    }
                    break;
                case AddNullItemBehavior.DisallowError:
                    if ( NewItem == null )
                    {
                        Debugger.Break();
                    }
                    else
                    {
                        base.Add(NewItem);
                    }
                    break;
                default:
                    Debugger.Break();
                    break;
            }
        }

        public int FindFirstItemPosition(ItemType itemToFind)
        {
            var position = 0;

            for ( position = 0; position < Count ; position++ )
            {
                if ( this[position] == itemToFind )
                {
                    return position;
                }
            }
            return -1;
        }
    }
}