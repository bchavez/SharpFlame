Public Class PathfinderLayer

    Public Network As PathfinderNetwork
    Public ReadOnly Property GetNetwork As PathfinderNetwork
        Get
            Return Network
        End Get
    End Property
    Public Network_LayerNum As Integer = -1
    Public ReadOnly Property GetNetwork_LayerNum As Integer
        Get
            Return Network_LayerNum
        End Get
    End Property
    Public ParentLayer As PathfinderLayer
    Public ReadOnly Property GetParentLayer As PathfinderLayer
        Get
            Return ParentLayer
        End Get
    End Property

    Public Nodes(-1) As PathfinderNode
    Public ReadOnly Property GetNode(Num As Integer) As PathfinderNode
        Get
            Return Nodes(Num)
        End Get
    End Property
    Public NodeCount As Integer
    Public ReadOnly Property GetNodeCount As Integer
        Get
            Return NodeCount
        End Get
    End Property

    Public Connections(-1) As PathfinderConnection
    Public ReadOnly Property GetConnection(Num As Integer) As PathfinderConnection
        Get
            Return Connections(Num)
        End Get
    End Property
    Public ConnectionCount As Integer
    Public ReadOnly Property GetConnectionCount As Integer
        Get
            Return ConnectionCount
        End Get
    End Property

    Public Sub Node_Add(NewNode As PathfinderNode)

        If Nodes.GetUpperBound(0) < NodeCount Then
            ReDim Preserve Nodes((NodeCount + 1) * 2 - 1)
        End If
        Nodes(NodeCount) = NewNode
        Nodes(NodeCount).Layer_NodeNum = NodeCount
        NodeCount += 1
    End Sub

    Public Sub Node_Remove(Num As Integer)

        Nodes(Num).Layer = Nothing
        Nodes(Num).Layer_NodeNum = -1

        NodeCount -= 1
        If Num < NodeCount Then
            Nodes(Num) = Nodes(NodeCount)
            Nodes(Num).Layer_NodeNum = Num
        End If
        If NodeCount * 3 < Nodes.GetUpperBound(0) Then
            ReDim Preserve Nodes(NodeCount * 2 - 1)
        End If
    End Sub

    Public Sub Connection_Add(NewConnection As PathfinderConnection)

        If Connections.GetUpperBound(0) < ConnectionCount Then
            ReDim Preserve Connections((ConnectionCount + 1) * 2 - 1)
        End If
        Connections(ConnectionCount) = NewConnection
        Connections(ConnectionCount).Layer_ConnectionNum = ConnectionCount
        ConnectionCount += 1
    End Sub

    Public Sub Connection_Remove(Num As Integer)

        Connections(Num).Layer_ConnectionNum = -1

        ConnectionCount -= 1
        If Num < ConnectionCount Then
            Connections(Num) = Connections(ConnectionCount)
            Connections(Num).Layer_ConnectionNum = Num
        End If
        If ConnectionCount * 3 < Connections.GetUpperBound(0) Then
            ReDim Preserve Connections(ConnectionCount * 2 - 1)
        End If
    End Sub

    Public Sub New(NewParentNetwork As PathfinderNetwork)

        Network = NewParentNetwork
        Network.NodeLayer_Add(Me)
    End Sub

    Public Sub ForceDeallocate()
        Dim A As Integer

        For A = 0 To NodeCount - 1
            Nodes(A).ForceDeallocate()
        Next

        Erase Nodes
        Erase Connections
        Network = Nothing
        ParentLayer = Nothing
    End Sub

    Public ChangedNodes(3) As PathfinderNode
    Public ChangedNodeCount As Integer

    Public ReadOnly Property GetChangedNode(Num As Integer) As PathfinderNode
        Get
            Return ChangedNodes(Num)
        End Get
    End Property
    Public ReadOnly Property GetChangedNodeCount As Integer
        Get
            Return ChangedNodeCount
        End Get
    End Property

    Public Sub ClearChangedNodes()

        Do While ChangedNodeCount > 0
            ChangedNode_Remove(0)
        Loop
    End Sub

    Public Sub ChangedNode_Add(NewChangedNode As PathfinderNode)

        If NewChangedNode.Layer_ChangedNodeNum >= 0 Then
            Exit Sub
        End If

        If ChangedNodes.GetUpperBound(0) < ChangedNodeCount Then
            ReDim Preserve ChangedNodes(ChangedNodeCount * 2 + 1)
        End If
        ChangedNodes(ChangedNodeCount) = NewChangedNode
        ChangedNodes(ChangedNodeCount).Layer_ChangedNodeNum = ChangedNodeCount
        ChangedNodeCount += 1
    End Sub

    Public Sub ChangedNode_Remove(Num As Integer)

        ChangedNodes(Num).Layer_ChangedNodeNum = -1
        ChangedNodes(Num) = Nothing

        ChangedNodeCount -= 1
        If Num < ChangedNodeCount Then
            ChangedNodes(Num) = ChangedNodes(ChangedNodeCount)
            ChangedNodes(Num).Layer_ChangedNodeNum = Num
        End If
        If ChangedNodeCount * 3 < ChangedNodes.GetUpperBound(0) Then
            ReDim Preserve ChangedNodes(ChangedNodeCount * 2 - 1)
        End If
    End Sub
End Class