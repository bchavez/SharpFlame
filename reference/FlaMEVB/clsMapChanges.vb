
Partial Public Class clsMap

    Public Class clsPointChanges

        Public PointIsChanged(,) As Boolean
        Public ChangedPoints As New SimpleList(Of clsXY_int)

        Public Sub New(PointSize As sXY_int)

            ReDim PointIsChanged(PointSize.X - 1, PointSize.Y - 1)
            ChangedPoints.MinSize = PointSize.X * PointSize.Y
            ChangedPoints.Clear()
        End Sub

        Public Sub Changed(Num As sXY_int)

            If Not PointIsChanged(Num.X, Num.Y) Then
                PointIsChanged(Num.X, Num.Y) = True
                ChangedPoints.Add(New clsXY_int(Num))
            End If
        End Sub

        Public Sub SetAllChanged()
            Dim X As Integer
            Dim Y As Integer
            Dim Num As sXY_int

            For Y = 0 To PointIsChanged.GetUpperBound(1)
                Num.Y = Y
                For X = 0 To PointIsChanged.GetUpperBound(0)
                    Num.X = X
                    Changed(Num)
                Next
            Next
        End Sub

        Public Sub Clear()
            Dim Point As clsXY_int

            For Each Point In ChangedPoints
                PointIsChanged(Point.X, Point.Y) = False
            Next
            ChangedPoints.Clear()
        End Sub

        Public Sub PerformTool(Tool As clsMap.clsAction)
            Dim Point As clsXY_int

            For Each Point In ChangedPoints
                Tool.PosNum = Point.XY
                Tool.ActionPerform()
            Next
        End Sub
    End Class

    Public MustInherit Class clsMapTileChanges
        Inherits clsPointChanges

        Public Map As clsMap
        Public Terrain As clsTerrain

        Public Sub New(Map As clsMap, PointSize As sXY_int)
            MyBase.New(PointSize)

            Me.Map = Map
            Me.Terrain = Map.Terrain
        End Sub

        Public Sub Deallocate()

            Map = Nothing
        End Sub

        Public MustOverride Sub TileChanged(Num As sXY_int)

        Public Sub VertexChanged(Num As sXY_int)

            If Num.X > 0 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y))
                End If
            End If
            If Num.X < Terrain.TileSize.X Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(Num)
                End If
            End If
        End Sub

        Public Sub VertexAndNormalsChanged(Num As sXY_int)

            If Num.X > 1 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 2, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 2, Num.Y))
                End If
            End If
            If Num.X > 0 Then
                If Num.Y > 1 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 2))
                End If
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y))
                End If
                If Num.Y < Terrain.TileSize.Y - 1 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y + 1))
                End If
            End If
            If Num.X < Terrain.TileSize.X Then
                If Num.Y > 1 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 2))
                End If
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(Num)
                End If
                If Num.Y < Terrain.TileSize.Y - 1 Then
                    TileChanged(New sXY_int(Num.X, Num.Y + 1))
                End If
            End If
            If Num.X < Terrain.TileSize.X - 1 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X + 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X + 1, Num.Y))
                End If
            End If
        End Sub

        Public Sub SideHChanged(Num As sXY_int)

            If Num.Y > 0 Then
                TileChanged(New sXY_int(Num.X, Num.Y - 1))
            End If
            If Num.Y < Map.Terrain.TileSize.Y Then
                TileChanged(Num)
            End If
        End Sub

        Public Sub SideVChanged(Num As sXY_int)

            If Num.X > 0 Then
                TileChanged(New sXY_int(Num.X - 1, Num.Y))
            End If
            If Num.X < Map.Terrain.TileSize.X Then
                TileChanged(Num)
            End If
        End Sub
    End Class

    Public Class clsSectorChanges
        Inherits clsMap.clsMapTileChanges

        Public Sub New(Map As clsMap)
            MyBase.New(Map, Map.SectorCount)

        End Sub

        Public Overrides Sub TileChanged(Num As sXY_int)
            Dim SectorNum As sXY_int

            SectorNum = Map.GetTileSectorNum(Num)
            Changed(SectorNum)
        End Sub
    End Class

    Public Class clsAutoTextureChanges
        Inherits clsMap.clsMapTileChanges

        Public Sub New(Map As clsMap)
            MyBase.New(Map, Map.Terrain.TileSize)

        End Sub

        Public Overrides Sub TileChanged(Num As sXY_int)

            Changed(Num)
        End Sub
    End Class

    Public SectorGraphicsChanges As clsSectorChanges
    Public SectorUnitHeightsChanges As clsSectorChanges
    Public SectorTerrainUndoChanges As clsSectorChanges
    Public AutoTextureChanges As clsAutoTextureChanges

    Public Class clsTerrainUpdate

        Public Vertices As clsPointChanges
        Public Tiles As clsPointChanges
        Public SidesH As clsPointChanges
        Public SidesV As clsPointChanges

        Public Sub Deallocate()


        End Sub

        Public Sub New(TileSize As sXY_int)

            Vertices = New clsPointChanges(New sXY_int(TileSize.X + 1, TileSize.Y + 1))
            Tiles = New clsPointChanges(New sXY_int(TileSize.X, TileSize.Y))
            SidesH = New clsPointChanges(New sXY_int(TileSize.X, TileSize.Y + 1))
            SidesV = New clsPointChanges(New sXY_int(TileSize.X + 1, TileSize.Y))
        End Sub

        Public Sub SetAllChanged()

            Vertices.SetAllChanged()
            Tiles.SetAllChanged()
            SidesH.SetAllChanged()
            SidesV.SetAllChanged()
        End Sub

        Public Sub ClearAll()

            Vertices.Clear()
            Tiles.Clear()
            SidesH.Clear()
            SidesV.Clear()
        End Sub
    End Class

    Public TerrainInterpretChanges As clsTerrainUpdate
End Class
