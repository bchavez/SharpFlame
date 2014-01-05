Public Class PathfinderConnection

    Public Destroyed As Boolean

    Public Layer_ConnectionNum As Integer = -1

    Public NodeA As PathfinderNode
    Public ReadOnly Property GetNodeA As PathfinderNode
        Get
            Return NodeA
        End Get
    End Property
    Public NodeA_ConnectionNum As Integer = -1
    Public ReadOnly Property GetNodeA_ConnectionNum As Integer
        Get
            Return NodeA_ConnectionNum
        End Get
    End Property

    Public NodeB As PathfinderNode
    Public ReadOnly Property GetNodeB As PathfinderNode
        Get
            Return NodeB
        End Get
    End Property
    Public NodeB_ConnectionNum As Integer = -1
    Public ReadOnly Property GetNodeB_ConnectionNum As Integer
        Get
            Return NodeB_ConnectionNum
        End Get
    End Property

    Private DependantConnection As PathfinderConnection 'the one above this that partially relies on this to exist

    Private LinkCount As Integer

    Public Value As Single = 1.0F
    Public ReadOnly Property GetValue As Single
        Get
            Return Value
        End Get
    End Property
    Public CalcValueNum As Integer = -1

    Public Sub New(NewNodeA As PathfinderNode, NewNodeB As PathfinderNode, NewValue As Single)

        If NewNodeA.Layer.Network_LayerNum > 0 Or NewNodeB.Layer.Network_LayerNum > 0 Or NewValue <= 0.0F Then
            Stop
            Exit Sub
        End If

        Value = NewValue

        LinkCount = 1

        NodeA = NewNodeA
        NodeB = NewNodeB
        NodeA.Connection_Add(Me, NodeA_ConnectionNum)
        NodeB.Connection_Add(Me, NodeB_ConnectionNum)

        NodeA.Layer.Connection_Add(Me)

        RaiseDependant()
    End Sub

    Public Sub New(SourceConnection As PathfinderConnection)

        NodeA = SourceConnection.NodeA.ParentNode
        NodeB = SourceConnection.NodeB.ParentNode
        NodeA.Connection_Add(Me, NodeA_ConnectionNum)
        NodeB.Connection_Add(Me, NodeB_ConnectionNum)

        NodeA.Layer.Connection_Add(Me)
        ValueCalc()
    End Sub

    Private Sub RemoveFromNodes()

        NodeA.Connection_Remove(NodeA_ConnectionNum)
        NodeA = Nothing
        NodeA_ConnectionNum = -1

        NodeB.Connection_Remove(NodeB_ConnectionNum)
        NodeB = Nothing
        NodeB_ConnectionNum = -1
    End Sub

    Public Function GetOtherNode(Self As PathfinderNode) As PathfinderNode

        If NodeA Is Self Then
            Return NodeB
        Else
            Return NodeA
        End If
    End Function

    Private Sub LinkIncrease()

        LinkCount += 1
    End Sub

    Private Sub LinkDecrease()

        LinkCount -= 1
        If LinkCount = 0 Then
            Destroy()
        ElseIf LinkCount < 0 Then
            Stop
        End If
    End Sub

    Public Sub RaiseDependant()
        Dim tmpConnectionA As PathfinderConnection

        If DependantConnection IsNot Nothing Then
            Exit Sub
        End If

        If NodeA.ParentNode IsNot NodeB.ParentNode Then
            If NodeA.ParentNode IsNot Nothing And NodeB.ParentNode IsNot Nothing Then
                tmpConnectionA = NodeA.ParentNode.FindConnection(NodeB.ParentNode)
                If tmpConnectionA Is Nothing Then
                    DependantConnection = New PathfinderConnection(Me)
                    DependantConnection.LinkIncrease()
                    DependantConnection.RaiseDependant()
                Else
                    DependantConnection = tmpConnectionA
                    DependantConnection.LinkIncrease()
                End If
            End If
        End If
    End Sub

    Public Sub Destroy()

        If Destroyed Then
            Exit Sub
        End If
        Destroyed = True

        Dim Layer As PathfinderLayer = NodeA.Layer

        Dim tmpA As PathfinderNode = NodeA.ParentNode
        Dim tmpB As PathfinderNode = NodeB.ParentNode
        RemoveFromNodes()
        If tmpA IsNot Nothing Then
            tmpA.CheckIntegrity()
        End If
        If tmpB IsNot Nothing And tmpB IsNot tmpA Then
            tmpB.CheckIntegrity()
        End If
        UnlinkParentDependants()
        Layer.Connection_Remove(Layer_ConnectionNum)
    End Sub

    Public Sub ForceDeallocate()

        DependantConnection = Nothing
        NodeA = Nothing
        NodeB = Nothing
    End Sub

    Public Sub UnlinkParentDependants()

        If DependantConnection IsNot Nothing Then
            Dim tmpConnection As PathfinderConnection = DependantConnection
            DependantConnection = Nothing
            tmpConnection.LinkDecrease()
        End If
    End Sub

    Public Sub ValueCalc()

        If NodeA.Layer.Network_LayerNum = 0 Then
            Stop
        End If

        Dim Args As New PathfinderNetwork.sFloodForValuesArgs
        Dim A As Integer
        Dim NumA As Integer
        Dim Map As PathfinderNetwork = NodeA.Layer.Network
        Dim TotalValue As Single
        Args.NodeValues = NodeA.Layer.Network.NetworkLargeArrays.Nodes_ValuesA
        Args.FinishIsParent = False
        Args.SourceNodes = NodeA.Layer.Network.NetworkLargeArrays.Nodes_Nodes
        Args.SourceParentNodeA = NodeA
        Args.SourceParentNodeB = NodeB
        Args.CurrentPath = NodeA.Layer.Network.NetworkLargeArrays.Nodes_Path
        Args.FinishNodeCount = NodeB.NodeCount
        ReDim Args.FinishNodes(NodeB.NodeCount - 1)
        For A = 0 To NodeB.NodeCount - 1
            Args.FinishNodes(A) = NodeB.Nodes(A)
        Next
        For NumA = 0 To NodeA.NodeCount - 1
            Args.CurrentPath.Nodes(0) = NodeA.Nodes(NumA)
            Args.CurrentPath.NodeCount = 1
            For A = 0 To NodeA.NodeCount - 1
                Args.NodeValues(NodeA.Nodes(A).Layer_NodeNum) = Single.MaxValue
            Next
            For A = 0 To NodeB.NodeCount - 1
                Args.NodeValues(NodeB.Nodes(A).Layer_NodeNum) = Single.MaxValue
            Next
            ReDim Args.BestPaths(Args.FinishNodeCount - 1)
            NodeA.Layer.Network.FloodForValues(Args)
            For A = 0 To NodeB.NodeCount - 1
                If Args.BestPaths(A) Is Nothing Then
                    Stop
                    Exit Sub
                End If
                TotalValue += Args.BestPaths(A).Value
            Next
        Next
        Value = TotalValue / (NodeA.NodeCount * NodeB.NodeCount)
        If Value = 0.0F Then
            Stop
        End If

        CalcValueNum = -1
    End Sub
End Class