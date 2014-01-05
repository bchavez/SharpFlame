using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SharpFlame
{
    public sealed class modLists
    {
        public static SimpleList<ItemType> MonoWorkaroundSimpleList<ItemType>(object value)
        {
            return ((SimpleList<ItemType>)value);
        }

        public static ConnectedList<ItemType, SourceType> MonoWorkaroundConnectedList<ItemType, SourceType>(object value) where ItemType : class
            where SourceType : class
        {
            return ((ConnectedList<ItemType, SourceType>)value);
        }

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

        public enum SimpleClassList_AddNothingAction
        {
            Allow,
            DisallowIgnore,
            DisallowError
        }

        public class SimpleClassList<ItemType> : SimpleList<ItemType> where ItemType : class
        {
            public SimpleClassList_AddNothingAction AddNothingAction = SimpleClassList_AddNothingAction.Allow;

            public override void Add(ItemType NewItem)
            {
                switch ( AddNothingAction )
                {
                    case SimpleClassList_AddNothingAction.Allow:
                        base.Add(NewItem);
                        break;
                    case SimpleClassList_AddNothingAction.DisallowIgnore:
                        if ( NewItem != null )
                        {
                            base.Add(NewItem);
                        }
                        break;
                    case SimpleClassList_AddNothingAction.DisallowError:
                        if ( NewItem == null )
                        {
                            Debugger.Break();
                        }
                        else
                        {
                            base.Add(NewItem);
                        }
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }

            public int FindFirstItemPosition(ItemType ItemToFind)
            {
                int Position = 0;

                for ( Position = 0; Position <= Count - 1; Position++ )
                {
                    if ( this[Position] == ItemToFind )
                    {
                        return Position;
                    }
                }
                return -1;
            }
        }

        public interface SimpleListTool<ItemType>
        {
            void SetItem(ItemType Item);
            void ActionPerform();
        }

        private class ConnectedListItemList<ItemType, SourceType> : SimpleClassList<ConnectedListItem<ItemType, SourceType>> where ItemType : class
            where SourceType : class
        {
            protected override void AfterMoveAction(int position)
            {
                this[position].AfterMove(position);
            }
        }

        public class ConnectedList<ItemType, SourceType> : IEnumerable<ItemType> where ItemType : class where SourceType : class
        {
            private ConnectedListItemList<ItemType, SourceType> List = new ConnectedListItemList<ItemType, SourceType>();
            private SourceType Source;

            public ConnectedList(SourceType Owner)
            {
                Source = Owner;
                List.AddNothingAction = SimpleClassList_AddNothingAction.DisallowError;
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

            public bool IsBusy
            {
                get { return List.Busy; }
            }

            public ConnectedListItem<ItemType, SourceType> get_ItemContainer(int Position)
            {
                return List[Position];
            }

            public ItemType this[int Position]
            {
                get { return List[Position].Item; }
            }

            public int Count
            {
                get { return List.Count; }
            }

            public virtual void Add(ConnectedListItem<ItemType, SourceType> NewItem)
            {
                if ( NewItem.CanAdd() )
                {
                    NewItem.BeforeAdd(MonoWorkaroundConnectedList<ItemType, SourceType>(this), List.Count);
                    List.Add(NewItem);
                }
            }

            public virtual void Insert(ConnectedListItem<ItemType, SourceType> NewItem, int Position)
            {
                if ( NewItem.CanAdd() )
                {
                    NewItem.BeforeAdd(MonoWorkaroundConnectedList<ItemType, SourceType>(this), Position);
                    List.Insert(NewItem, Position);
                }
            }

            public virtual void Remove(int Position)
            {
                ConnectedListItem<ItemType, SourceType> RemoveItem = default(ConnectedListItem<ItemType, SourceType>);

                RemoveItem = List[Position];
                RemoveItem.BeforeRemove();
                List.Remove(Position);
                RemoveItem.AfterRemove();
            }

            public ConnectedListItem<ItemType, SourceType> FindLinkTo(ItemType ItemToFind)
            {
                foreach ( ConnectedListItem<ItemType, SourceType> Link in List )
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
                SimpleList<ItemType> Result = new SimpleList<ItemType>();

                ItemType ConnectedItem = default(ItemType);
                foreach ( ItemType tempLoopVar_ConnectedItem in this )
                {
                    ConnectedItem = tempLoopVar_ConnectedItem;
                    Result.Add(ConnectedItem);
                }

                return Result;
            }

            public SimpleClassList<ItemType> GetItemsAsSimpleClassList()
            {
                SimpleClassList<ItemType> Result = new SimpleClassList<ItemType>();

                ItemType ConnectedItem = default(ItemType);
                foreach ( ItemType tempLoopVar_ConnectedItem in this )
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
            //							return new Enumerator(modLists.MonoWorkaroundConnectedList<ItemType, SourceType>(this));
            //#endif
            //}

            public class Enumerator : IEnumerator
            {
                private ConnectedList<ItemType, SourceType> list;
                private const int startPosition = -1;
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
                private ConnectedList<ItemType, SourceType> list;
                private const int startPosition = -1;
                private int position = startPosition;

                public EnumeratorType(ConnectedList<ItemType, SourceType> list)
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

        public abstract class ConnectedListItem<ItemType, SourceType> where ItemType : class where SourceType : class
        {
            public abstract ItemType Item { get; }
            public abstract SourceType Source { get; }
            public abstract bool CanAdd();
            public abstract void BeforeAdd(ConnectedList<ItemType, SourceType> NewList, int NewPosition);
            public abstract void BeforeRemove();
            public abstract void AfterRemove();
            public abstract void AfterMove(int NewPosition);
            public abstract void Disconnect();
        }

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

#if false
								public class ConnectedListsConnection<SourceTypeA, SourceTypeB> where SourceTypeA : class where SourceTypeB : class
								{
									
									private class Link<SourceType> : ConnectedListLink<ConnectedListsConnection<SourceTypeA, SourceTypeB>, SourceType> where SourceType : class
									{
										
										public Link(ConnectedListsConnection<SourceTypeA, SourceTypeB> Owner) : base(Owner)
										{
											
										}
										
										public override void AfterRemove()
										{
											base.AfterRemove();
											
											Item.Deallocate();
										}
									}
									
									private Link<SourceTypeA> _LinkA; 
									private Link<SourceTypeB> _LinkB; 
									
public SourceTypeA ItemA
									{
										get
										{
											return _LinkA.Source;
										}
									}
									
public SourceTypeB ItemB
									{
										get
										{
											return _LinkB.Source;
										}
									}
									
									public static ConnectedListsConnection<SourceTypeA, SourceTypeB> Create(ConnectedList<ConnectedListsConnection<SourceTypeA, SourceTypeB>, SourceTypeA> ListA, ConnectedList<ConnectedListsConnection<SourceTypeA, SourceTypeB>, SourceTypeB> ListB)
									{
										
										if (ListA == null)
										{
											return default(ConnectedListsConnection<SourceTypeA, SourceTypeB>);
										}
										if (ListB == null)
										{
											return default(ConnectedListsConnection<SourceTypeA, SourceTypeB>);
										}
										if (ListA.IsBusy)
										{
											return default(ConnectedListsConnection<SourceTypeA, SourceTypeB>);
										}
										if (ListB.IsBusy)
										{
											return default(ConnectedListsConnection<SourceTypeA, SourceTypeB>);
										}
										
										ConnectedListsConnection<SourceTypeA, SourceTypeB> Result = new ConnectedListsConnection<SourceTypeA, SourceTypeB>();
										Result._LinkA.Connect(ListA);
										Result._LinkB.Connect(ListB);
										return Result;
									}
									
									protected ConnectedListsConnection()
									{
										
										_LinkA = new Link<SourceTypeA>(this);
										_LinkB = new Link<SourceTypeB>(this);
										
										
										
									}
									
									private bool Deallocating = false;
									
									public void Deallocate()
									{
										
										Deallocating = true;
										
										_LinkA.Deallocate();
										_LinkB.Deallocate();
										AfterDeallocate();
									}
									
									protected virtual void AfterDeallocate()
									{
										
										
									}
								}
#endif
    }
}