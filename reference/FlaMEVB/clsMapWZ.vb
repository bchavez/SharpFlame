Imports ICSharpCode.SharpZipLib

Partial Public Class clsMap

    Public Class clsWZBJOUnit
        Public Code As String
        Public ID As UInteger
        Public Pos As sWorldPos
        Public Rotation As UInteger
        Public Player As UInteger
        Public ObjectType As clsUnitType.enumType
    End Class

    Public Class clsWZMapEntry
        Public Name As String
        Public Tileset As clsTileset
    End Class

    Public Function Load_WZ(Path As String) As clsResult
        Dim ReturnResult As New clsResult("Loading WZ from " & ControlChars.Quote & Path & ControlChars.Quote)
        Dim SubResult As sResult
        Dim Quote As String = ControlChars.Quote
        Dim ZipEntry As Zip.ZipEntry
        Dim GameFound As Boolean
        Dim DatasetFound As Boolean
        Dim Maps As New SimpleList(Of clsWZMapEntry)
        Dim GameTileset As clsTileset = Nothing
        Dim GameName As String = ""
        Dim strTemp As String = ""
        Dim SplitPath As sZipSplitPath
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer

        Dim File As IO.FileStream
        Try
            File = IO.File.OpenRead(Path)
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        Dim ZipStream As New Zip.ZipInputStream(File)

        'get all usable lev entries
        Do
            ZipEntry = ZipStream.GetNextEntry
            If ZipEntry Is Nothing Then
                Exit Do
            End If

            SplitPath = New sZipSplitPath(ZipEntry.Name)

            If SplitPath.FileExtension = "lev" And SplitPath.PartCount = 1 Then
                If ZipEntry.Size > 10 * 1024 * 1024 Then
                    ReturnResult.ProblemAdd("lev file is too large.")
                    ZipStream.Close()
                    Return ReturnResult
                End If
                Dim Reader As New IO.BinaryReader(ZipStream)
                Dim LineData As SimpleList(Of String) = BytesToLinesRemoveComments(Reader)
                'find each level block
                For A = 0 To LineData.Count - 1
                    If Strings.LCase(Strings.Left(LineData(A), 5)) = "level" Then
                        'find each levels game file
                        GameFound = False
                        B = 1
                        Do While A + B < LineData.Count
                            If Strings.LCase(Strings.Left(LineData(A + B), 4)) = "game" Then
                                C = Strings.InStr(LineData(A + B), Quote)
                                D = Strings.InStrRev(LineData(A + B), Quote)
                                If C > 0 And D > 0 And D - C > 1 Then
                                    GameName = Strings.LCase(Strings.Mid(LineData(A + B), C + 1, D - C - 1))
                                    'see if map is already counted
                                    For C = 0 To Maps.Count - 1
                                        If GameName = Maps(C).Name Then
                                            Exit For
                                        End If
                                    Next
                                    If C = Maps.Count Then
                                        GameFound = True
                                    End If
                                End If
                                Exit Do
                            ElseIf Strings.LCase(Strings.Left(LineData(A + B), 5)) = "level" Then
                                Exit Do
                            End If
                            B += 1
                        Loop
                        If GameFound Then
                            'find the dataset (determines tileset)
                            DatasetFound = False
                            B = 1
                            Do While A + B < LineData.Count
                                If Strings.LCase(Strings.Left(LineData(A + B), 7)) = "dataset" Then
                                    strTemp = Strings.LCase(Strings.Right(LineData(A + B), 1))
                                    If strTemp = "1" Then
                                        GameTileset = Tileset_Arizona
                                        DatasetFound = True
                                    ElseIf strTemp = "2" Then
                                        GameTileset = Tileset_Urban
                                        DatasetFound = True
                                    ElseIf strTemp = "3" Then
                                        GameTileset = Tileset_Rockies
                                        DatasetFound = True
                                    End If
                                    Exit Do
                                ElseIf Strings.LCase(Strings.Left(LineData(A + B), 5)) = "level" Then
                                    Exit Do
                                End If
                                B += 1
                            Loop
                            If DatasetFound Then
                                Dim NewMap As New clsWZMapEntry
                                NewMap.Name = GameName
                                NewMap.Tileset = GameTileset
                                Maps.Add(NewMap)
                            End If
                        End If
                    End If
                Next
            End If
        Loop
        ZipStream.Close()

        Dim MapLoadName As String

        'prompt user for which of the entries to load
        If Maps.Count < 1 Then
            ReturnResult.ProblemAdd("No maps found in file.")
            Return ReturnResult
        ElseIf Maps.Count = 1 Then
            MapLoadName = Maps(0).Name
            Tileset = Maps(0).Tileset
        Else
            Dim SelectToLoadResult As New frmWZLoad.clsOutput
            Dim Names(Maps.Count - 1) As String
            For A = 0 To Maps.Count - 1
                Names(A) = Maps(A).Name
            Next
            Dim SelectToLoadForm As New frmWZLoad(Names, SelectToLoadResult, "Select a map from " & New sSplitPath(Path).FileTitle)
            SelectToLoadForm.ShowDialog()
            If SelectToLoadResult.Result < 0 Then
                ReturnResult.ProblemAdd("No map selected.")
                Return ReturnResult
            End If
            MapLoadName = Maps(SelectToLoadResult.Result).Name
            Tileset = Maps(SelectToLoadResult.Result).Tileset
        End If

        TileType_Reset()
        SetPainterToDefaults()

        Dim GameSplitPath As New sZipSplitPath(MapLoadName)
        Dim GameFilesPath As String = GameSplitPath.FilePath & GameSplitPath.FileTitleWithoutExtension & "/"

        Dim ZipSearchResult As clsZipStreamEntry

        ZipSearchResult = FindZipEntryFromPath(Path, MapLoadName)
        If ZipSearchResult Is Nothing Then
            ReturnResult.ProblemAdd("Game file not found.")
            Return ReturnResult
        Else
            Dim Map_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            SubResult = Read_WZ_gam(Map_Reader)
            Map_Reader.Close()

            If Not SubResult.Success Then
                ReturnResult.ProblemAdd(SubResult.Problem)
                Return ReturnResult
            End If
        End If

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "game.map")
        If ZipSearchResult Is Nothing Then
            ReturnResult.ProblemAdd("game.map file not found")
            Return ReturnResult
        Else
            Dim Map_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            SubResult = Read_WZ_map(Map_Reader)
            Map_Reader.Close()

            If Not SubResult.Success Then
                ReturnResult.ProblemAdd(SubResult.Problem)
                Return ReturnResult
            End If
        End If

        Dim BJOUnits As New SimpleClassList(Of clsMap.clsWZBJOUnit)

        Dim INIFeatures As clsINIFeatures = Nothing

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "feature.ini")
        If ZipSearchResult Is Nothing Then

        Else
            Dim Result As New clsResult("feature.ini")
            Dim FeaturesINI As New clsINIRead
            Dim FeaturesINI_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            Result.Take(FeaturesINI.ReadFile(FeaturesINI_Reader))
            FeaturesINI_Reader.Close()
            INIFeatures = New clsINIFeatures(FeaturesINI.Sections.Count)
            Result.Take(FeaturesINI.Translate(INIFeatures))
            ReturnResult.Add(Result)
        End If

        If INIFeatures Is Nothing Then
            Dim Result As New clsResult("feat.bjo")
            ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "feat.bjo")
            If ZipSearchResult Is Nothing Then
                Result.WarningAdd("file not found")
            Else
                Dim Features_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
                SubResult = Read_WZ_Features(Features_Reader, BJOUnits)
                Features_Reader.Close()
                If Not SubResult.Success Then
                    Result.WarningAdd(SubResult.Problem)
                End If
            End If
            ReturnResult.Add(Result)
        End If

        If True Then
            Dim Result As New clsResult("ttypes.ttp")
            ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "ttypes.ttp")
            If ZipSearchResult Is Nothing Then
                Result.WarningAdd("file not found")
            Else
                Dim TileTypes_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
                SubResult = Read_WZ_TileTypes(TileTypes_Reader)
                TileTypes_Reader.Close()
                If Not SubResult.Success Then
                    Result.WarningAdd(SubResult.Problem)
                End If
            End If
            ReturnResult.Add(Result)
        End If

        Dim INIStructures As clsINIStructures = Nothing

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "struct.ini")
        If ZipSearchResult Is Nothing Then

        Else
            Dim Result As New clsResult("struct.ini")
            Dim StructuresINI As New clsINIRead
            Dim StructuresINI_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            Result.Take(StructuresINI.ReadFile(StructuresINI_Reader))
            StructuresINI_Reader.Close()
            INIStructures = New clsINIStructures(StructuresINI.Sections.Count, Me)
            Result.Take(StructuresINI.Translate(INIStructures))
            ReturnResult.Add(Result)
        End If

        If INIStructures Is Nothing Then
            Dim Result As New clsResult("struct.bjo")
            ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "struct.bjo")
            If ZipSearchResult Is Nothing Then
                Result.WarningAdd("file not found")
            Else
                Dim Structures_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
                SubResult = Read_WZ_Structures(Structures_Reader, BJOUnits)
                Structures_Reader.Close()
                If Not SubResult.Success Then
                    Result.WarningAdd(SubResult.Problem)
                End If
            End If
            ReturnResult.Add(Result)
        End If

        Dim INIDroids As clsINIDroids = Nothing

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "droid.ini")
        If ZipSearchResult Is Nothing Then

        Else
            Dim Result As New clsResult("droid.ini")
            Dim DroidsINI As New clsINIRead
            Dim DroidsINI_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            Result.Take(DroidsINI.ReadFile(DroidsINI_Reader))
            DroidsINI_Reader.Close()
            INIDroids = New clsINIDroids(DroidsINI.Sections.Count, Me)
            Result.Take(DroidsINI.Translate(INIDroids))
            ReturnResult.Add(Result)
        End If

        If INIDroids Is Nothing Then
            Dim Result As New clsResult("dinit.bjo")
            ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "dinit.bjo")
            If ZipSearchResult Is Nothing Then
                Result.WarningAdd("file not found")
            Else
                Dim Droids_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
                SubResult = Read_WZ_Droids(Droids_Reader, BJOUnits)
                Droids_Reader.Close()
                If Not SubResult.Success Then
                    Result.WarningAdd(SubResult.Problem)
                End If
            End If
            ReturnResult.Add(Result)
        End If

        Dim CreateObjectsArgs As sCreateWZObjectsArgs
        CreateObjectsArgs.BJOUnits = BJOUnits
        CreateObjectsArgs.INIStructures = INIStructures
        CreateObjectsArgs.INIDroids = INIDroids
        CreateObjectsArgs.INIFeatures = INIFeatures
        ReturnResult.Add(CreateWZObjects(CreateObjectsArgs))

        'objects are modified by this and must already exist
        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "labels.ini")
        If ZipSearchResult Is Nothing Then

        Else
            Dim Result As New clsResult("labels.ini")
            Dim LabelsINI As New clsINIRead
            Dim LabelsINI_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            Result.Take(LabelsINI.ReadFile(LabelsINI_Reader))
            LabelsINI_Reader.Close()
            Result.Take(Read_WZ_Labels(LabelsINI, False))
            ReturnResult.Add(Result)
        End If

        Return ReturnResult
    End Function

    Public Function Load_Game(Path As String) As clsResult
        Dim ReturnResult As New clsResult("Loading game file from " & ControlChars.Quote & Path & ControlChars.Quote)
        Dim SubResult As sResult
        Dim Quote As String = ControlChars.Quote

        Tileset = Nothing

        TileType_Reset()
        SetPainterToDefaults()

        Dim GameSplitPath As New sSplitPath(Path)
        Dim GameFilesPath As String = GameSplitPath.FilePath & GameSplitPath.FileTitleWithoutExtension & PlatformPathSeparator
        Dim MapDirectory As String
        Dim File As IO.FileStream = Nothing

        SubResult = TryOpenFileStream(Path, File)
        If Not SubResult.Success Then
            ReturnResult.ProblemAdd("Game file not found: " & SubResult.Problem)
            Return ReturnResult
        Else
            Dim Map_Reader As New IO.BinaryReader(File)
            SubResult = Read_WZ_gam(Map_Reader)
            Map_Reader.Close()

            If Not SubResult.Success Then
                ReturnResult.ProblemAdd(SubResult.Problem)
                Return ReturnResult
            End If
        End If

        SubResult = TryOpenFileStream(GameFilesPath & "game.map", File)
        If Not SubResult.Success Then
            Dim PromptResult As MsgBoxResult = MsgBox("game.map file not found at " & GameFilesPath & ControlChars.NewLine & "Do you want to select another directory to load the underlying map from?", (MsgBoxStyle.OkCancel Or MsgBoxStyle.Question))
            If PromptResult <> MsgBoxResult.Ok Then
                ReturnResult.ProblemAdd("Aborted.")
                Return ReturnResult
            End If
            Dim DirectorySelect As New FolderBrowserDialog
            DirectorySelect.SelectedPath = GameFilesPath
            If DirectorySelect.ShowDialog() <> DialogResult.OK Then
                ReturnResult.ProblemAdd("Aborted.")
                Return ReturnResult
            End If
            MapDirectory = DirectorySelect.SelectedPath & PlatformPathSeparator

            SubResult = TryOpenFileStream(MapDirectory & "game.map", File)
            If Not SubResult.Success Then
                ReturnResult.ProblemAdd("game.map file not found: " & SubResult.Problem)
                Return ReturnResult
            End If
        Else
            MapDirectory = GameFilesPath
        End If

        Dim Map_ReaderB As New IO.BinaryReader(File)
        SubResult = Read_WZ_map(Map_ReaderB)
        Map_ReaderB.Close()

        If Not SubResult.Success Then
            ReturnResult.ProblemAdd(SubResult.Problem)
            Return ReturnResult
        End If

        Dim BJOUnits As New SimpleClassList(Of clsMap.clsWZBJOUnit)

        Dim INIFeatures As clsINIFeatures = Nothing

        SubResult = TryOpenFileStream(GameFilesPath & "feature.ini", File)
        If Not SubResult.Success Then

        Else
            Dim Result As New clsResult("feature.ini")
            Dim FeaturesINI As New clsINIRead
            Dim FeaturesINI_Reader As New IO.StreamReader(File)
            Result.Take(FeaturesINI.ReadFile(FeaturesINI_Reader))
            FeaturesINI_Reader.Close()
            INIFeatures = New clsINIFeatures(FeaturesINI.Sections.Count)
            Result.Take(FeaturesINI.Translate(INIFeatures))
            ReturnResult.Add(Result)
        End If

        If INIFeatures Is Nothing Then
            Dim Result As New clsResult("feat.bjo")
            SubResult = TryOpenFileStream(GameFilesPath & "feat.bjo", File)
            If Not SubResult.Success Then
                Result.WarningAdd("file not found")
            Else
                Dim Features_Reader As New IO.BinaryReader(File)
                SubResult = Read_WZ_Features(Features_Reader, BJOUnits)
                Features_Reader.Close()
                If Not SubResult.Success Then
                    Result.WarningAdd(SubResult.Problem)
                End If
            End If
            ReturnResult.Add(Result)
        End If

        If True Then
            Dim Result As New clsResult("ttypes.ttp")
            SubResult = TryOpenFileStream(MapDirectory & "ttypes.ttp", File)
            If Not SubResult.Success Then
                Result.WarningAdd("file not found")
            Else
                Dim TileTypes_Reader As New IO.BinaryReader(File)
                SubResult = Read_WZ_TileTypes(TileTypes_Reader)
                TileTypes_Reader.Close()
                If Not SubResult.Success Then
                    Result.WarningAdd(SubResult.Problem)
                End If
            End If
            ReturnResult.Add(Result)
        End If

        Dim INIStructures As clsINIStructures = Nothing

        SubResult = TryOpenFileStream(GameFilesPath & "struct.ini", File)
        If Not SubResult.Success Then

        Else
            Dim Result As New clsResult("struct.ini")
            Dim StructuresINI As New clsINIRead
            Dim StructuresINI_Reader As New IO.StreamReader(File)
            Result.Take(StructuresINI.ReadFile(StructuresINI_Reader))
            StructuresINI_Reader.Close()
            INIStructures = New clsINIStructures(StructuresINI.Sections.Count, Me)
            Result.Take(StructuresINI.Translate(INIStructures))
            ReturnResult.Add(Result)
        End If

        If INIStructures Is Nothing Then
            Dim Result As New clsResult("struct.bjo")
            SubResult = TryOpenFileStream(GameFilesPath & "struct.bjo", File)
            If Not SubResult.Success Then
                Result.WarningAdd("struct.bjo file not found.")
            Else
                Dim Structures_Reader As New IO.BinaryReader(File)
                SubResult = Read_WZ_Structures(Structures_Reader, BJOUnits)
                Structures_Reader.Close()
                If Not SubResult.Success Then
                    Result.WarningAdd(SubResult.Problem)
                End If
            End If
            ReturnResult.Add(Result)
        End If

        Dim INIDroids As clsINIDroids = Nothing

        SubResult = TryOpenFileStream(GameFilesPath & "droid.ini", File)
        If Not SubResult.Success Then

        Else
            Dim Result As New clsResult("droid.ini")
            Dim DroidsINI As New clsINIRead
            Dim DroidsINI_Reader As New IO.StreamReader(File)
            Result.Take(DroidsINI.ReadFile(DroidsINI_Reader))
            DroidsINI_Reader.Close()
            INIDroids = New clsINIDroids(DroidsINI.Sections.Count, Me)
            Result.Take(DroidsINI.Translate(INIDroids))
            ReturnResult.Add(Result)
        End If

        If INIStructures Is Nothing Then
            Dim Result As New clsResult("dinit.bjo")
            SubResult = TryOpenFileStream(GameFilesPath & "dinit.bjo", File)
            If Not SubResult.Success Then
                Result.WarningAdd("dinit.bjo file not found.")
            Else
                Dim Droids_Reader As New IO.BinaryReader(File)
                SubResult = Read_WZ_Droids(Droids_Reader, BJOUnits)
                Droids_Reader.Close()
                If Not SubResult.Success Then
                    Result.WarningAdd(SubResult.Problem)
                End If
            End If
            ReturnResult.Add(Result)
        End If

        Dim CreateObjectsArgs As sCreateWZObjectsArgs
        CreateObjectsArgs.BJOUnits = BJOUnits
        CreateObjectsArgs.INIStructures = INIStructures
        CreateObjectsArgs.INIDroids = INIDroids
        CreateObjectsArgs.INIFeatures = INIFeatures
        ReturnResult.Add(CreateWZObjects(CreateObjectsArgs))

        'map objects are modified by this and must already exist
        SubResult = TryOpenFileStream(GameFilesPath & "labels.ini", File)
        If Not SubResult.Success Then

        Else
            Dim Result As New clsResult("labels.ini")
            Dim LabelsINI As New clsINIRead
            Dim LabelsINI_Reader As New IO.StreamReader(File)
            Result.Take(LabelsINI.ReadFile(LabelsINI_Reader))
            LabelsINI_Reader.Close()
            Result.Take(Read_WZ_Labels(LabelsINI, False))
            ReturnResult.Add(Result)
        End If

        Return ReturnResult
    End Function

    Public Structure sCreateWZObjectsArgs
        Public BJOUnits As SimpleClassList(Of clsMap.clsWZBJOUnit)
        Public INIStructures As clsINIStructures
        Public INIDroids As clsINIDroids
        Public INIFeatures As clsINIFeatures
    End Structure

    Public Function CreateWZObjects(Args As sCreateWZObjectsArgs) As clsResult
        Dim ReturnResult As New clsResult("Creating objects")
        Dim NewUnit As clsUnit
        Dim AvailableID As UInteger
        Dim BJOUnits As SimpleClassList(Of clsMap.clsWZBJOUnit) = Args.BJOUnits
        Dim INIStructures As clsINIStructures = Args.INIStructures
        Dim INIDroids As clsINIDroids = Args.INIDroids
        Dim INIFeatures As clsINIFeatures = Args.INIFeatures
        Dim UnitAdd As New clsMap.clsUnitAdd
        Dim A As Integer
        Dim B As Integer
        Dim BJOUnit As clsWZBJOUnit

        UnitAdd.Map = Me

        AvailableID = 1UI
        For Each BJOUnit In BJOUnits
            If BJOUnit.ID >= AvailableID Then
                AvailableID = BJOUnit.ID + 1UI
            End If
        Next
        If INIStructures IsNot Nothing Then
            For A = 0 To INIStructures.StructureCount - 1
                If INIStructures.Structures(A).ID >= AvailableID Then
                    AvailableID = INIStructures.Structures(A).ID + 1UI
                End If
            Next
        End If
        If INIFeatures IsNot Nothing Then
            For A = 0 To INIFeatures.FeatureCount - 1
                If INIFeatures.Features(A).ID >= AvailableID Then
                    AvailableID = INIFeatures.Features(A).ID + 1UI
                End If
            Next
        End If
        If INIDroids IsNot Nothing Then
            For A = 0 To INIDroids.DroidCount - 1
                If INIDroids.Droids(A).ID >= AvailableID Then
                    AvailableID = INIDroids.Droids(A).ID + 1UI
                End If
            Next
        End If

        For Each BJOUnit In BJOUnits
            NewUnit = New clsUnit
            NewUnit.ID = BJOUnit.ID
            NewUnit.Type = ObjectData.FindOrCreateUnitType(BJOUnit.Code, BJOUnit.ObjectType, -1)
            If NewUnit.Type Is Nothing Then
                ReturnResult.ProblemAdd("Unable to create object type.")
                Return ReturnResult
            End If
            If BJOUnit.Player >= PlayerCountMax Then
                NewUnit.UnitGroup = ScavengerUnitGroup
            Else
                NewUnit.UnitGroup = UnitGroups.Item(CInt(BJOUnit.Player))
            End If
            NewUnit.Pos = BJOUnit.Pos
            NewUnit.Rotation = CInt(Math.Min(BJOUnit.Rotation, 359UI))
            If BJOUnit.ID = 0UI Then
                BJOUnit.ID = AvailableID
                ZeroIDWarning(NewUnit, BJOUnit.ID, ReturnResult)
            End If
            UnitAdd.NewUnit = NewUnit
            UnitAdd.ID = BJOUnit.ID
            UnitAdd.Perform()
            ErrorIDChange(BJOUnit.ID, NewUnit, "CreateWZObjects")
            If AvailableID = BJOUnit.ID Then
                AvailableID = NewUnit.ID + 1UI
            End If
        Next

        Dim StructureType As clsStructureType
        Dim DroidType As clsDroidDesign
        Dim FeatureType As clsFeatureType
        Dim LoadPartsArgs As clsDroidDesign.sLoadPartsArgs
        Dim UnitType As clsUnitType = Nothing
        Dim ErrorCount As Integer = 0
        Dim UnknownDroidComponentCount As Integer = 0
        Dim UnknownDroidTypeCount As Integer = 0
        Dim DroidBadPositionCount As Integer = 0
        Dim StructureBadPositionCount As Integer = 0
        Dim StructureBadModulesCount As Integer = 0
        Dim FeatureBadPositionCount As Integer = 0
        Dim ModuleLimit As Integer
        Dim ZeroPos As New sXY_int(0, 0)
        Dim ModuleType As clsStructureType
        Dim NewModule As clsUnit

        Dim FactoryModule As clsStructureType = ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.FactoryModule)
        Dim ResearchModule As clsStructureType = ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.ResearchModule)
        Dim PowerModule As clsStructureType = ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.PowerModule)

        If FactoryModule Is Nothing Then
            ReturnResult.WarningAdd("No factory module loaded.")
        End If
        If ResearchModule Is Nothing Then
            ReturnResult.WarningAdd("No research module loaded.")
        End If
        If PowerModule Is Nothing Then
            ReturnResult.WarningAdd("No power module loaded.")
        End If

        If INIStructures IsNot Nothing Then
            For A = 0 To INIStructures.StructureCount - 1
                If INIStructures.Structures(A).Pos Is Nothing Then
                    StructureBadPositionCount += 1
                ElseIf Not PosIsWithinTileArea(INIStructures.Structures(A).Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) Then
                    StructureBadPositionCount += 1
                Else
                    UnitType = ObjectData.FindOrCreateUnitType(INIStructures.Structures(A).Code, clsUnitType.enumType.PlayerStructure, INIStructures.Structures(A).WallType)
                    If UnitType.Type = clsUnitType.enumType.PlayerStructure Then
                        StructureType = CType(UnitType, clsStructureType)
                    Else
                        StructureType = Nothing
                    End If
                    If StructureType Is Nothing Then
                        ErrorCount += 1
                    Else
                        NewUnit = New clsUnit
                        NewUnit.Type = StructureType
                        If INIStructures.Structures(A).UnitGroup Is Nothing Then
                            NewUnit.UnitGroup = ScavengerUnitGroup
                        Else
                            NewUnit.UnitGroup = INIStructures.Structures(A).UnitGroup
                        End If
                        NewUnit.Pos = INIStructures.Structures(A).Pos.WorldPos
                        NewUnit.Rotation = CInt(INIStructures.Structures(A).Rotation.Direction * 360.0# / INIRotationMax)
                        If NewUnit.Rotation = 360 Then
                            NewUnit.Rotation = 0
                        End If
                        If INIStructures.Structures(A).HealthPercent >= 0 Then
                            NewUnit.Health = Clamp_dbl(INIStructures.Structures(A).HealthPercent / 100.0#, 0.01#, 1.0#)
                        End If
                        If INIStructures.Structures(A).ID = 0UI Then
                            INIStructures.Structures(A).ID = AvailableID
                            ZeroIDWarning(NewUnit, INIStructures.Structures(A).ID, ReturnResult)
                        End If
                        UnitAdd.NewUnit = NewUnit
                        UnitAdd.ID = INIStructures.Structures(A).ID
                        UnitAdd.Perform()
                        ErrorIDChange(INIStructures.Structures(A).ID, NewUnit, "Load_WZ->INIStructures")
                        If AvailableID = INIStructures.Structures(A).ID Then
                            AvailableID = NewUnit.ID + 1UI
                        End If
                        'create modules
                        Select Case StructureType.StructureType
                            Case clsStructureType.enumStructureType.Factory
                                ModuleLimit = 2
                                ModuleType = FactoryModule
                            Case clsStructureType.enumStructureType.VTOLFactory
                                ModuleLimit = 2
                                ModuleType = FactoryModule
                            Case clsStructureType.enumStructureType.PowerGenerator
                                ModuleLimit = 1
                                ModuleType = PowerModule
                            Case clsStructureType.enumStructureType.Research
                                ModuleLimit = 1
                                ModuleType = ResearchModule
                            Case Else
                                ModuleLimit = 0
                                ModuleType = Nothing
                        End Select
                        If INIStructures.Structures(A).ModuleCount > ModuleLimit Then
                            INIStructures.Structures(A).ModuleCount = ModuleLimit
                            StructureBadModulesCount += 1
                        ElseIf INIStructures.Structures(A).ModuleCount < 0 Then
                            INIStructures.Structures(A).ModuleCount = 0
                            StructureBadModulesCount += 1
                        End If
                        If ModuleType IsNot Nothing Then
                            For B = 0 To INIStructures.Structures(A).ModuleCount - 1
                                NewModule = New clsUnit
                                NewModule.Type = ModuleType
                                NewModule.UnitGroup = NewUnit.UnitGroup
                                NewModule.Pos = NewUnit.Pos
                                NewModule.Rotation = NewUnit.Rotation
                                UnitAdd.NewUnit = NewModule
                                UnitAdd.ID = AvailableID
                                UnitAdd.Perform()
                                AvailableID = NewModule.ID + 1UI
                            Next
                        End If
                    End If
                End If
            Next
            If StructureBadPositionCount > 0 Then
                ReturnResult.WarningAdd(StructureBadPositionCount & " structures had an invalid position and were removed.")
            End If
            If StructureBadModulesCount > 0 Then
                ReturnResult.WarningAdd(StructureBadModulesCount & " structures had an invalid number of modules.")
            End If
        End If
        If INIFeatures IsNot Nothing Then
            For A = 0 To INIFeatures.FeatureCount - 1
                If INIFeatures.Features(A).Pos Is Nothing Then
                    FeatureBadPositionCount += 1
                ElseIf Not PosIsWithinTileArea(INIFeatures.Features(A).Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) Then
                    FeatureBadPositionCount += 1
                Else
                    UnitType = ObjectData.FindOrCreateUnitType(INIFeatures.Features(A).Code, clsUnitType.enumType.Feature, -1)
                    If UnitType.Type = clsUnitType.enumType.Feature Then
                        FeatureType = CType(UnitType, clsFeatureType)
                    Else
                        FeatureType = Nothing
                    End If
                    If FeatureType Is Nothing Then
                        ErrorCount += 1
                    Else
                        NewUnit = New clsUnit
                        NewUnit.Type = FeatureType
                        NewUnit.UnitGroup = ScavengerUnitGroup
                        NewUnit.Pos = INIFeatures.Features(A).Pos.WorldPos
                        NewUnit.Rotation = CInt(INIFeatures.Features(A).Rotation.Direction * 360.0# / INIRotationMax)
                        If NewUnit.Rotation = 360 Then
                            NewUnit.Rotation = 0
                        End If
                        If INIFeatures.Features(A).HealthPercent >= 0 Then
                            NewUnit.Health = Clamp_dbl(INIFeatures.Features(A).HealthPercent / 100.0#, 0.01#, 1.0#)
                        End If
                        If INIFeatures.Features(A).ID = 0UI Then
                            INIFeatures.Features(A).ID = AvailableID
                            ZeroIDWarning(NewUnit, INIFeatures.Features(A).ID, ReturnResult)
                        End If
                        UnitAdd.NewUnit = NewUnit
                        UnitAdd.ID = INIFeatures.Features(A).ID
                        UnitAdd.Perform()
                        ErrorIDChange(INIFeatures.Features(A).ID, NewUnit, "Load_WZ->INIFeatures")
                        If AvailableID = INIFeatures.Features(A).ID Then
                            AvailableID = NewUnit.ID + 1UI
                        End If
                    End If
                End If
            Next
            If FeatureBadPositionCount > 0 Then
                ReturnResult.WarningAdd(FeatureBadPositionCount & " features had an invalid position and were removed.")
            End If
        End If
        If INIDroids IsNot Nothing Then
            For A = 0 To INIDroids.DroidCount - 1
                If INIDroids.Droids(A).Pos Is Nothing Then
                    DroidBadPositionCount += 1
                ElseIf Not PosIsWithinTileArea(INIDroids.Droids(A).Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) Then
                    DroidBadPositionCount += 1
                Else
                    If INIDroids.Droids(A).Template = Nothing Or INIDroids.Droids(A).Template = "" Then
                        DroidType = New clsDroidDesign
                        If Not DroidType.SetDroidType(CType(INIDroids.Droids(A).DroidType, enumDroidType)) Then
                            UnknownDroidTypeCount += 1
                        End If
                        LoadPartsArgs.Body = ObjectData.FindOrCreateBody(INIDroids.Droids(A).Body)
                        If LoadPartsArgs.Body Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Body.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Propulsion = ObjectData.FindOrCreatePropulsion(INIDroids.Droids(A).Propulsion)
                        If LoadPartsArgs.Propulsion Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Propulsion.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Construct = ObjectData.FindOrCreateConstruct(INIDroids.Droids(A).Construct)
                        If LoadPartsArgs.Construct Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Construct.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Repair = ObjectData.FindOrCreateRepair(INIDroids.Droids(A).Repair)
                        If LoadPartsArgs.Repair Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Repair.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Sensor = ObjectData.FindOrCreateSensor(INIDroids.Droids(A).Sensor)
                        If LoadPartsArgs.Sensor Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Sensor.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Brain = ObjectData.FindOrCreateBrain(INIDroids.Droids(A).Brain)
                        If LoadPartsArgs.Brain Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Brain.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.ECM = ObjectData.FindOrCreateECM(INIDroids.Droids(A).ECM)
                        If LoadPartsArgs.ECM Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.ECM.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Weapon1 = ObjectData.FindOrCreateWeapon(INIDroids.Droids(A).Weapons(0))
                        If LoadPartsArgs.Weapon1 Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Weapon1.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Weapon2 = ObjectData.FindOrCreateWeapon(INIDroids.Droids(A).Weapons(1))
                        If LoadPartsArgs.Weapon2 Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Weapon2.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Weapon3 = ObjectData.FindOrCreateWeapon(INIDroids.Droids(A).Weapons(2))
                        If LoadPartsArgs.Weapon3 Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Weapon3.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        DroidType.LoadParts(LoadPartsArgs)
                    Else
                        UnitType = ObjectData.FindOrCreateUnitType(INIDroids.Droids(A).Template, clsUnitType.enumType.PlayerDroid, -1)
                        If UnitType Is Nothing Then
                            DroidType = Nothing
                        Else
                            If UnitType.Type = clsUnitType.enumType.PlayerDroid Then
                                DroidType = CType(UnitType, clsDroidDesign)
                            Else
                                DroidType = Nothing
                            End If
                        End If
                    End If
                    If DroidType Is Nothing Then
                        ErrorCount += 1
                    Else
                        NewUnit = New clsUnit
                        NewUnit.Type = DroidType
                        If INIDroids.Droids(A).UnitGroup Is Nothing Then
                            NewUnit.UnitGroup = ScavengerUnitGroup
                        Else
                            NewUnit.UnitGroup = INIDroids.Droids(A).UnitGroup
                        End If
                        NewUnit.Pos = INIDroids.Droids(A).Pos.WorldPos
                        NewUnit.Rotation = CInt(INIDroids.Droids(A).Rotation.Direction * 360.0# / INIRotationMax)
                        If NewUnit.Rotation = 360 Then
                            NewUnit.Rotation = 0
                        End If
                        If INIDroids.Droids(A).HealthPercent >= 0 Then
                            NewUnit.Health = Clamp_dbl(INIDroids.Droids(A).HealthPercent / 100.0#, 0.01#, 1.0#)
                        End If
                        If INIDroids.Droids(A).ID = 0UI Then
                            INIDroids.Droids(A).ID = AvailableID
                            ZeroIDWarning(NewUnit, INIDroids.Droids(A).ID, ReturnResult)
                        End If
                        UnitAdd.NewUnit = NewUnit
                        UnitAdd.ID = INIDroids.Droids(A).ID
                        UnitAdd.Perform()
                        ErrorIDChange(INIDroids.Droids(A).ID, NewUnit, "Load_WZ->INIDroids")
                        If AvailableID = INIDroids.Droids(A).ID Then
                            AvailableID = NewUnit.ID + 1UI
                        End If
                    End If
                End If
            Next
            If DroidBadPositionCount > 0 Then
                ReturnResult.WarningAdd(DroidBadPositionCount & " droids had an invalid position and were removed.")
            End If
            If UnknownDroidTypeCount > 0 Then
                ReturnResult.WarningAdd(UnknownDroidTypeCount & " droid designs had an unrecognised droidType and were removed.")
            End If
            If UnknownDroidComponentCount > 0 Then
                ReturnResult.WarningAdd(UnknownDroidComponentCount & " droid designs had components that are not loaded.")
            End If
        End If

        If ErrorCount > 0 Then
            ReturnResult.WarningAdd("Object Create Error.")
        End If

        Return ReturnResult
    End Function

    Public Class clsINIStructures
        Inherits clsINIRead.clsSectionTranslator

        Private ParentMap As clsMap

        Public Structure sStructure
            Public ID As UInteger
            Public Code As String
            Public UnitGroup As clsUnitGroup
            Public Pos As clsWorldPos
            Public Rotation As sWZAngle
            Public ModuleCount As Integer
            Public HealthPercent As Integer
            Public WallType As Integer
        End Structure
        Public Structures() As sStructure
        Public StructureCount As Integer

        Public Sub New(NewStructureCount As Integer, NewParentMap As clsMap)
            Dim A As Integer

            ParentMap = NewParentMap

            StructureCount = NewStructureCount
            ReDim Structures(StructureCount - 1)
            For A = 0 To StructureCount - 1
                Structures(A).HealthPercent = -1
                Structures(A).WallType = -1
            Next
        End Sub

        Public Overrides Function Translate(INISectionNum As Integer, INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "id"
                    Dim uintTemp As UInteger
                    If InvariantParse_uint(INIProperty.Value, uintTemp) Then
                        If uintTemp > 0 Then
                            Structures(INISectionNum).ID = uintTemp
                        End If
                    Else
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "name"
                    Structures(INISectionNum).Code = INIProperty.Value
                Case "startpos"
                    Dim StartPos As Integer
                    If Not InvariantParse_int(INIProperty.Value, StartPos) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If StartPos < 0 Or StartPos >= PlayerCountMax Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Structures(INISectionNum).UnitGroup = ParentMap.UnitGroups.Item(StartPos)
                Case "player"
                    If INIProperty.Value.ToLower <> "scavenger" Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Structures(INISectionNum).UnitGroup = ParentMap.ScavengerUnitGroup
                Case "position"
                    If Not WorldPosFromINIText(INIProperty.Value, Structures(INISectionNum).Pos) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "rotation"
                    If Not WZAngleFromINIText(INIProperty.Value, Structures(INISectionNum).Rotation) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "modules"
                    Dim ModuleCount As Integer
                    If Not InvariantParse_int(INIProperty.Value, ModuleCount) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If ModuleCount < 0 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Structures(INISectionNum).ModuleCount = ModuleCount
                Case "health"
                    If Not HealthFromINIText(INIProperty.Value, Structures(INISectionNum).HealthPercent) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "wall/type"
                    Dim WallType As Integer
                    If Not InvariantParse_int(INIProperty.Value, WallType) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If WallType < 0 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Structures(INISectionNum).WallType = WallType
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Class clsINIDroids
        Inherits clsINIRead.clsSectionTranslator

        Private ParentMap As clsMap

        Public Structure sDroid
            Public ID As UInteger
            Public Template As String
            Public UnitGroup As clsUnitGroup
            Public Pos As clsWorldPos
            Public Rotation As sWZAngle
            Public HealthPercent As Integer
            Public DroidType As Integer
            Public Body As String
            Public Propulsion As String
            Public Brain As String
            Public Repair As String
            Public ECM As String
            Public Sensor As String
            Public Construct As String
            Public Weapons() As String
            Public WeaponCount As Integer
        End Structure
        Public Droids() As sDroid
        Public DroidCount As Integer

        Public Sub New(NewDroidCount As Integer, NewParentMap As clsMap)
            Dim A As Integer

            ParentMap = NewParentMap

            DroidCount = NewDroidCount
            ReDim Droids(DroidCount - 1)
            For A = 0 To DroidCount - 1
                Droids(A).HealthPercent = -1
                Droids(A).DroidType = -1
                ReDim Droids(A).Weapons(2)
            Next
        End Sub

        Public Overrides Function Translate(INISectionNum As Integer, INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "id"
                    Dim uintTemp As UInteger
                    If InvariantParse_uint(INIProperty.Value, uintTemp) Then
                        If uintTemp > 0 Then
                            Droids(INISectionNum).ID = uintTemp
                        End If
                    Else
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "template"
                    Droids(INISectionNum).Template = INIProperty.Value
                Case "startpos"
                    Dim StartPos As Integer
                    If Not InvariantParse_int(INIProperty.Value, StartPos) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If StartPos < 0 Or StartPos >= PlayerCountMax Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Droids(INISectionNum).UnitGroup = ParentMap.UnitGroups.Item(StartPos)
                Case "player"
                    If INIProperty.Value.ToLower <> "scavenger" Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Droids(INISectionNum).UnitGroup = ParentMap.ScavengerUnitGroup
                Case "name"
                    'ignore
                Case "position"
                    If Not WorldPosFromINIText(INIProperty.Value, Droids(INISectionNum).Pos) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "rotation"
                    If Not WZAngleFromINIText(INIProperty.Value, Droids(INISectionNum).Rotation) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "health"
                    If Not HealthFromINIText(INIProperty.Value, Droids(INISectionNum).HealthPercent) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "droidtype"
                    If Not InvariantParse_int(INIProperty.Value, Droids(INISectionNum).DroidType) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "weapons"
                    If Not InvariantParse_int(INIProperty.Value, Droids(INISectionNum).WeaponCount) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "parts\body"
                    Droids(INISectionNum).Body = INIProperty.Value
                Case "parts\propulsion"
                    Droids(INISectionNum).Propulsion = INIProperty.Value
                Case "parts\brain"
                    Droids(INISectionNum).Brain = INIProperty.Value
                Case "parts\repair"
                    Droids(INISectionNum).Repair = INIProperty.Value
                Case "parts\ecm"
                    Droids(INISectionNum).ECM = INIProperty.Value
                Case "parts\sensor"
                    Droids(INISectionNum).Sensor = INIProperty.Value
                Case "parts\construct"
                    Droids(INISectionNum).Construct = INIProperty.Value
                Case "parts\weapon\1"
                    Droids(INISectionNum).Weapons(0) = INIProperty.Value
                Case "parts\weapon\2"
                    Droids(INISectionNum).Weapons(1) = INIProperty.Value
                Case "parts\weapon\3"
                    Droids(INISectionNum).Weapons(2) = INIProperty.Value
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Class clsINIFeatures
        Inherits clsINIRead.clsSectionTranslator

        Public Structure sFeatures
            Public ID As UInteger
            Public Code As String
            Public Pos As clsWorldPos
            Public Rotation As sWZAngle
            Public HealthPercent As Integer
        End Structure
        Public Features() As sFeatures
        Public FeatureCount As Integer

        Public Sub New(NewFeatureCount As Integer)
            Dim A As Integer

            FeatureCount = NewFeatureCount
            ReDim Features(FeatureCount - 1)
            For A = 0 To FeatureCount - 1
                Features(A).HealthPercent = -1
            Next
        End Sub

        Public Overrides Function Translate(INISectionNum As Integer, INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "id"
                    Dim uintTemp As UInteger
                    If InvariantParse_uint(INIProperty.Value, uintTemp) Then
                        If uintTemp > 0 Then
                            Features(INISectionNum).ID = uintTemp
                        End If
                    Else
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "name"
                    Features(INISectionNum).Code = INIProperty.Value
                Case "position"
                    If Not WorldPosFromINIText(INIProperty.Value, Features(INISectionNum).Pos) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "rotation"
                    If Not WZAngleFromINIText(INIProperty.Value, Features(INISectionNum).Rotation) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "health"
                    If Not HealthFromINIText(INIProperty.Value, Features(INISectionNum).HealthPercent) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Private Function Read_WZ_gam(File As IO.BinaryReader) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String
        Dim Version As UInteger

        Try

            strTemp = ReadOldTextOfLength(File, 4)
            If strTemp <> "game" Then
                ReturnResult.Problem = "Unknown game identifier."
                Return ReturnResult
            End If

            Version = File.ReadUInt32
            If Version <> 8UI Then
                If MsgBox("Game file version is unknown. Continue?", (MsgBoxStyle.OkCancel Or MsgBoxStyle.Question)) <> MsgBoxResult.Ok Then
                    ReturnResult.Problem = "Aborted."
                    Return ReturnResult
                End If
            End If

            If InterfaceOptions Is Nothing Then
                InterfaceOptions = New clsInterfaceOptions
            End If

            File.ReadInt32() 'game time
            InterfaceOptions.CampaignGameType = File.ReadInt32
            InterfaceOptions.AutoScrollLimits = False
            InterfaceOptions.ScrollMin.X = File.ReadInt32
            InterfaceOptions.ScrollMin.Y = File.ReadInt32
            InterfaceOptions.ScrollMax.X = File.ReadUInt32
            InterfaceOptions.ScrollMax.Y = File.ReadUInt32

        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_map(File As IO.BinaryReader) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim MapWidth As UInteger
        Dim MapHeight As UInteger
        Dim uintTemp As UInteger
        Dim Flip As Byte
        Dim FlipX As Boolean
        Dim FlipZ As Boolean
        Dim Rotate As Byte
        Dim TextureNum As Byte
        Dim A As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim PosA As sXY_int
        Dim PosB As sXY_int

        Try

            strTemp = ReadOldTextOfLength(File, 4)
            If strTemp <> "map " Then
                ReturnResult.Problem = "Unknown game.map identifier."
                Return ReturnResult
            End If

            Version = File.ReadUInt32
            If Version <> 10UI Then
                If MsgBox("game.map version is unknown. Continue?", (MsgBoxStyle.OkCancel Or MsgBoxStyle.Question)) <> MsgBoxResult.Ok Then
                    ReturnResult.Problem = "Aborted."
                    Return ReturnResult
                End If
            End If
            MapWidth = File.ReadUInt32
            MapHeight = File.ReadUInt32
            If MapWidth < 1UI Or MapWidth > MapMaxSize Or MapHeight < 1UI Or MapHeight > MapMaxSize Then
                ReturnResult.Problem = "Map size out of range."
                Return ReturnResult
            End If

            TerrainBlank(New sXY_int(CInt(MapWidth), CInt(MapHeight)))

            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    TextureNum = File.ReadByte
                    Terrain.Tiles(X, Y).Texture.TextureNum = TextureNum
                    Flip = File.ReadByte
                    Terrain.Vertices(X, Y).Height = File.ReadByte
                    'get flipx
                    A = CInt(Int(Flip / 128.0#))
                    Flip -= CByte(A * 128)
                    FlipX = (A = 1)
                    'get flipy
                    A = CInt(Int(Flip / 64.0#))
                    Flip -= CByte(A * 64)
                    FlipZ = (A = 1)
                    'get rotation
                    A = CInt(Int(Flip / 16.0#))
                    Flip -= CByte(A * 16)
                    Rotate = CByte(A)
                    OldOrientation_To_TileOrientation(Rotate, FlipX, FlipZ, Terrain.Tiles(X, Y).Texture.Orientation)
                    'get tri direction
                    A = CInt(Int(Flip / 8.0#))
                    Flip -= CByte(A * 8)
                    Terrain.Tiles(X, Y).Tri = (A = 1)
                Next
            Next

            If Version <> 2UI Then
                uintTemp = File.ReadUInt32
                If uintTemp <> 1 Then
                    ReturnResult.Problem = "Bad gateway version number."
                    Return ReturnResult
                End If

                uintTemp = File.ReadUInt32

                For A = 0 To CInt(uintTemp) - 1
                    PosA.X = File.ReadByte
                    PosA.Y = File.ReadByte
                    PosB.X = File.ReadByte
                    PosB.Y = File.ReadByte
                    If GatewayCreate(PosA, PosB) Is Nothing Then
                        ReturnResult.Problem = "Gateway placement error."
                        Return ReturnResult
                    End If
                Next
            End If
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_Features(File As IO.BinaryReader, WZUnits As SimpleClassList(Of clsWZBJOUnit)) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim uintTemp As UInteger
        Dim A As Integer
        Dim B As Integer
        Dim WZBJOUnit As clsMap.clsWZBJOUnit

        Try
            strTemp = ReadOldTextOfLength(File, 4)
            If strTemp <> "feat" Then
                ReturnResult.Problem = "Unknown feat.bjo identifier."
                Return ReturnResult
            End If

            Version = File.ReadUInt32
            If Version <> 8UI Then
                If MsgBox("feat.bjo version is unknown. Continue?", (MsgBoxStyle.OkCancel Or MsgBoxStyle.Question)) <> MsgBoxResult.Ok Then
                    ReturnResult.Problem = "Aborted."
                    Return ReturnResult
                End If
            End If

            uintTemp = File.ReadUInt32
            For A = 0 To CInt(uintTemp) - 1
                WZBJOUnit = New clsMap.clsWZBJOUnit
                WZBJOUnit.ObjectType = clsUnitType.enumType.Feature
                WZBJOUnit.Code = ReadOldTextOfLength(File, 40)
                B = Strings.InStr(WZBJOUnit.Code, Chr(0))
                If B > 0 Then
                    WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1)
                End If
                WZBJOUnit.ID = File.ReadUInt32
                WZBJOUnit.Pos.Horizontal.X = CInt(File.ReadUInt32)
                WZBJOUnit.Pos.Horizontal.Y = CInt(File.ReadUInt32)
                WZBJOUnit.Pos.Altitude = CInt(File.ReadUInt32)
                WZBJOUnit.Rotation = File.ReadUInt32
                WZBJOUnit.Player = File.ReadUInt32
                File.ReadBytes(12)
                WZUnits.Add(WZBJOUnit)
            Next
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_TileTypes(File As IO.BinaryReader) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim uintTemp As UInteger
        Dim ushortTemp As UShort
        Dim A As Integer

        Try
            strTemp = ReadOldTextOfLength(File, 4)
            If strTemp <> "ttyp" Then
                ReturnResult.Problem = "Unknown ttypes.ttp identifier."
                Return ReturnResult
            End If

            Version = File.ReadUInt32
            If Version <> 8UI Then
                'Load_WZ.Problem = "Unknown ttypes.ttp version."
                'Exit Function
                If MsgBox("ttypes.ttp version is unknown. Continue?", (MsgBoxStyle.OkCancel Or MsgBoxStyle.Question)) <> MsgBoxResult.Ok Then
                    ReturnResult.Problem = "Aborted."
                    Return ReturnResult
                End If
            End If

            uintTemp = File.ReadUInt32

            If Tileset IsNot Nothing Then
                For A = 0 To Math.Min(CInt(uintTemp), Tileset.TileCount) - 1
                    ushortTemp = File.ReadUInt16
                    If ushortTemp > 11US Then
                        ReturnResult.Problem = "Unknown tile type."
                        Return ReturnResult
                    End If
                    Tile_TypeNum(A) = CByte(ushortTemp)
                Next
            End If
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_Structures(File As IO.BinaryReader, ByRef WZUnits As SimpleClassList(Of clsWZBJOUnit)) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim uintTemp As UInteger
        Dim A As Integer
        Dim B As Integer
        Dim WZBJOUnit As clsMap.clsWZBJOUnit

        Try
            strTemp = ReadOldTextOfLength(File, 4)
            If strTemp <> "stru" Then
                ReturnResult.Problem = "Unknown struct.bjo identifier."
                Return ReturnResult
            End If

            Version = File.ReadUInt32
            If Version <> 8UI Then
                If MsgBox("struct.bjo version is unknown. Continue?", (MsgBoxStyle.OkCancel Or MsgBoxStyle.Question)) <> MsgBoxResult.Ok Then
                    ReturnResult.Problem = "Aborted."
                    Return ReturnResult
                End If
            End If

            uintTemp = File.ReadUInt32
            For A = 0 To CInt(uintTemp) - 1
                WZBJOUnit = New clsMap.clsWZBJOUnit
                WZBJOUnit.ObjectType = clsUnitType.enumType.PlayerStructure
                WZBJOUnit.Code = ReadOldTextOfLength(File, 40)
                B = Strings.InStr(WZBJOUnit.Code, Chr(0))
                If B > 0 Then
                    WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1)
                End If
                WZBJOUnit.ID = File.ReadUInt32
                WZBJOUnit.Pos.Horizontal.X = CInt(File.ReadUInt32)
                WZBJOUnit.Pos.Horizontal.Y = CInt(File.ReadUInt32)
                WZBJOUnit.Pos.Altitude = CInt(File.ReadUInt32)
                WZBJOUnit.Rotation = File.ReadUInt32
                WZBJOUnit.Player = File.ReadUInt32
                File.ReadBytes(56)
                WZUnits.Add(WZBJOUnit)
            Next
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_Droids(File As IO.BinaryReader, WZUnits As SimpleClassList(Of clsWZBJOUnit)) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim uintTemp As UInteger
        Dim A As Integer
        Dim B As Integer
        Dim WZBJOUnit As clsWZBJOUnit

        Try
            strTemp = ReadOldTextOfLength(File, 4)
            If strTemp <> "dint" Then
                ReturnResult.Problem = "Unknown dinit.bjo identifier."
                Return ReturnResult
            End If

            Version = File.ReadUInt32
            If Version > 19UI Then
                If MsgBox("dinit.bjo version is unknown. Continue?", (MsgBoxStyle.OkCancel Or MsgBoxStyle.Question)) <> MsgBoxResult.Ok Then
                    ReturnResult.Problem = "Aborted."
                    Return ReturnResult
                End If
            End If

            uintTemp = File.ReadUInt32
            For A = 0 To CInt(uintTemp) - 1
                WZBJOUnit = New clsWZBJOUnit
                WZBJOUnit.ObjectType = clsUnitType.enumType.PlayerDroid
                WZBJOUnit.Code = ReadOldTextOfLength(File, 40)
                B = Strings.InStr(WZBJOUnit.Code, Chr(0))
                If B > 0 Then
                    WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1)
                End If
                WZBJOUnit.ID = File.ReadUInt32
                WZBJOUnit.Pos.Horizontal.X = CInt(File.ReadUInt32)
                WZBJOUnit.Pos.Horizontal.Y = CInt(File.ReadUInt32)
                WZBJOUnit.Pos.Altitude = CInt(File.ReadUInt32)
                WZBJOUnit.Rotation = File.ReadUInt32
                WZBJOUnit.Player = File.ReadUInt32
                File.ReadBytes(12)
                WZUnits.Add(WZBJOUnit)
            Next
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function Read_WZ_Labels(INI As clsINIRead, IsFMap As Boolean) As clsResult
        Dim ReturnResult As New clsResult("Reading labels")

        Dim CharNum As Integer
        Dim PositionsA As clsPositionFromText
        Dim PositionsB As clsPositionFromText
        Dim TypeNum As Integer
        Dim NewPosition As clsMap.clsScriptPosition
        Dim NewArea As clsMap.clsScriptArea
        Dim NameText As String
        Dim strLabel As String
        Dim strPosA As String
        Dim strPosB As String
        Dim IDText As String
        Dim IDNum As UInteger

        Dim FailedCount As Integer = 0
        Dim ModifiedCount As Integer = 0

        Dim INISection As clsINIRead.clsSection
        For Each INISection In INI.Sections
            NameText = INISection.Name
            CharNum = NameText.IndexOf("_"c)
            NameText = Strings.Left(NameText, CharNum)
            Select Case NameText
                Case "position"
                    TypeNum = 0
                Case "area"
                    TypeNum = 1
                Case "object"
                    If IsFMap Then
                        TypeNum = Integer.MaxValue
                        FailedCount += 1
                        Continue For
                    Else
                        TypeNum = 2
                    End If
                Case Else
                    TypeNum = Integer.MaxValue
                    FailedCount += 1
                    Continue For
            End Select
            strLabel = INISection.GetLastPropertyValue("label")
            If strLabel Is Nothing Then
                FailedCount += 1
                Continue For
            End If
            strLabel = strLabel.Replace(CStr(ControlChars.Quote), "")
            Select Case TypeNum
                Case 0 'position
                    strPosA = INISection.GetLastPropertyValue("pos")
                    If strPosA Is Nothing Then
                        FailedCount += 1
                        Continue For
                    End If
                    PositionsA = New clsPositionFromText
                    If PositionsA.Translate(strPosA) Then
                        NewPosition = clsMap.clsScriptPosition.Create(Me)
                        NewPosition.PosX = PositionsA.Pos.X
                        NewPosition.PosY = PositionsA.Pos.Y
                        NewPosition.SetLabel(strLabel)
                        If NewPosition.Label <> strLabel Or NewPosition.PosX <> PositionsA.Pos.X Or NewPosition.PosY <> PositionsA.Pos.Y Then
                            ModifiedCount += 1
                        End If
                    Else
                        FailedCount += 1
                        Continue For
                    End If
                Case 1 'area
                    strPosA = INISection.GetLastPropertyValue("pos1")
                    If strPosA Is Nothing Then
                        FailedCount += 1
                        Continue For
                    End If
                    strPosB = INISection.GetLastPropertyValue("pos2")
                    If strPosB Is Nothing Then
                        FailedCount += 1
                        Continue For
                    End If
                    PositionsA = New clsPositionFromText
                    PositionsB = New clsPositionFromText
                    If PositionsA.Translate(strPosA) And PositionsB.Translate(strPosB) Then
                        NewArea = clsMap.clsScriptArea.Create(Me)
                        NewArea.SetPositions(PositionsA.Pos, PositionsB.Pos)
                        NewArea.SetLabel(strLabel)
                        If NewArea.Label <> strLabel Or NewArea.PosAX <> PositionsA.Pos.X Or NewArea.PosAY <> PositionsA.Pos.Y _
                            Or NewArea.PosBX <> PositionsB.Pos.X Or NewArea.PosBY <> PositionsB.Pos.Y Then
                            ModifiedCount += 1
                        End If
                    Else
                        FailedCount += 1
                        Continue For
                    End If
                Case 2 'object
                    IDText = INISection.GetLastPropertyValue("id")
                    If InvariantParse_uint(IDText, IDNum) Then
                        Dim Unit As clsUnit = IDUsage(IDNum)
                        If Unit IsNot Nothing Then
                            If Not Unit.SetLabel(strLabel).Success Then
                                FailedCount += 1
                                Continue For
                            End If
                        Else
                            FailedCount += 1
                            Continue For
                        End If
                    End If
                Case Else
                    ReturnResult.WarningAdd("Error! Bad type number for script label.")
            End Select
        Next

        If FailedCount > 0 Then
            ReturnResult.WarningAdd("Unable to translate " & FailedCount & " script labels.")
        End If
        If ModifiedCount > 0 Then
            ReturnResult.WarningAdd(ModifiedCount & " script labels had invalid values and were modified.")
        End If

        Return ReturnResult
    End Function

    Public Function Serialize_WZ_StructuresINI(File As clsINIWrite, PlayerCount As Integer) As clsResult
        Dim ReturnResult As New clsResult("Serializing structures INI")

        Dim StructureType As clsStructureType
        Dim Unit As clsUnit
        Dim UnitIsModule(Units.Count - 1) As Boolean
        Dim UnitModuleCount(Units.Count - 1) As Integer
        Dim SectorNum As sXY_int
        Dim OtherStructureType As clsStructureType
        Dim OtherUnit As clsUnit
        Dim ModuleMin As sXY_int
        Dim ModuleMax As sXY_int
        Dim Footprint As sXY_int
        Dim A As Integer
        Dim UnderneathTypes(1) As clsStructureType.enumStructureType
        Dim UnderneathTypeCount As Integer
        Dim BadModuleCount As Integer = 0
        Dim PriorityOrder As New clsObjectPriorityOrderList

        For Each Unit In Units
            If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                StructureType = CType(Unit.Type, clsStructureType)
                Select Case StructureType.StructureType
                    Case clsStructureType.enumStructureType.FactoryModule
                        UnderneathTypes(0) = clsStructureType.enumStructureType.Factory
                        UnderneathTypes(1) = clsStructureType.enumStructureType.VTOLFactory
                        UnderneathTypeCount = 2
                    Case clsStructureType.enumStructureType.PowerModule
                        UnderneathTypes(0) = clsStructureType.enumStructureType.PowerGenerator
                        UnderneathTypeCount = 1
                    Case clsStructureType.enumStructureType.ResearchModule
                        UnderneathTypes(0) = clsStructureType.enumStructureType.Research
                        UnderneathTypeCount = 1
                    Case Else
                        UnderneathTypeCount = 0
                End Select
                If UnderneathTypeCount = 0 Then
                    PriorityOrder.SetItem(Unit)
                    PriorityOrder.ActionPerform()
                Else
                    UnitIsModule(Unit.MapLink.ArrayPosition) = True
                    SectorNum = GetPosSectorNum(Unit.Pos.Horizontal)
                    Dim Underneath As clsUnit = Nothing
                    Dim Connection As clsUnitSectorConnection
                    For Each Connection In Sectors(SectorNum.X, SectorNum.Y).Units
                        OtherUnit = Connection.Unit
                        If OtherUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                            OtherStructureType = CType(OtherUnit.Type, clsStructureType)
                            If OtherUnit.UnitGroup Is Unit.UnitGroup Then
                                For A = 0 To UnderneathTypeCount - 1
                                    If OtherStructureType.StructureType = UnderneathTypes(A) Then
                                        Exit For
                                    End If
                                Next
                                If A < UnderneathTypeCount Then
                                    Footprint = OtherStructureType.GetFootprintSelected(OtherUnit.Rotation)
                                    ModuleMin.X = OtherUnit.Pos.Horizontal.X - CInt(Footprint.X * TerrainGridSpacing / 2.0#)
                                    ModuleMin.Y = OtherUnit.Pos.Horizontal.Y - CInt(Footprint.Y * TerrainGridSpacing / 2.0#)
                                    ModuleMax.X = OtherUnit.Pos.Horizontal.X + CInt(Footprint.X * TerrainGridSpacing / 2.0#)
                                    ModuleMax.Y = OtherUnit.Pos.Horizontal.Y + CInt(Footprint.Y * TerrainGridSpacing / 2.0#)
                                    If Unit.Pos.Horizontal.X >= ModuleMin.X And Unit.Pos.Horizontal.X < ModuleMax.X And _
                                      Unit.Pos.Horizontal.Y >= ModuleMin.Y And Unit.Pos.Horizontal.Y < ModuleMax.Y Then
                                        UnitModuleCount(OtherUnit.MapLink.ArrayPosition) += 1
                                        Underneath = OtherUnit
                                        Exit For
                                    End If
                                End If
                            End If
                        End If
                    Next
                    If Underneath Is Nothing Then
                        BadModuleCount += 1
                    End If
                End If
            End If
        Next

        If BadModuleCount > 0 Then
            ReturnResult.WarningAdd(BadModuleCount & " modules had no underlying structure.")
        End If

        Dim TooManyModulesWarningCount As Integer
        Dim TooManyModulesWarningMaxCount As Integer = 16
        Dim ModuleCount As Integer
        Dim ModuleLimit As Integer

        For A = 0 To PriorityOrder.Result.Count - 1
            Unit = PriorityOrder.Result.Item(A)
            StructureType = CType(Unit.Type, clsStructureType)
            If Unit.ID <= 0 Then
                ReturnResult.WarningAdd("Error. A structure's ID was zero. It was NOT saved. Delete and replace it to allow save.")
            Else
                File.SectionName_Append("structure_" & InvariantToString_uint(Unit.ID))
                File.Property_Append("id", InvariantToString_uint(Unit.ID))
                If Unit.UnitGroup Is ScavengerUnitGroup Or (PlayerCount >= 0 And Unit.UnitGroup.WZ_StartPos >= PlayerCount) Then
                    File.Property_Append("player", "scavenger")
                Else
                    File.Property_Append("startpos", InvariantToString_int(Unit.UnitGroup.WZ_StartPos))
                End If
                File.Property_Append("name", StructureType.Code)
                If StructureType.WallLink.IsConnected Then
                    File.Property_Append("wall/type", InvariantToString_int(StructureType.WallLink.ArrayPosition))
                End If
                File.Property_Append("position", Unit.GetINIPosition)
                File.Property_Append("rotation", Unit.GetINIRotation)
                If Unit.Health < 1.0# Then
                    File.Property_Append("health", Unit.GetINIHealthPercent)
                End If
                Select Case StructureType.StructureType
                    Case clsStructureType.enumStructureType.Factory
                        ModuleLimit = 2
                    Case clsStructureType.enumStructureType.VTOLFactory
                        ModuleLimit = 2
                    Case clsStructureType.enumStructureType.PowerGenerator
                        ModuleLimit = 1
                    Case clsStructureType.enumStructureType.Research
                        ModuleLimit = 1
                    Case Else
                        ModuleLimit = 0
                End Select
                If UnitModuleCount(Unit.MapLink.ArrayPosition) > ModuleLimit Then
                    ModuleCount = ModuleLimit
                    If TooManyModulesWarningCount < TooManyModulesWarningMaxCount Then
                        ReturnResult.WarningAdd("Structure " & StructureType.GetDisplayTextCode & " at " & Unit.GetPosText & " has too many modules (" & UnitModuleCount(Unit.MapLink.ArrayPosition) & ").")
                    End If
                    TooManyModulesWarningCount += 1
                Else
                    ModuleCount = UnitModuleCount(Unit.MapLink.ArrayPosition)
                End If
                File.Property_Append("modules", InvariantToString_int(ModuleCount))
                File.Gap_Append()
            End If
        Next

        If TooManyModulesWarningCount > TooManyModulesWarningMaxCount Then
            ReturnResult.WarningAdd(TooManyModulesWarningCount & " structures had too many modules.")
        End If

        Return ReturnResult
    End Function

    Public Function Serialize_WZ_DroidsINI(File As clsINIWrite, PlayerCount As Integer) As clsResult
        Dim ReturnResult As New clsResult("Serializing droids INI")

        Dim Droid As clsDroidDesign
        Dim Template As clsDroidTemplate
        Dim Text As String
        Dim Unit As clsUnit
        Dim AsPartsNotTemplate As Boolean
        Dim ValidDroid As Boolean
        Dim InvalidPartCount As Integer = 0
        Dim Brain As clsBrain

        For Each Unit In Units
            If Unit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                Droid = CType(Unit.Type, clsDroidDesign)
                ValidDroid = True
                If Unit.ID <= 0 Then
                    ValidDroid = False
                    ReturnResult.WarningAdd("Error. A droid's ID was zero. It was NOT saved. Delete and replace it to allow save.")
                End If
                If Droid.IsTemplate Then
                    Template = CType(Droid, clsDroidTemplate)
                    AsPartsNotTemplate = Unit.PreferPartsOutput
                Else
                    Template = Nothing
                    AsPartsNotTemplate = True
                End If
                If AsPartsNotTemplate Then
                    If Droid.Body Is Nothing Then
                        ValidDroid = False
                        InvalidPartCount += 1
                    ElseIf Droid.Propulsion Is Nothing Then
                        ValidDroid = False
                        InvalidPartCount += 1
                    ElseIf Droid.TurretCount >= 1 Then
                        If Droid.Turret1 Is Nothing Then
                            ValidDroid = False
                            InvalidPartCount += 1
                        End If
                    ElseIf Droid.TurretCount >= 2 Then
                        If Droid.Turret2 Is Nothing Then
                            ValidDroid = False
                            InvalidPartCount += 1
                        ElseIf Droid.Turret2.TurretType <> clsTurret.enumTurretType.Weapon Then
                            ValidDroid = False
                            InvalidPartCount += 1
                        End If
                    ElseIf Droid.TurretCount >= 3 And Droid.Turret3 Is Nothing Then
                        If Droid.Turret3 Is Nothing Then
                            ValidDroid = False
                            InvalidPartCount += 1
                        ElseIf Droid.Turret3.TurretType <> clsTurret.enumTurretType.Weapon Then
                            ValidDroid = False
                            InvalidPartCount += 1
                        End If
                    End If
                End If
                If ValidDroid Then
                    File.SectionName_Append("droid_" & InvariantToString_uint(Unit.ID))
                    File.Property_Append("id", InvariantToString_uint(Unit.ID))
                    If Unit.UnitGroup Is ScavengerUnitGroup Or (PlayerCount >= 0 And Unit.UnitGroup.WZ_StartPos >= PlayerCount) Then
                        File.Property_Append("player", "scavenger")
                    Else
                        File.Property_Append("startpos", InvariantToString_int(Unit.UnitGroup.WZ_StartPos))
                    End If
                    If AsPartsNotTemplate Then
                        File.Property_Append("name", Droid.GenerateName)
                    Else
                        Template = CType(Droid, clsDroidTemplate)
                        File.Property_Append("template", Template.Code)
                    End If
                    File.Property_Append("position", Unit.GetINIPosition)
                    File.Property_Append("rotation", Unit.GetINIRotation)
                    If Unit.Health < 1.0# Then
                        File.Property_Append("health", Unit.GetINIHealthPercent)
                    End If
                    If AsPartsNotTemplate Then
                        File.Property_Append("droidType", InvariantToString_int(CInt(Droid.GetDroidType)))
                        If Droid.TurretCount = 0 Then
                            Text = "0"
                        Else
                            If Droid.Turret1.TurretType = clsTurret.enumTurretType.Brain Then
                                If CType(Droid.Turret1, clsBrain).Weapon Is Nothing Then
                                    Text = "0"
                                Else
                                    Text = "1"
                                End If
                            Else
                                If Droid.Turret1.TurretType = clsTurret.enumTurretType.Weapon Then
                                    Text = InvariantToString_byte(Droid.TurretCount)
                                Else
                                    Text = "0"
                                End If
                            End If
                        End If
                        File.Property_Append("weapons", Text)
                        File.Property_Append("parts\body", Droid.Body.Code)
                        File.Property_Append("parts\propulsion", Droid.Propulsion.Code)
                        File.Property_Append("parts\sensor", Droid.GetSensorCode)
                        File.Property_Append("parts\construct", Droid.GetConstructCode)
                        File.Property_Append("parts\repair", Droid.GetRepairCode)
                        File.Property_Append("parts\brain", Droid.GetBrainCode)
                        File.Property_Append("parts\ecm", Droid.GetECMCode)
                        If Droid.TurretCount >= 1 Then
                            If Droid.Turret1.TurretType = clsTurret.enumTurretType.Weapon Then
                                File.Property_Append("parts\weapon\1", Droid.Turret1.Code)
                                If Droid.TurretCount >= 2 Then
                                    If Droid.Turret2.TurretType = clsTurret.enumTurretType.Weapon Then
                                        File.Property_Append("parts\weapon\2", Droid.Turret2.Code)
                                        If Droid.TurretCount >= 3 Then
                                            If Droid.Turret3.TurretType = clsTurret.enumTurretType.Weapon Then
                                                File.Property_Append("parts\weapon\3", Droid.Turret3.Code)
                                            End If
                                        End If
                                    End If
                                End If
                            ElseIf Droid.Turret1.TurretType = clsTurret.enumTurretType.Brain Then
                                Brain = CType(Droid.Turret1, clsBrain)
                                If Brain.Weapon Is Nothing Then
                                    Text = "ZNULLWEAPON"
                                Else
                                    Text = Brain.Weapon.Code
                                End If
                                File.Property_Append("parts\weapon\1", Text)
                            End If
                        End If
                    End If
                    File.Gap_Append()
                End If
            End If
        Next

        If InvalidPartCount > 0 Then
            ReturnResult.WarningAdd("There were " & InvalidPartCount & " droids with parts missing. They were not saved.")
        End If

        Return ReturnResult
    End Function

    Public Function Serialize_WZ_FeaturesINI(File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult("Serializing features INI")
        Dim FeatureType As clsFeatureType
        Dim Unit As clsUnit
        Dim Valid As Boolean

        For Each Unit In Units
            If Unit.Type.Type = clsUnitType.enumType.Feature Then
                FeatureType = CType(Unit.Type, clsFeatureType)
                Valid = True
                If Unit.ID <= 0 Then
                    Valid = False
                    ReturnResult.WarningAdd("Error. A features's ID was zero. It was NOT saved. Delete and replace it to allow save.")
                End If
                If Valid Then
                    File.SectionName_Append("feature_" & InvariantToString_uint(Unit.ID))
                    File.Property_Append("id", InvariantToString_uint(Unit.ID))
                    File.Property_Append("position", Unit.GetINIPosition)
                    File.Property_Append("rotation", Unit.GetINIRotation)
                    File.Property_Append("name", FeatureType.Code)
                    If Unit.Health < 1.0# Then
                        File.Property_Append("health", Unit.GetINIHealthPercent)
                    End If
                    File.Gap_Append()
                End If
            End If
        Next

        Return ReturnResult
    End Function

    Public Function Serialize_WZ_LabelsINI(File As clsINIWrite, PlayerCount As Integer) As clsResult
        Dim ReturnResult As New clsResult("Serializing labels INI")

        Try
            Dim ScriptPosition As clsScriptPosition
            For Each ScriptPosition In ScriptPositions
                ScriptPosition.WriteWZ(File)
            Next
            Dim ScriptArea As clsScriptArea
            For Each ScriptArea In ScriptAreas
                ScriptArea.WriteWZ(File)
            Next
            If PlayerCount >= 0 Then 'not an FMap
                Dim Unit As clsUnit
                For Each Unit In Units
                    Unit.WriteWZLabel(File, PlayerCount)
                Next
            End If
        Catch ex As Exception
            ReturnResult.WarningAdd(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Structure sWrite_WZ_Args
        Public Path As String
        Public Overwrite As Boolean
        Public MapName As String
        Public Class clsMultiplayer
            Public PlayerCount As Integer
            Public AuthorName As String
            Public License As String
            Public IsBetaPlayerFormat As Boolean
        End Class
        Public Multiplayer As clsMultiplayer
        Public Class clsCampaign
            'Public GAMTime As UInteger
            Public GAMType As UInteger
        End Class
        Public Campaign As clsCampaign
        Enum enumCompileType As Byte
            Unspecified
            Multiplayer
            Campaign
        End Enum
        Public ScrollMin As sXY_int
        Public ScrollMax As sXY_uint
        Public CompileType As enumCompileType
    End Structure

    Public Function Write_WZ(Args As sWrite_WZ_Args) As clsResult
        Dim ReturnResult As New clsResult("Compiling to " & ControlChars.Quote & Args.Path & ControlChars.Quote)

        Try

            Select Case Args.CompileType
                Case sWrite_WZ_Args.enumCompileType.Multiplayer
                    If Args.Multiplayer Is Nothing Then
                        ReturnResult.ProblemAdd("Multiplayer arguments were not passed.")
                        Return ReturnResult
                    End If
                    If Args.Multiplayer.PlayerCount < 2 Or Args.Multiplayer.PlayerCount > 10 Then
                        ReturnResult.ProblemAdd("Number of players was below 2 or above 10.")
                        Return ReturnResult
                    End If
                    If Not Args.Multiplayer.IsBetaPlayerFormat Then
                        If Not (Args.Multiplayer.PlayerCount = 2 Or Args.Multiplayer.PlayerCount = 4 Or Args.Multiplayer.PlayerCount = 8) Then
                            ReturnResult.ProblemAdd("Number of players was not 2, 4 or 8 in original format.")
                            Return ReturnResult
                        End If
                    End If
                Case sWrite_WZ_Args.enumCompileType.Campaign
                    If Args.Campaign Is Nothing Then
                        ReturnResult.ProblemAdd("Campaign arguments were not passed.")
                        Return ReturnResult
                    End If
                Case Else
                    ReturnResult.ProblemAdd("Unknown compile method.")
                    Return ReturnResult
            End Select

            If Not Args.Overwrite Then
                If IO.File.Exists(Args.Path) Then
                    ReturnResult.ProblemAdd("The selected file already exists.")
                    Return ReturnResult
                End If
            End If

            Dim Quote As Char = ControlChars.Quote
            Dim EndChar As Char = Chr(10)
            Dim Text As String

            Dim File_LEV_Memory As New IO.MemoryStream
            Dim File_LEV As New IO.StreamWriter(File_LEV_Memory, UTF8Encoding)
            Dim File_MAP_Memory As New IO.MemoryStream
            Dim File_MAP As New IO.BinaryWriter(File_MAP_Memory, ASCIIEncoding)
            Dim File_GAM_Memory As New IO.MemoryStream
            Dim File_GAM As New IO.BinaryWriter(File_GAM_Memory, ASCIIEncoding)
            Dim File_featBJO_Memory As New IO.MemoryStream
            Dim File_featBJO As New IO.BinaryWriter(File_featBJO_Memory, ASCIIEncoding)
            Dim INI_feature_Memory As New IO.MemoryStream
            Dim INI_feature As clsINIWrite = clsINIWrite.CreateFile(INI_feature_Memory)
            Dim File_TTP_Memory As New IO.MemoryStream
            Dim File_TTP As New IO.BinaryWriter(File_TTP_Memory, ASCIIEncoding)
            Dim File_structBJO_Memory As New IO.MemoryStream
            Dim File_structBJO As New IO.BinaryWriter(File_structBJO_Memory, ASCIIEncoding)
            Dim INI_struct_Memory As New IO.MemoryStream
            Dim INI_struct As clsINIWrite = clsINIWrite.CreateFile(INI_struct_Memory)
            Dim File_droidBJO_Memory As New IO.MemoryStream
            Dim File_droidBJO As New IO.BinaryWriter(File_droidBJO_Memory, ASCIIEncoding)
            Dim INI_droid_Memory As New IO.MemoryStream
            Dim INI_droid As clsINIWrite = clsINIWrite.CreateFile(INI_droid_Memory)
            Dim INI_Labels_Memory As New IO.MemoryStream
            Dim INI_Labels As clsINIWrite = clsINIWrite.CreateFile(INI_Labels_Memory)

            Dim PlayersPrefix As String = ""
            Dim PlayersText As String = ""

            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then

                PlayersText = InvariantToString_int(Args.Multiplayer.PlayerCount)
                PlayersPrefix = PlayersText & "c-"
                Dim fog As String
                Dim TilesetNum As String
                If Tileset Is Nothing Then
                    ReturnResult.ProblemAdd("Map must have a tileset.")
                    Return ReturnResult
                ElseIf Tileset Is Tileset_Arizona Then
                    fog = "fog1.wrf"
                    TilesetNum = "1"
                ElseIf Tileset Is Tileset_Urban Then
                    fog = "fog2.wrf"
                    TilesetNum = "2"
                ElseIf Tileset Is Tileset_Rockies Then
                    fog = "fog3.wrf"
                    TilesetNum = "3"
                Else
                    ReturnResult.ProblemAdd("Unknown tileset selected.")
                    Return ReturnResult
                End If

                Text = "// Made with " & ProgramName & " " & ProgramVersionNumber & " " & ProgramPlatform & EndChar
                File_LEV.Write(Text)
                Dim DateNow As Date = Now
                Text = "// Date: " & DateNow.Year & "/" & MinDigits(DateNow.Month, 2) & "/" & MinDigits(DateNow.Day, 2) & " " & MinDigits(DateNow.Hour, 2) & ":" & MinDigits(DateNow.Minute, 2) & ":" & MinDigits(DateNow.Second, 2) & EndChar
                File_LEV.Write(Text)
                Text = "// Author: " & Args.Multiplayer.AuthorName & EndChar
                File_LEV.Write(Text)
                Text = "// License: " & Args.Multiplayer.License & EndChar
                File_LEV.Write(Text)
                Text = EndChar
                File_LEV.Write(Text)
                Text = "level   " & Args.MapName & "-T1" & EndChar
                File_LEV.Write(Text)
                Text = "players " & PlayersText & EndChar
                File_LEV.Write(Text)
                Text = "type    14" & EndChar
                File_LEV.Write(Text)
                Text = "dataset MULTI_CAM_" & TilesetNum & EndChar
                File_LEV.Write(Text)
                Text = "game    " & Quote & "multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam" & Quote & EndChar
                File_LEV.Write(Text)
                Text = "data    " & Quote & "wrf/multi/skirmish" & PlayersText & ".wrf" & Quote & EndChar
                File_LEV.Write(Text)
                Text = "data    " & Quote & "wrf/multi/" & fog & Quote & EndChar
                File_LEV.Write(Text)
                Text = EndChar
                File_LEV.Write(Text)
                Text = "level   " & Args.MapName & "-T2" & EndChar
                File_LEV.Write(Text)
                Text = "players " & PlayersText & EndChar
                File_LEV.Write(Text)
                Text = "type    18" & EndChar
                File_LEV.Write(Text)
                Text = "dataset MULTI_T2_C" & TilesetNum & EndChar
                File_LEV.Write(Text)
                Text = "game    " & Quote & "multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam" & Quote & EndChar
                File_LEV.Write(Text)
                Text = "data    " & Quote & "wrf/multi/t2-skirmish" & PlayersText & ".wrf" & Quote & EndChar
                File_LEV.Write(Text)
                Text = "data    " & Quote & "wrf/multi/" & fog & Quote & EndChar
                File_LEV.Write(Text)
                Text = EndChar
                File_LEV.Write(Text)
                Text = "level   " & Args.MapName & "-T3" & EndChar
                File_LEV.Write(Text)
                Text = "players " & PlayersText & EndChar
                File_LEV.Write(Text)
                Text = "type    19" & EndChar
                File_LEV.Write(Text)
                Text = "dataset MULTI_T3_C" & TilesetNum & EndChar
                File_LEV.Write(Text)
                Text = "game    " & Quote & "multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam" & Quote & EndChar
                File_LEV.Write(Text)
                Text = "data    " & Quote & "wrf/multi/t3-skirmish" & PlayersText & ".wrf" & Quote & EndChar
                File_LEV.Write(Text)
                Text = "data    " & Quote & "wrf/multi/" & fog & Quote & EndChar
                File_LEV.Write(Text)
            End If

            Dim GameZeroBytes(19) As Byte

            WriteText(File_GAM, False, "game")
            File_GAM.Write(8UI)
            File_GAM.Write(0UI) 'Time
            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                File_GAM.Write(0UI)
            ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then
                File_GAM.Write(Args.Campaign.GAMType)
            End If
            File_GAM.Write(Args.ScrollMin.X)
            File_GAM.Write(Args.ScrollMin.Y)
            File_GAM.Write(Args.ScrollMax.X)
            File_GAM.Write(Args.ScrollMax.Y)
            File_GAM.Write(GameZeroBytes)

            Dim A As Integer
            Dim X As Integer
            Dim Y As Integer

            WriteText(File_MAP, False, "map ")
            File_MAP.Write(10UI)
            File_MAP.Write(CUInt(Terrain.TileSize.X))
            File_MAP.Write(CUInt(Terrain.TileSize.Y))
            Dim Flip As Byte
            Dim Rotation As Byte
            Dim DoFlipX As Boolean
            Dim InvalidTileCount As Integer
            Dim TextureNum As Integer
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    TileOrientation_To_OldOrientation(Terrain.Tiles(X, Y).Texture.Orientation, Rotation, DoFlipX)
                    Flip = 0
                    If Terrain.Tiles(X, Y).Tri Then
                        Flip += CByte(8)
                    End If
                    Flip += CByte(Rotation * 16)
                    If DoFlipX Then
                        Flip += CByte(128)
                    End If
                    TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum
                    If TextureNum < 0 Or TextureNum > 255 Then
                        TextureNum = 0
                        If InvalidTileCount < 16 Then
                            ReturnResult.WarningAdd("Tile texture number " & Terrain.Tiles(X, Y).Texture.TextureNum & " is invalid on tile " & X & ", " & Y & " and was compiled as texture number " & TextureNum & ".")
                        End If
                        InvalidTileCount += 1
                    End If
                    File_MAP.Write(CByte(TextureNum))
                    File_MAP.Write(Flip)
                    File_MAP.Write(Terrain.Vertices(X, Y).Height)
                Next
            Next
            If InvalidTileCount > 0 Then
                ReturnResult.WarningAdd(InvalidTileCount & " tile texture numbers were invalid.")
            End If
            File_MAP.Write(1UI) 'gateway version
            File_MAP.Write(CUInt(Gateways.Count))
            Dim Gateway As clsGateway
            For Each Gateway In Gateways
                File_MAP.Write(CByte(Clamp_int(Gateway.PosA.X, 0, 255)))
                File_MAP.Write(CByte(Clamp_int(Gateway.PosA.Y, 0, 255)))
                File_MAP.Write(CByte(Clamp_int(Gateway.PosB.X, 0, 255)))
                File_MAP.Write(CByte(Clamp_int(Gateway.PosB.Y, 0, 255)))
            Next

            Dim FeatureType As clsFeatureType
            Dim StructureType As clsStructureType
            Dim DroidType As clsDroidDesign
            Dim DroidTemplate As clsDroidTemplate
            Dim Unit As clsMap.clsUnit
            Dim StructureWrite As New clsMap.clsStructureWriteWZ
            StructureWrite.File = File_structBJO
            StructureWrite.CompileType = Args.CompileType
            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                StructureWrite.PlayerCount = Args.Multiplayer.PlayerCount
            Else
                StructureWrite.PlayerCount = 0
            End If

            Dim FeatZeroBytes(11) As Byte

            WriteText(File_featBJO, False, "feat")
            File_featBJO.Write(8UI)
            Dim FeatureOrder As New clsObjectPriorityOrderList
            For Each Unit In Units
                If Unit.Type.Type = clsUnitType.enumType.Feature Then
                    FeatureOrder.SetItem(Unit)
                    FeatureOrder.ActionPerform()
                End If
            Next
            File_featBJO.Write(CUInt(FeatureOrder.Result.Count))
            For A = 0 To FeatureOrder.Result.Count - 1
                Unit = FeatureOrder.Result.Item(A)
                FeatureType = CType(Unit.Type, clsFeatureType)
                WriteTextOfLength(File_featBJO, 40, FeatureType.Code)
                File_featBJO.Write(Unit.ID)
                File_featBJO.Write(CUInt(Unit.Pos.Horizontal.X))
                File_featBJO.Write(CUInt(Unit.Pos.Horizontal.Y))
                File_featBJO.Write(CUInt(Unit.Pos.Altitude))
                File_featBJO.Write(CUInt(Unit.Rotation))
                Select Case Args.CompileType
                    Case sWrite_WZ_Args.enumCompileType.Multiplayer
                        File_featBJO.Write(Unit.GetBJOMultiplayerPlayerNum(Args.Multiplayer.PlayerCount))
                    Case sWrite_WZ_Args.enumCompileType.Campaign
                        File_featBJO.Write(Unit.GetBJOCampaignPlayerNum)
                    Case Else
                        Stop
                End Select
                File_featBJO.Write(FeatZeroBytes)
            Next

            WriteText(File_TTP, False, "ttyp")
            File_TTP.Write(8UI)
            File_TTP.Write(CUInt(Tileset.TileCount))
            For A = 0 To Tileset.TileCount - 1
                File_TTP.Write(CUShort(Tile_TypeNum(A)))
            Next

            WriteText(File_structBJO, False, "stru")
            File_structBJO.Write(8UI)
            Dim NonModuleStructureOrder As New clsObjectPriorityOrderList
            'non-module structures
            For Each Unit In Units
                If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                    StructureType = CType(Unit.Type, clsStructureType)
                    If Not StructureType.IsModule Then
                        NonModuleStructureOrder.SetItem(Unit)
                        NonModuleStructureOrder.ActionPerform()
                    End If
                End If
            Next
            Dim ModuleStructureOrder As New clsObjectPriorityOrderList
            'module structures
            For Each Unit In Units
                If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                    StructureType = CType(Unit.Type, clsStructureType)
                    If StructureType.IsModule Then
                        ModuleStructureOrder.SetItem(Unit)
                        ModuleStructureOrder.ActionPerform()
                    End If
                End If
            Next
            File_structBJO.Write(CUInt(NonModuleStructureOrder.Result.Count + ModuleStructureOrder.Result.Count))
            NonModuleStructureOrder.Result.PerformTool(StructureWrite)
            ModuleStructureOrder.Result.PerformTool(StructureWrite)

            Dim DintZeroBytes(11) As Byte

            WriteText(File_droidBJO, False, "dint")
            File_droidBJO.Write(8UI)
            Dim Droids As New clsObjectPriorityOrderList
            For Each Unit In Units
                If Unit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                    DroidType = CType(Unit.Type, clsDroidDesign)
                    If DroidType.IsTemplate Then
                        Droids.SetItem(Unit)
                        Droids.ActionPerform()
                    End If
                End If
            Next
            File_droidBJO.Write(CUInt(Droids.Result.Count))
            For A = 0 To Droids.Result.Count - 1
                Unit = Droids.Result.Item(A)
                DroidTemplate = CType(Unit.Type, clsDroidTemplate)
                WriteTextOfLength(File_droidBJO, 40, DroidTemplate.Code)
                File_droidBJO.Write(Unit.ID)
                File_droidBJO.Write(CUInt(Unit.Pos.Horizontal.X))
                File_droidBJO.Write(CUInt(Unit.Pos.Horizontal.Y))
                File_droidBJO.Write(CUInt(Unit.Pos.Altitude))
                File_droidBJO.Write(CUInt(Unit.Rotation))
                Select Case Args.CompileType
                    Case sWrite_WZ_Args.enumCompileType.Multiplayer
                        File_droidBJO.Write(Unit.GetBJOMultiplayerPlayerNum(Args.Multiplayer.PlayerCount))
                    Case sWrite_WZ_Args.enumCompileType.Campaign
                        File_droidBJO.Write(Unit.GetBJOCampaignPlayerNum)
                    Case Else
                        Stop
                End Select
                File_droidBJO.Write(DintZeroBytes)
            Next

            ReturnResult.Add(Serialize_WZ_FeaturesINI(INI_feature))
            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                ReturnResult.Add(Serialize_WZ_StructuresINI(INI_struct, Args.Multiplayer.PlayerCount))
                ReturnResult.Add(Serialize_WZ_DroidsINI(INI_droid, Args.Multiplayer.PlayerCount))
                ReturnResult.Add(Serialize_WZ_LabelsINI(INI_Labels, Args.Multiplayer.PlayerCount))
            ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then
                ReturnResult.Add(Serialize_WZ_StructuresINI(INI_struct, -1))
                ReturnResult.Add(Serialize_WZ_DroidsINI(INI_droid, -1))
                ReturnResult.Add(Serialize_WZ_LabelsINI(INI_Labels, 0)) 'interprets -1 players as an FMap
            End If

            File_LEV.Flush()
            File_MAP.Flush()
            File_GAM.Flush()
            File_featBJO.Flush()
            INI_feature.File.Flush()
            File_TTP.Flush()
            File_structBJO.Flush()
            INI_struct.File.Flush()
            File_droidBJO.Flush()
            INI_droid.File.Flush()
            INI_Labels.File.Flush()

            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then

                If Not Args.Overwrite Then
                    If IO.File.Exists(Args.Path) Then
                        ReturnResult.ProblemAdd("A file already exists at: " & Args.Path)
                        Return ReturnResult
                    End If
                Else
                    If IO.File.Exists(Args.Path) Then
                        Try
                            IO.File.Delete(Args.Path)
                        Catch ex As Exception
                            ReturnResult.ProblemAdd("Unable to delete existing file: " & ex.Message)
                            Return ReturnResult
                        End Try
                    End If
                End If

                Dim WZStream As Zip.ZipOutputStream

                Try
                    WZStream = New Zip.ZipOutputStream(IO.File.Create(Args.Path))
                Catch ex As Exception
                    ReturnResult.ProblemAdd(ex.Message)
                    Return ReturnResult
                End Try

                WZStream.SetLevel(9)
                WZStream.UseZip64 = Zip.UseZip64.Off 'warzone crashes without this

                Try

                    Dim ZipPath As String
                    Dim ZipEntry As Zip.ZipEntry

                    If Args.Multiplayer.IsBetaPlayerFormat Then
                        ZipPath = PlayersPrefix & Args.MapName & ".xplayers.lev"
                    Else
                        ZipPath = PlayersPrefix & Args.MapName & ".addon.lev"
                    End If
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        File_LEV_Memory.WriteTo(WZStream)
                        WZStream.Flush()
                        WZStream.CloseEntry()
                    End If

                    ZipEntry = New Zip.ZipEntry("multiplay/")
                    WZStream.PutNextEntry(ZipEntry)
                    ZipEntry = New Zip.ZipEntry("multiplay/maps/")
                    WZStream.PutNextEntry(ZipEntry)
                    ZipEntry = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & "/")
                    WZStream.PutNextEntry(ZipEntry)

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(File_GAM_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "dinit.bjo"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(File_droidBJO_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "droid.ini"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(INI_droid_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "feat.bjo"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(File_featBJO_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "feature.ini"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(INI_feature_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "game.map"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(File_MAP_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "struct.bjo"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(File_structBJO_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "struct.ini"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(INI_struct_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "ttypes.ttp"
                    ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                    If ZipEntry IsNot Nothing Then
                        ReturnResult.Add(WriteMemoryToZipEntryAndFlush(File_TTP_Memory, WZStream))
                    Else
                        ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                    End If

                    If INI_Labels_Memory.Length > 0 Then
                        ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "labels.ini"
                        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
                        If ZipEntry IsNot Nothing Then
                            ReturnResult.Add(WriteMemoryToZipEntryAndFlush(INI_Labels_Memory, WZStream))
                        Else
                            ReturnResult.ProblemAdd("Unable to make entry " & ZipPath)
                        End If
                    End If

                    WZStream.Finish()
                    WZStream.Close()
                    Return ReturnResult

                Catch ex As Exception
                    WZStream.Close()
                    ReturnResult.ProblemAdd(ex.Message)
                    Return ReturnResult
                End Try

            ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then

                Dim CampDirectory As String = EndWithPathSeperator(Args.Path)

                If Not IO.Directory.Exists(CampDirectory) Then
                    ReturnResult.ProblemAdd("Directory " & CampDirectory & " does not exist.")
                    Return ReturnResult
                End If

                Dim FilePath As String

                FilePath = CampDirectory & Args.MapName & ".gam"
                ReturnResult.Add(WriteMemoryToNewFile(File_GAM_Memory, CampDirectory & Args.MapName & ".gam"))

                CampDirectory &= Args.MapName & PlatformPathSeparator
                Try
                    IO.Directory.CreateDirectory(CampDirectory)
                Catch ex As Exception
                    ReturnResult.ProblemAdd("Unable to create directory " & CampDirectory)
                    Return ReturnResult
                End Try

                FilePath = CampDirectory & "dinit.bjo"
                ReturnResult.Add(WriteMemoryToNewFile(File_droidBJO_Memory, FilePath))

                FilePath = CampDirectory & "droid.ini"
                ReturnResult.Add(WriteMemoryToNewFile(INI_droid_Memory, FilePath))

                FilePath = CampDirectory & "feat.bjo"
                ReturnResult.Add(WriteMemoryToNewFile(File_featBJO_Memory, FilePath))

                FilePath = CampDirectory & "feature.ini"
                ReturnResult.Add(WriteMemoryToNewFile(INI_feature_Memory, FilePath))

                FilePath = CampDirectory & "game.map"
                ReturnResult.Add(WriteMemoryToNewFile(File_MAP_Memory, FilePath))

                FilePath = CampDirectory & "struct.bjo"
                ReturnResult.Add(WriteMemoryToNewFile(File_structBJO_Memory, FilePath))

                FilePath = CampDirectory & "struct.ini"
                ReturnResult.Add(WriteMemoryToNewFile(INI_struct_Memory, FilePath))

                FilePath = CampDirectory & "ttypes.ttp"
                ReturnResult.Add(WriteMemoryToNewFile(File_TTP_Memory, FilePath))

                FilePath = CampDirectory & "labels.ini"
                ReturnResult.Add(WriteMemoryToNewFile(INI_Labels_Memory, FilePath))
            End If

        Catch ex As Exception
            Stop
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        Return ReturnResult
    End Function

    Private Function Read_TTP(File As IO.BinaryReader) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = ""
        Dim uintTemp As UInteger
        Dim ushortTemp As UShort
        Dim A As Integer

        Try
            strTemp = ReadOldTextOfLength(File, 4)
            If strTemp <> "ttyp" Then
                ReturnResult.Problem = "Incorrect identifier."
                Return ReturnResult
            End If

            uintTemp = File.ReadUInt32
            If uintTemp <> 8UI Then
                ReturnResult.Problem = "Unknown version."
                Return ReturnResult
            End If
            uintTemp = File.ReadUInt32
            For A = 0 To CInt(Math.Min(uintTemp, CUInt(Tileset.TileCount))) - 1
                ushortTemp = File.ReadUInt16
                If ushortTemp > 11 Then
                    ReturnResult.Problem = "Unknown tile type number."
                    Return ReturnResult
                End If
                Tile_TypeNum(A) = CByte(ushortTemp)
            Next
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function
End Class
