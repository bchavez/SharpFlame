namespace SharpFlame.Collections
{
    internal class ConnectedListItemList<ItemType, SourceType> : SimpleClassList<ConnectedListItem<ItemType, SourceType>> where ItemType : class
        where SourceType : class
    {
        protected override void AfterMoveAction(int position)
        {
            this[position].AfterMove(position);
        }
    }
}