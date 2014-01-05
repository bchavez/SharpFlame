Imports OpenTK.Graphics.OpenGL

Public Class clsObjectData

    Public UnitTypes As New ConnectedList(Of clsUnitType, clsObjectData)(Me)

    Public FeatureTypes As New ConnectedList(Of clsFeatureType, clsObjectData)(Me)
    Public StructureTypes As New ConnectedList(Of clsStructureType, clsObjectData)(Me)
    Public DroidTemplates As New ConnectedList(Of clsDroidTemplate, clsObjectData)(Me)

    Public WallTypes As New ConnectedList(Of clsWallType, clsObjectData)(Me)

    Public Bodies As New ConnectedList(Of clsBody, clsObjectData)(Me)
    Public Propulsions As New ConnectedList(Of clsPropulsion, clsObjectData)(Me)
    Public Turrets As New ConnectedList(Of clsTurret, clsObjectData)(Me)
    Public Weapons As New ConnectedList(Of clsWeapon, clsObjectData)(Me)
    Public Sensors As New ConnectedList(Of clsSensor, clsObjectData)(Me)
    Public Repairs As New ConnectedList(Of clsRepair, clsObjectData)(Me)
    Public Constructors As New ConnectedList(Of clsConstruct, clsObjectData)(Me)
    Public Brains As New ConnectedList(Of clsBrain, clsObjectData)(Me)
    Public ECMs As New ConnectedList(Of clsECM, clsObjectData)(Me)

    Public Class clsTexturePage
        Public FileTitle As String
        Public GLTexture_Num As Integer
    End Class
    Public TexturePages As New SimpleList(Of clsTexturePage)

    Public Class clsPIE
        Public Path As String
        Public LCaseFileTitle As String
        Public Model As clsModel
    End Class

    Public Class clsTextFile
        Public SubDirectory As String
        Public FieldCount As Integer = 0
        Public UniqueField As Integer = 0

        Public ResultData As New SimpleList(Of String())

        Public Function CalcIsFieldCountValid() As Boolean

            Dim Text() As String
            For Each Text In ResultData
                If Text.GetLength(0) <> FieldCount Then
                    Return False
                End If
            Next

            Return True
        End Function

        Public Function CalcUniqueField() As Boolean
            Dim A As Integer
            Dim B As Integer
            Dim Text As String

            If UniqueField >= 0 Then
                For A = 0 To ResultData.Count - 1
                    Text = ResultData(A)(UniqueField)
                    For B = A + 1 To ResultData.Count - 1
                        If Text = ResultData(B)(UniqueField) Then
                            Return False
                        End If
                    Next
                Next
            End If

            Return True
        End Function

        Public Function LoadCommaFile(Path As String) As clsResult
            Dim Result As New clsResult("Loading comma separated file " & ControlChars.Quote & SubDirectory & ControlChars.Quote)
            Dim Reader As IO.StreamReader

            Try
                Reader = New IO.StreamReader(Path & SubDirectory, UTF8Encoding)
            Catch ex As Exception
                Result.ProblemAdd(ex.Message)
                Return Result
            End Try

            Dim Line As String
            Dim LineFields() As String
            Dim A As Integer

            Do Until Reader.EndOfStream
                Line = Reader.ReadLine
                Line = Line.Trim
                If Line.Length > 0 Then
                    LineFields = Line.Split(","c)
                    For A = 0 To LineFields.GetUpperBound(0)
                        LineFields(A) = LineFields(A).Trim
                    Next
                    ResultData.Add(LineFields)
                End If
            Loop

            Reader.Close()

            Return Result
        End Function

        Public Function LoadNamesFile(Path As String) As clsResult
            Dim Result As New clsResult("Loading names file " & ControlChars.Quote & SubDirectory & ControlChars.Quote)
            Dim File As IO.FileStream
            Dim Reader As IO.BinaryReader

            Try
                File = New IO.FileStream(Path & SubDirectory, IO.FileMode.Open)
            Catch ex As Exception
                Result.ProblemAdd(ex.Message)
                Return Result
            End Try

            Try
                Reader = New IO.BinaryReader(File, UTF8Encoding)
            Catch ex As Exception
                File.Close()
                Result.ProblemAdd(ex.Message)
                Return Result
            End Try

            Dim CurrentChar As Char
            Dim InLineComment As Boolean
            Dim InCommentBlock As Boolean
            Dim PrevChar As Char
            Dim Line As String = ""
            Dim PrevCharExists As Boolean
            Dim CurrentCharExists As Boolean = False

            Do
