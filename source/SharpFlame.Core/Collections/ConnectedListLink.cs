

using System.Diagnostics;



namespace SharpFlame.Core.Collections
{
    public class ConnectedListLink<TItemType, TSourceType> : ConnectedListItem<TItemType, TSourceType>
        where TItemType : class where TSourceType : class
    {
        private ConnectedList<TItemType, TSourceType> connectedList;
        private TItemType owner;
        private int position = -1;

        public ConnectedListLink(TItemType owner)
        {
            this.owner = owner;
        }

        public ConnectedList<TItemType, TSourceType> ParentList
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

        public override TItemType Item
        {
            get { return owner; }
        }

        public override TSourceType Source
        {
            get
            {
                if (IsConnected)
                {
                    return connectedList.Owner;
                }
                return null;
            }
        }

        public override void AfterMove(int newPosition)
        {
            position = newPosition;
        }

        public override void BeforeRemove()
        {
            connectedList = null;
            position = -1;
        }

        public void Connect(ConnectedList<TItemType, TSourceType> list)
        {
            if (IsConnected)
            {
                Debugger.Break();
                return;
            }

            list.Add(this);
        }

        public void ConnectInsert(ConnectedList<TItemType, TSourceType> list, int pos)
        {
            if (IsConnected)
            {
                Debugger.Break();
                return;
            }

            list.Insert(this, pos);
        }

        public override void Disconnect()
        {
            if (connectedList == null)
            {
                Debugger.Break();
                return;
            }

            connectedList.Remove(connectedList.Count - 1);
        }

        public void Deallocate()
        {
            if (IsConnected)
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

        public override void BeforeAdd(ConnectedList<TItemType, TSourceType> newList, int newPosition)
        {
            connectedList = newList;
            position = newPosition;
        }
    }
}