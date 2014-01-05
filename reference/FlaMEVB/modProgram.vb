
Public Module modProgram

    Public Const ProgramName As String = "FlaME"

    Public Const ProgramVersionNumber As String = "1.29"

#If Mono Then
    Public Const ProgramPlatform As String = "Mono 2.10"
#Else
    Public Const ProgramPlatform As String = "Windows"
#End If

    Public Const PlayerCountMax As Integer = 10

    Public Const GameTypeCount As Integer = 3

    Public Const DefaultHeightMultiplier As Integer = 2

    Public Const MinimapDelay As Integer = 100

    Public Const SectorTileSize As Integer = 8

    Public Const MaxDroidWeapons As Integer = 3

    Public Const WZMapMaxSize As Integer = 250
    Public Const MapMaxSize As Integer = 512

    Public Const MinimapMaxSize As Integer = 512
    Public MinimapFeatureColour As sRGB_sng

    Public PlatformPathSeparator As Char

    Public Debug_GL As Boolean = False

    Public MyDocumentsProgramPath As String

    Public SettingsPath As String
    Public AutoSavePath As String
    Public InterfaceImagesPath As String

    Public Sub SetProgramSubDirs()

        MyDocumentsProgramPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments & PlatformPathSeparator & ".flaME"
#If Not Portable Then
        SettingsPath = MyDocumentsProgramPath & PlatformPathSeparator & "settings.ini"
        AutoSavePath = MyDocumentsProgramPath & PlatformPathSeparator & "autosave" & PlatformPathSeparator
#Else
        SettingsPath = My.Application.Info.DirectoryPath & PlatformPathSeparator & "settings.ini"
        AutoSavePath = My.Application.Info.DirectoryPath & PlatformPathSeparator & "autosave" & PlatformPathSeparator
