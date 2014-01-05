
Public Class frmCompile

    Private Map As clsMap

    Public Shared Function Create(Map As clsMap) As frmCompile

        If Map Is Nothing Then
            Stop
            Return Nothing
        End If

        If Map.CompileScreen IsNot Nothing Then
            Stop
            Return Nothing
        End If

        Return New frmCompile(Map)
    End Function

    Private Sub New(Map As clsMap)

        InitializeComponent()

        Icon = ProgramIcon

        Me.Map = Map
        Map.CompileScreen = Me

        UpdateControls()
    End Sub

    Private Sub UpdateControls()

        txtName.Text = Map.InterfaceOptions.CompileName

        txtMultiPlayers.Text = Map.InterfaceOptions.CompileMultiPlayers
        cbxLevFormat.Checked = Map.InterfaceOptions.CompileMultiXPlayers
        txtAuthor.Text = Map.InterfaceOptions.CompileMultiAuthor
        cboLicense.Text = Map.InterfaceOptions.CompileMultiLicense

        cboCampType.SelectedIndex = Map.InterfaceOptions.CampaignGameType

        cbxAutoScrollLimits.Checked = Map.InterfaceOptions.AutoScrollLimits
        AutoScrollLimits_Update()
        txtScrollMinX.Text = InvariantToString_int(Map.InterfaceOptions.ScrollMin.X)
        txtScrollMinY.Text = InvariantToString_int(Map.InterfaceOptions.ScrollMin.Y)
        txtScrollMaxX.Text = InvariantToString_uint(Map.InterfaceOptions.ScrollMax.X)
        txtScrollMaxY.Text = InvariantToString_uint(Map.InterfaceOptions.ScrollMax.Y)
    End Sub

    Private Sub SaveToMap()

        Map.InterfaceOptions.CompileName = txtName.Text

        Map.InterfaceOptions.CompileMultiPlayers = txtMultiPlayers.Text
        Map.InterfaceOptions.CompileMultiXPlayers = cbxLevFormat.Checked
        Map.InterfaceOptions.CompileMultiAuthor = txtAuthor.Text
        Map.InterfaceOptions.CompileMultiLicense = cboLicense.Text

        Map.InterfaceOptions.CampaignGameType = cboCampType.SelectedIndex

        Dim Invalid As Boolean = False

        Try
            Map.InterfaceOptions.ScrollMin.X = CInt(txtScrollMinX.Text)
        Catch ex As Exception
            Invalid = True
            Map.InterfaceOptions.ScrollMin.X = 0
        End Try
        Try
            Map.InterfaceOptions.ScrollMin.Y = CInt(txtScrollMinY.Text)
        Catch ex As Exception
            Invalid = True
            Map.InterfaceOptions.ScrollMin.Y = 0
        End Try
        Try
            Map.InterfaceOptions.ScrollMax.X = CUInt(txtScrollMaxX.Text)
        Catch ex As Exception
            Invalid = True
            Map.InterfaceOptions.ScrollMax.X = 0
        End Try
        Try
            Map.InterfaceOptions.ScrollMax.Y = CUInt(txtScrollMaxY.Text)
        Catch ex As Exception
            Invalid = True
            Map.InterfaceOptions.ScrollMax.Y = 0
        End Try
        Map.InterfaceOptions.AutoScrollLimits = (cbxAutoScrollLimits.Checked Or Invalid)

        Map.SetChanged()

        UpdateControls() 'display to show any changes
    End Sub

    Private Sub btnCompile_Click(sender As System.Object, e As System.EventArgs) Handles btnCompileMultiplayer.Click
        Dim ReturnResult As New clsResult("Compile multiplayer")
        Dim A As Integer

        SaveToMap()

        Dim MapName As String
        Dim License As String = cboLicense.Text
        Dim PlayerCount As Integer
        If Not InvariantParse_int(txtMultiPlayers.Text, PlayerCount) Then
            PlayerCount = 0
        End If
        Dim IsXPlayerFormat As Boolean = cbxLevFormat.Checked
        If PlayerCount < 2 Or PlayerCount > 10 Then
            ReturnResult.ProblemAdd("The number of players must be from 2 to " & PlayerCountMax)
        End If
        If Not IsXPlayerFormat Then
            If PlayerCount <> 2 And PlayerCount <> 4 And PlayerCount <> 8 Then
                ReturnResult.ProblemAdd("You must enable support for this number of players.")
            End If
        End If

        A = ValidateMap_WaterTris()
        If A > 0 Then
            ReturnResult.WarningAdd(A & " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.")
        End If

        ReturnResult.Add(ValidateMap())
        ReturnResult.Add(ValidateMap_UnitPositions())
        ReturnResult.Add(ValidateMap_Multiplayer(PlayerCount, IsXPlayerFormat))

        MapName = txtName.Text
        Dim CurrentChar As Char
        For A = 0 To MapName.Length - 1
            CurrentChar = MapName(A)
            If Not ((CurrentChar >= "a"c And CurrentChar <= "z"c) Or (CurrentChar >= "A"c And CurrentChar <= "Z"c) Or (A >= 1 And ((CurrentChar >= "0"c And CurrentChar <= "9"c) Or CurrentChar = "-"c Or CurrentChar = "_"c))) Then
                Exit For
            End If
        Next
        If A < MapName.Length Then
            ReturnResult.ProblemAdd("The map's name must contain only letters, numbers, underscores and hyphens, and must begin with a letter.")
        End If
        If MapName.Length < 1 Or MapName.Length > 16 Then
            ReturnResult.ProblemAdd("Map name must be from 1 to 16 characters.")
        End If
        If License = "" Then
            ReturnResult.ProblemAdd("Enter a valid license.")
        End If
        If ReturnResult.HasProblems Then
            ShowWarnings(ReturnResult)
            Exit Sub
        End If
        Dim CompileMultiDialog As New SaveFileDialog
        If Map.PathInfo IsNot Nothing Then
            CompileMultiDialog.InitialDirectory = Map.PathInfo.Path
        End If
        CompileMultiDialog.FileName = PlayerCount & "c-" & MapName
        CompileMultiDialog.Filter = "WZ Files (*.wz)|*.wz"
        If CompileMultiDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim WriteWZArgs As New clsMap.sWrite_WZ_Args
        WriteWZArgs.MapName = MapName
        WriteWZArgs.Path = CompileMultiDialog.FileName
        WriteWZArgs.Overwrite = True
        SetScrollLimits(WriteWZArgs.ScrollMin, WriteWZArgs.ScrollMax)
        WriteWZArgs.Multiplayer = New clsMap.sWrite_WZ_Args.clsMultiplayer
        WriteWZArgs.Multiplayer.AuthorName = txtAuthor.Text
        WriteWZArgs.Multiplayer.PlayerCount = PlayerCount
        WriteWZArgs.Multiplayer.IsBetaPlayerFormat = IsXPlayerFormat
        WriteWZArgs.Multiplayer.License = License
        WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Multiplayer
        ReturnResult.Add(Map.Write_WZ(WriteWZArgs))
        ShowWarnings(ReturnResult)
        If Not ReturnResult.HasWarnings Then
            Close()
        End If
    End Sub

    Private Sub frmCompile_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        Map.CompileScreen = Nothing
        Map = Nothing
    End Sub

    Private Sub frmCompile_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        SaveToMap()
    End Sub

    Private Function ValidateMap_UnitPositions() As clsResult
        Dim Result As New clsResult("Validate unit positions")

        'check unit positions

        Dim TileHasUnit(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1) As Boolean
        Dim TileStructureType(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1) As clsStructureType
        Dim TileFeatureType(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1) As clsFeatureType
        Dim TileObjectGroup(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1) As clsMap.clsUnitGroup
        Dim X As Integer
        Dim Y As Integer
        Dim StartPos As sXY_int
        Dim FinishPos As sXY_int
        Dim CentrePos As sXY_int
        Dim StructureTypeType As clsStructureType.enumStructureType
        Dim StructureType As clsStructureType
        Dim Footprint As sXY_int
        Dim UnitIsStructureModule(Map.Units.Count - 1) As Boolean
        Dim IsValid As Boolean
        Dim Unit As clsMap.clsUnit
        For Each Unit In Map.Units
            If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                StructureType = CType(Unit.Type, clsStructureType)
                StructureTypeType = StructureType.StructureType
                UnitIsStructureModule(Unit.MapLink.ArrayPosition) = (StructureType.IsModule Or StructureTypeType = clsStructureType.enumStructureType.ResourceExtractor)
            End If
        Next
        'check and store non-module units first. modules need to check for the underlying unit.
        For Each Unit In Map.Units
            If Not UnitIsStructureModule(Unit.MapLink.ArrayPosition) Then
                Footprint = Unit.Type.GetFootprintSelected(Unit.Rotation)
                Map.GetFootprintTileRange(Unit.Pos.Horizontal, Footprint, StartPos, FinishPos)
                If StartPos.X < 0 Or FinishPos.X >= Map.Terrain.TileSize.X _
                  Or StartPos.Y < 0 Or FinishPos.Y >= Map.Terrain.TileSize.Y Then
                    Dim resultItem As clsResultProblemGoto(Of clsResultItemPosGoto) = CreateResultProblemGotoForObject(Unit)
                    resultItem.Text = "Unit off map at position " & Unit.GetPosText & "."
                    Result.ItemAdd(resultItem)
                Else
                    For Y = StartPos.Y To FinishPos.Y
                        For X = StartPos.X To FinishPos.X
                            If TileHasUnit(X, Y) Then
                                Dim resultItem As clsResultProblemGoto(Of clsResultItemPosGoto) = CreateResultProblemGotoForObject(Unit)
                                resultItem.Text = "Bad unit overlap on tile " & X & ", " & Y & "."
                                Result.ItemAdd(resultItem)
                            Else
                                TileHasUnit(X, Y) = True
                                If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                                    TileStructureType(X, Y) = CType(Unit.Type, clsStructureType)
                                ElseIf Unit.Type.Type = clsUnitType.enumType.Feature Then
                                    TileFeatureType(X, Y) = CType(Unit.Type, clsFeatureType)
                                End If
                                TileObjectGroup(X, Y) = Unit.UnitGroup
                            End If
                        Next
                    Next
                End If
            End If
        Next
        'check modules and extractors
        For Each Unit In Map.Units
            If UnitIsStructureModule(Unit.MapLink.ArrayPosition) Then
                StructureTypeType = CType(Unit.Type, clsStructureType).StructureType
                CentrePos.X = CInt(Int(Unit.Pos.Horizontal.X / TerrainGridSpacing))
                CentrePos.Y = CInt(Int(Unit.Pos.Horizontal.Y / TerrainGridSpacing))
                If CentrePos.X < 0 Or CentrePos.X >= Map.Terrain.TileSize.X _
                  Or CentrePos.Y < 0 Or CentrePos.Y >= Map.Terrain.TileSize.Y Then
                    Dim resultItem As clsResultProblemGoto(Of clsResultItemPosGoto) = CreateResultProblemGotoForObject(Unit)
                    resultItem.Text = "Module off map at position " & Unit.GetPosText & "."
                    Result.ItemAdd(resultItem)
                Else
                    If TileStructureType(CentrePos.X, CentrePos.Y) IsNot Nothing Then
                        If TileObjectGroup(CentrePos.X, CentrePos.Y) Is Unit.UnitGroup Then
                            If StructureTypeType = clsStructureType.enumStructureType.FactoryModule Then
                                If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.Factory _
                                  Or TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.VTOLFactory Then
                                    IsValid = True
                                Else
                                    IsValid = False
                                End If
                            ElseIf StructureTypeType = clsStructureType.enumStructureType.PowerModule Then
                                If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.PowerGenerator Then
                                    IsValid = True
                                Else
                                    IsValid = False
                                End If
                            ElseIf StructureTypeType = clsStructureType.enumStructureType.ResearchModule Then
                                If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.Research Then
                                    IsValid = True
                                Else
                                    IsValid = False
                                End If
                            Else
                                IsValid = False
                            End If
                        Else
                            IsValid = False
                        End If
                    ElseIf TileFeatureType(CentrePos.X, CentrePos.Y) IsNot Nothing Then
                        If StructureTypeType = clsStructureType.enumStructureType.ResourceExtractor Then
                            If TileFeatureType(CentrePos.X, CentrePos.Y).FeatureType = clsFeatureType.enumFeatureType.OilResource Then
                                IsValid = True
                            Else
                                IsValid = False
                            End If
                        Else
                            IsValid = False
                        End If
                    ElseIf StructureTypeType = clsStructureType.enumStructureType.ResourceExtractor Then
                        IsValid = True
                    Else
                        IsValid = False
                    End If
                    If Not IsValid Then
                        Dim resultItem As clsResultProblemGoto(Of clsResultItemPosGoto) = CreateResultProblemGotoForObject(Unit)
                        resultItem.Text = "Bad module on tile " & CentrePos.X & ", " & CentrePos.Y & "."
                        Result.ItemAdd(resultItem)
                    End If
                End If
            End If
        Next

        Return Result
    End Function

    Private Function ValidateMap_Multiplayer(PlayerCount As Integer, IsXPlayerFormat As Boolean) As clsResult
        Dim Result As New clsResult("Validate for multiplayer")

        If PlayerCount < 2 Or PlayerCount > PlayerCountMax Then
            Result.ProblemAdd("Unable to evaluate for multiplayer due to bad number of players.")
            Return Result
        End If

        'check HQs, Trucks and unit counts

        Dim PlayerHQCount(PlayerCountMax - 1) As Integer
        Dim Player23TruckCount(PlayerCountMax - 1) As Integer
        Dim PlayerMasterTruckCount(PlayerCountMax - 1) As Integer
        Dim ScavPlayerNum As Integer
        Dim ScavObjectCount As Integer = 0
        Dim DroidType As clsDroidDesign
        Dim StructureType As clsStructureType
        Dim UnusedPlayerUnitWarningCount As Integer = 0
        Dim Unit As clsMap.clsUnit

        ScavPlayerNum = Math.Max(PlayerCount, 7)

        For Each Unit In Map.Units
            If Unit.UnitGroup Is Map.ScavengerUnitGroup Then

            Else
                If Unit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                    DroidType = CType(Unit.Type, clsDroidDesign)
                    If DroidType.Body IsNot Nothing And DroidType.Propulsion IsNot Nothing And DroidType.Turret1 IsNot Nothing And DroidType.TurretCount = 1 Then
                        If DroidType.Turret1.TurretType = clsTurret.enumTurretType.Construct Then
                            PlayerMasterTruckCount(Unit.UnitGroup.WZ_StartPos) += 1
                            If DroidType.IsTemplate Then
                                Player23TruckCount(Unit.UnitGroup.WZ_StartPos) += 1
                            End If
                        End If
                    End If
                ElseIf Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                    StructureType = CType(Unit.Type, clsStructureType)
                    If StructureType.Code = "A0CommandCentre" Then
                        PlayerHQCount(Unit.UnitGroup.WZ_StartPos) += 1
                    End If
                End If
            End If
            If Unit.Type.Type <> clsUnitType.enumType.Feature Then
                If Unit.UnitGroup.WZ_StartPos = ScavPlayerNum Or Unit.UnitGroup Is Map.ScavengerUnitGroup Then
                    ScavObjectCount += 1
                ElseIf Unit.UnitGroup.WZ_StartPos >= PlayerCount Then
                    If UnusedPlayerUnitWarningCount < 32 Then
                        UnusedPlayerUnitWarningCount += 1
                        Dim resultItem As clsResultProblemGoto(Of clsResultItemPosGoto) = CreateResultProblemGotoForObject(Unit)
                        resultItem.Text = "An unused player (" & Unit.UnitGroup.WZ_StartPos & ") has a unit at " & Unit.GetPosText & "."
                        Result.ItemAdd(resultItem)
                    End If
                End If
            End If
        Next

        If ScavPlayerNum <= 7 Or IsXPlayerFormat Then

        ElseIf ScavObjectCount > 0 Then 'only counts non-features
            Result.ProblemAdd("Scavengers are not supported on a map with this number of players without enabling X player support.")
        End If

        For A As Integer = 0 To PlayerCount - 1
            If PlayerHQCount(A) = 0 Then
                Result.ProblemAdd("There is no Command Centre for player " & A & ".")
            End If
            If PlayerMasterTruckCount(A) = 0 Then
                Result.ProblemAdd("There are no constructor units for player " & A & ".")
            ElseIf Player23TruckCount(A) = 0 Then
                Result.WarningAdd("All constructor units for player " & A & " will only exist in master.")
            End If
        Next

        Return Result
    End Function

    Private Function ValidateMap() As clsResult
        Dim ReturnResult As New clsResult("Validate map")

        If Map.Terrain.TileSize.X > WZMapMaxSize Then
            ReturnResult.WarningAdd("Map width is too large. The maximum is " & WZMapMaxSize & ".")
        End If
        If Map.Terrain.TileSize.Y > WZMapMaxSize Then
            ReturnResult.WarningAdd("Map height is too large. The maximum is " & WZMapMaxSize & ".")
        End If

        If Map.Tileset Is Nothing Then
            ReturnResult.ProblemAdd("No tileset selected.")
        End If

        Dim PlayerStructureTypeCount(PlayerCountMax - 1, ObjectData.StructureTypes.Count - 1) As Integer
        Dim ScavStructureTypeCount(ObjectData.StructureTypes.Count - 1) As Integer
        Dim StructureType As clsStructureType
        Dim Unit As clsMap.clsUnit

        For Each Unit In Map.Units
            If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                StructureType = CType(Unit.Type, clsStructureType)
                If Unit.UnitGroup Is Map.ScavengerUnitGroup Then
                    ScavStructureTypeCount(StructureType.StructureType_ObjectDataLink.ArrayPosition) += 1
                Else
                    PlayerStructureTypeCount(Unit.UnitGroup.WZ_StartPos, StructureType.StructureType_ObjectDataLink.ArrayPosition) += 1
                End If
            End If
        Next

        For Each StructureType In ObjectData.StructureTypes
            Dim StructureTypeNum As Integer = StructureType.StructureType_ObjectDataLink.ArrayPosition
            Dim PlayerNum As Integer
            For PlayerNum = 0 To PlayerCountMax - 1
                If PlayerStructureTypeCount(PlayerNum, StructureTypeNum) > 255 Then
                    ReturnResult.ProblemAdd("Player " & PlayerNum & " has too many (" & PlayerStructureTypeCount(PlayerNum, StructureTypeNum) & ") of structure " & ControlChars.Quote & StructureType.Code & ControlChars.Quote & ". The limit is 255 of any one structure type.")
                End If
            Next
            If ScavStructureTypeCount(StructureTypeNum) > 255 Then
                ReturnResult.ProblemAdd("Scavengers have too many (" & ScavStructureTypeCount(StructureTypeNum) & ") of structure " & ControlChars.Quote & StructureType.Code & ControlChars.Quote & ". The limit is 255 of any one structure type.")
            End If
        Next

        Return ReturnResult
    End Function

    Private Function ValidateMap_WaterTris() As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Count As Integer

        If Map.Tileset Is Nothing Then
            Return 0
        End If

        For Y = 0 To Map.Terrain.TileSize.Y - 1
            For X = 0 To Map.Terrain.TileSize.X - 1
                If Map.Terrain.Tiles(X, Y).Tri Then
                    If Map.Terrain.Tiles(X, Y).Texture.TextureNum >= 0 And Map.Terrain.Tiles(X, Y).Texture.TextureNum < Map.Tileset.TileCount Then
                        If Map.Tileset.Tiles(Map.Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                            Count += 1
                        End If
                    End If
                End If
            Next
        Next
        Return Count
    End Function

    Private Sub btnCompileCampaign_Click(sender As System.Object, e As System.EventArgs) Handles btnCompileCampaign.Click
        Dim ReturnResult As New clsResult("Compile campaign")
        Dim A As Integer

        SaveToMap()

        A = ValidateMap_WaterTris()
        If A > 0 Then
            ReturnResult.WarningAdd(A & " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.")
        End If

        ReturnResult.Add(ValidateMap())
        ReturnResult.Add(ValidateMap_UnitPositions())

        Dim MapName As String
        Dim TypeNum As Integer

        MapName = txtName.Text
        If MapName.Length < 1 Then
            ReturnResult.ProblemAdd("Enter a name for the campaign files.")
        End If
        TypeNum = cboCampType.SelectedIndex
        If TypeNum < 0 Or TypeNum > 2 Then
            ReturnResult.ProblemAdd("Select a campaign type.")
        End If
        If ReturnResult.HasProblems Then
            ShowWarnings(ReturnResult)
            Exit Sub
        End If
        Dim CompileCampDialog As New FolderBrowserDialog
        If CompileCampDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim WriteWZArgs As New clsMap.sWrite_WZ_Args
        WriteWZArgs.MapName = MapName
        WriteWZArgs.Path = CompileCampDialog.SelectedPath
        WriteWZArgs.Overwrite = False
        SetScrollLimits(WriteWZArgs.ScrollMin, WriteWZArgs.ScrollMax)
        WriteWZArgs.Campaign = New clsMap.sWrite_WZ_Args.clsCampaign
        WriteWZArgs.Campaign.GAMType = CUInt(TypeNum)
        WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Campaign
        ReturnResult.Add(Map.Write_WZ(WriteWZArgs))
        ShowWarnings(ReturnResult)
        If Not ReturnResult.HasWarnings Then
            Close()
        End If
    End Sub

    Public Sub AutoScrollLimits_Update()

        If cbxAutoScrollLimits.Checked Then
            txtScrollMinX.Enabled = False
            txtScrollMaxX.Enabled = False
            txtScrollMinY.Enabled = False
            txtScrollMaxY.Enabled = False
        Else
            txtScrollMinX.Enabled = True
            txtScrollMaxX.Enabled = True
            txtScrollMinY.Enabled = True
            txtScrollMaxY.Enabled = True
        End If
    End Sub

    Private Sub cbxAutoScrollLimits_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles cbxAutoScrollLimits.CheckedChanged
        If Not cbxAutoScrollLimits.Enabled Then
            Exit Sub
        End If

        AutoScrollLimits_Update()
    End Sub

    Private Sub SetScrollLimits(ByRef Min As sXY_int, ByRef Max As sXY_uint)

        Min.X = 0
        Min.Y = 0
        Max.X = CUInt(Map.Terrain.TileSize.X)
        Max.Y = CUInt(Map.Terrain.TileSize.Y)
        If Not cbxAutoScrollLimits.Checked Then
            InvariantParse_int(txtScrollMinX.Text, Min.X)
            InvariantParse_int(txtScrollMinY.Text, Min.Y)
            InvariantParse_uint(txtScrollMaxX.Text, Max.X)
            InvariantParse_uint(txtScrollMaxY.Text, Max.Y)
        End If
    End Sub
End Class
