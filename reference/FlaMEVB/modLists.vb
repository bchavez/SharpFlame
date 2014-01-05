
Public Module modLists

    Public Function MonoWorkaroundSimpleList(Of ItemType)(value As Object) As SimpleList(Of ItemType)

        Return CType(value, SimpleList(Of ItemType))
    End Function

    Public Function MonoWorkaroundConnectedList(Of ItemType As Class, SourceType As Class)(value As Object) As ConnectedList(Of ItemType, SourceType)

        Return CType(value, ConnectedList(Of ItemType, SourceType))
    End Function

    Public Class SimpleList(Of ItemType)
        Implements Collections.Generic.IEnumerable(Of ItemType)

        Private Items(0) As ItemType
        Private ItemCount As Integer = 0
        Private IsBusy As Boolean = False

        Public MaintainOrder As Boolean = False
        Public MinSize As Integer = 1

        Public ReadOnly Property Busy As Boolean
            Get
                Return IsBusy
            End Get
        End Property

        Public ReadOnly Property Count As Integer
            Get
                Return ItemCount
            End Get
        End Property

        Default Public Property Item(number As Integer) As ItemType
            Get
                If number < 0 Or number >= ItemCount Then
                    Stop
                    Return Nothing
                End If
                Return Items(number)
            End Get
            Set(value As ItemType)
                If number < 0 Or number >= ItemCount Then
                    Stop
                    Exit Property
                End If
                Items(number) = value
            End Set
        End Property

        Public Overridable Sub Add(NewItem As ItemType)

            If IsBusy Then
                Stop
                Exit Sub
            End If

            IsBusy = True

            Dim Position As Integer = ItemCount

            If UBound(Items) < ItemCount Then
                ReDim Preserve Items(ItemCount * 2 + 1)
            End If
            Items(Position) = NewItem
            ItemCount += 1

            IsBusy = False
        End Sub

        Public Overridable Sub AddList(NewItems As Collections.Generic.IList(Of ItemType))

            If IsBusy Then
                Stop
                Exit Sub
            End If

            IsBusy = True

            Dim ResultCount As Integer = ItemCount + NewItems.Count
            If UBound(Items) + 1 < ResultCount Then
                ReDim Preserve Items(ResultCount * 2 - 1)
            End If
            Dim Position As Integer
            For Position = 0 To NewItems.Count - 1
                Items(ItemCount + Position) = NewItems(Position)
            Next
            ItemCount = ResultCount

            IsBusy = False
        End Sub

        Public Overridable Sub AddSimpleList(NewItems As SimpleList(Of ItemType))

            If IsBusy Then
                Stop
                Exit Sub
            End If

            IsBusy = True

            Dim ResultCount As Integer = ItemCount + NewItems.Count
            If UBound(Items) + 1 < ResultCount Then
                ReDim Preserve Items(ResultCount * 2 - 1)
            End If
            Dim Position As Integer
            For Position = 0 To NewItems.Count - 1
                Items(ItemCount + Position) = NewItems(Position)
            Next
            ItemCount = ResultCount

            IsBusy = False
        End Sub

        Public Sub Insert(NewItem As ItemType, Position As Integer)

            If IsBusy Then
                Stop
                Exit Sub
            End If

            If Position < 0 Or Position > ItemCount Then
                Stop
                Exit Sub
            End If

            IsBusy = True

            If UBound(Items) < ItemCount Then
                ReDim Preserve Items(ItemCount * 2 + 1)
            End If
            Dim LastNum As Integer = ItemCount
            ItemCount += 1
            If MaintainOrder Then
                Dim A As Integer
                Dim NewPos As Integer
                For A = LastNum - 1 To Position Step -1
                    NewPos = A + 1
                    Items(NewPos) = Items(A)
                    AfterMoveAction(NewPos)
                Next
            Else
                Items(LastNum) = Items(Position)
                AfterMoveAction(LastNum)
            End If
            Items(Position) = NewItem

            IsBusy = False
        End Sub

        Public Sub Remove(Position As Integer)

            If IsBusy Then
                Stop
                Exit Sub
            End If

            If Position < 0 Or Position >= ItemCount Then
                Stop
                Exit Sub
            End If

            IsBusy = True

            ItemCount -= 1
            If MaintainOrder Then
                Dim A As Integer
                Dim NewPos As Integer
                For A = Position + 1 To ItemCount
                    NewPos = A - 1
                    Items(NewPos) = Items(A)
                    AfterMoveAction(NewPos)
                Next
            Else
                If Position < ItemCount Then
                    Dim LastItem As ItemType = Items(ItemCount)
                    Items(Position) = LastItem
                    AfterMoveAction(Position)
                End If
            End If
            Items(ItemCount) = Nothing
            Dim ArraySize As Integer = UBound(Items) + 1
            If ItemCount * 3 < ArraySize And ArraySize > MinSize Then
                ReDim Preserve Items(Math.Max(ItemCount * 2, MinSize) - 1)
            End If

            IsBusy = False
        End Sub

        Public Sub Swap(SwapPositionA As Integer, SwapPositionB As Integer)

            If IsBusy Then
                Stop
                Exit Sub
            End If

            If SwapPositionA = SwapPositionB Then
                Stop
                Exit Sub
            End If

            If SwapPositionA < 0 Or SwapPositionA >= ItemCount Then
                Stop
                Exit Sub
            End If
            If SwapPositionB < 0 Or SwapPositionB >= ItemCount Then
                Stop
                Exit Sub
            End If

            IsBusy = True

            Dim SwapItem As ItemType = Items(SwapPositionA)
            Items(SwapPositionA) = Items(SwapPositionB)
            Items(SwapPositionB) = SwapItem
            AfterMoveAction(SwapPositionA)
            AfterMoveAction(SwapPositionB)

            IsBusy = False
        End Sub

        Public Sub Clear()

            If UBound(Items) + 1 <> MinSize Then
                ReDim Items(MinSize - 1)
            End If
            ItemCount = 0
        End Sub

        Public Sub Deallocate()

            Clear()
            Items = Nothing

            IsBusy = True
        End Sub

        Public Sub PerformTool(Tool As SimpleListTool(Of ItemType))

            If IsBusy Then
                Stop
                Exit Sub
            End If

            IsBusy = True

            Dim A As Integer

            For A = 0 To ItemCount - 1
                Tool.SetItem(Items(A))
                Tool.ActionPerform()
            Next

            IsBusy = False
        End Sub

        Public Sub SendItemsShuffled(OtherList As SimpleList(Of ItemType), NumberGenerator As Random)
            Dim A As Integer
            Dim Copy As New SimpleList(Of ItemType)
            Dim Position As Integer

            For A = 0 To ItemCount - 1
                Copy.Add(Item(A))
            Next
            For A = 0 To ItemCount - 1
                Position = Math.Min(CInt(Int(NumberGenerator.NextDouble * Copy.Count)), Copy.Count - 1)
                OtherList.Add(Copy(Position))
                Copy.Remove(Position)
            Next
        End Sub

        Public Sub RemoveBuffer()

            ReDim Preserve Items(ItemCount - 1)
        End Sub

        Protected Overridable Sub AfterMoveAction(position As Integer)


        End Sub

        Public Function ToArray() As ItemType()
            Dim result(ItemCount - 1) As ItemType

            For i As Integer = 0 To ItemCount - 1
                result(i) = Items(i)
            Next

            Return result
        End Function

        Public Function GetEnumeratorType() As System.Collections.Generic.IEnumerator(Of ItemType) Implements System.Collections.Generic.IEnumerable(Of ItemType).GetEnumerator

            Return New EnumeratorType(Me)
        End Function

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

