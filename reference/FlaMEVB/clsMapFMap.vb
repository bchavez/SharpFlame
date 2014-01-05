Imports ICSharpCode.SharpZipLib

Partial Public Class clsMap

    Public Class clsFMap_INIObjects
        Inherits clsINIRead.clsSectionTranslator

        Public Structure sObject
            Public ID As UInteger
            Public Type As clsUnitType.enumType
            Public IsTemplate As Boolean
            Public Code As String
            Public UnitGroup As String
            Public GotAltitude As Boolean
            Public Pos As clsXY_int
            Public Heading As Double
            Public Health As Double
            Public TemplateDroidType As clsDroidDesign.clsTemplateDroidType
            Public BodyCode As String
            Public PropulsionCode As String
            Public TurretTypes() As clsTurret.enumTurretType
            Public TurretCodes() As String
            Public TurretCount As Integer
            Public Priority As Integer
            Public Label As String
            Public WallType As Integer
        End Structure
        Public Objects() As sObject
        Public ObjectCount As Integer

        Public Sub New(NewObjectCount As Integer)
            Dim A As Integer
            Dim B As Integer

            ObjectCount = NewObjectCount
            ReDim Objects(ObjectCount - 1)
            For A = 0 To ObjectCount - 1
                Objects(A).Type = clsUnitType.enumType.Unspecified
                Objects(A).Health = 1.0#
                Objects(A).WallType = -1
                ReDim Objects(A).TurretCodes(MaxDroidWeapons - 1)
                ReDim Objects(A).TurretTypes(MaxDroidWeapons - 1)
                For B = 0 To MaxDroidWeapons - 1
                    Objects(A).TurretTypes(B) = clsTurret.enumTurretType.Unknown
                Next
            Next
        End Sub

        Public Overrides Function Translate(INISectionNum As Integer, INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "type"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 1 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Select Case CommaText(0).ToLower
                        Case "feature"
                            Objects(INISectionNum).Type = clsUnitType.enumType.Feature
                            Objects(INISectionNum).Code = CommaText(1)
                        Case "structure"
                            Objects(INISectionNum).Type = clsUnitType.enumType.PlayerStructure
                            Objects(INISectionNum).Code = CommaText(1)
                        Case "droidtemplate"
                            Objects(INISectionNum).Type = clsUnitType.enumType.PlayerDroid
                            Objects(INISectionNum).IsTemplate = True
                            Objects(INISectionNum).Code = CommaText(1)
                        Case "droiddesign"
                            Objects(INISectionNum).Type = clsUnitType.enumType.PlayerDroid
                        Case Else
                            Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Select
                Case "droidtype"
                    Dim DroidType As clsDroidDesign.clsTemplateDroidType = GetTemplateDroidTypeFromTemplateCode(INIProperty.Value)
                    If DroidType Is Nothing Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).TemplateDroidType = DroidType
                Case "body"
                    Objects(INISectionNum).BodyCode = INIProperty.Value
                Case "propulsion"
                    Objects(INISectionNum).PropulsionCode = INIProperty.Value
                Case "turretcount"
                    Dim NewTurretCount As Integer
                    If Not InvariantParse_int(INIProperty.Value, NewTurretCount) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If NewTurretCount < 0 Or NewTurretCount > MaxDroidWeapons Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).TurretCount = NewTurretCount
                Case "turret1"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Dim TurretType As clsTurret.enumTurretType
                    TurretType = GetTurretTypeFromName(CommaText(0))
                    If TurretType <> clsTurret.enumTurretType.Unknown Then
                        Objects(INISectionNum).TurretTypes(0) = TurretType
                        Objects(INISectionNum).TurretCodes(0) = CommaText(1)
                    End If
                Case "turret2"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Dim TurretType As clsTurret.enumTurretType
                    TurretType = GetTurretTypeFromName(CommaText(0))
                    If TurretType <> clsTurret.enumTurretType.Unknown Then
                        Objects(INISectionNum).TurretTypes(1) = TurretType
                        Objects(INISectionNum).TurretCodes(1) = CommaText(1)
                    End If
                Case "turret3"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Dim TurretType As clsTurret.enumTurretType
                    TurretType = GetTurretTypeFromName(CommaText(0))
                    If TurretType <> clsTurret.enumTurretType.Unknown Then
                        Objects(INISectionNum).TurretTypes(2) = TurretType
                        Objects(INISectionNum).TurretCodes(2) = CommaText(1)
                    End If
                Case "id"
                    If Not InvariantParse_uint(INIProperty.Value, Objects(INISectionNum).ID) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "priority"
                    If Not InvariantParse_int(INIProperty.Value, Objects(INISectionNum).Priority) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "pos"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Dim Pos As sXY_int
                    If Not InvariantParse_int(CommaText(0), Pos.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If Not InvariantParse_int(CommaText(1), Pos.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Try
                        Objects(INISectionNum).Pos = New clsXY_int(Pos)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "heading"
                    Dim dblTemp As Double
                    If Not InvariantParse_dbl(INIProperty.Value, dblTemp) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If dblTemp < 0.0# Or dblTemp >= 360.0# Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).Heading = dblTemp
                Case "unitgroup"
                    Objects(INISectionNum).UnitGroup = INIProperty.Value
                Case "health"
                    Dim NewHealth As Double
                    If Not InvariantParse_dbl(INIProperty.Value, NewHealth) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If NewHealth < 0.0# Or NewHealth >= 1.0# Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).Health = NewHealth
                Case "walltype"
                    Dim WallType As Integer = -1
                    If Not InvariantParse_int(INIProperty.Value, WallType) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If WallType >= 0 And WallType <= 3 Then
                        Objects(INISectionNum).WallType = WallType
                    End If
                Case "scriptlabel"
                    Objects(INISectionNum).Label = INIProperty.Value
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Function Write_FMap(Path As String, Overwrite As Boolean, Compress As Boolean) As clsResult
        Dim ReturnResult As New clsResult("Writing FMap to " & ControlChars.Quote & Path & ControlChars.Quote)

        If Not Overwrite Then
            If IO.File.Exists(Path) Then
                ReturnResult.ProblemAdd("The file already exists")
                Return ReturnResult
            End If
        End If

        Dim FileStream As IO.FileStream
        Try
            FileStream = IO.File.Create(Path)
        Catch ex As Exception
            ReturnResult.ProblemAdd("Unable to create file")
            Return ReturnResult
        End Try

        Dim WZStream As New Zip.ZipOutputStream(FileStream)
        WZStream.UseZip64 = Zip.UseZip64.Off
        If Compress Then
            WZStream.SetLevel(9)
        Else
            WZStream.SetLevel(0)
        End If

        Dim BinaryWriter As New IO.BinaryWriter(WZStream, UTF8Encoding)
        Dim StreamWriter As New IO.StreamWriter(WZStream, UTF8Encoding)
        Dim ZipEntry As Zip.ZipEntry
        Dim ZipPath As String

        ZipPath = "Info.ini"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            Dim INI_Info As New clsINIWrite
            INI_Info.File = StreamWriter
            ReturnResult.Add(Serialize_FMap_Info(INI_Info))

            StreamWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "VertexHeight.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Add(Serialize_FMap_VertexHeight(BinaryWriter))

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "VertexTerrain.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Add(Serialize_FMap_VertexTerrain(BinaryWriter))

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "TileTexture.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Add(Serialize_FMap_TileTexture(BinaryWriter))

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "TileOrientation.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Add(Serialize_FMap_TileOrientation(BinaryWriter))

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "TileCliff.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Add(Serialize_FMap_TileCliff(BinaryWriter))

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "Roads.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Add(Serialize_FMap_Roads(BinaryWriter))

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "Objects.ini"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            Dim INI_Objects As New clsINIWrite
            INI_Objects.File = StreamWriter
            ReturnResult.Add(Serialize_FMap_Objects(INI_Objects))

            StreamWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "Gateways.ini"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            Dim INI_Gateways As New clsINIWrite
            INI_Gateways.File = StreamWriter
            ReturnResult.Add(Serialize_FMap_Gateways(INI_Gateways))

            StreamWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "TileTypes.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Add(Serialize_FMap_TileTypes(BinaryWriter))

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "ScriptLabels.ini"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            Dim INI_ScriptLabels As New clsINIWrite
            INI_ScriptLabels.File = StreamWriter
            ReturnResult.Add(Serialize_WZ_LabelsINI(INI_ScriptLabels, -1))

            StreamWriter.Flush()
            WZStream.CloseEntry()
        End If

        WZStream.Finish()
        WZStream.Close()
        BinaryWriter.Close()
        Return ReturnResult
    End Function

    Public Function Serialize_FMap_Info(File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult("Serializing general map info")

        Try
            If Tileset Is Nothing Then

            ElseIf Tileset Is Tileset_Arizona Then
                File.Property_Append("Tileset", "Arizona")
            ElseIf Tileset Is Tileset_Urban Then
                File.Property_Append("Tileset", "Urban")
            ElseIf Tileset Is Tileset_Rockies Then
                File.Property_Append("Tileset", "Rockies")
            End If

            File.Property_Append("Size", InvariantToString_int(Terrain.TileSize.X) & ", " & InvariantToString_int(Terrain.TileSize.Y))

            File.Property_Append("AutoScrollLimits", InvariantToString_bool(InterfaceOptions.AutoScrollLimits))
            File.Property_Append("ScrollMinX", InvariantToString_int(InterfaceOptions.ScrollMin.X))
            File.Property_Append("ScrollMinY", InvariantToString_int(InterfaceOptions.ScrollMin.Y))
            File.Property_Append("ScrollMaxX", InvariantToString_uint(InterfaceOptions.ScrollMax.X))
            File.Property_Append("ScrollMaxY", InvariantToString_uint(InterfaceOptions.ScrollMax.Y))

            File.Property_Append("Name", InterfaceOptions.CompileName)
            File.Property_Append("Players", InterfaceOptions.CompileMultiPlayers)
            File.Property_Append("XPlayerLev", InvariantToString_bool(InterfaceOptions.CompileMultiXPlayers))
            File.Property_Append("Author", InterfaceOptions.CompileMultiAuthor)
            File.Property_Append("License", InterfaceOptions.CompileMultiLicense)
            If InterfaceOptions.CampaignGameType >= 0 Then
                File.Property_Append("CampType", InvariantToString_int(InterfaceOptions.CampaignGameType))
            End If
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_VertexHeight(File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult("Serializing vertex heights")
        Dim X As Integer
        Dim Y As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X
                    File.Write(CByte(Terrain.Vertices(X, Y).Height))
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_VertexTerrain(File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult("Serializing vertex terrain")

        Dim X As Integer
        Dim Y As Integer
        Dim ErrorCount As Integer
        Dim Value As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X
                    If Terrain.Vertices(X, Y).Terrain Is Nothing Then
                        Value = 0
                    ElseIf Terrain.Vertices(X, Y).Terrain.Num < 0 Then
                        ErrorCount += 1
                        Value = 0
                    Else
                        Value = Terrain.Vertices(X, Y).Terrain.Num + 1
                        If Value > 255 Then
                            ErrorCount += 1
                            Value = 0
                        End If
                    End If
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        If ErrorCount > 0 Then
            ReturnResult.WarningAdd(ErrorCount & " vertices had an invalid painted terrain number.")
        End If

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_TileTexture(File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult("Serializing tile textures")

        Dim X As Integer
        Dim Y As Integer
        Dim ErrorCount As Integer
        Dim Value As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Value = Terrain.Tiles(X, Y).Texture.TextureNum + 1
                    If Value < 0 Or Value > 255 Then
                        ErrorCount += 1
                        Value = 0
                    End If
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        If ErrorCount > 0 Then
            ReturnResult.WarningAdd(ErrorCount & " tiles had an invalid texture number.")
        End If

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_TileOrientation(File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult("Serializing tile orientations")
        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Value = 0
                    If Terrain.Tiles(X, Y).Texture.Orientation.SwitchedAxes Then
                        Value += 8
                    End If
                    If Terrain.Tiles(X, Y).Texture.Orientation.ResultXFlip Then
                        Value += 4
                    End If
                    If Terrain.Tiles(X, Y).Texture.Orientation.ResultYFlip Then
                        Value += 2
                    End If
                    If Terrain.Tiles(X, Y).Tri Then
                        Value += 1
                    End If
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_TileCliff(File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult("Serializing tile cliffs")

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim DownSideValue As Integer
        Dim ErrorCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Value = 0
                    If Terrain.Tiles(X, Y).Tri Then
                        If Terrain.Tiles(X, Y).TriTopLeftIsCliff Then
                            Value += 2
                        End If
                        If Terrain.Tiles(X, Y).TriBottomRightIsCliff Then
                            Value += 1
                        End If
                    Else
                        If Terrain.Tiles(X, Y).TriBottomLeftIsCliff Then
                            Value += 2
                        End If
                        If Terrain.Tiles(X, Y).TriTopRightIsCliff Then
                            Value += 1
                        End If
                    End If
                    If Terrain.Tiles(X, Y).Terrain_IsCliff Then
                        Value += 4
                    End If
                    If IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_None) Then
                        DownSideValue = 0
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_Top) Then
                        DownSideValue = 1
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_Left) Then
                        DownSideValue = 2
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_Right) Then
                        DownSideValue = 3
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_Bottom) Then
                        DownSideValue = 4
                    Else
                        ErrorCount += 1
                        DownSideValue = 0
                    End If
                    Value += DownSideValue * 8
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        If ErrorCount > 0 Then
            ReturnResult.WarningAdd(ErrorCount & " tiles had an invalid cliff down side.")
        End If

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_Roads(File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult("Serializing roads")

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim ErrorCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X - 1
                    If Terrain.SideH(X, Y).Road Is Nothing Then
                        Value = 0
                    ElseIf Terrain.SideH(X, Y).Road.Num < 0 Then
                        ErrorCount += 1
                        Value = 0
                    Else
                        Value = Terrain.SideH(X, Y).Road.Num + 1
                        If Value > 255 Then
                            ErrorCount += 1
                            Value = 0
                        End If
                    End If
                    File.Write(CByte(Value))
                Next
            Next
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X
                    If Terrain.SideV(X, Y).Road Is Nothing Then
                        Value = 0
                    ElseIf Terrain.SideV(X, Y).Road.Num < 0 Then
                        ErrorCount += 1
                        Value = 0
                    Else
                        Value = Terrain.SideV(X, Y).Road.Num + 1
                        If Value > 255 Then
                            ErrorCount += 1
                            Value = 0
                        End If
                    End If
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        If ErrorCount > 0 Then
            ReturnResult.WarningAdd(ErrorCount & " sides had an invalid road number.")
        End If

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_Objects(File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult("Serializing objects")

        Dim A As Integer
        Dim Unit As clsUnit
        Dim Droid As clsDroidDesign
        Dim WarningCount As Integer
        Dim Text As String = Nothing

        Try
            For A = 0 To Units.Count - 1
                Unit = Units.Item(A)
                File.SectionName_Append(InvariantToString_int(A))
                Select Case Unit.Type.Type
                    Case clsUnitType.enumType.Feature
                        File.Property_Append("Type", "Feature, " & CType(Unit.Type, clsFeatureType).Code)
                    Case clsUnitType.enumType.PlayerStructure
                        Dim StructureType As clsStructureType = CType(Unit.Type, clsStructureType)
                        File.Property_Append("Type", "Structure, " & StructureType.Code)
                        If StructureType.WallLink.IsConnected Then
                            File.Property_Append("WallType", InvariantToString_int(StructureType.WallLink.ArrayPosition))
                        End If
                    Case clsUnitType.enumType.PlayerDroid
                        Droid = CType(Unit.Type, clsDroidDesign)
                        If Droid.IsTemplate Then
                            File.Property_Append("Type", "DroidTemplate, " & CType(Unit.Type, clsDroidTemplate).Code)
                        Else
                            File.Property_Append("Type", "DroidDesign")
                            If Droid.TemplateDroidType IsNot Nothing Then
                                File.Property_Append("DroidType", Droid.TemplateDroidType.TemplateCode)
                            End If
                            If Droid.Body IsNot Nothing Then
                                File.Property_Append("Body", Droid.Body.Code)
                            End If
                            If Droid.Propulsion IsNot Nothing Then
                                File.Property_Append("Propulsion", Droid.Propulsion.Code)
                            End If
                            File.Property_Append("TurretCount", InvariantToString_byte(Droid.TurretCount))
                            If Droid.Turret1 IsNot Nothing Then
                                If Droid.Turret1.GetTurretTypeName(Text) Then
                                    File.Property_Append("Turret1", Text & ", " & Droid.Turret1.Code)
                                End If
                            End If
                            If Droid.Turret2 IsNot Nothing Then
                                If Droid.Turret2.GetTurretTypeName(Text) Then
                                    File.Property_Append("Turret2", Text & ", " & Droid.Turret2.Code)
                                End If
                            End If
                            If Droid.Turret3 IsNot Nothing Then
                                If Droid.Turret3.GetTurretTypeName(Text) Then
                                    File.Property_Append("Turret3", Text & ", " & Droid.Turret3.Code)
                                End If
                            End If
                        End If
                    Case Else
                        WarningCount += 1
                End Select
                File.Property_Append("ID", InvariantToString_uint(Unit.ID))
                File.Property_Append("Priority", InvariantToString_int(Unit.SavePriority))
                File.Property_Append("Pos", InvariantToString_int(Unit.Pos.Horizontal.X) & ", " & InvariantToString_int(Unit.Pos.Horizontal.Y))
                File.Property_Append("Heading", InvariantToString_int(Unit.Rotation))
                File.Property_Append("UnitGroup", Unit.UnitGroup.GetFMapINIPlayerText)
                If Unit.Health < 1.0# Then
                    File.Property_Append("Health", InvariantToString_dbl(Unit.Health))
                End If
                If Unit.Label IsNot Nothing Then
                    File.Property_Append("ScriptLabel", Unit.Label)
                End If
                File.Gap_Append()
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        If WarningCount > 0 Then
            ReturnResult.WarningAdd("Error: " & WarningCount & " units were of an unhandled type.")
        End If

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_Gateways(File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult("Serializing gateways")
        Dim A As Integer
        Dim Gateway As clsGateway

        Try
            For A = 0 To Gateways.Count - 1
                Gateway = Gateways.Item(A)
                File.SectionName_Append(InvariantToString_int(A))
                File.Property_Append("AX", InvariantToString_int(Gateway.PosA.X))
                File.Property_Append("AY", InvariantToString_int(Gateway.PosA.Y))
                File.Property_Append("BX", InvariantToString_int(Gateway.PosB.X))
                File.Property_Append("BY", InvariantToString_int(Gateway.PosB.Y))
                File.Gap_Append()
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Serialize_FMap_TileTypes(File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult("Serializing tile types")
        Dim A As Integer

        Try
            If Tileset IsNot Nothing Then
                For A = 0 To Tileset.TileCount - 1
                    File.Write(CByte(Tile_TypeNum(A)))
                Next
            End If
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Load_FMap(Path As String) As clsResult
        Dim ReturnResult As New clsResult("Loading FMap from " & ControlChars.Quote & Path & ControlChars.Quote)

        Dim ZipSearchResult As clsZipStreamEntry
        Dim FindPath As String

        Dim ResultInfo As clsFMapInfo = Nothing

        FindPath = "info.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.ProblemAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
            Return ReturnResult
        Else
            Dim Info_StreamReader As New IO.StreamReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_Info(Info_StreamReader, ResultInfo))
            Info_StreamReader.Close()
            If ReturnResult.HasProblems Then
                Return ReturnResult
            End If
        End If

        Dim NewTerrainSize As sXY_int = ResultInfo.TerrainSize
        Tileset = ResultInfo.Tileset

        If NewTerrainSize.X <= 0 Or NewTerrainSize.X > MapMaxSize Then
            ReturnResult.ProblemAdd("Map width of " & NewTerrainSize.X & " is not valid.")
        End If
        If NewTerrainSize.Y <= 0 Or NewTerrainSize.Y > MapMaxSize Then
            ReturnResult.ProblemAdd("Map height of " & NewTerrainSize.Y & " is not valid.")
        End If
        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        SetPainterToDefaults() 'depends on tileset. must be called before loading the terrains.
        TerrainBlank(NewTerrainSize)
        TileType_Reset()

        FindPath = "vertexheight.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim VertexHeight_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_VertexHeight(VertexHeight_Reader))
            VertexHeight_Reader.Close()
        End If

        FindPath = "vertexterrain.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim VertexTerrain_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_VertexTerrain(VertexTerrain_Reader))
            VertexTerrain_Reader.Close()
        End If

        FindPath = "tiletexture.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim TileTexture_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_TileTexture(TileTexture_Reader))
            TileTexture_Reader.Close()
        End If

        FindPath = "tileorientation.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim TileOrientation_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_TileOrientation(TileOrientation_Reader))
            TileOrientation_Reader.Close()
        End If

        FindPath = "tilecliff.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim TileCliff_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_TileCliff(TileCliff_Reader))
            TileCliff_Reader.Close()
        End If

        FindPath = "roads.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim Roads_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_Roads(Roads_Reader))
            Roads_Reader.Close()
        End If

        FindPath = "objects.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim Objects_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_Objects(Objects_Reader))
            Objects_Reader.Close()
        End If

        FindPath = "gateways.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim Gateway_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_Gateways(Gateway_Reader))
            Gateway_Reader.Close()
        End If

        FindPath = "tiletypes.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.WarningAdd("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim TileTypes_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.Add(Read_FMap_TileTypes(TileTypes_Reader))
            TileTypes_Reader.Close()
        End If

        FindPath = "scriptlabels.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then

        Else
            Dim Result As New clsResult("Reading labels")
            Dim LabelsINI As New clsINIRead
            Dim LabelsINI_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            Result.Take(LabelsINI.ReadFile(LabelsINI_Reader))
            LabelsINI_Reader.Close()
            Result.Take(Read_WZ_Labels(LabelsINI, True))
            ReturnResult.Add(Result)
        End If

        InterfaceOptions = ResultInfo.InterfaceOptions

        Return ReturnResult
    End Function

    Public Class clsFMapInfo
        Inherits clsINIRead.clsTranslator

        Public TerrainSize As sXY_int = New sXY_int(-1, -1)
        Public InterfaceOptions As New clsMap.clsInterfaceOptions
        Public Tileset As clsTileset

        Public Overrides Function Translate(INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "tileset"
                    Select Case INIProperty.Value.ToLower
                        Case "arizona"
                            Tileset = Tileset_Arizona
                        Case "urban"
                            Tileset = Tileset_Urban
                        Case "rockies"
                            Tileset = Tileset_Rockies
                        Case Else
                            Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Select
                Case "size"
                    Dim CommaText() As String = INIProperty.Value.Split(","c)
                    If CommaText.GetUpperBound(0) + 1 < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Dim A As Integer
                    For A = 0 To CommaText.GetUpperBound(0)
                        CommaText(A) = CommaText(A).Trim
                    Next
                    Dim NewSize As sXY_int
                    If Not InvariantParse_int(CommaText(0), NewSize.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If Not InvariantParse_int(CommaText(1), NewSize.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If NewSize.X < 1 Or NewSize.Y < 1 Or NewSize.X > MapMaxSize Or NewSize.Y > MapMaxSize Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    TerrainSize = NewSize
                Case "autoscrolllimits"
                    If Not InvariantParse_bool(INIProperty.Value, InterfaceOptions.AutoScrollLimits) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "scrollminx"
                    If Not InvariantParse_int(INIProperty.Value, InterfaceOptions.ScrollMin.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "scrollminy"
                    If Not InvariantParse_int(INIProperty.Value, InterfaceOptions.ScrollMin.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "scrollmaxx"
                    If Not InvariantParse_uint(INIProperty.Value, InterfaceOptions.ScrollMax.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "scrollmaxy"
                    If Not InvariantParse_uint(INIProperty.Value, InterfaceOptions.ScrollMax.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "name"
                    InterfaceOptions.CompileName = INIProperty.Value
                Case "players"
                    InterfaceOptions.CompileMultiPlayers = INIProperty.Value
                Case "xplayerlev"
                    If Not InvariantParse_bool(INIProperty.Value, InterfaceOptions.CompileMultiXPlayers) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "author"
                    InterfaceOptions.CompileMultiAuthor = INIProperty.Value
                Case "license"
                    InterfaceOptions.CompileMultiLicense = INIProperty.Value
                Case "camptime"
                    'allow and ignore
                Case "camptype"
                    If Not InvariantParse_int(INIProperty.Value, InterfaceOptions.CampaignGameType) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Private Function Read_FMap_Info(File As IO.StreamReader, ByRef ResultInfo As clsFMapInfo) As clsResult
        Dim ReturnResult As New clsResult("Read general map info")

        Dim InfoINI As New clsINIRead.clsSection
        ReturnResult.Take(InfoINI.ReadFile(File))

        ResultInfo = New clsFMapInfo
        ReturnResult.Take(InfoINI.Translate(ResultInfo))

        If ResultInfo.TerrainSize.X < 0 Or ResultInfo.TerrainSize.Y < 0 Then
            ReturnResult.ProblemAdd("Map size was not specified or was invalid.")
        End If

        Return ReturnResult
    End Function

    Private Function Read_FMap_VertexHeight(File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult("Reading vertex heights")

        Dim X As Integer
        Dim Y As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X
                    Terrain.Vertices(X, Y).Height = File.ReadByte
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        If File.PeekChar >= 0 Then
            ReturnResult.WarningAdd("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Private Function Read_FMap_VertexTerrain(File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult("Reading vertex terrain")

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim byteTemp As Byte
        Dim WarningCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X
                    byteTemp = File.ReadByte
                    Value = CInt(byteTemp) - 1
                    If Value < 0 Then
                        Terrain.Vertices(X, Y).Terrain = Nothing
                    ElseIf Value >= Painter.TerrainCount Then
                        If WarningCount < 16 Then
                            ReturnResult.WarningAdd("Painted terrain at vertex " & X & ", " & Y & " was invalid.")
                        End If
                        WarningCount += 1
                        Terrain.Vertices(X, Y).Terrain = Nothing
                    Else
                        Terrain.Vertices(X, Y).Terrain = Painter.Terrains(Value)
                    End If
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        If WarningCount > 0 Then
            ReturnResult.WarningAdd(WarningCount & " painted terrain vertices were invalid.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.WarningAdd("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileTexture(File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult("Reading tile textures")

        Dim X As Integer
        Dim Y As Integer
        Dim byteTemp As Byte

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    byteTemp = File.ReadByte
                    Terrain.Tiles(X, Y).Texture.TextureNum = CInt(byteTemp) - 1
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        If File.PeekChar >= 0 Then
            ReturnResult.WarningAdd("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileOrientation(File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult("Reading tile orientations")

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim PartValue As Integer
        Dim WarningCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Value = File.ReadByte

                    PartValue = CInt(Math.Floor(Value / 16))
                    If PartValue > 0 Then
                        If WarningCount < 16 Then
                            ReturnResult.WarningAdd("Unknown bits used for tile " & X & ", " & Y & ".")
                        End If
                        WarningCount += 1
                    End If
                    Value -= PartValue * 16

                    PartValue = CInt(Int(Value / 8.0#))
                    Terrain.Tiles(X, Y).Texture.Orientation.SwitchedAxes = (PartValue > 0)
                    Value -= PartValue * 8

                    PartValue = CInt(Int(Value / 4.0#))
                    Terrain.Tiles(X, Y).Texture.Orientation.ResultXFlip = (PartValue > 0)
                    Value -= PartValue * 4

                    PartValue = CInt(Int(Value / 2.0#))
                    Terrain.Tiles(X, Y).Texture.Orientation.ResultYFlip = (PartValue > 0)
                    Value -= PartValue * 2

                    PartValue = Value
                    Terrain.Tiles(X, Y).Tri = (PartValue > 0)
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        If WarningCount > 0 Then
            ReturnResult.WarningAdd(WarningCount & " tiles had unknown bits used.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.WarningAdd("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileCliff(File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult("Reading tile cliffs")

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim PartValue As Integer
        Dim DownSideWarningCount As Integer
        Dim WarningCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1

                    Value = File.ReadByte

                    PartValue = CInt(Int(Value / 64.0#))
                    If PartValue > 0 Then
                        If WarningCount < 16 Then
                            ReturnResult.WarningAdd("Unknown bits used for tile " & X & ", " & Y & ".")
                        End If
                        WarningCount += 1
                    End If
                    Value -= PartValue * 64

                    PartValue = CInt(Int(Value / 8.0#))
                    Select Case PartValue
                        Case 0
                            Terrain.Tiles(X, Y).DownSide = TileDirection_None
                        Case 1
                            Terrain.Tiles(X, Y).DownSide = TileDirection_Top
                        Case 2
                            Terrain.Tiles(X, Y).DownSide = TileDirection_Left
                        Case 3
                            Terrain.Tiles(X, Y).DownSide = TileDirection_Right
                        Case 4
                            Terrain.Tiles(X, Y).DownSide = TileDirection_Bottom
                        Case Else
                            Terrain.Tiles(X, Y).DownSide = TileDirection_None
                            If DownSideWarningCount < 16 Then
                                ReturnResult.WarningAdd("Down side value for tile " & X & ", " & Y & " was invalid.")
                            End If
                            DownSideWarningCount += 1
                    End Select
                    Value -= PartValue * 8

                    PartValue = CInt(Int(Value / 4.0#))
                    Terrain.Tiles(X, Y).Terrain_IsCliff = (PartValue > 0)
                    Value -= PartValue * 4

                    PartValue = CInt(Int(Value / 2.0#))
                    If Terrain.Tiles(X, Y).Tri Then
                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = (PartValue > 0)
                    Else
                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = (PartValue > 0)
                    End If
                    Value -= PartValue * 2

                    PartValue = Value
                    If Terrain.Tiles(X, Y).Tri Then
                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = (PartValue > 0)
                    Else
                        Terrain.Tiles(X, Y).TriTopRightIsCliff = (PartValue > 0)
                    End If
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        If WarningCount > 0 Then
            ReturnResult.WarningAdd(WarningCount & " tiles had unknown bits used.")
        End If
        If DownSideWarningCount > 0 Then
            ReturnResult.WarningAdd(DownSideWarningCount & " tiles had invalid down side values.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.WarningAdd("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_Roads(File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult("Reading roads")

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim WarningCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X - 1
                    Value = File.ReadByte - 1
                    If Value < 0 Then
                        Terrain.SideH(X, Y).Road = Nothing
                    ElseIf Value >= Painter.RoadCount Then
                        If WarningCount < 16 Then
                            ReturnResult.WarningAdd("Invalid road value for horizontal side " & X & ", " & Y & ".")
                        End If
                        WarningCount += 1
                        Terrain.SideH(X, Y).Road = Nothing
                    Else
                        Terrain.SideH(X, Y).Road = Painter.Roads(Value)
                    End If
                Next
            Next
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X
                    Value = File.ReadByte - 1
                    If Value < 0 Then
                        Terrain.SideV(X, Y).Road = Nothing
                    ElseIf Value >= Painter.RoadCount Then
                        If WarningCount < 16 Then
                            ReturnResult.WarningAdd("Invalid road value for vertical side " & X & ", " & Y & ".")
                        End If
                        WarningCount += 1
                        Terrain.SideV(X, Y).Road = Nothing
                    Else
                        Terrain.SideV(X, Y).Road = Painter.Roads(Value)
                    End If
                Next
            Next
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        If WarningCount > 0 Then
            ReturnResult.WarningAdd(WarningCount & " sides had an invalid road value.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.WarningAdd("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Private Function Read_FMap_Objects(File As IO.StreamReader) As clsResult
        Dim ReturnResult As New clsResult("Reading objects")

        Dim A As Integer

        Dim ObjectsINI As New clsINIRead
        ReturnResult.Take(ObjectsINI.ReadFile(File))

        Dim INIObjects As New clsFMap_INIObjects(ObjectsINI.Sections.Count)
        ReturnResult.Take(ObjectsINI.Translate(INIObjects))

        Dim DroidComponentUnknownCount As Integer
        Dim ObjectTypeMissingCount As Integer
        Dim ObjectPlayerNumInvalidCount As Integer
        Dim ObjectPosInvalidCount As Integer
        Dim DesignTypeUnspecifiedCount As Integer
        Dim UnknownUnitTypeCount As Integer
        Dim MaxUnknownUnitTypeWarningCount As Integer = 16

        Dim DroidDesign As clsDroidDesign
        Dim NewObject As clsUnit
        Dim UnitAdd As New clsMap.clsUnitAdd
        Dim UnitType As clsUnitType
        Dim IsDesign As Boolean
        Dim UnitGroup As clsUnitGroup
        Dim ZeroPos As New sXY_int(0, 0)
        Dim AvailableID As UInteger

        UnitAdd.Map = Me

        AvailableID = 1UI
        For A = 0 To INIObjects.ObjectCount - 1
            If INIObjects.Objects(A).ID >= AvailableID Then
                AvailableID = INIObjects.Objects(A).ID + 1UI
            End If
        Next
        For A = 0 To INIObjects.ObjectCount - 1
            If INIObjects.Objects(A).Pos Is Nothing Then
                ObjectPosInvalidCount += 1
            ElseIf Not PosIsWithinTileArea(INIObjects.Objects(A).Pos.XY, ZeroPos, Terrain.TileSize) Then
                ObjectPosInvalidCount += 1
            Else
                UnitType = Nothing
                If INIObjects.Objects(A).Type <> clsUnitType.enumType.Unspecified Then
                    IsDesign = False
                    If INIObjects.Objects(A).Type = clsUnitType.enumType.PlayerDroid Then
                        If Not INIObjects.Objects(A).IsTemplate Then
                            IsDesign = True
                        End If
                    End If
                    If IsDesign Then
                        DroidDesign = New clsDroidDesign
                        DroidDesign.TemplateDroidType = INIObjects.Objects(A).TemplateDroidType
                        If DroidDesign.TemplateDroidType Is Nothing Then
                            DroidDesign.TemplateDroidType = TemplateDroidType_Droid
                            DesignTypeUnspecifiedCount += 1
                        End If
                        If INIObjects.Objects(A).BodyCode <> "" Then
                            DroidDesign.Body = ObjectData.FindOrCreateBody(INIObjects.Objects(A).BodyCode)
                            If DroidDesign.Body.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        If INIObjects.Objects(A).PropulsionCode <> "" Then
                            DroidDesign.Propulsion = ObjectData.FindOrCreatePropulsion(INIObjects.Objects(A).PropulsionCode)
                            If DroidDesign.Propulsion.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        DroidDesign.TurretCount = CByte(INIObjects.Objects(A).TurretCount)
                        If INIObjects.Objects(A).TurretCodes(0) <> "" Then
                            DroidDesign.Turret1 = ObjectData.FindOrCreateTurret(INIObjects.Objects(A).TurretTypes(0), INIObjects.Objects(A).TurretCodes(0))
                            If DroidDesign.Turret1.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        If INIObjects.Objects(A).TurretCodes(1) <> "" Then
                            DroidDesign.Turret2 = ObjectData.FindOrCreateTurret(INIObjects.Objects(A).TurretTypes(1), INIObjects.Objects(A).TurretCodes(1))
                            If DroidDesign.Turret2.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        If INIObjects.Objects(A).TurretCodes(2) <> "" Then
                            DroidDesign.Turret3 = ObjectData.FindOrCreateTurret(INIObjects.Objects(A).TurretTypes(2), INIObjects.Objects(A).TurretCodes(2))
                            If DroidDesign.Turret3.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        DroidDesign.UpdateAttachments()
                        UnitType = DroidDesign
                    Else
                        UnitType = ObjectData.FindOrCreateUnitType(INIObjects.Objects(A).Code, INIObjects.Objects(A).Type, INIObjects.Objects(A).WallType)
                        If UnitType.IsUnknown Then
                            If UnknownUnitTypeCount < MaxUnknownUnitTypeWarningCount Then
                                ReturnResult.WarningAdd(ControlChars.Quote & INIObjects.Objects(A).Code & ControlChars.Quote & " is not a loaded object.")
                            End If
                            UnknownUnitTypeCount += 1
                        End If
                    End If
                End If
                If UnitType Is Nothing Then
                    ObjectTypeMissingCount += 1
                Else
                    NewObject = New clsUnit
                    NewObject.Type = UnitType
                    NewObject.Pos.Horizontal.X = INIObjects.Objects(A).Pos.X
                    NewObject.Pos.Horizontal.Y = INIObjects.Objects(A).Pos.Y
                    NewObject.Health = INIObjects.Objects(A).Health
                    NewObject.SavePriority = INIObjects.Objects(A).Priority
                    NewObject.Rotation = CInt(INIObjects.Objects(A).Heading)
                    If NewObject.Rotation >= 360 Then
                        NewObject.Rotation -= 360
                    End If
                    If INIObjects.Objects(A).UnitGroup = Nothing Or INIObjects.Objects(A).UnitGroup = "" Then
                        If INIObjects.Objects(A).Type <> clsUnitType.enumType.Feature Then
                            ObjectPlayerNumInvalidCount += 1
                        End If
                        NewObject.UnitGroup = ScavengerUnitGroup
                    Else
                        If INIObjects.Objects(A).UnitGroup.ToLower = "scavenger" Then
                            NewObject.UnitGroup = ScavengerUnitGroup
                        Else
                            Dim PlayerNum As UInteger
                            Try
                                If Not InvariantParse_uint(INIObjects.Objects(A).UnitGroup, PlayerNum) Then
                                    Throw New Exception
                                End If
                                If PlayerNum < PlayerCountMax Then
                                    UnitGroup = UnitGroups.Item(CInt(PlayerNum))
                                Else
                                    UnitGroup = ScavengerUnitGroup
                                    ObjectPlayerNumInvalidCount += 1
                                End If
                            Catch ex As Exception
                                ObjectPlayerNumInvalidCount += 1
                                UnitGroup = ScavengerUnitGroup
                            End Try
                            NewObject.UnitGroup = UnitGroup
                        End If
                    End If
                    If INIObjects.Objects(A).ID = 0UI Then
                        INIObjects.Objects(A).ID = AvailableID
                        ZeroIDWarning(NewObject, INIObjects.Objects(A).ID, ReturnResult)
                    End If
                    UnitAdd.NewUnit = NewObject
                    UnitAdd.ID = INIObjects.Objects(A).ID
                    UnitAdd.Label = INIObjects.Objects(A).Label
                    UnitAdd.Perform()
                    ErrorIDChange(INIObjects.Objects(A).ID, NewObject, "Read_FMap_Objects")
                    If AvailableID = INIObjects.Objects(A).ID Then
                        AvailableID = NewObject.ID + 1UI
                    End If
                End If
            End If
        Next

        If UnknownUnitTypeCount > MaxUnknownUnitTypeWarningCount Then
            ReturnResult.WarningAdd(UnknownUnitTypeCount & " objects were not in the loaded object data.")
        End If
        If ObjectTypeMissingCount > 0 Then
            ReturnResult.WarningAdd(ObjectTypeMissingCount & " objects did not specify a type and were ignored.")
        End If
        If DroidComponentUnknownCount > 0 Then
            ReturnResult.WarningAdd(DroidComponentUnknownCount & " components used by droids were loaded as unknowns.")
        End If
        If ObjectPlayerNumInvalidCount > 0 Then
            ReturnResult.WarningAdd(ObjectPlayerNumInvalidCount & " objects had an invalid player number and were set to player 0.")
        End If
        If ObjectPosInvalidCount > 0 Then
            ReturnResult.WarningAdd(ObjectPosInvalidCount & " objects had a position that was off-map and were ignored.")
        End If
        If DesignTypeUnspecifiedCount > 0 Then
            ReturnResult.WarningAdd(DesignTypeUnspecifiedCount & " designed droids did not specify a template droid type and were set to droid.")
        End If

        Return ReturnResult
    End Function

    Public Class clsFMap_INIGateways
        Inherits clsINIRead.clsSectionTranslator

        Public Structure sGateway
            Public PosA As sXY_int
            Public PosB As sXY_int
        End Structure
        Public Gateways() As sGateway
        Public GatewayCount As Integer

        Public Sub New(NewGatewayCount As Integer)
            Dim A As Integer

            GatewayCount = NewGatewayCount
            ReDim Gateways(GatewayCount - 1)
            For A = 0 To GatewayCount - 1
                Gateways(A).PosA.X = -1
                Gateways(A).PosA.Y = -1
                Gateways(A).PosB.X = -1
                Gateways(A).PosB.Y = -1
            Next
        End Sub

        Public Overrides Function Translate(INISectionNum As Integer, INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "ax"
                    If Not InvariantParse_int(INIProperty.Value, Gateways(INISectionNum).PosA.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "ay"
                    If Not InvariantParse_int(INIProperty.Value, Gateways(INISectionNum).PosA.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "bx"
                    If Not InvariantParse_int(INIProperty.Value, Gateways(INISectionNum).PosB.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "by"
                    If Not InvariantParse_int(INIProperty.Value, Gateways(INISectionNum).PosB.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Function Read_FMap_Gateways(File As IO.StreamReader) As clsResult
        Dim ReturnResult As New clsResult("Reading gateways")

        Dim GatewaysINI As New clsINIRead
        ReturnResult.Take(GatewaysINI.ReadFile(File))

        Dim INIGateways As New clsFMap_INIGateways(GatewaysINI.Sections.Count)
        ReturnResult.Take(GatewaysINI.Translate(INIGateways))

        Dim A As Integer
        Dim InvalidGatewayCount As Integer = 0

        For A = 0 To INIGateways.GatewayCount - 1
            If GatewayCreate(INIGateways.Gateways(A).PosA, INIGateways.Gateways(A).PosB) Is Nothing Then
                InvalidGatewayCount += 1
            End If
        Next

        If InvalidGatewayCount > 0 Then
            ReturnResult.WarningAdd(InvalidGatewayCount & " gateways were invalid.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileTypes(File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult("Reading tile types")

        Dim A As Integer
        Dim byteTemp As Byte
        Dim InvalidTypeCount As Integer

        Try
            If Tileset IsNot Nothing Then
                For A = 0 To Tileset.TileCount - 1
                    byteTemp = File.ReadByte()
                    If byteTemp >= TileTypes.Count Then
                        InvalidTypeCount += 1
                    Else
                        Tile_TypeNum(A) = byteTemp
                    End If
                Next
            End If
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        If InvalidTypeCount > 0 Then
            ReturnResult.WarningAdd(InvalidTypeCount & " tile types were invalid.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.WarningAdd("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function
End Class
