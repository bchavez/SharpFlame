Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Public frmMainLink As New ConnectedListLink(Of clsMap, frmMain)(Me)

    Public Class clsTerrain

        Public Structure Vertex
            Public Height As Byte
            Public Terrain As clsPainter.clsTerrain
        End Structure

        Public Structure Tile
            Public Structure sTexture
                Public TextureNum As Integer
                Public Orientation As sTileOrientation
            End Structure
            Public Texture As sTexture
            Public Tri As Boolean
            Public TriTopLeftIsCliff As Boolean
            Public TriTopRightIsCliff As Boolean
            Public TriBottomLeftIsCliff As Boolean
            Public TriBottomRightIsCliff As Boolean
            Public Terrain_IsCliff As Boolean
            Public DownSide As sTileDirection

            Public Sub Copy(TileToCopy As Tile)

                Texture = TileToCopy.Texture
                Tri = TileToCopy.Tri
                TriTopLeftIsCliff = TileToCopy.TriTopLeftIsCliff
                TriTopRightIsCliff = TileToCopy.TriTopRightIsCliff
                TriBottomLeftIsCliff = TileToCopy.TriBottomLeftIsCliff
                TriBottomRightIsCliff = TileToCopy.TriBottomRightIsCliff
                Terrain_IsCliff = TileToCopy.Terrain_IsCliff
                DownSide = TileToCopy.DownSide
            End Sub

            Public Sub TriCliffAddDirection(Direction As sTileDirection)

                If Direction.X = 0 Then
                    If Direction.Y = 0 Then
                        TriTopLeftIsCliff = True
                    ElseIf Direction.Y = 2 Then
                        TriBottomLeftIsCliff = True
                    Else
                        Stop
                    End If
                ElseIf Direction.X = 2 Then
                    If Direction.Y = 0 Then
                        TriTopRightIsCliff = True
                    ElseIf Direction.Y = 2 Then
                        TriBottomRightIsCliff = True
                    Else
                        Stop
                    End If
                Else
                    Stop
                End If
            End Sub
        End Structure

        Public Structure Side
            Public Road As clsPainter.clsRoad
        End Structure

        Public TileSize As sXY_int

        Public Vertices(,) As clsMap.clsTerrain.Vertex
        Public Tiles(,) As clsMap.clsTerrain.Tile
        Public SideH(,) As clsMap.clsTerrain.Side
        Public SideV(,) As clsMap.clsTerrain.Side

        Public Sub New(NewSize As sXY_int)

            TileSize = NewSize

            ReDim Vertices(TileSize.X, TileSize.Y)
            ReDim Tiles(TileSize.X - 1, TileSize.Y - 1)
            ReDim SideH(TileSize.X - 1, TileSize.Y)
            ReDim SideV(TileSize.X, TileSize.Y - 1)
            Dim X As Integer
            Dim Y As Integer

            For Y = 0 To TileSize.Y - 1
                For X = 0 To TileSize.X - 1
                    Tiles(X, Y).Texture.TextureNum = -1
                    Tiles(X, Y).DownSide = TileDirection_None
                Next
            Next
        End Sub

    End Class

    Public Terrain As clsTerrain

    Public Class clsSector
        Public Pos As sXY_int
        Public GLList_Textured As Integer
        Public GLList_Wireframe As Integer
        Public Units As New ConnectedList(Of clsMap.clsUnitSectorConnection, clsMap.clsSector)(Me)

        Public Sub DeleteLists()

            If GLList_Textured <> 0 Then
                GL.DeleteLists(GLList_Textured, 1)
                GLList_Textured = 0
            End If
            If GLList_Wireframe <> 0 Then
                GL.DeleteLists(GLList_Wireframe, 1)
                GLList_Wireframe = 0
            End If
        End Sub

        Public Sub Deallocate()

            Units.Deallocate()
        End Sub

        Public Sub New(NewPos As sXY_int)

            Pos = NewPos
        End Sub
    End Class
    Public Sectors(-1, -1) As clsSector
    Public SectorCount As sXY_int

    Public Class clsShadowSector
        Public Num As sXY_int
        Public Terrain As New clsTerrain(New sXY_int(SectorTileSize, SectorTileSize))
    End Class
    Public ShadowSectors(-1, -1) As clsShadowSector

    Public Class clsUnitChange
        Public Enum enumType As Byte
            Added
            Deleted
        End Enum
        Public Type As enumType
        Public Unit As clsUnit
    End Class

    Public Class clsGatewayChange
        Public Enum enumType As Byte
            Added
            Deleted
        End Enum
        Public Type As enumType
        Public Gateway As clsGateway
    End Class

    Public Class clsUndo
        Public Name As String
        Public ChangedSectors As New SimpleList(Of clsShadowSector)
        Public UnitChanges As New SimpleList(Of clsMap.clsUnitChange)
        Public GatewayChanges As New SimpleList(Of clsMap.clsGatewayChange)
    End Class
    Public Undos As SimpleClassList(Of clsMap.clsUndo)
    Public UndoPosition As Integer

    Public UnitChanges As SimpleClassList(Of clsMap.clsUnitChange)
    Public GatewayChanges As SimpleClassList(Of clsMap.clsGatewayChange)

    Public HeightMultiplier As Integer = DefaultHeightMultiplier

    Public ReadOnly Property ReadyForUserInput As Boolean
        Get
            Return _ReadyForUserInput
        End Get
    End Property
    Private _ReadyForUserInput As Boolean = False

    Public SelectedUnits As ConnectedList(Of clsMap.clsUnit, clsMap)
    Public Selected_Tile_A As clsXY_int
    Public Selected_Tile_B As clsXY_int
    Public Selected_Area_VertexA As clsXY_int
    Public Selected_Area_VertexB As clsXY_int
    Public Unit_Selected_Area_VertexA As clsXY_int

    Public Minimap_GLTexture As Integer
    Public Minimap_Texture_Size As Integer

    Public Class clsMessage
        Public Text As String
        Private _CreatedDate As Date = Now

        Public ReadOnly Property CreatedDate As Date
            Get
                Return _CreatedDate
            End Get
        End Property
    End Class
    Public Messages As SimpleClassList(Of clsMessage)

    Public Tileset As clsTileset

    Public Class clsPathInfo
        Private _Path As String
        Private _IsFMap As Boolean

        Public ReadOnly Property Path As String
            Get
                Return _Path
            End Get
        End Property

        Public ReadOnly Property IsFMap As Boolean
            Get
                Return _IsFMap
            End Get
        End Property

        Public Sub New(Path As String, IsFMap As Boolean)

            _Path = Path
            _IsFMap = IsFMap
        End Sub
    End Class
    Public PathInfo As clsPathInfo

    Public ChangedSinceSave As Boolean = False
    Public Event Changed()

    Public Class clsAutoSave
        Public ChangeCount As Integer
        Public SavedDate As Date

        Public Sub New()

            SavedDate = Now
        End Sub
    End Class
    Public AutoSave As New clsAutoSave

    Public Painter As New clsPainter

    Public Tile_TypeNum(-1) As Byte

    Public Class clsGateway
        Public MapLink As New ConnectedListLink(Of clsGateway, clsMap)(Me)
        Public PosA As sXY_int
        Public PosB As sXY_int

        Public Function IsOffMap() As Boolean
            Dim TerrainSize As sXY_int = MapLink.Source.Terrain.TileSize

            Return PosA.X < 0 _
                Or PosA.X >= TerrainSize.X _
                Or PosA.Y < 0 _
                Or PosA.Y >= TerrainSize.Y _
                Or PosB.X < 0 _
                Or PosB.X >= TerrainSize.X _
                Or PosB.Y < 0 _
                Or PosB.Y >= TerrainSize.Y
        End Function

        Public Sub Deallocate()

            MapLink.Deallocate()
        End Sub
    End Class
    Public Gateways As New ConnectedList(Of clsGateway, clsMap)(Me)

    Public Sub New()

        Initialize()
    End Sub

    Public Sub New(TileSize As sXY_int)

        Initialize()

        TerrainBlank(TileSize)
        TileType_Reset()
    End Sub

    Public Sub Initialize()

        MakeMinimapTimer = New Timer
        MakeMinimapTimer.Interval = MinimapDelay

        MakeDefaultUnitGroups()
        ScriptPositions.MaintainOrder = True
        ScriptAreas.MaintainOrder = True
    End Sub

    Public Sub New(MapToCopy As clsMap, Offset As sXY_int, Area As sXY_int)
        Dim EndX As Integer
        Dim EndY As Integer
        Dim X As Integer
        Dim Y As Integer

        Initialize()

        'make some map data for selection

        EndX = Math.Min(MapToCopy.Terrain.TileSize.X - Offset.X, Area.X)
        EndY = Math.Min(MapToCopy.Terrain.TileSize.Y - Offset.Y, Area.Y)

        Terrain = New clsTerrain(Area)

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Terrain.Tiles(X, Y).Texture.TextureNum = -1
            Next
        Next

        For Y = 0 To EndY
            For X = 0 To EndX
                Terrain.Vertices(X, Y).Height = MapToCopy.Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height
                Terrain.Vertices(X, Y).Terrain = MapToCopy.Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain
            Next
        Next
        For Y = 0 To EndY - 1
            For X = 0 To EndX - 1
                Terrain.Tiles(X, Y).Copy(MapToCopy.Terrain.Tiles(Offset.X + X, Offset.Y + Y))
            Next
        Next
        For Y = 0 To EndY
            For X = 0 To EndX - 1
                Terrain.SideH(X, Y).Road = MapToCopy.Terrain.SideH(Offset.X + X, Offset.Y + Y).Road
            Next
        Next
        For Y = 0 To EndY - 1
            For X = 0 To EndX
                Terrain.SideV(X, Y).Road = MapToCopy.Terrain.SideV(Offset.X + X, Offset.Y + Y).Road
            Next
        Next

        SectorCount.X = CInt(Math.Ceiling(Area.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Area.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        Dim PosDif As sXY_int
        Dim NewUnitAdd As New clsMap.clsUnitAdd
        NewUnitAdd.Map = Me
        Dim NewUnit As clsMap.clsUnit

        Dim Gateway As clsGateway
        For Each Gateway In MapToCopy.Gateways
            GatewayCreate(New sXY_int(Gateway.PosA.X - Offset.X, Gateway.PosA.Y - Offset.Y), New sXY_int(Gateway.PosB.X - Offset.X, Gateway.PosB.Y - Offset.Y))
        Next

        PosDif.X = -Offset.X * TerrainGridSpacing
        PosDif.Y = -Offset.Y * TerrainGridSpacing
        Dim Unit As clsUnit
        Dim NewPos As sXY_int
        For Each Unit In MapToCopy.Units
            NewPos = Unit.Pos.Horizontal + PosDif
            If PosIsOnMap(NewPos) Then
                NewUnit = New clsUnit(Unit, Me)
                NewUnit.Pos.Horizontal = NewPos
                NewUnitAdd.NewUnit = NewUnit
                NewUnitAdd.Label = Unit.Label
                NewUnitAdd.Perform()
            End If
        Next
    End Sub

    Protected Sub TerrainBlank(TileSize As sXY_int)
        Dim X As Integer
        Dim Y As Integer

        Terrain = New clsTerrain(TileSize)
        SectorCount.X = CInt(Math.Ceiling(Terrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Terrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next
    End Sub

    Public Function GetTerrainTri(Horizontal As sXY_int) As Boolean
        Dim X1 As Integer
        Dim Y1 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Return False
            Else
                Return True
            End If
        Else
            If InTileZ <= InTileX Then
                Return True
            Else
                Return False
            End If
        End If
    End Function

    Public Function GetTerrainSlopeAngle(Horizontal As sXY_int) As Double
        Dim X1 As Integer
        Dim X2 As Integer
        Dim Y1 As Integer
        Dim Y2 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer
        Dim GradientX As Double
        Dim GradientY As Double
        Dim Offset As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim XYZ_dbl3 As Matrix3D.XYZ_dbl
        Dim AnglePY As Matrix3D.AnglePY

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        X2 = Clamp_int(XG + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(YG + 1, 0, Terrain.TileSize.Y)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = Terrain.Vertices(X1, Y1).Height
                GradientX = Terrain.Vertices(X2, Y1).Height - Offset
                GradientY = Terrain.Vertices(X1, Y2).Height - Offset
            Else
                Offset = Terrain.Vertices(X2, Y2).Height
                GradientX = Terrain.Vertices(X1, Y2).Height - Offset
                GradientY = Terrain.Vertices(X2, Y1).Height - Offset
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = Terrain.Vertices(X2, Y1).Height
                GradientX = Terrain.Vertices(X1, Y1).Height - Offset
                GradientY = Terrain.Vertices(X2, Y2).Height - Offset
            Else
                Offset = Terrain.Vertices(X1, Y2).Height
                GradientX = Terrain.Vertices(X2, Y2).Height - Offset
                GradientY = Terrain.Vertices(X1, Y1).Height - Offset
            End If
        End If

        XYZ_dbl.X = TerrainGridSpacing
        XYZ_dbl.Y = GradientX * HeightMultiplier
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = GradientY * HeightMultiplier
        XYZ_dbl2.Z = TerrainGridSpacing
        Matrix3D.VectorCrossProduct(XYZ_dbl, XYZ_dbl2, XYZ_dbl3)
        If XYZ_dbl3.X <> 0.0# Or XYZ_dbl3.Z <> 0.0# Then
            Matrix3D.VectorToPY(XYZ_dbl3, AnglePY)
            Return RadOf90Deg - Math.Abs(AnglePY.Pitch)
        Else
            Return 0.0#
        End If
    End Function

    Public Function GetTerrainHeight(Horizontal As sXY_int) As Double
        Dim X1 As Integer
        Dim X2 As Integer
        Dim Y1 As Integer
        Dim Y2 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer
        Dim GradientX As Double
        Dim GradientY As Double
        Dim Offset As Double
        Dim RatioX As Double
        Dim RatioY As Double

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        X2 = Clamp_int(XG + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(YG + 1, 0, Terrain.TileSize.Y)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = Terrain.Vertices(X1, Y1).Height
                GradientX = Terrain.Vertices(X2, Y1).Height - Offset
                GradientY = Terrain.Vertices(X1, Y2).Height - Offset
                RatioX = InTileX
                RatioY = InTileZ
            Else
                Offset = Terrain.Vertices(X2, Y2).Height
                GradientX = Terrain.Vertices(X1, Y2).Height - Offset
                GradientY = Terrain.Vertices(X2, Y1).Height - Offset
                RatioX = 1.0# - InTileX
                RatioY = 1.0# - InTileZ
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = Terrain.Vertices(X2, Y1).Height
                GradientX = Terrain.Vertices(X1, Y1).Height - Offset
                GradientY = Terrain.Vertices(X2, Y2).Height - Offset
                RatioX = 1.0# - InTileX
                RatioY = InTileZ
            Else
                Offset = Terrain.Vertices(X1, Y2).Height
                GradientX = Terrain.Vertices(X2, Y2).Height - Offset
                GradientY = Terrain.Vertices(X1, Y1).Height - Offset
                RatioX = InTileX
                RatioY = 1.0# - InTileZ
            End If
        End If
        Return (Offset + GradientX * RatioX + GradientY * RatioY) * HeightMultiplier
    End Function

    Public Function TerrainVertexNormalCalc(X As Integer, Y As Integer) As sXYZ_sng
        Dim ReturnResult As sXYZ_sng
        Dim TerrainHeightX1 As Integer
        Dim TerrainHeightX2 As Integer
        Dim TerrainHeightY1 As Integer
        Dim TerrainHeightY2 As Integer
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim dblTemp As Double

        X2 = Clamp_int(X - 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y, 0, Terrain.TileSize.Y)
        TerrainHeightX1 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y, 0, Terrain.TileSize.Y)
        TerrainHeightX2 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y - 1, 0, Terrain.TileSize.Y)
        TerrainHeightY1 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y + 1, 0, Terrain.TileSize.Y)
        TerrainHeightY2 = Terrain.Vertices(X2, Y2).Height
        XYZ_dbl.X = (TerrainHeightX1 - TerrainHeightX2) * HeightMultiplier
        XYZ_dbl.Y = TerrainGridSpacing * 2.0#
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = TerrainGridSpacing * 2.0#
        XYZ_dbl2.Z = (TerrainHeightY1 - TerrainHeightY2) * HeightMultiplier
        XYZ_dbl.X = XYZ_dbl.X + XYZ_dbl2.X
        XYZ_dbl.Y = XYZ_dbl.Y + XYZ_dbl2.Y
        XYZ_dbl.Z = XYZ_dbl.Z + XYZ_dbl2.Z
        dblTemp = Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Y * XYZ_dbl.Y + XYZ_dbl.Z * XYZ_dbl.Z)
        ReturnResult.X = CSng(XYZ_dbl.X / dblTemp)
        ReturnResult.Y = CSng(XYZ_dbl.Y / dblTemp)
        ReturnResult.Z = CSng(XYZ_dbl.Z / dblTemp)
        Return ReturnResult
    End Function

    Public Sub SectorAll_GLLists_Delete()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y).DeleteLists()
            Next
        Next
    End Sub

    Public Overridable Sub Deallocate()

        CancelUserInput()

        MakeMinimapTimer.Enabled = False
        MakeMinimapTimer.Dispose()
        MakeMinimapTimer = Nothing

        frmMainLink.Deallocate()
        frmMainLink = Nothing

        UnitGroups.Deallocate()
        UnitGroups = Nothing

        Do While Units.Count > 0
            Units(0).Deallocate()
        Loop
        Units.Deallocate()
        Units = Nothing

        Do While Gateways.Count > 0
            Gateways(0).Deallocate()
        Loop
        Gateways.Deallocate()
        Gateways = Nothing

        Do While ScriptPositions.Count > 0
            ScriptPositions(0).Deallocate()
        Loop
        ScriptPositions.Deallocate()
        ScriptPositions = Nothing

        Do While ScriptAreas.Count > 0
            ScriptAreas(0).Deallocate()
        Loop
        ScriptAreas.Deallocate()
        ScriptAreas = Nothing
    End Sub

    Public Sub TerrainResize(Offset As sXY_int, Size As sXY_int)
        Dim StartX As Integer
        Dim StartY As Integer
        Dim EndX As Integer
        Dim EndY As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim NewTerrain As New clsTerrain(Size)

        StartX = Math.Max(0 - Offset.X, 0)
        StartY = Math.Max(0 - Offset.Y, 0)
        EndX = Math.Min(Terrain.TileSize.X - Offset.X, Size.X)
        EndY = Math.Min(Terrain.TileSize.Y - Offset.Y, Size.Y)

        For Y = 0 To NewTerrain.TileSize.Y - 1
            For X = 0 To NewTerrain.TileSize.X - 1
                NewTerrain.Tiles(X, Y).Texture.TextureNum = -1
            Next
        Next

        For Y = StartY To EndY
            For X = StartX To EndX
                NewTerrain.Vertices(X, Y).Height = Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height
                NewTerrain.Vertices(X, Y).Terrain = Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX - 1
                NewTerrain.Tiles(X, Y).Copy(Terrain.Tiles(Offset.X + X, Offset.Y + Y))
            Next
        Next
        For Y = StartY To EndY
            For X = StartX To EndX - 1
                NewTerrain.SideH(X, Y).Road = Terrain.SideH(Offset.X + X, Offset.Y + Y).Road
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX
                NewTerrain.SideV(X, Y).Road = Terrain.SideV(Offset.X + X, Offset.Y + Y).Road
            Next
        Next

        Dim PosDifX As Integer
        Dim PosDifZ As Integer
        Dim Unit As clsUnit
        Dim Gateway As clsGateway

        PosDifX = -Offset.X * TerrainGridSpacing
        PosDifZ = -Offset.Y * TerrainGridSpacing
        For Each Unit In Units
            Unit.Pos.Horizontal.X += PosDifX
            Unit.Pos.Horizontal.Y += PosDifZ
        Next
        For Each Gateway In Gateways
            Gateway.PosA.X -= Offset.X
            Gateway.PosA.Y -= Offset.Y
            Gateway.PosB.X -= Offset.X
            Gateway.PosB.Y -= Offset.Y
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        Dim Position As Integer
        For Each Unit In Units.GetItemsAsSimpleList
            Position = Unit.MapLink.ArrayPosition
            If Not PosIsWithinTileArea(Units(Position).Pos.Horizontal, ZeroPos, NewTerrain.TileSize) Then
                UnitRemove(Position)
            End If
        Next

        Terrain = NewTerrain

        For Each Gateway In Gateways.GetItemsAsSimpleList
            If Gateway.IsOffMap Then
                Gateway.Deallocate()
            End If
        Next

        Dim PosOffset As New sXY_int(Offset.X * TerrainGridSpacing, Offset.Y * TerrainGridSpacing)

        Dim ScriptPosition As clsScriptPosition
        For Each ScriptPosition In ScriptPositions.GetItemsAsSimpleList
            ScriptPosition.MapResizing(PosOffset)
        Next

        Dim ScriptArea As clsScriptArea
        For Each ScriptArea In ScriptAreas.GetItemsAsSimpleList
            ScriptArea.MapResizing(PosOffset)
        Next

        If _ReadyForUserInput Then
            CancelUserInput()
            InitializeUserInput()
        End If
    End Sub

    Public Sub Sector_GLList_Make(X As Integer, Y As Integer)
        Dim TileX As Integer
        Dim TileY As Integer
        Dim StartX As Integer
        Dim StartY As Integer
        Dim FinishX As Integer
        Dim FinishY As Integer

        Sectors(X, Y).DeleteLists()

        StartX = X * SectorTileSize
        StartY = Y * SectorTileSize
        FinishX = Math.Min(StartX + SectorTileSize, Terrain.TileSize.X) - 1
        FinishY = Math.Min(StartY + SectorTileSize, Terrain.TileSize.Y) - 1

        Sectors(X, Y).GLList_Textured = GL.GenLists(1)
        GL.NewList(Sectors(X, Y).GLList_Textured, ListMode.Compile)

        If Draw_Units Then
            Dim IsBasePlate(SectorTileSize - 1, SectorTileSize - 1) As Boolean
            Dim Unit As clsUnit
            Dim StructureType As clsStructureType
            Dim Footprint As sXY_int
            Dim Connection As clsUnitSectorConnection
            Dim FootprintStart As sXY_int
            Dim FootprintFinish As sXY_int
            For Each Connection In Sectors(X, Y).Units
                Unit = Connection.Unit
                If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                    StructureType = CType(Unit.Type, clsStructureType)
                    If StructureType.StructureBasePlate IsNot Nothing Then
                        Footprint = StructureType.GetFootprintSelected(Unit.Rotation)
                        GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, FootprintStart, FootprintFinish)
                        For TileY = Math.Max(FootprintStart.Y, StartY) To Math.Min(FootprintFinish.Y, FinishY)
                            For TileX = Math.Max(FootprintStart.X, StartX) To Math.Min(FootprintFinish.X, FinishX)
                                IsBasePlate(TileX - StartX, TileY - StartY) = True
                            Next
                        Next
                    End If
                End If
            Next
            Dim drawTile As New clsDrawTileOld
            drawTile.Map = Me
            For TileY = StartY To FinishY
                drawTile.TileY = TileY
                For TileX = StartX To FinishX
                    If Not IsBasePlate(TileX - StartX, TileY - StartY) Then
                        drawTile.TileX = TileX
                        drawTile.Perform()
                    End If
                Next
            Next
        Else
            Dim drawTile As New clsDrawTileOld
            drawTile.Map = Me
            For TileY = StartY To FinishY
                drawTile.TileY = TileY
                For TileX = StartX To FinishX
                    drawTile.TileX = TileX
                    drawTile.Perform()
                Next
            Next
        End If

        GL.EndList()

        Sectors(X, Y).GLList_Wireframe = GL.GenLists(1)
        GL.NewList(Sectors(X, Y).GLList_Wireframe, ListMode.Compile)

        For TileY = StartY To FinishY
            For TileX = StartX To FinishX
                DrawTileWireframe(TileX, TileY)
            Next
        Next

        GL.EndList()
    End Sub

    Public Sub DrawTileWireframe(TileX As Integer, TileY As Integer)
        Dim TileTerrainHeight(3) As Double
        Dim Vertex0 As sXYZ_sng
        Dim Vertex1 As sXYZ_sng
        Dim Vertex2 As sXYZ_sng
        Dim Vertex3 As sXYZ_sng

        TileTerrainHeight(0) = Terrain.Vertices(TileX, TileY).Height
        TileTerrainHeight(1) = Terrain.Vertices(TileX + 1, TileY).Height
        TileTerrainHeight(2) = Terrain.Vertices(TileX, TileY + 1).Height
        TileTerrainHeight(3) = Terrain.Vertices(TileX + 1, TileY + 1).Height

        Vertex0.X = CSng(TileX * TerrainGridSpacing)
        Vertex0.Y = CSng(TileTerrainHeight(0) * HeightMultiplier)
        Vertex0.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex1.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex1.Y = CSng(TileTerrainHeight(1) * HeightMultiplier)
        Vertex1.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex2.X = CSng(TileX * TerrainGridSpacing)
        Vertex2.Y = CSng(TileTerrainHeight(2) * HeightMultiplier)
        Vertex2.Z = CSng(-(TileY + 1) * TerrainGridSpacing)
        Vertex3.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex3.Y = CSng(TileTerrainHeight(3) * HeightMultiplier)
        Vertex3.Z = CSng(-(TileY + 1) * TerrainGridSpacing)

        GL.Begin(BeginMode.Lines)
        If Terrain.Tiles(TileX, TileY).Tri Then
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)

            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
        Else
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)

            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
        End If
        GL.End()
    End Sub

    Public Sub DrawTileOrientation(Tile As sXY_int)
        Dim TileOrientation As sTileOrientation
        Dim UnrotatedPos As sXY_int
        Dim Vertex0 As sWorldPos
        Dim Vertex1 As sWorldPos
        Dim Vertex2 As sWorldPos

        TileOrientation = Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation

        UnrotatedPos.X = 32
        UnrotatedPos.Y = 32
        Vertex0 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        UnrotatedPos.X = 64
        UnrotatedPos.Y = 32
        Vertex1 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        UnrotatedPos.X = 64
        UnrotatedPos.Y = 64
        Vertex2 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        GL.Vertex3(Vertex0.Horizontal.X, Vertex0.Altitude, Vertex0.Horizontal.Y)
        GL.Vertex3(Vertex1.Horizontal.X, Vertex1.Altitude, Vertex1.Horizontal.Y)
        GL.Vertex3(Vertex2.Horizontal.X, Vertex2.Altitude, Vertex2.Horizontal.Y)
    End Sub

    Protected Sub MinimapTextureFill(Texture As clsMinimapTexture)
        Dim X As Integer
        Dim Y As Integer
        Dim Low As sXY_int
        Dim High As sXY_int
        Dim Footprint As sXY_int
        Dim Flag As Boolean
        Dim UnitMap(Texture.Size.Y - 1, Texture.Size.X - 1) As Boolean
        Dim sngTexture(Texture.Size.Y - 1, Texture.Size.X - 1, 2) As Single
        Dim Alpha As Single
        Dim AntiAlpha As Single
        Dim RGB_sng As sRGB_sng

        If frmMainInstance.menuMiniShowTex.Checked Then
            If Tileset IsNot Nothing Then
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 And Terrain.Tiles(X, Y).Texture.TextureNum < Tileset.TileCount Then
                            sngTexture(Y, X, 0) = Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).AverageColour.Red
                            sngTexture(Y, X, 1) = Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).AverageColour.Green
                            sngTexture(Y, X, 2) = Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).AverageColour.Blue
                        End If
                    Next
                Next
            End If
            If frmMainInstance.menuMiniShowHeight.Checked Then
                Dim Height As Single
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        Height = (CInt(Terrain.Vertices(X, Y).Height) + Terrain.Vertices(X + 1, Y).Height + Terrain.Vertices(X, Y + 1).Height + Terrain.Vertices(X + 1, Y + 1).Height) / 1020.0F
                        sngTexture(Y, X, 0) = (sngTexture(Y, X, 0) * 2.0F + Height) / 3.0F
                        sngTexture(Y, X, 1) = (sngTexture(Y, X, 1) * 2.0F + Height) / 3.0F
                        sngTexture(Y, X, 2) = (sngTexture(Y, X, 2) * 2.0F + Height) / 3.0F
                    Next
                Next
            End If
        ElseIf frmMainInstance.menuMiniShowHeight.Checked Then
            Dim Height As Single
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Height = (CInt(Terrain.Vertices(X, Y).Height) + Terrain.Vertices(X + 1, Y).Height + Terrain.Vertices(X, Y + 1).Height + Terrain.Vertices(X + 1, Y + 1).Height) / 1020.0F
                    sngTexture(Y, X, 0) = Height
                    sngTexture(Y, X, 1) = Height
                    sngTexture(Y, X, 2) = Height
                Next
            Next
        Else
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    sngTexture(Y, X, 0) = 0.0F
                    sngTexture(Y, X, 1) = 0.0F
                    sngTexture(Y, X, 2) = 0.0F
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowCliffs.Checked Then
            If Tileset IsNot Nothing Then
                Alpha = Settings.MinimapCliffColour.Alpha
                AntiAlpha = 1.0F - Alpha
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 And Terrain.Tiles(X, Y).Texture.TextureNum < Tileset.TileCount Then
                            If Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Cliff Then
                                sngTexture(Y, X, 0) = sngTexture(Y, X, 0) * AntiAlpha + Settings.MinimapCliffColour.Red * Alpha
                                sngTexture(Y, X, 1) = sngTexture(Y, X, 1) * AntiAlpha + Settings.MinimapCliffColour.Green * Alpha
                                sngTexture(Y, X, 2) = sngTexture(Y, X, 2) * AntiAlpha + Settings.MinimapCliffColour.Blue * Alpha
                            End If
                        End If
                    Next
                Next
            End If
        End If
        If frmMainInstance.menuMiniShowGateways.Checked Then
            Dim Gateway As clsGateway
            For Each Gateway In Gateways
                ReorderXY(Gateway.PosA, Gateway.PosB, Low, High)
                For Y = Low.Y To High.Y
                    For X = Low.X To High.X
                        sngTexture(Y, X, 0) = 1.0F
                        sngTexture(Y, X, 1) = 1.0F
                        sngTexture(Y, X, 2) = 0.0F
                    Next
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowUnits.Checked Then
            'units that are not selected
            Dim Unit As clsUnit
            For Each Unit In Units
                Flag = True
                If Unit.Type.UnitType_frmMainSelectedLink.IsConnected Then
                    Flag = False
                Else
                    Footprint = Unit.Type.GetFootprintSelected(Unit.Rotation)
                End If
                If Flag Then
                    GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, Low, High)
                    For Y = Low.Y To High.Y
                        For X = Low.X To High.X
                            If Not UnitMap(Y, X) Then
                                UnitMap(Y, X) = True
                                If Settings.MinimapTeamColours Then
                                    If Settings.MinimapTeamColoursExceptFeatures And Unit.Type.Type = clsUnitType.enumType.Feature Then
                                        sngTexture(Y, X, 0) = MinimapFeatureColour.Red
                                        sngTexture(Y, X, 1) = MinimapFeatureColour.Green
                                        sngTexture(Y, X, 2) = MinimapFeatureColour.Blue
                                    Else
                                        RGB_sng = GetUnitGroupMinimapColour(Unit.UnitGroup)
                                        sngTexture(Y, X, 0) = RGB_sng.Red
                                        sngTexture(Y, X, 1) = RGB_sng.Green
                                        sngTexture(Y, X, 2) = RGB_sng.Blue
                                    End If
                                Else
                                    sngTexture(Y, X, 0) = sngTexture(Y, X, 0) * 0.6666667F + 0.333333343F
                                    sngTexture(Y, X, 1) = sngTexture(Y, X, 1) * 0.6666667F
                                    sngTexture(Y, X, 2) = sngTexture(Y, X, 2) * 0.6666667F
                                End If
                            End If
                        Next
                    Next
                End If
            Next
            'reset unit map
            For Y = 0 To Texture.Size.Y - 1
                For X = 0 To Texture.Size.X - 1
                    UnitMap(Y, X) = False
                Next
            Next
            'units that are selected and highlighted
            Alpha = Settings.MinimapSelectedObjectsColour.Alpha
            AntiAlpha = 1.0F - Alpha
            For Each Unit In Units
                Flag = False
                If Unit.Type.UnitType_frmMainSelectedLink.IsConnected Then
                    Flag = True
                    Footprint = Unit.Type.GetFootprintSelected(Unit.Rotation)
                    Footprint.X += 2
                    Footprint.Y += 2
                End If
                If Flag Then
                    GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, Low, High)
                    For Y = Low.Y To High.Y
                        For X = Low.X To High.X
                            If Not UnitMap(Y, X) Then
                                UnitMap(Y, X) = True
                                sngTexture(Y, X, 0) = sngTexture(Y, X, 0) * AntiAlpha + Settings.MinimapSelectedObjectsColour.Red * Alpha
                                sngTexture(Y, X, 1) = sngTexture(Y, X, 1) * AntiAlpha + Settings.MinimapSelectedObjectsColour.Green * Alpha
                                sngTexture(Y, X, 2) = sngTexture(Y, X, 2) * AntiAlpha + Settings.MinimapSelectedObjectsColour.Blue * Alpha
                            End If
                        Next
                    Next
                End If
            Next
        End If
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Texture.Pixels(X, Y) = New sRGBA_sng( _
                    sngTexture(Y, X, 0), _
                    sngTexture(Y, X, 1), _
                    sngTexture(Y, X, 2), _
                    1.0F)
            Next
        Next
    End Sub

    Private MinimapPending As Boolean
    Private WithEvents MakeMinimapTimer As Timer
    Public SuppressMinimap As Boolean

    Private Sub MinimapTimer_Tick(sender As Object, e As EventArgs) Handles MakeMinimapTimer.Tick

        If MainMap IsNot Me Then
            MinimapPending = False
        End If
        If MinimapPending Then
            If Not SuppressMinimap Then
                MinimapPending = False
                MinimapMake()
            End If
        Else
            MakeMinimapTimer.Enabled = False
        End If
    End Sub

    Public Sub MinimapMakeLater()

        If MakeMinimapTimer.Enabled Then
            MinimapPending = True
        Else
            MakeMinimapTimer.Enabled = True
            If SuppressMinimap Then
                MinimapPending = True
            Else
                MinimapMake()
            End If
        End If
    End Sub

    Public Class clsMinimapTexture
        Public InlinePixels() As sRGBA_sng
        Public Size As sXY_int

        Public Property Pixels(X As Integer, Y As Integer) As sRGBA_sng
            Get
                Return InlinePixels(Y * Size.X + X)
            End Get
            Set(value As sRGBA_sng)
                InlinePixels(Y * Size.X + X) = value
            End Set
        End Property

        Public Sub New(Size As sXY_int)

            Me.Size = Size
            ReDim InlinePixels(Size.X * Size.Y - 1)
        End Sub
    End Class

    Private Sub MinimapMake()

        Dim NewTextureSize As Integer = CInt(Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(Terrain.TileSize.X, Terrain.TileSize.Y)) / Math.Log(2.0#))))

        If NewTextureSize <> Minimap_Texture_Size Then
            Minimap_Texture_Size = NewTextureSize
        End If

        Dim Texture As New clsMinimapTexture(New sXY_int(Minimap_Texture_Size, Minimap_Texture_Size))

        MinimapTextureFill(Texture)

        MinimapGLDelete()

        GL.GenTextures(1, Minimap_GLTexture)
        GL.BindTexture(TextureTarget.Texture2D, Minimap_GLTexture)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest)
        GL.TexImage2D(Of sRGBA_sng)(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Minimap_Texture_Size, Minimap_Texture_Size, 0, PixelFormat.Rgba, PixelType.Float, Texture.InlinePixels)

        frmMainInstance.View_DrawViewLater()
    End Sub

    Public Sub MinimapGLDelete()

        If Minimap_GLTexture <> 0 Then
            GL.DeleteTextures(1, Minimap_GLTexture)
            Minimap_GLTexture = 0
        End If
    End Sub

    Public Function GetTileSectorNum(Tile As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = CInt(Int(Tile.X / SectorTileSize))
        Result.Y = CInt(Int(Tile.Y / SectorTileSize))

        Return Result
    End Function

    Public Sub GetTileSectorRange(StartTile As sXY_int, FinishTile As sXY_int, ByRef ResultSectorStart As sXY_int, ByRef ResultSectorFinish As sXY_int)

        ResultSectorStart = GetTileSectorNum(StartTile)
        ResultSectorFinish = GetTileSectorNum(FinishTile)
        ResultSectorStart.X = Clamp_int(ResultSectorStart.X, 0, SectorCount.X - 1)
        ResultSectorStart.Y = Clamp_int(ResultSectorStart.Y, 0, SectorCount.Y - 1)
        ResultSectorFinish.X = Clamp_int(ResultSectorFinish.X, 0, SectorCount.X - 1)
        ResultSectorFinish.Y = Clamp_int(ResultSectorFinish.Y, 0, SectorCount.Y - 1)
    End Sub

    Public Function TileAlignedPos(TileNum As sXY_int, Footprint As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Result.Horizontal.X = CInt((TileNum.X + Footprint.X / 2.0#) * TerrainGridSpacing)
        Result.Horizontal.Y = CInt((TileNum.Y + Footprint.Y / 2.0#) * TerrainGridSpacing)
        Result.Altitude = CInt(GetTerrainHeight(Result.Horizontal))

        Return Result
    End Function

    Public Function TileAlignedPosFromMapPos(Horizontal As sXY_int, Footprint As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Result.Horizontal.X = CInt((Math.Round((Horizontal.X - Footprint.X * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.X / 2.0#) * TerrainGridSpacing)
        Result.Horizontal.Y = CInt((Math.Round((Horizontal.Y - Footprint.Y * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.Y / 2.0#) * TerrainGridSpacing)
        Result.Altitude = CInt(GetTerrainHeight(Result.Horizontal))

        Return Result
    End Function

    Public Sub UnitSectorsCalc(Unit As clsUnit)
        Dim Start As sXY_int
        Dim Finish As sXY_int
        Dim TileStart As sXY_int
        Dim TileFinish As sXY_int
        Dim Connection As clsMap.clsUnitSectorConnection
        Dim X As Integer
        Dim Y As Integer

        GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.Type.GetFootprintSelected(Unit.Rotation), TileStart, TileFinish)
        Start = GetTileSectorNum(TileStart)
        Finish = GetTileSectorNum(TileFinish)
        Start.X = Clamp_int(Start.X, 0, SectorCount.X - 1)
        Start.Y = Clamp_int(Start.Y, 0, SectorCount.Y - 1)
        Finish.X = Clamp_int(Finish.X, 0, SectorCount.X - 1)
        Finish.Y = Clamp_int(Finish.Y, 0, SectorCount.Y - 1)
        Unit.Sectors.Clear()
        For Y = Start.Y To Finish.Y
            For X = Start.X To Finish.X
                Connection = clsMap.clsUnitSectorConnection.Create(Unit, Sectors(X, Y))
            Next
        Next
    End Sub

    Public Sub AutoSaveTest()

        If Not Settings.AutoSaveEnabled Then
            Exit Sub
        End If
        If AutoSave.ChangeCount < Settings.AutoSaveMinChanges Then
            Exit Sub
        End If
        If DateDiff("s", AutoSave.SavedDate, Now) < Settings.AutoSaveMinInterval_s Then
            Exit Sub
        End If

        AutoSave.ChangeCount = 0
        AutoSave.SavedDate = Now

        ShowWarnings(AutoSavePerform())
    End Sub

    Public Function AutoSavePerform() As clsResult
        Dim ReturnResult As New clsResult("Autosave")

        If Not IO.Directory.Exists(AutoSavePath) Then
            Try
                IO.Directory.CreateDirectory(AutoSavePath)
            Catch ex As Exception
                ReturnResult.ProblemAdd("Unable to create directory at " & ControlChars.Quote & AutoSavePath & ControlChars.Quote)
            End Try
        End If

        Dim DateNow As Date = Now
        Dim Path As String

        Path = AutoSavePath & "autosaved-" & InvariantToString_int(DateNow.Year) & "-" & MinDigits(DateNow.Month, 2) & "-" & MinDigits(DateNow.Day, 2) & "-" & MinDigits(DateNow.Hour, 2) & "-" & MinDigits(DateNow.Minute, 2) & "-" & MinDigits(DateNow.Second, 2) & "-" & MinDigits(DateNow.Millisecond, 3) & ".fmap"

        ReturnResult.Add(Write_FMap(Path, False, Settings.AutoSaveCompress))

        Return ReturnResult
    End Function

    Public Sub UndoStepCreate(StepName As String)
        Dim NewUndo As New clsUndo

        NewUndo.Name = StepName

        Dim SectorNum As clsXY_int
        For Each SectorNum In SectorTerrainUndoChanges.ChangedPoints
            NewUndo.ChangedSectors.Add(ShadowSectors(SectorNum.X, SectorNum.Y))
            ShadowSector_Create(SectorNum.XY)
        Next
        SectorTerrainUndoChanges.Clear()

        NewUndo.UnitChanges.AddSimpleList(UnitChanges)
        UnitChanges.Clear()

        NewUndo.GatewayChanges.AddSimpleList(GatewayChanges)
        GatewayChanges.Clear()

        If NewUndo.ChangedSectors.Count + NewUndo.UnitChanges.Count + NewUndo.GatewayChanges.Count > 0 Then
            Do While Undos.Count > UndoPosition 'a new line has been started so remove redos
                Undos.Remove(Undos.Count - 1)
            Loop

            Undos.Add(NewUndo)
            UndoPosition = Undos.Count

            SetChanged()
        End If
    End Sub

    Public Sub ShadowSector_Create(SectorNum As sXY_int)
        Dim TileX As Integer
        Dim TileY As Integer
        Dim StartX As Integer
        Dim StartY As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Sector As clsShadowSector
        Dim LastTileX As Integer
        Dim LastTileY As Integer

        Sector = New clsShadowSector
        ShadowSectors(SectorNum.X, SectorNum.Y) = Sector
        Sector.Num = SectorNum
        StartX = SectorNum.X * SectorTileSize
        StartY = SectorNum.Y * SectorTileSize
        LastTileX = Math.Min(SectorTileSize, Terrain.TileSize.X - StartX)
        LastTileY = Math.Min(SectorTileSize, Terrain.TileSize.Y - StartY)
        For Y = 0 To LastTileY
            For X = 0 To LastTileX
                TileX = StartX + X
                TileY = StartY + Y
                Sector.Terrain.Vertices(X, Y).Height = Terrain.Vertices(TileX, TileY).Height
                Sector.Terrain.Vertices(X, Y).Terrain = Terrain.Vertices(TileX, TileY).Terrain
            Next
        Next
        For Y = 0 To LastTileY - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileY = StartY + Y
                Sector.Terrain.Tiles(X, Y).Copy(Terrain.Tiles(TileX, TileY))
            Next
        Next
        For Y = 0 To LastTileY
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileY = StartY + Y
                Sector.Terrain.SideH(X, Y).Road = Terrain.SideH(TileX, TileY).Road
            Next
        Next
        For Y = 0 To LastTileY - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileY = StartY + Y
                Sector.Terrain.SideV(X, Y).Road = Terrain.SideV(TileX, TileY).Road
            Next
        Next
    End Sub

    Public Sub UndoClear()

        UndoStepCreate("") 'absorb current changes
        Dim UnitChange As clsUnitChange
        Dim Undo As clsUndo

        For Each Undo In Undos
            For Each UnitChange In Undo.UnitChanges
                UnitChange.Unit.Deallocate()
            Next
        Next

        Undos.Clear()
        UndoPosition = Undos.Count
    End Sub

    Public Sub UndoPerform()
        Dim ThisUndo As clsMap.clsUndo

        UndoStepCreate("Incomplete Action") 'make another redo step incase something has changed, such as if user presses undo while still dragging a tool

        UndoPosition -= 1

        ThisUndo = Undos(UndoPosition)

        Dim SectorNum As sXY_int
        Dim CurrentSector As clsShadowSector
        Dim UndoSector As clsShadowSector
        Dim NewSectorsForThisUndo As New SimpleList(Of clsShadowSector)
        For Each UndoSector In ThisUndo.ChangedSectors
            SectorNum = UndoSector.Num
            'store existing state for redo
            CurrentSector = ShadowSectors(SectorNum.X, SectorNum.Y)
            'remove graphics from sector
            Sectors(SectorNum.X, SectorNum.Y).DeleteLists()
            'perform the undo
            Undo_Sector_Rejoin(UndoSector)
            'update the backup
            ShadowSector_Create(SectorNum)
            'add old state to the redo step (that was this undo step)
            NewSectorsForThisUndo.Add(CurrentSector)
            'prepare to update graphics on this sector
            SectorGraphicsChanges.Changed(SectorNum)
        Next
        ThisUndo.ChangedSectors = NewSectorsForThisUndo

        Dim ID As UInteger
        Dim UnitAdd As New clsMap.clsUnitAdd
        UnitAdd.Map = Me
        Dim Unit As clsUnit
        For A As Integer = ThisUndo.UnitChanges.Count - 1 To 0 Step -1 'must do in reverse order, otherwise may try to delete units that havent been added yet
            Unit = ThisUndo.UnitChanges(A).Unit
            Select Case ThisUndo.UnitChanges(A).Type
                Case clsUnitChange.enumType.Added
                    'remove the unit from the map
                    UnitRemove(Unit.MapLink.ArrayPosition)
                Case clsUnitChange.enumType.Deleted
                    'add the unit back on to the map
                    ID = Unit.ID
                    UnitAdd.ID = ID
                    UnitAdd.NewUnit = Unit
                    UnitAdd.Perform()
                    ErrorIDChange(ID, Unit, "Undo_Perform")
                Case Else
                    Stop
            End Select
        Next

        Dim GatewayChange As clsGatewayChange
        For A As Integer = ThisUndo.GatewayChanges.Count - 1 To 0 Step -1
            GatewayChange = ThisUndo.GatewayChanges(A)
            Select Case GatewayChange.Type
                Case clsGatewayChange.enumType.Added
                    'remove the unit from the map
                    GatewayChange.Gateway.MapLink.Disconnect()
                Case clsGatewayChange.enumType.Deleted
                    'add the unit back on to the map
                    GatewayChange.Gateway.MapLink.Connect(Gateways)
                Case Else
                    Stop
            End Select
        Next

        SectorsUpdateGraphics()
        MinimapMakeLater()
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Public Sub RedoPerform()
        Dim ThisUndo As clsUndo

        ThisUndo = Undos(UndoPosition)

        Dim SectorNum As sXY_int
        Dim CurrentSector As clsShadowSector
        Dim UndoSector As clsShadowSector
        Dim NewSectorsForThisUndo As New SimpleList(Of clsShadowSector)
        For Each UndoSector In ThisUndo.ChangedSectors
            SectorNum = UndoSector.Num
            'store existing state for undo
            CurrentSector = ShadowSectors(SectorNum.X, SectorNum.Y)
            'remove graphics from sector
            Sectors(SectorNum.X, SectorNum.Y).DeleteLists()
            'perform the redo
            Undo_Sector_Rejoin(UndoSector)
            'update the backup
            ShadowSector_Create(SectorNum)
            'add old state to the undo step (that was this redo step)
            NewSectorsForThisUndo.Add(CurrentSector)
            'prepare to update graphics on this sector
            SectorGraphicsChanges.Changed(SectorNum)
        Next
        ThisUndo.ChangedSectors = NewSectorsForThisUndo

        Dim ID As UInteger
        Dim UnitAdd As New clsMap.clsUnitAdd
        UnitAdd.Map = Me
        Dim Unit As clsUnit
        For A As Integer = 0 To ThisUndo.UnitChanges.Count - 1 'forward order is important
            Unit = ThisUndo.UnitChanges(A).Unit
            Select Case ThisUndo.UnitChanges(A).Type
                Case clsUnitChange.enumType.Added
                    'add the unit back on to the map
                    ID = Unit.ID
                    UnitAdd.ID = ID
                    UnitAdd.NewUnit = Unit
                    UnitAdd.Perform()
                    ErrorIDChange(ID, Unit, "Redo_Perform")
                Case clsUnitChange.enumType.Deleted
                    'remove the unit from the map
                    UnitRemove(Unit.MapLink.ArrayPosition)
                Case Else
                    Stop
            End Select
        Next

        Dim GatewayChange As clsGatewayChange
        For A As Integer = 0 To ThisUndo.GatewayChanges.Count - 1 'forward order is important
            GatewayChange = ThisUndo.GatewayChanges(A)
            Select Case GatewayChange.Type
                Case clsGatewayChange.enumType.Added
                    'add the unit back on to the map
                    GatewayChange.Gateway.MapLink.Connect(Gateways)
                Case clsGatewayChange.enumType.Deleted
                    'remove the unit from the map
                    GatewayChange.Gateway.MapLink.Disconnect()
                Case Else
                    Stop
            End Select
        Next

        UndoPosition += 1

        SectorsUpdateGraphics()
        MinimapMakeLater()
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Public Sub Undo_Sector_Rejoin(Shadow_Sector_To_Rejoin As clsShadowSector)
        Dim TileX As Integer
        Dim TileZ As Integer
        Dim StartX As Integer
        Dim StartZ As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim LastTileX As Integer
        Dim LastTileZ As Integer

        StartX = Shadow_Sector_To_Rejoin.Num.X * SectorTileSize
        StartZ = Shadow_Sector_To_Rejoin.Num.Y * SectorTileSize
        LastTileX = Math.Min(SectorTileSize, Terrain.TileSize.X - StartX)
        LastTileZ = Math.Min(SectorTileSize, Terrain.TileSize.Y - StartZ)
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.Vertices(TileX, TileZ).Height = Shadow_Sector_To_Rejoin.Terrain.Vertices(X, Y).Height
                Terrain.Vertices(TileX, TileZ).Terrain = Shadow_Sector_To_Rejoin.Terrain.Vertices(X, Y).Terrain
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.Tiles(TileX, TileZ).Copy(Shadow_Sector_To_Rejoin.Terrain.Tiles(X, Y))
            Next
        Next
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.SideH(TileX, TileZ).Road = Shadow_Sector_To_Rejoin.Terrain.SideH(X, Y).Road
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.SideV(TileX, TileZ).Road = Shadow_Sector_To_Rejoin.Terrain.SideV(X, Y).Road
            Next
        Next
    End Sub

    Public Sub MapInsert(MapToInsert As clsMap, Offset As sXY_int, Area As sXY_int, InsertHeights As Boolean, InsertTextures As Boolean, InsertUnits As Boolean, DeleteUnits As Boolean, InsertGateways As Boolean, DeleteGateways As Boolean)
        Dim Finish As sXY_int
        Dim X As Integer
        Dim Y As Integer
        Dim SectorStart As sXY_int
        Dim SectorFinish As sXY_int
        Dim AreaAdjusted As sXY_int
        Dim SectorNum As sXY_int

        Finish.X = Math.Min(Offset.X + Math.Min(Area.X, MapToInsert.Terrain.TileSize.X), Terrain.TileSize.X)
        Finish.Y = Math.Min(Offset.Y + Math.Min(Area.Y, MapToInsert.Terrain.TileSize.Y), Terrain.TileSize.Y)
        AreaAdjusted.X = Finish.X - Offset.X
        AreaAdjusted.Y = Finish.Y - Offset.Y

        GetTileSectorRange(New sXY_int(Offset.X - 1, Offset.Y - 1), Finish, SectorStart, SectorFinish)
        For Y = SectorStart.Y To SectorFinish.Y
            SectorNum.Y = Y
            For X = SectorStart.X To SectorFinish.X
                SectorNum.X = X
                SectorGraphicsChanges.Changed(SectorNum)
                SectorUnitHeightsChanges.Changed(SectorNum)
                SectorTerrainUndoChanges.Changed(SectorNum)
            Next
        Next

        If InsertHeights Then
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height = MapToInsert.Terrain.Vertices(X, Y).Height
                Next
            Next
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri = MapToInsert.Terrain.Tiles(X, Y).Tri
                Next
            Next
        End If
        If InsertTextures Then
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain = MapToInsert.Terrain.Vertices(X, Y).Terrain
                Next
            Next
            Dim TriDirection As Boolean
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    TriDirection = Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Copy(MapToInsert.Terrain.Tiles(X, Y))
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri = TriDirection
                Next
            Next
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X - 1
                    Terrain.SideH(Offset.X + X, Offset.Y + Y).Road = MapToInsert.Terrain.SideH(X, Y).Road
                Next
            Next
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X
                    Terrain.SideV(Offset.X + X, Offset.Y + Y).Road = MapToInsert.Terrain.SideV(X, Y).Road
                Next
            Next
        End If

        Dim LastTile As sXY_int
        LastTile = Finish
        LastTile.X -= 1
        LastTile.Y -= 1
        If DeleteGateways Then
            Dim A As Integer
            A = 0
            Do While A < Gateways.Count
                If Gateways(A).PosA.IsInRange(Offset, LastTile) Or Gateways(A).PosB.IsInRange(Offset, LastTile) Then
                    GatewayRemoveStoreChange(A)
                Else
                    A += 1
                End If
            Loop
        End If
        If InsertGateways Then
            Dim GateStart As sXY_int
            Dim GateFinish As sXY_int
            Dim Gateway As clsGateway
            For Each Gateway In MapToInsert.Gateways
                GateStart.X = Offset.X + Gateway.PosA.X
                GateStart.Y = Offset.Y + Gateway.PosA.Y
                GateFinish.X = Offset.X + Gateway.PosB.X
                GateFinish.Y = Offset.Y + Gateway.PosB.Y
                If GateStart.IsInRange(Offset, LastTile) Or GateFinish.IsInRange(Offset, LastTile) Then
                    GatewayCreateStoreChange(GateStart, GateFinish)
                End If
            Next
        End If

        If DeleteUnits Then
            Dim UnitsToDelete As New SimpleList(Of clsUnit)
            Dim UnitToDeleteCount As Integer = 0
            Dim Unit As clsUnit
            For Y = SectorStart.Y To SectorFinish.Y
                For X = SectorStart.X To SectorFinish.X
                    Dim Connection As clsUnitSectorConnection
                    For Each Connection In Sectors(X, Y).Units
                        Unit = Connection.Unit
                        If PosIsWithinTileArea(Unit.Pos.Horizontal, Offset, Finish) Then
                            UnitsToDelete.Add(Unit)
                        End If
                    Next
                Next
            Next
            For Each Unit In UnitsToDelete
                If Unit.MapLink.IsConnected Then 'units may be in the list multiple times and already be deleted
                    UnitRemoveStoreChange(Unit.MapLink.ArrayPosition)
                End If
            Next
        End If
        If InsertUnits Then
            Dim PosDif As sXY_int
            Dim NewUnit As clsUnit
            Dim Unit As clsUnit
            Dim ZeroPos As New sXY_int(0, 0)
            Dim UnitAdd As New clsMap.clsUnitAdd

            UnitAdd.Map = Me
            UnitAdd.StoreChange = True

            PosDif.X = Offset.X * TerrainGridSpacing
            PosDif.Y = Offset.Y * TerrainGridSpacing
            For Each Unit In MapToInsert.Units
                If PosIsWithinTileArea(Unit.Pos.Horizontal, ZeroPos, AreaAdjusted) Then
                    NewUnit = New clsUnit(Unit, Me)
                    NewUnit.Pos.Horizontal.X += PosDif.X
                    NewUnit.Pos.Horizontal.Y += PosDif.Y
                    UnitAdd.NewUnit = NewUnit
                    UnitAdd.Label = Unit.Label
                    UnitAdd.Perform()
                End If
            Next
        End If

        SectorsUpdateGraphics()
        SectorsUpdateUnitHeights()
        MinimapMakeLater()
    End Sub

    Public Function GatewayCreate(PosA As sXY_int, PosB As sXY_int) As clsGateway

        If PosA.X >= 0 And PosA.X < Terrain.TileSize.X And _
            PosA.Y >= 0 And PosA.Y < Terrain.TileSize.Y And _
            PosB.X >= 0 And PosB.X < Terrain.TileSize.X And _
            PosB.Y >= 0 And PosB.Y < Terrain.TileSize.Y Then 'is on map
            If PosA.X = PosB.X Or PosA.Y = PosB.Y Then 'is straight

                Dim Gateway As New clsGateway

                Gateway.PosA = PosA
                Gateway.PosB = PosB

                Gateway.MapLink.Connect(Gateways)

                Return Gateway
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Function GatewayCreateStoreChange(PosA As sXY_int, PosB As sXY_int) As clsGateway
        Dim Gateway As clsGateway

        Gateway = GatewayCreate(PosA, PosB)

        Dim GatewayChange As New clsGatewayChange
        GatewayChange.Type = clsGatewayChange.enumType.Added
        GatewayChange.Gateway = Gateway
        GatewayChanges.Add(GatewayChange)

        Return Gateway
    End Function

    Public Sub GatewayRemoveStoreChange(Num As Integer)

        Dim GatewayChange As New clsGatewayChange
        GatewayChange.Type = clsGatewayChange.enumType.Deleted
        GatewayChange.Gateway = Gateways.Item(Num)
        GatewayChanges.Add(GatewayChange)

        Gateways.Item(Num).MapLink.Disconnect()
    End Sub

    Public Sub TileType_Reset()

        If Tileset Is Nothing Then
            ReDim Tile_TypeNum(-1)
        Else
            Dim A As Integer

            ReDim Tile_TypeNum(Tileset.TileCount - 1)
            For A = 0 To Tileset.TileCount - 1
                Tile_TypeNum(A) = Tileset.Tiles(A).Default_Type
            Next
        End If
    End Sub

    Public Sub SetPainterToDefaults()

        If Tileset Is Nothing Then
            Painter = New clsPainter
        ElseIf Tileset Is Tileset_Arizona Then
            Painter = Painter_Arizona
        ElseIf Tileset Is Tileset_Urban Then
            Painter = Painter_Urban
        ElseIf Tileset Is Tileset_Rockies Then
            Painter = Painter_Rockies
        Else
            Painter = New clsPainter
        End If
    End Sub

    Private Sub UnitSectorsGraphicsChanged(UnitToUpdateFor As clsUnit)

        If SectorGraphicsChanges Is Nothing Then
            Stop
            Exit Sub
        End If

        Dim Connection As clsUnitSectorConnection

        For Each Connection In UnitToUpdateFor.Sectors
            SectorGraphicsChanges.Changed(Connection.Sector.Pos)
        Next
    End Sub

    Public Function GetTileOffsetRotatedWorldPos(Tile As sXY_int, TileOffsetToRotate As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Dim RotatedOffset As sXY_int

        RotatedOffset = GetTileRotatedOffset(Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation, TileOffsetToRotate)
        Result.Horizontal.X = Tile.X * TerrainGridSpacing + RotatedOffset.X
        Result.Horizontal.Y = Tile.Y * TerrainGridSpacing + RotatedOffset.Y
        Result.Altitude = CInt(GetTerrainHeight(Result.Horizontal))

        Return Result
    End Function

    Public Sub GetFootprintTileRangeClamped(Horizontal As sXY_int, Footprint As sXY_int, ByRef ResultStart As sXY_int, ByRef ResultFinish As sXY_int)
        Dim Remainder As Integer
        Dim Centre As sXY_int = GetPosTileNum(Horizontal)
        Dim Half As Integer

        Half = Math.DivRem(Footprint.X, 2, Remainder)
        ResultStart.X = Clamp_int(Centre.X - Half, 0, Terrain.TileSize.X - 1)
        ResultFinish.X = Clamp_int(ResultStart.X + Footprint.X - 1, 0, Terrain.TileSize.X - 1)
        Half = Math.DivRem(Footprint.Y, 2, Remainder)
        ResultStart.Y = Clamp_int(Centre.Y - Half, 0, Terrain.TileSize.Y - 1)
        ResultFinish.Y = Clamp_int(ResultStart.Y + Footprint.Y - 1, 0, Terrain.TileSize.Y - 1)
    End Sub

    Public Sub GetFootprintTileRange(Horizontal As sXY_int, Footprint As sXY_int, ByRef ResultStart As sXY_int, ByRef ResultFinish As sXY_int)

        Dim Remainder As Integer
        Dim Centre As sXY_int = GetPosTileNum(Horizontal)
        Dim Half As Integer

        Half = Math.DivRem(Footprint.X, 2, Remainder)
        ResultStart.X = Centre.X - Half
        ResultFinish.X = ResultStart.X + Footprint.X - 1
        Half = Math.DivRem(Footprint.Y, 2, Remainder)
        ResultStart.Y = Centre.Y - Half
        ResultFinish.Y = ResultStart.Y + Footprint.Y - 1
    End Sub

    Public Function GetPosTileNum(Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = CInt(Int(Horizontal.X / TerrainGridSpacing))
        Result.Y = CInt(Int(Horizontal.Y / TerrainGridSpacing))

        Return Result
    End Function

    Public Function GetPosVertexNum(Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = CInt(Math.Round(Horizontal.X / TerrainGridSpacing))
        Result.Y = CInt(Math.Round(Horizontal.Y / TerrainGridSpacing))

        Return Result
    End Function

    Public Function GetPosSectorNum(Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result = GetTileSectorNum(GetPosTileNum(Horizontal))

        Return Result
    End Function

    Public Function GetSectorNumClamped(SectorNum As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Clamp_int(SectorNum.X, 0, SectorCount.X - 1)
        Result.Y = Clamp_int(SectorNum.Y, 0, SectorCount.Y - 1)

        Return Result
    End Function

    Public Function GetVertexAltitude(VertexNum As sXY_int) As Integer

        Return Terrain.Vertices(VertexNum.X, VertexNum.Y).Height * HeightMultiplier
    End Function

    Public Function PosIsOnMap(Horizontal As sXY_int) As Boolean

        Return PosIsWithinTileArea(Horizontal, New sXY_int(0, 0), Terrain.TileSize)
    End Function

    Public Function TileNumClampToMap(TileNum As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Clamp_int(TileNum.X, 0, Terrain.TileSize.X - 1)
        Result.Y = Clamp_int(TileNum.Y, 0, Terrain.TileSize.Y - 1)

        Return Result
    End Function

    Public CompileScreen As frmCompile

    Public Sub CancelUserInput()

        If Not _ReadyForUserInput Then
            Exit Sub
        End If

        _ReadyForUserInput = False

        Dim X As Integer
        Dim Y As Integer

        If CompileScreen IsNot Nothing Then
            CompileScreen.Close()
            CompileScreen = Nothing
        End If

        SectorAll_GLLists_Delete()
        MinimapGLDelete()

        ShadowSectors = Nothing
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y).Deallocate()
            Next
        Next
        Sectors = Nothing
        SectorGraphicsChanges.Deallocate()
        SectorGraphicsChanges = Nothing
        SectorUnitHeightsChanges.Deallocate()
        SectorUnitHeightsChanges = Nothing
        SectorTerrainUndoChanges.Deallocate()
        SectorTerrainUndoChanges = Nothing
        AutoTextureChanges.Deallocate()
        AutoTextureChanges = Nothing
        TerrainInterpretChanges.Deallocate()
        TerrainInterpretChanges = Nothing

        UnitChanges = Nothing
        GatewayChanges = Nothing
        Undos = Nothing

        SelectedUnits.Deallocate()
        SelectedUnits = Nothing

        Selected_Tile_A = Nothing
        Selected_Tile_B = Nothing
        Selected_Area_VertexA = Nothing
        Selected_Area_VertexB = Nothing
        Unit_Selected_Area_VertexA = Nothing

        ViewInfo = Nothing

        _SelectedUnitGroup = Nothing

        Messages = Nothing
    End Sub

    Public Sub InitializeUserInput()

        If _ReadyForUserInput Then
            Exit Sub
        End If

        _ReadyForUserInput = True

        Dim X As Integer
        Dim Y As Integer

        SectorCount.X = CInt(Math.Ceiling(Terrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Terrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        Dim Unit As clsUnit
        For Each Unit In Units
            UnitSectorsCalc(Unit)
        Next

        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                ShadowSector_Create(New sXY_int(X, Y))
            Next
        Next

        SectorGraphicsChanges = New clsSectorChanges(Me)
        SectorGraphicsChanges.SetAllChanged()
        SectorUnitHeightsChanges = New clsSectorChanges(Me)
        SectorTerrainUndoChanges = New clsSectorChanges(Me)
        AutoTextureChanges = New clsAutoTextureChanges(Me)
        TerrainInterpretChanges = New clsMap.clsTerrainUpdate(Terrain.TileSize)

        UnitChanges = New SimpleClassList(Of clsMap.clsUnitChange)
        UnitChanges.MaintainOrder = True
        GatewayChanges = New SimpleClassList(Of clsMap.clsGatewayChange)
        GatewayChanges.MaintainOrder = True
        Undos = New SimpleClassList(Of clsMap.clsUndo)
        Undos.MaintainOrder = True
        UndoPosition = 0

        SelectedUnits = New ConnectedList(Of clsMap.clsUnit, clsMap)(Me)

        If InterfaceOptions Is Nothing Then
            InterfaceOptions = New clsInterfaceOptions
        End If

        ViewInfo = New clsViewInfo(Me, frmMainInstance.MapView)

        _SelectedUnitGroup = New clsMap.clsUnitGroupContainer
        SelectedUnitGroup.Item = ScavengerUnitGroup

        Messages = New SimpleClassList(Of clsMessage)
        Messages.MaintainOrder = True
    End Sub

    Public Function GetDirectory() As String

        If PathInfo Is Nothing Then
            Return My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Else
            Dim SplitPath As New sSplitPath(PathInfo.Path)
            Return SplitPath.FilePath
        End If
    End Function

    Public Class clsUpdateSectorGraphics
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()

            Map.Sector_GLList_Make(PosNum.X, PosNum.Y)
            Map.MinimapMakeLater()
        End Sub
    End Class

    Public Sub Update()
        Dim PrevSuppress As Boolean = SuppressMinimap

        SuppressMinimap = True
        UpdateAutoTextures()
        TerrainInterpretUpdate()
        SectorsUpdateGraphics()
        SectorsUpdateUnitHeights()
        SuppressMinimap = PrevSuppress
    End Sub

    Public Sub SectorsUpdateUnitHeights()
        Dim UpdateSectorUnitHeights As New clsUpdateSectorUnitHeights
        UpdateSectorUnitHeights.Map = Me

        UpdateSectorUnitHeights.Start()
        SectorUnitHeightsChanges.PerformTool(UpdateSectorUnitHeights)
        UpdateSectorUnitHeights.Finish()
        SectorUnitHeightsChanges.Clear()
    End Sub

    Public Sub SectorsUpdateGraphics()
        Dim UpdateSectorGraphics As New clsUpdateSectorGraphics
        UpdateSectorGraphics.Map = Me

        If MainMap Is Me Then
            SectorGraphicsChanges.PerformTool(UpdateSectorGraphics)
        End If
        SectorGraphicsChanges.Clear()
    End Sub

    Public Sub UpdateAutoTextures()
        Dim UpdateAutotextures As New clsUpdateAutotexture
        UpdateAutotextures.Map = Me
        UpdateAutotextures.MakeInvalidTiles = frmMainInstance.cbxInvalidTiles.Checked

        AutoTextureChanges.PerformTool(UpdateAutotextures)
        AutoTextureChanges.Clear()
    End Sub

    Public Sub TerrainInterpretUpdate()
        Dim ApplyVertexInterpret As New clsMap.clsApplyVertexTerrainInterpret
        Dim ApplyTileInterpret As New clsMap.clsApplyTileTerrainInterpret
        Dim ApplySideHInterpret As New clsMap.clsApplySideHTerrainInterpret
        Dim ApplySideVInterpret As New clsMap.clsApplySideVTerrainInterpret
        ApplyVertexInterpret.Map = Me
        ApplyTileInterpret.Map = Me
        ApplySideHInterpret.Map = Me
        ApplySideVInterpret.Map = Me

        TerrainInterpretChanges.Vertices.PerformTool(ApplyVertexInterpret)
        TerrainInterpretChanges.Tiles.PerformTool(ApplyTileInterpret)
        TerrainInterpretChanges.SidesH.PerformTool(ApplySideHInterpret)
        TerrainInterpretChanges.SidesV.PerformTool(ApplySideVInterpret)
        TerrainInterpretChanges.ClearAll()
    End Sub

    Public Class clsUpdateSectorUnitHeights
        Inherits clsMap.clsAction

        Private NewUnit As clsUnit
        Private ID As UInteger
        Private OldUnits() As clsUnit
        Private OldUnitCount As Integer = 0
        Private NewAltitude As Integer
        Private Started As Boolean

        Public Sub Start()

            ReDim OldUnits(Map.Units.Count - 1)

            Started = True
        End Sub

        Public Sub Finish()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim A As Integer
            Dim UnitAdd As New clsMap.clsUnitAdd
            Dim Unit As clsUnit

            UnitAdd.Map = Map
            UnitAdd.StoreChange = True

            For A = 0 To OldUnitCount - 1
                Unit = OldUnits(A)
                NewAltitude = CInt(Map.GetTerrainHeight(Unit.Pos.Horizontal))
                If NewAltitude <> Unit.Pos.Altitude Then
                    NewUnit = New clsUnit(Unit, Map)
                    ID = Unit.ID
                    'NewUnit.Pos.Altitude = NewAltitude
                    'these create changed sectors and must be done before drawing the new sectors
                    Map.UnitRemoveStoreChange(Unit.MapLink.ArrayPosition)
                    UnitAdd.NewUnit = NewUnit
                    UnitAdd.ID = ID
                    UnitAdd.Perform()
                    ErrorIDChange(ID, NewUnit, "UpdateSectorUnitHeights")
                End If
            Next

            Started = False
        End Sub

        Public Overrides Sub ActionPerform()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim Connection As clsUnitSectorConnection
            Dim Unit As clsUnit
            Dim Sector As clsMap.clsSector
            Dim A As Integer

            Sector = Map.Sectors(PosNum.X, PosNum.Y)
            For Each Connection In Sector.Units
                Unit = Connection.Unit
                'units can be in multiple sectors, so dont include multiple times
                For A = 0 To OldUnitCount - 1
                    If OldUnits(A) Is Unit Then
                        Exit For
                    End If
                Next
                If A = OldUnitCount Then
                    OldUnits(OldUnitCount) = Unit
                    OldUnitCount += 1
                End If
            Next
        End Sub
    End Class

    Public Class clsUpdateAutotexture
        Inherits clsMap.clsAction

        Public MakeInvalidTiles As Boolean

        Private Terrain_Inner As clsPainter.clsTerrain
        Private Terrain_Outer As clsPainter.clsTerrain
        Private Road As clsPainter.clsRoad
        Private RoadTop As Boolean
        Private RoadLeft As Boolean
        Private RoadRight As Boolean
        Private RoadBottom As Boolean
        Private Painter As clsPainter
        Private Terrain As clsTerrain
        Private ResultTiles As clsPainter.clsTileList
        Private ResultDirection As sTileDirection
        Private ResultTexture As clsPainter.clsTileList.sTileOrientationChance

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            Painter = Map.Painter

            ResultTiles = Nothing
            ResultDirection = TileDirection_None

            'apply centre brushes
            If Not Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff Then
                For BrushNum As Integer = 0 To Painter.TerrainCount - 1
                    Terrain_Inner = Painter.Terrains(BrushNum)
                    If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i i i
                                    ResultTiles = Terrain_Inner.Tiles
                                    ResultDirection = TileDirection_None
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'apply transition brushes
            If Not Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff Then
                For BrushNum As Integer = 0 To Painter.TransitionBrushCount - 1
                    Terrain_Inner = Painter.TransitionBrushes(BrushNum).Terrain_Inner
                    Terrain_Outer = Painter.TransitionBrushes(BrushNum).Terrain_Outer
                    If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i i i
                                    'nothing to do here
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i i i o
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Corner_In
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i o i
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Corner_In
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i i o o
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Straight
                                    ResultDirection = TileDirection_Bottom
                                    Exit For
                                End If
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i o i i
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i o i o
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Straight
                                    ResultDirection = TileDirection_Right
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i o o i
                                    ResultTiles = Nothing
                                    ResultDirection = TileDirection_None
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i o o o
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            End If
                        End If
                    ElseIf Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o i i i
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o i i o
                                    ResultTiles = Nothing
                                    ResultDirection = TileDirection_None
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o i o i
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Straight
                                    ResultDirection = TileDirection_Left
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o i o o
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                End If
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o o i i
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Straight
                                    ResultDirection = TileDirection_Top
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o o i o
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Corner_Out
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o o o i
                                    ResultTiles = Painter.TransitionBrushes(BrushNum).Tiles_Corner_Out
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o o o o
                                    'nothing to do here
                                    Exit For
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'set cliff tiles
            If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                        Dim BrushNum As Integer = 0
                        For BrushNum = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(BrushNum).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(BrushNum).Terrain_Outer
                            If Terrain_Inner Is Terrain_Outer Then
                                Dim A As Integer = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 3 Then
                                    ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                    ResultDirection = Terrain.Tiles(PosNum.X, PosNum.Y).DownSide
                                    Exit For
                                End If
                            End If
                            If ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                ResultDirection = TileDirection_Bottom
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                ResultDirection = TileDirection_Left
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                ResultDirection = TileDirection_Top
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                ResultDirection = TileDirection_Right
                                Exit For
                            End If
                        Next
                        If BrushNum = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    Else
                        Dim BrushNum As Integer = 0
                        For BrushNum = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(BrushNum).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(BrushNum).Terrain_Outer
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                                Dim A As Integer = 0
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                                Dim A As Integer = 0
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            End If
                        Next
                        If BrushNum = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    End If
                ElseIf Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                    Dim BrushNum As Integer = 0
                    For BrushNum = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(BrushNum).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(BrushNum).Terrain_Outer
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            Dim A As Integer = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Corner_In
                                ResultDirection = TileDirection_BottomRight
                                Exit For
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                            Dim A As Integer = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Corner_Out
                                ResultDirection = TileDirection_TopLeft
                                Exit For
                            End If
                        End If
                    Next
                    If BrushNum = Painter.CliffBrushCount Then
                        ResultTiles = Nothing
                        ResultDirection = TileDirection_None
                    End If
                Else
                    'no cliff
                End If
            Else
                'default tri orientation
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                        Dim BrushNum As Integer = 0
                        For BrushNum = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(BrushNum).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(BrushNum).Terrain_Outer
                            If Terrain_Inner Is Terrain_Outer Then
                                Dim A As Integer = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 3 Then
                                    ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                    ResultDirection = Terrain.Tiles(PosNum.X, PosNum.Y).DownSide
                                    Exit For
                                End If
                            End If
                            If ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                ResultDirection = TileDirection_Bottom
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                ResultDirection = TileDirection_Left
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                ResultDirection = TileDirection_Top
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Straight
                                ResultDirection = TileDirection_Right
                                Exit For
                            End If
                        Next
                        If BrushNum = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    Else
                        Dim BrushNum As Integer = 0
                        For BrushNum = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(BrushNum).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(BrushNum).Terrain_Outer
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                                Dim A As Integer = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                                Dim A As Integer = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                End If
                            End If
                        Next
                        If BrushNum = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    End If
                ElseIf Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                    Dim BrushNum As Integer = 0
                    For BrushNum = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(BrushNum).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(BrushNum).Terrain_Outer
                        If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            Dim A As Integer = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Corner_In
                                ResultDirection = TileDirection_BottomLeft
                                Exit For
                            End If
                        ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                            Dim A As Integer = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(BrushNum).Tiles_Corner_Out
                                ResultDirection = TileDirection_TopRight
                                Exit For
                            End If
                        End If
                    Next
                    If BrushNum = Painter.CliffBrushCount Then
                        ResultTiles = Nothing
                        ResultDirection = TileDirection_None
                    End If
                Else
                    'no cliff
                End If
            End If

            'apply roads
            Road = Nothing
            If Terrain.SideH(PosNum.X, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideH(PosNum.X, PosNum.Y).Road
            ElseIf Terrain.SideH(PosNum.X, PosNum.Y + 1).Road IsNot Nothing Then
                Road = Terrain.SideH(PosNum.X, PosNum.Y + 1).Road
            ElseIf Terrain.SideV(PosNum.X + 1, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideV(PosNum.X + 1, PosNum.Y).Road
            ElseIf Terrain.SideV(PosNum.X, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideV(PosNum.X, PosNum.Y).Road
            End If
            If Road IsNot Nothing Then
                Dim BrushNum As Integer = 0
                For BrushNum = 0 To Painter.RoadBrushCount - 1
                    If Painter.RoadBrushes(BrushNum).Road Is Road Then
                        Terrain_Outer = Painter.RoadBrushes(BrushNum).Terrain
                        Dim A As Integer = 0
                        If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If A >= 2 Then Exit For
                    End If
                Next

                ResultTiles = Nothing
                ResultDirection = TileDirection_None

                If BrushNum < Painter.RoadBrushCount Then
                    RoadTop = (Terrain.SideH(PosNum.X, PosNum.Y).Road Is Road)
                    RoadLeft = (Terrain.SideV(PosNum.X, PosNum.Y).Road Is Road)
                    RoadRight = (Terrain.SideV(PosNum.X + 1, PosNum.Y).Road Is Road)
                    RoadBottom = (Terrain.SideH(PosNum.X, PosNum.Y + 1).Road Is Road)
                    'do cross intersection
                    If RoadTop And RoadLeft And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_CrossIntersection
                        ResultDirection = TileDirection_None
                        'do T intersection
                    ElseIf RoadTop And RoadLeft And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_TIntersection
                        ResultDirection = TileDirection_Top
                    ElseIf RoadTop And RoadLeft And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_TIntersection
                        ResultDirection = TileDirection_Left
                    ElseIf RoadTop And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_TIntersection
                        ResultDirection = TileDirection_Right
                    ElseIf RoadLeft And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_TIntersection
                        ResultDirection = TileDirection_Bottom
                        'do straight
                    ElseIf RoadTop And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_Straight
                        If Rnd() >= 0.5F Then
                            ResultDirection = TileDirection_Top
                        Else
                            ResultDirection = TileDirection_Bottom
                        End If
                    ElseIf RoadLeft And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_Straight
                        If Rnd() >= 0.5F Then
                            ResultDirection = TileDirection_Left
                        Else
                            ResultDirection = TileDirection_Right
                        End If
                        'do corner
                    ElseIf RoadTop And RoadLeft Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_Corner_In
                        ResultDirection = TileDirection_TopLeft
                    ElseIf RoadTop And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_Corner_In
                        ResultDirection = TileDirection_TopRight
                    ElseIf RoadLeft And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_Corner_In
                        ResultDirection = TileDirection_BottomLeft
                    ElseIf RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_Corner_In
                        ResultDirection = TileDirection_BottomRight
                        'do end
                    ElseIf RoadTop Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_End
                        ResultDirection = TileDirection_Top
                    ElseIf RoadLeft Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_End
                        ResultDirection = TileDirection_Left
                    ElseIf RoadRight Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_End
                        ResultDirection = TileDirection_Right
                    ElseIf RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(BrushNum).Tile_End
                        ResultDirection = TileDirection_Bottom
                    End If
                End If
            End If

            If ResultTiles Is Nothing Then
                ResultTexture.TextureNum = -1
                ResultTexture.Direction = TileDirection_None
            Else
                ResultTexture = ResultTiles.GetRandom()
            End If
            If ResultTexture.TextureNum < 0 Then
                If MakeInvalidTiles Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Texture = OrientateTile(ResultTexture, ResultDirection)
                End If
            Else
                Terrain.Tiles(PosNum.X, PosNum.Y).Texture = OrientateTile(ResultTexture, ResultDirection)
            End If

            Map.SectorGraphicsChanges.TileChanged(PosNum)
            Map.SectorTerrainUndoChanges.TileChanged(PosNum)
        End Sub
    End Class

    Public Sub TileNeedsInterpreting(Pos As sXY_int)

        TerrainInterpretChanges.Tiles.Changed(Pos)
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X + 1, Pos.Y))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X, Pos.Y + 1))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X + 1, Pos.Y + 1))
        TerrainInterpretChanges.SidesH.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.SidesH.Changed(New sXY_int(Pos.X, Pos.Y + 1))
        TerrainInterpretChanges.SidesV.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.SidesV.Changed(New sXY_int(Pos.X + 1, Pos.Y))
    End Sub

    Public Sub TileTextureChangeTerrainAction(Pos As sXY_int, Action As enumTextureTerrainAction)

        Select Case Action
            Case enumTextureTerrainAction.Ignore

            Case enumTextureTerrainAction.Reinterpret
                TileNeedsInterpreting(Pos)
            Case enumTextureTerrainAction.Remove
                Terrain.Vertices(Pos.X, Pos.Y).Terrain = Nothing
                Terrain.Vertices(Pos.X + 1, Pos.Y).Terrain = Nothing
                Terrain.Vertices(Pos.X, Pos.Y + 1).Terrain = Nothing
                Terrain.Vertices(Pos.X + 1, Pos.Y + 1).Terrain = Nothing
        End Select
    End Sub

    Public Class clsInterfaceOptions
        Public CompileName As String
        Public CompileMultiPlayers As String
        Public CompileMultiXPlayers As Boolean
        Public CompileMultiAuthor As String
        Public CompileMultiLicense As String
        Public AutoScrollLimits As Boolean
        Public ScrollMin As sXY_int
        Public ScrollMax As sXY_uint
        Public CampaignGameType As Integer

        Public Sub New()

            'set to default
            CompileName = ""
            CompileMultiPlayers = InvariantToString_int(2)
            CompileMultiXPlayers = False
            CompileMultiAuthor = ""
            CompileMultiLicense = ""
            AutoScrollLimits = True
            ScrollMin.X = 0
            ScrollMin.Y = 0
            ScrollMax.X = 0UI
            ScrollMax.Y = 0UI
            CampaignGameType = -1
        End Sub
    End Class
    Public InterfaceOptions As clsInterfaceOptions

    Public Function GetTitle() As String
        Dim ReturnResult As String

        If PathInfo Is Nothing Then
            ReturnResult = "Unsaved map"
        Else
            Dim SplitPath As New sSplitPath(PathInfo.Path)
            If PathInfo.IsFMap Then
                ReturnResult = SplitPath.FileTitleWithoutExtension
            Else
                ReturnResult = SplitPath.FileTitle
            End If
        End If
        Return ReturnResult
    End Function

    Public Sub SetChanged()

        ChangedSinceSave = True
        RaiseEvent Changed()

        AutoSave.ChangeCount += 1
        AutoSaveTest()
    End Sub

    Public MapView_TabPage As TabPage

    Public Sub SetTabText()
        Const MaxLength As Integer = 24

        Dim Result As String
        Result = GetTitle()
        If Result.Length > MaxLength Then
            Result = Strings.Left(Result, MaxLength - 3) & "..."
        End If
#If Mono Then
        MapView_TabPage.Text = Result & " "
#Else
        MapView_TabPage.Text = Result
#End If
    End Sub

    Public Function SideHIsCliffOnBothSides(SideNum As sXY_int) As Boolean
        Dim TileNum As sXY_int

        If SideNum.Y > 0 Then
            TileNum.X = SideNum.X
            TileNum.Y = SideNum.Y - 1
            If Terrain.Tiles(TileNum.X, TileNum.Y).Tri Then
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriBottomRightIsCliff Then
                    Return False
                End If
            Else
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriBottomLeftIsCliff Then
                    Return False
                End If
            End If
        End If

        If SideNum.Y < Terrain.TileSize.Y Then
            TileNum.X = SideNum.X
            TileNum.Y = SideNum.Y
            If Terrain.Tiles(TileNum.X, TileNum.Y).Tri Then
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriTopLeftIsCliff Then
                    Return False
                End If
            Else
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriTopRightIsCliff Then
                    Return False
                End If
            End If
        End If

        Return True
    End Function

    Public Function SideVIsCliffOnBothSides(SideNum As sXY_int) As Boolean
        Dim TileNum As sXY_int

        If SideNum.X > 0 Then
            TileNum.X = SideNum.X - 1
            TileNum.Y = SideNum.Y
            If Terrain.Tiles(TileNum.X, TileNum.Y).Tri Then
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriBottomRightIsCliff Then
                    Return False
                End If
            Else
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriTopRightIsCliff Then
                    Return False
                End If
            End If
        End If

        If SideNum.X < Terrain.TileSize.X Then
            TileNum.X = SideNum.X
            TileNum.Y = SideNum.Y
            If Terrain.Tiles(TileNum.X, TileNum.Y).Tri Then
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriTopLeftIsCliff Then
                    Return False
                End If
            Else
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriBottomLeftIsCliff Then
                    Return False
                End If
            End If
        End If

        Return True
    End Function

    Public Function VertexIsCliffEdge(VertexNum As sXY_int) As Boolean
        Dim TileNum As sXY_int

        If VertexNum.X > 0 Then
            If VertexNum.Y > 0 Then
                TileNum.X = VertexNum.X - 1
                TileNum.Y = VertexNum.Y - 1
                If Terrain.Tiles(TileNum.X, TileNum.Y).Terrain_IsCliff Then
                    Return True
                End If
            End If
            If VertexNum.Y < Terrain.TileSize.Y Then
                TileNum.X = VertexNum.X - 1
                TileNum.Y = VertexNum.Y
                If Terrain.Tiles(TileNum.X, TileNum.Y).Terrain_IsCliff Then
                    Return True
                End If
            End If
        End If
        If VertexNum.X < Terrain.TileSize.X Then
            If VertexNum.Y > 0 Then
                TileNum.X = VertexNum.X
                TileNum.Y = VertexNum.Y - 1
                If Terrain.Tiles(TileNum.X, TileNum.Y).Terrain_IsCliff Then
                    Return True
                End If
            End If
            If VertexNum.Y < Terrain.TileSize.Y Then
                TileNum.X = VertexNum.X
                TileNum.Y = VertexNum.Y
                If Terrain.Tiles(TileNum.X, TileNum.Y).Terrain_IsCliff Then
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Public Sub SelectedUnitsAction(Tool As clsObjectAction)
        Dim SelectAction As New clsObjectSelect

        SelectedUnits.GetItemsAsSimpleClassList.PerformTool(Tool)
        SelectedUnits.Clear()
        Tool.ResultUnits.PerformTool(SelectAction)
    End Sub

    Public Function CheckMessages() As Boolean
        Dim A As Integer
        Dim DateNow As Date = Now
        Dim Changed As Boolean = False

        A = 0
        Do While A < Messages.Count
            If DateDiff(DateInterval.Second, Messages(A).CreatedDate, DateNow) >= 6L Then
                Messages.Remove(A)
                Changed = True
            Else
                A += 1
            End If
        Loop
        Return Changed
    End Function

    Public ReadOnly Property MainMap As clsMap
        Get
            If Not frmMainLink.IsConnected Then
                Return Nothing
            Else
                Return frmMainLink.Source.MainMap
            End If
        End Get
    End Property

    Public Sub PerformTileWall(WallType As clsWallType, TileNum As sXY_int, Expand As Boolean)
        Dim SectorNum As sXY_int
        Dim Unit As clsUnit
        Dim UnitTile As sXY_int
        Dim Difference As sXY_int
        Dim TileWalls As enumTileWalls = enumTileWalls.None
        Dim Walls As New SimpleList(Of clsUnit)
        Dim Removals As New SimpleList(Of clsUnit)
        Dim UnitType As clsUnitType
        Dim StructureType As clsStructureType
        Dim X As Integer
        Dim Y As Integer
        Dim MinTile As sXY_int
        Dim MaxTile As sXY_int
        Dim Connection As clsUnitSectorConnection
        MinTile.X = TileNum.X - 1
        MinTile.Y = TileNum.Y - 1
        MaxTile.X = TileNum.X + 1
        MaxTile.Y = TileNum.Y + 1
        Dim SectorStart As sXY_int = GetSectorNumClamped(GetTileSectorNum(MinTile))
        Dim SectorFinish As sXY_int = GetSectorNumClamped(GetTileSectorNum(MaxTile))

        For Y = SectorStart.Y To SectorFinish.Y
            For X = SectorStart.X To SectorFinish.X
                SectorNum.X = X
                SectorNum.Y = Y
                For Each Connection In Sectors(SectorNum.X, SectorNum.Y).Units
                    Unit = Connection.Unit
                    UnitType = Unit.Type
                    If UnitType.Type = clsUnitType.enumType.PlayerStructure Then
                        StructureType = CType(UnitType, clsStructureType)
                        If StructureType.WallLink.Source Is WallType Then
                            UnitTile = GetPosTileNum(Unit.Pos.Horizontal)
                            Difference.X = UnitTile.X - TileNum.X
                            Difference.Y = UnitTile.Y - TileNum.Y
                            If Difference.Y = 1 Then
                                If Difference.X = 0 Then
                                    TileWalls = (TileWalls Or enumTileWalls.Bottom)
                                    Walls.Add(Unit)
                                End If
                            ElseIf Difference.Y = 0 Then
                                If Difference.X = 0 Then
                                    Removals.Add(Unit)
                                ElseIf Difference.X = -1 Then
                                    TileWalls = (TileWalls Or enumTileWalls.Left)
                                    Walls.Add(Unit)
                                ElseIf Difference.X = 1 Then
                                    TileWalls = (TileWalls Or enumTileWalls.Right)
                                    Walls.Add(Unit)
                                End If
                            ElseIf Difference.Y = -1 Then
                                If Difference.X = 0 Then
                                    TileWalls = (TileWalls Or enumTileWalls.Top)
                                    Walls.Add(Unit)
                                End If
                            End If
                        End If
                    End If
                Next
            Next
        Next

        For Each Unit In Removals
            UnitRemoveStoreChange(Unit.MapLink.ArrayPosition)
        Next

        Dim NewUnit As New clsUnit
        Dim NewUnitType As clsUnitType = WallType.Segments(WallType.TileWalls_Segment(TileWalls))
        NewUnit.Rotation = WallType.TileWalls_Direction(TileWalls)
        If Expand Then
            NewUnit.UnitGroup = SelectedUnitGroup.Item
        Else
            If Removals.Count = 0 Then
                Stop
                Exit Sub
            End If
            NewUnit.UnitGroup = Removals(0).UnitGroup
        End If
        NewUnit.Pos = TileAlignedPos(TileNum, New sXY_int(1, 1))
        NewUnit.Type = NewUnitType
        Dim UnitAdd As New clsUnitAdd
        UnitAdd.Map = Me
        UnitAdd.NewUnit = NewUnit
        UnitAdd.StoreChange = True
        UnitAdd.Perform()

        If Expand Then
            Dim Wall As clsUnit
            For Each Wall In Walls
                PerformTileWall(WallType, GetPosTileNum(Wall.Pos.Horizontal), False)
            Next
        End If
    End Sub

    Public Function Save_FMap_Prompt() As Boolean
        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = GetDirectory()
        Dialog.FileName = ""
        Dialog.Filter = ProgramName & " Map Files (*.fmap)|*.fmap"
        If Dialog.ShowDialog(frmMainInstance) <> Windows.Forms.DialogResult.OK Then
            Return False
        End If
        Settings.SavePath = IO.Path.GetDirectoryName(Dialog.FileName)
        Dim Result As clsResult
        Result = Write_FMap(Dialog.FileName, True, True)
        If Not Result.HasProblems Then
            PathInfo = New clsMap.clsPathInfo(Dialog.FileName, True)
            ChangedSinceSave = False
        End If
        ShowWarnings(Result)
        Return (Not Result.HasProblems)
    End Function

    Public Function Save_FMap_Quick() As Boolean

        If PathInfo Is Nothing Then
            Return Save_FMap_Prompt()
        ElseIf PathInfo.IsFMap Then
            Dim Result As clsResult = Write_FMap(PathInfo.Path, True, True)
            If Not Result.HasProblems Then
                ChangedSinceSave = False
            End If
            ShowWarnings(Result)
            Return (Not Result.HasProblems)
        Else
            Return Save_FMap_Prompt()
        End If
    End Function

    Public Function ClosePrompt() As Boolean

        If ChangedSinceSave Then
            Dim Prompt As New frmClose(GetTitle)
            Dim Result As DialogResult = Prompt.ShowDialog(frmMainInstance)
            Select Case Result
                Case DialogResult.OK
                    Return Save_FMap_Prompt()
                Case DialogResult.Yes
                    Return Save_FMap_Quick()
                Case DialogResult.No
                    Return True
                Case DialogResult.Cancel
                    Return False
                Case Else
                    Stop
                    Return False
            End Select
        Else
            Return True
        End If
    End Function
End Class
