

using System.Diagnostics;



namespace SharpFlame.Core.Collections
{
    public class SimpleClassList<TItemType> : SimpleList<TItemType> where TItemType : class
    {
        public AddNullItemBehavior AddNullItemBehavior = AddNullItemBehavior.Allow;

        public new void Add(TItemType newItem)
        {
            switch (AddNullItemBehavior)
            {
                case AddNullItemBehavior.Allow:
                    base.Add(newItem);
                    break;
                case AddNullItemBehavior.DisallowIgnore:
                    if (newItem != null)
                    {
                        base.Add(newItem);
                    }
                    break;
                case AddNullItemBehavior.DisallowError:
                    if (newItem == null)
                    {
                        Debugger.Break();
                    }
                    else
                    {
                        base.Add(newItem);
                    }
                    break;
                default:
                    Debugger.Break();
                    break;
            }
        }

        public int FindFirstItemPosition(TItemType itemToFind)
        {
            return IndexOf(itemToFind);
        }
    }
}