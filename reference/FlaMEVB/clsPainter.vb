
Public Class clsPainter
    Public Class clsTerrain
        Public Num As Integer
        Public Name As String

        Public Tiles As New clsTileList
    End Class
    Public Terrains() As clsTerrain
    Public TerrainCount As Integer
    Public Class clsTileList
        Public Structure sTileOrientationChance
            Public TextureNum As Integer
            Public Direction As sTileDirection
            Public Chance As UInteger
        End Structure
        Public Tiles() As sTileOrientationChance
        Public TileCount As Integer
        Public TileChanceTotal As Integer

        Public Sub Tile_Add(TileNum As Integer, TileOutwardOrientation As sTileDirection, Chance As UInteger)

            ReDim Preserve Tiles(TileCount)
            Tiles(TileCount).TextureNum = TileNum
            Tiles(TileCount).Direction = TileOutwardOrientation
            Tiles(TileCount).Chance = Chance
            TileCount += 1

            TileChanceTotal += CInt(Chance)
        End Sub

        Public Sub Tile_Remove(Num As Integer)

            TileChanceTotal -= CInt(Tiles(Num).Chance)

            TileCount -= 1
            If Num <> TileCount Then
                Tiles(Num) = Tiles(TileCount)
            End If
            ReDim Preserve Tiles(TileCount - 1)
        End Sub

        Public Function GetRandom() As sTileOrientationChance
            Dim ReturnResult As sTileOrientationChance
            Dim A As Integer
            Dim intRandom As Integer
            Dim Total As Integer

            intRandom = CInt(Int(Rnd() * TileChanceTotal))
            For A = 0 To TileCount - 1
                Total += CInt(Tiles(A).Chance)
                If intRandom < Total Then
                    Exit For
                End If
            Next
            If A = TileCount Then
                ReturnResult.TextureNum = -1
                ReturnResult.Direction = TileDirection_None
            Else
                ReturnResult = Tiles(A)
            End If
            Return ReturnResult
        End Function
    End Class
    Public Class clsTransition_Brush
        Public Name As String
        Public Terrain_Inner As clsTerrain
        Public Terrain_Outer As clsTerrain
        Public Tiles_Straight As New clsTileList
        Public Tiles_Corner_In As New clsTileList
        Public Tiles_Corner_Out As New clsTileList
    End Class
    Public TransitionBrushes() As clsTransition_Brush
    Public TransitionBrushCount As Integer
    Public Class clsCliff_Brush
        Public Name As String
        Public Terrain_Inner As clsTerrain
        Public Terrain_Outer As clsTerrain
        Public Tiles_Straight As New clsTileList
        Public Tiles_Corner_In As New clsTileList
        Public Tiles_Corner_Out As New clsTileList
    End Class
    Public CliffBrushes() As clsCliff_Brush
    Public CliffBrushCount As Integer

    Public Class clsRoad
        Public Num As Integer
        Public Name As String
    End Class
    Public Roads() As clsRoad
    Public RoadCount As Integer
    Public Class clsRoad_Brush
        Public Road As clsRoad
        Public Terrain As clsTerrain
        Public Tile_Straight As New clsTileList
        Public Tile_Corner_In As New clsTileList
        Public Tile_TIntersection As New clsTileList
        Public Tile_CrossIntersection As New clsTileList
        Public Tile_End As New clsTileList
    End Class
    Public RoadBrushes() As clsRoad_Brush
    Public RoadBrushCount As Integer

    Public Sub TransitionBrush_Add(NewBrush As clsTransition_Brush)

        ReDim Preserve TransitionBrushes(TransitionBrushCount)
        TransitionBrushes(TransitionBrushCount) = NewBrush
        TransitionBrushCount += 1
    End Sub

    Public Sub TransitionBrush_Remove(Num As Integer)

        TransitionBrushCount -= 1
        If Num <> TransitionBrushCount Then
            TransitionBrushes(Num) = TransitionBrushes(TransitionBrushCount)
        End If
        ReDim Preserve TransitionBrushes(TransitionBrushCount - 1)
    End Sub

    Public Sub CliffBrush_Add(NewBrush As clsCliff_Brush)

        ReDim Preserve CliffBrushes(CliffBrushCount)
        CliffBrushes(CliffBrushCount) = NewBrush
        CliffBrushCount += 1
    End Sub

    Public Sub CliffBrush_Remove(Num As Integer)

        CliffBrushCount -= 1
        If Num <> CliffBrushCount Then
            CliffBrushes(Num) = CliffBrushes(CliffBrushCount)
        End If
        ReDim Preserve CliffBrushes(CliffBrushCount - 1)
    End Sub

    Public Sub Terrain_Add(NewTerrain As clsTerrain)

        NewTerrain.Num = TerrainCount
        ReDim Preserve Terrains(TerrainCount)
        Terrains(TerrainCount) = NewTerrain
        TerrainCount += 1
    End Sub

    Public Sub Terrain_Remove(Num As Integer)

        Terrains(Num).Num = -1
        TerrainCount -= 1
        If Num <> TerrainCount Then
            Terrains(Num) = Terrains(TerrainCount)
            Terrains(Num).Num = Num
        End If
        ReDim Preserve Terrains(TerrainCount - 1)
    End Sub

    Public Sub RoadBrush_Add(NewRoadBrush As clsRoad_Brush)

        ReDim Preserve RoadBrushes(RoadBrushCount)
        RoadBrushes(RoadBrushCount) = NewRoadBrush
        RoadBrushCount += 1
    End Sub

    Public Sub RoadBrush_Remove(Num As Integer)

        RoadBrushCount -= 1
        If Num <> RoadBrushCount Then
            RoadBrushes(Num) = RoadBrushes(RoadBrushCount)
        End If
        ReDim Preserve RoadBrushes(RoadBrushCount - 1)
    End Sub

    Public Sub Road_Add(NewRoad As clsRoad)

        NewRoad.Num = RoadCount
        ReDim Preserve Roads(RoadCount)
        Roads(RoadCount) = NewRoad
        RoadCount += 1
    End Sub

    Public Sub Road_Remove(Num As Integer)

        Roads(Num).Num = -1
        RoadCount -= 1
        If Num <> RoadCount Then
            Roads(Num) = Roads(RoadCount)
            Roads(Num).Num = Num
        End If
        ReDim Preserve Roads(RoadCount - 1)
    End Sub
End Class
