namespace SharpFlame.Core.Collections
{
    public abstract class ConnectedListItem<TItemType, TSourceType> where TItemType : class where TSourceType : class
    {
        public abstract TItemType Item { get; }
        public abstract TSourceType Source { get; }
        public abstract bool CanAdd();
        public abstract void BeforeAdd(ConnectedList<TItemType, TSourceType> newList, int newPosition);
        public abstract void BeforeRemove();
        public abstract void AfterRemove();
        public abstract void AfterMove(int newPosition);
        public abstract void Disconnect();
    }
}