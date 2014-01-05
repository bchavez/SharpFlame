
Partial Public Class frmGenerator

    Private _Owner As frmMain

    Private PlayerCount As Integer = 4
    Private StopTrying As Boolean

    Private Function ValidateTextbox(TextBoxToValidate As TextBox, Min As Double, Max As Double, Multiplier As Double) As Integer
        Dim dblTemp As Double
        Dim Result As Integer

        If Not InvariantParse_dbl(TextBoxToValidate.Text, dblTemp) Then
            Return 0
        End If
        Result = CInt(Int(Clamp_dbl(dblTemp, Min, Max) * Multiplier))
        TextBoxToValidate.Text = InvariantToString_sng(CSng(Result / Multiplier))
        Return Result
    End Function

    Private Generator As New clsGenerateMap

    Private Sub btnGenerateLayout_Click(sender As System.Object, e As System.EventArgs) Handles btnGenerateLayout.Click

        lstResult.Items.Clear()
        btnGenerateLayout.Enabled = False
        lstResult_AddText("Generating layout.")
        Application.DoEvents()

        StopTrying = False

        Dim LoopCount As Integer

        Generator.ClearLayout()

        Generator.GenerateTileset = Nothing
        Generator.Map = Nothing

        Generator.TopLeftPlayerCount = PlayerCount

        Select Case cboSymmetry.SelectedIndex
            Case 0 'none
                Generator.SymmetryBlockCountXY.X = 1
                Generator.SymmetryBlockCountXY.Y = 1
                Generator.SymmetryBlockCount = 1
                ReDim Generator.SymmetryBlocks(Generator.SymmetryBlockCount - 1)
                Generator.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                Generator.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                Generator.SymmetryIsRotational = False
            Case 1 'h rotation
                Generator.SymmetryBlockCountXY.X = 2
                Generator.SymmetryBlockCountXY.Y = 1
                Generator.SymmetryBlockCount = 2
                ReDim Generator.SymmetryBlocks(Generator.SymmetryBlockCount - 1)
                Generator.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                Generator.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim Generator.SymmetryBlocks(0).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(0).ReflectToNum(0) = 1
                Generator.SymmetryBlocks(1).XYNum = New sXY_int(1, 0)
                Generator.SymmetryBlocks(1).Orientation = New sTileOrientation(True, True, False)
                ReDim Generator.SymmetryBlocks(1).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(1).ReflectToNum(0) = 0
                Generator.SymmetryIsRotational = True
            Case 2 'v rotation
                Generator.SymmetryBlockCountXY.X = 1
                Generator.SymmetryBlockCountXY.Y = 2
                Generator.SymmetryBlockCount = 2
                ReDim Generator.SymmetryBlocks(Generator.SymmetryBlockCount - 1)
                Generator.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                Generator.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim Generator.SymmetryBlocks(0).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(0).ReflectToNum(0) = 1
                Generator.SymmetryBlocks(1).XYNum = New sXY_int(0, 1)
                Generator.SymmetryBlocks(1).Orientation = New sTileOrientation(True, True, False)
                ReDim Generator.SymmetryBlocks(1).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(1).ReflectToNum(0) = 0
                Generator.SymmetryIsRotational = True
            Case 3 'h flip
                Generator.SymmetryBlockCountXY.X = 2
                Generator.SymmetryBlockCountXY.Y = 1
                Generator.SymmetryBlockCount = 2
                ReDim Generator.SymmetryBlocks(Generator.SymmetryBlockCount - 1)
                Generator.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                Generator.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim Generator.SymmetryBlocks(0).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(0).ReflectToNum(0) = 1
                Generator.SymmetryBlocks(1).XYNum = New sXY_int(1, 0)
                Generator.SymmetryBlocks(1).Orientation = New sTileOrientation(True, False, False)
                ReDim Generator.SymmetryBlocks(1).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(1).ReflectToNum(0) = 0
                Generator.SymmetryIsRotational = False
            Case 4 'v flip
                Generator.SymmetryBlockCountXY.X = 1
                Generator.SymmetryBlockCountXY.Y = 2
                Generator.SymmetryBlockCount = 2
                ReDim Generator.SymmetryBlocks(Generator.SymmetryBlockCount - 1)
                Generator.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                Generator.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim Generator.SymmetryBlocks(0).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(0).ReflectToNum(0) = 1
                Generator.SymmetryBlocks(1).XYNum = New sXY_int(0, 1)
                Generator.SymmetryBlocks(1).Orientation = New sTileOrientation(False, True, False)
                ReDim Generator.SymmetryBlocks(1).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(1).ReflectToNum(0) = 0
                Generator.SymmetryIsRotational = False
            Case 5 '4x rotation
                Generator.SymmetryBlockCountXY.X = 2
                Generator.SymmetryBlockCountXY.Y = 2
                Generator.SymmetryBlockCount = 4
                ReDim Generator.SymmetryBlocks(Generator.SymmetryBlockCount - 1)
                Generator.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                Generator.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim Generator.SymmetryBlocks(0).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(0).ReflectToNum(0) = 1
                Generator.SymmetryBlocks(0).ReflectToNum(1) = 2
                Generator.SymmetryBlocks(1).XYNum = New sXY_int(1, 0)
                Generator.SymmetryBlocks(1).Orientation = New sTileOrientation(True, False, True)
                ReDim Generator.SymmetryBlocks(1).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(1).ReflectToNum(0) = 3
                Generator.SymmetryBlocks(1).ReflectToNum(1) = 0
                Generator.SymmetryBlocks(2).XYNum = New sXY_int(0, 1)
                Generator.SymmetryBlocks(2).Orientation = New sTileOrientation(False, True, True)
                ReDim Generator.SymmetryBlocks(2).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(2).ReflectToNum(0) = 0
                Generator.SymmetryBlocks(2).ReflectToNum(1) = 3
                Generator.SymmetryBlocks(3).XYNum = New sXY_int(1, 1)
                Generator.SymmetryBlocks(3).Orientation = New sTileOrientation(True, True, False)
                ReDim Generator.SymmetryBlocks(3).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(3).ReflectToNum(0) = 2
                Generator.SymmetryBlocks(3).ReflectToNum(1) = 1
                Generator.SymmetryIsRotational = True
            Case 6 'hv flip
                Generator.SymmetryBlockCountXY.X = 2
                Generator.SymmetryBlockCountXY.Y = 2
                Generator.SymmetryBlockCount = 4
                ReDim Generator.SymmetryBlocks(Generator.SymmetryBlockCount - 1)
                Generator.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                Generator.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim Generator.SymmetryBlocks(0).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(0).ReflectToNum(0) = 1
                Generator.SymmetryBlocks(0).ReflectToNum(1) = 2
                Generator.SymmetryBlocks(1).XYNum = New sXY_int(1, 0)
                Generator.SymmetryBlocks(1).Orientation = New sTileOrientation(True, False, False)
                ReDim Generator.SymmetryBlocks(1).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(1).ReflectToNum(0) = 0
                Generator.SymmetryBlocks(1).ReflectToNum(1) = 3
                Generator.SymmetryBlocks(2).XYNum = New sXY_int(0, 1)
                Generator.SymmetryBlocks(2).Orientation = New sTileOrientation(False, True, False)
                ReDim Generator.SymmetryBlocks(2).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(2).ReflectToNum(0) = 3
                Generator.SymmetryBlocks(2).ReflectToNum(1) = 0
                Generator.SymmetryBlocks(3).XYNum = New sXY_int(1, 1)
                Generator.SymmetryBlocks(3).Orientation = New sTileOrientation(True, True, False)
                ReDim Generator.SymmetryBlocks(3).ReflectToNum(CInt(Generator.SymmetryBlockCount / 2.0#) - 1)
                Generator.SymmetryBlocks(3).ReflectToNum(0) = 2
                Generator.SymmetryBlocks(3).ReflectToNum(1) = 1
                Generator.SymmetryIsRotational = False
            Case Else
                MsgBox("Select symmetry")
                btnGenerateLayout.Enabled = True
                Exit Sub
        End Select

        If Generator.TopLeftPlayerCount * Generator.SymmetryBlockCount < 2 Then
            MsgBox("That configuration only produces 1 player.")
            btnGenerateLayout.Enabled = True
            Exit Sub
        End If
        If Generator.TopLeftPlayerCount * Generator.SymmetryBlockCount > 10 Then
            MsgBox("That configuration produces more than 10 players.")
            btnGenerateLayout.Enabled = True
            Exit Sub
        End If

        Generator.TileSize.X = ValidateTextbox(txtWidth, 48.0#, 250.0#, 1.0#)
        Generator.TileSize.Y = ValidateTextbox(txtHeight, 48.0#, 250.0#, 1.0#)
        If Generator.SymmetryBlockCount = 4 Then
            If Generator.TileSize.X <> Generator.TileSize.Y And Generator.SymmetryIsRotational Then
                MsgBox("Width and height must be equal if map is rotated on two axes.")
                btnGenerateLayout.Enabled = True
                Exit Sub
            End If
        End If
        ReDim Generator.PlayerBasePos(Generator.TopLeftPlayerCount - 1)
        Dim BaseMin As Double = 12.0#
        Dim BaseMax As Matrix3D.XY_dbl = New Matrix3D.XY_dbl(Math.Min(Generator.TileSize.X / Generator.SymmetryBlockCountXY.X, Generator.TileSize.X - 12.0#), Math.Min(Generator.TileSize.Y / Generator.SymmetryBlockCountXY.Y, Generator.TileSize.Y - 12.0#))
        Generator.PlayerBasePos(0) = New sXY_int(ValidateTextbox(txt1x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt1y, BaseMin, BaseMax.X, TerrainGridSpacing))
        If Generator.TopLeftPlayerCount >= 2 Then
            Generator.PlayerBasePos(1) = New sXY_int(ValidateTextbox(txt2x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt2y, BaseMin, BaseMax.Y, TerrainGridSpacing))
            If Generator.TopLeftPlayerCount >= 3 Then
                Generator.PlayerBasePos(2) = New sXY_int(ValidateTextbox(txt3x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt3y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                If Generator.TopLeftPlayerCount >= 4 Then
                    Generator.PlayerBasePos(3) = New sXY_int(ValidateTextbox(txt4x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt4y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                    If Generator.TopLeftPlayerCount >= 5 Then
                        Generator.PlayerBasePos(4) = New sXY_int(ValidateTextbox(txt5x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt5y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                        If Generator.TopLeftPlayerCount >= 6 Then
                            Generator.PlayerBasePos(5) = New sXY_int(ValidateTextbox(txt6x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt6y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                            If Generator.TopLeftPlayerCount >= 7 Then
                                Generator.PlayerBasePos(6) = New sXY_int(ValidateTextbox(txt7x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt7y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                                If Generator.TopLeftPlayerCount >= 8 Then
                                    Generator.PlayerBasePos(7) = New sXY_int(ValidateTextbox(txt8x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt8y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                                    If Generator.TopLeftPlayerCount >= 9 Then
                                        Generator.PlayerBasePos(8) = New sXY_int(ValidateTextbox(txt9x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt9y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                                        If Generator.TopLeftPlayerCount >= 10 Then
                                            Generator.PlayerBasePos(9) = New sXY_int(ValidateTextbox(txt10x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt10y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Generator.LevelCount = ValidateTextbox(txtLevels, 3.0#, 5.0#, 1.0#)
        Generator.BaseLevel = ValidateTextbox(txtBaseLevel, -1.0#, CDbl(Generator.LevelCount - 1), 1.0#)
        Generator.JitterScale = 1
        Generator.MaxLevelTransition = 2
        Generator.PassagesChance = ValidateTextbox(txtLevelFrequency, 0.0#, 100.0#, 1.0#)
        Generator.VariationChance = ValidateTextbox(txtVariation, 0.0#, 100.0#, 1.0#)
        Generator.FlatsChance = ValidateTextbox(txtFlatness, 0.0#, 100.0#, 1.0#)
        Generator.BaseFlatArea = ValidateTextbox(txtBaseArea, 1.0#, 16.0#, 1.0#)
        Generator.NodeScale = 4.0F
        Generator.WaterSpawnQuantity = ValidateTextbox(txtWaterQuantity, 0.0#, 9999.0#, 1.0#)
        Generator.TotalWaterQuantity = ValidateTextbox(txtConnectedWater, 0.0#, 9999.0#, 1.0#)

        Application.DoEvents()
        LoopCount = 0
        Dim Result As clsResult
        Do
            Result = New clsResult("")
            Result = Generator.GenerateLayout
            If Not Result.HasProblems Then
                Dim HeightsResult As clsResult = FinishHeights()
                Result.Add(HeightsResult)
                If Not HeightsResult.HasProblems Then
                    lstResult_AddResult(Result)
                    lstResult_AddText("Done.")
                    btnGenerateLayout.Enabled = True
                    Exit Do
                End If
            End If
            LoopCount += 1
            lstResult_AddText("Attempt " & LoopCount & " failed.")
            Application.DoEvents()
            If StopTrying Then
                Generator.Map = Nothing
                lstResult_AddResult(Result)
                lstResult_AddText("Stopped.")
                btnGenerateLayout.Enabled = True
                Exit Sub
            End If
            lstResult_AddResult(Result)
            lstResult_AddText("Retrying...")
            Application.DoEvents()
            Generator.ClearLayout()
        Loop
        lstResult_AddResult(Result)
    End Sub

    Private Function FinishHeights() As clsResult
        Dim ReturnResult As New clsResult("")

        ReturnResult.Take(Generator.GenerateLayoutTerrain())
        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        Generator.Map.RandomizeHeights(Generator.LevelCount)

        Generator.Map.InterfaceOptions = New clsMap.clsInterfaceOptions
        Generator.Map.InterfaceOptions.CompileMultiPlayers = InvariantToString_int(Generator.GetTotalPlayerCount)

        _Owner.NewMainMap(Generator.Map)

        Return ReturnResult
    End Function

    Private Function FinishTextures() As clsResult
        Dim ReturnResult As New clsResult("")

        If cbxMasterTexture.Checked Then
            Select Case cboTileset.SelectedIndex
                Case 0
                    Generator.GenerateTileset = Generator_TilesetArizona
                    TerrainStyle_Arizona.Watermap = Generator.GetWaterMap
                    TerrainStyle_Arizona.LevelCount = Generator.LevelCount
                    Generator.Map.GenerateMasterTerrain(TerrainStyle_Arizona)
                    TerrainStyle_Arizona.Watermap = Nothing
                Case 1
                    Generator.GenerateTileset = Generator_TilesetUrban
                    TerrainStyle_Urban.Watermap = Generator.GetWaterMap
                    TerrainStyle_Urban.LevelCount = Generator.LevelCount
                    Generator.Map.GenerateMasterTerrain(TerrainStyle_Urban)
                    TerrainStyle_Urban.Watermap = Nothing
                Case 2
                    Generator.GenerateTileset = Generator_TilesetRockies
                    TerrainStyle_Rockies.Watermap = Generator.GetWaterMap
                    TerrainStyle_Rockies.LevelCount = Generator.LevelCount
                    Generator.Map.GenerateMasterTerrain(TerrainStyle_Rockies)
                    TerrainStyle_Rockies.Watermap = Nothing
                Case Else
                    ReturnResult.ProblemAdd("Error: bad tileset selection.")
                    btnGenerateLayout.Enabled = True
                    Return ReturnResult
            End Select
            Generator.Map.TileType_Reset()
            Generator.Map.SetPainterToDefaults()
        Else
            Select Case cboTileset.SelectedIndex
                Case 0
                    Generator.Map.Tileset = Tileset_Arizona
                    Generator.GenerateTileset = Generator_TilesetArizona
                Case 1
                    Generator.Map.Tileset = Tileset_Urban
                    Generator.GenerateTileset = Generator_TilesetUrban
                Case 2
                    Generator.Map.Tileset = Tileset_Rockies
                    Generator.GenerateTileset = Generator_TilesetRockies
                Case Else
                    ReturnResult.ProblemAdd("Error: bad tileset selection.")
                    btnGenerateLayout.Enabled = True
                    Return ReturnResult
            End Select
            Generator.Map.TileType_Reset()
            Generator.Map.SetPainterToDefaults()
            Dim CliffAngle As Double = Math.Atan(255.0# * Generator.Map.HeightMultiplier / (2.0# * (Generator.LevelCount - 1.0#) * TerrainGridSpacing)) - RadOf1Deg
            Dim tmpBrush As New clsBrush(CDbl(Math.Max(Generator.Map.Terrain.TileSize.X, Generator.Map.Terrain.TileSize.Y)) * 1.1#, clsBrush.enumShape.Square)
            Dim ApplyCliff As New clsMap.clsApplyCliff
            ApplyCliff.Map = Generator.Map
            ApplyCliff.Angle = CliffAngle
            ApplyCliff.SetTris = True
            Dim Alignments As clsBrush.sPosNum
            Alignments.Normal = New sXY_int(CInt(Int(Generator.Map.Terrain.TileSize.X / 2.0#)), CInt(Int(Generator.Map.Terrain.TileSize.Y / 2.0#)))
            Alignments.Alignment = Alignments.Normal
            tmpBrush.PerformActionMapTiles(ApplyCliff, Alignments)
            Dim RevertSlope() As Boolean
            Dim RevertHeight() As Boolean
            Dim WaterMap As New clsBooleanMap
            Dim bmTemp As New clsBooleanMap
            Dim A As Integer
            WaterMap = Generator.GetWaterMap
            With Generator.GenerateTileset
                ReDim RevertSlope(.OldTextureLayers.LayerCount - 1)
                ReDim RevertHeight(.OldTextureLayers.LayerCount - 1)
                For A = 0 To .OldTextureLayers.LayerCount - 1
                    With .OldTextureLayers.Layers(A)
                        .Terrainmap = Generator.Map.GenerateTerrainMap(.Scale, .Density)
                        If .SlopeMax < 0.0F Then
                            .SlopeMax = CSng(CliffAngle - RadOf1Deg)
                            If .HeightMax < 0.0F Then
                                .HeightMax = 255.0F
                                bmTemp.Within(.Terrainmap, WaterMap)
                                .Terrainmap.ValueData = bmTemp.ValueData
                                bmTemp.ValueData = New clsBooleanMap.clsValueData
                                RevertHeight(A) = True
                            End If
                            RevertSlope(A) = True
                        End If
                    End With
                Next
                Generator.Map.MapTexturer(.OldTextureLayers)
                For A = 0 To .OldTextureLayers.LayerCount - 1
                    With .OldTextureLayers.Layers(A)
                        .Terrainmap = Nothing
                        If RevertSlope(A) Then
                            .SlopeMax = -1.0F
                        End If
                        If RevertHeight(A) Then
                            .HeightMax = -1.0F
                        End If
                    End With
                Next
            End With
        End If

        Generator.Map.LevelWater()

        Generator.Map.WaterTriCorrection()

        Generator.Map.SectorGraphicsChanges.SetAllChanged()
        Generator.Map.SectorUnitHeightsChanges.SetAllChanged()

        Generator.Map.Update()

        Generator.Map.UndoStepCreate("Generated Textures")

        If Generator.Map Is _Owner.MainMap Then
            frmMainInstance.PainterTerrains_Refresh(-1, -1)
            frmMainInstance.MainMapTilesetChanged()
        End If

        Return ReturnResult
    End Function

    Private Sub btnGenerateObjects_Click(sender As System.Object, e As System.EventArgs) Handles btnGenerateObjects.Click

        If Generator.Map Is Nothing Or Generator.GenerateTileset Is Nothing Then
            Exit Sub
        End If
        If Not Generator.Map.frmMainLink.IsConnected Then
            Exit Sub
        End If

        Generator.BaseOilCount = ValidateTextbox(txtBaseOil, 0.0#, 16.0#, 1.0#)
        Generator.ExtraOilCount = ValidateTextbox(txtOilElsewhere, 0.0#, 9999.0#, 1.0#)
        Generator.ExtraOilClusterSizeMax = ValidateTextbox(txtOilClusterMax, 0.0#, 99.0#, 1.0#)
        Generator.ExtraOilClusterSizeMin = ValidateTextbox(txtOilClusterMin, 0.0#, CDbl(Generator.ExtraOilClusterSizeMax), 1.0#)
        Generator.OilDispersion = ValidateTextbox(txtOilDispersion, 0.0#, 9999.0#, 1.0#) / 100.0F
        Generator.OilAtATime = ValidateTextbox(txtOilAtATime, 1.0#, 2.0#, 1.0#)

        Generator.FeatureClusterChance = ValidateTextbox(txtFClusterChance, 0.0#, 100.0#, 1.0#) / 100.0F
        Generator.FeatureClusterMaxUnits = ValidateTextbox(txtFClusterMax, 0.0#, 99.0#, 1.0#)
        Generator.FeatureClusterMinUnits = ValidateTextbox(txtFClusterMin, 0.0#, Generator.FeatureClusterMaxUnits, 1.0#)
        Generator.FeatureScatterCount = ValidateTextbox(txtFScatterChance, 0.0#, 99999.0#, 1.0#)
        Generator.FeatureScatterGap = ValidateTextbox(txtFScatterGap, 0.0#, 99999.0#, 1.0#)
        Generator.BaseTruckCount = ValidateTextbox(txtTrucks, 0.0#, 15.0#, 1.0#)

        Generator.GenerateTilePathMap()

        Generator.TerrainBlockPaths()

        Generator.BlockEdgeTiles()

        Generator.GenerateGateways()

        lstResult_AddText("Generating objects.")
        Dim Result As New clsResult("")
        Result.Take(Generator.GenerateOil)
        Result.Take(Generator.GenerateUnits)
        lstResult_AddResult(Result)
        If Result.HasProblems Then
            lstResult_AddText("Failed.")
        Else
            lstResult_AddText("Done.")
        End If

        Generator.Map.SectorGraphicsChanges.SetAllChanged()
        Generator.Map.Update()
        Generator.Map.UndoStepCreate("Generator objects")
    End Sub

    Private Sub btnGenerateRamps_Click(sender As System.Object, e As System.EventArgs) Handles btnGenerateRamps.Click

        If Generator.Map Is Nothing Then
            Exit Sub
        End If

        Generator.MaxDisconnectionDist = ValidateTextbox(txtRampDistance, 0.0#, 99999.0#, TerrainGridSpacing)
        Generator.RampBase = ValidateTextbox(txtRampBase, 10.0#, 1000.0#, 10.0#) / 1000.0#

        Dim Result As New clsResult("")

        lstResult_AddText("Generating ramps.")
        Result = Generator.GenerateRamps()
        If Not Result.HasProblems Then
            Result.Add(FinishHeights)
        End If
        lstResult_AddResult(Result)
        If Result.HasProblems Then
            lstResult_AddText("Failed.")
            Exit Sub
        Else
            lstResult_AddText("Done.")
        End If
    End Sub

    Private Sub btnGenerateTextures_Click(sender As System.Object, e As System.EventArgs) Handles btnGenerateTextures.Click

        If Generator.Map Is Nothing Then
            Exit Sub
        End If
        If Not Generator.Map.frmMainLink.IsConnected Then
            Exit Sub
        End If

        lstResult_AddResult(FinishTextures)
        frmMainInstance.View_DrawViewLater()
    End Sub

    Private Sub frmGenerator_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Hide()
        e.Cancel = True
    End Sub

    Private Sub frmWZMapGen_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        cboTileset.SelectedIndex = 0
        cboSymmetry.SelectedIndex = 0
    End Sub

    Private Sub rdoPlayer2_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer2.CheckedChanged

        If rdoPlayer2.Checked Then
            PlayerCount = 2
            rdoPlayer1.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer3_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer3.CheckedChanged

        If rdoPlayer3.Checked Then
            PlayerCount = 3
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer4_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer4.CheckedChanged

        If rdoPlayer4.Checked Then
            PlayerCount = 4
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer5_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer5.CheckedChanged

        If rdoPlayer5.Checked Then
            PlayerCount = 5
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer6_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer6.CheckedChanged

        If rdoPlayer6.Checked Then
            PlayerCount = 6
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer7_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer7.CheckedChanged

        If rdoPlayer7.Checked Then
            PlayerCount = 7
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer8_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer8.CheckedChanged

        If rdoPlayer8.Checked Then
            PlayerCount = 8
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub btnStop_Click(sender As System.Object, e As System.EventArgs) Handles btnStop.Click

        StopTrying = True
    End Sub

    Private Sub rdoPlayer1_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer1.CheckedChanged

        If rdoPlayer1.Checked Then
            PlayerCount = 1
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer9_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer9.CheckedChanged

        If rdoPlayer9.Checked Then
            PlayerCount = 9
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer10_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoPlayer10.CheckedChanged

        If rdoPlayer10.Checked Then
            PlayerCount = 10
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
        End If
    End Sub

    Public Sub New(Owner As frmMain)
        InitializeComponent()

        _Owner = Owner
    End Sub

    Private Sub lstResult_AddResult(Result As clsResult)

        'todo

        'Dim A As Integer

        'For A = 0 To Result.Problems.Count - 1
        '    lstResult.Items.Add("Problem: " & Result.Problems.Item(A))
        'Next
        'For A = 0 To Result.Warnings.Count - 1
        '    lstResult.Items.Add("Warning: " & Result.Warnings.Item(A))
        'Next
        lstResult.SelectedIndex = lstResult.Items.Count - 1
    End Sub

    Private Sub lstResult_AddText(Text As String)

        lstResult.Items.Add(Text)
        lstResult.SelectedIndex = lstResult.Items.Count - 1
    End Sub
End Class
