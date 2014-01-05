
Partial Public Class clsMap

    Public Sub Rotate(Orientation As sTileOrientation, ObjectRotateMode As enumObjectRotateMode)
        Dim X As Integer
        Dim Y As Integer
        Dim Pos As sXY_int
        Dim RotatedPos As sXY_int
        Dim NewTerrainPosA As sXY_int = GetRotatedPos(Orientation, New sXY_int(0, 0), Terrain.TileSize)
        Dim NewTerrainPosB As sXY_int = GetRotatedPos(Orientation, Terrain.TileSize, Terrain.TileSize)
        Dim VertexLimits As New sXY_int(Math.Max(NewTerrainPosA.X, NewTerrainPosB.X), Math.Max(NewTerrainPosA.Y, NewTerrainPosB.Y))
        Dim NewTerrain As New clsTerrain(VertexLimits)
        Dim NewTileLimits As New sXY_int(NewTerrain.TileSize.X - 1, NewTerrain.TileSize.Y - 1)
        Dim NewSideHLimits As New sXY_int(NewTerrain.TileSize.X - 1, NewTerrain.TileSize.Y)
        Dim NewSideVLimits As New sXY_int(NewTerrain.TileSize.X, NewTerrain.TileSize.Y - 1)
        Dim OldTileLimits As New sXY_int(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1)
        Dim OldPosLimits As New sXY_int(Terrain.TileSize.X * TerrainGridSpacing, Terrain.TileSize.Y * TerrainGridSpacing)
        Dim ReverseOrientation As sTileOrientation
        Dim TriDirection As sTileDirection

        ReverseOrientation = Orientation
        ReverseOrientation.Reverse()

        For Y = 0 To NewTerrain.TileSize.Y
            Pos.Y = Y
            For X = 0 To NewTerrain.TileSize.X
                Pos.X = X
                RotatedPos = GetRotatedPos(ReverseOrientation, Pos, VertexLimits)
                NewTerrain.Vertices(X, Y).Height = Terrain.Vertices(RotatedPos.X, RotatedPos.Y).Height
                NewTerrain.Vertices(X, Y).Terrain = Terrain.Vertices(RotatedPos.X, RotatedPos.Y).Terrain
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y - 1
            Pos.Y = Y
            For X = 0 To NewTerrain.TileSize.X - 1
                Pos.X = X
                RotatedPos = GetRotatedPos(ReverseOrientation, Pos, NewTileLimits)
                NewTerrain.Tiles(X, Y).Texture = Terrain.Tiles(RotatedPos.X, RotatedPos.Y).Texture
                NewTerrain.Tiles(X, Y).Texture.Orientation = NewTerrain.Tiles(X, Y).Texture.Orientation.GetRotated(Orientation)
                NewTerrain.Tiles(X, Y).DownSide = Terrain.Tiles(RotatedPos.X, RotatedPos.Y).DownSide
                NewTerrain.Tiles(X, Y).DownSide = NewTerrain.Tiles(X, Y).DownSide.GetRotated(Orientation)
                If Terrain.Tiles(RotatedPos.X, RotatedPos.Y).Tri Then
                    TriDirection = TileDirection_TopLeft
                Else
                    TriDirection = TileDirection_TopRight
                End If
                TriDirection = TriDirection.GetRotated(Orientation)
                NewTerrain.Tiles(X, Y).Tri = (IdenticalTileDirections(TriDirection, TileDirection_TopLeft) Or IdenticalTileDirections(TriDirection, TileDirection_BottomRight))
                If Terrain.Tiles(RotatedPos.X, RotatedPos.Y).Tri Then
                    If Terrain.Tiles(RotatedPos.X, RotatedPos.Y).TriTopLeftIsCliff Then
                        RotateDirection(TileDirection_TopLeft, Orientation, TriDirection)
                        NewTerrain.Tiles(X, Y).TriCliffAddDirection(TriDirection)
                    End If
                    If Terrain.Tiles(RotatedPos.X, RotatedPos.Y).TriBottomRightIsCliff Then
                        RotateDirection(TileDirection_BottomRight, Orientation, TriDirection)
                        NewTerrain.Tiles(X, Y).TriCliffAddDirection(TriDirection)
                    End If
                Else
                    If Terrain.Tiles(RotatedPos.X, RotatedPos.Y).TriTopRightIsCliff Then
                        RotateDirection(TileDirection_TopRight, Orientation, TriDirection)
                        NewTerrain.Tiles(X, Y).TriCliffAddDirection(TriDirection)
                    End If
                    If Terrain.Tiles(RotatedPos.X, RotatedPos.Y).TriBottomLeftIsCliff Then
                        RotateDirection(TileDirection_BottomLeft, Orientation, TriDirection)
                        NewTerrain.Tiles(X, Y).TriCliffAddDirection(TriDirection)
                    End If
                End If
                NewTerrain.Tiles(X, Y).Terrain_IsCliff = Terrain.Tiles(RotatedPos.X, RotatedPos.Y).Terrain_IsCliff
            Next
        Next
        If Orientation.SwitchedAxes Then
            For Y = 0 To NewTerrain.TileSize.Y
                Pos.Y = Y
                For X = 0 To NewTerrain.TileSize.X - 1
                    Pos.X = X
                    RotatedPos = GetRotatedPos(ReverseOrientation, Pos, NewSideHLimits)
                    NewTerrain.SideH(X, Y).Road = Terrain.SideV(RotatedPos.X, RotatedPos.Y).Road
                Next
            Next
            For Y = 0 To NewTerrain.TileSize.Y - 1
                Pos.Y = Y
                For X = 0 To NewTerrain.TileSize.X
                    Pos.X = X
                    RotatedPos = GetRotatedPos(ReverseOrientation, Pos, NewSideVLimits)
                    NewTerrain.SideV(X, Y).Road = Terrain.SideH(RotatedPos.X, RotatedPos.Y).Road
                Next
            Next
        Else
            For Y = 0 To NewTerrain.TileSize.Y
                Pos.Y = Y
                For X = 0 To NewTerrain.TileSize.X - 1
                    Pos.X = X
                    RotatedPos = GetRotatedPos(ReverseOrientation, Pos, NewSideHLimits)
                    NewTerrain.SideH(X, Y).Road = Terrain.SideH(RotatedPos.X, RotatedPos.Y).Road
                Next
            Next
            For Y = 0 To NewTerrain.TileSize.Y - 1
                Pos.Y = Y
                For X = 0 To NewTerrain.TileSize.X
                    Pos.X = X
                    RotatedPos = GetRotatedPos(ReverseOrientation, Pos, NewSideVLimits)
                    NewTerrain.SideV(X, Y).Road = Terrain.SideV(RotatedPos.X, RotatedPos.Y).Road
                Next
            Next
        End If

        Dim Unit As clsMap.clsUnit
        For Each Unit In Units
            Unit.Sectors.Clear()
            If ObjectRotateMode = enumObjectRotateMode.All Then
                Unit.Rotation = CInt(AngleClamp(RadOf360Deg - GetRotatedAngle(Orientation, AngleClamp(RadOf360Deg - Unit.Rotation * RadOf1Deg))) / RadOf1Deg)
                If Unit.Rotation < 0 Then
                    Unit.Rotation += 360
                End If
            ElseIf ObjectRotateMode = enumObjectRotateMode.Walls Then
                If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                    If CType(Unit.Type, clsStructureType).StructureType = clsStructureType.enumStructureType.Wall Then
                        Unit.Rotation = CInt(AngleClamp(RadOf360Deg - GetRotatedAngle(Orientation, AngleClamp(RadOf360Deg - Unit.Rotation * RadOf1Deg))) / RadOf1Deg)
                        If Unit.Rotation < 0 Then
                            Unit.Rotation += 360
                        End If
                        'If Unit.Rotation = 180 Then
                        '    Unit.Rotation = 0
                        'ElseIf Unit.Rotation = 270 Then
                        '    Unit.Rotation = 90
                        'End If
                    End If
                End If
            End If
            Unit.Pos.Horizontal = GetRotatedPos(Orientation, Unit.Pos.Horizontal, OldPosLimits)
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        Dim Position As Integer
        For Each Unit In Units.GetItemsAsSimpleList
            If Not PosIsWithinTileArea(Unit.Pos.Horizontal, ZeroPos, NewTerrain.TileSize) Then
                Position = Unit.MapLink.ArrayPosition
                UnitRemove(Position)
            End If
        Next

        Terrain = NewTerrain

        Dim Gateway As clsGateway
        For Each Gateway In Gateways.GetItemsAsSimpleClassList
            GatewayCreate(GetRotatedPos(Orientation, Gateway.PosA, OldTileLimits), _
                           GetRotatedPos(Orientation, Gateway.PosB, OldTileLimits))
            Gateway.Deallocate()
        Next

        If _ReadyForUserInput Then
            CancelUserInput()
            InitializeUserInput()
        End If
    End Sub

    Public Sub RandomizeHeights(LevelCount As Integer)
        Dim hmSource As New clsHeightmap
        Dim hmA As New clsHeightmap
        Dim hmB As New clsHeightmap
        Dim IntervalCount As Integer
        Dim AlterationLevels As clsHeightmap.sHeights
        Dim hmAlteration As sHeightmaps
        Dim LevelHeight As Single
        Dim HeightRange As Double
        Dim Level As Integer
        Dim IntervalHeight As Double
        Dim Variation As Double
        Dim X As Integer
        Dim Y As Integer

        IntervalCount = LevelCount - 1

        ReDim AlterationLevels.Heights(IntervalCount)
        Dim MinMax As New clsHeightmap.sMinMax
        ReDim hmAlteration.Heightmaps(IntervalCount)
        ReDim hmSource.HeightData.Height(Terrain.TileSize.Y, Terrain.TileSize.X)
        hmSource.HeightData.SizeX = Terrain.TileSize.X + 1
        hmSource.HeightData.SizeY = Terrain.TileSize.Y + 1
        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                hmSource.HeightData.Height(Y, X) = CLng(Terrain.Vertices(X, Y).Height / hmSource.HeightScale)
            Next
        Next
        hmSource.MinMaxGet(MinMax)
        HeightRange = 255.0#
        IntervalHeight = HeightRange / IntervalCount
        Variation = IntervalHeight / 4.0#
        For Level = 0 To IntervalCount
            LevelHeight = CSng((MinMax.Min + Level * MinMax.Max / IntervalCount) * hmSource.HeightScale)
            AlterationLevels.Heights(Level) = LevelHeight
            hmB.GenerateNewOfSize(Terrain.TileSize.Y + 1, Terrain.TileSize.X + 1, 2.0F, 10000.0#)
            hmAlteration.Heightmaps(Level) = New clsHeightmap
            hmAlteration.Heightmaps(Level).Rescale(hmB, LevelHeight - Variation, LevelHeight + Variation)
        Next
        hmA.FadeMultiple(hmSource, hmAlteration, AlterationLevels)
        hmB.Rescale(hmA, Math.Max(MinMax.Min * hmSource.HeightScale - Variation, 0.0#), Math.Min(MinMax.Max * hmSource.HeightScale + Variation, 255.9#))
        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                Terrain.Vertices(X, Y).Height = CByte(Int(hmB.HeightData.Height(Y, X) * hmB.HeightScale))
            Next
        Next
    End Sub

    Public Sub LevelWater()
        Dim X As Integer
        Dim Y As Integer
        Dim TextureNum As Integer

        If Tileset Is Nothing Then
            Exit Sub
        End If

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum
                If TextureNum >= 0 And TextureNum < Tileset.TileCount Then
                    If Tileset.Tiles(TextureNum).Default_Type = TileTypeNum_Water Then
                        Terrain.Vertices(X, Y).Height = 0
                        Terrain.Vertices(X + 1, Y).Height = 0
                        Terrain.Vertices(X, Y + 1).Height = 0
                        Terrain.Vertices(X + 1, Y + 1).Height = 0
                    End If
                End If
            Next
        Next
    End Sub

    Public Structure sGenerateMasterTerrainArgs
        Public Tileset As clsGeneratorTileset
        Public LevelCount As Integer
        Public Class clsLayer
            Public WithinLayer As Integer
            Public AvoidLayers() As Boolean
            Public TileNum As Integer
            Public Terrainmap As clsBooleanMap
            Public TerrainmapScale As Single
            Public TerrainmapDensity As Single
            Public HeightMin As Single
            Public HeightMax As Single
            Public IsCliff As Boolean
        End Class
        Public Layers() As clsLayer
        Public LayerCount As Integer
        Public Watermap As clsBooleanMap
    End Structure

    Public Sub GenerateMasterTerrain(ByRef Args As sGenerateMasterTerrainArgs)
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim TerrainType(,) As Integer
        Dim Slope(,) As Single

        Dim TerrainNum As Integer

        Dim bmA As New clsBooleanMap
        Dim Layer_Num As Integer
        Dim LayerResult(Args.LayerCount - 1) As clsBooleanMap
        Dim bmB As New clsBooleanMap
        Dim BestSlope As Double
        Dim CurrentSlope As Double
        Dim hmB As New clsHeightmap
        Dim hmC As New clsHeightmap

        Dim difA As Double
        Dim difB As Double
        Dim NewTri As Boolean
        Dim CliffSlope As Double = Math.Atan(255.0# * DefaultHeightMultiplier / (2.0# * (Args.LevelCount - 1.0#) * TerrainGridSpacing)) - RadOf1Deg 'divided by 2 due to the terrain height randomization

        Tileset = Args.Tileset.Tileset

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                difA = Math.Abs(CDbl(Terrain.Vertices(X + 1, Y + 1).Height) - Terrain.Vertices(X, Y).Height)
                difB = Math.Abs(CDbl(Terrain.Vertices(X, Y + 1).Height) - Terrain.Vertices(X + 1, Y).Height)
                If difA = difB Then
                    If Rnd() >= 0.5F Then
                        NewTri = False
                    Else
                        NewTri = True
                    End If
                ElseIf difA < difB Then
                    NewTri = False
                Else
                    NewTri = True
                End If
                If Not Terrain.Tiles(X, Y).Tri = NewTri Then
                    Terrain.Tiles(X, Y).Tri = NewTri
                End If
            Next
        Next

        For A = 0 To Args.LayerCount - 1
            Args.Layers(A).Terrainmap = New clsBooleanMap
            If Args.Layers(A).TerrainmapDensity = 1.0F Then
                ReDim Args.Layers(A).Terrainmap.ValueData.Value(Terrain.TileSize.Y - 1, Terrain.TileSize.X - 1)
                Args.Layers(A).Terrainmap.ValueData.Size = Terrain.TileSize
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        Args.Layers(A).Terrainmap.ValueData.Value(Y, X) = True
                    Next
                Next
            Else
                hmB.GenerateNewOfSize(Terrain.TileSize.Y, Terrain.TileSize.X, Args.Layers(A).TerrainmapScale, 1.0#)
                hmC.Rescale(hmB, 0.0#, 1.0#)
                Args.Layers(A).Terrainmap.Convert_Heightmap(hmC, CLng((1.0F - Args.Layers(A).TerrainmapDensity) / hmC.HeightScale))
            End If
        Next

        Dim Pos As sXY_int

        ReDim TerrainType(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1)
        ReDim Slope(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1)
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                'get slope
                BestSlope = 0.0#

                Pos.X = CInt((X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.25#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.25#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.75#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.75#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Slope(X, Y) = CSng(BestSlope)
            Next
        Next
        For Layer_Num = 0 To Args.LayerCount - 1
            TerrainNum = Args.Layers(Layer_Num).TileNum
            If TerrainNum >= 0 Then
                'do other layer constraints
                LayerResult(Layer_Num) = New clsBooleanMap
                LayerResult(Layer_Num).Copy(Args.Layers(Layer_Num).Terrainmap)
                If Args.Layers(Layer_Num).WithinLayer >= 0 Then
                    If Args.Layers(Layer_Num).WithinLayer < Layer_Num Then
                        bmA.Within(LayerResult(Layer_Num), LayerResult(Args.Layers(Layer_Num).WithinLayer))
                        LayerResult(Layer_Num).ValueData = bmA.ValueData
                        bmA.ValueData = New clsBooleanMap.clsValueData
                    End If
                End If
                For A = 0 To Layer_Num - 1
                    If Args.Layers(Layer_Num).AvoidLayers(A) Then
                        bmA.Expand_One_Tile(LayerResult(A))
                        bmB.Remove(LayerResult(Layer_Num), bmA)
                        LayerResult(Layer_Num).ValueData = bmB.ValueData
                        bmB.ValueData = New clsBooleanMap.clsValueData
                    End If
                Next
                'do height and slope constraints
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If LayerResult(Layer_Num).ValueData.Value(Y, X) Then
                            If Terrain.Vertices(X, Y).Height < Args.Layers(Layer_Num).HeightMin _
                            Or Terrain.Vertices(X, Y).Height > Args.Layers(Layer_Num).HeightMax Then
                                LayerResult(Layer_Num).ValueData.Value(Y, X) = False
                            End If
                            If Args.Layers(Layer_Num).IsCliff Then
                                If LayerResult(Layer_Num).ValueData.Value(Y, X) Then
                                    If Slope(X, Y) < CliffSlope Then
                                        LayerResult(Layer_Num).ValueData.Value(Y, X) = False
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next

                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If LayerResult(Layer_Num).ValueData.Value(Y, X) Then
                            TerrainType(X, Y) = TerrainNum
                        End If
                    Next
                Next
            End If
        Next

        'set water tiles

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                If Args.Watermap.ValueData.Value(Y, X) Then
                    If Slope(X, Y) < CliffSlope Then
                        TerrainType(X, Y) = 17
                    End If
                End If
            Next
        Next

        'set border tiles to cliffs
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To 2
                TerrainType(X, Y) = Args.Tileset.BorderTextureNum
            Next
            For X = Terrain.TileSize.X - 4 To Terrain.TileSize.X - 1
                TerrainType(X, Y) = Args.Tileset.BorderTextureNum
            Next
        Next
        For X = 3 To Terrain.TileSize.X - 5
            For Y = 0 To 2
                TerrainType(X, Y) = Args.Tileset.BorderTextureNum
            Next
            For Y = Terrain.TileSize.Y - 4 To Terrain.TileSize.Y - 1
                TerrainType(X, Y) = Args.Tileset.BorderTextureNum
            Next
        Next

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Terrain.Tiles(X, Y).Texture.TextureNum = TerrainType(X, Y)
            Next
        Next
    End Sub

    Public Sub RandomizeTileOrientations()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Terrain.Tiles(X, Y).Texture.Orientation = New sTileOrientation(Rnd() >= 0.5F, Rnd() >= 0.5F, Rnd() >= 0.5F)
            Next
        Next
        SectorTerrainUndoChanges.SetAllChanged()
        SectorGraphicsChanges.SetAllChanged()
    End Sub

    Public Sub MapTexturer(ByRef LayerList As sLayerList)
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim TerrainType(,) As clsPainter.clsTerrain
        Dim Slope(,) As Single
        Dim tmpTerrain As clsPainter.clsTerrain
        Dim bmA As New clsBooleanMap
        Dim bmB As New clsBooleanMap
        Dim LayerNum As Integer
        Dim LayerResult(LayerList.LayerCount - 1) As clsBooleanMap
        Dim BestSlope As Double
        Dim CurrentSlope As Double
        Dim AllowSlope As Boolean
        Dim Pos As sXY_int

        ReDim TerrainType(Terrain.TileSize.X, Terrain.TileSize.Y)
        ReDim Slope(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1)
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                'get slope
                BestSlope = 0.0#

                Pos.X = CInt((X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.25#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.25#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.75#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.75#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Slope(X, Y) = CSng(BestSlope)
            Next
        Next
        For LayerNum = 0 To LayerList.LayerCount - 1
            tmpTerrain = LayerList.Layers(LayerNum).Terrain
            If tmpTerrain IsNot Nothing Then
                'do other layer constraints
                LayerResult(LayerNum) = New clsBooleanMap
                LayerResult(LayerNum).Copy(LayerList.Layers(LayerNum).Terrainmap)
                If LayerList.Layers(LayerNum).WithinLayer >= 0 Then
                    If LayerList.Layers(LayerNum).WithinLayer < LayerNum Then
                        bmA.Within(LayerResult(LayerNum), LayerResult(LayerList.Layers(LayerNum).WithinLayer))
                        LayerResult(LayerNum).ValueData = bmA.ValueData
                        bmA.ValueData = New clsBooleanMap.clsValueData
                    End If
                End If
                For A = 0 To LayerNum - 1
                    If LayerList.Layers(LayerNum).AvoidLayers(A) Then
                        bmA.Expand_One_Tile(LayerResult(A))
                        bmB.Remove(LayerResult(LayerNum), bmA)
                        LayerResult(LayerNum).ValueData = bmB.ValueData
                        bmB.ValueData = New clsBooleanMap.clsValueData
                    End If
                Next
                'do height and slope constraints
                For Y = 0 To Terrain.TileSize.Y
                    For X = 0 To Terrain.TileSize.X
                        If LayerResult(LayerNum).ValueData.Value(Y, X) Then
                            If Terrain.Vertices(X, Y).Height < LayerList.Layers(LayerNum).HeightMin _
                            Or Terrain.Vertices(X, Y).Height > LayerList.Layers(LayerNum).HeightMax Then
                                LayerResult(LayerNum).ValueData.Value(Y, X) = False
                            End If
                            If LayerResult(LayerNum).ValueData.Value(Y, X) Then
                                AllowSlope = True
                                If X > 0 Then
                                    If Y > 0 Then
                                        If Slope(X - 1, Y - 1) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X - 1, Y - 1) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                    If Y < Terrain.TileSize.Y Then
                                        If Slope(X - 1, Y) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X - 1, Y) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                End If
                                If X < Terrain.TileSize.X Then
                                    If Y > 0 Then
                                        If Slope(X, Y - 1) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X, Y - 1) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                    If Y < Terrain.TileSize.Y Then
                                        If Slope(X, Y) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X, Y) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                End If
                                If Not AllowSlope Then
                                    LayerResult(LayerNum).ValueData.Value(Y, X) = False
                                End If
                            End If
                        End If
                    Next
                Next

                LayerResult(LayerNum).Remove_Diagonals()

                For Y = 0 To Terrain.TileSize.Y
                    For X = 0 To Terrain.TileSize.X
                        If LayerResult(LayerNum).ValueData.Value(Y, X) Then
                            TerrainType(X, Y) = tmpTerrain
                        End If
                    Next
                Next
            End If
        Next

        'set vertex terrain by terrain map
        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                If TerrainType(X, Y) IsNot Nothing Then
                    Terrain.Vertices(X, Y).Terrain = TerrainType(X, Y)
                End If
            Next
        Next
        AutoTextureChanges.SetAllChanged()
        UpdateAutoTextures()
    End Sub

    Public Function GenerateTerrainMap(Scale As Single, Density As Single) As clsBooleanMap
        Dim ReturnResult As clsBooleanMap
        Dim hmB As New clsHeightmap
        Dim hmC As New clsHeightmap

        hmB.GenerateNewOfSize(Terrain.TileSize.Y + 1, Terrain.TileSize.X + 1, Scale, 1.0#)
        hmC.Rescale(hmB, 0.0#, 1.0#)
        ReturnResult = New clsBooleanMap
        ReturnResult.Convert_Heightmap(hmC, CLng((1.0# - Density) / hmC.HeightScale))
        Return ReturnResult
    End Function

    Public Sub WaterTriCorrection()

        If Tileset Is Nothing Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Y As Integer
        Dim TileNum As sXY_int

        For Y = 0 To Terrain.TileSize.Y - 1
            TileNum.Y = Y
            For X = 0 To Terrain.TileSize.X - 1
                TileNum.X = X
                If Terrain.Tiles(X, Y).Tri Then
                    If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 Then
                        If Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                            Terrain.Tiles(X, Y).Tri = False
                            SectorGraphicsChanges.TileChanged(TileNum)
                            SectorTerrainUndoChanges.TileChanged(TileNum)
                        End If
                    End If
                End If
            Next
        Next
    End Sub

    Public MustInherit Class clsAction

        Public Map As clsMap
        Public PosNum As sXY_int
        Public UseEffect As Boolean
        Public Effect As Double

        Public MustOverride Sub ActionPerform()
    End Class

    Public MustInherit Class clsObjectAction
        Implements SimpleListTool(Of clsMap.clsUnit)

        Public Map As clsMap
        Public Unit As clsUnit
        Private _ResultUnits As New SimpleClassList(Of clsMap.clsUnit)
        Public ActionPerformed As Boolean

        Protected ResultUnit As clsMap.clsUnit

        Public ReadOnly Property ResultUnits As SimpleClassList(Of clsMap.clsUnit)
            Get
                Return _ResultUnits
            End Get
        End Property

        Protected Overridable Sub ActionCondition()

        End Sub

        Public Sub ActionPerform() Implements SimpleListTool(Of clsUnit).ActionPerform

            ResultUnit = Nothing
            ActionPerformed = False
            If Unit Is Nothing Then
                Stop
                Exit Sub
            End If
            ActionPerformed = True
            ActionCondition()
            If Not ActionPerformed Then
                Exit Sub
            End If
            ResultUnit = New clsMap.clsUnit(Unit, Map)
            _ActionPerform()
            If ResultUnit Is Nothing Then
                ResultUnit = Unit
            Else
                _ResultUnits.Add(ResultUnit)
                Map.UnitSwap(Unit, ResultUnit)
            End If
        End Sub

        Protected MustOverride Sub _ActionPerform()

        Public Sub SetItem(Item As clsUnit) Implements SimpleListTool(Of clsUnit).SetItem

            Unit = Item
        End Sub
    End Class

    Public Class clsApplyCliff
        Inherits clsMap.clsAction

        Public Angle As Double
        Public SetTris As Boolean

        Private RandomNum As Integer
        Private DifA As Double
        Private DifB As Double
        Private HeightA As Double
        Private HeightB As Double
        Private TriTopLeftMaxSlope As Double
        Private TriTopRightMaxSlope As Double
        Private TriBottomLeftMaxSlope As Double
        Private TriBottomRightMaxSlope As Double
        Private CliffChanged As Boolean
        Private TriChanged As Boolean
        Private NewVal As Boolean
        Private Terrain As clsTerrain
        Private Pos As sXY_int

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            HeightA = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height) / 2.0#
            HeightB = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) / 2.0#
            DifA = HeightB - HeightA
            HeightA = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) / 2.0#
            HeightB = (CDbl(Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) / 2.0#
            DifB = HeightB - HeightA
            If Math.Abs(DifA) = Math.Abs(DifB) Then
                RandomNum = CInt(Int(Rnd() * 4.0F))
                If RandomNum = 0 Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Top
                ElseIf RandomNum = 1 Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Right
                ElseIf RandomNum = 2 Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Bottom
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Left
                End If
            ElseIf Math.Abs(DifA) > Math.Abs(DifB) Then
                If DifA < 0.0# Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Bottom
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Top
                End If
            Else
                If DifB < 0.0# Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Right
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Left
                End If
            End If

            CliffChanged = False
            TriChanged = False

            If SetTris Then
                DifA = Math.Abs(CDbl(Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) - Terrain.Vertices(PosNum.X, PosNum.Y).Height)
                DifB = Math.Abs(CDbl(Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) - Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height)
                If DifA = DifB Then
                    If Rnd() >= 0.5F Then
                        NewVal = False
                    Else
                        NewVal = True
                    End If
                ElseIf DifA < DifB Then
                    NewVal = False
                Else
                    NewVal = True
                End If
                If Terrain.Tiles(PosNum.X, PosNum.Y).Tri <> NewVal Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Tri = NewVal
                    TriChanged = True
                End If
            End If

            If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                Pos.X = CInt((PosNum.X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((PosNum.Y + 0.25#) * TerrainGridSpacing)
                TriTopLeftMaxSlope = Map.GetTerrainSlopeAngle(Pos)
                Pos.X = CInt((PosNum.X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((PosNum.Y + 0.75#) * TerrainGridSpacing)
                TriBottomRightMaxSlope = Map.GetTerrainSlopeAngle(Pos)

                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False
                    CliffChanged = True
                End If
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
                    CliffChanged = True
                End If

                NewVal = (TriTopLeftMaxSlope >= Angle)
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff <> NewVal Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = NewVal
                    CliffChanged = True
                End If

                NewVal = (TriBottomRightMaxSlope >= Angle)
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff <> NewVal Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = NewVal
                    CliffChanged = True
                End If

                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = False
                End If
            Else
                Pos.X = CInt((PosNum.X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((PosNum.Y + 0.25#) * TerrainGridSpacing)
                TriTopRightMaxSlope = Map.GetTerrainSlopeAngle(Pos)
                Pos.X = CInt((PosNum.X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((PosNum.Y + 0.75#) * TerrainGridSpacing)
                TriBottomLeftMaxSlope = Map.GetTerrainSlopeAngle(Pos)

                If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False
                    CliffChanged = True
                End If
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
                    CliffChanged = True
                End If

                NewVal = (TriTopRightMaxSlope >= Angle)
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff <> NewVal Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = NewVal
                    CliffChanged = True
                End If

                NewVal = (TriBottomLeftMaxSlope >= Angle)
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff <> NewVal Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = NewVal
                    CliffChanged = True
                End If

                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = False
                End If
            End If

            If CliffChanged Then
                Map.AutoTextureChanges.TileChanged(PosNum)
            End If
            If TriChanged Or CliffChanged Then
                Map.SectorGraphicsChanges.TileChanged(PosNum)
                Map.SectorTerrainUndoChanges.TileChanged(PosNum)
            End If
        End Sub
    End Class

    Public Class clsApplyCliffRemove
        Inherits clsMap.clsAction

        Private Terrain As clsTerrain

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            If Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False

                Map.AutoTextureChanges.TileChanged(PosNum)
                Map.SectorGraphicsChanges.TileChanged(PosNum)
                Map.SectorTerrainUndoChanges.TileChanged(PosNum)
            End If
        End Sub
    End Class

    Public Class clsApplyCliffTriangle
        Inherits clsMap.clsAction

        Public Triangle As Boolean

        Private Terrain As clsTerrain
        Private CliffChanged As Boolean

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True

            CliffChanged = False
            If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                If Triangle Then
                    If Not Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = True
                        CliffChanged = True
                    End If
                Else
                    If Not Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = True
                        CliffChanged = True
                    End If
                End If
            Else
                If Triangle Then
                    If Not Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = True
                        CliffChanged = True
                    End If
                Else
                    If Not Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = True
                        CliffChanged = True
                    End If
                End If
            End If

            If Not CliffChanged Then
                Exit Sub
            End If

            Dim HeightA As Double
            Dim HeightB As Double
            Dim difA As Double
            Dim difB As Double
            Dim A As Integer

            Map.AutoTextureChanges.TileChanged(PosNum)
            Map.SectorGraphicsChanges.TileChanged(PosNum)
            Map.SectorTerrainUndoChanges.TileChanged(PosNum)

            HeightA = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height) / 2.0#
            HeightB = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) / 2.0#
            difA = HeightB - HeightA
            HeightA = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) / 2.0#
            HeightB = (CDbl(Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) / 2.0#
            difB = HeightB - HeightA
            If Math.Abs(difA) = Math.Abs(difB) Then
                A = CInt(Int(Rnd() * 4.0F))
                If A = 0 Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Top
                ElseIf A = 1 Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Right
                ElseIf A = 2 Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Bottom
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Left
                End If
            ElseIf Math.Abs(difA) > Math.Abs(difB) Then
                If difA < 0.0# Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Bottom
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Top
                End If
            Else
                If difB < 0.0# Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Right
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Left
                End If
            End If
        End Sub
    End Class

    Public Class clsApplyCliffTriangleRemove
        Inherits clsMap.clsAction

        Public Triangle As Boolean

        Private Terrain As clsTerrain
        Private CliffChanged As Boolean

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            CliffChanged = False
            If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                If Triangle Then
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False
                        CliffChanged = True
                    End If
                Else
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
                        CliffChanged = True
                    End If
                End If
                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = (Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff)
            Else
                If Triangle Then
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False
                        CliffChanged = True
                    End If
                Else
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
                        CliffChanged = True
                    End If
                End If
                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = (Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff)
            End If

            If Not CliffChanged Then
                Exit Sub
            End If

            Map.AutoTextureChanges.TileChanged(PosNum)
            Map.SectorGraphicsChanges.TileChanged(PosNum)
            Map.SectorTerrainUndoChanges.TileChanged(PosNum)
        End Sub
    End Class

    Public Class clsApplyHeightChange
        Inherits clsMap.clsAction

        Public Rate As Double

        Private Terrain As clsTerrain

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            Terrain.Vertices(PosNum.X, PosNum.Y).Height = CByte(Clamp_int(CInt(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + CInt(Rate * Effect), Byte.MinValue, Byte.MaxValue))

            Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum)
            Map.SectorUnitHeightsChanges.VertexChanged(PosNum)
            Map.SectorTerrainUndoChanges.VertexChanged(PosNum)
        End Sub
    End Class

    Public Class clsApplyHeightSet
        Inherits clsMap.clsAction

        Public Height As Byte

        Private Terrain As clsTerrain

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            If Terrain.Vertices(PosNum.X, PosNum.Y).Height <> Height Then
                Terrain.Vertices(PosNum.X, PosNum.Y).Height = Height
                Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum)
                Map.SectorUnitHeightsChanges.VertexChanged(PosNum)
                Map.SectorTerrainUndoChanges.VertexChanged(PosNum)
            End If
        End Sub
    End Class

    Public Class clsApplyHeightSmoothing
        Inherits clsMap.clsAction

        Public Ratio As Double
        Public Offset As sXY_int
        Public AreaTileSize As sXY_int

        Private NewHeight(,) As Byte
        Private Started As Boolean
        Private TempHeight As Integer
        Private Samples As Integer
        Private LimitX As Integer
        Private LimitY As Integer
        Private XNum As Integer
        Private VertexNum As sXY_int
        Private Terrain As clsTerrain

        Public Sub Start()
            Dim X As Integer
            Dim Y As Integer

            Terrain = Map.Terrain

            ReDim NewHeight(AreaTileSize.X, AreaTileSize.Y)
            For Y = 0 To AreaTileSize.Y
                For X = 0 To AreaTileSize.X
                    NewHeight(X, Y) = Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height
                Next
            Next

            Started = True
        End Sub

        Public Sub Finish()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim X As Integer
            Dim Y As Integer

            Terrain = Map.Terrain

            For Y = 0 To AreaTileSize.Y
                VertexNum.Y = Offset.Y + Y
                For X = 0 To AreaTileSize.X
                    VertexNum.X = Offset.X + X
                    Terrain.Vertices(VertexNum.X, VertexNum.Y).Height = NewHeight(X, Y)

                    Map.SectorGraphicsChanges.VertexAndNormalsChanged(VertexNum)
                    Map.SectorUnitHeightsChanges.VertexChanged(VertexNum)
                    Map.SectorTerrainUndoChanges.VertexChanged(VertexNum)
                Next
            Next

            Started = False
        End Sub

        Public Overrides Sub ActionPerform()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim X As Integer
            Dim Y As Integer
            Dim X2 As Integer
            Dim Y2 As Integer

            Terrain = Map.Terrain

            LimitX = Terrain.TileSize.X
            LimitY = Terrain.TileSize.Y
            TempHeight = 0
            Samples = 0
            For Y = Clamp_int(SmoothRadius.Tiles.YMin + PosNum.Y, 0, LimitY) - PosNum.Y To Clamp_int(SmoothRadius.Tiles.YMax + PosNum.Y, 0, LimitY) - PosNum.Y
                Y2 = PosNum.Y + Y
                XNum = Y - SmoothRadius.Tiles.YMin
                For X = Clamp_int(SmoothRadius.Tiles.XMin(XNum) + PosNum.X, 0, LimitX) - PosNum.X To Clamp_int(SmoothRadius.Tiles.XMax(XNum) + PosNum.X, 0, LimitX) - PosNum.X
                    X2 = PosNum.X + X
                    TempHeight += Terrain.Vertices(X2, Y2).Height
                    Samples += 1
                Next
            Next
            NewHeight(PosNum.X - Offset.X, PosNum.Y - Offset.Y) = CByte(Math.Min(CInt(Terrain.Vertices(PosNum.X, PosNum.Y).Height * (1.0# - Ratio) + TempHeight / Samples * Ratio), Byte.MaxValue))
        End Sub
    End Class

    Public Class clsApplyRoadRemove
        Inherits clsMap.clsAction

        Private Terrain As clsTerrain

        Private Sub ToolPerformSideH(SideNum As sXY_int)

            Terrain = Map.Terrain

            If Terrain.SideH(SideNum.X, SideNum.Y).Road IsNot Nothing Then
                Terrain.SideH(SideNum.X, SideNum.Y).Road = Nothing
                Map.AutoTextureChanges.SideHChanged(SideNum)
                Map.SectorGraphicsChanges.SideHChanged(SideNum)
                Map.SectorTerrainUndoChanges.SideHChanged(SideNum)
            End If
        End Sub

        Private Sub ToolPerformSideV(SideNum As sXY_int)

            Terrain = Map.Terrain

            If Terrain.SideV(SideNum.X, SideNum.Y).Road IsNot Nothing Then
                Terrain.SideV(SideNum.X, SideNum.Y).Road = Nothing
                Map.AutoTextureChanges.SideVChanged(SideNum)
                Map.SectorGraphicsChanges.SideVChanged(SideNum)
                Map.SectorTerrainUndoChanges.SideVChanged(SideNum)
            End If
        End Sub

        Public Overrides Sub ActionPerform()

            ToolPerformSideH(PosNum)
            ToolPerformSideH(New sXY_int(PosNum.X, PosNum.Y + 1))
            ToolPerformSideV(PosNum)
            ToolPerformSideV(New sXY_int(PosNum.X + 1, PosNum.Y))
        End Sub
    End Class

    Public Class clsApplyVertexTerrain
        Inherits clsMap.clsAction

        Public VertexTerrain As clsPainter.clsTerrain

        Private Terrain As clsTerrain

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain IsNot VertexTerrain Then
                Terrain.Vertices(PosNum.X, PosNum.Y).Terrain = VertexTerrain
                Map.SectorGraphicsChanges.VertexChanged(PosNum)
                Map.SectorTerrainUndoChanges.VertexChanged(PosNum)
                Map.AutoTextureChanges.VertexChanged(PosNum)
            End If
        End Sub
    End Class

    Public Class clsApplyTexture
        Inherits clsMap.clsAction

        Public TextureNum As Integer
        Public SetTexture As Boolean
        Public Orientation As sTileOrientation
        Public SetOrientation As Boolean
        Public RandomOrientation As Boolean
        Public TerrainAction As enumTextureTerrainAction

        Private Terrain As clsTerrain

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = False
            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False
            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False

            If SetTexture Then
                Terrain.Tiles(PosNum.X, PosNum.Y).Texture.TextureNum = TextureNum
            End If
            If SetOrientation Then
                If RandomOrientation Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Texture.Orientation = New sTileOrientation(Rnd() < 0.5F, Rnd() < 0.5F, Rnd() < 0.5F)
                Else
                    Terrain.Tiles(PosNum.X, PosNum.Y).Texture.Orientation = Orientation
                End If
            End If

            Map.TileTextureChangeTerrainAction(PosNum, TerrainAction)

            Map.SectorGraphicsChanges.TileChanged(PosNum)
            Map.SectorTerrainUndoChanges.TileChanged(PosNum)
        End Sub
    End Class

    Public Class clsApplyVertexTerrainInterpret
        Inherits clsMap.clsAction

        Private TerrainCount() As Integer
        Private VertexDirection As sTileDirection
        Private Painter As clsPainter
        Private PainterTerrainA As clsPainter.clsTerrain
        Private PainterTerrainB As clsPainter.clsTerrain
        Private Texture As clsTerrain.Tile.sTexture
        Private ResultDirection As sTileDirection
        Private PainterTexture As clsPainter.clsTileList.sTileOrientationChance
        Private OppositeDirection As sTileDirection
        Private BestNum As Integer
        Private BestCount As Integer
        Private Tile As clsTerrain.Tile
        Private Terrain As clsTerrain

        Private Sub ToolPerformTile()
            Dim PainterBrushNum As Integer
            Dim A As Integer

            For PainterBrushNum = 0 To Painter.TerrainCount - 1
                PainterTerrainA = Painter.Terrains(PainterBrushNum)
                For A = 0 To PainterTerrainA.Tiles.TileCount - 1
                    PainterTexture = PainterTerrainA.Tiles.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        TerrainCount(PainterTerrainA.Num) += 1
                    End If
                Next
            Next
            For PainterBrushNum = 0 To Painter.TransitionBrushCount - 1
                PainterTerrainA = Painter.TransitionBrushes(PainterBrushNum).Terrain_Inner
                PainterTerrainB = Painter.TransitionBrushes(PainterBrushNum).Terrain_Outer
                For A = 0 To Painter.TransitionBrushes(PainterBrushNum).Tiles_Straight.TileCount - 1
                    PainterTexture = Painter.TransitionBrushes(PainterBrushNum).Tiles_Straight.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If DirectionsOnSameSide(VertexDirection, ResultDirection) Then
                            TerrainCount(PainterTerrainB.Num) += 1
                        Else
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    End If
                Next
                For A = 0 To Painter.TransitionBrushes(PainterBrushNum).Tiles_Corner_In.TileCount - 1
                    PainterTexture = Painter.TransitionBrushes(PainterBrushNum).Tiles_Corner_In.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If IdenticalTileDirections(VertexDirection, ResultDirection) Then
                            TerrainCount(PainterTerrainB.Num) += 1
                        Else
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    End If
                Next
                For A = 0 To Painter.TransitionBrushes(PainterBrushNum).Tiles_Corner_Out.TileCount - 1
                    PainterTexture = Painter.TransitionBrushes(PainterBrushNum).Tiles_Corner_Out.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        OppositeDirection = PainterTexture.Direction
                        OppositeDirection.FlipX()
                        OppositeDirection.FlipY()
                        RotateDirection(OppositeDirection, Texture.Orientation, ResultDirection)
                        If IdenticalTileDirections(VertexDirection, ResultDirection) Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        Else
                            TerrainCount(PainterTerrainB.Num) += 1
                        End If
                    End If
                Next
            Next

            For PainterBrushNum = 0 To Painter.CliffBrushCount - 1
                PainterTerrainA = Painter.CliffBrushes(PainterBrushNum).Terrain_Inner
                PainterTerrainB = Painter.CliffBrushes(PainterBrushNum).Terrain_Outer
                For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Straight.TileCount - 1
                    PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Straight.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If DirectionsOnSameSide(VertexDirection, ResultDirection) Then
                            TerrainCount(PainterTerrainB.Num) += 1
                        Else
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    End If
                Next
                For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_In.TileCount - 1
                    PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_In.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If IdenticalTileDirections(VertexDirection, ResultDirection) Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        Else
                            TerrainCount(PainterTerrainB.Num) += 1
                        End If
                    End If
                Next
                For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_Out.TileCount - 1
                    PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_Out.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        OppositeDirection = PainterTexture.Direction
                        OppositeDirection.FlipX()
                        OppositeDirection.FlipY()
                        RotateDirection(OppositeDirection, Texture.Orientation, ResultDirection)
                        If IdenticalTileDirections(VertexDirection, ResultDirection) Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        Else
                            TerrainCount(PainterTerrainB.Num) += 1
                        End If
                    End If
                Next
            Next

            For PainterBrushNum = 0 To Painter.RoadBrushCount - 1
                PainterTerrainA = Painter.RoadBrushes(PainterBrushNum).Terrain
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_Corner_In.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_Corner_In.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        TerrainCount(PainterTerrainA.Num) += 1
                    End If
                Next
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_CrossIntersection.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_CrossIntersection.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        TerrainCount(PainterTerrainA.Num) += 1
                    End If
                Next
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_End.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_End.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        TerrainCount(PainterTerrainA.Num) += 1
                    End If
                Next
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_Straight.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_Straight.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        TerrainCount(PainterTerrainA.Num) += 1
                    End If
                Next
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_TIntersection.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_TIntersection.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        TerrainCount(PainterTerrainA.Num) += 1
                    End If
                Next
            Next
        End Sub

        Public Overrides Sub ActionPerform()
            Dim A As Integer

            Terrain = Map.Terrain

            Painter = Map.Painter
            ReDim TerrainCount(Painter.TerrainCount - 1)

            If PosNum.Y > 0 Then
                If PosNum.X > 0 Then
                    VertexDirection = TileDirection_BottomRight
                    Tile = Terrain.Tiles(PosNum.X - 1, PosNum.Y - 1)
                    Texture = Tile.Texture
                    ToolPerformTile()
                End If
                If PosNum.X < Terrain.TileSize.X Then
                    VertexDirection = TileDirection_BottomLeft
                    Tile = Terrain.Tiles(PosNum.X, PosNum.Y - 1)
                    Texture = Tile.Texture
                    ToolPerformTile()
                End If
            End If
            If PosNum.Y < Terrain.TileSize.Y Then
                If PosNum.X > 0 Then
                    VertexDirection = TileDirection_TopRight
                    Tile = Terrain.Tiles(PosNum.X - 1, PosNum.Y)
                    Texture = Tile.Texture
                    ToolPerformTile()
                End If
                If PosNum.X < Terrain.TileSize.X Then
                    VertexDirection = TileDirection_TopLeft
                    Tile = Terrain.Tiles(PosNum.X, PosNum.Y)
                    Texture = Tile.Texture
                    ToolPerformTile()
                End If
            End If

            BestNum = -1
            BestCount = 0
            For A = 0 To Painter.TerrainCount - 1
                If TerrainCount(A) > BestCount Then
                    BestNum = A
                    BestCount = TerrainCount(A)
                End If
            Next
            If BestCount > 0 Then
                Terrain.Vertices(PosNum.X, PosNum.Y).Terrain = Painter.Terrains(BestNum)
            Else
                Terrain.Vertices(PosNum.X, PosNum.Y).Terrain = Nothing
            End If

            Map.SectorTerrainUndoChanges.VertexChanged(PosNum)
        End Sub
    End Class

    Public Class clsApplyTileTerrainInterpret
        Inherits clsMap.clsAction

        Private Painter As clsPainter
        Private PainterTerrainA As clsPainter.clsTerrain
        Private PainterTerrainB As clsPainter.clsTerrain
        Private Texture As clsTerrain.Tile.sTexture
        Private ResultDirection As sTileDirection
        Private PainterTexture As clsPainter.clsTileList.sTileOrientationChance
        Private OppositeDirection As sTileDirection
        Private Tile As clsTerrain.Tile
        Private Terrain As clsTerrain

        Public Overrides Sub ActionPerform()
            Dim PainterBrushNum As Integer
            Dim A As Integer

            Terrain = Map.Terrain

            Painter = Map.Painter

            Tile = Terrain.Tiles(PosNum.X, PosNum.Y)
            Texture = Tile.Texture

            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False
            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False
            Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_None

            For PainterBrushNum = 0 To Painter.CliffBrushCount - 1
                PainterTerrainA = Painter.CliffBrushes(PainterBrushNum).Terrain_Inner
                PainterTerrainB = Painter.CliffBrushes(PainterBrushNum).Terrain_Outer
                For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Straight.TileCount - 1
                    PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Straight.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        If Tile.Tri Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = True
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = True
                        Else
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = True
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = True
                        End If
                        Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = ResultDirection
                    End If
                Next
                For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_In.TileCount - 1
                    PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_In.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If Tile.Tri Then
                            If IdenticalTileDirections(ResultDirection, TileDirection_TopLeft) Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            ElseIf IdenticalTileDirections(ResultDirection, TileDirection_BottomRight) Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            End If
                        Else
                            If IdenticalTileDirections(ResultDirection, TileDirection_TopRight) Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            ElseIf IdenticalTileDirections(ResultDirection, TileDirection_BottomLeft) Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            End If
                        End If
                    End If
                Next
                For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_Out.TileCount - 1
                    PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_Out.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        OppositeDirection = PainterTexture.Direction
                        OppositeDirection.FlipX()
                        OppositeDirection.FlipY()
                        RotateDirection(OppositeDirection, Texture.Orientation, ResultDirection)
                        If Tile.Tri Then
                            If IdenticalTileDirections(ResultDirection, TileDirection_TopLeft) Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            ElseIf IdenticalTileDirections(ResultDirection, TileDirection_BottomRight) Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            End If
                        Else
                            If IdenticalTileDirections(ResultDirection, TileDirection_TopRight) Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            ElseIf IdenticalTileDirections(ResultDirection, TileDirection_BottomLeft) Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            End If
                        End If
                    End If
                Next
            Next

            Map.SectorTerrainUndoChanges.TileChanged(PosNum)
        End Sub
    End Class

    Public MustInherit Class clsApplySideTerrainInterpret
        Inherits clsMap.clsAction

        Protected Painter As clsPainter
        Protected PainterTerrain As clsPainter.clsTerrain
        Protected PainterRoad As clsPainter.clsRoad
        Protected Texture As clsTerrain.Tile.sTexture
        Protected ResultDirection As sTileDirection
        Protected PainterTexture As clsPainter.clsTileList.sTileOrientationChance
        Protected OppositeDirection As sTileDirection
        Protected Tile As clsTerrain.Tile
        Protected RoadCount() As Integer
        Protected SideDirection As sTileDirection
        Protected BestNum As Integer
        Protected BestCount As Integer
        Protected Terrain As clsTerrain

        Protected Sub ToolPerformTile()
            Dim PainterBrushNum As Integer
            Dim A As Integer

            For PainterBrushNum = 0 To Painter.RoadBrushCount - 1
                PainterRoad = Painter.RoadBrushes(PainterBrushNum).Road
                PainterTerrain = Painter.RoadBrushes(PainterBrushNum).Terrain
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_Corner_In.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_Corner_In.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If DirectionsOnSameSide(SideDirection, ResultDirection) Then
                            RoadCount(PainterRoad.Num) += 1
                        End If
                    End If
                Next
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_CrossIntersection.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_CrossIntersection.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RoadCount(PainterRoad.Num) += 1
                    End If
                Next
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_End.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_End.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If IdenticalTileDirections(SideDirection, ResultDirection) Then
                            RoadCount(PainterRoad.Num) += 1
                        End If
                    End If
                Next
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_Straight.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_Straight.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If DirectionsAreInLine(SideDirection, ResultDirection) Then
                            RoadCount(PainterRoad.Num) += 1
                        End If
                    End If
                Next
                For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_TIntersection.TileCount - 1
                    PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_TIntersection.Tiles(A)
                    If PainterTexture.TextureNum = Texture.TextureNum Then
                        RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                        If Not DirectionsOnSameSide(SideDirection, ResultDirection) Then
                            RoadCount(PainterRoad.Num) += 1
                        End If
                    End If
                Next
            Next
        End Sub

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            Painter = Map.Painter
            ReDim RoadCount(Painter.RoadCount - 1)
        End Sub
    End Class

    Public Class clsApplySideHTerrainInterpret
        Inherits clsMap.clsApplySideTerrainInterpret

        Public Overrides Sub ActionPerform()
            MyBase.ActionPerform()

            Dim A As Integer

            If PosNum.Y > 0 Then
                SideDirection = TileDirection_Bottom
                Tile = Terrain.Tiles(PosNum.X, PosNum.Y - 1)
                Texture = Tile.Texture
                ToolPerformTile()
            End If
            If PosNum.Y < Terrain.TileSize.Y Then
                SideDirection = TileDirection_Top
                Tile = Terrain.Tiles(PosNum.X, PosNum.Y)
                Texture = Tile.Texture
                ToolPerformTile()
            End If

            BestNum = -1
            BestCount = 0
            For A = 0 To Painter.RoadCount - 1
                If RoadCount(A) > BestCount Then
                    BestNum = A
                    BestCount = RoadCount(A)
                End If
            Next
            If BestCount > 0 Then
                Terrain.SideH(PosNum.X, PosNum.Y).Road = Painter.Roads(BestNum)
            Else
                Terrain.SideH(PosNum.X, PosNum.Y).Road = Nothing
            End If

            Map.SectorTerrainUndoChanges.SideHChanged(PosNum)
        End Sub
    End Class

    Public Class clsApplySideVTerrainInterpret
        Inherits clsMap.clsApplySideTerrainInterpret

        Public Overrides Sub ActionPerform()
            MyBase.ActionPerform()

            Dim A As Integer

            If PosNum.X > 0 Then
                SideDirection = TileDirection_Right
                Tile = Terrain.Tiles(PosNum.X - 1, PosNum.Y)
                Texture = Tile.Texture
                ToolPerformTile()
            End If
            If PosNum.X < Terrain.TileSize.X Then
                SideDirection = TileDirection_Left
                Tile = Terrain.Tiles(PosNum.X, PosNum.Y)
                Texture = Tile.Texture
                ToolPerformTile()
            End If

            BestNum = -1
            BestCount = 0
            For A = 0 To Painter.RoadCount - 1
                If RoadCount(A) > BestCount Then
                    BestNum = A
                    BestCount = RoadCount(A)
                End If
            Next
            If BestCount > 0 Then
                Terrain.SideV(PosNum.X, PosNum.Y).Road = Painter.Roads(BestNum)
            Else
                Terrain.SideV(PosNum.X, PosNum.Y).Road = Nothing
            End If

            Map.SectorTerrainUndoChanges.SideVChanged(PosNum)
        End Sub
    End Class

    Public Class clsApplyAutoTri
        Inherits clsAction

        Private difA As Double
        Private difB As Double
        Private NewTri As Boolean

        Public Overrides Sub ActionPerform()

            'tri set to the direction where the diagonal edge will be the flattest, so that cliff edges are level
            difA = Math.Abs(CDbl(Map.Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) - Map.Terrain.Vertices(PosNum.X, PosNum.Y).Height)
            difB = Math.Abs(CDbl(Map.Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) - Map.Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height)
            If difA = difB Then
                NewTri = (Rnd() < 0.5F)
            Else
                NewTri = (difA > difB)
            End If
            Map.Terrain.Tiles(PosNum.X, PosNum.Y).Tri = NewTri

            Map.SectorGraphicsChanges.TileChanged(PosNum)
            Map.SectorUnitHeightsChanges.TileChanged(PosNum)
            Map.SectorTerrainUndoChanges.TileChanged(PosNum)
        End Sub
    End Class

    Public Class clsObjectRotation
        Inherits clsObjectAction

        Public Angle As Integer

        Protected Overrides Sub _ActionPerform()

            ResultUnit.Rotation = Angle
        End Sub
    End Class

    Public Class clsObjectUnitGroup
        Inherits clsObjectAction

        Public UnitGroup As clsUnitGroup

        Protected Overrides Sub _ActionPerform()

            ResultUnit.UnitGroup = UnitGroup
        End Sub
    End Class

    Public Class clsObjectPriority
        Inherits clsObjectAction

        Public Priority As Integer

        Protected Overrides Sub _ActionPerform()

            ResultUnit.SavePriority = Priority
        End Sub
    End Class

    Public Class clsObjectHealth
        Inherits clsObjectAction

        Public Health As Double

        Protected Overrides Sub _ActionPerform()

            ResultUnit.Health = Health
        End Sub
    End Class

    Public Class clsObjectPosOffset
        Inherits clsObjectAction

        Public Offset As sXY_int

        Private NewPos As sXY_int

        Protected Overrides Sub _ActionPerform()

            NewPos.X = Unit.Pos.Horizontal.X + Offset.X
            NewPos.Y = Unit.Pos.Horizontal.Y + Offset.Y
            ResultUnit.Pos = Map.TileAlignedPosFromMapPos(NewPos, ResultUnit.Type.GetFootprintSelected(Unit.Rotation))
        End Sub
    End Class

    Public Class clsObjectRotationOffset
        Inherits clsObjectAction

        Public Offset As Integer

        Private NewPos As sXY_int

        Protected Overrides Sub _ActionPerform()

            ResultUnit.Rotation = Unit.Rotation + Offset
            If ResultUnit.Rotation < 0 Then
                ResultUnit.Rotation += 360
            ElseIf ResultUnit.Rotation >= 360 Then
                ResultUnit.Rotation -= 360
            End If
        End Sub
    End Class

    Public Class clsObjectTemplateToDesign
        Inherits clsObjectAction

        Private OldDroidType As clsDroidDesign
        Private NewDroidType As clsDroidDesign

        Protected Overrides Sub ActionCondition()
            MyBase.ActionCondition()

            If Unit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(Unit.Type, clsDroidDesign)
                ActionPerformed = OldDroidType.IsTemplate
            Else
                OldDroidType = Nothing
                ActionPerformed = False
            End If
        End Sub

        Protected Overrides Sub _ActionPerform()

            NewDroidType = New clsDroidDesign
            ResultUnit.Type = NewDroidType
            NewDroidType.CopyDesign(OldDroidType)
            NewDroidType.UpdateAttachments()
        End Sub
    End Class

    Public MustInherit Class clsObjectComponent
        Inherits clsObjectAction

        Private OldDroidType As clsDroidDesign
        Protected NewDroidType As clsDroidDesign

        Protected MustOverride Sub ChangeComponent()

        Protected Overrides Sub ActionCondition()
            MyBase.ActionCondition()

            If Unit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(Unit.Type, clsDroidDesign)
                ActionPerformed = Not OldDroidType.IsTemplate
            Else
                OldDroidType = Nothing
                ActionPerformed = False
            End If
        End Sub

        Protected Overrides Sub _ActionPerform()

            NewDroidType = New clsDroidDesign
            ResultUnit.Type = NewDroidType
            NewDroidType.CopyDesign(OldDroidType)

            ChangeComponent()

            NewDroidType.UpdateAttachments()
        End Sub
    End Class

    Public Class clsObjectAlignment
        Inherits clsObjectAction

        Protected Overrides Sub _ActionPerform()

            ResultUnit.Pos = Unit.MapLink.Source.TileAlignedPosFromMapPos(Unit.Pos.Horizontal, Unit.Type.GetFootprintNew(Unit.Rotation))
        End Sub
    End Class

    Public Class clsObjectBody
        Inherits clsObjectComponent

        Public Body As clsBody

        Protected Overrides Sub ChangeComponent()

            NewDroidType.Body = Body
        End Sub
    End Class

    Public Class clsObjectPropulsion
        Inherits clsObjectComponent

        Public Propulsion As clsPropulsion

        Protected Overrides Sub ChangeComponent()

            NewDroidType.Propulsion = Propulsion
        End Sub
    End Class

    Public Class clsObjectTurret
        Inherits clsObjectComponent

        Public Turret As clsTurret
        Public TurretNum As Integer

        Protected Overrides Sub ChangeComponent()

            Select Case TurretNum
                Case 0
                    NewDroidType.Turret1 = Turret
                Case 1
                    NewDroidType.Turret2 = Turret
                Case 2
                    NewDroidType.Turret3 = Turret
            End Select
        End Sub
    End Class

    Public Class clsObjectTurretCount
        Inherits clsObjectComponent

        Public TurretCount As Byte

        Protected Overrides Sub ChangeComponent()

            NewDroidType.TurretCount = TurretCount
        End Sub
    End Class

    Public Class clsObjectDroidType
        Inherits clsObjectComponent

        Public DroidType As clsDroidDesign.clsTemplateDroidType

        Protected Overrides Sub ChangeComponent()

            NewDroidType.TemplateDroidType = DroidType
        End Sub
    End Class

    Public Class clsObjectSelect
        Implements SimpleListTool(Of clsUnit)

        Private Unit As clsMap.clsUnit

        Public Sub ActionPerform() Implements SimpleListTool(Of clsUnit).ActionPerform

            Unit.MapSelect()
        End Sub

        Public Sub SetItem(Item As clsUnit) Implements SimpleListTool(Of clsUnit).SetItem

            Unit = Item
        End Sub
    End Class

    Public Class clsObjectPriorityOrderList
        Implements SimpleListTool(Of clsMap.clsUnit)

        Private _Result As New SimpleClassList(Of clsMap.clsUnit)
        Public ReadOnly Property Result As SimpleClassList(Of clsMap.clsUnit)
            Get
                Return _Result
            End Get
        End Property

        Private Unit As clsMap.clsUnit

        Public Sub New()

            _Result.MaintainOrder = True
        End Sub

        Public Sub ActionPerform() Implements SimpleListTool(Of clsMap.clsUnit).ActionPerform
            Dim A As Integer

            For A = 0 To _Result.Count - 1
                If Unit.SavePriority > _Result.Item(A).SavePriority Then
                    Exit For
                End If
            Next
            _Result.Insert(Unit, A)
        End Sub

        Public Sub SetItem(Item As clsUnit) Implements SimpleListTool(Of clsUnit).SetItem

            Unit = Item
        End Sub
    End Class

    Public Class clsObjectFlattenTerrain
        Implements SimpleListTool(Of clsUnit)

        Private Unit As clsMap.clsUnit

        Public Sub ActionPerform() Implements SimpleListTool(Of clsUnit).ActionPerform
            Dim Map As clsMap = Unit.MapLink.Source
            Dim VertexPos As sXY_int
            Dim X As Integer
            Dim Y As Integer
            Dim Total As Double
            Dim Average As Byte
            Dim Footprint As sXY_int = Unit.Type.GetFootprintSelected(Unit.Rotation)
            Dim Start As sXY_int
            Dim Finish As sXY_int
            Dim Samples As Integer

            Map.GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, Start, Finish)

            For Y = Start.Y To Finish.Y + 1
                VertexPos.Y = Y
                For X = Start.X To Finish.X + 1
                    VertexPos.X = X

                    Total += Map.Terrain.Vertices(VertexPos.X, VertexPos.Y).Height
                    Samples += 1
                Next
            Next

            If Samples >= 1 Then
                Average = CByte(Clamp_int(CInt(Total / Samples), Byte.MinValue, Byte.MaxValue))
                For Y = Start.Y To Finish.Y + 1
                    VertexPos.Y = Y
                    For X = Start.X To Finish.X + 1
                        VertexPos.X = X

                        Map.Terrain.Vertices(VertexPos.X, VertexPos.Y).Height = Average
                        Map.SectorGraphicsChanges.VertexAndNormalsChanged(VertexPos)
                        Map.SectorUnitHeightsChanges.VertexChanged(VertexPos)
                        Map.SectorTerrainUndoChanges.VertexChanged(VertexPos)
                    Next
                Next
            End If
        End Sub

        Public Sub SetItem(Item As clsUnit) Implements SimpleListTool(Of clsUnit).SetItem

            Unit = Item
        End Sub
    End Class

    Public Class clsStructureWriteWZ
        Implements SimpleListTool(Of clsMap.clsUnit)

        Public File As IO.BinaryWriter
        Public CompileType As clsMap.sWrite_WZ_Args.enumCompileType
        Public PlayerCount As Integer

        Private Unit As clsMap.clsUnit

        Private StructureType As clsStructureType
        Private StruZeroBytesA(11) As Byte
        Private StruZeroBytesB(39) As Byte

        Public Sub ActionPerform() Implements SimpleListTool(Of clsUnit).ActionPerform

            If CompileType = sWrite_WZ_Args.enumCompileType.Unspecified Then
                Stop
                Exit Sub
            End If

            StructureType = CType(Unit.Type, clsStructureType)
            WriteTextOfLength(File, 40, StructureType.Code)
            File.Write(Unit.ID)
            File.Write(CUInt(Unit.Pos.Horizontal.X))
            File.Write(CUInt(Unit.Pos.Horizontal.Y))
            File.Write(CUInt(Unit.Pos.Altitude))
            File.Write(CUInt(Unit.Rotation))
            Select Case CompileType
                Case sWrite_WZ_Args.enumCompileType.Multiplayer
                    File.Write(Unit.GetBJOMultiplayerPlayerNum(PlayerCount))
                Case sWrite_WZ_Args.enumCompileType.Campaign
                    File.Write(Unit.GetBJOCampaignPlayerNum)
                Case Else
                    Stop
            End Select
            File.Write(StruZeroBytesA)
            File.Write(CByte(1))
            File.Write(CByte(26))
            File.Write(CByte(127))
            File.Write(CByte(0))
            File.Write(StruZeroBytesB)
        End Sub

        Public Sub SetItem(Item As clsUnit) Implements SimpleListTool(Of clsUnit).SetItem

            Unit = Item
        End Sub
    End Class
End Class