#End If
        InterfaceImagesPath = My.Application.Info.DirectoryPath & PlatformPathSeparator & "interface" & PlatformPathSeparator
    End Sub

    Public ProgramInitialized As Boolean = False
    Public ProgramInitializeFinished As Boolean = False

    Public ProgramIcon As Icon

    Public CommandLinePaths As New SimpleList(Of String)

    Public GLTexture_NoTile As Integer
    Public GLTexture_OverflowTile As Integer

    Public IsViewKeyDown As New clsKeysActive

    Public TextureBrush As New clsBrush(0.0#, clsBrush.enumShape.Circle)
    Public TerrainBrush As New clsBrush(2.0#, clsBrush.enumShape.Circle)
    Public HeightBrush As New clsBrush(2.0#, clsBrush.enumShape.Circle)
    Public CliffBrush As New clsBrush(2.0#, clsBrush.enumShape.Circle)

    Public SmoothRadius As New clsBrush(1.0#, clsBrush.enumShape.Square)

    Public DisplayTileOrientation As Boolean

    Public ObjectData As clsObjectData

    Public SelectedTextureNum As Integer = -1
    Public TextureOrientation As New sTileOrientation(False, False, False)

    Public SelectedTerrain As clsPainter.clsTerrain
    Public SelectedRoad As clsPainter.clsRoad

    Public Class clsTileType
        Public Name As String
        Public DisplayColour As sRGB_sng
    End Class
    Public TileTypes As New SimpleList(Of clsTileType)

    Public Const TileTypeNum_Water As Integer = 7
    Public Const TileTypeNum_Cliff As Integer = 8

    Public TemplateDroidTypes(-1) As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidTypeCount As Integer

    Public ReadOnly UTF8Encoding As New System.Text.UTF8Encoding(False, False)
    Public ReadOnly ASCIIEncoding As New System.Text.ASCIIEncoding

    Public Const INIRotationMax As Integer = 65536

    Public Enum enumTileWalls As Integer
        None = 0
        Left = 1
        Right = 2
        Top = 4
        Bottom = 8
    End Enum

    Public Enum enumObjectRotateMode As Byte
        None
        Walls
        All
    End Enum

    Public Enum enumTextureTerrainAction As Byte
        Ignore
        Reinterpret
        Remove
    End Enum

    Public Enum enumFillCliffAction As Byte
        Ignore
        StopBefore
        StopAfter
    End Enum

    Public Structure sResult
        Public Success As Boolean
        Public Problem As String
    End Structure

    Public Structure sWZAngle
        Public Direction As UShort
        Public Pitch As UShort
        Public Roll As UShort
    End Structure

    Public Const TerrainGridSpacing As Integer = 128

    Public VisionRadius_2E As Integer
    Public VisionRadius As Double

    Public Copied_Map As clsMap

    Public Tilesets As New SimpleList(Of clsTileset)

    Public Tileset_Arizona As clsTileset
    Public Tileset_Urban As clsTileset
    Public Tileset_Rockies As clsTileset

    Public Painter_Arizona As clsPainter
    Public Painter_Urban As clsPainter
    Public Painter_Rockies As clsPainter

    Public UnitLabelFont As GLFont
    'Public TextureViewFont As GLFont

    Public Class clsPlayer
        Public Colour As sRGB_sng
        Public MinimapColour As sRGB_sng

        Public Sub CalcMinimapColour()

            MinimapColour.Red = Math.Min(Colour.Red * 0.6666667F + 0.333333343F, 1.0F)
            MinimapColour.Green = Math.Min(Colour.Green * 0.6666667F + 0.333333343F, 1.0F)
            MinimapColour.Blue = Math.Min(Colour.Blue * 0.6666667F + 0.333333343F, 1.0F)
        End Sub
    End Class
    Public PlayerColour(15) As clsPlayer

    Public Structure sSplitPath

        Public Parts() As String
        Public PartCount As Integer
        Public FilePath As String
        Public FileTitle As String
        Public FileTitleWithoutExtension As String
        Public FileExtension As String

        Public Sub New(Path As String)
            Dim A As Integer

            Parts = Path.Split(PlatformPathSeparator)
            PartCount = Parts.GetUpperBound(0) + 1
            FilePath = ""
            For A = 0 To PartCount - 2
                FilePath &= Parts(A) & PlatformPathSeparator
            Next
            FileTitle = Parts(A)
            A = InStrRev(FileTitle, ".")
            If A > 0 Then
                FileExtension = Strings.Right(FileTitle, FileTitle.Length - A)
                FileTitleWithoutExtension = Strings.Left(FileTitle, A - 1)
            Else
                FileExtension = ""
                FileTitleWithoutExtension = FileTitle
            End If
        End Sub
    End Structure

    Public Structure sZipSplitPath

        Public Parts() As String
        Public PartCount As Integer
        Public FilePath As String
        Public FileTitle As String
        Public FileTitleWithoutExtension As String
        Public FileExtension As String

        Public Sub New(Path As String)
            Dim PathFixed As String = Path.ToLower.Replace("\"c, "/"c)
            Dim A As Integer

            Parts = PathFixed.Split("/"c)
            PartCount = Parts.GetUpperBound(0) + 1
            FilePath = ""
            For A = 0 To PartCount - 2
                FilePath &= Parts(A) & "/"
            Next
            FileTitle = Parts(A)
            A = InStrRev(FileTitle, ".")
            If A > 0 Then
                FileExtension = Strings.Right(FileTitle, FileTitle.Length - A)
                FileTitleWithoutExtension = Strings.Left(FileTitle, A - 1)
            Else
                FileExtension = ""
                FileTitleWithoutExtension = FileTitle
            End If
        End Sub
    End Structure

    Public Sub VisionRadius_2E_Changed()

        VisionRadius = 256.0# * 2.0# ^ (VisionRadius_2E / 2.0#)
        If frmMainInstance.MapView IsNot Nothing Then
            View_Radius_Set(VisionRadius)
            frmMainInstance.View_DrawViewLater()
        End If
    End Sub

    Public Function EndWithPathSeperator(Text As String) As String

        If Strings.Right(Text, 1) = PlatformPathSeparator Then
            Return Text
        Else
            Return Text & PlatformPathSeparator
        End If
    End Function

    Public Function MinDigits(Number As Integer, Digits As Integer) As String
        Dim ReturnResult As String
        Dim A As Integer

        ReturnResult = InvariantToString_int(Number)
        A = Digits - ReturnResult.Length
        If A > 0 Then
            ReturnResult = Strings.StrDup(A, "0"c) & ReturnResult
        End If
        Return ReturnResult
    End Function

    Public Sub ViewKeyDown_Clear()

        IsViewKeyDown.Deactivate()

        For Each control As clsOption(Of clsKeyboardControl) In Options_KeyboardControls.Options
            CType(KeyboardProfile.Value(control), clsKeyboardControl).KeysChanged(IsViewKeyDown)
        Next
    End Sub

    Public TemplateDroidType_Droid As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Cyborg As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgConstruct As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgRepair As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgSuper As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Transporter As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Person As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Null As clsDroidDesign.clsTemplateDroidType

    Public Sub CreateTemplateDroidTypes()

        TemplateDroidType_Droid = New clsDroidDesign.clsTemplateDroidType("Droid", "DROID")
        TemplateDroidType_Droid.Num = TemplateDroidType_Add(TemplateDroidType_Droid)

        TemplateDroidType_Cyborg = New clsDroidDesign.clsTemplateDroidType("Cyborg", "CYBORG")
        TemplateDroidType_Cyborg.Num = TemplateDroidType_Add(TemplateDroidType_Cyborg)

        TemplateDroidType_CyborgConstruct = New clsDroidDesign.clsTemplateDroidType("Cyborg Construct", "CYBORG_CONSTRUCT")
        TemplateDroidType_CyborgConstruct.Num = TemplateDroidType_Add(TemplateDroidType_CyborgConstruct)

        TemplateDroidType_CyborgRepair = New clsDroidDesign.clsTemplateDroidType("Cyborg Repair", "CYBORG_REPAIR")
        TemplateDroidType_CyborgRepair.Num = TemplateDroidType_Add(TemplateDroidType_CyborgRepair)

        TemplateDroidType_CyborgSuper = New clsDroidDesign.clsTemplateDroidType("Cyborg Super", "CYBORG_SUPER")
        TemplateDroidType_CyborgSuper.Num = TemplateDroidType_Add(TemplateDroidType_CyborgSuper)

        TemplateDroidType_Transporter = New clsDroidDesign.clsTemplateDroidType("Transporter", "TRANSPORTER")
        TemplateDroidType_Transporter.Num = TemplateDroidType_Add(TemplateDroidType_Transporter)

        TemplateDroidType_Person = New clsDroidDesign.clsTemplateDroidType("Person", "PERSON")
        TemplateDroidType_Person.Num = TemplateDroidType_Add(TemplateDroidType_Person)

        TemplateDroidType_Null = New clsDroidDesign.clsTemplateDroidType("Null Droid", "ZNULLDROID")
        TemplateDroidType_Null.Num = TemplateDroidType_Add(TemplateDroidType_Null)
    End Sub

    Public Function GetTemplateDroidTypeFromTemplateCode(Code As String) As clsDroidDesign.clsTemplateDroidType
        Dim LCaseCode As String = Code.ToLower
        Dim A As Integer

        For A = 0 To TemplateDroidTypeCount - 1
            If TemplateDroidTypes(A).TemplateCode.ToLower = LCaseCode Then
                Return TemplateDroidTypes(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function TemplateDroidType_Add(NewDroidType As clsDroidDesign.clsTemplateDroidType) As Integer
        Dim ReturnResult As Integer

        ReDim Preserve TemplateDroidTypes(TemplateDroidTypeCount)
        TemplateDroidTypes(TemplateDroidTypeCount) = NewDroidType
        ReturnResult = TemplateDroidTypeCount
        TemplateDroidTypeCount += 1

        Return ReturnResult
    End Function

    Public Enum enumDroidType As Byte
        Weapon = 0
        Sensor = 1
        ECM = 2
        Construct = 3
        Person = 4
        Cyborg = 5
        Transporter = 6
        Command = 7
        Repair = 8
        Default_ = 9
        Cyborg_Construct = 10
        Cyborg_Repair = 11
        Cyborg_Super = 12
    End Enum

    Public Sub ShowWarnings(Result As clsResult)

        If Not Result.HasWarnings Then
            Exit Sub
        End If

        Dim WarningsForm As New frmWarnings(Result, Result.Text)
        WarningsForm.Show()
        WarningsForm.Activate()
    End Sub

    Public Function GetTurretTypeFromName(TurretTypeName As String) As clsTurret.enumTurretType

        Select Case TurretTypeName.ToLower
            Case "weapon"
                Return clsTurret.enumTurretType.Weapon
            Case "construct"
                Return clsTurret.enumTurretType.Construct
            Case "repair"
                Return clsTurret.enumTurretType.Repair
            Case "sensor"
                Return clsTurret.enumTurretType.Sensor
            Case "brain"
                Return clsTurret.enumTurretType.Brain
            Case "ecm"
                Return clsTurret.enumTurretType.ECM
            Case Else
                Return clsTurret.enumTurretType.Unknown
        End Select
    End Function

    Public ShowIDErrorMessage As Boolean = True

    Public Sub ErrorIDChange(IntendedID As UInteger, IDUnit As clsMap.clsUnit, NameOfErrorSource As String)

        If Not ShowIDErrorMessage Then
            Exit Sub
        End If

        If IDUnit.ID = IntendedID Then
            Exit Sub
        End If

        Dim MessageText As String

        MessageText = "An object's ID has been changed unexpectedly. The error was in " & ControlChars.Quote & NameOfErrorSource & ControlChars.Quote & "." & ControlChars.CrLf & ControlChars.CrLf & "The object is of type " & IDUnit.Type.GetDisplayTextCode & " and is at map position " & IDUnit.GetPosText & ". It's ID was " & InvariantToString_uint(IntendedID) & ", but is now " & InvariantToString_uint(IDUnit.ID) & "." & ControlChars.CrLf & ControlChars.CrLf & "Click Cancel to stop seeing this message. Otherwise, click OK."

        If MsgBox(MessageText, MsgBoxStyle.OkCancel) = MsgBoxResult.Cancel Then
            ShowIDErrorMessage = False
        End If
    End Sub

    Public Sub ZeroIDWarning(IDUnit As clsMap.clsUnit, NewID As UInteger, Output As clsResult)
        Dim MessageText As String

        MessageText = "An object's ID has been changed from 0 to " & InvariantToString_uint(NewID) & ". Zero is not a valid ID. The object is of type " & IDUnit.Type.GetDisplayTextCode & " and is at map position " & IDUnit.GetPosText & "."

        'MsgBox(MessageText, MsgBoxStyle.OkOnly)
        Output.WarningAdd(MessageText)
    End Sub

    Public Structure sWorldPos
        Public Horizontal As sXY_int
        Public Altitude As Integer

        Public Sub New(NewHorizontal As sXY_int, NewAltitude As Integer)

            Horizontal = NewHorizontal
            Altitude = NewAltitude
        End Sub
    End Structure

    Public Class clsWorldPos
        Public WorldPos As sWorldPos

        Public Sub New(NewWorldPos As sWorldPos)

            WorldPos = NewWorldPos
        End Sub
    End Class

    Public Function PosIsWithinTileArea(WorldHorizontal As sXY_int, StartTile As sXY_int, FinishTile As sXY_int) As Boolean

        Return (WorldHorizontal.X >= StartTile.X * TerrainGridSpacing And _
            WorldHorizontal.Y >= StartTile.Y * TerrainGridSpacing And _
            WorldHorizontal.X < FinishTile.X * TerrainGridSpacing And _
            WorldHorizontal.Y < FinishTile.Y * TerrainGridSpacing)
    End Function

    Public Function SizeIsPowerOf2(Size As Integer) As Boolean

        Dim Power As Double = Math.Log(Size) / Math.Log(2.0#)
        Return (Power = CInt(Power))
    End Function

    Public Class clsKeysActive
        Public Keys(255) As Boolean

        Public Sub Deactivate()

            For i As Integer = 0 To 255
                Keys(i) = False
            Next
        End Sub
    End Class

    Public Function LoadTilesets(TilesetsPath As String) As clsResult
        Dim ReturnResult As New clsResult("Loading tilesets")

        Dim TilesetDirs() As String
        Try
            TilesetDirs = IO.Directory.GetDirectories(TilesetsPath)
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        If TilesetDirs Is Nothing Then
            Return ReturnResult
        End If

        Dim Result As clsResult
        Dim Path As String
        Dim Tileset As clsTileset

        For Each Path In TilesetDirs
            Tileset = New clsTileset
            Result = Tileset.LoadDirectory(Path)
            ReturnResult.Add(Result)
            If Not Result.HasProblems Then
                Tilesets.Add(Tileset)
            End If
        Next

        For Each Tileset In Tilesets
            If Tileset.Name = "tertilesc1hw" Then
                Tileset.Name = "Arizona"
                Tileset_Arizona = Tileset
                Tileset.IsOriginal = True
                Tileset.BGColour = New sRGB_sng(204.0# / 255.0#, 149.0# / 255.0#, 70.0# / 255.0#)
            ElseIf Tileset.Name = "tertilesc2hw" Then
                Tileset.Name = "Urban"
                Tileset_Urban = Tileset
                Tileset.IsOriginal = True
                Tileset.BGColour = New sRGB_sng(118.0# / 255.0#, 165.0# / 255.0#, 203.0# / 255.0#)
            ElseIf Tileset.Name = "tertilesc3hw" Then
                Tileset.Name = "Rocky Mountains"
                Tileset_Rockies = Tileset
                Tileset.IsOriginal = True
                Tileset.BGColour = New sRGB_sng(182.0# / 255.0#, 225.0# / 255.0#, 236.0# / 255.0#)
            End If
        Next

        If Tileset_Arizona Is Nothing Then
            ReturnResult.WarningAdd("Arizona tileset is missing.")
        End If
        If Tileset_Urban Is Nothing Then
            ReturnResult.WarningAdd("Urban tileset is missing.")
        End If
        If Tileset_Rockies Is Nothing Then
            ReturnResult.WarningAdd("Rocky Mountains tileset is missing.")
        End If

        Return ReturnResult
    End Function

    Public Draw_TileTextures As Boolean = True
    Public Enum enumDrawLighting As Byte
        Off
        Half
        Normal
    End Enum
    Public Draw_Lighting As enumDrawLighting = enumDrawLighting.Half
    Public Draw_TileWireframe As Boolean
    Public Draw_Units As Boolean = True
    Public Draw_VertexTerrain As Boolean
    Public Draw_Gateways As Boolean
    Public Draw_ScriptMarkers As Boolean = True

    Enum enumView_Move_Type As Byte
        Free
        RTS
    End Enum
    Public ViewMoveType As enumView_Move_Type = enumView_Move_Type.RTS
    Public RTSOrbit As Boolean = True

    Public SunAngleMatrix As New Matrix3D.Matrix3D
    Public VisionSectors As New clsBrush(0.0#, clsBrush.enumShape.Circle)

    Public Sub View_Radius_Set(Radius As Double)

        VisionSectors.Radius = Radius / (TerrainGridSpacing * SectorTileSize)
    End Sub

    Public Structure sLayerList
        Public Class clsLayer
            Public WithinLayer As Integer
            Public AvoidLayers() As Boolean
            Public Terrain As clsPainter.clsTerrain
            Public Terrainmap As clsBooleanMap
            Public HeightMin As Single
            Public HeightMax As Single
            Public SlopeMin As Single
            Public SlopeMax As Single
            'for generator only
            Public Scale As Single
            Public Density As Single
        End Class
        Public Layers() As clsLayer
        Public LayerCount As Integer

        Public Sub Layer_Insert(PositionNum As Integer, NewLayer As clsLayer)
            Dim A As Integer
            Dim B As Integer

            ReDim Preserve Layers(LayerCount)
            'shift the ones below down
            For A = LayerCount - 1 To PositionNum Step -1
                Layers(A + 1) = Layers(A)
            Next
            'insert the new entry
            Layers(PositionNum) = NewLayer
            LayerCount += 1

            For A = 0 To LayerCount - 1
                If Layers(A).WithinLayer >= PositionNum Then
                    Layers(A).WithinLayer = Layers(A).WithinLayer + 1
                End If
                ReDim Preserve Layers(A).AvoidLayers(LayerCount - 1)
                For B = LayerCount - 2 To PositionNum Step -1
                    Layers(A).AvoidLayers(B + 1) = Layers(A).AvoidLayers(B)
                Next
                Layers(A).AvoidLayers(PositionNum) = False
            Next
        End Sub

        Public Sub Layer_Remove(Layer_Num As Integer)
            Dim A As Integer
            Dim B As Integer

            LayerCount = LayerCount - 1
            For A = Layer_Num To LayerCount - 1
                Layers(A) = Layers(A + 1)
            Next
            ReDim Preserve Layers(LayerCount - 1)

            For A = 0 To LayerCount - 1
                If Layers(A).WithinLayer = Layer_Num Then
                    Layers(A).WithinLayer = -1
                ElseIf Layers(A).WithinLayer > Layer_Num Then
                    Layers(A).WithinLayer = Layers(A).WithinLayer - 1
                End If
                For B = Layer_Num To LayerCount - 1
                    Layers(A).AvoidLayers(B) = Layers(A).AvoidLayers(B + 1)
                Next
                ReDim Preserve Layers(A).AvoidLayers(LayerCount - 1)
            Next
        End Sub

        Public Sub Layer_Move(Layer_Num As Integer, Layer_Dest_Num As Integer)
            Dim Layer_Temp As clsLayer
            Dim boolTemp As Boolean
            Dim A As Integer
            Dim B As Integer

            If Layer_Dest_Num < Layer_Num Then
                'move the variables
                Layer_Temp = Layers(Layer_Num)
                For A = Layer_Num - 1 To Layer_Dest_Num Step -1
                    Layers(A + 1) = Layers(A)
                Next
                Layers(Layer_Dest_Num) = Layer_Temp
                'update the layer nums
                For A = 0 To LayerCount - 1
                    If Layers(A).WithinLayer = Layer_Num Then
                        Layers(A).WithinLayer = Layer_Dest_Num
                    ElseIf Layers(A).WithinLayer >= Layer_Dest_Num And Layers(A).WithinLayer < Layer_Num Then
                        Layers(A).WithinLayer = Layers(A).WithinLayer + 1
                    End If
                    boolTemp = Layers(A).AvoidLayers(Layer_Num)
                    For B = Layer_Num - 1 To Layer_Dest_Num Step -1
                        Layers(A).AvoidLayers(B + 1) = Layers(A).AvoidLayers(B)
                    Next
                    Layers(A).AvoidLayers(Layer_Dest_Num) = boolTemp
                Next
            ElseIf Layer_Dest_Num > Layer_Num Then
                'move the variables
                Layer_Temp = Layers(Layer_Num)
                For A = Layer_Num To Layer_Dest_Num - 1
                    Layers(A) = Layers(A + 1)
                Next
                Layers(Layer_Dest_Num) = Layer_Temp
                'update the layer nums
                For A = 0 To LayerCount - 1
                    If Layers(A).WithinLayer = Layer_Num Then
                        Layers(A).WithinLayer = Layer_Dest_Num
                    ElseIf Layers(A).WithinLayer > Layer_Num And Layers(A).WithinLayer <= Layer_Dest_Num Then
                        Layers(A).WithinLayer = Layers(A).WithinLayer - 1
                    End If
                    boolTemp = Layers(A).AvoidLayers(Layer_Num)
                    For B = Layer_Num To Layer_Dest_Num - 1
                        Layers(A).AvoidLayers(B) = Layers(A).AvoidLayers(B + 1)
                    Next
                    Layers(A).AvoidLayers(Layer_Dest_Num) = boolTemp
                Next
            End If
        End Sub
    End Structure
    Public LayerList As sLayerList

    Public Function CalcUnitsCentrePos(Units As SimpleList(Of clsMap.clsUnit)) As Matrix3D.XY_dbl
        Dim Result As Matrix3D.XY_dbl

        Result.X = 0.0#
        Result.Y = 0.0#
        Dim Unit As clsMap.clsUnit
        For Each Unit In Units
            Result += Unit.Pos.Horizontal.ToDoubles
        Next
        Result /= CDbl(Units.Count)

        Return Result
    End Function
End Module

Public Class clsContainer(Of ItemType)

    Public Item As ItemType

    Public Sub New(item As ItemType)

        Me.Item = item
    End Sub
End Class
