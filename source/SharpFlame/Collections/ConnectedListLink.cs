#region

using System.Diagnostics;

#endregion

namespace SharpFlame.Collections
{
    public class ConnectedListLink<ItemType, SourceType> : ConnectedListItem<ItemType, SourceType> where ItemType : class where SourceType : class
    {
        private ConnectedList<ItemType, SourceType> connectedList;
        private ItemType owner;
        private int position = -1;

        public ConnectedListLink(ItemType owner)
        {
            this.owner = owner;
        }

        public ConnectedList<ItemType, SourceType> ParentList
        {
            get { return connectedList; }
        }

        public int ArrayPosition
        {
            get { return position; }
        }

        public bool IsConnected
        {
            get { return position >= 0; }
        }

        public override ItemType Item
        {
            get { return owner; }
        }

        public override SourceType Source
        {
            get
            {
                if ( IsConnected )
                {
                    return connectedList.Owner;
                }
                return null;
            }
        }

        public override void AfterMove(int newPosition)
        {
            this.position = newPosition;
        }

        public override void BeforeRemove()
        {
            connectedList = null;
            position = -1;
        }

        public void Connect(ConnectedList<ItemType, SourceType> List)
        {
            if ( IsConnected )
            {
                Debugger.Break();
                return;
            }

            List.Add(this);
        }

        public void ConnectInsert(ConnectedList<ItemType, SourceType> list, int position)
        {
            if ( IsConnected )
            {
                Debugger.Break();
                return;
            }

            list.Insert(this, position);
        }

        public override void Disconnect()
        {
            if ( connectedList == null )
            {
                Debugger.Break();
                return;
            }

            connectedList.Remove(connectedList.Count - 1);
        }

        public void Deallocate()
        {
            if ( IsConnected )
            {
                Disconnect();
            }
            owner = null;
        }

        public override void AfterRemove()
        {
        }

        public override bool CanAdd()
        {
            return !IsConnected;
        }

        public override void BeforeAdd(ConnectedList<ItemType, SourceType> newList, int newPosition)
        {
            connectedList = newList;
            position = newPosition;
        }
    }
}