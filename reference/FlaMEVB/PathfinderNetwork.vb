Public Class PathfinderNetwork

    Public NodeLayers(-1) As PathfinderLayer
    Public ReadOnly Property GetNodeLayer(Num As Integer) As PathfinderLayer
        Get
            Return NodeLayers(Num)
        End Get
    End Property
    Public NodeLayerCount As Integer
    Public ReadOnly Property GetNodeLayerCount As Integer
        Get
            Return NodeLayerCount
        End Get
    End Property

    Public FindParentNodes(-1) As PathfinderNode
    Public FindParentNodeCount As Integer

    Public Class LargeArrays
        Public Nodes_Booleans() As Boolean
        Public Nodes_ValuesA() As Single
        Public Nodes_ValuesB() As Single
        Public Nodes_Path As New Path
        Public Nodes_Nodes() As PathfinderNode
        Public Size As Integer

        Public SizeEnlargementRatio As Single = 2.0F
        Public SizeReductionRatio As Single = 3.0F

        Public Sub Resize(NetworkForSize As PathfinderNetwork)
            Dim NewSize As Integer

            If NetworkForSize.NodeLayerCount > 0 Then
                NewSize = NetworkForSize.NodeLayers(0).NodeCount
            Else
                NewSize = 0
            End If
            If Size < NewSize Then
                Dim Num As Integer
                Size = CInt(NewSize * SizeEnlargementRatio)
                Num = Size - 1
                ReDim Nodes_Booleans(Num)
                ReDim Nodes_ValuesA(Num)
                ReDim Nodes_ValuesB(Num)
                ReDim Nodes_Booleans(Num)
                ReDim Nodes_Path.Nodes(Num)
                ReDim Nodes_Nodes(Num)
            Else
                If Size > NewSize * SizeReductionRatio Then
                    Dim Num As Integer
                    Size = CInt(NewSize * SizeEnlargementRatio)
                    Num = Size - 1
                    ReDim Nodes_Booleans(Num)
                    ReDim Nodes_ValuesA(Num)
                    ReDim Nodes_ValuesB(Num)
                    ReDim Nodes_Booleans(Num)
                    ReDim Nodes_Path.Nodes(Num)
                    ReDim Nodes_Nodes(Num)
                End If
            End If
        End Sub
    End Class
    Public NetworkLargeArrays As New LargeArrays

    Public Class Path
        Public Nodes(-1) As PathfinderNode
        Public NodeCount As Integer
        Public Value As Single
    End Class

    Public Sub NodeLayer_Add(NewNodeLayer As PathfinderLayer)

        If NodeLayerCount > 0 Then
            NodeLayers(NodeLayerCount - 1).ParentLayer = NewNodeLayer
        End If

        ReDim Preserve NodeLayers(NodeLayerCount)
        NodeLayers(NodeLayerCount) = NewNodeLayer
        NodeLayers(NodeLayerCount).Network_LayerNum = NodeLayerCount
        NodeLayerCount += 1
    End Sub

    Public Sub FindParentNode_Add(NewFindParentNode As PathfinderNode)

        If NewFindParentNode.Network_FindParentNum >= 0 Then
            Exit Sub
        End If

        If FindParentNodes.GetUpperBound(0) < FindParentNodeCount Then
            ReDim Preserve FindParentNodes((FindParentNodeCount + 1) * 2 - 1)
        End If
        FindParentNodes(FindParentNodeCount) = NewFindParentNode
        FindParentNodes(FindParentNodeCount).Network_FindParentNum = FindParentNodeCount
        FindParentNodeCount += 1
    End Sub

    Public Sub FindParentNode_Remove(Num As Integer)

        FindParentNodes(Num).Network_FindParentNum = -1
        FindParentNodes(Num) = Nothing

        FindParentNodeCount -= 1
        If Num < FindParentNodeCount Then
            FindParentNodes(Num) = FindParentNodes(FindParentNodeCount)
            FindParentNodes(Num).Network_FindParentNum = Num
        End If
        If FindParentNodeCount * 3 < FindParentNodes.GetUpperBound(0) Then
            ReDim Preserve FindParentNodes(FindParentNodeCount * 2 - 1)
        End If
    End Sub

    Public Sub Deallocate()
        Dim A As Integer

        For A = 0 To NodeLayerCount - 1
            NodeLayers(A).ForceDeallocate()
        Next

        Erase NodeLayers
        Erase FindParentNodes
    End Sub

    Public Structure PathList
        Public Paths() As Path
        Public PathCount As Integer
    End Structure

    Public Function GetPath(StartNodes() As PathfinderNode, FinishNode As PathfinderNode, Accuracy As Integer, MinClearance As Integer) As PathList()
        Dim StartNodeCount As Integer = StartNodes.GetUpperBound(0) + 1
        Dim Paths(NodeLayerCount - 1) As PathList
        Dim LayerStartNodes(NodeLayerCount - 1, StartNodeCount - 1) As PathfinderNode
        Dim LayerFinishNodes(NodeLayerCount - 1) As PathfinderNode
        Dim LayerNum As Integer
        Dim Destinations(23) As PathfinderNode
        Dim DestinationCount As Integer
        Dim FinishIsParent As Boolean
        Dim IsInaccurate As Boolean
        Dim CalcNodeCount(23) As Integer
        Dim FloodRouteArgs As sFloodRouteArgs
        Dim FinalLayer As Integer
        Dim StartCanReach(StartNodeCount - 1) As Boolean
        Dim tmpNodeA As PathfinderNode
        Dim tmpNodeB As PathfinderNode
        Dim CanReachCount As Integer
        Dim FirstLayer As Integer
        Dim BestPaths(23) As Path
        Dim BestValues(23) As Single
        Dim PathNum As Integer
        Dim StopMultiPathing As Boolean
        Dim Visit() As Boolean = NetworkLargeArrays.Nodes_Booleans
        Dim NodeValues() As Single = NetworkLargeArrays.Nodes_ValuesA
        Dim Nodes_Nodes() As PathfinderNode = NetworkLargeArrays.Nodes_Nodes
        Dim StartPath As Path = NetworkLargeArrays.Nodes_Path
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim E As Integer

        FinalLayer = StartNodes(0).Layer.Network_LayerNum
        LayerFinishNodes(FinalLayer) = FinishNode
        B = FinalLayer
        Do
            If LayerFinishNodes(B).ParentNode Is Nothing Then
                FirstLayer = B
                Exit Do
            End If
            LayerFinishNodes(B + 1) = LayerFinishNodes(B).ParentNode
            B += 1
        Loop
        For A = 0 To StartNodeCount - 1
            LayerStartNodes(FinalLayer, A) = StartNodes(A)
            B = FinalLayer
            Do
                If LayerStartNodes(B, A).ParentNode Is Nothing Then
                    If LayerStartNodes(B, A) Is LayerFinishNodes(B) Then
                        StartCanReach(A) = True
                        CanReachCount += 1
                    End If
                    Exit Do
                End If
                LayerStartNodes(B + 1, A) = LayerStartNodes(B, A).ParentNode
                B += 1
            Loop
        Next
        If CanReachCount = 0 Then
            Return Nothing
        End If
        LayerNum = FirstLayer
        ReDim Paths(LayerNum).Paths(0)
        Paths(LayerNum).Paths(0) = New Path
        Paths(LayerNum).PathCount = 1
        ReDim Paths(LayerNum).Paths(0).Nodes(0)
        Paths(LayerNum).Paths(0).Nodes(0) = LayerFinishNodes(LayerNum)
        Paths(LayerNum).Paths(0).NodeCount = 1
        Dim LastLayer As Integer
        Do
            LastLayer = LayerNum
            LayerNum -= 1
            If LayerNum < FinalLayer Then
                Exit Do
            ElseIf StopMultiPathing Then
                If Accuracy < 0 Then
                    Stop
                End If
                For PathNum = 0 To Paths(LastLayer).PathCount - 1
                    CalcNodeCount(PathNum) = Math.Min(Accuracy, Paths(LastLayer).Paths(PathNum).NodeCount - 1)
                Next
                Destinations(0) = Paths(LastLayer).Paths(0).Nodes(CalcNodeCount(0))
                DestinationCount = 1
                FinishIsParent = True
                IsInaccurate = True
            Else
                If Accuracy >= 0 Then
                    For PathNum = 0 To Paths(LastLayer).PathCount - 1
                        If Paths(LastLayer).Paths(PathNum).NodeCount > Accuracy Then
                            StopMultiPathing = True
                            Exit For
                        End If
                    Next
                End If
                Destinations(0) = LayerFinishNodes(LayerNum)
                If LayerNum = FinalLayer Then
                    DestinationCount = 1
                Else
                    For A = 0 To Destinations(0).ConnectionCount - 1
                        Destinations(1 + A) = Destinations(0).Connections(A).GetOtherNode(Destinations(0))
                    Next
                    DestinationCount = 1 + Destinations(0).ConnectionCount
                End If
                For PathNum = 0 To Paths(LastLayer).PathCount - 1
                    CalcNodeCount(PathNum) = Paths(LastLayer).Paths(PathNum).NodeCount - 1
                Next
                FinishIsParent = False
            End If
            For PathNum = 0 To Paths(LastLayer).PathCount - 1
                For A = 0 To CalcNodeCount(PathNum)
                    tmpNodeA = Paths(LastLayer).Paths(PathNum).Nodes(A)
                    For D = 0 To tmpNodeA.ConnectionCount - 1
                        tmpNodeB = tmpNodeA.Connections(D).GetOtherNode(tmpNodeA)
                        For E = 0 To tmpNodeB.ConnectionCount - 1
                            C = tmpNodeB.Connections(E).GetOtherNode(tmpNodeB).Layer_NodeNum
                            Visit(C) = False
                        Next
                    Next
                Next
            Next
            For PathNum = 0 To Paths(LastLayer).PathCount - 1
                For A = 0 To CalcNodeCount(PathNum)
                    tmpNodeA = Paths(LastLayer).Paths(PathNum).Nodes(A)
                    C = tmpNodeA.Layer_NodeNum
                    Visit(C) = True
                    For E = 0 To tmpNodeA.NodeCount - 1
                        C = tmpNodeA.Nodes(E).Layer_NodeNum
                        NodeValues(C) = Single.MaxValue
                    Next
                    For D = 0 To tmpNodeA.ConnectionCount - 1
                        tmpNodeB = tmpNodeA.Connections(D).GetOtherNode(tmpNodeA)
                        C = tmpNodeB.Layer_NodeNum
                        Visit(C) = True
                        For E = 0 To tmpNodeB.NodeCount - 1
                            C = tmpNodeB.Nodes(E).Layer_NodeNum
                            NodeValues(C) = Single.MaxValue
                        Next
                    Next
                Next
            Next
            FloodRouteArgs = New sFloodRouteArgs
            With FloodRouteArgs
                .CurrentPath = StartPath
                .FinishNodes = Destinations
                .FinishNodeCount = DestinationCount
                .FinishIsParent = FinishIsParent
                .Visit = Visit
                .NodeValues = NodeValues
                .SourceNodes = Nodes_Nodes
                .MinClearance = MinClearance
            End With
            For A = 0 To DestinationCount - 1
                BestPaths(A) = Nothing
                BestValues(A) = Single.MaxValue
            Next
            For A = 0 To StartNodeCount - 1
                If StartCanReach(A) Then
                    StartPath.NodeCount = 1
                    StartPath.Nodes(0) = LayerStartNodes(LayerNum, A)
                    StartPath.Value = 0.0F
                    ReDim FloodRouteArgs.BestPaths(DestinationCount - 1)
                    FloodRoute(FloodRouteArgs)
                    For PathNum = 0 To DestinationCount - 1
                        If FloodRouteArgs.BestPaths(PathNum) IsNot Nothing Then
                            If FloodRouteArgs.BestPaths(PathNum).Value < BestValues(PathNum) Then
                                BestValues(PathNum) = FloodRouteArgs.BestPaths(PathNum).Value
                                BestPaths(PathNum) = FloodRouteArgs.BestPaths(PathNum)
                            End If
                        End If
                    Next
                End If
            Next
            ReDim Paths(LayerNum).Paths(DestinationCount - 1)
            Paths(LayerNum).PathCount = 0
            For PathNum = 0 To DestinationCount - 1
                If BestPaths(PathNum) IsNot Nothing Then
                    Paths(LayerNum).Paths(Paths(LayerNum).PathCount) = BestPaths(PathNum)
                    Paths(LayerNum).PathCount += 1
                End If
            Next
            ReDim Preserve Paths(LayerNum).Paths(Paths(LayerNum).PathCount - 1)
            If Paths(LayerNum).PathCount = 0 Then
                Return Nothing
            End If
        Loop
        Return Paths
    End Function

    Public Function GetAllPaths(StartNodes() As PathfinderNode, FinishNode As PathfinderNode, MinClearance As Integer) As Path()
        Dim StartNodeCount As Integer = StartNodes.GetUpperBound(0) + 1
        Dim LayerStartNodes(31, StartNodeCount - 1) As PathfinderNode
        Dim LayerFinishNodes(31) As PathfinderNode
        Dim LayerNum As Integer
        Dim Destinations(23) As PathfinderNode
        Dim FloodRouteDArgs As sFloodRouteAllArgs
        Dim FinalLayer As Integer
        Dim StartCanReach(StartNodeCount - 1) As Boolean
        Dim tmpNodeA As PathfinderNode
        Dim CanReachCount As Integer
        Dim FirstLayer As Integer
        Dim SubPaths(31) As Path
        Dim Nodes_Nodes() As PathfinderNode = NetworkLargeArrays.Nodes_Nodes
        Dim Visit() As Boolean = NetworkLargeArrays.Nodes_Booleans
        Dim NodeValues() As Single = NetworkLargeArrays.Nodes_ValuesA
        Dim NodeValuesB() As Single = NetworkLargeArrays.Nodes_ValuesB
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer

        FinalLayer = StartNodes(0).Layer.Network_LayerNum
        LayerFinishNodes(FinalLayer) = FinishNode
        B = FinalLayer
        Do
            If LayerFinishNodes(B).ParentNode Is Nothing Then
                FirstLayer = B
                Exit Do
            End If
            LayerFinishNodes(B + 1) = LayerFinishNodes(B).ParentNode
            B += 1
        Loop
        For A = 0 To StartNodeCount - 1
            LayerStartNodes(FinalLayer, A) = StartNodes(A)
            B = FinalLayer
            Do
                If LayerStartNodes(B, A).ParentNode Is Nothing Then
                    If LayerStartNodes(B, A) Is LayerFinishNodes(B) Then
                        StartCanReach(A) = True
                        CanReachCount += 1
                    End If
                    Exit Do
                End If
                LayerStartNodes(B + 1, A) = LayerStartNodes(B, A).ParentNode
                B += 1
            Loop
        Next
        If CanReachCount = 0 Then
            Return Nothing
        End If
        LayerNum = FirstLayer
        SubPaths(LayerNum) = New Path
        ReDim SubPaths(LayerNum).Nodes(0)
        SubPaths(LayerNum).Nodes(0) = LayerFinishNodes(LayerNum)
        SubPaths(LayerNum).NodeCount = 1
        Dim LastLayer As Integer
        Do
            LastLayer = LayerNum
            LayerNum -= 1
            If LayerNum < FinalLayer Then
                Exit Do
            End If
            For A = 0 To SubPaths(LastLayer).NodeCount - 1
                tmpNodeA = SubPaths(LastLayer).Nodes(A)
                For B = 0 To tmpNodeA.ConnectionCount - 1
                    C = tmpNodeA.Connections(B).GetOtherNode(tmpNodeA).Layer_NodeNum
                    Visit(C) = False
                Next
            Next
            For A = 0 To SubPaths(LastLayer).NodeCount - 1
                tmpNodeA = SubPaths(LastLayer).Nodes(A)
                C = tmpNodeA.Layer_NodeNum
                Visit(C) = True
                For D = 0 To tmpNodeA.NodeCount - 1
                    C = tmpNodeA.Nodes(D).Layer_NodeNum
                    NodeValues(C) = Single.MaxValue
                    NodeValuesB(C) = Single.MaxValue
                Next
            Next
            FloodRouteDArgs = New sFloodRouteAllArgs
            With FloodRouteDArgs
                .FinishNode = LayerFinishNodes(LayerNum)
                .Visit = Visit
                .NodeValuesA = NodeValues
                .SourceNodes = Nodes_Nodes
                .NodeValuesB = NodeValuesB
                .MinClearance = MinClearance
                .BestTolerance = CSng(1.1# ^ LayerNum)
                ReDim .StartNodes(StartNodeCount - 1)
                For A = 0 To StartNodeCount - 1
                    If StartCanReach(A) Then
                        .StartNodes(.StartNodeCount) = LayerStartNodes(LayerNum, A)
                        .StartNodeCount += 1
                    End If
                Next
                ReDim Preserve .StartNodes(.StartNodeCount - 1)
            End With
            FloodRouteAll(FloodRouteDArgs)
            SubPaths(LayerNum) = New Path
            ReDim SubPaths(LayerNum).Nodes(FloodRouteDArgs.BestNodeCount - 1)
            For A = 0 To FloodRouteDArgs.BestNodeCount - 1
                SubPaths(LayerNum).Nodes(A) = FloodRouteDArgs.SourceNodes(A)
            Next
            SubPaths(LayerNum).NodeCount = FloodRouteDArgs.BestNodeCount
            If FloodRouteDArgs.BestNodeCount = 0 Then
                Stop
                Return SubPaths
            End If
        Loop
        Return SubPaths
    End Function

    Public Structure sFloodRouteArgs
        Public CurrentPath As Path
        Public FinishNodes() As PathfinderNode
        Public FinishNodeCount As Integer
        Public FinishIsParent As Boolean
        Public Visit() As Boolean
        Public NodeValues() As Single
        Public SourceNodes() As PathfinderNode
        Public BestPaths() As Path
        Public MinClearance As Integer
    End Structure

    Public Structure sFloodRouteAllArgs
        Public StartNodes() As PathfinderNode
        Public StartNodeCount As Integer
        Public FinishNode As PathfinderNode
        Public Visit() As Boolean
        Public NodeValuesA() As Single
        Public NodeValuesB() As Single
        Public SourceNodes() As PathfinderNode
        Public BestTolerance As Single
        Public BestNodeCount As Integer
        Public MinClearance As Integer
    End Structure

    Public Sub FloodRoute(ByRef Args As sFloodRouteArgs)
        Dim CurrentNode As PathfinderNode
        Dim ConnectedNode As PathfinderNode
        Dim A As Integer
        Dim SourceNodeCount As Integer
        Dim SourceNodeNum As Integer
        Dim tmpConnection As PathfinderConnection
        Dim ResultValue As Single
        Dim BestPath As Path
        Dim StartNode As PathfinderNode
        Dim B As Integer
        Dim BestDist As Single
        Dim Dist As Single
        Dim BestNode As PathfinderNode
        Dim C As Integer

        StartNode = Args.CurrentPath.Nodes(0)

        Args.SourceNodes(0) = StartNode
        SourceNodeCount = 1
        Args.NodeValues(StartNode.Layer_NodeNum) = 0.0F

        SourceNodeNum = 0
        Do While SourceNodeNum < SourceNodeCount
            CurrentNode = Args.SourceNodes(SourceNodeNum)
            For A = 0 To CurrentNode.ConnectionCount - 1
                tmpConnection = CurrentNode.Connections(A)
                ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                If ConnectedNode.ParentNode IsNot Nothing Then
                    If Args.Visit(ConnectedNode.ParentNode.Layer_NodeNum) Then
                        If ConnectedNode.Clearance >= Args.MinClearance Then
                            ResultValue = Args.NodeValues(CurrentNode.Layer_NodeNum) + tmpConnection.Value
                            If ResultValue < Args.NodeValues(ConnectedNode.Layer_NodeNum) Then
                                Args.NodeValues(ConnectedNode.Layer_NodeNum) = ResultValue
                                Args.SourceNodes(SourceNodeCount) = ConnectedNode
                                SourceNodeCount += 1
                            End If
                        End If
                    End If
                End If
            Next
            SourceNodeNum += 1
        Loop

        For A = 0 To Args.FinishNodeCount - 1
            If Args.FinishIsParent Then
                BestNode = Nothing
                BestDist = Single.MaxValue
                For C = 0 To Args.FinishNodes(A).NodeCount - 1
                    CurrentNode = Args.FinishNodes(A).Nodes(C)
                    Dist = Args.NodeValues(CurrentNode.Layer_NodeNum)
                    If Dist < BestDist Then
                        BestDist = Dist
                        BestNode = CurrentNode
                    End If
                Next
                CurrentNode = BestNode
            Else
                CurrentNode = Args.FinishNodes(A)
            End If
            If CurrentNode Is Nothing Then
                GoTo NoPath
            End If
            SourceNodeCount = 0
            BestDist = Args.NodeValues(CurrentNode.Layer_NodeNum)
            If BestDist = Single.MaxValue Then
                GoTo NoPath
            End If
            Do
                Args.SourceNodes(SourceNodeCount) = CurrentNode
                SourceNodeCount += 1
                If CurrentNode Is StartNode Then
                    Exit Do
                End If
                BestNode = Nothing
                For B = 0 To CurrentNode.ConnectionCount - 1
                    tmpConnection = CurrentNode.Connections(B)
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                    If ConnectedNode.ParentNode IsNot Nothing Then
                        If Args.Visit(ConnectedNode.ParentNode.Layer_NodeNum) Then
                            Dist = Args.NodeValues(ConnectedNode.Layer_NodeNum)
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestNode = ConnectedNode
                            End If
                        End If
                    End If
                Next
                If BestNode Is Nothing Then
                    Args.BestPaths(A) = Nothing
                    GoTo NoPath
                End If
                CurrentNode = BestNode
            Loop

            BestPath = New Path
            Args.BestPaths(A) = BestPath
            BestPath.Value = Args.NodeValues(Args.FinishNodes(A).Layer_NodeNum)
            BestPath.NodeCount = SourceNodeCount
            ReDim BestPath.Nodes(BestPath.NodeCount - 1)
            For B = 0 To BestPath.NodeCount - 1
                BestPath.Nodes(B) = Args.SourceNodes(SourceNodeCount - B - 1)
            Next
NoPath:
        Next
    End Sub

    Public Structure sFloodSpanArgs
        Public CurrentPath As Path
        Public SourceParentNode As PathfinderNode
        Public FinishNodes() As PathfinderNode
        Public FinishNodeCount As Integer
        Public FinishIsParent As Boolean
        Public NodeValues() As Single
        Public SourceNodes() As PathfinderNode
        Public BestPaths() As Path
        Public MinClearance As Integer
    End Structure

    Public Sub FloodSpan(ByRef Args As sFloodSpanArgs)
        Dim CurrentNode As PathfinderNode
        Dim ConnectedNode As PathfinderNode
        Dim A As Integer
        Dim SourceNodeCount As Integer
        Dim SourceNodeNum As Integer
        Dim tmpConnection As PathfinderConnection
        Dim ResultValue As Single
        Dim BestPath As Path
        Dim StartNode As PathfinderNode
        Dim B As Integer
        Dim BestDist As Single
        Dim Dist As Single
        Dim BestNode As PathfinderNode
        Dim C As Integer

        StartNode = Args.CurrentPath.Nodes(0)

        Args.SourceNodes(0) = StartNode
        SourceNodeCount = 1
        Args.NodeValues(StartNode.Layer_NodeNum) = 0.0F

        SourceNodeNum = 0
        Do While SourceNodeNum < SourceNodeCount
            CurrentNode = Args.SourceNodes(SourceNodeNum)
            For A = 0 To CurrentNode.ConnectionCount - 1
                tmpConnection = CurrentNode.Connections(A)
                ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                If ConnectedNode.ParentNode Is Args.SourceParentNode Then
                    ResultValue = Args.NodeValues(CurrentNode.Layer_NodeNum) + tmpConnection.Value
                    If ResultValue < Args.NodeValues(ConnectedNode.Layer_NodeNum) Then
                        Args.NodeValues(ConnectedNode.Layer_NodeNum) = ResultValue
                        Args.SourceNodes(SourceNodeCount) = ConnectedNode
                        SourceNodeCount += 1
                    End If
                End If
            Next
            SourceNodeNum += 1
        Loop

        For A = 0 To Args.FinishNodeCount - 1
            If Args.FinishIsParent Then
                BestNode = Nothing
                BestDist = Single.MaxValue
                For C = 0 To Args.FinishNodes(A).NodeCount - 1
                    CurrentNode = Args.FinishNodes(A).Nodes(C)
                    Dist = Args.NodeValues(CurrentNode.Layer_NodeNum)
                    If Dist < BestDist Then
                        BestDist = Dist
                        BestNode = CurrentNode
                    End If
                Next
                CurrentNode = BestNode
            Else
                CurrentNode = Args.FinishNodes(A)
            End If
            If CurrentNode Is Nothing Then
                GoTo NoPath
            End If
            SourceNodeCount = 0
            BestDist = Args.NodeValues(CurrentNode.Layer_NodeNum)
            If BestDist = Single.MaxValue Then
                GoTo NoPath
            End If
            Do
                Args.SourceNodes(SourceNodeCount) = CurrentNode
                SourceNodeCount += 1
                If CurrentNode Is StartNode Then
                    Exit Do
                End If
                BestNode = Nothing
                For B = 0 To CurrentNode.ConnectionCount - 1
                    tmpConnection = CurrentNode.Connections(B)
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                    If ConnectedNode.ParentNode Is Args.SourceParentNode Then
                        Dist = Args.NodeValues(ConnectedNode.Layer_NodeNum)
                        If Dist < BestDist Then
                            BestDist = Dist
                            BestNode = ConnectedNode
                        End If
                    End If
                Next
                If BestNode Is Nothing Then
                    Args.BestPaths(A) = Nothing
                    GoTo NoPath
                End If
                CurrentNode = BestNode
            Loop

            BestPath = New Path
            Args.BestPaths(A) = BestPath
            BestPath.Value = Args.NodeValues(Args.FinishNodes(A).Layer_NodeNum)
            BestPath.NodeCount = SourceNodeCount
            ReDim BestPath.Nodes(BestPath.NodeCount - 1)
            For B = 0 To BestPath.NodeCount - 1
                BestPath.Nodes(B) = Args.SourceNodes(SourceNodeCount - B - 1)
            Next
NoPath:
        Next
    End Sub

    Public Structure sFloodForValuesArgs
        Public CurrentPath As Path
        Public SourceParentNodeA As PathfinderNode
        Public SourceParentNodeB As PathfinderNode
        Public FinishNodes() As PathfinderNode
        Public FinishNodeCount As Integer
        Public FinishIsParent As Boolean
        Public NodeValues() As Single
        Public SourceNodes() As PathfinderNode
        Public BestPaths() As Path
        Public MinClearance As Integer
    End Structure

    Public Sub FloodForValues(ByRef Args As sFloodForValuesArgs)
        Dim CurrentNode As PathfinderNode
        Dim ConnectedNode As PathfinderNode
        Dim A As Integer
        Dim SourceNodeCount As Integer
        Dim SourceNodeNum As Integer
        Dim tmpConnection As PathfinderConnection
        Dim ResultValue As Single
        Dim BestPath As Path
        Dim StartNode As PathfinderNode
        Dim B As Integer
        Dim BestDist As Single
        Dim Dist As Single
        Dim BestNode As PathfinderNode
        Dim C As Integer

        StartNode = Args.CurrentPath.Nodes(0)

        Args.SourceNodes(0) = StartNode
        SourceNodeCount = 1
        Args.NodeValues(StartNode.Layer_NodeNum) = 0.0F

        SourceNodeNum = 0
        Do While SourceNodeNum < SourceNodeCount
            CurrentNode = Args.SourceNodes(SourceNodeNum)
            For A = 0 To CurrentNode.ConnectionCount - 1
                tmpConnection = CurrentNode.Connections(A)
                ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                If ConnectedNode.ParentNode Is Args.SourceParentNodeA Or ConnectedNode.ParentNode Is Args.SourceParentNodeB Then
                    ResultValue = Args.NodeValues(CurrentNode.Layer_NodeNum) + tmpConnection.Value
                    If ResultValue < Args.NodeValues(ConnectedNode.Layer_NodeNum) Then
                        Args.NodeValues(ConnectedNode.Layer_NodeNum) = ResultValue
                        Args.SourceNodes(SourceNodeCount) = ConnectedNode
                        SourceNodeCount += 1
                    End If
                End If
            Next
            SourceNodeNum += 1
        Loop

        For A = 0 To Args.FinishNodeCount - 1
            If Args.FinishIsParent Then
                BestNode = Nothing
                BestDist = Single.MaxValue
                For C = 0 To Args.FinishNodes(A).NodeCount - 1
                    CurrentNode = Args.FinishNodes(A).Nodes(C)
                    Dist = Args.NodeValues(CurrentNode.Layer_NodeNum)
                    If Dist < BestDist Then
                        BestDist = Dist
                        BestNode = CurrentNode
                    End If
                Next
                CurrentNode = BestNode
            Else
                CurrentNode = Args.FinishNodes(A)
            End If
            If CurrentNode Is Nothing Then
                GoTo NoPath
            End If
            SourceNodeCount = 0
            BestDist = Args.NodeValues(CurrentNode.Layer_NodeNum)
            If BestDist = Single.MaxValue Then
                GoTo NoPath
            End If
            Do
                Args.SourceNodes(SourceNodeCount) = CurrentNode
                SourceNodeCount += 1
                If CurrentNode Is StartNode Then
                    Exit Do
                End If
                BestNode = Nothing
                For B = 0 To CurrentNode.ConnectionCount - 1
                    tmpConnection = CurrentNode.Connections(B)
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                    If ConnectedNode.ParentNode Is Args.SourceParentNodeA Or ConnectedNode.ParentNode Is Args.SourceParentNodeB Then
                        Dist = Args.NodeValues(ConnectedNode.Layer_NodeNum)
                        If Dist < BestDist Then
                            BestDist = Dist
                            BestNode = ConnectedNode
                        End If
                    End If
                Next
                If BestNode Is Nothing Then
                    Args.BestPaths(A) = Nothing
                    GoTo NoPath
                End If
                CurrentNode = BestNode
            Loop

            BestPath = New Path
            Args.BestPaths(A) = BestPath
            BestPath.Value = Args.NodeValues(Args.FinishNodes(A).Layer_NodeNum)
            BestPath.NodeCount = SourceNodeCount
            ReDim BestPath.Nodes(BestPath.NodeCount - 1)
            For B = 0 To BestPath.NodeCount - 1
                BestPath.Nodes(B) = Args.SourceNodes(SourceNodeCount - B - 1)
            Next
NoPath:
        Next
    End Sub

    Public Sub FloodRouteAll(ByRef Args As sFloodRouteAllArgs)
        Dim CurrentNode As PathfinderNode
        Dim ConnectedNode As PathfinderNode
        Dim SourceNodeCount As Integer
        Dim SourceNodeNum As Integer
        Dim tmpConnection As PathfinderConnection
        Dim ResultValue As Single
        Dim BestValue As Single
        Dim A As Integer

        SourceNodeCount = Args.StartNodeCount
        For A = 0 To SourceNodeCount - 1
            Args.SourceNodes(A) = Args.StartNodes(A)
            Args.NodeValuesA(Args.StartNodes(A).Layer_NodeNum) = 0.0F
        Next

        SourceNodeNum = 0
        Do While SourceNodeNum < SourceNodeCount
            CurrentNode = Args.SourceNodes(SourceNodeNum)
            For A = 0 To CurrentNode.ConnectionCount - 1
                tmpConnection = CurrentNode.Connections(A)
                ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                If Args.Visit(ConnectedNode.ParentNode.Layer_NodeNum) Then
                    If ConnectedNode.Clearance >= Args.MinClearance Then
                        ResultValue = Args.NodeValuesA(CurrentNode.Layer_NodeNum) + tmpConnection.Value
                        If ResultValue < Args.NodeValuesA(ConnectedNode.Layer_NodeNum) Then
                            Args.NodeValuesA(ConnectedNode.Layer_NodeNum) = ResultValue
                            Args.SourceNodes(SourceNodeCount) = ConnectedNode
                            SourceNodeCount += 1
                        End If
                    End If
                End If
            Next
            SourceNodeNum += 1
        Loop

        SourceNodeCount = 0
        BestValue = Args.NodeValuesA(Args.FinishNode.Layer_NodeNum)
        If BestValue = Single.MaxValue Then
            Args.BestNodeCount = 0
            Exit Sub
        End If

        BestValue *= Args.BestTolerance

        Args.SourceNodes(0) = Args.FinishNode
        SourceNodeCount = 1
        Args.NodeValuesB(Args.FinishNode.Layer_NodeNum) = 0.0F

        SourceNodeNum = 0
        Do While SourceNodeNum < SourceNodeCount
            CurrentNode = Args.SourceNodes(SourceNodeNum)
            For A = 0 To CurrentNode.ConnectionCount - 1
                tmpConnection = CurrentNode.Connections(A)
                ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                If Args.Visit(ConnectedNode.ParentNode.Layer_NodeNum) Then
                    ResultValue = Args.NodeValuesB(CurrentNode.Layer_NodeNum) + tmpConnection.Value
                    If ResultValue < Args.NodeValuesB(ConnectedNode.Layer_NodeNum) Then
                        Args.NodeValuesB(ConnectedNode.Layer_NodeNum) = ResultValue
                        If Args.NodeValuesA(ConnectedNode.Layer_NodeNum) + ResultValue <= BestValue + 500.0F Then
                            Args.SourceNodes(SourceNodeCount) = ConnectedNode
                            SourceNodeCount += 1
                        End If
                    End If
                End If
            Next
            SourceNodeNum += 1
        Loop

        Args.BestNodeCount = SourceNodeCount
    End Sub

    Public Sub FindCalc()

        Dim ShuffledNodes() As PathfinderNode
        Dim ShuffledNodeCount As Integer
        Dim Positions() As Integer
        Dim PositionCount As Integer
        Dim RandNum As Integer
        Dim A As Integer

        Do While FindParentNodeCount > 0

            ReDim Positions(FindParentNodeCount - 1)
            ShuffledNodeCount = FindParentNodeCount
            ReDim ShuffledNodes(ShuffledNodeCount - 1)

            For A = 0 To FindParentNodeCount - 1
                Positions(PositionCount) = PositionCount
                PositionCount += 1
            Next
            For A = 0 To FindParentNodeCount - 1
                RandNum = CInt(Int(Rnd() * PositionCount))
                ShuffledNodes(Positions(RandNum)) = FindParentNodes(A)
                PositionCount -= 1
                If RandNum < PositionCount Then
                    Positions(RandNum) = Positions(PositionCount)
                End If
            Next

            For A = 0 To ShuffledNodeCount - 1
                If ShuffledNodes(A).Network_FindParentNum >= 0 Then
                    If ShuffledNodes(A).ParentNode Is Nothing Then
                        ShuffledNodes(A).FindParent()
                    End If
                    FindParentNode_Remove(ShuffledNodes(A).Network_FindParentNum)
                End If
            Next

        Loop

        'remove empty layers
        Dim LayerNum As Integer = NodeLayerCount - 1
        Do
            If NodeLayers(LayerNum).NodeCount > 0 Then
                Exit Do
            End If
            NodeLayers(LayerNum).Network_LayerNum = -1
            If LayerNum = 0 Then
                Exit Do
            End If
            NodeLayers(LayerNum - 1).ParentLayer = Nothing
            LayerNum -= 1
        Loop
        If LayerNum < NodeLayerCount - 1 Then
            ReDim Preserve NodeLayers(LayerNum)
            NodeLayerCount = LayerNum + 1
        End If
    End Sub

    Public Sub LargeArraysResize()

        NetworkLargeArrays.Resize(Me)
    End Sub

    Public Structure sFloodProximityArgs
        Public StartNode As PathfinderNode
        Public NodeValues() As Single
    End Structure

    'maps lowest values from the start node to all other nodes
    Public Sub FloodProximity(ByRef Args As sFloodProximityArgs)
        Dim CurrentNode As PathfinderNode
        Dim ConnectedNode As PathfinderNode
        Dim A As Integer
        Dim SourceNodeCount As Integer
        Dim SourceNodeNum As Integer
        Dim tmpConnection As PathfinderConnection
        Dim ResultValue As Single
        Dim StartNode As PathfinderNode

        StartNode = Args.StartNode

        NetworkLargeArrays.Nodes_Nodes(0) = StartNode
        SourceNodeCount = 1
        Args.NodeValues(StartNode.Layer_NodeNum) = 0.0F

        SourceNodeNum = 0
        Do While SourceNodeNum < SourceNodeCount
            CurrentNode = NetworkLargeArrays.Nodes_Nodes(SourceNodeNum)
            For A = 0 To CurrentNode.ConnectionCount - 1
                tmpConnection = CurrentNode.Connections(A)
                ConnectedNode = tmpConnection.GetOtherNode(CurrentNode)
                ResultValue = Args.NodeValues(CurrentNode.Layer_NodeNum) + tmpConnection.Value
                If ResultValue < Args.NodeValues(ConnectedNode.Layer_NodeNum) Then
                    Args.NodeValues(ConnectedNode.Layer_NodeNum) = ResultValue
                    NetworkLargeArrays.Nodes_Nodes(SourceNodeCount) = ConnectedNode
                    SourceNodeCount += 1
                End If
            Next
            SourceNodeNum += 1
        Loop
    End Sub

    Public Sub ClearChangedNodes()
        Dim A As Integer

        For A = 0 To NodeLayerCount - 1
            NodeLayers(A).ClearChangedNodes()
        Next
    End Sub

    Public Function NodeCanReachNode(StartNode As PathfinderNode, FinishNode As PathfinderNode) As Boolean
        Dim StartParent As PathfinderNode = StartNode
        Dim FinishParent As PathfinderNode = FinishNode

        Do
            If StartParent Is FinishParent Then
                Return True
            End If
            StartParent = StartParent.ParentNode
            If StartParent Is Nothing Then
                Return False
            End If
            FinishParent = FinishParent.ParentNode
            If FinishParent Is Nothing Then
                Return False
            End If
        Loop
        Return False
    End Function
End Class