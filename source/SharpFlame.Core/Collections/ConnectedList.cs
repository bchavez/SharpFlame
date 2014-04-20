

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



namespace SharpFlame.Core.Collections
{
    public class ConnectedList<TItemType, TSourceType> : IEnumerable<TItemType> where TItemType : class
        where TSourceType : class
    {
        private readonly ConnectedListItemList<TItemType, TSourceType> list =
            new ConnectedListItemList<TItemType, TSourceType>();

        private TSourceType source;

        public ConnectedList(TSourceType owner)
        {
            source = owner;
            list.AddNullItemBehavior = AddNullItemBehavior.DisallowError;
        }

        public TSourceType Owner
        {
            get { return source; }
        }

        public bool MaintainOrder
        {
            get { return list.MaintainOrder; }
            set { list.MaintainOrder = value; }
        }

        public TItemType this[int position]
        {
            get { return list[position].Item; }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public IEnumerator<TItemType> GetEnumerator()
        {
            return GetEnumeratorType();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ConnectedListItem<TItemType, TSourceType> get_ItemContainer(int position)
        {
            return list[position];
        }

        public virtual void Add(ConnectedListItem<TItemType, TSourceType> newItem)
        {
            if (newItem.CanAdd())
            {
                newItem.BeforeAdd(this, list.Count);
                list.Add(newItem);
            }
        }

        public virtual void Insert(ConnectedListItem<TItemType, TSourceType> newItem, int position)
        {
            if (newItem.CanAdd())
            {
                newItem.BeforeAdd(this, position);
                list.Insert(newItem, position);
            }
        }

        public virtual void Remove(int position)
        {
            var removeItem = list[position];
            removeItem.BeforeRemove();
            list.RemoveAt(position);
            removeItem.AfterRemove();
        }

        public ConnectedListItem<TItemType, TSourceType> FindLinkTo(TItemType itemToFind)
        {
            return list.FirstOrDefault(link => link.Item == itemToFind);
        }

        public void Deallocate()
        {
            Clear();
            source = null;
        }

        public SimpleList<TItemType> GetItemsAsSimpleList()
        {
            var result = new SimpleList<TItemType>();
            result.AddRange(this);
            return result;
        }

        public SimpleClassList<TItemType> GetItemsAsSimpleClassList()
        {
            var result = new SimpleClassList<TItemType>();

            foreach (var connectedItem in this)
            {
                result.Add(connectedItem);
            }

            return result;
        }

        public void Clear()
        {
            while (list.Count > 0)
            {
                Remove(0);
            }
        }

        public IEnumerator<TItemType> GetEnumeratorType()
        {
            return new EnumeratorType(this);
        }

        //public System.Collections.IEnumerator GetEnumerator()
        //{

        //#if !Mono
        //return new Enumerator(this);
        //#else
        //							return new Enumerator(modLists.MonoWorkaroundConnectedList<ItemType, SourceType>(this));
        //#endif
        //}

        public class Enumerator : IEnumerator
        {
            private const int StartPosition = -1;
            private readonly ConnectedList<TItemType, TSourceType> list;
            private int position = StartPosition;

            public Enumerator(ConnectedList<TItemType, TSourceType> list)
            {
                this.list = list;
            }

            public object Current
            {
                get { return list[position]; }
            }

            public bool MoveNext()
            {
                position++;
                return position < list.Count;
            }

            public void Reset()
            {
                position = StartPosition;
            }
        }

        public class EnumeratorType : IEnumerator<TItemType>
        {
            private const int StartPosition = -1;
            private readonly ConnectedList<TItemType, TSourceType> list;
            private int position = StartPosition;

            public EnumeratorType(ConnectedList<TItemType, TSourceType> list)
            {
                this.list = list;
            }

            //public object Current
            //{
            //get
            //{
            //return this.Current1;
            //}
            //}

            public object Current1
            {
                get { return list[position]; }
            }

            public TItemType Current
            {
                get { return list[position]; }
            }

            public bool MoveNext()
            {
                position++;
                return position < list.Count;
            }

            public void Reset()
            {
                position = StartPosition;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            
            private bool disposedValue; // To detect redundant calls

            // IDisposable

            // TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
            //Protected Overrides Sub Finalize()
            //    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
            //    Dispose(False)
            //    MyBase.Finalize()
            //End Sub

            // This code added by Visual Basic to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    // TODO: set large fields to null.
                }
                disposedValue = true;
            }

            
        }
    }
}