MonoContinueDo:
                PrevChar = CurrentChar
                PrevCharExists = CurrentCharExists
                Try
                    CurrentChar = Reader.ReadChar
                    CurrentCharExists = True
                Catch ex As Exception
                    CurrentCharExists = False
                End Try
                If CurrentCharExists Then
                    Select Case CurrentChar
                        Case ControlChars.Cr, ControlChars.Lf
                            InLineComment = False
                            If PrevCharExists Then
                                Line &= PrevChar
                            End If
                            CurrentCharExists = False

                            If Line.Length > 0 Then
                                Dim EndCodeTab As Integer = Line.IndexOf(ControlChars.Tab)
                                Dim EndCodeSpace As Integer = Line.IndexOf(" "c)
                                Dim EndCode As Integer = EndCodeTab
                                If EndCodeSpace >= 0 And (EndCodeSpace < EndCode Or EndCode < 0) Then
                                    EndCode = EndCodeSpace
                                End If
                                If EndCode >= 0 Then
                                    Dim FirstQuote As Integer = Line.IndexOf(ControlChars.Quote, EndCode + 1, Line.Length - (EndCode + 1))
                                    If FirstQuote >= 0 Then
                                        Dim SecondQuote As Integer = Line.IndexOf(ControlChars.Quote, FirstQuote + 1, Line.Length - (FirstQuote + 1))
                                        If SecondQuote >= 0 Then
                                            Dim Value(1) As String
                                            Value(0) = Line.Substring(0, EndCode)
                                            Value(1) = Line.Substring(FirstQuote + 1, SecondQuote - (FirstQuote + 1))
                                            ResultData.Add(Value)
                                        End If
                                    End If
                                End If
                                Line = ""
                            End If

                            GoTo MonoContinueDo
                        Case "*"c
                            If PrevCharExists And PrevChar = "/"c Then
                                InCommentBlock = True
                                CurrentCharExists = False
                                GoTo MonoContinueDo
                            End If
                        Case "/"c
                            If PrevCharExists Then
                                If PrevChar = "/"c Then
                                    InLineComment = True
                                    CurrentCharExists = False
                                    GoTo MonoContinueDo
                                ElseIf PrevChar = "*"c Then
                                    InCommentBlock = False
                                    CurrentCharExists = False
                                    GoTo MonoContinueDo
                                End If
                            End If
                    End Select
                Else
                    If PrevCharExists Then
                        Line &= PrevChar
                    End If
                    If Line.Length > 0 Then
                        Dim EndCodeTab As Integer = Line.IndexOf(ControlChars.Tab)
                        Dim EndCodeSpace As Integer = Line.IndexOf(" "c)
                        Dim EndCode As Integer = EndCodeTab
                        If EndCodeSpace >= 0 And (EndCodeSpace < EndCode Or EndCode < 0) Then
                            EndCode = EndCodeSpace
                        End If
                        If EndCode >= 0 Then
                            Dim FirstQuote As Integer = Line.IndexOf(ControlChars.Quote, EndCode + 1, Line.Length - (EndCode + 1))
                            If FirstQuote >= 0 Then
                                Dim SecondQuote As Integer = Line.IndexOf(ControlChars.Quote, FirstQuote + 1, Line.Length - (FirstQuote + 1))
                                If SecondQuote >= 0 Then
                                    Dim Value(1) As String
                                    Value(0) = Line.Substring(0, EndCode)
                                    Value(1) = Line.Substring(FirstQuote + 1, SecondQuote - (FirstQuote + 1))
                                    ResultData.Add(Value)
                                End If
                            End If
                        End If
                        Line = ""
                    End If

                    Exit Do
                End If
                If PrevCharExists Then
                    If Not (InCommentBlock Or InLineComment) Then
                        Line &= PrevChar
                    End If
                End If
            Loop

            Reader.Close()

            Return Result
        End Function
    End Class

    Private Structure BodyProp
        Public LeftPIE As String
        Public RightPIE As String
    End Structure

    Public Function LoadDirectory(Path As String) As clsResult
        Dim ReturnResult As New clsResult("Loading object data from " & ControlChars.Quote & Path & ControlChars.Quote)

        Path = EndWithPathSeperator(Path)

        Dim SubDirNames As String
        Dim SubDirStructures As String
        Dim SubDirBrain As String
        Dim SubDirBody As String
        Dim SubDirPropulsion As String
        Dim SubDirBodyPropulsion As String
        Dim SubDirConstruction As String
        Dim SubDirSensor As String
        Dim SubDirRepair As String
        Dim SubDirTemplates As String
        Dim SubDirWeapons As String
        Dim SubDirECM As String
        Dim SubDirFeatures As String
        Dim SubDirStructurePIE As String
        Dim SubDirBodiesPIE As String
        Dim SubDirPropPIE As String
        Dim SubDirWeaponsPIE As String
        Dim SubDirTexpages As String
        Dim SubDirAssignWeapons As String
        Dim SubDirFeaturePIE As String
        Dim SubDirStructureWeapons As String
        Dim SubDirPIEs As String

        SubDirNames = "messages" & PlatformPathSeparator & "strings" & PlatformPathSeparator & "names.txt"
        SubDirStructures = "stats" & PlatformPathSeparator & "structures.txt"
        SubDirBrain = "stats" & PlatformPathSeparator & "brain.txt"
        SubDirBody = "stats" & PlatformPathSeparator & "body.txt"
        SubDirPropulsion = "stats" & PlatformPathSeparator & "propulsion.txt"
        SubDirBodyPropulsion = "stats" & PlatformPathSeparator & "bodypropulsionimd.txt"
        SubDirConstruction = "stats" & PlatformPathSeparator & "construction.txt"
        SubDirSensor = "stats" & PlatformPathSeparator & "sensor.txt"
        SubDirRepair = "stats" & PlatformPathSeparator & "repair.txt"
        SubDirTemplates = "stats" & PlatformPathSeparator & "templates.txt"
        SubDirWeapons = "stats" & PlatformPathSeparator & "weapons.txt"
        SubDirECM = "stats" & PlatformPathSeparator & "ecm.txt"
        SubDirFeatures = "stats" & PlatformPathSeparator & "features.txt"
        SubDirPIEs = "pies" & PlatformPathSeparator
        'SubDirStructurePIE = "structs" & ospathseperator
        SubDirStructurePIE = SubDirPIEs
        'SubDirBodiesPIE = "components" & ospathseperator & "bodies" & ospathseperator 
        SubDirBodiesPIE = SubDirPIEs
        'SubDirPropPIE = "components" & ospathseperator & "prop" & ospathseperator
        SubDirPropPIE = SubDirPIEs
        'SubDirWeaponsPIE = "components" & ospathseperator & "weapons" & ospathseperator 
        SubDirWeaponsPIE = SubDirPIEs
        SubDirTexpages = "texpages" & PlatformPathSeparator
        SubDirAssignWeapons = "stats" & PlatformPathSeparator & "assignweapons.txt"
        'SubDirFeaturePIE = "features" & ospathseperator 
        SubDirFeaturePIE = SubDirPIEs
        SubDirStructureWeapons = "stats" & PlatformPathSeparator & "structureweapons.txt"

        Dim CommaFiles As New SimpleList(Of clsTextFile)

        Dim DataNames As New clsTextFile
        DataNames.SubDirectory = SubDirNames
        DataNames.UniqueField = 0

        ReturnResult.Add(DataNames.LoadNamesFile(Path))
        If Not DataNames.CalcUniqueField Then
            ReturnResult.ProblemAdd("There are two entries for the same code in " & SubDirNames & ".")
        End If

        Dim DataStructures As New clsTextFile
        DataStructures.SubDirectory = SubDirStructures
        DataStructures.FieldCount = 25
        CommaFiles.Add(DataStructures)

        Dim DataBrain As New clsTextFile
        DataBrain.SubDirectory = SubDirBrain
        DataBrain.FieldCount = 9
        CommaFiles.Add(DataBrain)

        Dim DataBody As New clsTextFile
        DataBody.SubDirectory = SubDirBody
        DataBody.FieldCount = 25
        CommaFiles.Add(DataBody)

        Dim DataPropulsion As New clsTextFile
        DataPropulsion.SubDirectory = SubDirPropulsion
        DataPropulsion.FieldCount = 12
        CommaFiles.Add(DataPropulsion)

        Dim DataBodyPropulsion As New clsTextFile
        DataBodyPropulsion.SubDirectory = SubDirBodyPropulsion
        DataBodyPropulsion.FieldCount = 5
        DataBodyPropulsion.UniqueField = -1 'no unique requirement
        CommaFiles.Add(DataBodyPropulsion)

        Dim DataConstruction As New clsTextFile
        DataConstruction.SubDirectory = SubDirConstruction
        DataConstruction.FieldCount = 12
        CommaFiles.Add(DataConstruction)

        Dim DataSensor As New clsTextFile
        DataSensor.SubDirectory = SubDirSensor
        DataSensor.FieldCount = 16
        CommaFiles.Add(DataSensor)

        Dim DataRepair As New clsTextFile
        DataRepair.SubDirectory = SubDirRepair
        DataRepair.FieldCount = 14
        CommaFiles.Add(DataRepair)

        Dim DataTemplates As New clsTextFile
        DataTemplates.SubDirectory = SubDirTemplates
        DataTemplates.FieldCount = 12
        CommaFiles.Add(DataTemplates)

        Dim DataECM As New clsTextFile
        DataECM.SubDirectory = SubDirECM
        DataECM.FieldCount = 14
        CommaFiles.Add(DataECM)

        Dim DataFeatures As New clsTextFile
        DataFeatures.SubDirectory = SubDirFeatures
        DataFeatures.FieldCount = 11
        CommaFiles.Add(DataFeatures)

        Dim DataAssignWeapons As New clsTextFile
        DataAssignWeapons.SubDirectory = SubDirAssignWeapons
        DataAssignWeapons.FieldCount = 5
        CommaFiles.Add(DataAssignWeapons)

        Dim DataWeapons As New clsTextFile
        DataWeapons.SubDirectory = SubDirWeapons
        DataWeapons.FieldCount = 53
        CommaFiles.Add(DataWeapons)

        Dim DataStructureWeapons As New clsTextFile
        DataStructureWeapons.SubDirectory = SubDirStructureWeapons
        DataStructureWeapons.FieldCount = 6
        CommaFiles.Add(DataStructureWeapons)

        Dim TextFile As clsTextFile

        For Each TextFile In CommaFiles
            Dim Result As clsResult = TextFile.LoadCommaFile(Path)
            ReturnResult.Add(Result)
            If Not Result.HasProblems Then
                If TextFile.CalcIsFieldCountValid Then
                    If Not TextFile.CalcUniqueField Then
                        ReturnResult.ProblemAdd("An entry in field " & TextFile.UniqueField & " was not unique for file " & TextFile.SubDirectory & ".")
                    End If
                Else
                    ReturnResult.ProblemAdd("There were entries with the wrong number of fields for file " & TextFile.SubDirectory & ".")
                End If
            End If
        Next

        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        'load texpages

        Dim TexFiles() As String

        Try
            TexFiles = IO.Directory.GetFiles(Path & SubDirTexpages)
        Catch ex As Exception
            ReturnResult.WarningAdd("Unable to access texture pages.")
            ReDim TexFiles(-1)
        End Try

        Dim Text As String
        Dim Bitmap As Bitmap = Nothing
        Dim InstrPos2 As Integer
        Dim BitmapTextureArgs As sBitmapGLTexture
        Dim BitmapResult As sResult

        For Each Text In TexFiles
            If Right(Text, 4).ToLower = ".png" Then
                Dim Result As New clsResult("Loading texture page " & ControlChars.Quote & Text & ControlChars.Quote)
                If IO.File.Exists(Text) Then
                    BitmapResult = LoadBitmap(Text, Bitmap)
                    Dim NewPage As New clsTexturePage
                    If BitmapResult.Success Then
                        Result.Take(BitmapIsGLCompatible(Bitmap))
                        BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest
                        BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
                        BitmapTextureArgs.TextureNum = 0
                        BitmapTextureArgs.MipMapLevel = 0
                        BitmapTextureArgs.Texture = Bitmap
                        BitmapTextureArgs.Perform()
                        NewPage.GLTexture_Num = BitmapTextureArgs.TextureNum
                    Else
                        Result.WarningAdd(BitmapResult.Problem)
                    End If
                    InstrPos2 = InStrRev(Text, PlatformPathSeparator)
                    NewPage.FileTitle = Strings.Mid(Text, InstrPos2 + 1, Text.Length - 4 - InstrPos2)
                    TexturePages.Add(NewPage)
                Else
                    Result.WarningAdd("Texture page missing (" & Text & ").")
                End If
                ReturnResult.Add(Result)
            End If
        Next

        'load PIEs

        Dim PIE_Files() As String
        Dim PIE_List As New SimpleList(Of clsPIE)
        Dim NewPIE As clsPIE

        Try
            PIE_Files = IO.Directory.GetFiles(Path & SubDirPIEs)
        Catch ex As Exception
            ReturnResult.WarningAdd("Unable to access PIE files.")
            ReDim PIE_Files(-1)
        End Try

        Dim SplitPath As sSplitPath

        For Each Text In PIE_Files
            SplitPath = New sSplitPath(Text)
            If SplitPath.FileExtension.ToLower = "pie" Then
                NewPIE = New clsPIE
                NewPIE.Path = Text
                NewPIE.LCaseFileTitle = SplitPath.FileTitle.ToLower
                PIE_List.Add(NewPIE)
            End If
        Next

        'interpret stats

        Dim Attachment As clsUnitType.clsAttachment
        Dim BaseAttachment As clsUnitType.clsAttachment
        Dim Connector As sXYZ_sng
        Dim StructureType As clsStructureType
        Dim FeatureType As clsFeatureType
        Dim Template As clsDroidTemplate
        Dim Body As clsBody
        Dim Propulsion As clsPropulsion
        Dim Construct As clsConstruct
        Dim Weapon As clsWeapon
        Dim Repair As clsRepair
        Dim Sensor As clsSensor
        Dim Brain As clsBrain
        Dim ECM As clsECM
        Dim Fields() As String

        'interpret body

        For Each Fields In DataBody.ResultData
            Body = New clsBody
            Body.ObjectDataLink.Connect(Bodies)
            Body.Code = Fields(0)
            SetComponentName(DataNames.ResultData, Body, ReturnResult)
            InvariantParse_int(Fields(6), Body.Hitpoints)
            Body.Designable = (Fields(24) <> "0")
            Body.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(7).ToLower, ReturnResult))
        Next

        'interpret propulsion

        For Each Fields In DataPropulsion.ResultData
            Propulsion = New clsPropulsion(Bodies.Count)
            Propulsion.ObjectDataLink.Connect(Propulsions)
            Propulsion.Code = Fields(0)
            SetComponentName(DataNames.ResultData, Propulsion, ReturnResult)
            InvariantParse_int(Fields(7), Propulsion.HitPoints)
            '.Propulsions(Propulsion_Num).PIE = LCase(DataPropulsion.Entries(Propulsion_Num).FieldValues(8))
            Propulsion.Designable = (Fields(11) <> "0")
        Next

        'interpret body-propulsions

        Dim BodyPropulsionPIEs(Bodies.Count - 1, Propulsions.Count - 1) As BodyProp
        For A As Integer = 0 To Bodies.Count - 1
            For B As Integer = 0 To Propulsions.Count - 1
                BodyPropulsionPIEs(A, B) = New BodyProp
                BodyPropulsionPIEs(A, B).LeftPIE = "0"
                BodyPropulsionPIEs(A, B).RightPIE = "0"
            Next
        Next

        For Each Fields In DataBodyPropulsion.ResultData
            Body = FindBodyCode(Fields(0))
            Propulsion = FindPropulsionCode(Fields(1))
            If Body IsNot Nothing And Propulsion IsNot Nothing Then
                If Fields(2) <> "0" Then
                    BodyPropulsionPIEs(Body.ObjectDataLink.ArrayPosition, Propulsion.ObjectDataLink.ArrayPosition).LeftPIE = Fields(2).ToLower
                End If
                If Fields(3) <> "0" Then
                    BodyPropulsionPIEs(Body.ObjectDataLink.ArrayPosition, Propulsion.ObjectDataLink.ArrayPosition).RightPIE = Fields(3).ToLower
                End If
            End If
        Next

        'set propulsion-body PIEs

        For A As Integer = 0 To Propulsions.Count - 1
            Propulsion = Propulsions(A)
            For B As Integer = 0 To Bodies.Count - 1
                Body = Bodies(B)
                Propulsion.Bodies(B).LeftAttachment = New clsUnitType.clsAttachment
                Propulsion.Bodies(B).LeftAttachment.Models.Add(GetModelForPIE(PIE_List, BodyPropulsionPIEs(B, A).LeftPIE, ReturnResult))
                Propulsion.Bodies(B).RightAttachment = New clsUnitType.clsAttachment
                Propulsion.Bodies(B).RightAttachment.Models.Add(GetModelForPIE(PIE_List, BodyPropulsionPIEs(B, A).RightPIE, ReturnResult))
            Next
        Next

        'interpret construction

        For Each Fields In DataConstruction.ResultData
            Construct = New clsConstruct
            Construct.ObjectDataLink.Connect(Constructors)
            Construct.TurretObjectDataLink.Connect(Turrets)
            Construct.Code = Fields(0)
            SetComponentName(DataNames.ResultData, Construct, ReturnResult)
            Construct.Designable = (Fields(11) <> "0")
            Construct.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(8).ToLower, ReturnResult))
        Next

        'interpret weapons

        For Each Fields In DataWeapons.ResultData
            Weapon = New clsWeapon
            Weapon.ObjectDataLink.Connect(Weapons)
            Weapon.TurretObjectDataLink.Connect(Turrets)
            Weapon.Code = Fields(0)
            SetComponentName(DataNames.ResultData, Weapon, ReturnResult)
            InvariantParse_int(Fields(7), Weapon.HitPoints)
            Weapon.Designable = (Fields(51) <> "0")
            Weapon.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(8).ToLower, ReturnResult))
            Weapon.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(9).ToLower, ReturnResult))
        Next

        'interpret sensor

        For Each Fields In DataSensor.ResultData
            Sensor = New clsSensor
            Sensor.ObjectDataLink.Connect(Sensors)
            Sensor.TurretObjectDataLink.Connect(Turrets)
            Sensor.Code = Fields(0)
            SetComponentName(DataNames.ResultData, Sensor, ReturnResult)
            InvariantParse_int(Fields(7), Sensor.HitPoints)
            Sensor.Designable = (Fields(15) <> "0")
            Select Case Fields(11).ToLower
                Case "turret"
                    Sensor.Location = clsSensor.enumLocation.Turret
                Case "default"
                    Sensor.Location = clsSensor.enumLocation.Invisible
                Case Else
                    Sensor.Location = clsSensor.enumLocation.Invisible
            End Select
            Sensor.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(8).ToLower, ReturnResult))
            Sensor.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(9).ToLower, ReturnResult))
        Next

        'interpret repair

        For Each Fields In DataRepair.ResultData
            Repair = New clsRepair
            Repair.ObjectDataLink.Connect(Repairs)
            Repair.TurretObjectDataLink.Connect(Turrets)
            Repair.Code = Fields(0)
            SetComponentName(DataNames.ResultData, Repair, ReturnResult)
            Repair.Designable = (Fields(13) <> "0")
            Repair.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(9).ToLower, ReturnResult))
            Repair.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(10).ToLower, ReturnResult))
        Next

        'interpret brain

        For Each Fields In DataBrain.ResultData
            Brain = New clsBrain
            Brain.ObjectDataLink.Connect(Brains)
            Brain.TurretObjectDataLink.Connect(Turrets)
            Brain.Code = Fields(0)
            SetComponentName(DataNames.ResultData, Brain, ReturnResult)
            Brain.Designable = True
            Weapon = FindWeaponCode(Fields(7))
            If Weapon IsNot Nothing Then
                Brain.Weapon = Weapon
                Brain.Attachment = Weapon.Attachment
            End If
        Next

        'interpret ecm

        For Each Fields In DataECM.ResultData
            ECM = New clsECM
            ECM.ObjectDataLink.Connect(ECMs)
            ECM.TurretObjectDataLink.Connect(Turrets)
            ECM.Code = Fields(0)
            SetComponentName(DataNames.ResultData, ECM, ReturnResult)
            InvariantParse_int(Fields(7), ECM.HitPoints)
            ECM.Designable = False
            ECM.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(8).ToLower, ReturnResult))
        Next

        'interpret feature

        For Each Fields In DataFeatures.ResultData
            FeatureType = New clsFeatureType
            FeatureType.UnitType_ObjectDataLink.Connect(UnitTypes)
            FeatureType.FeatureType_ObjectDataLink.Connect(FeatureTypes)
            FeatureType.Code = Fields(0)
            If Fields(7) = "OIL RESOURCE" Then 'type
                FeatureType.FeatureType = clsFeatureType.enumFeatureType.OilResource
            End If
            SetFeatureName(DataNames.ResultData, FeatureType, ReturnResult)
            If Not InvariantParse_int(Fields(1), FeatureType.Footprint.X) Then
                ReturnResult.WarningAdd("Feature footprint-x was not an integer for " & FeatureType.Code & ".")
            End If
            If Not InvariantParse_int(Fields(2), FeatureType.Footprint.Y) Then
                ReturnResult.WarningAdd("Feature footprint-y was not an integer for " & FeatureType.Code & ".")
            End If
            FeatureType.BaseAttachment = New clsUnitType.clsAttachment
            BaseAttachment = FeatureType.BaseAttachment
            Text = Fields(6).ToLower
            Attachment = BaseAttachment.CreateAttachment()
            Attachment.Models.Add(GetModelForPIE(PIE_List, Text, ReturnResult))
        Next

        'interpret structure

        For Each Fields In DataStructures.ResultData
            Dim StructureCode As String = Fields(0)
            Dim StructureTypeText As String = Fields(1)
            Dim StructurePIEs() As String = Fields(21).ToLower.Split("@"c)
            Dim StructureFootprint As sXY_int
            Dim StructureBasePIE As String = Fields(22).ToLower
            If Not InvariantParse_int(Fields(5), StructureFootprint.X) Then
                ReturnResult.WarningAdd("Structure footprint-x was not an integer for " & StructureCode & ".")
            End If
            If Not InvariantParse_int(Fields(6), StructureFootprint.Y) Then
                ReturnResult.WarningAdd("Structure footprint-y was not an integer for " & StructureCode & ".")
            End If
            If StructureTypeText <> "WALL" Or StructurePIEs.GetLength(0) <> 4 Then
                'this is NOT a generic wall
                StructureType = New clsStructureType
                StructureType.UnitType_ObjectDataLink.Connect(UnitTypes)
                StructureType.StructureType_ObjectDataLink.Connect(StructureTypes)
                StructureType.Code = StructureCode
                SetStructureName(DataNames.ResultData, StructureType, ReturnResult)
                StructureType.Footprint = StructureFootprint
                Select Case StructureTypeText
                    Case "DEMOLISH"
                        StructureType.StructureType = clsStructureType.enumStructureType.Demolish
                    Case "WALL"
                        StructureType.StructureType = clsStructureType.enumStructureType.Wall
                    Case "CORNER WALL"
                        StructureType.StructureType = clsStructureType.enumStructureType.CornerWall
                    Case "FACTORY"
                        StructureType.StructureType = clsStructureType.enumStructureType.Factory
                    Case "CYBORG FACTORY"
                        StructureType.StructureType = clsStructureType.enumStructureType.CyborgFactory
                    Case "VTOL FACTORY"
                        StructureType.StructureType = clsStructureType.enumStructureType.VTOLFactory
                    Case "COMMAND"
                        StructureType.StructureType = clsStructureType.enumStructureType.Command
                    Case "HQ"
                        StructureType.StructureType = clsStructureType.enumStructureType.HQ
                    Case "DEFENSE"
                        StructureType.StructureType = clsStructureType.enumStructureType.Defense
                    Case "POWER GENERATOR"
                        StructureType.StructureType = clsStructureType.enumStructureType.PowerGenerator
                    Case "POWER MODULE"
                        StructureType.StructureType = clsStructureType.enumStructureType.PowerModule
                    Case "RESEARCH"
                        StructureType.StructureType = clsStructureType.enumStructureType.Research
                    Case "RESEARCH MODULE"
                        StructureType.StructureType = clsStructureType.enumStructureType.ResearchModule
                    Case "FACTORY MODULE"
                        StructureType.StructureType = clsStructureType.enumStructureType.FactoryModule
                    Case "DOOR"
                        StructureType.StructureType = clsStructureType.enumStructureType.DOOR
                    Case "REPAIR FACILITY"
                        StructureType.StructureType = clsStructureType.enumStructureType.RepairFacility
                    Case "SAT UPLINK"
                        StructureType.StructureType = clsStructureType.enumStructureType.DOOR
                    Case "REARM PAD"
                        StructureType.StructureType = clsStructureType.enumStructureType.RearmPad
                    Case "MISSILE SILO"
                        StructureType.StructureType = clsStructureType.enumStructureType.MissileSilo
                    Case "RESOURCE EXTRACTOR"
                        StructureType.StructureType = clsStructureType.enumStructureType.ResourceExtractor
                    Case Else
                        StructureType.StructureType = clsStructureType.enumStructureType.Unknown
                End Select

                BaseAttachment = StructureType.BaseAttachment
                If StructurePIEs.GetLength(0) > 0 Then
                    BaseAttachment.Models.Add(GetModelForPIE(PIE_List, StructurePIEs(0), ReturnResult))
                End If
                StructureType.StructureBasePlate = GetModelForPIE(PIE_List, StructureBasePIE, ReturnResult)
                If BaseAttachment.Models.Count = 1 Then
                    If BaseAttachment.Models.Item(0).ConnectorCount >= 1 Then
                        Connector = BaseAttachment.Models.Item(0).Connectors(0)
                        Dim StructureWeapons As SimpleList(Of String())
                        StructureWeapons = GetRowsWithValue(DataStructureWeapons.ResultData, StructureType.Code)
                        If StructureWeapons.Count > 0 Then
                            Weapon = FindWeaponCode(StructureWeapons(0)(1))
                        Else
                            Weapon = Nothing
                        End If
                        ECM = FindECMCode(Fields(18))
                        Sensor = FindSensorCode(Fields(19))
                        If Weapon IsNot Nothing Then
                            If Weapon.Code <> "ZNULLWEAPON" Then
                                Attachment = BaseAttachment.CopyAttachment(Weapon.Attachment)
                                Attachment.Pos_Offset = Connector
                            End If
                        End If
                        If ECM IsNot Nothing Then
                            If ECM.Code <> "ZNULLECM" Then
                                Attachment = BaseAttachment.CopyAttachment(ECM.Attachment)
                                Attachment.Pos_Offset = Connector
                            End If
                        End If
                        If Sensor IsNot Nothing Then
                            If Sensor.Code <> "ZNULLSENSOR" Then
                                Attachment = BaseAttachment.CopyAttachment(Sensor.Attachment)
                                Attachment.Pos_Offset = Connector
                            End If
                        End If
                    End If
                End If
            Else
                'this is a generic wall
                Dim NewWall As New clsWallType
                NewWall.WallType_ObjectDataLink.Connect(WallTypes)
                NewWall.Code = StructureCode
                SetWallName(DataNames.ResultData, NewWall, ReturnResult)
                Dim WallBasePlate As clsModel = GetModelForPIE(PIE_List, StructureBasePIE, ReturnResult)

                Dim WallNum As Integer
                Dim WallStructureType As clsStructureType
                For WallNum = 0 To 3
                    WallStructureType = New clsStructureType
                    WallStructureType.UnitType_ObjectDataLink.Connect(UnitTypes)
                    WallStructureType.StructureType_ObjectDataLink.Connect(StructureTypes)
                    WallStructureType.WallLink.Connect(NewWall.Segments)
                    WallStructureType.Code = StructureCode
                    Text = NewWall.Name
                    Select Case WallNum
                        Case 0
                            Text &= " - "
                        Case 1
                            Text &= " + "
                        Case 2
                            Text &= " T "
                        Case 3
                            Text &= " L "
                    End Select
                    WallStructureType.Name = Text
                    WallStructureType.Footprint = StructureFootprint
                    WallStructureType.StructureType = clsStructureType.enumStructureType.Wall

                    BaseAttachment = WallStructureType.BaseAttachment

                    Text = StructurePIEs(WallNum)
                    BaseAttachment.Models.Add(GetModelForPIE(PIE_List, Text, ReturnResult))
                    WallStructureType.StructureBasePlate = WallBasePlate
                Next
            End If
        Next

        'interpret templates

        Dim TurretConflictCount As Integer = 0
        For Each Fields In DataTemplates.ResultData
            Template = New clsDroidTemplate
            Template.UnitType_ObjectDataLink.Connect(UnitTypes)
            Template.DroidTemplate_ObjectDataLink.Connect(DroidTemplates)
            Template.Code = Fields(0)
            SetTemplateName(DataNames.ResultData, Template, ReturnResult)
            Select Case Fields(9) 'type
                Case "ZNULLDROID"
                    Template.TemplateDroidType = TemplateDroidType_Null
                Case "DROID"
                    Template.TemplateDroidType = TemplateDroidType_Droid
                Case "CYBORG"
                    Template.TemplateDroidType = TemplateDroidType_Cyborg
                Case "CYBORG_CONSTRUCT"
                    Template.TemplateDroidType = TemplateDroidType_CyborgConstruct
                Case "CYBORG_REPAIR"
                    Template.TemplateDroidType = TemplateDroidType_CyborgRepair
                Case "CYBORG_SUPER"
                    Template.TemplateDroidType = TemplateDroidType_CyborgSuper
                Case "TRANSPORTER"
                    Template.TemplateDroidType = TemplateDroidType_Transporter
                Case "PERSON"
                    Template.TemplateDroidType = TemplateDroidType_Person
                Case Else
                    Template.TemplateDroidType = Nothing
                    ReturnResult.WarningAdd("Template " & Template.GetDisplayTextCode & " had an unrecognised type.")
            End Select
            Dim LoadPartsArgs As New clsDroidDesign.sLoadPartsArgs
            LoadPartsArgs.Body = FindBodyCode(Fields(2))
            LoadPartsArgs.Brain = FindBrainCode(Fields(3))
            LoadPartsArgs.Construct = FindConstructorCode(Fields(4))
            LoadPartsArgs.ECM = FindECMCode(Fields(5))
            LoadPartsArgs.Propulsion = FindPropulsionCode(Fields(7))
            LoadPartsArgs.Repair = FindRepairCode(Fields(8))
            LoadPartsArgs.Sensor = FindSensorCode(Fields(10))
            Dim TemplateWeapons As SimpleList(Of String()) = GetRowsWithValue(DataAssignWeapons.ResultData, Template.Code)
            If TemplateWeapons.Count > 0 Then
                Text = TemplateWeapons(0)(1)
                If Text <> "NULL" Then
                    LoadPartsArgs.Weapon1 = FindWeaponCode(Text)
                End If
                Text = TemplateWeapons(0)(2)
                If Text <> "NULL" Then
                    LoadPartsArgs.Weapon2 = FindWeaponCode(Text)
                End If
                Text = TemplateWeapons(0)(3)
                If Text <> "NULL" Then
                    LoadPartsArgs.Weapon3 = FindWeaponCode(Text)
                End If
            End If
            If Not Template.LoadParts(LoadPartsArgs) Then
                If TurretConflictCount < 16 Then
                    ReturnResult.WarningAdd("Template " & Template.GetDisplayTextCode & " had multiple conflicting turrets.")
                End If
                TurretConflictCount += 1
            End If
        Next
        If TurretConflictCount > 0 Then
            ReturnResult.WarningAdd(TurretConflictCount & " templates had multiple conflicting turrets.")
        End If

        Return ReturnResult
    End Function

    Public Function GetRowsWithValue(TextLines As SimpleList(Of String()), Value As String) As SimpleList(Of String())
        Dim Result As New SimpleList(Of String())

        Dim Line() As String
        For Each Line In TextLines
            If Line(0) = Value Then
                Result.Add(Line)
            End If
        Next

        Return Result
    End Function

    Public Structure sBytes
        Public Bytes() As Byte
    End Structure
    Public Structure sLines
        Public Lines() As String

        Public Sub RemoveComments()
            Dim LineNum As Integer
            Dim LineCount As Integer = Lines.GetUpperBound(0) + 1
            Dim InCommentBlock As Boolean
            Dim CommentStart As Integer
            Dim CharNum As Integer
            Dim CommentLength As Integer

            For LineNum = 0 To LineCount - 1
                CharNum = 0
                If InCommentBlock Then
                    CommentStart = 0
                End If
                Do
                    If CharNum >= Lines(LineNum).Length Then
                        If InCommentBlock Then
                            Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart)
                        End If
                        Exit Do
                    ElseIf InCommentBlock Then
                        If Lines(LineNum).Chars(CharNum) = "*"c Then
                            CharNum += 1
                            If CharNum >= Lines(LineNum).Length Then

                            ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                                CharNum += 1
                                CommentLength = CharNum - CommentStart
                                InCommentBlock = False
                                Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart) & Strings.Right(Lines(LineNum), Lines(LineNum).Length - (CommentStart + CommentLength))
                                CharNum -= CommentLength
                            End If
                        Else
                            CharNum += 1
                        End If
                    ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                        CharNum += 1
                        If CharNum >= Lines(LineNum).Length Then

                        ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                            CommentStart = CharNum - 1
                            CharNum = Lines(LineNum).Length
                            CommentLength = CharNum - CommentStart
                            Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart) & Strings.Right(Lines(LineNum), Lines(LineNum).Length - (CommentStart + CommentLength))
                            Exit Do
                        ElseIf Lines(LineNum).Chars(CharNum) = "*"c Then
                            CommentStart = CharNum - 1
                            CharNum += 1
                            InCommentBlock = True
                        End If
                    Else
                        CharNum += 1
                    End If
                Loop
            Next
        End Sub
    End Structure

    Public Function GetModelForPIE(PIE_List As SimpleList(Of clsPIE), PIE_LCaseFileTitle As String, ResultOutput As clsResult) As clsModel

        If PIE_LCaseFileTitle = "0" Then
            Return Nothing
        End If

        Dim A As Integer
        Dim PIEFile As IO.StreamReader
        Dim PIE As clsPIE

        Dim Result As New clsResult("Loading PIE file " & PIE_LCaseFileTitle)

        For A = 0 To PIE_List.Count - 1
            PIE = PIE_List(A)
            If PIE.LCaseFileTitle = PIE_LCaseFileTitle Then
                If PIE.Model Is Nothing Then
                    PIE.Model = New clsModel
                    Try
                        PIEFile = New IO.StreamReader(PIE.Path)
                        Try
                            Result.Take(PIE.Model.ReadPIE(PIEFile, Me))
                        Catch ex As Exception
                            PIEFile.Close()
                            Result.WarningAdd(ex.Message)
                            ResultOutput.Add(Result)
                            Return PIE.Model
                        End Try
                    Catch ex As Exception
                        Result.WarningAdd(ex.Message)
                    End Try
                End If
                ResultOutput.Add(Result)
                Return PIE.Model
            End If
        Next

        If Not Result.HasWarnings Then
            Result.WarningAdd("file is missing")
        End If
        ResultOutput.Add(Result)

        Return Nothing
    End Function

    Public Sub SetComponentName(Names As SimpleList(Of String()), Component As clsComponent, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, Component.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for component " & Component.Code & ".")
        Else
            Component.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Sub SetFeatureName(Names As SimpleList(Of String()), FeatureType As clsFeatureType, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, FeatureType.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for feature type " & FeatureType.Code & ".")
        Else
            FeatureType.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Sub SetStructureName(Names As SimpleList(Of String()), StructureType As clsStructureType, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, StructureType.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for structure type " & StructureType.Code & ".")
        Else
            StructureType.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Sub SetTemplateName(Names As SimpleList(Of String()), Template As clsDroidTemplate, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, Template.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for droid template " & Template.Code & ".")
        Else
            Template.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Sub SetWallName(Names As SimpleList(Of String()), WallType As clsWallType, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, WallType.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for structure type " & WallType.Code & ".")
        Else
            WallType.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Function FindBodyCode(Code As String) As clsBody
        Dim Component As clsBody

        For Each Component In Bodies
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindPropulsionCode(Code As String) As clsPropulsion
        Dim Component As clsPropulsion

        For Each Component In Propulsions
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindConstructorCode(Code As String) As clsConstruct
        Dim Component As clsConstruct

        For Each Component In Constructors
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindSensorCode(Code As String) As clsSensor
        Dim Component As clsSensor

        For Each Component In Sensors
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindRepairCode(Code As String) As clsRepair
        Dim Component As clsRepair

        For Each Component In Repairs
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindECMCode(Code As String) As clsECM
        Dim Component As clsECM

        For Each Component In ECMs
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindBrainCode(Code As String) As clsBrain
        Dim Component As clsBrain

        For Each Component In Brains
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindWeaponCode(Code As String) As clsWeapon
        Dim Component As clsWeapon

        For Each Component In Weapons
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function Get_TexturePage_GLTexture(FileTitle As String) As Integer
        Dim LCaseTitle As String = FileTitle.ToLower
        Dim TexPage As clsTexturePage

        For Each TexPage In TexturePages
            If TexPage.FileTitle.ToLower = LCaseTitle Then
                Return TexPage.GLTexture_Num
            End If
        Next
        Return 0
    End Function

    Public Function FindOrCreateWeapon(Code As String) As clsWeapon
        Dim Result As clsWeapon

        Result = FindWeaponCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsWeapon
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateConstruct(Code As String) As clsConstruct
        Dim Result As clsConstruct

        Result = FindConstructorCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsConstruct
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateRepair(Code As String) As clsRepair
        Dim Result As clsRepair

        Result = FindRepairCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsRepair
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateSensor(Code As String) As clsSensor
        Dim Result As clsSensor

        Result = FindSensorCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsSensor
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateBrain(Code As String) As clsBrain
        Dim Result As clsBrain

        Result = FindBrainCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsBrain
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateECM(Code As String) As clsECM
        Dim Result As clsECM

        Result = FindECMCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsECM
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateTurret(TurretType As clsTurret.enumTurretType, TurretCode As String) As clsTurret

        Select Case TurretType
            Case clsTurret.enumTurretType.Weapon
                Return FindOrCreateWeapon(TurretCode)
            Case clsTurret.enumTurretType.Construct
                Return FindOrCreateConstruct(TurretCode)
            Case clsTurret.enumTurretType.Repair
                Return FindOrCreateRepair(TurretCode)
            Case clsTurret.enumTurretType.Sensor
                Return FindOrCreateSensor(TurretCode)
            Case clsTurret.enumTurretType.Brain
                Return FindOrCreateBrain(TurretCode)
            Case clsTurret.enumTurretType.ECM
                Return FindOrCreateECM(TurretCode)
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function FindOrCreateBody(Code As String) As clsBody
        Dim Result As clsBody

        Result = FindBodyCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsBody
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreatePropulsion(Code As String) As clsPropulsion
        Dim Result As clsPropulsion

        Result = FindPropulsionCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsPropulsion(Bodies.Count)
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateUnitType(Code As String, Type As clsUnitType.enumType, WallType As Integer) As clsUnitType

        Select Case Type
            Case clsUnitType.enumType.Feature
                Dim FeatureType As clsFeatureType
                For Each FeatureType In FeatureTypes
                    If FeatureType.Code = Code Then
                        Return FeatureType
                    End If
                Next
                FeatureType = New clsFeatureType
                FeatureType.IsUnknown = True
                FeatureType.Code = Code
                FeatureType.Footprint.X = 1
                FeatureType.Footprint.Y = 1
                Return FeatureType
            Case clsUnitType.enumType.PlayerStructure
                Dim StructureType As clsStructureType
                For Each StructureType In StructureTypes
                    If StructureType.Code = Code Then
                        If WallType < 0 Then
                            Return StructureType
                        ElseIf StructureType.WallLink.IsConnected Then
                            If StructureType.WallLink.ArrayPosition = WallType Then
                                Return StructureType
                            End If
                        End If
                    End If
                Next
                StructureType = New clsStructureType
                StructureType.IsUnknown = True
                StructureType.Code = Code
                StructureType.Footprint.X = 1
                StructureType.Footprint.Y = 1
                Return StructureType
            Case clsUnitType.enumType.PlayerDroid
                Dim DroidType As clsDroidTemplate
                For Each DroidType In DroidTemplates
                    If DroidType.IsTemplate Then
                        If DroidType.Code = Code Then
                            Return DroidType
                        End If
                    End If
                Next
                DroidType = New clsDroidTemplate
                DroidType.IsUnknown = True
                DroidType.Code = Code
                Return DroidType
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function FindFirstStructureType(Type As clsStructureType.enumStructureType) As clsStructureType
        Dim StructureType As clsStructureType

        For Each StructureType In StructureTypes
            If StructureType.StructureType = Type Then
                Return StructureType
            End If
        Next

        Return Nothing
    End Function
End Class
