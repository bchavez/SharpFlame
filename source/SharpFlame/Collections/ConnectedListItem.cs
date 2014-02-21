namespace SharpFlame.Collections
{
    public abstract class ConnectedListItem<ItemType, SourceType> where ItemType : class where SourceType : class
    {
        public abstract ItemType Item { get; }
        public abstract SourceType Source { get; }
        public abstract bool CanAdd();
        public abstract void BeforeAdd(ConnectedList<ItemType, SourceType> newList, int newPosition);
        public abstract void BeforeRemove();
        public abstract void AfterRemove();
        public abstract void AfterMove(int newPosition);
        public abstract void Disconnect();
    }
}