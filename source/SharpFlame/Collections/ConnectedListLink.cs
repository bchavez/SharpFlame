using System.Diagnostics;

namespace SharpFlame.Collections
{
    public class ConnectedListLink<ItemType, SourceType> : ConnectedListItem<ItemType, SourceType> where ItemType : class where SourceType : class
    {
        private ItemType Owner;
        private ConnectedList<ItemType, SourceType> ConnectedList;
        private int Position = -1;

        public ConnectedListLink(ItemType Owner)
        {
            this.Owner = Owner;
        }

        public ConnectedList<ItemType, SourceType> ParentList
        {
            get { return ConnectedList; }
        }

        public int ArrayPosition
        {
            get { return Position; }
        }

        public bool IsConnected
        {
            get { return Position >= 0; }
        }

        public override void AfterMove(int Position)
        {
            this.Position = Position;
        }

        public override void BeforeRemove()
        {
            ConnectedList = null;
            Position = -1;
        }

        public override ItemType Item
        {
            get { return Owner; }
        }

        public override SourceType Source
        {
            get
            {
                if ( IsConnected )
                {
                    return ConnectedList.Owner;
                }
                else
                {
                    return null;
                }
            }
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

        public void ConnectInsert(ConnectedList<ItemType, SourceType> List, int Position)
        {
            if ( IsConnected )
            {
                Debugger.Break();
                return;
            }

            List.Insert(this, Position);
        }

        public override void Disconnect()
        {
            if ( ConnectedList == null )
            {
                Debugger.Break();
                return;
            }

            ConnectedList.Remove(Position);
        }

        public void Deallocate()
        {
            if ( IsConnected )
            {
                Disconnect();
            }
            Owner = null;
        }

        public override void AfterRemove()
        {
        }

        public override bool CanAdd()
        {
            return !IsConnected;
        }

        public override void BeforeAdd(ConnectedList<ItemType, SourceType> NewList, int NewPosition)
        {
            ConnectedList = NewList;
            Position = NewPosition;
        }
    }
}