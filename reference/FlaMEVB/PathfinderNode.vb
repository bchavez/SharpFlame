Public Class PathfinderNode

    Public Tag As Object

    Public Network_FindParentNum As Integer = -1

    Public Layer As PathfinderLayer
    Public ReadOnly Property GetLayer As PathfinderLayer
        Get
            Return Layer
        End Get
    End Property
    Public Layer_NodeNum As Integer = -1
    Public ReadOnly Property GetLayer_NodeNum As Integer
        Get
            Return Layer_NodeNum
        End Get
    End Property

    Public Layer_ChangedNodeNum As Integer = -1

    Public ParentNode As PathfinderNode
    Public ReadOnly Property GetParentNode As PathfinderNode
        Get
            Return ParentNode
        End Get
    End Property
    Public ParentNode_NodeNum As Integer = -1
    Public ReadOnly Property GetParentNode_NodeNum As Integer
        Get
            Return ParentNode_NodeNum
        End Get
    End Property

    Public Nodes(3) As PathfinderNode
    Public ReadOnly Property GetChildNode(Num As Integer) As PathfinderNode
        Get
            Return Nodes(Num)
        End Get
    End Property
    Public NodeCount As Integer
    Public ReadOnly Property GetChildNodeCount As Integer
        Get
            Return NodeCount
        End Get
    End Property

    Public Connections(1) As PathfinderConnection
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

    Public SiblingSpan As Single
    Public ReadOnly Property GetSiblingSpan As Single
        Get
            Return SiblingSpan
        End Get
    End Property
    Public ChildrenSpan As Single
    Public ReadOnly Property GetChildrenSpan As Single
        Get
            Return ChildrenSpan
        End Get
    End Property

    Public Clearance As Integer = Integer.MaxValue
    Public ReadOnly Property GetClearance As Integer
        Get
            Return Clearance
        End Get
    End Property

    Public Sub Node_Add(NodeToAdd As PathfinderNode)

        If Layer Is Nothing Then
            Stop
            Exit Sub
        End If

        If NodeToAdd.Layer.Network_LayerNum <> Layer.Network_LayerNum - 1 Then
            Stop
            Exit Sub
        End If

        If NodeToAdd.ParentNode IsNot Nothing Then
            Stop
            Exit Sub
        End If

        If Layer_ChangedNodeNum < 0 Then
            Layer.ChangedNode_Add(Me)
        End If

        NodeToAdd.ParentNode = Me
        NodeToAdd.ParentNode_NodeNum = NodeCount

        Nodes(NodeCount) = NodeToAdd
        NodeCount += 1

        If NodeToAdd.ConnectionCount = 0 Then
            Stop
        End If

        If NodeToAdd.Clearance > Clearance Then
            ClearanceSet(Clearance)
        Else
            ClearanceCalc()
        End If
    End Sub

    Public Sub RaiseConnections()
        Dim A As Integer

        For A = 0 To ConnectionCount - 1
            Connections(A).RaiseDependant()
        Next
    End Sub

    Public Function FindConnection(NodeToFind As PathfinderNode) As PathfinderConnection
        Dim A As Integer
        Dim tmpConnection As PathfinderConnection

        For A = 0 To ConnectionCount - 1
            tmpConnection = Connections(A)
            If tmpConnection.GetOtherNode(Me) Is NodeToFind Then
                Return tmpConnection
            End If
        Next
        Return Nothing
    End Function

    Public Sub Node_Remove(Num As Integer)
        Dim tmpNodeA As PathfinderNode = Nodes(Num)
        Dim tmpConnection As PathfinderConnection
        Dim A As Integer

        If tmpNodeA.ParentNode_NodeNum <> Num Or tmpNodeA.ParentNode IsNot Me Then
            Stop
            Exit Sub
        End If

        If Layer_ChangedNodeNum < 0 Then
            Layer.ChangedNode_Add(Me)
        End If

        tmpNodeA.ParentNode = Nothing
        tmpNodeA.ParentNode_NodeNum = -1

        NodeCount -= 1
        If Num < NodeCount Then
            Nodes(Num) = Nodes(NodeCount)
            Nodes(Num).ParentNode_NodeNum = Num
        End If
        Nodes(NodeCount) = Nothing

        For A = 0 To tmpNodeA.ConnectionCount - 1
            tmpConnection = tmpNodeA.Connections(A)
            tmpConnection.UnlinkParentDependants()
        Next

        If NodeCount = 0 And ConnectionCount > 0 Then
            Stop
        End If

        ClearanceCalc()
    End Sub

    Public Structure sVisited
        Public Visited() As Boolean
    End Structure

    Public Sub FloodCheckInternal(CurrentNode As PathfinderNode, ByRef Visited As sVisited)
        Dim A As Integer
        Dim tmpNode As PathfinderNode
        Dim tmpConnection As PathfinderConnection

        Visited.Visited(CurrentNode.ParentNode_NodeNum) = True

        For A = 0 To CurrentNode.ConnectionCount - 1
            tmpConnection = CurrentNode.Connections(A)
            tmpNode = tmpConnection.GetOtherNode(CurrentNode)
            If tmpNode.ParentNode Is Me Then
                If Not Visited.Visited(tmpNode.ParentNode_NodeNum) Then
                    FloodCheckInternal(tmpNode, Visited)
                End If
            End If
        Next
    End Sub

    Public Sub Deallocate()

        If Network_FindParentNum >= 0 Then
            Layer.Network.FindParentNode_Remove(Network_FindParentNum)
        End If
        If Layer_ChangedNodeNum >= 0 Then
            Layer.ChangedNode_Remove(Layer_ChangedNodeNum)
        End If
        If ConnectionCount > 0 Then
            Stop
        End If
        If NodeCount > 0 Then
            Stop
        End If
        If ParentNode IsNot Nothing Then
            Stop
        End If
        Layer.Node_Remove(Layer_NodeNum)
    End Sub

    Public Sub ForceDeallocate()
        Dim A As Integer

        For A = 0 To ConnectionCount - 1
            Connections(A).ForceDeallocate()
        Next
        Erase Connections
        Erase Nodes
        ParentNode = Nothing
        Layer = Nothing
    End Sub

    Public Sub FindParent()
        Dim tmpNodeA As PathfinderNode
        Dim BestScore As Single
        Dim BestNode As PathfinderNode = Nothing
        Dim Score As Single
        Dim A As Integer
        Dim MakeNew As Boolean
        Dim B As Integer
        Dim Count As Integer
        Dim C As Integer
        Dim Allow As Boolean
        Dim tmpConnection As PathfinderConnection
        Dim DestNode As PathfinderNode

        If NodeCount = 0 And Layer.Network_LayerNum > 0 Then
            Stop
            Exit Sub
        End If

        If ParentNode IsNot Nothing Then
            Stop
            Exit Sub
        End If

        BestScore = Single.MaxValue
        For A = 0 To ConnectionCount - 1
            tmpConnection = Connections(A)
            DestNode = tmpConnection.GetOtherNode(Me)
            tmpNodeA = DestNode.ParentNode
            If tmpNodeA Is Nothing Then
                tmpNodeA = tmpConnection.GetOtherNode(Me)
                Score = tmpConnection.Value * (0.98F + Rnd() * 0.04F)
                If Score < BestScore Then
                    BestScore = Score
                    BestNode = tmpNodeA
                    MakeNew = True
                End If
            Else
                'dont allow this to join to another when the other has 3 nodes and they only have one connection
                If tmpNodeA.NodeCount = 3 Then
                    Count = 0
                    Allow = False
                    For B = 0 To tmpNodeA.NodeCount - 1
                        For C = 0 To tmpNodeA.Nodes(B).ConnectionCount - 1
                            If tmpNodeA.Nodes(B).Connections(C).GetOtherNode(tmpNodeA.Nodes(B)) Is Me Then
                                Count += 1
                                If Count >= 2 Then
                                    Allow = True
                                    GoTo CountFinished
                                End If
                                Exit For
                            End If
                        Next
                    Next
