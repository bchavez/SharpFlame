
Public Class clsGenerateMap

    Public Map As clsMap

    Public TileSize As sXY_int
    Public LevelCount As Integer
    Public LevelHeight As Single
    Public SymmetryIsRotational As Boolean
    Public Structure sSymmetryBlock
        Public XYNum As sXY_int
        Public Orientation As sTileOrientation
        Public ReflectToNum() As Integer
    End Structure
    Public SymmetryBlocks() As sSymmetryBlock
    Public SymmetryBlockCountXY As sXY_int
    Public SymmetryBlockCount As Integer
    Public JitterScale As Integer
    Public MaxLevelTransition As Integer
    Public NodeScale As Single
    Public BaseLevel As Integer
    Public BaseFlatArea As Integer
    Public PlayerBasePos() As sXY_int
    Public TopLeftPlayerCount As Integer
    Public PassagesChance As Integer
    Public VariationChance As Integer
    Public FlatsChance As Integer
    Public MaxDisconnectionDist As Single
    Public RampBase As Double
    Public BaseOilCount As Integer
    Public ExtraOilCount As Integer
    Public ExtraOilClusterSizeMin As Integer
    Public ExtraOilClusterSizeMax As Integer
    Public OilDispersion As Single
    Public OilAtATime As Integer
    Public WaterSpawnQuantity As Integer
    Public TotalWaterQuantity As Integer
    Public FeatureClusterChance As Single
    Public FeatureClusterMinUnits As Integer
    Public FeatureClusterMaxUnits As Integer
    Public FeatureScatterCount As Integer
    Public FeatureScatterGap As Integer
    Public BaseTruckCount As Integer

    Private Connections() As clsConnection
    Private ConnectionCount As Integer
    Private PassageNodes(,) As clsPassageNode
    Private PassageNodeCount As Integer
    Private PassageNodeDists(,,,) As Single
    Private Nearests() As clsNearest
    Private NearestCount As Integer
    Private Structure sPlayerBase
        Public Nodes() As clsPassageNode
        Public NodeCount As Integer
        Public Pos As sXY_int

        Public Sub CalcPos()
            Dim A As Integer
            Dim Total As Matrix3D.XY_dbl

            For A = 0 To NodeCount - 1
                Total.X += Nodes(A).Pos.X
                Total.Y += Nodes(A).Pos.Y
            Next
            Pos.X = CInt(Total.X / NodeCount)
            Pos.Y = CInt(Total.Y / NodeCount)
        End Sub
    End Structure
    Private PlayerBases() As sPlayerBase
    Private TotalPlayerCount As Integer
    Public ReadOnly Property GetTotalPlayerCount As Integer
        Get
            Return TotalPlayerCount
        End Get
    End Property

    Public Class clsPassageNode
        Public Num As Integer = -1

        Public MirrorNum As Integer = -1

        Public Level As Integer = -1

        Public Pos As sXY_int

        Public IsOnBorder As Boolean
        Public IsNearBorder As Boolean

        Public OilCount As Integer

        Public HasFeatureCluster As Boolean

        Public IsWater As Boolean

        Public PlayerBaseNum As Integer = -1

        Public Structure sConnection
            Public Connection As clsConnection
            Public IsB As Boolean

            Public Function GetOther() As clsPassageNode
                If IsB Then
                    Return Connection.PassageNodeA
                Else
                    Return Connection.PassageNodeB
                End If
            End Function
        End Structure
        Public Connections() As sConnection
        Public ConnectionCount As Integer

        Public Sub Connection_Add(NewConnection As sConnection)

            If NewConnection.IsB Then
                NewConnection.Connection.PassageNodeB_ConnectionNum = ConnectionCount
            Else
                NewConnection.Connection.PassageNodeA_ConnectionNum = ConnectionCount
            End If

            ReDim Preserve Connections(ConnectionCount)
            Connections(ConnectionCount) = NewConnection
            ConnectionCount += 1
        End Sub

        Public Sub Connection_Remove(Num As Integer)

            If Connections(Num).IsB Then
                Connections(Num).Connection.PassageNodeB_ConnectionNum = -1
            Else
                Connections(Num).Connection.PassageNodeA_ConnectionNum = -1
            End If

            ConnectionCount -= 1
            If Num <> ConnectionCount Then
                Connections(Num) = Connections(ConnectionCount)
                If Connections(Num).IsB Then
                    Connections(Num).Connection.PassageNodeB_ConnectionNum = Num
                Else
                    Connections(Num).Connection.PassageNodeA_ConnectionNum = Num
                End If
            End If
        End Sub

        Public Function FindConnection(PassageNode As clsPassageNode) As clsConnection
            Dim A As Integer

            For A = 0 To ConnectionCount - 1
                If Connections(A).GetOther Is PassageNode Then
                    Return Connections(A).Connection
                End If
            Next
            Return Nothing
        End Function

        Public Sub ReorderConnections()
            Dim A As Integer
            Dim B As Integer
            Dim C As Integer
            Dim NewOrder(ConnectionCount - 1) As sConnection
            Dim AwayAngles(ConnectionCount - 1) As Double
            Dim OtherNode As clsPassageNode
            Dim XY_int As sXY_int
            Dim AwayAngle As Double

            For A = 0 To ConnectionCount - 1
                OtherNode = Connections(A).GetOther
                XY_int.X = OtherNode.Pos.X - Pos.X
                XY_int.Y = OtherNode.Pos.Y - Pos.Y
                AwayAngle = XY_int.ToDoubles.GetAngle
                For B = 0 To A - 1
                    If AwayAngle < AwayAngles(B) Then
                        Exit For
                    End If
                Next
                For C = A - 1 To B Step -1
                    NewOrder(C + 1) = NewOrder(C)
                    AwayAngles(C + 1) = AwayAngles(C)
                Next
                NewOrder(B) = Connections(A)
                AwayAngles(B) = AwayAngle
            Next
            For A = 0 To ConnectionCount - 1
                Connections(A) = NewOrder(A)
                If Connections(A).IsB Then
                    Connections(A).Connection.PassageNodeB_ConnectionNum = A
                Else
                    Connections(A).Connection.PassageNodeA_ConnectionNum = A
                End If
            Next
        End Sub

        Public Sub CalcIsNearBorder()
            Dim A As Integer

            For A = 0 To ConnectionCount - 1
                If Connections(A).GetOther.IsOnBorder Then
                    IsNearBorder = True
                    Exit Sub
                End If
            Next
            IsNearBorder = False
        End Sub
    End Class

    Public Class clsConnection
        Public PassageNodeA As clsPassageNode
        Public PassageNodeA_ConnectionNum As Integer = -1
        Public PassageNodeB As clsPassageNode
        Public PassageNodeB_ConnectionNum As Integer = -1
        Public IsRamp As Boolean
        Public Reflections() As clsConnection
        Public ReflectionCount As Integer

        Public Sub New(NewPassageNodeA As clsPassageNode, NewPassageNodeB As clsPassageNode)
            Dim NewConnection As clsPassageNode.sConnection

            PassageNodeA = NewPassageNodeA
            NewConnection.Connection = Me
            NewConnection.IsB = False
            PassageNodeA.Connection_Add(NewConnection)

            PassageNodeB = NewPassageNodeB
            NewConnection.Connection = Me
            NewConnection.IsB = True
            PassageNodeB.Connection_Add(NewConnection)
        End Sub
    End Class


    Public TilePathMap As PathfinderNetwork
    Public VertexPathMap As PathfinderNetwork

    Public GenerateTileset As clsGeneratorTileset

    Public Structure GenerateTerrainVertex
        Public Node As PathfinderNode
        Public TopLink As PathfinderConnection
        Public TopRightLink As PathfinderConnection
        Public RightLink As PathfinderConnection
        Public BottomRightLink As PathfinderConnection
        Public BottomLink As PathfinderConnection
        Public BottomLeftLink As PathfinderConnection
        Public LeftLink As PathfinderConnection
        Public TopLeftLink As PathfinderConnection
    End Structure

    Public GenerateTerrainVertices(,) As GenerateTerrainVertex

    Public Structure GenerateTerrainTile
        Public Node As PathfinderNode
        Public TopLeftLink As PathfinderConnection
        Public TopLink As PathfinderConnection
        Public TopRightLink As PathfinderConnection
        Public RightLink As PathfinderConnection
        Public BottomRightLink As PathfinderConnection
        Public BottomLink As PathfinderConnection
        Public BottomLeftLink As PathfinderConnection
        Public LeftLink As PathfinderConnection
    End Structure

    Public GenerateTerrainTiles(-1, -1) As GenerateTerrainTile

    Public Function GenerateLayout() As clsResult
        Dim ReturnResult As New clsResult("Layout")

        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim E As Integer
        Dim F As Integer
        Dim G As Integer
        Dim H As Integer

        TotalPlayerCount = TopLeftPlayerCount * SymmetryBlockCount

        Dim SymmetrySize As sXY_int

        SymmetrySize.X = CInt(TileSize.X * TerrainGridSpacing / SymmetryBlockCountXY.X)
        SymmetrySize.Y = CInt(TileSize.Y * TerrainGridSpacing / SymmetryBlockCountXY.Y)

        'create passage nodes

        Dim PassageRadius As Integer = CInt(128.0F * NodeScale)
        Dim MaxLikelyPassageNodeCount As Integer
        MaxLikelyPassageNodeCount = CInt(Math.Ceiling(2.0# * TileSize.X * 128 * TileSize.Y * 128 / (Math.PI * PassageRadius * PassageRadius)))

        ReDim PassageNodes(SymmetryBlockCount - 1, MaxLikelyPassageNodeCount - 1)
        Dim LoopCount As Integer
        Dim EdgeOffset As Integer = 0 * 128
        Dim PointIsValid As Boolean
        Dim EdgeSections As sXY_int
        Dim EdgeSectionSize As Matrix3D.XY_dbl
        Dim NewPointPos As sXY_int

        If SymmetryBlockCountXY.X = 1 Then
            EdgeSections.X = CInt(Int((TileSize.X * TerrainGridSpacing - EdgeOffset * 2.0#) / (NodeScale * TerrainGridSpacing * 2.0F)))
            EdgeSectionSize.X = (TileSize.X * TerrainGridSpacing - EdgeOffset * 2.0#) / EdgeSections.X
            EdgeSections.X -= 1
        Else
            EdgeSections.X = CInt(Int((TileSize.X * TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) / (NodeScale * TerrainGridSpacing * 2.0F) - 0.5#))
            EdgeSectionSize.X = (TileSize.X * TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) / (Int((TileSize.X * TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) / (NodeScale * TerrainGridSpacing * 2.0F) - 0.5#) + 0.5#)
        End If
        If SymmetryBlockCountXY.Y = 1 Then
            EdgeSections.Y = CInt(Int((TileSize.Y * TerrainGridSpacing - EdgeOffset * 2.0#) / (NodeScale * TerrainGridSpacing * 2.0F)))
            EdgeSectionSize.Y = (TileSize.Y * TerrainGridSpacing - EdgeOffset * 2.0#) / EdgeSections.Y
            EdgeSections.Y -= 1
        Else
            EdgeSections.Y = CInt(Int((TileSize.Y * TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) / (NodeScale * TerrainGridSpacing * 2.0F) - 0.5#))
            EdgeSectionSize.Y = (TileSize.Y * TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) / (Int((TileSize.Y * TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) / (NodeScale * TerrainGridSpacing * 2.0F) - 0.5#) + 0.5#)
        End If

        PassageNodeCount = 0
        For Y = 0 To EdgeSections.Y
            If Not MakePassageNodes(New sXY_int(EdgeOffset, EdgeOffset + CInt(Y * EdgeSectionSize.Y)), True) Then
                ReturnResult.ProblemAdd("Error: Bad border node.")
                Return ReturnResult
            End If
            If SymmetryBlockCountXY.X = 1 Then
                If Not MakePassageNodes(New sXY_int(TileSize.X * TerrainGridSpacing - EdgeOffset, EdgeOffset + CInt(Y * EdgeSectionSize.Y)), True) Then
                    ReturnResult.ProblemAdd("Error: Bad border node.")
                    Return ReturnResult
                End If
            End If
        Next
        For X = 1 To EdgeSections.X
            If Not MakePassageNodes(New sXY_int(EdgeOffset + CInt(X * EdgeSectionSize.X), EdgeOffset), True) Then
                ReturnResult.ProblemAdd("Error: Bad border node.")
                Return ReturnResult
            End If
            If SymmetryBlockCountXY.Y = 1 Then
                If Not MakePassageNodes(New sXY_int(EdgeOffset + CInt(X * EdgeSectionSize.X), TileSize.Y * TerrainGridSpacing - EdgeOffset), True) Then
                    ReturnResult.ProblemAdd("Error: Bad border node.")
                    Return ReturnResult
                End If
            End If
        Next
        Do
            LoopCount = 0
            Do
                PointIsValid = True
                If SymmetryBlockCountXY.X = 1 Then
                    NewPointPos.X = EdgeOffset + CInt(Int(Rnd() * (SymmetrySize.X - EdgeOffset * 2 + 1)))
                Else
                    NewPointPos.X = EdgeOffset + CInt(Int(Rnd() * (SymmetrySize.X - EdgeOffset + 1)))
                End If
                If SymmetryBlockCountXY.Y = 1 Then
                    NewPointPos.Y = EdgeOffset + CInt(Int(Rnd() * (SymmetrySize.Y - EdgeOffset * 2 + 1)))
                Else
                    NewPointPos.Y = EdgeOffset + CInt(Int(Rnd() * (SymmetrySize.Y - EdgeOffset + 1)))
                End If
                For A = 0 To PassageNodeCount - 1
                    For B = 0 To SymmetryBlockCount - 1
                        If (PassageNodes(B, A).Pos - NewPointPos).ToDoubles.GetMagnitude < PassageRadius * 2 Then
                            GoTo PointTooClose
                        End If
                    Next
                Next
PointTooClose:
                If A = PassageNodeCount Then
                    If MakePassageNodes(NewPointPos, False) Then
                        Exit Do
                    End If
                End If
                LoopCount += 1
                If LoopCount >= CInt(64.0F * TileSize.X * TileSize.Y / (NodeScale * NodeScale)) Then
                    GoTo PointMakingFinished
                End If
            Loop
        Loop
PointMakingFinished:
        ReDim Preserve PassageNodes(SymmetryBlockCount - 1, PassageNodeCount - 1)

        'connect until all are connected without intersecting

        Dim IntersectPos As sIntersectPos
        Dim MaxConDist2 As Integer = PassageRadius * 2 * 4
        MaxConDist2 *= MaxConDist2
        Dim NearestA As clsNearest
        ReDim Nearests(PassageNodeCount * 64 - 1)
        Dim tmpPassageNodeA As clsPassageNode
        Dim tmpPassageNodeB As clsPassageNode
        Dim NearestArgs As New clsTestNearestArgs
        Dim MinConDist As Integer = CInt(NodeScale * 1.25F * 128.0F)

        NearestArgs.MaxConDist2 = MaxConDist2
        NearestArgs.MinConDist = MinConDist

        For A = 0 To PassageNodeCount - 1
            NearestArgs.PassageNodeA = PassageNodes(0, A)
            For B = A To PassageNodeCount - 1
                For C = 0 To SymmetryBlockCount - 1
                    NearestArgs.PassageNodeB = PassageNodes(C, B)
                    If NearestArgs.PassageNodeA IsNot NearestArgs.PassageNodeB Then
                        TestNearest(NearestArgs)
                    End If
                Next
            Next
        Next

        Dim NearestB As clsNearest
        Dim Flag As Boolean

        For G = 0 To NearestCount - 1
            NearestA = Nearests(G)
            For A = 0 To NearestA.NodeCount - 1
                tmpPassageNodeA = NearestA.NodeA(A)
                tmpPassageNodeB = NearestA.NodeB(A)
                For H = 0 To NearestCount - 1
                    NearestB = Nearests(H)
                    If NearestB IsNot NearestA Then
                        If NearestB.Dist2 < NearestA.Dist2 Then
                            Flag = True
                        ElseIf NearestB.Dist2 = NearestA.Dist2 Then
                            Flag = (NearestA.Num > NearestB.Num)
                        Else
                            Flag = False
                        End If
                        If Flag Then
                            For B = 0 To NearestB.NodeCount - 1
                                If Not (tmpPassageNodeA Is NearestB.NodeA(B) Or tmpPassageNodeA Is NearestB.NodeB(B) _
                                  Or tmpPassageNodeB Is NearestB.NodeA(B) Or tmpPassageNodeB Is NearestB.NodeB(B)) Then
                                    IntersectPos = GetLinesIntersectBetween(tmpPassageNodeA.Pos, tmpPassageNodeB.Pos, NearestB.NodeA(B).Pos, NearestB.NodeB(B).Pos)
                                    If IntersectPos.Exists Then
                                        Exit For
                                    End If
                                End If
                            Next
                            If B < NearestB.NodeCount Then
                                NearestA.BlockedCount += 1
                                With NearestB
                                    .BlockedNearests(.BlockedNearestCount) = NearestA
                                    .BlockedNearestCount += 1
                                End With
                            End If
                        End If
                    End If
                Next
            Next
        Next

        Dim ChangeCount As Integer
        ReDim Connections(PassageNodeCount * 16 - 1)

        Do
            'create valid connections
            ChangeCount = 0
            G = 0
            Do While G < NearestCount
                NearestA = Nearests(G)
                Flag = True
                If NearestA.BlockedCount = 0 And Flag Then
                    F = ConnectionCount
                    For D = 0 To NearestA.NodeCount - 1
                        Connections(ConnectionCount) = New clsConnection(NearestA.NodeA(D), NearestA.NodeB(D))
                        ConnectionCount += 1
                    Next
                    For D = 0 To NearestA.NodeCount - 1
                        A = F + D
                        Connections(A).ReflectionCount = NearestA.NodeCount - 1
                        ReDim Connections(A).Reflections(Connections(A).ReflectionCount - 1)
                        B = 0
                        For E = 0 To NearestA.NodeCount - 1
                            If E <> D Then
                                Connections(A).Reflections(B) = Connections(F + E)
                                B += 1
                            End If
                        Next
                    Next
                    For C = 0 To NearestA.BlockedNearestCount - 1
                        NearestA.BlockedNearests(C).Invalid = True
                    Next
                    NearestCount -= 1
                    H = NearestA.Num
                    NearestA.Num = -1
                    If H <> NearestCount Then
                        Nearests(H) = Nearests(NearestCount)
                        Nearests(H).Num = H
                    End If
                    ChangeCount += 1
                Else
                    If Not Flag Then
                        NearestA.Invalid = True
                    End If
                    G += 1
                End If
            Loop
            'remove blocked ones and their blocking effect
            G = 0
            Do While G < NearestCount
                NearestA = Nearests(G)
                If NearestA.Invalid Then
                    NearestA.Num = -1
                    For D = 0 To NearestA.BlockedNearestCount - 1
                        NearestA.BlockedNearests(D).BlockedCount -= 1
                    Next
                    NearestCount -= 1
                    If G <> NearestCount Then
                        Nearests(G) = Nearests(NearestCount)
                        Nearests(G).Num = G
                    End If
                Else
                    G += 1
                End If
            Loop
        Loop While ChangeCount > 0

        'put connections in order of angle

        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                PassageNodes(B, A).ReorderConnections()
                PassageNodes(B, A).CalcIsNearBorder()
            Next
        Next

        'get nodes in random order

        Dim PassageNodeListOrder(PassageNodeCount - 1) As clsPassageNode
        Dim PassageNodeListOrderCount As Integer = 0
        Dim PassageNodeOrder(PassageNodeCount - 1) As clsPassageNode
        For A = 0 To PassageNodeCount - 1
            PassageNodeListOrder(PassageNodeListOrderCount) = PassageNodes(0, A)
            PassageNodeListOrderCount += 1
        Next
        B = 0
        Do While PassageNodeListOrderCount > 0
            A = CInt(Int(Rnd() * PassageNodeListOrderCount))
            PassageNodeOrder(B) = PassageNodeListOrder(A)
            B += 1
            PassageNodeListOrderCount -= 1
            PassageNodeListOrder(A) = PassageNodeListOrder(PassageNodeListOrderCount)
        Loop

        'designate height levels

        LevelHeight = 255.0F / (LevelCount - 1)
        Dim BestNum As Integer
        Dim Dist As Double
        Dim HeightsArgs As New clsPassageNodeHeightLevelArgs
        ReDim HeightsArgs.PassageNodesMinLevel.Nodes(PassageNodeCount - 1)
        ReDim HeightsArgs.PassageNodesMaxLevel.Nodes(PassageNodeCount - 1)
        ReDim HeightsArgs.MapLevelCount(LevelCount - 1)
        Dim RotatedPos As sXY_int

        For A = 0 To PassageNodeCount - 1
            HeightsArgs.PassageNodesMinLevel.Nodes(A) = 0
            HeightsArgs.PassageNodesMaxLevel.Nodes(A) = LevelCount - 1
        Next

        'create bases
        Dim BestDists(BaseFlatArea - 1) As Double
        Dim BestNodes(BaseFlatArea - 1) As clsPassageNode
        Dim BestNodesReflectionNums(BaseFlatArea - 1) As Integer
        Dim BestDistCount As Integer
        ReDim PlayerBases(TotalPlayerCount - 1)
        For B = 0 To TopLeftPlayerCount - 1
            BestDistCount = 0
            For A = 0 To PassageNodeCount - 1
                For E = 0 To SymmetryBlockCount - 1
                    tmpPassageNodeA = PassageNodes(E, A)
                    If Not tmpPassageNodeA.IsOnBorder Then
                        Dist = (tmpPassageNodeA.Pos - PlayerBasePos(B)).ToDoubles.GetMagnitude
                        For C = BestDistCount - 1 To 0 Step -1
                            If Dist > BestDists(C) Then
                                Exit For
                            End If
                        Next
                        C += 1
                        For D = Math.Min(BestDistCount - 1, BaseFlatArea - 2) To C Step -1
                            BestDists(D + 1) = BestDists(D)
                            BestNodes(D + 1) = BestNodes(D)
                        Next
                        If C < BaseFlatArea Then
                            BestDists(C) = Dist
                            BestNodes(C) = tmpPassageNodeA
                            BestDistCount = Math.Max(BestDistCount, C + 1)
                        End If
                    End If
                Next
            Next

            If BaseLevel < 0 Then
                D = CInt(Int(Rnd() * LevelCount))
            Else
                D = BaseLevel
            End If

            HeightsArgs.MapLevelCount(D) += BestDistCount

            For A = 0 To BestDistCount - 1
                If BestNodes(A).MirrorNum = 0 Then
                    BestNodesReflectionNums(A) = -1
                Else
                    For C = 0 To CInt(SymmetryBlockCount / 2.0#) - 1
                        If SymmetryBlocks(0).ReflectToNum(C) = BestNodes(A).MirrorNum Then
                            Exit For
                        End If
                    Next
                    BestNodesReflectionNums(A) = C
                End If
            Next

            For A = 0 To SymmetryBlockCount - 1
                E = A * TopLeftPlayerCount + B
                PlayerBases(E).NodeCount = BestDistCount
                ReDim PlayerBases(E).Nodes(PlayerBases(E).NodeCount - 1)
                For C = 0 To BestDistCount - 1
                    If BestNodesReflectionNums(C) < 0 Then
                        PlayerBases(E).Nodes(C) = PassageNodes(A, BestNodes(C).Num)
                    Else
                        PlayerBases(E).Nodes(C) = PassageNodes(SymmetryBlocks(A).ReflectToNum(BestNodesReflectionNums(C)), BestNodes(C).Num)
                    End If
                    PlayerBases(E).Nodes(C).PlayerBaseNum = E
                    PlayerBases(E).Nodes(C).Level = D
                    PassageNodesMinLevelSet(PlayerBases(E).Nodes(C), HeightsArgs.PassageNodesMinLevel, D, MaxLevelTransition)
                    PassageNodesMaxLevelSet(PlayerBases(E).Nodes(C), HeightsArgs.PassageNodesMaxLevel, D, MaxLevelTransition)
                Next
                'PlayerBases(E).CalcPos()
                RotatedPos = GetRotatedPos(SymmetryBlocks(A).Orientation, PlayerBasePos(B), New sXY_int(SymmetrySize.X - 1, SymmetrySize.Y - 1))
                PlayerBases(E).Pos.X = SymmetryBlocks(A).XYNum.X * SymmetrySize.X + RotatedPos.X
                PlayerBases(E).Pos.Y = SymmetryBlocks(A).XYNum.Y * SymmetrySize.Y + RotatedPos.Y
            Next
        Next

        Dim WaterCount As Integer
        Dim CanDoFlatsAroundWater As Boolean
        Dim TotalWater As Integer
        Dim WaterSpawns As Integer

        For A = 0 To PassageNodeCount - 1
            tmpPassageNodeA = PassageNodeOrder(A)
            If tmpPassageNodeA.Level < 0 And Not tmpPassageNodeA.IsOnBorder Then
                WaterCount = 0
                For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                    tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                    If tmpPassageNodeB.IsWater Then
                        WaterCount += 1
                    End If
                Next
                CanDoFlatsAroundWater = True
                For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                    If HeightsArgs.PassageNodesMinLevel.Nodes(tmpPassageNodeA.Connections(B).GetOther.Num) > 0 Then
                        CanDoFlatsAroundWater = False
                    End If
                Next
                If CanDoFlatsAroundWater And ((WaterCount = 0 And WaterSpawns < WaterSpawnQuantity) Or (WaterCount = 1 And TotalWaterQuantity - TotalWater > WaterSpawnQuantity - WaterSpawns)) And HeightsArgs.PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) = 0 And TotalWater < TotalWaterQuantity Then
                    If WaterCount = 0 Then
                        WaterSpawns += 1
                    End If
                    TotalWater += 1
                    C = tmpPassageNodeA.Num
                    For D = 0 To SymmetryBlockCount - 1
                        PassageNodes(D, C).IsWater = True
                        PassageNodes(D, C).Level = 0
                    Next
                    PassageNodesMinLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMinLevel, 0, MaxLevelTransition)
                    PassageNodesMaxLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMaxLevel, 0, MaxLevelTransition)
                    HeightsArgs.MapLevelCount(0) += 1
                    For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                        tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                        PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, 0, MaxLevelTransition)
                        PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, 0, MaxLevelTransition)
                    Next
                End If
            End If
        Next

        Dim tmpPassageNodeC As clsPassageNode
        Dim Result As sResult

        HeightsArgs.FlatsCutoff = 1
        HeightsArgs.PassagesCutoff = 1
        HeightsArgs.VariationCutoff = 1
        HeightsArgs.ActionTotal = 1

        For A = 0 To PassageNodeCount - 1
            tmpPassageNodeA = PassageNodeOrder(A)
            If tmpPassageNodeA.Level < 0 And Not tmpPassageNodeA.IsOnBorder And tmpPassageNodeA.IsNearBorder Then
                HeightsArgs.PassageNode = tmpPassageNodeA
                Result = PassageNodeHeightLevel(HeightsArgs)
                If Not Result.Success Then
                    ReturnResult.ProblemAdd(Result.Problem)
                    Return ReturnResult
                End If
            End If
        Next

        HeightsArgs.FlatsCutoff = FlatsChance
        HeightsArgs.PassagesCutoff = HeightsArgs.FlatsCutoff + PassagesChance
        HeightsArgs.VariationCutoff = HeightsArgs.PassagesCutoff + VariationChance
        HeightsArgs.ActionTotal = HeightsArgs.VariationCutoff
        If HeightsArgs.ActionTotal <= 0 Then
            ReturnResult.ProblemAdd("All height level behaviors are zero")
            Return ReturnResult
        End If

        For A = 0 To PassageNodeCount - 1
            tmpPassageNodeA = PassageNodeOrder(A)
            If tmpPassageNodeA.Level < 0 And Not tmpPassageNodeA.IsOnBorder Then
                HeightsArgs.PassageNode = tmpPassageNodeA
                Result = PassageNodeHeightLevel(HeightsArgs)
                If Not Result.Success Then
                    ReturnResult.ProblemAdd(Result.Problem)
                    Return ReturnResult
                End If
            End If
        Next

        'set edge points to the level of their neighbour
        For A = 0 To PassageNodeCount - 1
            tmpPassageNodeA = PassageNodes(0, A)
            If tmpPassageNodeA.IsOnBorder Then
                If tmpPassageNodeA.Level >= 0 Then
                    ReturnResult.ProblemAdd("Error: Border has had its height set.")
                    Return ReturnResult
                End If
                'If tmpPassageNodeA.ConnectionCount <> 1 Then
                '    ReturnResult.Problem = "Error: Border has incorrect connections."
                '    Exit Function
                'End If
                tmpPassageNodeC = Nothing
                CanDoFlatsAroundWater = True
                For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                    tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                    If tmpPassageNodeB.Level >= 0 And Not tmpPassageNodeB.IsOnBorder Then
                        If HeightsArgs.PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) <= tmpPassageNodeB.Level And HeightsArgs.PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num) >= tmpPassageNodeB.Level Then
                            If tmpPassageNodeC Is Nothing Then
                                tmpPassageNodeC = tmpPassageNodeB
                            End If
                        End If
                    End If
                    If HeightsArgs.PassageNodesMinLevel.Nodes(tmpPassageNodeB.Num) > 0 Then
                        CanDoFlatsAroundWater = False
                    End If
                Next
                'If tmpPassageNodeC Is Nothing Then
                '    ReturnResult.Problem_Add("Error: No connection for border node")
                '    Return ReturnResult
                'End If
                If tmpPassageNodeC IsNot Nothing Then
                    BestNum = tmpPassageNodeC.Level
                    PassageNodesMinLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMinLevel, BestNum, MaxLevelTransition)
                    PassageNodesMaxLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMaxLevel, BestNum, MaxLevelTransition)
                    For D = 0 To SymmetryBlockCount - 1
                        PassageNodes(D, A).IsWater = (tmpPassageNodeC.IsWater And CanDoFlatsAroundWater)
                        PassageNodes(D, A).Level = BestNum
                    Next
                    If tmpPassageNodeA.IsWater Then
                        For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                            tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                            PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, tmpPassageNodeA.Level, MaxLevelTransition)
                            PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, tmpPassageNodeA.Level, MaxLevelTransition)
                        Next
                    End If
                End If
            ElseIf tmpPassageNodeA.Level < 0 Then
                ReturnResult.ProblemAdd("Error: Node height not set")
                Return ReturnResult
            End If
        Next
        'set level of edge points only connected to another border point
        For A = 0 To PassageNodeCount - 1
            tmpPassageNodeA = PassageNodes(0, A)
            If tmpPassageNodeA.IsOnBorder And tmpPassageNodeA.Level < 0 Then
                tmpPassageNodeC = Nothing
                CanDoFlatsAroundWater = True
                For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                    tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                    If tmpPassageNodeB.Level >= 0 Then
                        If HeightsArgs.PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) <= tmpPassageNodeB.Level And HeightsArgs.PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num) >= tmpPassageNodeB.Level Then
                            If tmpPassageNodeC Is Nothing Then
                                tmpPassageNodeC = tmpPassageNodeB
                            End If
                        End If
                    End If
                    If HeightsArgs.PassageNodesMinLevel.Nodes(tmpPassageNodeB.Num) > 0 Then
                        CanDoFlatsAroundWater = False
                    End If
                Next
                If tmpPassageNodeC Is Nothing Then
                    ReturnResult.ProblemAdd("Error: No connection for border node")
                    Return ReturnResult
                End If
                BestNum = tmpPassageNodeC.Level
                PassageNodesMinLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMinLevel, BestNum, MaxLevelTransition)
                PassageNodesMaxLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMaxLevel, BestNum, MaxLevelTransition)
                For D = 0 To SymmetryBlockCount - 1
                    PassageNodes(D, A).IsWater = (tmpPassageNodeC.IsWater And CanDoFlatsAroundWater)
                    PassageNodes(D, A).Level = BestNum
                Next
                If tmpPassageNodeA.IsWater Then
                    For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                        tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                        PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, tmpPassageNodeA.Level, MaxLevelTransition)
                        PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, tmpPassageNodeA.Level, MaxLevelTransition)
                    Next
                End If
            End If
        Next

        RampBase = 1.0#
        MaxDisconnectionDist = 99999.0#

        Dim RampResult As clsResult = GenerateRamps()
        ReturnResult.Add(RampResult)

        Return ReturnResult
    End Function

    Private Class clsNearest
        Public Num As Integer = -1
        Public NodeA() As clsPassageNode
        Public NodeB() As clsPassageNode
        Public NodeCount As Integer
        Public Dist2 As Single
        Public BlockedCount As Integer
        Public BlockedNearests() As clsNearest
        Public BlockedNearestCount As Integer
        Public Invalid As Boolean
    End Class

    Private Class clsTestNearestArgs
        Public MaxConDist2 As Integer
        Public MinConDist As Integer
        Public PassageNodeA As clsPassageNode
        Public PassageNodeB As clsPassageNode
    End Class

    Private Function TestNearest(Args As clsTestNearestArgs) As Boolean
        Dim XY_int As sXY_int
        Dim NearestA As clsNearest
        Dim Dist2 As Integer
        Dim A As Integer
        Dim B As Integer
        Dim ReflectionNum As Integer
        Dim ReflectionCount As Integer

        If Args.PassageNodeA.MirrorNum <> 0 Then
            Stop
            Return False
        End If

        XY_int.X = Args.PassageNodeB.Pos.X - Args.PassageNodeA.Pos.X
        XY_int.Y = Args.PassageNodeB.Pos.Y - Args.PassageNodeA.Pos.Y
        Dist2 = XY_int.X * XY_int.X + XY_int.Y * XY_int.Y
        If Dist2 > Args.MaxConDist2 Then
            Return False
        End If
        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                If PassageNodes(B, A) IsNot Args.PassageNodeA And PassageNodes(B, A) IsNot Args.PassageNodeB Then
                    XY_int = PointGetClosestPosOnLine(Args.PassageNodeA.Pos, Args.PassageNodeB.Pos, PassageNodes(B, A).Pos)
                    If (XY_int - PassageNodes(B, A).Pos).ToDoubles.GetMagnitude < Args.MinConDist Then
                        Return False
                    End If
                End If
            Next
        Next

        NearestA = New clsNearest
        With NearestA
            .Num = NearestCount
            .Dist2 = Dist2
            If Args.PassageNodeA.MirrorNum = Args.PassageNodeB.MirrorNum Then
                ReDim .NodeA(SymmetryBlockCount - 1)
                ReDim .NodeB(SymmetryBlockCount - 1)
                For A = 0 To SymmetryBlockCount - 1
                    .NodeA(A) = PassageNodes(A, Args.PassageNodeA.Num)
                    .NodeB(A) = PassageNodes(A, Args.PassageNodeB.Num)
                Next
                .NodeCount = SymmetryBlockCount
            Else
                If SymmetryIsRotational Then
                    ReDim .NodeA(SymmetryBlockCount - 1)
                    ReDim .NodeB(SymmetryBlockCount - 1)
                    ReflectionCount = CInt(SymmetryBlockCount / 2.0#)
                    For ReflectionNum = 0 To ReflectionCount - 1
                        If SymmetryBlocks(0).ReflectToNum(ReflectionNum) = Args.PassageNodeB.MirrorNum Then
                            Exit For
                        End If
                    Next
                    If ReflectionNum = ReflectionCount Then
                        Return False
                    End If
                    For A = 0 To SymmetryBlockCount - 1
                        .NodeA(A) = PassageNodes(A, Args.PassageNodeA.Num)
                        .NodeB(A) = PassageNodes(SymmetryBlocks(A).ReflectToNum(ReflectionNum), Args.PassageNodeB.Num)
                    Next
                    .NodeCount = SymmetryBlockCount
                Else
                    If Args.PassageNodeA.Num <> Args.PassageNodeB.Num Then
                        Return False
                    End If
                    If SymmetryBlockCount = 4 Then
                        ReDim .NodeA(1)
                        ReDim .NodeB(1)
                        ReflectionCount = CInt(SymmetryBlockCount / 2.0#)
                        For ReflectionNum = 0 To ReflectionCount - 1
                            If SymmetryBlocks(0).ReflectToNum(ReflectionNum) = Args.PassageNodeB.MirrorNum Then
                                Exit For
                            End If
                        Next
                        If ReflectionNum = ReflectionCount Then
                            Return False
                        End If
                        .NodeA(0) = Args.PassageNodeA
                        .NodeB(0) = Args.PassageNodeB
                        B = SymmetryBlocks(0).ReflectToNum(1 - ReflectionNum)
                        .NodeA(1) = PassageNodes(B, Args.PassageNodeA.Num)
                        .NodeB(1) = PassageNodes(SymmetryBlocks(B).ReflectToNum(ReflectionNum), Args.PassageNodeB.Num)
                        .NodeCount = 2
                    Else
                        ReDim .NodeA(0)
                        ReDim .NodeB(0)
                        .NodeA(0) = Args.PassageNodeA
                        .NodeB(0) = Args.PassageNodeB
                        .NodeCount = 1
                    End If
                End If
            End If

            ReDim .BlockedNearests(511)
        End With
        Nearests(NearestCount) = NearestA
        NearestCount += 1

        Return True
    End Function

    Public Class clsNodeTag
        Public Pos As sXY_int
    End Class

    Public Function GetNodePosDist(NodeA As PathfinderNode, NodeB As PathfinderNode) As Single
        Dim TagA As clsNodeTag = CType(NodeA.Tag, clsNodeTag)
        Dim TagB As clsNodeTag = CType(NodeB.Tag, clsNodeTag)

        Return CSng((TagA.Pos - TagB.Pos).ToDoubles.GetMagnitude)
    End Function

    Public Sub CalcNodePos(Node As PathfinderNode, ByRef Pos As Matrix3D.XY_dbl, ByRef SampleCount As Integer)

        If Node.GetLayer.GetNetwork_LayerNum = 0 Then
            Dim NodeTag As clsNodeTag
            NodeTag = CType(Node.Tag, clsNodeTag)
            Pos.X += NodeTag.Pos.X
            Pos.Y += NodeTag.Pos.Y
        Else
            Dim A As Integer
            For A = 0 To Node.GetChildNodeCount - 1
                CalcNodePos(Node.GetChildNode(A), Pos, SampleCount)
            Next
            SampleCount += Node.GetChildNodeCount
        End If
    End Sub

    Public Function GenerateLayoutTerrain() As clsResult
        Dim ReturnResult As New clsResult("Terrain heights")

        Dim NodeTag As clsNodeTag
        Dim tmpNodeA As PathfinderNode
        Dim tmpNodeB As PathfinderNode
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim XY_int As sXY_int
        Dim Dist As Double
        Dim BestDist As Double
        Dim Flag As Boolean

        Map = New clsMap(TileSize)
        ReDim GenerateTerrainTiles(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1)
        ReDim GenerateTerrainVertices(Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y)

        'set terrain heights

        VertexPathMap = New PathfinderNetwork

        For Y = 0 To Map.Terrain.TileSize.Y
            For X = 0 To Map.Terrain.TileSize.X
                GenerateTerrainVertices(X, Y) = New GenerateTerrainVertex
                GenerateTerrainVertices(X, Y).Node = New PathfinderNode(VertexPathMap)
                NodeTag = New clsNodeTag
                NodeTag.Pos = New sXY_int(X * 128, Y * 128)
                GenerateTerrainVertices(X, Y).Node.Tag = NodeTag
            Next
        Next
        For Y = 0 To Map.Terrain.TileSize.Y
            For X = 0 To Map.Terrain.TileSize.X
                tmpNodeA = GenerateTerrainVertices(X, Y).Node
                If X > 0 Then
                    tmpNodeB = GenerateTerrainVertices(X - 1, Y).Node
                    GenerateTerrainVertices(X, Y).LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y > 0 Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainVertices(X - 1, Y - 1).Node
                        GenerateTerrainVertices(X, Y).TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainVertices(X, Y - 1).Node
                    GenerateTerrainVertices(X, Y).TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < Map.Terrain.TileSize.X Then
                        tmpNodeB = GenerateTerrainVertices(X + 1, Y - 1).Node
                        GenerateTerrainVertices(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
                If X < Map.Terrain.TileSize.X Then
                    tmpNodeB = GenerateTerrainVertices(X + 1, Y).Node
                    GenerateTerrainVertices(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y < Map.Terrain.TileSize.Y Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainVertices(X - 1, Y + 1).Node
                        GenerateTerrainVertices(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainVertices(X, Y + 1).Node
                    GenerateTerrainVertices(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < Map.Terrain.TileSize.X Then
                        tmpNodeB = GenerateTerrainVertices(X + 1, Y + 1).Node
                        GenerateTerrainVertices(X, Y).BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
            Next
        Next

        VertexPathMap.LargeArraysResize()
        VertexPathMap.FindCalc()

        Dim BaseLayer As PathfinderLayer = VertexPathMap.GetNodeLayer(0)
        Dim JitterLayer As PathfinderLayer = VertexPathMap.GetNodeLayer(JitterScale)
        A = JitterLayer.GetNodeCount - 1
        Dim NodeLevel(A) As Integer
        Dim BaseNodeLevel As New clsBaseNodeLevels
        ReDim BaseNodeLevel.NodeLevels(BaseLayer.GetNodeCount - 1)

        'set position of jitter layer nodes

        Dim XY_dbl As Matrix3D.XY_dbl

        If A > 0 Then
            For B = 0 To A
                tmpNodeA = JitterLayer.GetNode(B)
                C = 0
                XY_dbl.X = 0.0#
                XY_dbl.Y = 0.0#
                CalcNodePos(tmpNodeA, XY_dbl, C)
                NodeTag = New clsNodeTag
                NodeTag.Pos.X = CInt(XY_dbl.X / C)
                NodeTag.Pos.Y = CInt(XY_dbl.Y / C)
                tmpNodeA.Tag = NodeTag
            Next
        End If

        'set node heights

        Dim BestConnection As clsConnection
        Dim BestNode As clsPassageNode

        For A = 0 To JitterLayer.GetNodeCount - 1
            NodeTag = CType(JitterLayer.GetNode(A).Tag, clsNodeTag)
            NodeLevel(A) = -1
            BestDist = Single.MaxValue
            BestConnection = Nothing
            BestNode = Nothing
            For B = 0 To ConnectionCount - 1
                'If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                If Connections(B).PassageNodeA.Level = Connections(B).PassageNodeB.Level Then
                    'only do this if the levels are the same
                    'this is to make sure nodes that are connected are actually connected on the terrain
                    XY_int = PointGetClosestPosOnLine(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos, NodeTag.Pos)
                    Dist = CSng((XY_int - NodeTag.Pos).ToDoubles.GetMagnitude)
                    If Dist < BestDist Then
                        BestDist = Dist
                        If (NodeTag.Pos - Connections(B).PassageNodeA.Pos).ToDoubles.GetMagnitude <= (NodeTag.Pos - Connections(B).PassageNodeB.Pos).ToDoubles.GetMagnitude Then
                            BestNode = Connections(B).PassageNodeA
                        Else
                            BestNode = Connections(B).PassageNodeB
                        End If
                        Flag = True
                    End If
                End If
            Next
            For C = 0 To PassageNodeCount - 1
                'If Not PassageNodesA(C).IsOnBorder Then
                For D = 0 To SymmetryBlockCount - 1
                    Dist = CSng((NodeTag.Pos - PassageNodes(D, C).Pos).ToDoubles.GetMagnitude)
                    If Dist < BestDist Then
                        BestDist = Dist
                        BestNode = PassageNodes(D, C)
                        Flag = True
                    End If
                Next
                'End If
            Next
            If Flag Then
                NodeLevel(A) = BestNode.Level
            Else
                NodeLevel(A) = BestConnection.PassageNodeA.Level
            End If
            If NodeLevel(A) < 0 Then
                ReturnResult.ProblemAdd("Error: Node height is not set.")
                Return ReturnResult
            End If
        Next

        For A = 0 To LevelCount - 1
            For B = 0 To JitterLayer.GetNodeCount - 1
                If NodeLevel(B) >= A Then
                    SetBaseLevel(JitterLayer.GetNode(B), A, BaseNodeLevel)
                End If
            Next
        Next

        'make ramp slopes

        Dim MinRampLength As Integer = CInt(LevelHeight * Map.HeightMultiplier * 2.0#) + 128
        Dim RampArgs As New clsSetBaseLevelRampArgs
        RampArgs.BaseLevel = BaseNodeLevel
        RampArgs.RampRadius = 320.0F
        For B = 0 To ConnectionCount - 1
            RampArgs.Connection = Connections(B)
            RampArgs.RampLength = Math.Max(CInt((Connections(B).PassageNodeA.Pos - Connections(B).PassageNodeB.Pos).ToDoubles.GetMagnitude * 0.75#), MinRampLength * Math.Abs(Connections(B).PassageNodeA.Level - Connections(B).PassageNodeB.Level))
            For A = 0 To JitterLayer.GetNodeCount - 1
                If Connections(B).IsRamp Then
                    NodeTag = CType(JitterLayer.GetNode(A).Tag, clsNodeTag)
                    XY_int = PointGetClosestPosOnLine(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos, NodeTag.Pos)
                    Dist = CSng((XY_int - NodeTag.Pos).ToDoubles.GetMagnitude)
                    If Dist < RampArgs.RampLength * 2.0F Then
                        SetBaseLevelRamp(RampArgs, JitterLayer.GetNode(A))
                    End If
                End If
            Next
        Next

        For A = 0 To BaseLayer.GetNodeCount - 1
            NodeTag = CType(BaseLayer.GetNode(A).Tag, clsNodeTag)
            Map.Terrain.Vertices(CInt(NodeTag.Pos.X / 128.0F), CInt(NodeTag.Pos.Y / 128.0F)).Height = CByte(BaseNodeLevel.NodeLevels(A) * LevelHeight)
        Next

        Return ReturnResult
    End Function

    Public Sub GenerateTilePathMap()
        Dim NodeTag As clsNodeTag
        Dim tmpNodeA As PathfinderNode
        Dim tmpNodeB As PathfinderNode
        Dim X As Integer
        Dim Y As Integer

        TilePathMap = New PathfinderNetwork

        For Y = 0 To Map.Terrain.TileSize.Y - 1
            For X = 0 To Map.Terrain.TileSize.X - 1
                GenerateTerrainTiles(X, Y) = New GenerateTerrainTile
                GenerateTerrainTiles(X, Y).Node = New PathfinderNode(TilePathMap)
                NodeTag = New clsNodeTag
                NodeTag.Pos = New sXY_int(CInt((X + 0.5#) * 128.0#), CInt((Y + 0.5#) * 128.0#))
                GenerateTerrainTiles(X, Y).Node.Tag = NodeTag
            Next
        Next
        For Y = 0 To Map.Terrain.TileSize.Y - 1
            For X = 0 To Map.Terrain.TileSize.X - 1
                tmpNodeA = GenerateTerrainTiles(X, Y).Node
                If X > 0 Then
                    tmpNodeB = GenerateTerrainTiles(X - 1, Y).Node
                    GenerateTerrainTiles(X, Y).LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y > 0 Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainTiles(X - 1, Y - 1).Node
                        GenerateTerrainTiles(X, Y).TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainTiles(X, Y - 1).Node
                    GenerateTerrainTiles(X, Y).TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < Map.Terrain.TileSize.X - 1 Then
                        tmpNodeB = GenerateTerrainTiles(X + 1, Y - 1).Node
                        GenerateTerrainTiles(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
                If X < Map.Terrain.TileSize.X - 1 Then
                    tmpNodeB = GenerateTerrainTiles(X + 1, Y).Node
                    GenerateTerrainTiles(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y < Map.Terrain.TileSize.Y - 1 Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainTiles(X - 1, Y + 1).Node
                        GenerateTerrainTiles(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainTiles(X, Y + 1).Node
                    GenerateTerrainTiles(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < Map.Terrain.TileSize.X - 1 Then
                        tmpNodeB = GenerateTerrainTiles(X + 1, Y + 1).Node
                        GenerateTerrainTiles(X, Y).BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
            Next
        Next

        TilePathMap.LargeArraysResize()
        TilePathMap.FindCalc()
    End Sub

    Private Class clsPassageNodeLevels
        Public Nodes() As Integer
    End Class

    Private Sub PassageNodesMinLevelSet(PassageNode As clsPassageNode, PassageNodesMinLevel As clsPassageNodeLevels, Level As Integer, LevelChange As Integer)
        Dim A As Integer
        Dim tmpPassageNode As clsPassageNode

        If Level > PassageNodesMinLevel.Nodes(PassageNode.Num) Then
            PassageNodesMinLevel.Nodes(PassageNode.Num) = Level
            For A = 0 To PassageNode.ConnectionCount - 1
                tmpPassageNode = PassageNode.Connections(A).GetOther
                If PassageNode.IsNearBorder Or tmpPassageNode.IsNearBorder Then
                    PassageNodesMinLevelSet(tmpPassageNode, PassageNodesMinLevel, Level - 1, LevelChange)
                Else
                    PassageNodesMinLevelSet(tmpPassageNode, PassageNodesMinLevel, Level - LevelChange, LevelChange)
                End If
            Next
        End If
    End Sub

    Private Sub PassageNodesMaxLevelSet(PassageNode As clsPassageNode, PassageNodesMaxLevel As clsPassageNodeLevels, Level As Integer, LevelChange As Integer)
        Dim A As Integer
        Dim tmpPassageNode As clsPassageNode

        If Level < PassageNodesMaxLevel.Nodes(PassageNode.Num) Then
            PassageNodesMaxLevel.Nodes(PassageNode.Num) = Level
            For A = 0 To PassageNode.ConnectionCount - 1
                tmpPassageNode = PassageNode.Connections(A).GetOther
                If PassageNode.IsNearBorder Or tmpPassageNode.IsNearBorder Then
                    PassageNodesMaxLevelSet(tmpPassageNode, PassageNodesMaxLevel, Level + 1, LevelChange)
                Else
                    PassageNodesMaxLevelSet(tmpPassageNode, PassageNodesMaxLevel, Level + LevelChange, LevelChange)
                End If
            Next
        End If
    End Sub

    Private Class clsNodeConnectedness
        Public NodeConnectedness() As Single
        Public PassageNodeVisited(,) As Boolean
        Public PassageNodePathMap As PathfinderNetwork
        Public PassageNodePathNodes(,) As PathfinderNode
    End Class

    Private Class clsUpdateNodeConnectednessArgs
        Public OriginalNode As clsPassageNode
        Public Args As clsNodeConnectedness
    End Class

    Private Sub UpdateNodeConnectedness(Args As clsUpdateNodeConnectednessArgs, PassageNode As clsPassageNode)
        Dim A As Integer
        Dim tmpConnection As clsConnection
        Dim tmpOtherNode As clsPassageNode
        Dim PassableCount As Integer

        Args.Args.PassageNodeVisited(PassageNode.MirrorNum, PassageNode.Num) = True

        For A = 0 To PassageNode.ConnectionCount - 1
            tmpConnection = PassageNode.Connections(A).Connection
            If Not (tmpConnection.PassageNodeA.IsOnBorder Or tmpConnection.PassageNodeB.IsOnBorder Or tmpConnection.PassageNodeA.IsWater Or tmpConnection.PassageNodeB.IsWater) And (tmpConnection.IsRamp Or tmpConnection.PassageNodeA.Level = tmpConnection.PassageNodeB.Level) Then
                tmpOtherNode = PassageNode.Connections(A).GetOther
                If Not Args.Args.PassageNodeVisited(tmpOtherNode.MirrorNum, tmpOtherNode.Num) Then
                    UpdateNodeConnectedness(Args, tmpOtherNode)
                End If
                PassableCount += 1
            End If
        Next

        Dim Paths() As PathfinderNetwork.PathList
        Dim StartNodes(0) As PathfinderNode
        StartNodes(0) = Args.Args.PassageNodePathNodes(0, Args.OriginalNode.Num)
        Paths = Args.Args.PassageNodePathMap.GetPath(StartNodes, Args.Args.PassageNodePathNodes(PassageNode.MirrorNum, PassageNode.Num), -1, 0)
        Args.Args.NodeConnectedness(Args.OriginalNode.Num) += CSng(PassableCount * 0.999# ^ Paths(0).Paths(0).Value)
    End Sub

    Private Class clsUpdateNetworkConnectednessArgs
        Public PassageNodeUpdated() As Boolean
        Public SymmetryBlockCount As Integer
        Public Args As clsNodeConnectedness
    End Class

    Private Sub UpdateNetworkConnectedness(Args As clsUpdateNetworkConnectednessArgs, PassageNode As clsPassageNode)
        Dim A As Integer
        Dim tmpConnection As clsConnection
        Dim tmpOtherNode As clsPassageNode
        Dim NodeConnectednessArgs As New clsUpdateNodeConnectednessArgs
        Dim B As Integer
        Dim C As Integer

        Args.PassageNodeUpdated(PassageNode.Num) = True

        For A = 0 To PassageNode.ConnectionCount - 1
            tmpConnection = PassageNode.Connections(A).Connection
            If Not (tmpConnection.PassageNodeA.IsOnBorder Or tmpConnection.PassageNodeB.IsOnBorder Or tmpConnection.PassageNodeA.IsWater Or tmpConnection.PassageNodeB.IsWater) And (tmpConnection.IsRamp Or tmpConnection.PassageNodeA.Level = tmpConnection.PassageNodeB.Level) Then
                tmpOtherNode = PassageNode.Connections(A).GetOther
                If Not Args.PassageNodeUpdated(tmpOtherNode.Num) And tmpOtherNode.MirrorNum = 0 Then
                    For B = 0 To PassageNodeCount - 1
                        For C = 0 To Args.SymmetryBlockCount - 1
                            Args.Args.PassageNodeVisited(C, B) = False
                        Next
                    Next
                    NodeConnectednessArgs.OriginalNode = PassageNode
                    NodeConnectednessArgs.Args = Args.Args
                    UpdateNodeConnectedness(NodeConnectednessArgs, PassageNode)
                End If
            End If
        Next
    End Sub

    Private Class clsOilBalanceLoopArgs
        Public OilNodes() As clsPassageNode
        Public OilClusterSizes() As Integer
        Public OilPossibilities As clsGenerateMap.clsOilPossibilities
        Public PlayerOilScore() As Double
    End Class

    Private Sub OilBalanceLoop(Args As clsOilBalanceLoopArgs, LoopNum As Integer)
        Dim A As Integer
        Dim C As Integer
        Dim NextLoopNum As Integer = LoopNum + 1
        Dim tmpPassageNodeA As clsPassageNode

        For A = 0 To PassageNodeCount - 1
            tmpPassageNodeA = PassageNodes(0, A)
            If tmpPassageNodeA.PlayerBaseNum < 0 And Not tmpPassageNodeA.IsOnBorder And tmpPassageNodeA.OilCount = 0 And Not tmpPassageNodeA.IsWater Then
                For C = 0 To LoopNum - 1
                    If Args.OilNodes(C) Is tmpPassageNodeA Then
                        Exit For
                    End If
                Next
                If C = LoopNum Then
                    Args.OilNodes(LoopNum) = tmpPassageNodeA
                    If NextLoopNum < OilAtATime Then
                        OilBalanceLoop(Args, NextLoopNum)
                    Else
                        OilValueCalc(Args)
                    End If
                End If
            End If
        Next
    End Sub

    Public Class clsOilPossibilities
        Public Class clsPossibility
            Public Nodes() As clsPassageNode
            Public Score As Double
            Public PlayerOilScoreAddition() As Double
        End Class
        Public BestPossibility As clsPossibility

        Public Sub NewPossibility(Possibility As clsPossibility)

            If BestPossibility Is Nothing Then
                BestPossibility = Possibility
            ElseIf Possibility.Score < BestPossibility.Score Then
                BestPossibility = Possibility
            End If
        End Sub
    End Class

    Private Sub OilValueCalc(Args As clsOilBalanceLoopArgs)
        'Dim OilDistScore As Double
        'Dim OilStraightDistScore As Double
        Dim LowestScore As Double
        Dim HighestScore As Double
        'Dim TotalOilScore As Double
        Dim UnbalancedScore As Double
        Dim dblTemp As Double
        Dim Value As Double
        Dim NewPossibility As New clsGenerateMap.clsOilPossibilities.clsPossibility
        Dim BaseOilScore(TopLeftPlayerCount - 1) As Double

        ReDim NewPossibility.PlayerOilScoreAddition(TopLeftPlayerCount - 1)

        Dim NewOilNum As Integer
        Dim OtherOilNum As Integer
        Dim NewOilNodeNum As Integer
        Dim OtherOilNodeNum As Integer
        Dim SymmetryBlockNum As Integer
        Dim MapNodeNum As Integer
        Dim PlayerNum As Integer
        'Dim NewOilCount As Integer
        Dim OilMassMultiplier As Double
        Dim OilDistValue As Double
        Dim NearestOilValue As Double = Double.MaxValue

        'OilDistScore = 0.0#
        'OilStraightDistScore = 0.0#
        For PlayerNum = 0 To TopLeftPlayerCount - 1
            NewPossibility.PlayerOilScoreAddition(PlayerNum) = 0.0#
        Next
        For NewOilNum = 0 To OilAtATime - 1
            NewOilNodeNum = Args.OilNodes(NewOilNum).Num
            'other oil to be placed in the first area
            For OtherOilNum = NewOilNum + 1 To OilAtATime - 1
                OtherOilNodeNum = Args.OilNodes(OtherOilNum).Num
                'OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * Args.OilClusterSizes(OtherOilNum)
                'OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, 0, OtherOilNodeNum)
                'OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(0, OtherOilNodeNum).Pos)
                OilDistValue = (4.0# * PassageNodeDists(0, NewOilNodeNum, 0, OtherOilNodeNum)) '+ GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(0, OtherOilNodeNum).Pos))
                If OilDistValue < NearestOilValue Then
                    NearestOilValue = OilDistValue
                End If
            Next
            'other oil to be placed in symmetrical areas
            For OtherOilNum = 0 To OilAtATime - 1
                OtherOilNodeNum = Args.OilNodes(OtherOilNum).Num
                'OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * Args.OilClusterSizes(OtherOilNum)
                For SymmetryBlockNum = 1 To SymmetryBlockCount - 1
                    'OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, OtherOilNodeNum)
                    'OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, OtherOilNodeNum).Pos)
                    OilDistValue = (4.0# * PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, OtherOilNodeNum)) ' + GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, OtherOilNodeNum).Pos))
                    If OilDistValue < NearestOilValue Then
                        NearestOilValue = OilDistValue
                    End If
                Next
            Next
            'oil on the map
            For MapNodeNum = 0 To PassageNodeCount - 1
                For SymmetryBlockNum = 0 To SymmetryBlockCount - 1
                    If PassageNodes(SymmetryBlockNum, MapNodeNum).OilCount > 0 Then
                        'OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * PassageNodes(SymmetryBlockNum, MapNodeNum).OilCount
                        'OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, MapNodeNum)
                        'OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, MapNodeNum).Pos)
                        OilDistValue = (4.0# * OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, MapNodeNum)) ' + GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, MapNodeNum).Pos))
                        If OilDistValue < NearestOilValue Then
                            NearestOilValue = OilDistValue
                        End If
                    End If
                Next
            Next
            'extra oil score for players
            For PlayerNum = 0 To TopLeftPlayerCount - 1
                BaseOilScore(PlayerNum) = 0.0#
                For SymmetryBlockNum = 0 To SymmetryBlockCount - 1
                    dblTemp = PassageNodeDists(0, PlayerBases(PlayerNum).Nodes(0).Num, SymmetryBlockNum, NewOilNodeNum) * 2.0# + (PlayerBases(PlayerNum).Nodes(0).Pos - PassageNodes(SymmetryBlockNum, NewOilNodeNum).Pos).ToDoubles.GetMagnitude
                    BaseOilScore(PlayerNum) += 100.0# / dblTemp
                Next
            Next
            'TotalOilScore = 0.0#
            'For PlayerNum = 0 To PlayerCount - 1
            '    TotalOilScore += BaseOilScore(PlayerNum)
            'Next
            For PlayerNum = 0 To TopLeftPlayerCount - 1
                NewPossibility.PlayerOilScoreAddition(PlayerNum) += Args.OilClusterSizes(NewOilNum) * BaseOilScore(PlayerNum)
            Next
        Next

        LowestScore = Double.MaxValue
        HighestScore = Double.MinValue
        For PlayerNum = 0 To TopLeftPlayerCount - 1
            dblTemp = Args.PlayerOilScore(PlayerNum) + NewPossibility.PlayerOilScoreAddition(PlayerNum)
            If dblTemp < LowestScore Then
                LowestScore = dblTemp
            End If
            If dblTemp > HighestScore Then
                HighestScore = dblTemp
            End If
        Next
        UnbalancedScore = HighestScore - LowestScore

        'NewOilCount = 0
        'For NewOilNum = 0 To OilAtATime - 1
        '    NewOilCount += Args.OilClusterSizes(NewOilNum)
        'Next
        'divide all dists by the number of oil resources placed. does not include other symmetries, since they were never added in, and are exactly the same.
        If NearestOilValue = Double.MaxValue Then
            NearestOilValue = 0.0#
        Else
            NearestOilValue = 10.0# / NearestOilValue
        End If
        'Value = OilDispersion * (OilDistScore * 4.0# + OilStraightDistScore) + UnbalancedScore
        Value = OilDispersion * NearestOilValue + UnbalancedScore
        NewPossibility.Score = Value
        ReDim NewPossibility.Nodes(OilAtATime - 1)
        For NewOilNum = 0 To OilAtATime - 1
            NewPossibility.Nodes(NewOilNum) = Args.OilNodes(NewOilNum)
        Next
        Args.OilPossibilities.NewPossibility(NewPossibility)
    End Sub

    Private Class clsBaseNodeLevels
        Public NodeLevels() As Single
    End Class

    Private Sub SetBaseLevel(Node As PathfinderNode, NewLevel As Integer, BaseLevel As clsBaseNodeLevels)

        If Node.GetChildNodeCount = 0 Then
            Dim A As Integer
            Dim Height As Single
            Dim Lowest As Single = NewLevel
            For A = 0 To Node.GetConnectionCount - 1
                Height = BaseLevel.NodeLevels(Node.GetConnection(A).GetOtherNode(Node).GetLayer_NodeNum)
                If Height < Lowest Then
                    Lowest = Height
                End If
            Next
            If NewLevel - Lowest > 1.0F Then
                BaseLevel.NodeLevels(Node.GetLayer_NodeNum) = Lowest + 1.0F
            Else
                BaseLevel.NodeLevels(Node.GetLayer_NodeNum) = NewLevel
            End If
        Else
            Dim A As Integer
            For A = 0 To Node.GetChildNodeCount - 1
                SetBaseLevel(Node.GetChildNode(A), NewLevel, BaseLevel)
            Next
        End If
    End Sub

    Private Class clsSetBaseLevelRampArgs
        Public Connection As clsConnection
        Public BaseLevel As New clsBaseNodeLevels
        Public RampLength As Integer
        Public RampRadius As Single
    End Class

    Private Sub SetBaseLevelRamp(Args As clsSetBaseLevelRampArgs, Node As PathfinderNode)

        If Node.GetChildNodeCount = 0 Then
            Dim NodeTag As clsNodeTag = CType(Node.Tag, clsNodeTag)
            Dim XY_int As sXY_int = PointGetClosestPosOnLine(Args.Connection.PassageNodeA.Pos, Args.Connection.PassageNodeB.Pos, NodeTag.Pos)
            Dim ConnectionLength As Single = CSng((Args.Connection.PassageNodeA.Pos - Args.Connection.PassageNodeB.Pos).ToDoubles.GetMagnitude)
            Dim Extra As Single = ConnectionLength - Args.RampLength
            Dim ConnectionPos As Single = CSng((XY_int - Args.Connection.PassageNodeA.Pos).ToDoubles.GetMagnitude)
            Dim RampPos As Single = Clamp_sng((ConnectionPos - Extra / 2.0F) / Args.RampLength, 0.0F, 1.0F)
            Dim Layer_NodeNum As Integer = Node.GetLayer_NodeNum
            RampPos = CSng(1.0# - (Math.Cos(RampPos * Math.PI) + 1.0#) / 2.0#)
            If RampPos > 0.0F And RampPos < 1.0F Then
                Dim Dist2 As Single = CSng((NodeTag.Pos - XY_int).ToDoubles.GetMagnitude)
                If Dist2 < Args.RampRadius Then
                    Dim Dist2Factor As Single = 1.0F 'Math.Min(3.0F - 3.0F * Dist2 / 384.0F, 1.0F) 'distance fading
                    If Args.BaseLevel.NodeLevels(Layer_NodeNum) = Int(Args.BaseLevel.NodeLevels(Layer_NodeNum)) Then
                        Args.BaseLevel.NodeLevels(Layer_NodeNum) = Args.BaseLevel.NodeLevels(Layer_NodeNum) * (1.0F - Dist2Factor) + (Args.Connection.PassageNodeA.Level * (1.0F - RampPos) + Args.Connection.PassageNodeB.Level * RampPos) * Dist2Factor
                    Else
                        Args.BaseLevel.NodeLevels(Layer_NodeNum) = (Args.BaseLevel.NodeLevels(Layer_NodeNum) * (2.0F - Dist2Factor) + (Args.Connection.PassageNodeA.Level * (1.0F - RampPos) + Args.Connection.PassageNodeB.Level * RampPos) * Dist2Factor) / 2.0F
                    End If
                End If
            End If
        Else
            Dim A As Integer
            For A = 0 To Node.GetChildNodeCount - 1
                SetBaseLevelRamp(Args, Node.GetChildNode(A))
            Next
        End If
    End Sub

    Public Sub TerrainBlockPaths()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Map.Terrain.TileSize.Y - 1
            For X = 0 To Map.Terrain.TileSize.X - 1
                If Map.Terrain.Tiles(X, Y).Texture.TextureNum >= 0 Then
                    If GenerateTileset.Tileset.Tiles(Map.Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Cliff Or GenerateTileset.Tileset.Tiles(Map.Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                        TileNodeBlock(X, Y)
                    End If
                End If
            Next
        Next
        TilePathMap.FindCalc()
    End Sub

    Public Function GetWaterMap() As clsBooleanMap
        Dim ReturnResult As New clsBooleanMap
        Dim BestDist As Single
        Dim BestIsWater As Boolean
        Dim Pos As sXY_int
        Dim Dist As Single
        Dim B As Integer
        Dim C As Integer
        Dim XY_int As sXY_int
        Dim X As Integer
        Dim Y As Integer

        ReturnResult.Blank(Map.Terrain.TileSize.X + 1, Map.Terrain.TileSize.Y + 1)
        For Y = 0 To Map.Terrain.TileSize.Y
            For X = 0 To Map.Terrain.TileSize.X
                BestDist = Single.MaxValue
                Pos = New sXY_int(X * TerrainGridSpacing, Y * TerrainGridSpacing)
                For B = 0 To ConnectionCount - 1
                    'If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                    If Connections(B).PassageNodeA.IsWater = Connections(B).PassageNodeB.IsWater Then
                        'only do this if the waters are the same
                        'this is to make sure nodes that are connected are actually connected as water
                        XY_int = PointGetClosestPosOnLine(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos, Pos)
                        Dist = CSng((XY_int - Pos).ToDoubles.GetMagnitude)
                        If Dist < BestDist Then
                            BestDist = Dist
                            If (Pos - Connections(B).PassageNodeA.Pos).ToDoubles.GetMagnitude <= (Pos - Connections(B).PassageNodeB.Pos).ToDoubles.GetMagnitude Then
                                BestIsWater = Connections(B).PassageNodeA.IsWater
                            Else
                                BestIsWater = Connections(B).PassageNodeB.IsWater
                            End If
                        End If
                    End If
                Next
                For C = 0 To PassageNodeCount - 1
                    For B = 0 To SymmetryBlockCount - 1
                        Dist = CSng((Pos - PassageNodes(B, C).Pos).ToDoubles.GetMagnitude)
                        If Dist < BestDist Then
                            BestDist = Dist
                            BestIsWater = PassageNodes(B, C).IsWater
                        End If
                    Next
                Next
                ReturnResult.ValueData.Value(Y, X) = BestIsWater
            Next
        Next
        Return ReturnResult
    End Function

    Public Function GetNearestNode(Network As PathfinderNetwork, Pos As sXY_int, MinClearance As Integer) As PathfinderNode
        Dim A As Integer
        Dim Dist As Double
        Dim tmpNode As PathfinderNode
        Dim BestNode As PathfinderNode
        Dim BestDist As Double
        Dim tmpNodeTag As clsNodeTag

        BestDist = Double.MaxValue
        BestNode = Nothing
        For A = 0 To Network.GetNodeLayer(0).GetNodeCount - 1
            tmpNode = Network.GetNodeLayer(0).GetNode(A)
            If tmpNode.GetClearance >= MinClearance Then
                tmpNodeTag = CType(tmpNode.Tag, clsNodeTag)
                Dist = (tmpNodeTag.Pos - Pos).ToDoubles.GetMagnitude
                If Dist < BestDist Then
                    BestDist = Dist
                    BestNode = tmpNode
                End If
            End If
        Next
        Return BestNode
    End Function

    Private Function GetNearestNodeConnection(Network As PathfinderNetwork, Pos As sXY_int, MinClearance As Integer, MaxDistance As Single) As PathfinderNode
        Dim A As Integer
        Dim TravelNodes(Network.GetNodeLayer(0).GetNodeCount * 10 - 1) As PathfinderNode
        Dim TravelNodeCount As Integer
        Dim NodeTravelDists(Network.GetNodeLayer(0).GetNodeCount - 1) As Single
        Dim TravelNodeNum As Integer = 0
        Dim CurrentNode As PathfinderNode
        Dim OtherNode As PathfinderNode
        Dim tmpConnection As PathfinderConnection
        Dim BestNode As PathfinderNode = Nothing
        Dim TravelDist As Single
        Dim Flag As Boolean

        For A = 0 To Network.GetNodeLayer(0).GetNodeCount - 1
            NodeTravelDists(A) = Single.MaxValue
        Next
        TravelNodes(0) = GetNearestNode(Network, Pos, 1)
        If TravelNodes(0) Is Nothing Then
            Return Nothing
        End If
        TravelNodeCount = 1
        NodeTravelDists(TravelNodes(0).Layer_NodeNum) = 0.0F
        Do While TravelNodeNum < TravelNodeCount
            CurrentNode = TravelNodes(TravelNodeNum)
            If CurrentNode.Clearance >= MinClearance Then
                If BestNode Is Nothing Then
                    BestNode = CurrentNode
                ElseIf NodeTravelDists(CurrentNode.Layer_NodeNum) < NodeTravelDists(BestNode.Layer_NodeNum) Then
                    BestNode = CurrentNode
                End If
            End If
            For A = 0 To CurrentNode.GetConnectionCount - 1
                tmpConnection = CurrentNode.GetConnection(A)
                OtherNode = tmpConnection.GetOtherNode(CurrentNode)
                TravelDist = NodeTravelDists(CurrentNode.Layer_NodeNum) + tmpConnection.GetValue
                If BestNode Is Nothing Then
                    Flag = True
                ElseIf TravelDist < NodeTravelDists(BestNode.Layer_NodeNum) Then
                    Flag = True
                Else
                    Flag = False
                End If
                If Flag And TravelDist < NodeTravelDists(OtherNode.Layer_NodeNum) Then
                    NodeTravelDists(OtherNode.Layer_NodeNum) = TravelDist
                    TravelNodes(TravelNodeCount) = OtherNode
                    TravelNodeCount += 1
                End If
            Next
            TravelNodeNum += 1
        Loop
        Return BestNode
    End Function

    Public Function PlaceUnitNear(Type As clsUnitType, Pos As sXY_int, UnitGroup As clsMap.clsUnitGroup, Clearance As Integer, Rotation As Integer, MaxDistFromPos As Integer) As clsMap.clsUnit
        Dim PosNode As PathfinderNode
        Dim NodeTag As clsNodeTag
        Dim FinalTilePos As sXY_int
        Dim TilePosA As sXY_int
        Dim TilePosB As sXY_int
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim Remainder As Integer
        Dim Footprint As sXY_int

        PosNode = GetNearestNodeConnection(TilePathMap, Pos, Clearance, MaxDistFromPos)
        If PosNode IsNot Nothing Then
            NodeTag = CType(PosNode.Tag, clsNodeTag)
            If (Pos - NodeTag.Pos).ToDoubles.GetMagnitude <= MaxDistFromPos Then

                Dim NewUnitAdd As New clsMap.clsUnitAdd
                NewUnitAdd.Map = Map
                NewUnitAdd.StoreChange = True
                Dim NewUnit As New clsMap.clsUnit
                NewUnitAdd.NewUnit = NewUnit
                NewUnit.Type = Type
                NewUnit.UnitGroup = UnitGroup

                FinalTilePos.X = CInt(Int(NodeTag.Pos.X / TerrainGridSpacing))
                FinalTilePos.Y = CInt(Int(NodeTag.Pos.Y / TerrainGridSpacing))
                Footprint = Type.GetFootprintSelected(Rotation)
                Remainder = Footprint.X Mod 2
                If Remainder > 0 Then
                    NewUnit.Pos.Horizontal.X = NodeTag.Pos.X
                Else
                    If Rnd() >= 0.5F Then
                        NewUnit.Pos.Horizontal.X = NodeTag.Pos.X - CInt(TerrainGridSpacing / 2.0#)
                    Else
                        NewUnit.Pos.Horizontal.X = NodeTag.Pos.X + CInt(TerrainGridSpacing / 2.0#)
                    End If
                End If
                Remainder = Footprint.Y Mod 2
                If Remainder > 0 Then
                    NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y
                Else
                    If Rnd() >= 0.5F Then
                        NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y - CInt(TerrainGridSpacing / 2.0#)
                    Else
                        NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y + CInt(TerrainGridSpacing / 2.0#)
                    End If
                End If
                TilePosA.X = CInt(Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.5#))
                TilePosA.Y = CInt(Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.5#))
                TilePosB.X = CInt(Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing + Footprint.X / 2.0# - 0.5#))
                TilePosB.Y = CInt(Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing + Footprint.Y / 2.0# - 0.5#))
                NewUnit.Rotation = Rotation

                NewUnitAdd.Perform()

                For Y2 = Math.Max(TilePosA.Y, 0) To Math.Min(TilePosB.Y, Map.Terrain.TileSize.Y - 1)
                    For X2 = Math.Max(TilePosA.X, 0) To Math.Min(TilePosB.X, Map.Terrain.TileSize.X - 1)
                        TileNodeBlock(X2, Y2)
                    Next
                Next

                TilePathMap.FindCalc()

                Return NewUnit
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Function PlaceUnit(Type As clsUnitType, Pos As sWorldPos, UnitGroup As clsMap.clsUnitGroup, Rotation As Integer) As clsMap.clsUnit
        Dim TilePosA As sXY_int
        Dim TilePosB As sXY_int
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim FinalTilePos As sXY_int
        Dim Footprint As sXY_int

        Dim NewUnitAdd As New clsMap.clsUnitAdd
        NewUnitAdd.Map = Map
        NewUnitAdd.StoreChange = True
        Dim NewUnit As New clsMap.clsUnit
        NewUnitAdd.NewUnit = NewUnit
        NewUnit.Type = Type
        NewUnit.UnitGroup = UnitGroup

        FinalTilePos.X = CInt(Int(Pos.Horizontal.X / TerrainGridSpacing))
        FinalTilePos.Y = CInt(Int(Pos.Horizontal.Y / TerrainGridSpacing))

        Footprint = Type.GetFootprintSelected(Rotation)

        NewUnit.Pos = Pos
        TilePosA.X = CInt(Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.5#))
        TilePosA.Y = CInt(Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.5#))
        TilePosB.X = CInt(Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing + Footprint.X / 2.0# - 0.5#))
        TilePosB.Y = CInt(Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing + Footprint.Y / 2.0# - 0.5#))
        NewUnit.Rotation = Rotation

        NewUnitAdd.Perform()

        For Y2 = Math.Max(TilePosA.Y, 0) To Math.Min(TilePosB.Y, Map.Terrain.TileSize.Y - 1)
            For X2 = Math.Max(TilePosA.X, 0) To Math.Min(TilePosB.X, Map.Terrain.TileSize.X - 1)
                TileNodeBlock(X2, Y2)
            Next
        Next

        TilePathMap.FindCalc()

        Return NewUnit
    End Function

    Public Sub TileNodeBlock(X As Integer, Y As Integer)
        Dim X2 As Integer
        Dim Y2 As Integer

        For Y2 = Math.Max(Y - 6, 0) To Math.Min(Y + 6, Map.Terrain.TileSize.Y - 1)
            For X2 = Math.Max(X - 6, 0) To Math.Min(X + 6, Map.Terrain.TileSize.X - 1)
                GenerateTerrainTiles(X2, Y2).Node.ClearanceSet(Math.Min(GenerateTerrainTiles(X2, Y2).Node.GetClearance, Math.Max(Math.Abs(Y2 - Y), Math.Abs(X2 - X))))
            Next
        Next

        If GenerateTerrainTiles(X, Y).TopLeftLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).TopLeftLink.Destroy()
            GenerateTerrainTiles(X, Y).TopLeftLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).TopLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).TopLink.Destroy()
            GenerateTerrainTiles(X, Y).TopLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).TopRightLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).TopRightLink.Destroy()
            GenerateTerrainTiles(X, Y).TopRightLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).RightLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).RightLink.Destroy()
            GenerateTerrainTiles(X, Y).RightLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).BottomRightLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).BottomRightLink.Destroy()
            GenerateTerrainTiles(X, Y).BottomRightLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).BottomLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).BottomLink.Destroy()
            GenerateTerrainTiles(X, Y).BottomLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).BottomLeftLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).BottomLeftLink.Destroy()
            GenerateTerrainTiles(X, Y).BottomLeftLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).LeftLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).LeftLink.Destroy()
            GenerateTerrainTiles(X, Y).LeftLink = Nothing
        End If
    End Sub

    Public Sub BlockEdgeTiles()
        Dim X As Integer
        Dim Y As Integer
        Dim Terrain As clsMap.clsTerrain = Map.Terrain

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To 2
                TileNodeBlock(X, Y)
            Next
            For X = Terrain.TileSize.X - 4 To Terrain.TileSize.X - 1
                TileNodeBlock(X, Y)
            Next
        Next
        For X = 3 To Terrain.TileSize.X - 5
            For Y = 0 To 2
                TileNodeBlock(X, Y)
            Next
            For Y = Terrain.TileSize.Y - 4 To Terrain.TileSize.Y - 1
                TileNodeBlock(X, Y)
            Next
        Next
        TilePathMap.FindCalc()
    End Sub

    Public Function GenerateUnits() As clsResult
        Dim ReturnResult As New clsResult("Objects")

        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim tmpUnit As clsMap.clsUnit
        Dim Count As Integer
        Dim FeaturePlaceRange As Integer = 6 * 128
        Dim BasePlaceRange As Integer = 16 * 128
        Dim TilePos As sXY_int
        Dim AverageHeight As Byte
        Dim PlayerNum As Integer
        Dim Terrain As clsMap.clsTerrain = Map.Terrain

        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                PassageNodes(B, A).HasFeatureCluster = False
            Next
        Next

        For A = 0 To TotalPlayerCount - 1
            PlayerNum = A
            tmpUnit = PlaceUnitNear(UnitType_CommandCentre, PlayerBases(A).Pos, Map.UnitGroups.Item(PlayerNum), 3, 0, BasePlaceRange)
            If tmpUnit Is Nothing Then
                ReturnResult.ProblemAdd("No room for base structures")
                Return ReturnResult
            End If
            tmpUnit = PlaceUnitNear(UnitType_PowerGenerator, PlayerBases(A).Pos, Map.UnitGroups.Item(PlayerNum), 3, 0, BasePlaceRange)
            If tmpUnit Is Nothing Then
                ReturnResult.ProblemAdd("No room for base structures.")
                Return ReturnResult
            End If
            tmpUnit = PlaceUnit(UnitType_PowerModule, tmpUnit.Pos, Map.UnitGroups.Item(PlayerNum), 0)
            If tmpUnit Is Nothing Then
                ReturnResult.ProblemAdd("No room for module.")
                Return ReturnResult
            End If
            For B = 1 To 2
                tmpUnit = PlaceUnitNear(UnitType_ResearchFacility, PlayerBases(A).Pos, Map.UnitGroups.Item(PlayerNum), 3, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    ReturnResult.ProblemAdd("No room for base structures")
                    Return ReturnResult
                End If
                tmpUnit = PlaceUnit(UnitType_ResearchModule, tmpUnit.Pos, Map.UnitGroups.Item(PlayerNum), 0)
                If tmpUnit Is Nothing Then
                    ReturnResult.ProblemAdd("No room for module.")
                    Return ReturnResult
                End If
            Next
            For B = 1 To 2
                tmpUnit = PlaceUnitNear(UnitType_Factory, PlayerBases(A).Pos, Map.UnitGroups.Item(PlayerNum), 4, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    ReturnResult.ProblemAdd("No room for base structures")
                    Return ReturnResult
                End If
                tmpUnit = PlaceUnit(UnitType_FactoryModule, tmpUnit.Pos, Map.UnitGroups.Item(PlayerNum), 0)
                If tmpUnit Is Nothing Then
                    ReturnResult.ProblemAdd("No room for module.")
                    Return ReturnResult
                End If
            Next
            tmpUnit = PlaceUnitNear(UnitType_CyborgFactory, PlayerBases(A).Pos, Map.UnitGroups.Item(PlayerNum), 3, 0, BasePlaceRange)
            If tmpUnit Is Nothing Then
                ReturnResult.ProblemAdd("No room for base structures")
                Return ReturnResult
            End If
            For B = 1 To BaseTruckCount
                tmpUnit = PlaceUnitNear(UnitType_Truck, PlayerBases(A).Pos, Map.UnitGroups.Item(PlayerNum), 2, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    ReturnResult.ProblemAdd("No room for trucks")
                    Return ReturnResult
                End If
            Next
        Next
        For A = 0 To PassageNodeCount - 1
            For D = 0 To SymmetryBlockCount - 1
                For B = 0 To PassageNodes(D, A).OilCount - 1
                    If PassageNodes(D, A).PlayerBaseNum >= 0 Then
                        tmpUnit = PlaceUnitNear(UnitType_OilResource, PassageNodes(D, A).Pos, Map.ScavengerUnitGroup, 2, 0, BasePlaceRange)
                    Else
                        tmpUnit = PlaceUnitNear(UnitType_OilResource, PassageNodes(D, A).Pos, Map.ScavengerUnitGroup, 2, 0, FeaturePlaceRange)
                    End If
                    If tmpUnit Is Nothing Then
                        ReturnResult.ProblemAdd("No room for oil.")
                        Return ReturnResult
                    End If
                    'flatten ground underneath
                    TilePos.X = CInt(Int(tmpUnit.Pos.Horizontal.X / TerrainGridSpacing))
                    TilePos.Y = CInt(Int(tmpUnit.Pos.Horizontal.Y / TerrainGridSpacing))
                    AverageHeight = CByte((CInt(Terrain.Vertices(TilePos.X, TilePos.Y).Height) + CInt(Terrain.Vertices(TilePos.X + 1, TilePos.Y).Height) + CInt(Terrain.Vertices(TilePos.X, TilePos.Y + 1).Height) + CInt(Terrain.Vertices(TilePos.X + 1, TilePos.Y + 1).Height)) / 4.0#)
                    Terrain.Vertices(TilePos.X, TilePos.Y).Height = AverageHeight
                    Terrain.Vertices(TilePos.X + 1, TilePos.Y).Height = AverageHeight
                    Terrain.Vertices(TilePos.X, TilePos.Y + 1).Height = AverageHeight
                    Terrain.Vertices(TilePos.X + 1, TilePos.Y + 1).Height = AverageHeight
                    Map.SectorGraphicsChanges.TileChanged(TilePos)
                    Map.SectorUnitHeightsChanges.TileChanged(TilePos)
                    Map.SectorTerrainUndoChanges.TileChanged(TilePos)
                    tmpUnit.Pos.Altitude = AverageHeight * Map.HeightMultiplier
                    If PassageNodes(D, A).PlayerBaseNum >= 0 Then
                        'place base derrick
                        tmpUnit = PlaceUnit(UnitType_Derrick, tmpUnit.Pos, Map.UnitGroups.Item(PassageNodes(D, A).PlayerBaseNum), 0)
                        If tmpUnit Is Nothing Then
                            ReturnResult.ProblemAdd("No room for derrick.")
                            Return ReturnResult
                        End If
                    End If
                Next
            Next
        Next

        'feature clusters
        For A = 0 To PassageNodeCount - 1
            For D = 0 To SymmetryBlockCount - 1
                If PassageNodes(D, A).PlayerBaseNum < 0 And Not PassageNodes(D, A).IsOnBorder Then
                    PassageNodes(D, A).HasFeatureCluster = (Rnd() < FeatureClusterChance)
                End If
            Next
        Next

        Dim RandNum As UInteger
        Dim uintTemp As UInteger
        Dim tmpNode As PathfinderNode
        Dim E As Integer
        Dim Footprint As sXY_int
        Dim MissingUnitCount As Integer = 0
        Dim Rotation As Integer

        If GenerateTileset.ClusteredUnitChanceTotal > 0 Then
            For A = 0 To PassageNodeCount - 1
                For D = 0 To SymmetryBlockCount - 1
                    If PassageNodes(D, A).HasFeatureCluster Then
                        Count = FeatureClusterMinUnits + CInt(Int(Rnd() * (FeatureClusterMaxUnits - FeatureClusterMinUnits + 1)))
                        For B = 1 To Count
                            RandNum = CUInt(Int(Rnd() * CDbl(GenerateTileset.ClusteredUnitChanceTotal)))
                            uintTemp = 0
                            For C = 0 To GenerateTileset.ClusteredUnitCount - 1
                                uintTemp += GenerateTileset.ClusteredUnits(C).Chance
                                If RandNum < uintTemp Then
                                    Exit For
                                End If
                            Next
                            Rotation = 0
                            Footprint = GenerateTileset.ClusteredUnits(C).Type.GetFootprintSelected(Rotation)
                            E = CInt(Math.Ceiling(Math.Max(Footprint.X, Footprint.Y) / 2.0F)) + 1
                            tmpUnit = PlaceUnitNear(GenerateTileset.ClusteredUnits(C).Type, PassageNodes(D, A).Pos, Map.ScavengerUnitGroup, E, Rotation, FeaturePlaceRange)
                            If tmpUnit Is Nothing Then
                                MissingUnitCount += Count - B + 1
                                Exit For
                            End If
                        Next
                    End If
                Next
            Next
            If MissingUnitCount > 0 Then
                ReturnResult.WarningAdd("Not enough space for " & MissingUnitCount & " clustered objects.")
            End If
        End If

        If TilePathMap.GetNodeLayer(TilePathMap.GetNodeLayerCount - 1).GetNodeCount <> 1 Then
            ReturnResult.ProblemAdd("Error: bad node count on top layer!")
            Return ReturnResult
        End If

        If GenerateTileset.ScatteredUnitChanceTotal > 0 Then
            For A = 1 To FeatureScatterCount
                RandNum = CUInt(Int(Rnd() * CDbl(GenerateTileset.ScatteredUnitChanceTotal)))
                uintTemp = 0
                For C = 0 To GenerateTileset.ScatteredUnitCount - 1
                    uintTemp += GenerateTileset.ScatteredUnits(C).Chance
                    If RandNum < uintTemp Then
                        Exit For
                    End If
                Next
                Rotation = 0
                Footprint = GenerateTileset.ScatteredUnits(C).Type.GetFootprintSelected(Rotation)
                B = FeatureScatterGap + CInt(Math.Ceiling(Math.Max(Footprint.X, Footprint.Y) / 2.0F))
                tmpNode = GetRandomChildNode(TilePathMap.GetNodeLayer(TilePathMap.GetNodeLayerCount - 1).GetNode(0), B)
                If tmpNode Is Nothing Then
                    Exit For
                Else
                    Dim NodeTag As clsNodeTag = CType(tmpNode.Tag, clsNodeTag)
                    If PlaceUnitNear(GenerateTileset.ScatteredUnits(C).Type, NodeTag.Pos, Map.ScavengerUnitGroup, B, Rotation, FeaturePlaceRange) Is Nothing Then
                        Exit For
                    End If
                End If
            Next
            If A < FeatureScatterCount + 1 Then
                ReturnResult.WarningAdd("Only enough space for " & A - 1 & " scattered objects.")
            End If
        End If

        Return ReturnResult
    End Function

    Public Function GetRandomChildNode(InputNode As PathfinderNode, MinClearance As Integer) As PathfinderNode

        If InputNode.GetClearance < MinClearance Then
            Return Nothing
        End If

        If InputNode.GetChildNodeCount = 0 Then
            Return InputNode
        Else
            Dim A As Integer
            Do
                A = CInt(Int(Rnd() * InputNode.GetChildNodeCount))
            Loop While InputNode.GetChildNode(A).GetClearance < MinClearance

            Dim ReturnResult As PathfinderNode = GetRandomChildNode(InputNode.GetChildNode(A), MinClearance)
            Return ReturnResult
        End If
    End Function

    Private Structure sPossibleGateway
        Public StartPos As sXY_int
        Public MiddlePos As sXY_int
        Public IsVertical As Boolean
        Public Length As Integer
    End Structure

    Public Function GenerateGateways() As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        'must be done before units otherwise the units will be treated as gateway obstacles

        Dim Terrain As clsMap.clsTerrain = Map.Terrain

        Dim X As Integer
        Dim SpaceCount As Integer
        Dim Y As Integer
        Dim PossibleGateways(Terrain.TileSize.X * Terrain.TileSize.Y * 2 - 1) As sPossibleGateway
        Dim PossibleGatewayCount As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            SpaceCount = 0
            For X = 0 To Terrain.TileSize.X - 1
                If GenerateTerrainTiles(X, Y).Node.GetClearance < 1 Then

                ElseIf GenerateTerrainTiles(X, Y).Node.GetClearance = 1 Then
                    If SpaceCount > 3 And SpaceCount <= 9 Then
                        PossibleGateways(PossibleGatewayCount).StartPos.X = X - SpaceCount
                        PossibleGateways(PossibleGatewayCount).StartPos.Y = Y
                        PossibleGateways(PossibleGatewayCount).Length = SpaceCount + 1
                        PossibleGateways(PossibleGatewayCount).IsVertical = False
                        PossibleGateways(PossibleGatewayCount).MiddlePos.X = PossibleGateways(PossibleGatewayCount).StartPos.X * 128 + PossibleGateways(PossibleGatewayCount).Length * 64
                        PossibleGateways(PossibleGatewayCount).MiddlePos.Y = PossibleGateways(PossibleGatewayCount).StartPos.Y * 128
                        PossibleGatewayCount += 1
                    End If
                    SpaceCount = 1
                Else
                    SpaceCount += 1
                End If
            Next
        Next
        For X = 0 To Terrain.TileSize.X - 1
            SpaceCount = 0
            Y = 0
            Do While Y < Terrain.TileSize.Y
                If GenerateTerrainTiles(X, Y).Node.GetClearance < 1 Then

                ElseIf GenerateTerrainTiles(X, Y).Node.GetClearance = 1 Then
                    If SpaceCount >= 3 And SpaceCount <= 9 Then
                        PossibleGateways(PossibleGatewayCount).StartPos.X = X
                        PossibleGateways(PossibleGatewayCount).StartPos.Y = Y - SpaceCount
                        PossibleGateways(PossibleGatewayCount).Length = SpaceCount + 1
                        PossibleGateways(PossibleGatewayCount).IsVertical = True
                        PossibleGateways(PossibleGatewayCount).MiddlePos.X = PossibleGateways(PossibleGatewayCount).StartPos.X * 128
                        PossibleGateways(PossibleGatewayCount).MiddlePos.Y = PossibleGateways(PossibleGatewayCount).StartPos.Y * 128 + PossibleGateways(PossibleGatewayCount).Length * 64
                        PossibleGatewayCount += 1
                    End If
                    SpaceCount = 1
                Else
                    SpaceCount += 1
                End If
                Y += 1
            Loop
        Next

        'add the best gateways

        Dim A As Integer
        Dim Value As Single
        Dim BestValue As Single
        Dim BestNum As Integer
        Dim TileIsGateway(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1) As Boolean
        Dim Valid As Boolean
        Dim InvalidPos As sXY_int
        Dim InvalidDist As Double

        Do While PossibleGatewayCount > 0
            BestNum = -1
            BestValue = Single.MaxValue
            For A = 0 To PossibleGatewayCount - 1
                'Value = 0.0F
                'For B = 0 To PossibleGatewayCount - 1
                '    Value += GetDist(PossibleGateways(A).MiddlePos, PossibleGateways(B).MiddlePos)
                'Next
                Value = PossibleGateways(A).Length
                If Value < BestValue Then
                    BestValue = Value
                    BestNum = A
                End If
            Next
            If PossibleGateways(BestNum).IsVertical Then
                Map.GatewayCreateStoreChange(PossibleGateways(BestNum).StartPos, New sXY_int(PossibleGateways(BestNum).StartPos.X, PossibleGateways(BestNum).StartPos.Y + PossibleGateways(BestNum).Length - 1))
                For Y = PossibleGateways(BestNum).StartPos.Y To PossibleGateways(BestNum).StartPos.Y + PossibleGateways(BestNum).Length - 1
                    TileIsGateway(PossibleGateways(BestNum).StartPos.X, Y) = True
                Next
            Else
                Map.GatewayCreateStoreChange(PossibleGateways(BestNum).StartPos, New sXY_int(PossibleGateways(BestNum).StartPos.X + PossibleGateways(BestNum).Length - 1, PossibleGateways(BestNum).StartPos.Y))
                For X = PossibleGateways(BestNum).StartPos.X To PossibleGateways(BestNum).StartPos.X + PossibleGateways(BestNum).Length - 1
                    TileIsGateway(X, PossibleGateways(BestNum).StartPos.Y) = True
                Next
            End If
            InvalidPos = PossibleGateways(BestNum).MiddlePos
            InvalidDist = PossibleGateways(BestNum).Length * 128
            A = 0
            Do While A < PossibleGatewayCount
                Valid = True
                If PossibleGateways(A).IsVertical Then
                    For Y = PossibleGateways(A).StartPos.Y To PossibleGateways(A).StartPos.Y + PossibleGateways(A).Length - 1
                        If TileIsGateway(PossibleGateways(A).StartPos.X, Y) Then
                            Valid = False
                            Exit For
                        End If
                    Next
                Else
                    For X = PossibleGateways(A).StartPos.X To PossibleGateways(A).StartPos.X + PossibleGateways(A).Length - 1
                        If TileIsGateway(X, PossibleGateways(A).StartPos.Y) Then
                            Valid = False
                            Exit For
                        End If
                    Next
                End If
                If Valid Then
                    If (InvalidPos - PossibleGateways(A).MiddlePos).ToDoubles.GetMagnitude < InvalidDist Then
                        Valid = False
                    End If
                End If
                If Not Valid Then
                    PossibleGatewayCount -= 1
                    If A <> PossibleGatewayCount Then
                        PossibleGateways(A) = PossibleGateways(PossibleGatewayCount)
                    End If
                Else
                    A += 1
                End If
            Loop
        Loop

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    'Public Structure sGenerateSide
    '    Public Node As PathfinderNode
    '    Public TopLink As PathfinderNode
    '    Public TopRightLink As PathfinderNode
    '    Public RightLink As PathfinderNode
    '    Public BottomRightLink As PathfinderNode
    '    Public BottomLink As PathfinderNode
    '    Public BottomLeftLink As PathfinderNode
    '    Public LeftLink As PathfinderNode
    '    Public TopLeftLink As PathfinderNode
    'End Structure

    'Public Structure sGenerateRoadsArgs
    '    Public RoadType As sPainter.clsRoad
    '    Public MaxAlt As Byte
    '    Public Terrain As sPainter.clsTerrain
    '    Public MinLength As Integer
    '    Public MaxLength As Integer
    '    Public MinTurnRatio As Single
    '    Public MaxTurnRatio As Single
    '    Public Quantity As Integer
    'End Structure

    'Public Sub GenerateRoads( Args As sGenerateRoadsArgs)
    '    Dim RoadPathMap As New PathfinderNetwork
    '    Dim tmpNode As PathfinderNode
    '    Dim NodeTag As clsNodeTag


    '    For Y = 0 To Terrain.Size.Y
    '        For X = 0 To Terrain.Size.X
    '            GenerateTerrainVertex(X, Y).Node = New PathfinderNode(RoadPathMap)
    '            NodeTag = New clsNodeTag
    '            NodeTag.Pos = New sXY_int(X * 128, Y * 128)
    '            GenerateTerrainVertex(X, Y).Node.Tag = NodeTag
    '        Next
    '    Next
    '    For Y = 0 To Terrain.Size.Y
    '        For X = 0 To Terrain.Size.X
    '            tmpNodeA = GenerateTerrainVertex(X, Y).Node
    '            If X > 0 Then
    '                tmpNodeB = GenerateTerrainVertex(X - 1, Y).Node
    '                GenerateTerrainVertex(X, Y).LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '            End If
    '            If Y > 0 Then
    '                If X > 0 Then
    '                    tmpNodeB = GenerateTerrainVertex(X - 1, Y - 1).Node
    '                    GenerateTerrainVertex(X, Y).TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '                tmpNodeB = GenerateTerrainVertex(X, Y - 1).Node
    '                GenerateTerrainVertex(X, Y).TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                If X < Terrain.Size.X Then
    '                    tmpNodeB = GenerateTerrainVertex(X + 1, Y - 1).Node
    '                    GenerateTerrainVertex(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '            End If
    '            If X < Terrain.Size.X Then
    '                tmpNodeB = GenerateTerrainVertex(X + 1, Y).Node
    '                GenerateTerrainVertex(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '            End If
    '            If Y < Terrain.Size.Y Then
    '                If X > 0 Then
    '                    tmpNodeB = GenerateTerrainVertex(X - 1, Y + 1).Node
    '                    GenerateTerrainVertex(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '                tmpNodeB = GenerateTerrainVertex(X, Y + 1).Node
    '                GenerateTerrainVertex(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                If X < Terrain.Size.X Then
    '                    tmpNodeB = GenerateTerrainVertex(X + 1, Y + 1).Node
    '                    GenerateTerrainVertex(X, Y).BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '            End If
    '        Next
    '    Next

    '    RoadPathMap.LargeArraysResize()
    '    RoadPathMap.FindCalc()

    '    RoadPathMap.Deallocate()
    'End Sub

    Public Sub ClearLayout()

        Dim A As Integer
        Dim B As Integer

        If TilePathMap IsNot Nothing Then
            TilePathMap.Deallocate()
            TilePathMap = Nothing
        End If
        If VertexPathMap IsNot Nothing Then
            VertexPathMap.Deallocate()
            VertexPathMap = Nothing
        End If

        For A = 0 To ConnectionCount - 1
            Connections(A).PassageNodeA = Nothing
            Connections(A).PassageNodeB = Nothing
            Erase Connections(A).Reflections
        Next
        ConnectionCount = 0

        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                Erase PassageNodes(B, A).Connections
            Next
        Next
        PassageNodeCount = 0

        NearestCount = 0
    End Sub

    Private Function MakePassageNodes(Pos As sXY_int, IsOnBorder As Boolean) As Boolean
        Dim A As Integer
        Dim B As Integer
        Dim tmpNode As clsPassageNode
        Dim RotatedPos As sXY_int
        Dim SymmetrySize As sXY_int
        Dim Positions(3) As sXY_int
        Dim Limits As sXY_int

        SymmetrySize.X = CInt(TileSize.X * TerrainGridSpacing / SymmetryBlockCountXY.X)
        SymmetrySize.Y = CInt(TileSize.Y * TerrainGridSpacing / SymmetryBlockCountXY.Y)

        Limits.X = SymmetrySize.X - 1
        Limits.Y = SymmetrySize.Y - 1

        For A = 0 To SymmetryBlockCount - 1
            RotatedPos = GetRotatedPos(SymmetryBlocks(A).Orientation, Pos, Limits)
            Positions(A).X = SymmetryBlocks(A).XYNum.X * SymmetrySize.X + RotatedPos.X
            Positions(A).Y = SymmetryBlocks(A).XYNum.Y * SymmetrySize.Y + RotatedPos.Y
            For B = 0 To A - 1
                If (Positions(A) - Positions(B)).ToDoubles.GetMagnitude < NodeScale * TerrainGridSpacing * 2.0# Then
                    Return False
                End If
            Next
        Next

        For A = 0 To SymmetryBlockCount - 1
            tmpNode = New clsPassageNode
            PassageNodes(A, PassageNodeCount) = tmpNode
            tmpNode.Num = PassageNodeCount
            tmpNode.MirrorNum = A
            tmpNode.Pos = Positions(A)
            tmpNode.IsOnBorder = IsOnBorder
        Next
        PassageNodeCount += 1

        Return True
    End Function

    Private Function CheckRampAngles(NewRampConnection As clsConnection, MinSpacingAngle As Double, MinSpacingAngle2 As Double, MinPassageSpacingAngle As Double) As Boolean
        Dim XY_int As sXY_int
        Dim NodeAAwayAngle As Double
        Dim NodeBAwayAngle As Double

        XY_int.X = NewRampConnection.PassageNodeB.Pos.X - NewRampConnection.PassageNodeA.Pos.X
        XY_int.Y = NewRampConnection.PassageNodeB.Pos.Y - NewRampConnection.PassageNodeA.Pos.Y
        If NewRampConnection.PassageNodeA.Connections(NewRampConnection.PassageNodeA_ConnectionNum).IsB Then
            NodeBAwayAngle = XY_int.ToDoubles.GetAngle
            NodeAAwayAngle = AngleClamp(NodeBAwayAngle - Math.PI)
        Else
            NodeAAwayAngle = XY_int.ToDoubles.GetAngle
            NodeBAwayAngle = AngleClamp(NodeBAwayAngle - Math.PI)
        End If
        If Not CheckRampNodeRampAngles(NewRampConnection.PassageNodeA, NewRampConnection.PassageNodeB, NodeAAwayAngle, MinSpacingAngle, MinSpacingAngle2) Then
            Return False
        End If
        If Not CheckRampNodeRampAngles(NewRampConnection.PassageNodeB, NewRampConnection.PassageNodeA, NodeBAwayAngle, MinSpacingAngle, MinSpacingAngle2) Then
            Return False
        End If
        If Not CheckRampNodeLevelAngles(NewRampConnection.PassageNodeA, NodeAAwayAngle, MinPassageSpacingAngle) Then
            Return False
        End If
        If Not CheckRampNodeLevelAngles(NewRampConnection.PassageNodeB, NodeBAwayAngle, MinPassageSpacingAngle) Then
            Return False
        End If
        Return True
    End Function

    Private Function CheckRampNodeRampAngles(RampPassageNode As clsPassageNode, OtherRampPassageNode As clsPassageNode, RampAwayAngle As Double, MinSpacingAngle As Double, MinSpacingAngle2 As Double) As Boolean
        Dim ConnectionNum As Integer
        Dim tmpConnection As clsConnection
        Dim OtherNode As clsPassageNode
        Dim XY_int As sXY_int
        Dim SpacingAngle As Double
        Dim RampDifference As Integer

        For ConnectionNum = 0 To RampPassageNode.ConnectionCount - 1
            tmpConnection = RampPassageNode.Connections(ConnectionNum).Connection
            If tmpConnection.IsRamp Then
                OtherNode = RampPassageNode.Connections(ConnectionNum).GetOther
                XY_int.X = OtherNode.Pos.X - RampPassageNode.Pos.X
                XY_int.Y = OtherNode.Pos.Y - RampPassageNode.Pos.Y
                SpacingAngle = Math.Abs(AngleClamp(RampAwayAngle - XY_int.ToDoubles.GetAngle))
                RampDifference = Math.Abs(OtherNode.Level - OtherRampPassageNode.Level)
                If RampDifference >= 2 Then
                    If SpacingAngle < MinSpacingAngle2 Then
                        Return False
                    End If
                ElseIf RampDifference = 0 Then
                    If SpacingAngle < MinSpacingAngle Then
                        Return False
                    End If
                Else
                    Stop
                    Return False
                End If
            End If
        Next
        Return True
    End Function

    Private Function PassageNodeHasRamp(PassageNode As clsPassageNode) As Boolean
        Dim ConnectionNum As Integer

        For ConnectionNum = 0 To PassageNode.ConnectionCount - 1
            If PassageNode.Connections(ConnectionNum).Connection.IsRamp Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Function CheckRampNodeLevelAngles(RampPassageNode As clsPassageNode, RampAwayAngle As Double, MinSpacingAngle As Double) As Boolean
        Dim ConnectionNum As Integer
        Dim tmpConnection As clsConnection
        Dim OtherPassageNode As clsPassageNode
        Dim OtherNum As Integer
        Dim NarrowConnection As Boolean
        Dim XY_int As sXY_int
        Dim HasRamp As Boolean = PassageNodeHasRamp(RampPassageNode)

        For ConnectionNum = 0 To RampPassageNode.ConnectionCount - 1
            tmpConnection = RampPassageNode.Connections(ConnectionNum).Connection
            OtherPassageNode = RampPassageNode.Connections(ConnectionNum).GetOther
            If OtherPassageNode.Level = RampPassageNode.Level Then
                NarrowConnection = True
                If ConnectionNum = 0 Then
                    OtherNum = RampPassageNode.ConnectionCount - 1
                Else
                    OtherNum = ConnectionNum - 1
                End If
                If OtherNum <> ConnectionNum Then
                    If RampPassageNode.Connections(OtherNum).GetOther.Level = OtherPassageNode.Level Then
                        NarrowConnection = False
                    End If
                Else
                    NarrowConnection = False
                End If
                If ConnectionNum = RampPassageNode.ConnectionCount - 1 Then
                    OtherNum = 0
                Else
                    OtherNum = ConnectionNum + 1
                End If
                If OtherNum <> ConnectionNum Then
                    If RampPassageNode.Connections(OtherNum).GetOther.Level = OtherPassageNode.Level Then
                        NarrowConnection = False
                    End If
                Else
                    NarrowConnection = False
                End If
                If NarrowConnection Then
                    XY_int.X = OtherPassageNode.Pos.X - RampPassageNode.Pos.X
                    XY_int.Y = OtherPassageNode.Pos.Y - RampPassageNode.Pos.Y
                    'If HasRamp Then
                    '    Return False
                    'End If
                    If Math.Abs(AngleClamp(XY_int.ToDoubles.GetAngle - RampAwayAngle)) < MinSpacingAngle Then
                        Return False
                    End If
                    'If PassageNodeHasRamp(OtherPassageNode) Then
                    '    Return False
                    'End If
                End If
            End If
        Next
        Return True
    End Function

    Private Class clsPassageNodeHeightLevelArgs
        Public PassageNode As clsPassageNode
        Public MapLevelCount() As Integer
        Public PassageNodesMinLevel As New clsPassageNodeLevels
        Public PassageNodesMaxLevel As New clsPassageNodeLevels
        Public FlatsCutoff As Integer
        Public PassagesCutoff As Integer
        Public VariationCutoff As Integer
        Public ActionTotal As Integer
    End Class

    Private Function PassageNodeHeightLevel(Args As clsPassageNodeHeightLevelArgs) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Problem = ""
        ReturnResult.Success = False

        Dim LevelCounts(LevelCount - 1) As Integer
        Dim WaterCount As Integer
        Dim ConnectedToLevel As Boolean
        Dim tmpPassageNodeB As clsPassageNode
        Dim tmpPassageNodeC As clsPassageNode
        Dim EligableCount As Integer
        Dim Eligables(LevelCount - 1) As Integer
        Dim NewHeightLevel As Integer
        Dim RandomAction As Integer
        Dim A As Integer
        Dim B As Integer

        For B = 0 To Args.PassageNode.ConnectionCount - 1
            tmpPassageNodeB = Args.PassageNode.Connections(B).GetOther
            If tmpPassageNodeB.Level >= 0 Then
                LevelCounts(tmpPassageNodeB.Level) += 1
                ConnectedToLevel = True
            End If
            If tmpPassageNodeB.IsWater Then
                WaterCount += 1
            End If
        Next
        If WaterCount > 0 Then
            NewHeightLevel = 0
        ElseIf Args.PassageNodesMinLevel.Nodes(Args.PassageNode.Num) > Args.PassageNodesMaxLevel.Nodes(Args.PassageNode.Num) Then
            ReturnResult.Problem = "Error: Min height more than max."
            Return ReturnResult
        ElseIf Not ConnectedToLevel Then
            'switch to the most uncommon level on the map
            A = Integer.MaxValue
            EligableCount = 0
            For B = Args.PassageNodesMinLevel.Nodes(Args.PassageNode.Num) To Args.PassageNodesMaxLevel.Nodes(Args.PassageNode.Num)
                If Args.MapLevelCount(B) < A Then
                    A = Args.MapLevelCount(B)
                    Eligables(0) = B
                    EligableCount = 1
                ElseIf Args.MapLevelCount(B) = A Then
                    Eligables(EligableCount) = B
                    EligableCount += 1
                End If
            Next
            NewHeightLevel = Eligables(CInt(Int(Rnd() * EligableCount)))
        Else
            RandomAction = CInt(Int(Rnd() * Args.ActionTotal))
            If RandomAction < Args.FlatsCutoff Then
                'extend the level that surrounds this most
                A = 0
                EligableCount = 0
                For B = Args.PassageNodesMinLevel.Nodes(Args.PassageNode.Num) To Args.PassageNodesMaxLevel.Nodes(Args.PassageNode.Num)
                    If LevelCounts(B) > A Then
                        A = LevelCounts(B)
                        Eligables(0) = B
                        EligableCount = 1
                    ElseIf LevelCounts(B) = A Then
                        Eligables(EligableCount) = B
                        EligableCount += 1
                    End If
                Next
            ElseIf RandomAction < Args.PassagesCutoff Then
                'extend any level that surrounds only once, or twice by nodes that aren't already connected
                EligableCount = 0
                For B = Args.PassageNodesMinLevel.Nodes(Args.PassageNode.Num) To Args.PassageNodesMaxLevel.Nodes(Args.PassageNode.Num)
                    If LevelCounts(B) = 1 Then
                        Eligables(EligableCount) = B
                        EligableCount += 1
                    ElseIf LevelCounts(B) = 2 Then
                        EligableCount = 0
                        tmpPassageNodeC = Nothing
                        For A = 0 To Args.PassageNode.ConnectionCount - 1
                            tmpPassageNodeB = Args.PassageNode.Connections(A).GetOther
                            If tmpPassageNodeB.Level = B Then
                                If tmpPassageNodeC Is Nothing Then
                                    tmpPassageNodeC = tmpPassageNodeB
                                Else
                                    If tmpPassageNodeC.FindConnection(tmpPassageNodeB) Is Nothing Then
                                        Eligables(EligableCount) = B
                                        EligableCount += 1
                                    End If
                                    Exit For
                                End If
                            End If
                        Next
                        If A = Args.PassageNode.ConnectionCount Then
                            MsgBox("Error: two nodes not found")
                        End If
                    End If
                Next
            ElseIf RandomAction < Args.VariationCutoff Then
                EligableCount = 0
            Else
                ReturnResult.Problem = "Error: Random number out of range."
                Return ReturnResult
            End If
            If EligableCount = 0 Then
                'extend the most uncommon surrounding
                A = Integer.MaxValue
                EligableCount = 0
                For B = Args.PassageNodesMinLevel.Nodes(Args.PassageNode.Num) To Args.PassageNodesMaxLevel.Nodes(Args.PassageNode.Num)
                    If LevelCounts(B) < A Then
                        A = LevelCounts(B)
                        Eligables(0) = B
                        EligableCount = 1
                    ElseIf LevelCounts(B) = A Then
                        Eligables(EligableCount) = B
                        EligableCount += 1
                    End If
                Next
            End If
            NewHeightLevel = Eligables(CInt(Int(Rnd() * EligableCount)))
        End If
        For B = 0 To SymmetryBlockCount - 1
            PassageNodes(B, Args.PassageNode.Num).Level = NewHeightLevel
        Next
        PassageNodesMinLevelSet(Args.PassageNode, Args.PassageNodesMinLevel, NewHeightLevel, MaxLevelTransition)
        PassageNodesMaxLevelSet(Args.PassageNode, Args.PassageNodesMaxLevel, NewHeightLevel, MaxLevelTransition)
        Args.MapLevelCount(NewHeightLevel) += 1

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function GenerateRamps() As clsResult
        Dim ReturnResult As New clsResult("Ramps")

        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim E As Integer
        Dim BestDist As Double
        Dim BestNum As Integer
        Dim XY_int As sXY_int
        Dim tmpPassageNodeA As clsPassageNode
        Dim tmpPassageNodeB As clsPassageNode
        Dim Dist As Double

        'make ramps

        For A = 0 To ConnectionCount - 1
            Connections(A).IsRamp = False
        Next

        Dim tmpNodeA As PathfinderNode
        Dim tmpNodeB As PathfinderNode
        Dim PassageNodePathNodes(,) As PathfinderNode
        Dim NewConnection As PathfinderConnection

        Dim PassageNodeNetwork As clsPassageNodeNework = MakePassageNodeNetwork()
        PassageNodePathNodes = PassageNodeNetwork.PassageNodePathNodes

        Dim PossibleRamps(ConnectionCount - 1) As clsConnection
        Dim PossibleRampCount As Integer
        Dim GetPathStartNodes(0) As PathfinderNode
        Dim ResultPaths() As PathfinderNetwork.PathList

        'ramp connections whose points are too far apart

        Dim ConnectionsCanRamp(ConnectionCount - 1) As Boolean

        For B = 0 To ConnectionCount - 1
            C = Math.Abs(Connections(B).PassageNodeA.Level - Connections(B).PassageNodeB.Level)
            If C = 1 Then
                If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) _
                And Connections(B).PassageNodeA.MirrorNum = 0 _
                And Connections(B).PassageNodeA.Num <> Connections(B).PassageNodeB.Num Then
                    ConnectionsCanRamp(B) = True
                Else
                    ConnectionsCanRamp(B) = False
                End If
            Else
                ConnectionsCanRamp(B) = False
            End If
        Next

        Dim Connectedness As New clsNodeConnectedness
        ReDim Connectedness.NodeConnectedness(PassageNodeCount - 1)
        ReDim Connectedness.PassageNodeVisited(SymmetryBlockCount - 1, PassageNodeCount - 1)
        Connectedness.PassageNodePathNodes = PassageNodePathNodes
        Connectedness.PassageNodePathMap = PassageNodeNetwork.Network

        Dim tmpPathConnection(3) As PathfinderConnection
        Dim Value As Double
        Dim BestDistB As Double
        Dim BaseDist As Double
        Dim RampDist As Double
        Dim UpdateNodeConnectednessArgs As New clsUpdateNodeConnectednessArgs
        Dim UpdateNetworkConnectednessArgs As New clsUpdateNetworkConnectednessArgs

        UpdateNodeConnectednessArgs.Args = Connectedness
        UpdateNetworkConnectednessArgs.Args = Connectedness
        ReDim UpdateNetworkConnectednessArgs.PassageNodeUpdated(PassageNodeCount - 1)
        UpdateNetworkConnectednessArgs.SymmetryBlockCount = SymmetryBlockCount

        For A = 0 To PassageNodeCount - 1
            Connectedness.NodeConnectedness(A) = 0.0F
            For B = 0 To PassageNodeCount - 1
                For C = 0 To SymmetryBlockCount - 1
                    Connectedness.PassageNodeVisited(C, B) = False
                Next
            Next
            UpdateNodeConnectednessArgs.OriginalNode = PassageNodes(0, A)
            UpdateNodeConnectedness(UpdateNodeConnectednessArgs, PassageNodes(0, A))
        Next

        Do
            BestNum = -1
            BestDist = 1.0F 'for connections that can already reach the other side
            BestDistB = 0.0F 'for connections that cant
            PossibleRampCount = 0
            For B = 0 To ConnectionCount - 1
                If ConnectionsCanRamp(B) And Not Connections(B).IsRamp Then
                    If CheckRampAngles(Connections(B), 80.0# * RadOf1Deg, 120.0# * RadOf1Deg, 0.0# * RadOf1Deg) Then
                        GetPathStartNodes(0) = PassageNodePathNodes(Connections(B).PassageNodeA.MirrorNum, Connections(B).PassageNodeA.Num)
                        ResultPaths = PassageNodeNetwork.Network.GetPath(GetPathStartNodes, PassageNodePathNodes(Connections(B).PassageNodeB.MirrorNum, Connections(B).PassageNodeB.Num), -1, 0)
                        BaseDist = Double.MaxValue
                        XY_int.X = CInt((Connections(B).PassageNodeA.Pos.X + Connections(B).PassageNodeB.Pos.X) / 2.0#)
                        XY_int.Y = CInt((Connections(B).PassageNodeA.Pos.Y + Connections(B).PassageNodeB.Pos.Y) / 2.0#)
                        For E = 0 To TotalPlayerCount - 1
                            Dist = (PlayerBases(E).Pos - XY_int).ToDoubles.GetMagnitude
                            If Dist < BaseDist Then
                                BaseDist = Dist
                            End If
                        Next
                        RampDist = Math.Max(MaxDisconnectionDist * RampBase ^ (BaseDist / 1024.0#), 1.0F)
                        If ResultPaths Is Nothing Then
                            Value = Connectedness.NodeConnectedness(Connections(B).PassageNodeA.Num) + Connectedness.NodeConnectedness(Connections(B).PassageNodeB.Num)
                            If Double.MaxValue > BestDist Then
                                BestDist = Double.MaxValue
                                BestDistB = Value
                                PossibleRamps(0) = Connections(B)
                                PossibleRampCount = 1
                            Else
                                If Value < BestDistB Then
                                    BestDistB = Value
                                    PossibleRamps(0) = Connections(B)
                                    PossibleRampCount = 1
                                ElseIf Value = BestDistB Then
                                    PossibleRamps(PossibleRampCount) = Connections(B)
                                    PossibleRampCount += 1
                                End If
                            End If
                        ElseIf ResultPaths(0).PathCount <> 1 Then
                            ReturnResult.ProblemAdd("Error: Invalid number of routes returned.")
                            GoTo Finish
                        ElseIf ResultPaths(0).Paths(0).Value / RampDist > BestDist Then
                            BestDist = ResultPaths(0).Paths(0).Value / RampDist
                            PossibleRamps(0) = Connections(B)
                            PossibleRampCount = 1
                        ElseIf ResultPaths(0).Paths(0).Value / RampDist = BestDist Then
                            PossibleRamps(PossibleRampCount) = Connections(B)
                            PossibleRampCount += 1
                        ElseIf ResultPaths(0).Paths(0).Value <= RampDist Then
                            ConnectionsCanRamp(B) = False
                        End If
                    Else
                        ConnectionsCanRamp(B) = False
                    End If
                Else
                    ConnectionsCanRamp(B) = False
                End If
            Next
            If PossibleRampCount > 0 Then
                BestNum = CInt(Int(Rnd() * PossibleRampCount))
                PossibleRamps(BestNum).IsRamp = True
                tmpPassageNodeA = PossibleRamps(BestNum).PassageNodeA
                tmpPassageNodeB = PossibleRamps(BestNum).PassageNodeB
                tmpNodeA = PassageNodePathNodes(tmpPassageNodeA.MirrorNum, tmpPassageNodeA.Num)
                tmpNodeB = PassageNodePathNodes(tmpPassageNodeB.MirrorNum, tmpPassageNodeB.Num)
                NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                For C = 0 To PossibleRamps(BestNum).ReflectionCount - 1
                    PossibleRamps(BestNum).Reflections(C).IsRamp = True
                    tmpPassageNodeA = PossibleRamps(BestNum).Reflections(C).PassageNodeA
                    tmpPassageNodeB = PossibleRamps(BestNum).Reflections(C).PassageNodeB
                    tmpNodeA = PassageNodePathNodes(tmpPassageNodeA.MirrorNum, tmpPassageNodeA.Num)
                    tmpNodeB = PassageNodePathNodes(tmpPassageNodeB.MirrorNum, tmpPassageNodeB.Num)
                    NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                Next
                PassageNodeNetwork.Network.FindCalc()
                For E = 0 To PassageNodeCount - 1
                    UpdateNetworkConnectednessArgs.PassageNodeUpdated(E) = False
                Next
                If PossibleRamps(BestNum).PassageNodeA.MirrorNum = 0 Then
                    UpdateNetworkConnectedness(UpdateNetworkConnectednessArgs, PossibleRamps(BestNum).PassageNodeA)
                ElseIf PossibleRamps(BestNum).PassageNodeB.MirrorNum = 0 Then
                    UpdateNetworkConnectedness(UpdateNetworkConnectednessArgs, PossibleRamps(BestNum).PassageNodeB)
                Else
                    ReturnResult.ProblemAdd("Error: Initial ramp not in area 0.")
                    GoTo Finish
                End If
            Else
                Exit Do
            End If
        Loop

        Dim FloodArgs As PathfinderNetwork.sFloodProximityArgs
        FloodArgs.StartNode = PassageNodeNetwork.PassageNodePathNodes(0, 0)
        FloodArgs.NodeValues = PassageNodeNetwork.Network.NetworkLargeArrays.Nodes_ValuesA
        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                FloodArgs.NodeValues(PassageNodeNetwork.PassageNodePathNodes(B, A).Layer_NodeNum) = Single.MaxValue
            Next
        Next
        PassageNodeNetwork.Network.FloodProximity(FloodArgs)
        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                If Not PassageNodes(B, A).IsWater Then
                    If FloodArgs.NodeValues(PassageNodeNetwork.PassageNodePathNodes(B, A).Layer_NodeNum) = Single.MaxValue Then
                        ReturnResult.ProblemAdd("Land is unreachable. Reduce variation or retry.")
                        GoTo Finish
                    End If
                End If
            Next
        Next

Finish:
        PassageNodeNetwork.Network.Deallocate()

        Return ReturnResult
    End Function

    Private Class clsPassageNodeNework
        Public Network As PathfinderNetwork
        Public PassageNodePathNodes(,) As PathfinderNode
    End Class

    Private Function MakePassageNodeNetwork() As clsPassageNodeNework
        Dim ReturnResult As New clsPassageNodeNework
        Dim NewConnection As PathfinderConnection
        Dim NodeTag As clsNodeTag
        Dim tmpNodeA As PathfinderNode
        Dim tmpNodeB As PathfinderNode
        Dim A As Integer
        Dim B As Integer

        ReturnResult.Network = New PathfinderNetwork
        ReDim ReturnResult.PassageNodePathNodes(SymmetryBlockCount - 1, PassageNodeCount - 1)
        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                ReturnResult.PassageNodePathNodes(B, A) = New PathfinderNode(ReturnResult.Network)
                NodeTag = New clsNodeTag
                NodeTag.Pos = PassageNodes(B, A).Pos
                ReturnResult.PassageNodePathNodes(B, A).Tag = NodeTag
            Next
        Next
        For A = 0 To ConnectionCount - 1
            If Connections(A).PassageNodeA.Level = Connections(A).PassageNodeB.Level Or Connections(A).IsRamp Then
                If Not (Connections(A).PassageNodeA.IsWater Or Connections(A).PassageNodeB.IsWater) Then
                    tmpNodeA = ReturnResult.PassageNodePathNodes(Connections(A).PassageNodeA.MirrorNum, Connections(A).PassageNodeA.Num)
                    tmpNodeB = ReturnResult.PassageNodePathNodes(Connections(A).PassageNodeB.MirrorNum, Connections(A).PassageNodeB.Num)
                    NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
            End If
        Next
        ReturnResult.Network.LargeArraysResize()
        ReturnResult.Network.FindCalc()

        Return ReturnResult
    End Function

    Public Function GenerateOil() As clsResult
        Dim ReturnResult As New clsResult("Oil")

        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer

        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                PassageNodes(B, A).OilCount = 0
            Next
        Next

        'store passage node route distances
        Dim PassageNodePathMap As clsPassageNodeNework = MakePassageNodeNetwork()
        Dim GetPathStartNodes(0) As PathfinderNode
        Dim ResultPaths() As PathfinderNetwork.PathList

        ReDim PassageNodeDists(SymmetryBlockCount - 1, PassageNodeCount - 1, SymmetryBlockCount - 1, PassageNodeCount - 1)
        For A = 0 To PassageNodeCount - 1
            For D = 0 To SymmetryBlockCount - 1
                PassageNodeDists(D, A, D, A) = 0.0F
                For B = 0 To PassageNodeCount - 1
                    For C = 0 To SymmetryBlockCount - 1
                        If PassageNodes(0, A).IsWater Or PassageNodes(C, B).IsWater Or (C <> 0 And D <> 0) Then
                            PassageNodeDists(D, A, C, B) = Single.MaxValue
                            PassageNodeDists(C, B, D, A) = Single.MaxValue
                        Else
                            GetPathStartNodes(0) = PassageNodePathMap.PassageNodePathNodes(D, A)
                            ResultPaths = PassageNodePathMap.Network.GetPath(GetPathStartNodes, PassageNodePathMap.PassageNodePathNodes(C, B), -1, 0)
                            If ResultPaths Is Nothing Then
                                ReturnResult.ProblemAdd("Map is not all connected.")
                                PassageNodePathMap.Network.Deallocate()
                                Return ReturnResult
                            Else
                                If ResultPaths(0).PathCount <> 1 Then
                                    Stop
                                End If
                                PassageNodeDists(D, A, C, B) = ResultPaths(0).Paths(0).Value
                                PassageNodeDists(C, B, D, A) = ResultPaths(0).Paths(0).Value
                            End If
                        End If
                    Next
                Next
            Next
        Next

        PassageNodePathMap.Network.Deallocate()

        'place oil
        Dim PlacedExtraOilCount As Integer
        Dim MaxBestNodeCount As Integer
        MaxBestNodeCount = 1
        For A = 0 To OilAtATime - 1
            MaxBestNodeCount *= PassageNodeCount
        Next
        Dim BestPossibility As clsGenerateMap.clsOilPossibilities.clsPossibility
        Dim OilArgs As New clsGenerateMap.clsOilBalanceLoopArgs
        ReDim OilArgs.OilClusterSizes(OilAtATime - 1)
        ReDim OilArgs.PlayerOilScore(TopLeftPlayerCount - 1)
        ReDim OilArgs.OilNodes(OilAtATime - 1)

        'balanced oil
        Do While PlacedExtraOilCount < ExtraOilCount
            'place oil farthest away from other oil and where it best balances the player oil score
            For A = 0 To OilAtATime - 1
                OilArgs.OilClusterSizes(A) = Math.Min(ExtraOilClusterSizeMin + CInt(Int(Rnd() * (ExtraOilClusterSizeMax - ExtraOilClusterSizeMin + 1))), Math.Max(CInt(Math.Ceiling((ExtraOilCount - PlacedExtraOilCount) / SymmetryBlockCount)), 1))
            Next
            OilArgs.OilPossibilities = New clsGenerateMap.clsOilPossibilities
            OilBalanceLoop(OilArgs, 0)

            BestPossibility = OilArgs.OilPossibilities.BestPossibility
            If BestPossibility IsNot Nothing Then
                For B = 0 To OilAtATime - 1
                    For A = 0 To SymmetryBlockCount - 1
                        PassageNodes(A, BestPossibility.Nodes(B).Num).OilCount += OilArgs.OilClusterSizes(B)
                    Next
                    PlacedExtraOilCount += OilArgs.OilClusterSizes(B) * SymmetryBlockCount
                Next
                For A = 0 To TopLeftPlayerCount - 1
                    OilArgs.PlayerOilScore(A) += BestPossibility.PlayerOilScoreAddition(A)
                Next
            Else
                ReturnResult.WarningAdd("Could not place all of the oil. " & PlacedExtraOilCount & " oil was placed.")
                Exit Do
            End If
        Loop

        'base oil
        For A = 0 To TopLeftPlayerCount - 1
            For B = 0 To SymmetryBlockCount - 1
                PassageNodes(B, PlayerBases(A).Nodes(0).Num).OilCount += BaseOilCount
            Next
        Next

        Return ReturnResult
    End Function
End Class
