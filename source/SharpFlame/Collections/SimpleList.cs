using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SharpFlame.Collections
{
    public class SimpleList<ItemType> : IEnumerable<ItemType>
    {
        private ItemType[] Items = new ItemType[1];
        private int ItemCount = 0;
        private bool IsBusy = false;

        public bool MaintainOrder = false;
        public int MinSize = 1;

        public bool Busy
        {
            get { return IsBusy; }
        }

        public int Count
        {
            get { return ItemCount; }
        }

        public ItemType this[int number]
        {
            get
            {
                if ( number < 0 | number >= ItemCount )
                {
                    Debugger.Break();
                    return default(ItemType);
                }
                return Items[number];
            }
            set
            {
                if ( number < 0 | number >= ItemCount )
                {
                    Debugger.Break();
                    return;
                }
                Items[number] = value;
            }
        }

        public virtual void Add(ItemType NewItem)
        {
            if ( IsBusy )
            {
                Debugger.Break();
                return;
            }

            IsBusy = true;

            int Position = ItemCount;

            if ( (Items.Length - 1) < ItemCount )
            {
                Array.Resize(ref Items, ItemCount * 2 + 1 + 1);
            }
            Items[Position] = NewItem;
            ItemCount++;

            IsBusy = false;
        }

        public virtual void AddList(IList<ItemType> NewItems)
        {
            if ( IsBusy )
            {
                Debugger.Break();
                return;
            }

            IsBusy = true;

            int ResultCount = ItemCount + NewItems.Count;
            if ( (Items.Length - 1) + 1 < ResultCount )
            {
                Array.Resize(ref Items, ResultCount * 2);
            }
            int Position = 0;
            for ( Position = 0; Position <= NewItems.Count - 1; Position++ )
            {
                Items[ItemCount + Position] = NewItems[Position];
            }
            ItemCount = ResultCount;

            IsBusy = false;
        }

        public virtual void AddSimpleList(SimpleList<ItemType> NewItems)
        {
            if ( IsBusy )
            {
                Debugger.Break();
                return;
            }

            IsBusy = true;

            int ResultCount = ItemCount + NewItems.Count;
            if ( (Items.Length - 1) + 1 < ResultCount )
            {
                Array.Resize(ref Items, ResultCount * 2);
            }
            int Position = 0;
            for ( Position = 0; Position <= NewItems.Count - 1; Position++ )
            {
                Items[ItemCount + Position] = NewItems[Position];
            }
            ItemCount = ResultCount;

            IsBusy = false;
        }

        public void Insert(ItemType NewItem, int Position)
        {
            if ( IsBusy )
            {
                Debugger.Break();
                return;
            }

            if ( Position < 0 | Position > ItemCount )
            {
                Debugger.Break();
                return;
            }

            IsBusy = true;

            if ( (Items.Length - 1) < ItemCount )
            {
                Array.Resize(ref Items, ItemCount * 2 + 1 + 1);
            }
            int LastNum = ItemCount;
            ItemCount++;
            if ( MaintainOrder )
            {
                int A = 0;
                int NewPos = 0;
                for ( A = LastNum - 1; A >= Position; A-- )
                {
                    NewPos = A + 1;
                    Items[NewPos] = Items[A];
                    AfterMoveAction(NewPos);
                }
            }
            else
            {
                Items[LastNum] = Items[Position];
                AfterMoveAction(LastNum);
            }
            Items[Position] = NewItem;

            IsBusy = false;
        }

        public void Remove(int Position)
        {
            if ( IsBusy )
            {
                Debugger.Break();
                return;
            }

            if ( Position < 0 | Position >= ItemCount )
            {
                Debugger.Break();
                return;
            }

            IsBusy = true;

            ItemCount--;
            if ( MaintainOrder )
            {
                int A = 0;
                int NewPos = 0;
                for ( A = Position + 1; A <= ItemCount; A++ )
                {
                    NewPos = A - 1;
                    Items[NewPos] = Items[A];
                    AfterMoveAction(NewPos);
                }
            }
            else
            {
                if ( Position < ItemCount )
                {
                    ItemType LastItem = Items[ItemCount];
                    Items[Position] = LastItem;
                    AfterMoveAction(Position);
                }
            }
            Items[ItemCount] = default(ItemType);
            int ArraySize = (Items.Length - 1) + 1;
            if ( ItemCount * 3 < ArraySize & ArraySize > MinSize )
            {
                Items = (ItemType[])Utils.CopyArray((Array)Items, new ItemType[Math.Max(ItemCount * 2, MinSize)]);
            }

            IsBusy = false;
        }

        public void Swap(int SwapPositionA, int SwapPositionB)
        {
            if ( IsBusy )
            {
                Debugger.Break();
                return;
            }

            if ( SwapPositionA == SwapPositionB )
            {
                Debugger.Break();
                return;
            }

            if ( SwapPositionA < 0 | SwapPositionA >= ItemCount )
            {
                Debugger.Break();
                return;
            }
            if ( SwapPositionB < 0 | SwapPositionB >= ItemCount )
            {
                Debugger.Break();
                return;
            }

            IsBusy = true;

            ItemType SwapItem = Items[SwapPositionA];
            Items[SwapPositionA] = Items[SwapPositionB];
            Items[SwapPositionB] = SwapItem;
            AfterMoveAction(SwapPositionA);
            AfterMoveAction(SwapPositionB);

            IsBusy = false;
        }

        public void Clear()
        {
            if ( (Items.Length - 1) + 1 != MinSize )
            {
                Items = new ItemType[MinSize];
            }
            ItemCount = 0;
        }

        public void Deallocate()
        {
            Clear();
            Items = null;

            IsBusy = true;
        }

        public void PerformTool(SimpleListTool<ItemType> Tool)
        {
            if ( IsBusy )
            {
                Debugger.Break();
                return;
            }

            IsBusy = true;

            int A = 0;

            for ( A = 0; A <= ItemCount - 1; A++ )
            {
                Tool.SetItem(Items[A]);
                Tool.ActionPerform();
            }

            IsBusy = false;
        }

        public void SendItemsShuffled(SimpleList<ItemType> OtherList, Random NumberGenerator)
        {
            int A = 0;
            SimpleList<ItemType> Copy = new SimpleList<ItemType>();
            int Position = 0;

            for ( A = 0; A <= ItemCount - 1; A++ )
            {
                Copy.Add(this[A]);
            }
            for ( A = 0; A <= ItemCount - 1; A++ )
            {
                Position = Math.Min((int)(Conversion.Int(NumberGenerator.NextDouble() * Copy.Count)), Copy.Count - 1);
                OtherList.Add(Copy[Position]);
                Copy.Remove(Position);
            }
        }

        public void RemoveBuffer()
        {
            Array.Resize(ref Items, ItemCount);
        }

        protected virtual void AfterMoveAction(int position)
        {
        }

        public ItemType[] ToArray()
        {
            ItemType[] result = new ItemType[ItemCount];

            for ( int i = 0; i <= ItemCount - 1; i++ )
            {
                result[i] = Items[i];
            }

            return result;
        }

        public IEnumerator<ItemType> GetEnumerator()
        {
            return GetEnumeratorType();
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
        //				return new Enumerator(MonoWorkaroundSimpleList<ItemType>(this));
        //#endif
        //}

        private class Enumerator : IEnumerator
        {
            private SimpleList<ItemType> list;
            private const int startPosition = -1;
            private int position = startPosition;

            public Enumerator(SimpleList<ItemType> list)
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

        private class EnumeratorType : IEnumerator<ItemType>
        {
            private SimpleList<ItemType> list;
            private const int startPosition = -1;
            private int position = startPosition;

            public EnumeratorType(SimpleList<ItemType> list)
            {
                this.list = list;
            }

            public ItemType Current
            {
                get { return list[position]; }
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

            #endregion
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}