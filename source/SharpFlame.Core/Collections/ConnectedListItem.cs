namespace SharpFlame.Core.Collections
{
    public interface IConnectedListItem<TItemType, TSourceType> where TItemType : class where TSourceType : class
    {
        TItemType Item { get; }
        TSourceType Owner { get; }
        bool CanAdd();
        void OnInserting(ConnectedList<TItemType, TSourceType> newList, int newPosition);
        void OnRemoving();
        void OnRemoved();
        void OnMoved(int newPosition);
        void Disconnect();
    }
}