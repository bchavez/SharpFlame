using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpFlame.Core.Collections
{
    public static class ExtensionsForList
    {
        public static void AddRange<TItemType>(this ObservableCollection<TItemType> list, IEnumerable<TItemType> items )
        {
            items.ForEach(list.Add);
        }


        public static void PerformTool<TItemType>(this ObservableCollection<TItemType> list, ISimpleListTool<TItemType> tool)
        {
            list.ForEach(item =>
                {
                    tool.SetItem(item);
                    tool.ActionPerform();
                });
        }
    }
}