#If Not Mono Then
            Return New Enumerator(Me)
#Else
            Return New Enumerator(MonoWorkaroundSimpleList(Of ItemType)(Me))
#End If
        End Function

        Private Class Enumerator
            Implements Collections.IEnumerator

            Private list As SimpleList(Of ItemType)
            Private Const startPosition As Integer = -1
            Private position As Integer = startPosition

            Public Sub New(list As SimpleList(Of ItemType))

                Me.list = list
            End Sub

            Public ReadOnly Property Current As Object Implements System.Collections.IEnumerator.Current
                Get
                    Return list(position)
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext

                position += 1
                Return position < list.Count
            End Function

            Public Sub Reset() Implements System.Collections.IEnumerator.Reset

                position = startPosition
            End Sub
        End Class

        Private Class EnumeratorType
            Implements Collections.Generic.IEnumerator(Of ItemType)

            Private list As SimpleList(Of ItemType)
            Private Const startPosition As Integer = -1
            Private position As Integer = startPosition

            Public Sub New(ByRef list As SimpleList(Of ItemType))

                Me.list = list
            End Sub

            Public ReadOnly Property Current As ItemType Implements System.Collections.Generic.IEnumerator(Of ItemType).Current
                Get
                    Return list(position)
                End Get
            End Property

            Public ReadOnly Property Current1 As Object Implements System.Collections.IEnumerator.Current
                Get
                    Return list(position)
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext

                position += 1
                Return position < list.Count
            End Function

            Public Sub Reset() Implements System.Collections.IEnumerator.Reset

                position = startPosition
            End Sub

