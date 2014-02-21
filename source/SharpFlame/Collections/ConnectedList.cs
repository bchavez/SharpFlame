#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace SharpFlame.Collections
{
    public class ConnectedList<ItemType, SourceType> : IEnumerable<ItemType> where ItemType : class where SourceType : class
    {
        private readonly ConnectedListItemList<ItemType, SourceType> list = new ConnectedListItemList<ItemType, SourceType>();
        private SourceType source;

        public ConnectedList(SourceType owner)
        {
            source = owner;
            list.AddNullItemBehavior = AddNullItemBehavior.DisallowError;
        }

        public SourceType Owner
        {
            get { return source; }
        }

        public bool MaintainOrder
        {
            get { return list.MaintainOrder; }
            set { list.MaintainOrder = value; }
        }

        public ItemType this[int position]
        {
            get { return list[position].Item; }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public IEnumerator<ItemType> GetEnumerator()
        {
            return GetEnumeratorType();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ConnectedListItem<ItemType, SourceType> get_ItemContainer(int position)
        {
            return list[position];
        }

        public virtual void Add(ConnectedListItem<ItemType, SourceType> newItem)
        {
            if ( newItem.CanAdd() )
            {
                newItem.BeforeAdd(this, list.Count);
                list.Add(newItem);
            }
        }

        public virtual void Insert(ConnectedListItem<ItemType, SourceType> newItem, int position)
        {
            if ( newItem.CanAdd() )
            {
                newItem.BeforeAdd(this, position);
                list.Insert(newItem, position);
            }
        }

        public virtual void Remove(int position)
        {
            var removeItem = default(ConnectedListItem<ItemType, SourceType>);

            removeItem = list[position];
            removeItem.BeforeRemove();
            list.RemoveAt(position);
            removeItem.AfterRemove();
        }

        public ConnectedListItem<ItemType, SourceType> FindLinkTo(ItemType itemToFind)
        {
            foreach ( var link in list )
            {
                if ( link.Item == itemToFind )
                {
                    return link;
                }
            }
            return default(ConnectedListItem<ItemType, SourceType>);
        }

        public void Deallocate()
        {
            Clear();
            source = null;
        }

        public SimpleList<ItemType> GetItemsAsSimpleList()
        {
            var result = new SimpleList<ItemType>();

            foreach ( var connectedItem in this )
            {
                result.Add(connectedItem);
            }

            return result;
        }

        public SimpleClassList<ItemType> GetItemsAsSimpleClassList()
        {
            var result = new SimpleClassList<ItemType>();

            foreach ( var connectedItem in this )
            {
                result.Add(connectedItem);
            }

            return result;
        }

        public void Clear()
        {
            while ( list.Count > 0 )
            {
                Remove(0);
            }
        }

        public IEnumerator<ItemType> GetEnumeratorType()
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
            private readonly ConnectedList<ItemType, SourceType> list;
            private int position = StartPosition;

            public Enumerator(ConnectedList<ItemType, SourceType> list)
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

        public class EnumeratorType : IEnumerator<ItemType>
        {
            private const int StartPosition = -1;
            private readonly ConnectedList<ItemType, SourceType> list;
            private int position = StartPosition;

            public EnumeratorType(ConnectedList<ItemType, SourceType> list)
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

            public ItemType Current
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

            #region IDisposable Support

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
                if ( !disposedValue )
                {
                    if ( disposing )
                    {
                        // TODO: dispose managed state (managed objects).
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    // TODO: set large fields to null.
                }
                disposedValue = true;
            }

            #endregion
        }
    }
}