

using System.Diagnostics;
using System.Linq;


namespace SharpFlame.Core.Collections
{
    public class ConnectedListItem<TItem, TOwner> : IConnectedListItem<TItem, TOwner>
        where TItem : class where TOwner : class
    {
        public ConnectedListItem(TItem item)
        {
            this.Item = item;
        }

        public ConnectedList<TItem, TOwner> List { get; private set; }

        public int Position { get; private set; } = -1; //my position in the list

        public bool IsConnected => this.Position >= 0;

        public virtual TItem Item { get; private set; }

        public virtual TOwner Owner => IsConnected ? this.List.Owner : null;

        public void Connect(ConnectedList<TItem, TOwner> list)
        {
            if (IsConnected)
            {
                Debugger.Break();
                return;
            }

            list.Add(this);
        }

        public void ConnectInsert(ConnectedList<TItem, TOwner> list, int pos)
        {
            if (IsConnected)
            {
                Debugger.Break();
                return;
            }

            list.Insert(this, pos);
        }

        public virtual void Disconnect()
        {
            if (this.List == null)
            {
                Debugger.Break();
                return;
            }
            
            this.List.Remove(this.Position);
        }

        public void Deallocate()
        {
            if (IsConnected)
            {
                Disconnect();
            }
            this.Item = null;
        }

        public virtual bool CanAdd()
        {
            return !IsConnected;
        }

        public virtual void OnInserting(ConnectedList<TItem, TOwner> newList, int newPosition)
        {
            this.List = newList;
            this.Position = newPosition;
        }

        public virtual void OnRemoving()
        {
            this.List = null;
            this.Position = -1;
        }

        public virtual void OnRemoved()
        {
        }

        public virtual void OnMoved(int newPosition)
        {
            this.Position = newPosition;
        }
    }
}