#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                End If
                Me.disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
            'Protected Overrides Sub Finalize()
            '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
            '    Dispose(False)
            '    MyBase.Finalize()
            'End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region

        End Class

    End Class

    Public Enum SimpleClassList_AddNothingAction As Byte
        Allow
        DisallowIgnore
        DisallowError
    End Enum

    Public Class SimpleClassList(Of ItemType As Class)
        Inherits SimpleList(Of ItemType)

        Public AddNothingAction As SimpleClassList_AddNothingAction = SimpleClassList_AddNothingAction.Allow

        Public Overrides Sub Add(NewItem As ItemType)

            Select Case AddNothingAction
                Case SimpleClassList_AddNothingAction.Allow
                    MyBase.Add(NewItem)
                Case SimpleClassList_AddNothingAction.DisallowIgnore
                    If NewItem IsNot Nothing Then
                        MyBase.Add(NewItem)
                    End If
                Case SimpleClassList_AddNothingAction.DisallowError
                    If NewItem Is Nothing Then
                        Stop
                    Else
                        MyBase.Add(NewItem)
                    End If
                Case Else
                    Stop
            End Select
        End Sub

        Public Function FindFirstItemPosition(ItemToFind As ItemType) As Integer
            Dim Position As Integer

            For Position = 0 To Count - 1
                If Item(Position) Is ItemToFind Then
                    Return Position
                End If
            Next
            Return -1
        End Function
    End Class

    Public Interface SimpleListTool(Of ItemType)

        Sub SetItem(Item As ItemType)
        Sub ActionPerform()
    End Interface

    Private Class ConnectedListItemList(Of ItemType As Class, SourceType As Class)
        Inherits SimpleClassList(Of ConnectedListItem(Of ItemType, SourceType))

        Protected Overrides Sub AfterMoveAction(position As Integer)

            Item(position).AfterMove(position)
        End Sub
    End Class

    Public Class ConnectedList(Of ItemType As Class, SourceType As Class)
        Implements Collections.Generic.IEnumerable(Of ItemType)

        Private List As New ConnectedListItemList(Of ItemType, SourceType)
        Private Source As SourceType

        Public Sub New(Owner As SourceType)

            Source = Owner
            List.AddNothingAction = SimpleClassList_AddNothingAction.DisallowError
        End Sub

        Public ReadOnly Property Owner As SourceType
            Get
                Return Source
            End Get
        End Property

        Public Property MaintainOrder As Boolean
            Get
                Return List.MaintainOrder
            End Get
            Set(value As Boolean)
                List.MaintainOrder = value
            End Set
        End Property

        Public ReadOnly Property IsBusy As Boolean
            Get
                Return List.Busy
            End Get
        End Property

        Public ReadOnly Property ItemContainer(Position As Integer) As ConnectedListItem(Of ItemType, SourceType)
            Get
                Return List.Item(Position)
            End Get
        End Property

        Default Public ReadOnly Property Item(Position As Integer) As ItemType
            Get
                Return List.Item(Position).Item
            End Get
        End Property

        Public ReadOnly Property Count() As Integer
            Get
                Return List.Count
            End Get
        End Property

        Public Overridable Sub Add(NewItem As ConnectedListItem(Of ItemType, SourceType))

            If NewItem.CanAdd Then
                NewItem.BeforeAdd(MonoWorkaroundConnectedList(Of ItemType, SourceType)(Me), List.Count)
                List.Add(NewItem)
            End If
        End Sub

        Public Overridable Sub Insert(NewItem As ConnectedListItem(Of ItemType, SourceType), Position As Integer)

            If NewItem.CanAdd Then
                NewItem.BeforeAdd(MonoWorkaroundConnectedList(Of ItemType, SourceType)(Me), Position)
                List.Insert(NewItem, Position)
            End If
        End Sub

        Public Overridable Sub Remove(Position As Integer)
            Dim RemoveItem As ConnectedListItem(Of ItemType, SourceType)

            RemoveItem = List(Position)
            RemoveItem.BeforeRemove()
            List.Remove(Position)
            RemoveItem.AfterRemove()
        End Sub

        Public Function FindLinkTo(ItemToFind As ItemType) As ConnectedListItem(Of ItemType, SourceType)

            For Each Link As ConnectedListItem(Of ItemType, SourceType) In List
                If Link.Item Is ItemToFind Then
                    Return Link
                End If
            Next
            Return Nothing
        End Function

        Public Sub Deallocate()

            Clear()
            Source = Nothing
        End Sub

        Public Function GetItemsAsSimpleList() As SimpleList(Of ItemType)
            Dim Result As New SimpleList(Of ItemType)

            Dim ConnectedItem As ItemType
            For Each ConnectedItem In Me
                Result.Add(ConnectedItem)
            Next

            Return Result
        End Function

        Public Function GetItemsAsSimpleClassList() As SimpleClassList(Of ItemType)
            Dim Result As New SimpleClassList(Of ItemType)

            Dim ConnectedItem As ItemType
            For Each ConnectedItem In Me
                Result.Add(ConnectedItem)
            Next

            Return Result
        End Function

        Public Sub Clear()

            Do While List.Count > 0
                Remove(0)
            Loop
        End Sub

        Public Function GetEnumeratorType() As System.Collections.Generic.IEnumerator(Of ItemType) Implements System.Collections.Generic.IEnumerable(Of ItemType).GetEnumerator

            Return New EnumeratorType(Me)
        End Function

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator

