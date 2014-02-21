namespace SharpFlame.Core.Collections
{
    internal class ConnectedListItemList<TItemType, TSourceType> :
        SimpleClassList<ConnectedListItem<TItemType, TSourceType>> where TItemType : class
        where TSourceType : class
    {
        protected override void AfterMoveAction(int position)
        {
            this[position].AfterMove(position);
        }
    }
}