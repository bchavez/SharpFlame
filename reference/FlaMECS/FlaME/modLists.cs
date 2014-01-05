namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [StandardModule]
    public sealed class modLists
    {
        public static ConnectedList<ItemType, SourceType> MonoWorkaroundConnectedList<ItemType, SourceType>(object value) where ItemType: class where SourceType: class
        {
            return (ConnectedList<ItemType, SourceType>) value;
        }

        public static SimpleList<ItemType> MonoWorkaroundSimpleList<ItemType>(object value)
        {
            return (SimpleList<ItemType>) value;
        }

        public class ConnectedList<ItemType, SourceType> : IEnumerable<ItemType> where ItemType: class where SourceType: class
        {
            private modLists.ConnectedListItemList<ItemType, SourceType> List;
            private SourceType Source;

            public ConnectedList(SourceType Owner)
            {
                this.List = new modLists.ConnectedListItemList<ItemType, SourceType>();
                this.Source = Owner;
                this.List.AddNothingAction = modLists.SimpleClassList_AddNothingAction.DisallowError;
            }

            public virtual void Add(modLists.ConnectedListItem<ItemType, SourceType> NewItem)
            {
                if (NewItem.CanAdd())
                {
                    NewItem.BeforeAdd(modLists.MonoWorkaroundConnectedList<ItemType, SourceType>(this), this.List.Count);
                    this.List.Add(NewItem);
                }
            }

            public void Clear()
            {
                while (this.List.Count > 0)
                {
                    this.Remove(0);
                }
            }

            public void Deallocate()
            {
                this.Clear();
                this.Source = default(SourceType);
            }

            public modLists.ConnectedListItem<ItemType, SourceType> FindLinkTo(ItemType ItemToFind)
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = this.List.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        modLists.ConnectedListItem<ItemType, SourceType> current = (modLists.ConnectedListItem<ItemType, SourceType>) enumerator.Current;
                        if (current.Item == ItemToFind)
                        {
                            return current;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                return null;
            }

            public IEnumerator GetEnumerator()
            {
                return new Enumerator<ItemType, SourceType>((modLists.ConnectedList<ItemType, SourceType>) this);
            }

            public IEnumerator<ItemType> GetEnumeratorType()
            {
                return new EnumeratorType<ItemType, SourceType>(ref list);
            }

            public modLists.SimpleClassList<ItemType> GetItemsAsSimpleClassList()
            {
                IEnumerator enumerator;
                modLists.SimpleClassList<ItemType> list2 = new modLists.SimpleClassList<ItemType>();
                try
                {
                    enumerator = this.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ItemType newItem = Conversions.ToGenericParameter<ItemType>(RuntimeHelpers.GetObjectValue(enumerator.Current));
                        list2.Add(newItem);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                return list2;
            }

            public modLists.SimpleList<ItemType> GetItemsAsSimpleList()
            {
                IEnumerator enumerator;
                modLists.SimpleList<ItemType> list2 = new modLists.SimpleList<ItemType>();
                try
                {
                    enumerator = this.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ItemType newItem = Conversions.ToGenericParameter<ItemType>(RuntimeHelpers.GetObjectValue(enumerator.Current));
                        list2.Add(newItem);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                return list2;
            }

            public virtual void Insert(modLists.ConnectedListItem<ItemType, SourceType> NewItem, int Position)
            {
                if (NewItem.CanAdd())
                {
                    NewItem.BeforeAdd(modLists.MonoWorkaroundConnectedList<ItemType, SourceType>(this), Position);
                    this.List.Insert(NewItem, Position);
                }
            }

            public virtual void Remove(int Position)
            {
                modLists.ConnectedListItem<ItemType, SourceType> item = this.List[Position];
                item.BeforeRemove();
                this.List.Remove(Position);
                item.AfterRemove();
            }

            public int Count
            {
                get
                {
                    return this.List.Count;
                }
            }

            public bool IsBusy
            {
                get
                {
                    return this.List.Busy;
                }
            }

            public ItemType this[int Position]
            {
                get
                {
                    return this.List[Position].Item;
                }
            }

            public modLists.ConnectedListItem<ItemType, SourceType> this[int Position]
            {
                get
                {
                    return this.List[Position];
                }
            }

            public bool MaintainOrder
            {
                get
                {
                    return this.List.MaintainOrder;
                }
                set
                {
                    this.List.MaintainOrder = value;
                }
            }

            public SourceType Owner
            {
                get
                {
                    return this.Source;
                }
            }

            public class Enumerator : IEnumerator
            {
                private modLists.ConnectedList<ItemType, SourceType> list;
                private int position;
                private const int startPosition = -1;

                public Enumerator(modLists.ConnectedList<ItemType, SourceType> list)
                {
                    this.position = -1;
                    this.list = list;
                }

                public bool MoveNext()
                {
                    this.position++;
                    return (this.position < this.list.Count);
                }

                public void Reset()
                {
                    this.position = -1;
                }

                public object Current
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }

                [__DynamicallyInvokable]
                public object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }
            }

            public class EnumeratorType : IEnumerator<ItemType>
            {
                private bool disposedValue;
                private modLists.ConnectedList<ItemType, SourceType> list;
                private int position;
                private const int startPosition = -1;

                public EnumeratorType(ref modLists.ConnectedList<ItemType, SourceType> list)
                {
                    this.position = -1;
                    this.list = list;
                }

                public void Dispose()
                {
                    this.Dispose(true);
                    GC.SuppressFinalize(this);
                }

                protected virtual void Dispose(bool disposing)
                {
                    if (!this.disposedValue)
                    {
                    }
                    this.disposedValue = true;
                }

                public bool MoveNext()
                {
                    this.position++;
                    return (this.position < this.list.Count);
                }

                public void Reset()
                {
                    this.position = -1;
                }

                public ItemType Current
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }

                public object Current1
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }

                [__DynamicallyInvokable]
                public object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }
            }
        }

        public abstract class ConnectedListItem<ItemType, SourceType> where ItemType: class where SourceType: class
        {
            protected ConnectedListItem()
            {
            }

            public abstract void AfterMove(int NewPosition);
            public abstract void AfterRemove();
            public abstract void BeforeAdd(modLists.ConnectedList<ItemType, SourceType> NewList, int NewPosition);
            public abstract void BeforeRemove();
            public abstract bool CanAdd();
            public abstract void Disconnect();

            public abstract ItemType Item { get; }

            public abstract SourceType Source { get; }
        }

        private class ConnectedListItemList<ItemType, SourceType> : modLists.SimpleClassList<modLists.ConnectedListItem<ItemType, SourceType>> where ItemType: class where SourceType: class
        {
            protected override void AfterMoveAction(int position)
            {
                this[position].AfterMove(position);
            }
        }

        public class ConnectedListLink<ItemType, SourceType> : modLists.ConnectedListItem<ItemType, SourceType> where ItemType: class where SourceType: class
        {
            private modLists.ConnectedList<ItemType, SourceType> ConnectedList;
            private ItemType Owner;
            private int Position;

            public ConnectedListLink(ItemType Owner)
            {
                this.Position = -1;
                this.Owner = Owner;
            }

            public override void AfterMove(int Position)
            {
                this.Position = Position;
            }

            public override void AfterRemove()
            {
            }

            public override void BeforeAdd(modLists.ConnectedList<ItemType, SourceType> NewList, int NewPosition)
            {
                this.ConnectedList = NewList;
                this.Position = NewPosition;
            }

            public override void BeforeRemove()
            {
                this.ConnectedList = null;
                this.Position = -1;
            }

            public override bool CanAdd()
            {
                return !this.IsConnected;
            }

            public void Connect(modLists.ConnectedList<ItemType, SourceType> List)
            {
                if (this.IsConnected)
                {
                    Debugger.Break();
                }
                else
                {
                    List.Add(this);
                }
            }

            public void ConnectInsert(modLists.ConnectedList<ItemType, SourceType> List, int Position)
            {
                if (this.IsConnected)
                {
                    Debugger.Break();
                }
                else
                {
                    List.Insert(this, Position);
                }
            }

            public void Deallocate()
            {
                if (this.IsConnected)
                {
                    this.Disconnect();
                }
                this.Owner = default(ItemType);
            }

            public override void Disconnect()
            {
                if (this.ConnectedList == null)
                {
                    Debugger.Break();
                }
                else
                {
                    this.ConnectedList.Remove(this.Position);
                }
            }

            public int ArrayPosition
            {
                get
                {
                    return this.Position;
                }
            }

            public bool IsConnected
            {
                get
                {
                    return (this.Position >= 0);
                }
            }

            public override ItemType Item
            {
                get
                {
                    return this.Owner;
                }
            }

            public modLists.ConnectedList<ItemType, SourceType> ParentList
            {
                get
                {
                    return this.ConnectedList;
                }
            }

            public override SourceType Source
            {
                get
                {
                    if (this.IsConnected)
                    {
                        return this.ConnectedList.Owner;
                    }
                    return default(SourceType);
                }
            }
        }

        public class SimpleClassList<ItemType> : modLists.SimpleList<ItemType> where ItemType: class
        {
            public modLists.SimpleClassList_AddNothingAction AddNothingAction;

            public SimpleClassList()
            {
                this.AddNothingAction = modLists.SimpleClassList_AddNothingAction.Allow;
            }

            public override void Add(ItemType NewItem)
            {
                switch (this.AddNothingAction)
                {
                    case modLists.SimpleClassList_AddNothingAction.Allow:
                        base.Add(NewItem);
                        break;

                    case modLists.SimpleClassList_AddNothingAction.DisallowIgnore:
                        if (NewItem != null)
                        {
                            base.Add(NewItem);
                        }
                        break;

                    case modLists.SimpleClassList_AddNothingAction.DisallowError:
                        if (NewItem != null)
                        {
                            base.Add(NewItem);
                            break;
                        }
                        Debugger.Break();
                        break;

                    default:
                        Debugger.Break();
                        break;
                }
            }

            public int FindFirstItemPosition(ItemType ItemToFind)
            {
                int num3 = this.Count - 1;
                for (int i = 0; i <= num3; i++)
                {
                    if (this[i] == ItemToFind)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public enum SimpleClassList_AddNothingAction : byte
        {
            Allow = 0,
            DisallowError = 2,
            DisallowIgnore = 1
        }

        public class SimpleList<ItemType> : IEnumerable<ItemType>
        {
            private bool IsBusy;
            private int ItemCount;
            private ItemType[] Items;
            public bool MaintainOrder;
            public int MinSize;

            public SimpleList()
            {
                this.Items = new ItemType[1];
                this.ItemCount = 0;
                this.IsBusy = false;
                this.MaintainOrder = false;
                this.MinSize = 1;
            }

            public virtual void Add(ItemType NewItem)
            {
                if (this.IsBusy)
                {
                    Debugger.Break();
                }
                else
                {
                    this.IsBusy = true;
                    int itemCount = this.ItemCount;
                    if (Information.UBound(this.Items, 1) < this.ItemCount)
                    {
                        this.Items = (ItemType[]) Utils.CopyArray((Array) this.Items, new ItemType[((this.ItemCount * 2) + 1) + 1]);
                    }
                    this.Items[itemCount] = NewItem;
                    this.ItemCount++;
                    this.IsBusy = false;
                }
            }

            public virtual void AddList(IList<ItemType> NewItems)
            {
                if (this.IsBusy)
                {
                    Debugger.Break();
                }
                else
                {
                    this.IsBusy = true;
                    int num2 = this.ItemCount + NewItems.Count;
                    if ((Information.UBound(this.Items, 1) + 1) < num2)
                    {
                        this.Items = (ItemType[]) Utils.CopyArray((Array) this.Items, new ItemType[((num2 * 2) - 1) + 1]);
                    }
                    int num3 = NewItems.Count - 1;
                    for (int i = 0; i <= num3; i++)
                    {
                        this.Items[this.ItemCount + i] = NewItems[i];
                    }
                    this.ItemCount = num2;
                    this.IsBusy = false;
                }
            }

            public virtual void AddSimpleList(modLists.SimpleList<ItemType> NewItems)
            {
                if (this.IsBusy)
                {
                    Debugger.Break();
                }
                else
                {
                    this.IsBusy = true;
                    int num2 = this.ItemCount + NewItems.Count;
                    if ((Information.UBound(this.Items, 1) + 1) < num2)
                    {
                        this.Items = (ItemType[]) Utils.CopyArray((Array) this.Items, new ItemType[((num2 * 2) - 1) + 1]);
                    }
                    int num3 = NewItems.Count - 1;
                    for (int i = 0; i <= num3; i++)
                    {
                        this.Items[this.ItemCount + i] = NewItems[i];
                    }
                    this.ItemCount = num2;
                    this.IsBusy = false;
                }
            }

            protected virtual void AfterMoveAction(int position)
            {
            }

            public void Clear()
            {
                if ((Information.UBound(this.Items, 1) + 1) != this.MinSize)
                {
                    this.Items = new ItemType[(this.MinSize - 1) + 1];
                }
                this.ItemCount = 0;
            }

            public void Deallocate()
            {
                this.Clear();
                this.Items = null;
                this.IsBusy = true;
            }

            public IEnumerator GetEnumerator()
            {
                return new Enumerator<ItemType>((modLists.SimpleList<ItemType>) this);
            }

            public IEnumerator<ItemType> GetEnumeratorType()
            {
                return new EnumeratorType<ItemType>(ref list);
            }

            public void Insert(ItemType NewItem, int Position)
            {
                if (this.IsBusy)
                {
                    Debugger.Break();
                }
                else if ((Position < 0) | (Position > this.ItemCount))
                {
                    Debugger.Break();
                }
                else
                {
                    this.IsBusy = true;
                    if (Information.UBound(this.Items, 1) < this.ItemCount)
                    {
                        this.Items = (ItemType[]) Utils.CopyArray((Array) this.Items, new ItemType[((this.ItemCount * 2) + 1) + 1]);
                    }
                    int itemCount = this.ItemCount;
                    this.ItemCount++;
                    if (this.MaintainOrder)
                    {
                        int num4 = Position;
                        for (int i = itemCount - 1; i >= num4; i += -1)
                        {
                            int index = i + 1;
                            this.Items[index] = this.Items[i];
                            this.AfterMoveAction(index);
                        }
                    }
                    else
                    {
                        this.Items[itemCount] = this.Items[Position];
                        this.AfterMoveAction(itemCount);
                    }
                    this.Items[Position] = NewItem;
                    this.IsBusy = false;
                }
            }

            public void PerformTool(modLists.SimpleListTool<ItemType> Tool)
            {
                if (this.IsBusy)
                {
                    Debugger.Break();
                }
                else
                {
                    this.IsBusy = true;
                    int num2 = this.ItemCount - 1;
                    for (int i = 0; i <= num2; i++)
                    {
                        Tool.SetItem(this.Items[i]);
                        Tool.ActionPerform();
                    }
                    this.IsBusy = false;
                }
            }

            public void Remove(int Position)
            {
                if (this.IsBusy)
                {
                    Debugger.Break();
                }
                else if ((Position < 0) | (Position >= this.ItemCount))
                {
                    Debugger.Break();
                }
                else
                {
                    this.IsBusy = true;
                    this.ItemCount--;
                    if (this.MaintainOrder)
                    {
                        int itemCount = this.ItemCount;
                        for (int i = Position + 1; i <= itemCount; i++)
                        {
                            int index = i - 1;
                            this.Items[index] = this.Items[i];
                            this.AfterMoveAction(index);
                        }
                    }
                    else if (Position < this.ItemCount)
                    {
                        this.Items[Position] = this.Items[this.ItemCount];
                        this.AfterMoveAction(Position);
                    }
                    this.Items[this.ItemCount] = default(ItemType);
                    int num = Information.UBound(this.Items, 1) + 1;
                    if (((this.ItemCount * 3) < num) & (num > this.MinSize))
                    {
                        this.Items = (ItemType[]) Utils.CopyArray((Array) this.Items, new ItemType[(Math.Max(this.ItemCount * 2, this.MinSize) - 1) + 1]);
                    }
                    this.IsBusy = false;
                }
            }

            public void RemoveBuffer()
            {
                this.Items = (ItemType[]) Utils.CopyArray((Array) this.Items, new ItemType[(this.ItemCount - 1) + 1]);
            }

            public void SendItemsShuffled(modLists.SimpleList<ItemType> OtherList, Random NumberGenerator)
            {
                int num;
                modLists.SimpleList<ItemType> list = new modLists.SimpleList<ItemType>();
                int num3 = this.ItemCount - 1;
                for (num = 0; num <= num3; num++)
                {
                    list.Add(this[num]);
                }
                int num4 = this.ItemCount - 1;
                for (num = 0; num <= num4; num++)
                {
                    int position = Math.Min((int) Math.Round(Conversion.Int((double) (NumberGenerator.NextDouble() * list.Count))), list.Count - 1);
                    OtherList.Add(list[position]);
                    list.Remove(position);
                }
            }

            public void Swap(int SwapPositionA, int SwapPositionB)
            {
                if (this.IsBusy)
                {
                    Debugger.Break();
                }
                else if (SwapPositionA == SwapPositionB)
                {
                    Debugger.Break();
                }
                else if ((SwapPositionA < 0) | (SwapPositionA >= this.ItemCount))
                {
                    Debugger.Break();
                }
                else if ((SwapPositionB < 0) | (SwapPositionB >= this.ItemCount))
                {
                    Debugger.Break();
                }
                else
                {
                    this.IsBusy = true;
                    ItemType local = this.Items[SwapPositionA];
                    this.Items[SwapPositionA] = this.Items[SwapPositionB];
                    this.Items[SwapPositionB] = local;
                    this.AfterMoveAction(SwapPositionA);
                    this.AfterMoveAction(SwapPositionB);
                    this.IsBusy = false;
                }
            }

            public ItemType[] ToArray()
            {
                ItemType[] localArray = new ItemType[(this.ItemCount - 1) + 1];
                int num2 = this.ItemCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    localArray[i] = this.Items[i];
                }
                return localArray;
            }

            public bool Busy
            {
                get
                {
                    return this.IsBusy;
                }
            }

            public int Count
            {
                get
                {
                    return this.ItemCount;
                }
            }

            public ItemType this[int number]
            {
                get
                {
                    if ((number < 0) | (number >= this.ItemCount))
                    {
                        Debugger.Break();
                        return default(ItemType);
                    }
                    return this.Items[number];
                }
                set
                {
                    if ((number < 0) | (number >= this.ItemCount))
                    {
                        Debugger.Break();
                    }
                    else
                    {
                        this.Items[number] = value;
                    }
                }
            }

            private class Enumerator : IEnumerator
            {
                private modLists.SimpleList<ItemType> list;
                private int position;
                private const int startPosition = -1;

                public Enumerator(modLists.SimpleList<ItemType> list)
                {
                    this.position = -1;
                    this.list = list;
                }

                public bool MoveNext()
                {
                    this.position++;
                    return (this.position < this.list.Count);
                }

                public void Reset()
                {
                    this.position = -1;
                }

                public object Current
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }

                [__DynamicallyInvokable]
                public object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }
            }

            private class EnumeratorType : IEnumerator<ItemType>
            {
                private bool disposedValue;
                private modLists.SimpleList<ItemType> list;
                private int position;
                private const int startPosition = -1;

                public EnumeratorType(ref modLists.SimpleList<ItemType> list)
                {
                    this.position = -1;
                    this.list = list;
                }

                public void Dispose()
                {
                    this.Dispose(true);
                    GC.SuppressFinalize(this);
                }

                protected virtual void Dispose(bool disposing)
                {
                    if (!this.disposedValue)
                    {
                    }
                    this.disposedValue = true;
                }

                public bool MoveNext()
                {
                    this.position++;
                    return (this.position < this.list.Count);
                }

                public void Reset()
                {
                    this.position = -1;
                }

                public ItemType Current
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }

                public object Current1
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }

                [__DynamicallyInvokable]
                public object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        return this.list[this.position];
                    }
                }
            }
        }

        public interface SimpleListTool<ItemType>
        {
            void ActionPerform();
            void SetItem(ItemType Item);
        }
    }
}