#If Not Mono Then
            Return New Enumerator(Me)
#Else
            Return New Enumerator(MonoWorkaroundConnectedList(Of ItemType, SourceType)(Me))
#End If
        End Function

        Public Class Enumerator
            Implements Collections.IEnumerator

            Private list As ConnectedList(Of ItemType, SourceType)
            Private Const startPosition As Integer = -1
            Private position As Integer = startPosition

            Public Sub New(list As ConnectedList(Of ItemType, SourceType))

                Me.list = list
            End Sub

            Public ReadOnly Property Current As Object Implements System.Collections.IEnumerator.Current
                Get
                    Return list(position)
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext

                position += 1
                Return position < list.Count
            End Function

            Public Sub Reset() Implements System.Collections.IEnumerator.Reset

                position = startPosition
            End Sub
        End Class

        Public Class EnumeratorType
            Implements Collections.Generic.IEnumerator(Of ItemType)

            Private list As ConnectedList(Of ItemType, SourceType)
            Private Const startPosition As Integer = -1
            Private position As Integer = startPosition

            Public Sub New(ByRef list As ConnectedList(Of ItemType, SourceType))

                Me.list = list
            End Sub

            Public ReadOnly Property Current As ItemType Implements System.Collections.Generic.IEnumerator(Of ItemType).Current
                Get
                    Return list(position)
                End Get
            End Property

            Public ReadOnly Property Current1 As Object Implements System.Collections.IEnumerator.Current
                Get
                    Return list(position)
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext

                position += 1
                Return position < list.Count
            End Function

            Public Sub Reset() Implements System.Collections.IEnumerator.Reset

                position = startPosition
            End Sub

