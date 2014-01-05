
Partial Public Class clsMap

    Public Class clsUnit
        Public MapLink As New ConnectedListLink(Of clsUnit, clsMap)(Me)
        Public MapSelectedUnitLink As New ConnectedListLink(Of clsUnit, clsMap)(Me)
        Public Sectors As New ConnectedList(Of clsUnitSectorConnection, clsUnit)(Me)

        Public ID As UInteger
        Public Type As clsUnitType
        Public Pos As sWorldPos
        Public Rotation As Integer
        Public UnitGroup As clsUnitGroup
        Public SavePriority As Integer
        Public Health As Double = 1.0#
        Public PreferPartsOutput As Boolean = False

        Private _Label As String

        Public Sub New()

        End Sub

        Public Sub New(UnitToCopy As clsUnit, TargetMap As clsMap)
            Dim IsDesign As Boolean

            If UnitToCopy.Type.Type = clsUnitType.enumType.PlayerDroid Then
                IsDesign = Not CType(UnitToCopy.Type, clsDroidDesign).IsTemplate
            Else
                IsDesign = False
            End If
            If IsDesign Then
                Dim DroidDesign As New clsDroidDesign
                Type = DroidDesign
                DroidDesign.CopyDesign(CType(UnitToCopy.Type, clsDroidDesign))
                DroidDesign.UpdateAttachments()
            Else
                Type = UnitToCopy.Type
            End If
            Pos = UnitToCopy.Pos
            Rotation = UnitToCopy.Rotation
            Dim OtherUnitGroup As clsMap.clsUnitGroup
            OtherUnitGroup = UnitToCopy.UnitGroup
            If OtherUnitGroup.WZ_StartPos < 0 Then
                UnitGroup = TargetMap.ScavengerUnitGroup
            Else
                UnitGroup = TargetMap.UnitGroups.Item(OtherUnitGroup.WZ_StartPos)
            End If
            SavePriority = UnitToCopy.SavePriority
            Health = UnitToCopy.Health
            PreferPartsOutput = UnitToCopy.PreferPartsOutput
        End Sub

        Public ReadOnly Property Label As String
            Get
                Return _Label
            End Get
        End Property

        Public Function GetINIPosition() As String

            Return InvariantToString_int(Pos.Horizontal.X) & ", " & InvariantToString_int(Pos.Horizontal.Y) & ", 0"
        End Function

        Public Function GetINIRotation() As String
            Dim Rotation16 As Integer

            Rotation16 = CInt(Rotation * INIRotationMax / 360.0#)
            If Rotation16 >= INIRotationMax Then
                Rotation16 -= INIRotationMax
            ElseIf Rotation16 < 0 Then
                Stop
                Rotation16 += INIRotationMax
            End If

            Return InvariantToString_int(Rotation16) & ", 0, 0"
        End Function

        Public Function GetINIHealthPercent() As String

            Return InvariantToString_int(CInt(Clamp_dbl(Health * 100.0#, 1.0#, 100.0#))) & "%"
        End Function

        Public Function GetPosText() As String

            Return InvariantToString_int(Pos.Horizontal.X) & ", " & InvariantToString_int(Pos.Horizontal.Y)
        End Function

        Public Function SetLabel(Text As String) As sResult
            Dim Result As sResult

            If Type.Type = clsUnitType.enumType.PlayerStructure Then
                Dim StructureType As clsStructureType = CType(Type, clsStructureType)
                Dim StructureTypeType As clsStructureType.enumStructureType = StructureType.StructureType
                If StructureTypeType = clsStructureType.enumStructureType.FactoryModule _
                    Or StructureTypeType = clsStructureType.enumStructureType.PowerModule _
                    Or StructureTypeType = clsStructureType.enumStructureType.ResearchModule Then
                    Result.Problem = "Error: Trying to assign label to structure module."
                    Return Result
                End If
            End If

            If Not MapLink.IsConnected Then
                Stop
                Result.Problem = "Error: Unit not on a map."
                Return Result
            End If

            If Text Is Nothing Then
                _Label = Nothing
                Result.Success = True
                Result.Problem = ""
                Return Result
            Else
                Result = MapLink.Source.ScriptLabelIsValid(Text)
                If Result.Success Then
                    _Label = Text
                End If
                Return Result
            End If
        End Function

        Public Sub WriteWZLabel(File As clsINIWrite, PlayerCount As Integer)

            If _Label IsNot Nothing Then
                Dim TypeNum As Integer
                Select Case Type.Type
                    Case clsUnitType.enumType.PlayerDroid
                        TypeNum = 0
                    Case clsUnitType.enumType.PlayerStructure
                        TypeNum = 1
                    Case clsUnitType.enumType.Feature
                        TypeNum = 2
                    Case Else
                        Exit Sub
                End Select
                File.SectionName_Append("object_" & InvariantToString_int(MapLink.ArrayPosition))
                File.Property_Append("id", InvariantToString_uint(ID))
                If PlayerCount >= 0 Then 'not an FMap
                    File.Property_Append("type", InvariantToString_int(TypeNum))
                    File.Property_Append("player", InvariantToString_int(UnitGroup.GetPlayerNum(PlayerCount)))
                End If
                File.Property_Append("label", _Label)
                File.Gap_Append()
            End If
        End Sub

        Public Function GetBJOMultiplayerPlayerNum(PlayerCount As Integer) As UInteger
            Dim PlayerNum As Integer

            If UnitGroup Is MapLink.Source.ScavengerUnitGroup Or UnitGroup.WZ_StartPos < 0 Then
                PlayerNum = Math.Max(PlayerCount, 7)
            Else
                PlayerNum = UnitGroup.WZ_StartPos
            End If
            Return CUInt(PlayerNum)
        End Function

        Public Function GetBJOCampaignPlayerNum() As UInteger
            Dim PlayerNum As Integer

            If UnitGroup Is MapLink.Source.ScavengerUnitGroup Or UnitGroup.WZ_StartPos < 0 Then
                PlayerNum = 7
            Else
                PlayerNum = UnitGroup.WZ_StartPos
            End If
            Return CUInt(PlayerNum)
        End Function

        Public Sub MapSelect()

            If MapSelectedUnitLink.IsConnected Then
                Stop
                Exit Sub
            End If

            MapSelectedUnitLink.Connect(MapLink.Source.SelectedUnits)
        End Sub

        Public Sub MapDeselect()

            If Not MapSelectedUnitLink.IsConnected Then
                Stop
                Exit Sub
            End If

            MapSelectedUnitLink.Disconnect()
        End Sub

        Public Sub DisconnectFromMap()

            If MapLink.IsConnected Then
                MapLink.Disconnect()
            End If
            If MapSelectedUnitLink.IsConnected Then
                MapSelectedUnitLink.Disconnect()
            End If
            Sectors.Clear()
        End Sub

        Public Sub Deallocate()

            MapLink.Deallocate()
            MapSelectedUnitLink.Deallocate()
            Sectors.Deallocate()
        End Sub
    End Class

    Public Units As New ConnectedList(Of clsMap.clsUnit, clsMap)(Me)

    Public Class clsUnitSectorConnection

        Protected Class Link(Of SourceType As Class)
            Inherits ConnectedListLink(Of clsUnitSectorConnection, SourceType)

            Public Sub New(Owner As clsUnitSectorConnection)
                MyBase.New(Owner)

            End Sub

            Public Overrides Sub AfterRemove()
                MyBase.AfterRemove()

                Item.Deallocate()
            End Sub
        End Class

        Protected _UnitLink As New Link(Of clsMap.clsUnit)(Me)
        Protected _SectorLink As New Link(Of clsMap.clsSector)(Me)

        Public Overridable ReadOnly Property Unit As clsMap.clsUnit
            Get
                Return _UnitLink.Source
            End Get
        End Property

        Public Overridable ReadOnly Property Sector As clsMap.clsSector
            Get
                Return _SectorLink.Source
            End Get
        End Property

        Public Shared Function Create(Unit As clsMap.clsUnit, Sector As clsMap.clsSector) As clsUnitSectorConnection

            If Unit Is Nothing Then
                Return Nothing
            End If
            If Unit.Sectors Is Nothing Then
                Return Nothing
            End If
            If Unit.Sectors.IsBusy Then
                Return Nothing
            End If
            If Sector Is Nothing Then
                Return Nothing
            End If
            If Sector.Units Is Nothing Then
                Return Nothing
            End If
            If Sector.Units.IsBusy Then
                Return Nothing
            End If

            Dim Result As New clsUnitSectorConnection
            Result._UnitLink.Connect(Unit.Sectors)
            Result._SectorLink.Connect(Sector.Units)
            Return Result
        End Function

        Protected Sub New()


        End Sub

        Public Sub Deallocate()

            _UnitLink.Deallocate()
            _SectorLink.Deallocate()
        End Sub
    End Class

    Public Class clsUnitGroupContainer

        Private _Item As clsUnitGroup

        Public Property Item As clsUnitGroup
            Get
                Return _Item
            End Get
            Set(value As clsUnitGroup)
                If value Is _Item Then
                    Exit Property
                End If
                _Item = value
                RaiseEvent Changed()
            End Set
        End Property

        Public Event Changed()
    End Class

    Private _SelectedUnitGroup As clsUnitGroupContainer
    Public ReadOnly Property SelectedUnitGroup As clsUnitGroupContainer
        Get
            Return _SelectedUnitGroup
        End Get
    End Property

    Public Class clsUnitGroup

        Public MapLink As New ConnectedListLink(Of clsUnitGroup, clsMap)(Me)

        Public WZ_StartPos As Integer = -1

        Public Function GetFMapINIPlayerText() As String

            If WZ_StartPos < 0 Or WZ_StartPos >= PlayerCountMax Then
                Return "scavenger"
            Else
                Return InvariantToString_int(WZ_StartPos)
            End If
        End Function

        Public Function GetLNDPlayerText() As String

            If WZ_StartPos < 0 Or WZ_StartPos >= PlayerCountMax Then
                Return InvariantToString_int(7)
            Else
                Return InvariantToString_int(WZ_StartPos)
            End If
        End Function

        Public Function GetPlayerNum(PlayerCount As Integer) As Integer

            If WZ_StartPos < 0 Or WZ_StartPos >= PlayerCountMax Then
                Return Math.Max(PlayerCount, 7)
            Else
                Return WZ_StartPos
            End If
        End Function
    End Class

    Public UnitGroups As New ConnectedList(Of clsUnitGroup, clsMap)(Me)
    Public ScavengerUnitGroup As clsUnitGroup

    Public Function GetAvailableID() As UInteger
        Dim Unit As clsUnit
        Dim ID As UInteger

        ID = 1UI
        For Each Unit In Units
            If Unit.ID >= ID Then
                ID = Unit.ID + 1UI
            End If
        Next

        Return ID
    End Function

    Public Class clsUnitAdd
        Public Map As clsMap
        Public NewUnit As clsUnit
        Public ID As UInteger = 0UI
        Public Label As String = Nothing
        Public StoreChange As Boolean = False

        Public Function Perform() As Boolean

            If Map Is Nothing Then
                Stop
                Return False
            End If
            If NewUnit Is Nothing Then
                Stop
                Return False
            End If

            If NewUnit.MapLink.IsConnected Then
                MsgBox("Error: Added object already has a map assigned.")
                Return False
            End If
            If NewUnit.UnitGroup Is Nothing Then
                MsgBox("Error: Added object has no group.")
                NewUnit.UnitGroup = Map.ScavengerUnitGroup
                Return False
            End If
            If NewUnit.UnitGroup.MapLink.Source IsNot Map Then
                MsgBox("Error: Something terrible happened.")
                Return False
            End If

            If StoreChange Then
                Dim UnitChange As New clsMap.clsUnitChange
                UnitChange.Type = clsUnitChange.enumType.Added
                UnitChange.Unit = NewUnit
                Map.UnitChanges.Add(UnitChange)
            End If

            If ID <= 0UI Then
                ID = Map.GetAvailableID
            ElseIf Map.IDUsage(ID) IsNot Nothing Then
                ID = Map.GetAvailableID
            End If

            NewUnit.ID = ID

            NewUnit.MapLink.Connect(Map.Units)

            NewUnit.Pos.Horizontal.X = Clamp_int(NewUnit.Pos.Horizontal.X, 0, Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
            NewUnit.Pos.Horizontal.Y = Clamp_int(NewUnit.Pos.Horizontal.Y, 0, Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
            NewUnit.Pos.Altitude = CInt(Math.Ceiling(Map.GetTerrainHeight(NewUnit.Pos.Horizontal)))

            If Label IsNot Nothing Then
                NewUnit.SetLabel(Label)
            End If

            Map.UnitSectorsCalc(NewUnit)

            If Map.SectorGraphicsChanges IsNot Nothing Then
                Map.UnitSectorsGraphicsChanged(NewUnit)
            End If

            Return True
        End Function
    End Class

    Public Sub UnitRemoveStoreChange(Num As Integer)

        Dim UnitChange As New clsMap.clsUnitChange
        UnitChange.Type = clsUnitChange.enumType.Deleted
        UnitChange.Unit = Units(Num)
        UnitChanges.Add(UnitChange)

        UnitRemove(Num)
    End Sub

    Public Sub UnitRemove(Num As Integer)
        Dim Unit As clsMap.clsUnit

        Unit = Units(Num)

        If SectorGraphicsChanges IsNot Nothing Then
            UnitSectorsGraphicsChanged(Unit)
        End If

        If ViewInfo IsNot Nothing Then
            Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = ViewInfo.GetMouseOverTerrain
            If MouseOverTerrain IsNot Nothing Then
                Dim Pos As Integer = MouseOverTerrain.Units.FindFirstItemPosition(Unit)
                If Pos >= 0 Then
                    MouseOverTerrain.Units.Remove(Pos)
                End If
            End If
        End If

        Unit.DisconnectFromMap()
    End Sub

    Public Sub UnitSwap(OldUnit As clsMap.clsUnit, NewUnit As clsMap.clsUnit)

        If OldUnit.MapLink.Source IsNot Me Then
            Stop
            Exit Sub
        End If

        UnitRemoveStoreChange(OldUnit.MapLink.ArrayPosition)
        Dim UnitAdd As New clsMap.clsUnitAdd
        UnitAdd.Map = Me
        UnitAdd.StoreChange = True
        UnitAdd.ID = OldUnit.ID
        UnitAdd.NewUnit = NewUnit
        UnitAdd.Label = OldUnit.Label
        UnitAdd.Perform()
        ErrorIDChange(OldUnit.ID, NewUnit, "UnitSwap")
    End Sub

    Public Sub MakeDefaultUnitGroups()
        Dim A As Integer
        Dim NewGroup As clsMap.clsUnitGroup

        UnitGroups.Clear()
        For A = 0 To PlayerCountMax - 1
            NewGroup = New clsUnitGroup
            NewGroup.WZ_StartPos = A
            NewGroup.MapLink.Connect(UnitGroups)
        Next
        ScavengerUnitGroup = New clsUnitGroup
        ScavengerUnitGroup.MapLink.Connect(UnitGroups)
        ScavengerUnitGroup.WZ_StartPos = -1
    End Sub

    Public Function GetUnitGroupColour(ColourUnitGroup As clsUnitGroup) As sRGB_sng

        If ColourUnitGroup.WZ_StartPos < 0 Then
            Return New sRGB_sng(1.0F, 1.0F, 1.0F)
        Else
            Return PlayerColour(ColourUnitGroup.WZ_StartPos).Colour
        End If
    End Function

    Public Function GetUnitGroupMinimapColour(ColourUnitGroup As clsUnitGroup) As sRGB_sng

        If ColourUnitGroup.WZ_StartPos < 0 Then
            Return New sRGB_sng(1.0F, 1.0F, 1.0F)
        Else
            Return PlayerColour(ColourUnitGroup.WZ_StartPos).MinimapColour
        End If
    End Function

    Public Function IDUsage(ID As UInteger) As clsUnit

        For Each Unit As clsUnit In Units
            If Unit.ID = ID Then
                Return Unit
                Exit For
            End If
        Next

        Return Nothing
    End Function

    Public Class clsUnitCreate

        Public Map As clsMap
        Public ObjectType As clsUnitType
        Public Horizontal As sXY_int
        Public UnitGroup As clsUnitGroup
        Public AutoWalls As Boolean = False
        Public Rotation As Integer = 0
        Public RandomizeRotation As Boolean = False

        Public Function Perform() As clsUnit

            If AutoWalls Then
                If ObjectType.Type = clsUnitType.enumType.PlayerStructure Then
                    Dim StructureType As clsStructureType = CType(ObjectType, clsStructureType)
                    If StructureType.WallLink.IsConnected Then
                        Dim AutoWallType As clsWallType = Nothing
                        AutoWallType = StructureType.WallLink.Source
                        Map.PerformTileWall(AutoWallType, Map.GetPosTileNum(Horizontal), True)
                        Return Nothing
                    End If
                End If
            End If
            Dim newUnit As New clsMap.clsUnit
            If RandomizeRotation Then
                newUnit.Rotation = CInt(Int(Rnd() * 360.0#))
            Else
                newUnit.Rotation = Rotation
            End If
            newUnit.UnitGroup = unitGroup
            newUnit.Pos = Map.TileAlignedPosFromMapPos(Horizontal, ObjectType.GetFootprintSelected(newUnit.Rotation))
            newUnit.Type = objectType
            Dim UnitAdd As New clsUnitAdd
            UnitAdd.Map = Map
            UnitAdd.NewUnit = newUnit
            UnitAdd.StoreChange = True
            UnitAdd.Perform()
            Return newUnit
        End Function
    End Class

    Public Sub SetObjectCreatorDefaults(objectCreator As clsMap.clsUnitCreate)

        objectCreator.Map = Me

        objectCreator.ObjectType = frmMainInstance.SingleSelectedObjectType
        objectCreator.AutoWalls = frmMainInstance.cbxAutoWalls.Checked
        objectCreator.UnitGroup = SelectedUnitGroup.Item
        Try
            Dim Rotation As Integer
            InvariantParse_int(frmMainInstance.txtNewObjectRotation.Text, Rotation)
            If Rotation < 0 Or Rotation > 359 Then
                objectCreator.Rotation = 0
            Else
                objectCreator.Rotation = Rotation
            End If
        Catch
            objectCreator.Rotation = 0
        End Try
        objectCreator.RandomizeRotation = frmMainInstance.cbxObjectRandomRotation.Checked
    End Sub
End Class
