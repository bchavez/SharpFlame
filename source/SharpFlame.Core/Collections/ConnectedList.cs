using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace SharpFlame.Core.Collections
{
    public class ConnectedList<TItem, TOwner> : IEnumerable<TItem>
        where TItem : class
        where TOwner : class
    {
        private readonly ObservableCollection<IConnectedListItem<TItem, TOwner>> list = new ObservableCollection<IConnectedListItem<TItem, TOwner>>();

        public ConnectedList(TOwner owner)
        {
            this.Owner = owner;

            list.CollectionChanged += (sender, args) =>
                {
                    if( args.Action == NotifyCollectionChangedAction.Add &&
                        args.NewItems.Contains(null) )
                    {
                        throw new ArgumentNullException(nameof(args.NewItems), "Adding null values is disalowed.");
                    }
                };
        }

        public bool IsBusy { get; private set; } = false;

        public TOwner Owner { get; private set; }

        public TItem this[int position] => list[position].Item;

        public int Count => list.Count;

        public IEnumerator<TItem> GetEnumerator()
        {
            return list.Select(l => l.Item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IConnectedListItem<TItem, TOwner> GetLinkItem(int position)
        {
            return list[position];
        }

        public virtual void Add(IConnectedListItem<TItem, TOwner> newItem)
        {
            if( this.IsBusy ) return;

            this.IsBusy = true;

            if( newItem.CanAdd())
            {
                newItem.OnInserting(this, 0.Max(list.Count));
                list.Add(newItem);
            }

            this.IsBusy = false;
        }

        public virtual void Insert(IConnectedListItem<TItem, TOwner> newItem, int position)
        {
            if( this.IsBusy ) return;

            this.IsBusy = true;

            if( newItem.CanAdd())
            {
                newItem.OnInserting(this, position);
                list.Insert(position, newItem);
                for( var i = position + 1; i < list.Count; i++ )
                {
                    list[i].OnMoved(i);
                }
            }

            this.IsBusy = false;
        }

        public virtual void Remove(int position)
        {
            if( this.IsBusy ) return;

            this.IsBusy = true;

            var removeItem = list[position];
            removeItem.OnRemoving();
            list.RemoveAt(position);
            for( var i = position; i < list.Count; i++ )
            {
                list[i].OnMoved(i);
            }
            removeItem.OnRemoved();


            this.IsBusy = false;
        }

        public IConnectedListItem<TItem, TOwner> FindLinkTo(TItem itemToFind)
        {
            return list.FirstOrDefault(link => link.Item == itemToFind);
        }

        public void Deallocate()
        {
            Clear();
            this.Owner = null;

            this.IsBusy = true;
        }

        public ObservableCollection<TItem> CopyList()
        {
            var result = new ObservableCollection<TItem>();
            result.AddRange(this.ToArray());
            return result;
        }

        public void Clear()
        {
            while (list.Count > 0)
            {
                Remove(0);
            }
        }
    }
}