#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                End If
                Me.disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
            'Protected Overrides Sub Finalize()
            '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
            '    Dispose(False)
            '    MyBase.Finalize()
            'End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region

        End Class

    End Class

    Public MustInherit Class ConnectedListItem(Of ItemType As Class, SourceType As Class)

        MustOverride ReadOnly Property Item As ItemType
        MustOverride ReadOnly Property Source As SourceType
        MustOverride Function CanAdd() As Boolean
        MustOverride Sub BeforeAdd(NewList As ConnectedList(Of ItemType, SourceType), NewPosition As Integer)
        MustOverride Sub BeforeRemove()
        MustOverride Sub AfterRemove()
        MustOverride Sub AfterMove(NewPosition As Integer)
        MustOverride Sub Disconnect()
    End Class

    Public Class ConnectedListLink(Of ItemType As Class, SourceType As Class)
        Inherits ConnectedListItem(Of ItemType, SourceType)

        Private Owner As ItemType
        Private ConnectedList As ConnectedList(Of ItemType, SourceType)
        Private Position As Integer = -1

        Public Sub New(Owner As ItemType)

            Me.Owner = Owner
        End Sub

        Public ReadOnly Property ParentList As ConnectedList(Of ItemType, SourceType)
            Get
                Return ConnectedList
            End Get
        End Property

        Public ReadOnly Property ArrayPosition As Integer
            Get
                Return Position
            End Get
        End Property

        Public ReadOnly Property IsConnected As Boolean
            Get
                Return (Position >= 0)
            End Get
        End Property

        Public Overrides Sub AfterMove(Position As Integer)

            Me.Position = Position
        End Sub

        Public Overrides Sub BeforeRemove()

            ConnectedList = Nothing
            Position = -1
        End Sub

        Public Overrides ReadOnly Property Item As ItemType
            Get
                Return Owner
            End Get
        End Property

        Public Overrides ReadOnly Property Source As SourceType
            Get
                If IsConnected Then
                    Return ConnectedList.Owner
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Sub Connect(List As ConnectedList(Of ItemType, SourceType))

            If IsConnected Then
                Stop
                Exit Sub
            End If

            List.Add(Me)
        End Sub

        Public Sub ConnectInsert(List As ConnectedList(Of ItemType, SourceType), Position As Integer)

            If IsConnected Then
                Stop
                Exit Sub
            End If

            List.Insert(Me, Position)
        End Sub

        Public Overrides Sub Disconnect()

            If ConnectedList Is Nothing Then
                Stop
                Exit Sub
            End If

            ConnectedList.Remove(Position)
        End Sub

        Public Sub Deallocate()

            If IsConnected Then
                Disconnect()
            End If
            Owner = Nothing
        End Sub

        Public Overrides Sub AfterRemove()

        End Sub

        Public Overrides Function CanAdd() As Boolean

             Return (not IsConnected)
        End Function

        Public Overrides Sub BeforeAdd(NewList As ConnectedList(Of ItemType, SourceType), NewPosition As Integer)

            ConnectedList = NewList
            Position = NewPosition
        End Sub
    End Class

#If False Then
    Public Class ConnectedListsConnection(Of SourceTypeA As Class, SourceTypeB As Class)

        Private Class Link(Of SourceType As Class)
            Inherits ConnectedListLink(Of ConnectedListsConnection(Of SourceTypeA, SourceTypeB), SourceType)

            Public Sub New(Owner As ConnectedListsConnection(Of SourceTypeA, SourceTypeB))
                MyBase.New(Owner)

            End Sub

            Public Overrides Sub AfterRemove()
                MyBase.AfterRemove()

                Item.Deallocate()
            End Sub
        End Class

        Private _LinkA As New Link(Of SourceTypeA)(Me)
        Private _LinkB As New Link(Of SourceTypeB)(Me)

        Public ReadOnly Property ItemA As SourceTypeA
            Get
                Return _LinkA.Source
            End Get
        End Property

        Public ReadOnly Property ItemB As SourceTypeB
            Get
                Return _LinkB.Source
            End Get
        End Property

        Public Shared Function Create(ListA As ConnectedList(Of ConnectedListsConnection(Of SourceTypeA, SourceTypeB), SourceTypeA), ListB As ConnectedList(Of ConnectedListsConnection(Of SourceTypeA, SourceTypeB), SourceTypeB)) As ConnectedListsConnection(Of SourceTypeA, SourceTypeB)

            If ListA Is Nothing Then
                Return Nothing
            End If
            If ListB Is Nothing Then
                Return Nothing
            End If
            If ListA.IsBusy Then
                Return Nothing
            End If
            If ListB.IsBusy Then
                Return Nothing
            End If

            Dim Result As New ConnectedListsConnection(Of SourceTypeA, SourceTypeB)
            Result._LinkA.Connect(ListA)
            Result._LinkB.Connect(ListB)
            Return Result
        End Function

        Protected Sub New()


        End Sub

        Private Deallocating As Boolean = False

        Public Sub Deallocate()

            Deallocating = True

            _LinkA.Deallocate()
            _LinkB.Deallocate()
            AfterDeallocate()
        End Sub

        Protected Overridable Sub AfterDeallocate()


        End Sub
    End Class
#End If

End Module
