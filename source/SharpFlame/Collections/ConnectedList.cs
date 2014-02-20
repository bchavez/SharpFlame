#region

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace SharpFlame.Collections
{
    public class ConnectedList<ItemType, SourceType> : IEnumerable<ItemType> where ItemType : class where SourceType : class
    {
        private readonly ConnectedListItemList<ItemType, SourceType> List = new ConnectedListItemList<ItemType, SourceType>();
        private SourceType Source;

        public ConnectedList(SourceType Owner)
        {
            Source = Owner;
            List.AddNullItemBehavior = AddNullItemBehavior.DisallowError;
        }

        public SourceType Owner
        {
            get { return Source; }
        }

        public bool MaintainOrder
        {
            get { return List.MaintainOrder; }
            set { List.MaintainOrder = value; }
        }

        public ItemType this[int Position]
        {
            get { return List[Position].Item; }
        }

        public int Count
        {
            get { return List.Count; }
        }

        public IEnumerator<ItemType> GetEnumerator()
        {
            return GetEnumeratorType();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ConnectedListItem<ItemType, SourceType> get_ItemContainer(int Position)
        {
            return List[Position];
        }

        public virtual void Add(ConnectedListItem<ItemType, SourceType> NewItem)
        {
            if ( NewItem.CanAdd() )
            {
                NewItem.BeforeAdd(ListUtil.MonoWorkaroundConnectedList<ItemType, SourceType>(this), List.Count);
                List.Add(NewItem);
            }
        }

        public virtual void Insert(ConnectedListItem<ItemType, SourceType> NewItem, int Position)
        {
            if ( NewItem.CanAdd() )
            {
                NewItem.BeforeAdd(ListUtil.MonoWorkaroundConnectedList<ItemType, SourceType>(this), Position);
                List.Insert(NewItem, Position);
            }
        }

        public virtual void Remove(int Position)
        {
            var RemoveItem = default(ConnectedListItem<ItemType, SourceType>);

            RemoveItem = List[Position];
            RemoveItem.BeforeRemove();
            List.Remove(Position);
            RemoveItem.AfterRemove();
        }

        public ConnectedListItem<ItemType, SourceType> FindLinkTo(ItemType ItemToFind)
        {
            foreach ( var Link in List )
            {
                if ( Link.Item == ItemToFind )
                {
                    return Link;
                }
            }
            return default(ConnectedListItem<ItemType, SourceType>);
        }

        public void Deallocate()
        {
            Clear();
            Source = null;
        }

        public SimpleList<ItemType> GetItemsAsSimpleList()
        {
            var Result = new SimpleList<ItemType>();

            var ConnectedItem = default(ItemType);
            foreach ( var tempLoopVar_ConnectedItem in this )
            {
                ConnectedItem = tempLoopVar_ConnectedItem;
                Result.Add(ConnectedItem);
            }

            return Result;
        }

        public SimpleClassList<ItemType> GetItemsAsSimpleClassList()
        {
            var Result = new SimpleClassList<ItemType>();

            var ConnectedItem = default(ItemType);
            foreach ( var tempLoopVar_ConnectedItem in this )
            {
                ConnectedItem = tempLoopVar_ConnectedItem;
                Result.Add(ConnectedItem);
            }

            return Result;
        }

        public void Clear()
        {
            while ( List.Count > 0 )
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
            private const int startPosition = -1;
            private readonly ConnectedList<ItemType, SourceType> list;
            private int position = startPosition;

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
                position = startPosition;
            }
        }

        public class EnumeratorType : IEnumerator<ItemType>
        {
            private const int startPosition = -1;
            private readonly ConnectedList<ItemType, SourceType> list;
            private int position = startPosition;

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
                position = startPosition;
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