CountFinished:
                Else
                    Allow = True
                End If
                If Allow Then
                    Score = (DestNode.SiblingSpan + tmpConnection.Value) * (0.98F + Rnd() * 0.04F)
                    If Score < BestScore Then
                        BestScore = Score
                        BestNode = tmpNodeA
                        MakeNew = False
                    End If
                End If
            End If
        Next
        If BestNode IsNot Nothing Then
            If MakeNew Then
                Dim tmpLayer As PathfinderLayer
                If Layer.ParentLayer Is Nothing Then
                    tmpLayer = New PathfinderLayer(Layer.Network)
                Else
                    tmpLayer = Layer.ParentLayer
                End If
                Dim NewNode As New PathfinderNode(tmpLayer)
                NewNode.Node_Add(Me)
                NewNode.Node_Add(BestNode)
                NewNode.SpanCalc()
                RaiseConnections()
                BestNode.RaiseConnections()
                NewNode.Layer.Network.FindParentNode_Add(NewNode)
            Else
                If BestNode IsNot Nothing Then
                    BestNode.Node_Add(Me)
                    If BestNode.NodeCount >= 4 Then
                        BestNode.Split()
                    Else
                        BestNode.SpanCalc()
                        RaiseConnections()
                        If BestNode.ParentNode Is Nothing Then
                            BestNode.Layer.Network.FindParentNode_Add(BestNode)
                        End If
                    End If
                End If
            End If
        ElseIf ConnectionCount > 0 Then
            'it is part of a network but there is no suitable parent to join, so make a new isolated parent
            Dim tmpLayer As PathfinderLayer
            If Layer.ParentLayer Is Nothing Then
                tmpLayer = New PathfinderLayer(Layer.Network)
            Else
                tmpLayer = Layer.ParentLayer
            End If
            Dim NewNode As New PathfinderNode(tmpLayer)
            NewNode.Node_Add(Me)
            NewNode.SpanCalc()
            RaiseConnections()
            NewNode.Layer.Network.FindParentNode_Add(NewNode)
        End If
    End Sub

    Public Sub Split()

        If NodeCount <> 4 Then
            Stop
        End If

        Dim Value As Single
        Dim BestValue As Single
        Dim BestNodeA As PathfinderNode = Nothing
        Dim BestNodeB As PathfinderNode = Nothing
        Dim BestNodeC As PathfinderNode = Nothing
        Dim BestNodeD As PathfinderNode = Nothing
        Dim A As Integer
        Dim B As Integer
        Dim tmpNodeA As PathfinderNode
        Dim tmpNodeB As PathfinderNode
        Dim tmpNodeC As PathfinderNode
        Dim tmpNodeD As PathfinderNode
        Dim tmpConnectionA As PathfinderConnection = Nothing
        Dim tmpConnectionB As PathfinderConnection = Nothing
        Dim C As Integer
        Dim D As Integer

        Dim Children(NodeCount - 1) As PathfinderNode
        For A = 0 To NodeCount - 1
            Children(A) = Nodes(A)
        Next
        Dim ChildCount As Integer = NodeCount
        Dim ThisLayer As PathfinderLayer = Layer

        Disband()

        BestValue = Single.MaxValue
        For A = 0 To ChildCount - 1
            tmpNodeA = Children(A)
            For B = A + 1 To ChildCount - 1
                tmpNodeB = Children(B)
                For C = 0 To ChildCount - 1
                    If Children(C) IsNot tmpNodeA And Children(C) IsNot tmpNodeB Then
                        Exit For
                    End If
                Next
                tmpNodeC = Children(C)
                For D = C + 1 To ChildCount - 1
                    If Children(D) IsNot tmpNodeA And Children(D) IsNot tmpNodeB Then
                        Exit For
                    End If
                Next
                tmpNodeD = Children(D)
                For C = 0 To tmpNodeA.ConnectionCount - 1
                    tmpConnectionA = tmpNodeA.Connections(C)
                    If tmpConnectionA.GetOtherNode(tmpNodeA) Is tmpNodeB Then
                        Exit For
                    End If
                Next
                For D = 0 To tmpNodeC.ConnectionCount - 1
                    tmpConnectionB = tmpNodeC.Connections(D)
                    If tmpConnectionB.GetOtherNode(tmpNodeC) Is tmpNodeD Then
                        Exit For
                    End If
                Next
                If C < tmpNodeA.ConnectionCount And D < tmpNodeC.ConnectionCount Then
                    Value = tmpConnectionA.Value + tmpConnectionB.Value
                    If Value < BestValue Then
                        BestValue = Value
                        BestNodeA = tmpNodeA
                        BestNodeB = tmpNodeB
                        BestNodeC = tmpNodeC
                        BestNodeD = tmpNodeD
                    End If
                End If
            Next
        Next

        If BestNodeA IsNot Nothing Then

            If ParentNode IsNot Nothing Then
                tmpNodeA = ParentNode
                tmpNodeA.Node_Remove(ParentNode_NodeNum)
            Else
                tmpNodeA = Nothing
            End If
            If tmpNodeA IsNot Nothing Then
                tmpNodeA.CheckIntegrity()
            End If

            Dim NewNodeA As New PathfinderNode(ThisLayer)
            Dim NewNodeB As New PathfinderNode(ThisLayer)

            NewNodeA.Node_Add(BestNodeA)
            NewNodeA.Node_Add(BestNodeB)

            NewNodeA.SpanCalc()
            BestNodeA.RaiseConnections()
            BestNodeB.RaiseConnections()

            NewNodeA.Layer.Network.FindParentNode_Add(NewNodeA)

            NewNodeB.Node_Add(BestNodeC)
            NewNodeB.Node_Add(BestNodeD)

            NewNodeB.SpanCalc()
            BestNodeC.RaiseConnections()
            BestNodeD.RaiseConnections()

            NewNodeB.Layer.Network.FindParentNode_Add(NewNodeB)
        Else
            Stop
        End If
    End Sub

    Public Sub CheckIntegrity()
        'make sure im still a good parent

        If NodeCount >= 2 Then
            Dim Visited As sVisited
            ReDim Visited.Visited(NodeCount - 1)
            Dim A As Integer
            FloodCheckInternal(Nodes(0), Visited)
            For A = 0 To NodeCount - 1
                If Not Visited.Visited(A) Then
                    GoTo DisbandAndFind
                End If
            Next
        End If

        If NodeCount = 1 And ConnectionCount = 0 Then
            GoTo DisbandAndFind
        ElseIf NodeCount > 1 Then
            SpanCalc()
        ElseIf NodeCount = 0 Then
            If ParentNode IsNot Nothing Then
                Dim tmpNode As PathfinderNode = ParentNode
                tmpNode.Node_Remove(ParentNode_NodeNum)
                tmpNode.CheckIntegrity()
            End If
            If Layer.Network_LayerNum > 0 Then
                Deallocate()
            End If
        End If

        Exit Sub
