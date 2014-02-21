namespace SharpFlame.Core.Collections
{
    public interface ISimpleListTool<ItemType>
    {
        void SetItem(ItemType item);
        void ActionPerform();
    }
}