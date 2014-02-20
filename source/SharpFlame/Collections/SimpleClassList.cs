#region

using System.Diagnostics;

#endregion

namespace SharpFlame.Collections
{
    public class SimpleClassList<ItemType> : SimpleList<ItemType> where ItemType : class
    {
        public AddNullItemBehavior AddNullItemBehavior = AddNullItemBehavior.Allow;

        public override void Add(ItemType NewItem)
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

        public int FindFirstItemPosition(ItemType ItemToFind)
        {
            var Position = 0;

            for ( Position = 0; Position <= Count - 1; Position++ )
            {
                if ( this[Position] == ItemToFind )
                {
                    return Position;
                }
            }
            return -1;
        }
    }
}