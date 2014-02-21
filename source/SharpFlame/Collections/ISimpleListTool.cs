namespace SharpFlame.Collections
{
    public interface ISimpleListTool<ItemType>
    {
        void SetItem(ItemType item);
        void ActionPerform();
    }
}