DisbandAndFind:
        Dim B As Integer
        Dim Children(NodeCount - 1) As PathfinderNode
        For B = 0 To NodeCount - 1
            Children(B) = Nodes(B)
        Next
        Dim ChildCount As Integer = NodeCount

        Disband()

        For B = 0 To ChildCount - 1
            Children(B).Layer.Network.FindParentNode_Add(Children(B))
        Next
    End Sub

    Public Sub New(ParentNetwork As PathfinderNetwork)
        Dim tmpLayer As PathfinderLayer

        If ParentNetwork.NodeLayerCount = 0 Then
            tmpLayer = New PathfinderLayer(ParentNetwork)
        Else
            tmpLayer = ParentNetwork.NodeLayers(0)
        End If

        Layer = tmpLayer
        tmpLayer.Node_Add(Me)
    End Sub

    Public Sub New(NewParentLayer As PathfinderLayer)

        Layer = NewParentLayer
        Layer.Node_Add(Me)
    End Sub

    Public Sub Disband()
        Dim tmpNode As PathfinderNode

        tmpNode = ParentNode
        If tmpNode IsNot Nothing Then
            tmpNode.Node_Remove(ParentNode_NodeNum)
            tmpNode.CheckIntegrity()
        End If

        Do While NodeCount > 0
            Node_Remove(0)
        Loop

        Deallocate()
    End Sub

    Public Function CreateConnection(OtherNode As PathfinderNode, Value As Single) As PathfinderConnection
        Dim tmpConnection As PathfinderConnection

        If OtherNode.Layer IsNot Layer Then
            Return Nothing
        End If

        tmpConnection = FindConnection(OtherNode)
        If tmpConnection Is Nothing Then
            Return New PathfinderConnection(Me, OtherNode, Value)
        End If
        Return Nothing
    End Function

    Public Function GetOrCreateConnection(OtherNode As PathfinderNode, Value As Single) As PathfinderConnection
        Dim tmpConnection As PathfinderConnection

        If OtherNode.Layer IsNot Layer Then
            Return Nothing
        End If

        tmpConnection = FindConnection(OtherNode)
        If tmpConnection Is Nothing Then
            Return New PathfinderConnection(Me, OtherNode, Value)
        End If
        Return tmpConnection
    End Function

    Public Sub Connection_Add(Connection As PathfinderConnection, ByRef OutputNum As Integer)

        OutputNum = ConnectionCount

        If Connections.GetUpperBound(0) < ConnectionCount Then
            ReDim Preserve Connections(ConnectionCount * 2 + 1)
        End If
        Connections(ConnectionCount) = Connection
        ConnectionCount += 1

        If ParentNode Is Nothing Then
            Layer.Network.FindParentNode_Add(Me)
        End If
    End Sub

    Public Sub Connection_Remove(Num As Integer)

        ConnectionCount -= 1
        If Num < ConnectionCount Then
            Connections(Num) = Connections(ConnectionCount)
            If Connections(Num).NodeA Is Me Then
                Connections(Num).NodeA_ConnectionNum = Num
            ElseIf Connections(Num).NodeB Is Me Then
                Connections(Num).NodeB_ConnectionNum = Num
            Else
                Stop
            End If
        End If
        If Connections.GetUpperBound(0) + 1 > ConnectionCount * 3 Then
            ReDim Preserve Connections(ConnectionCount * 2 + 1)
        End If
    End Sub

    Public Sub ClearanceSet(Value As Integer)

        If Value = Clearance Then
            Exit Sub
        End If

        Clearance = Value

        If ParentNode IsNot Nothing Then
            If Clearance > ParentNode.Clearance Then
                ParentNode.ClearanceSet(Clearance)
            ElseIf Clearance < ParentNode.Clearance Then
                ParentNode.ClearanceCalc()
            End If
        End If
    End Sub

    Public Sub ClearanceCalc()
        Dim A As Integer

        If Layer.Network_LayerNum = 0 Then
            Stop
        End If

        Clearance = 0
        For A = 0 To NodeCount - 1
            If Nodes(A).Clearance > Clearance Then
                Clearance = Nodes(A).Clearance
            End If
        Next

        If ParentNode IsNot Nothing Then
            If Clearance > ParentNode.Clearance Then
                ParentNode.ClearanceSet(Clearance)
            ElseIf Clearance < ParentNode.Clearance Then
                ParentNode.ClearanceCalc()
            End If
        End If
    End Sub

    Public Sub SpanCalc()
        Dim Args As New PathfinderNetwork.sFloodSpanArgs
        Dim A As Integer
        Dim NumA As Integer

        Args.NodeValues = Layer.Network.NetworkLargeArrays.Nodes_ValuesA
        Args.FinishIsParent = False
        Args.SourceNodes = Layer.Network.NetworkLargeArrays.Nodes_Nodes
        Args.SourceParentNode = Me
        Args.CurrentPath = Layer.Network.NetworkLargeArrays.Nodes_Path
        Args.FinishNodeCount = NodeCount
        ReDim Args.FinishNodes(NodeCount - 1)
        For A = 0 To NodeCount - 1
            Args.FinishNodes(A) = Nodes(A)
        Next
        ChildrenSpan = 0.0F
        For A = 0 To NodeCount - 1
            Nodes(A).SiblingSpan = 0.0F
        Next
        For NumA = 0 To NodeCount - 1
            Args.CurrentPath.Nodes(0) = Nodes(NumA)
            Args.CurrentPath.NodeCount = 1
            For A = 0 To NodeCount - 1
                Args.NodeValues(Nodes(A).Layer_NodeNum) = Single.MaxValue
            Next
            ReDim Args.BestPaths(Args.FinishNodeCount - 1)
            Layer.Network.FloodSpan(Args)
            For A = NumA + 1 To NodeCount - 1
                If Args.BestPaths(A) Is Nothing Then
                    Stop
                    Exit Sub
                End If
                If Args.BestPaths(A).Value > ChildrenSpan Then
                    ChildrenSpan = Args.BestPaths(A).Value
                End If
                If Args.BestPaths(A).Value > Nodes(NumA).SiblingSpan Then
                    Nodes(NumA).SiblingSpan = Args.BestPaths(A).Value
                End If
                If Args.BestPaths(A).Value > Nodes(A).SiblingSpan Then
                    Nodes(A).SiblingSpan = Args.BestPaths(A).Value
                End If
            Next
        Next
    End Sub
End Class