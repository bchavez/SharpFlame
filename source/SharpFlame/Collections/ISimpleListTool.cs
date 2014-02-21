namespace SharpFlame.Collections
{
    public interface ISimpleListTool<ItemType>
    {
        void SetItem(ItemType Item);
        void ActionPerform();
    }
}