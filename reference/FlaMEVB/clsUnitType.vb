Imports OpenTK.Graphics.OpenGL

Public MustInherit Class clsUnitType

    Public ReadOnly UnitType_ObjectDataLink As New ConnectedListLink(Of clsUnitType, clsObjectData)(Me)

    Public ReadOnly UnitType_frmMainSelectedLink As New ConnectedListLink(Of clsUnitType, frmMain)(Me)

    Public IsUnknown As Boolean = False

    Enum enumType As Byte
        Unspecified
        Feature
        PlayerStructure
        PlayerDroid
    End Enum
    Public Type As enumType

    Public Class clsAttachment
        Public Pos_Offset As sXYZ_sng
        Public AngleOffsetMatrix As New Matrix3D.Matrix3D
        Public Models As New SimpleClassList(Of clsModel)
        Public Attachments As New SimpleClassList(Of clsAttachment)

        Public Sub New()

            Models.AddNothingAction = SimpleClassList_AddNothingAction.DisallowIgnore
            Matrix3D.MatrixSetToIdentity(AngleOffsetMatrix)
        End Sub

        Public Sub GLDraw()
            Dim AngleRPY As Matrix3D.AngleRPY
            Dim matrixA As New Matrix3D.Matrix3D
            Dim Attachment As clsAttachment
            Dim Model As clsModel

            For Each Model In Models
                Model.GLDraw()
            Next

            For Each Attachment In Attachments
                GL.PushMatrix()
                Matrix3D.MatrixInvert(Attachment.AngleOffsetMatrix, matrixA)
                Matrix3D.MatrixToRPY(matrixA, AngleRPY)
                GL.Translate(Attachment.Pos_Offset.X, Attachment.Pos_Offset.Y, -Attachment.Pos_Offset.Z)
                GL.Rotate(AngleRPY.Roll / RadOf1Deg, 0.0F, 0.0F, -1.0F)
                GL.Rotate(AngleRPY.Pitch / RadOf1Deg, 1.0F, 0.0F, 0.0F)
                GL.Rotate(AngleRPY.Yaw / RadOf1Deg, 0.0F, 1.0F, 0.0F)
                Attachment.GLDraw()
                GL.PopMatrix()
            Next
        End Sub

        Public Function CreateAttachment() As clsAttachment
            Dim Result As New clsAttachment

            Attachments.Add(Result)
            Return Result
        End Function

        Public Function CopyAttachment(Other As clsAttachment) As clsAttachment
            Dim Result As New clsAttachment

            Result.Pos_Offset = Other.Pos_Offset
            Attachments.Add(Result)
            Matrix3D.MatrixCopy(Other.AngleOffsetMatrix, Result.AngleOffsetMatrix)
            Result.Models.AddSimpleList(Other.Models)
            Result.Attachments.AddSimpleList(Other.Attachments)

            Return Result
        End Function

        Public Function AddCopyOfAttachment(AttachmentToCopy As clsAttachment) As clsAttachment
            Dim ResultAttachment As New clsAttachment
            Dim Attachment As clsAttachment

            Attachments.Add(ResultAttachment)
            Matrix3D.MatrixCopy(AttachmentToCopy.AngleOffsetMatrix, ResultAttachment.AngleOffsetMatrix)
            ResultAttachment.Models.AddSimpleList(AttachmentToCopy.Models)
            For Each Attachment In AttachmentToCopy.Attachments
                ResultAttachment.AddCopyOfAttachment(Attachment)
            Next

            Return ResultAttachment
        End Function
    End Class

    Public Sub GLDraw(RotationDegrees As Single)

        Select Case Draw_Lighting
            Case enumDrawLighting.Off
                GL.Color3(1.0F, 1.0F, 1.0F)
            Case enumDrawLighting.Half
                GL.Color3(0.875F, 0.875F, 0.875F)
            Case enumDrawLighting.Normal
                GL.Color3(0.75F, 0.75F, 0.75F)
        End Select
        'GL.Rotate(x, 1.0F, 0.0F, 0.0F)
        GL.Rotate(RotationDegrees, 0.0F, 1.0F, 0.0F)
        'GL.Rotate(z, 0.0F, 0.0F, -1.0F)

        TypeGLDraw()
    End Sub

    Protected Overridable Sub TypeGLDraw()

    End Sub

    Public ReadOnly Property GetFootprintOld As sXY_int
        Get
            Select Case Type
                Case enumType.Feature
                    Return CType(Me, clsFeatureType).Footprint
                Case enumType.PlayerStructure
                    Return CType(Me, clsStructureType).Footprint
                Case Else
                    Dim XY_int As New sXY_int(1, 1)
                    Return XY_int
            End Select
        End Get
    End Property

    Public ReadOnly Property GetFootprintNew(Rotation As Integer) As sXY_int
        Get
            'get initial footprint
            Dim Result As sXY_int
            Select Case Type
                Case enumType.Feature
                    Result = CType(Me, clsFeatureType).Footprint
                Case enumType.PlayerStructure
                    Result = CType(Me, clsStructureType).Footprint
                Case Else
                    'return droid footprint
                    Result = New sXY_int(1, 1)
                    Return Result
            End Select
            'switch footprint axes if not a droid
            Dim Remainder As Double = (Rotation / 90.0# + 0.5#) Mod 2.0#
            If Remainder < 0.0# Then
                Remainder += 2.0#
            End If
            If Remainder >= 1.0# Then
                Dim X As Integer = Result.X
                Result.X = Result.Y
                Result.Y = X
            End If
            Return Result
        End Get
    End Property

    Public ReadOnly Property GetFootprintSelected(Rotation As Integer) As sXY_int
        Get
            If frmMainInstance.cbxFootprintRotate.Checked Then
                Return GetFootprintNew(Rotation)
            Else
                Return GetFootprintOld
            End If
        End Get
    End Property

    Public Function GetCode(ByRef Result As String) As Boolean

        Select Case Type
            Case enumType.Feature
                Result = CType(Me, clsFeatureType).Code
                Return True
            Case enumType.PlayerStructure
                Result = CType(Me, clsStructureType).Code
                Return True
            Case enumType.PlayerDroid
                Dim Droid As clsDroidDesign = CType(Me, clsDroidDesign)
                If Droid.IsTemplate Then
                    Result = CType(Me, clsDroidTemplate).Code
                    Return True
                Else
                    Result = Nothing
                    Return False
                End If
            Case Else
                Result = Nothing
                Return False
        End Select
    End Function

    Public Function GetDisplayTextCode() As String

        Select Case Type
            Case enumType.Feature
                Dim FeatureType As clsFeatureType = CType(Me, clsFeatureType)
                Return FeatureType.Code & " (" & FeatureType.Name & ")"
            Case enumType.PlayerStructure
                Dim StructureType As clsStructureType = CType(Me, clsStructureType)
                Return StructureType.Code & " (" & StructureType.Name & ")"
            Case enumType.PlayerDroid
                Dim DroidType As clsDroidDesign = CType(Me, clsDroidDesign)
                If DroidType.IsTemplate Then
                    Dim Template As clsDroidTemplate = CType(Me, clsDroidTemplate)
                    Return Template.Code & " (" & Template.Name & ")"
                Else
                    Return "<droid> (" & DroidType.GenerateName & ")"
                End If
            Case Else
                Return ""
        End Select
    End Function

    Public Function GetDisplayTextName() As String

        Select Case Type
            Case enumType.Feature
                Dim FeatureType As clsFeatureType = CType(Me, clsFeatureType)
                Return FeatureType.Name & " (" & FeatureType.Code & ")"
            Case enumType.PlayerStructure
                Dim StructureType As clsStructureType = CType(Me, clsStructureType)
                Return StructureType.Name & " (" & StructureType.Code & ")"
            Case enumType.PlayerDroid
                Dim DroidType As clsDroidDesign = CType(Me, clsDroidDesign)
                If DroidType.IsTemplate Then
                    Dim Template As clsDroidTemplate = CType(Me, clsDroidTemplate)
                    Return Template.Name & " (" & Template.Code & ")"
                Else
                    Return DroidType.GenerateName & " (<droid>)"
                End If
            Case Else
                Return ""
        End Select
    End Function

    Public Overridable Function GetName() As String

        Return ""
    End Function
End Class

Public Class clsFeatureType
    Inherits clsUnitType

    Public FeatureType_ObjectDataLink As New ConnectedListLink(Of clsFeatureType, clsObjectData)(Me)

    Public Code As String = ""
    Public Name As String = "Unknown"
    Public Footprint As sXY_int
    Public Enum enumFeatureType As Byte
        Unknown
        OilResource
    End Enum
    Public FeatureType As enumFeatureType = enumFeatureType.Unknown

    Public BaseAttachment As clsUnitType.clsAttachment

    Public Sub New()

        Type = enumType.Feature
    End Sub

    Protected Overrides Sub TypeGLDraw()

        If BaseAttachment IsNot Nothing Then
            BaseAttachment.GLDraw()
        End If
    End Sub

    Public Overrides Function GetName() As String
        
        Return Name
    End Function
End Class

Public Class clsStructureType
    Inherits clsUnitType

    Public StructureType_ObjectDataLink As New ConnectedListLink(Of clsStructureType, clsObjectData)(Me)

    Public Code As String = ""
    Public Name As String = "Unknown"
    Public Footprint As sXY_int
    Public Enum enumStructureType As Byte
        Unknown
        Demolish
        Wall
        CornerWall
        Factory
        CyborgFactory
        VTOLFactory
        Command
        HQ
        Defense
        PowerGenerator
        PowerModule
        Research
        ResearchModule
        FactoryModule
        DOOR
        RepairFacility
        SatUplink
        RearmPad
        MissileSilo
        ResourceExtractor
    End Enum
    Public StructureType As enumStructureType = enumStructureType.Unknown

    Public WallLink As New ConnectedListLink(Of clsStructureType, clsWallType)(Me)

    Public BaseAttachment As New clsUnitType.clsAttachment
    Public StructureBasePlate As clsModel

    Public Sub New()

        Type = enumType.PlayerStructure
    End Sub

    Protected Overrides Sub TypeGLDraw()

        If BaseAttachment IsNot Nothing Then
            BaseAttachment.GLDraw()
        End If
        If StructureBasePlate IsNot Nothing Then
            StructureBasePlate.GLDraw()
        End If
    End Sub

    Public Function IsModule() As Boolean

        Return (StructureType = clsStructureType.enumStructureType.FactoryModule _
            Or StructureType = clsStructureType.enumStructureType.PowerModule _
            Or StructureType = clsStructureType.enumStructureType.ResearchModule)
    End Function

    Public Overrides Function GetName() As String

        Return Name
    End Function
End Class

Public Class clsDroidDesign
    Inherits clsUnitType

    Public IsTemplate As Boolean

    Public Name As String = ""

    Public Class clsTemplateDroidType

        Public Num As Integer = -1

        Public Name As String

        Public TemplateCode As String

        Public Sub New(NewName As String, NewTemplateCode As String)

            Name = NewName
            TemplateCode = NewTemplateCode
        End Sub
    End Class
    Public TemplateDroidType As clsTemplateDroidType

    Public Body As clsBody
    Public Propulsion As clsPropulsion
    Public TurretCount As Byte
    Public Turret1 As clsTurret
    Public Turret2 As clsTurret
    Public Turret3 As clsTurret

    Public BaseAttachment As New clsUnitType.clsAttachment

    Public AlwaysDrawTextLabel As Boolean

    Public Sub New()

        Type = enumType.PlayerDroid
    End Sub

    Public Sub CopyDesign(DroidTypeToCopy As clsDroidDesign)

        TemplateDroidType = DroidTypeToCopy.TemplateDroidType
        Body = DroidTypeToCopy.Body
        Propulsion = DroidTypeToCopy.Propulsion
        TurretCount = DroidTypeToCopy.TurretCount
        Turret1 = DroidTypeToCopy.Turret1
        Turret2 = DroidTypeToCopy.Turret2
        Turret3 = DroidTypeToCopy.Turret3
    End Sub

    Protected Overrides Sub TypeGLDraw()

        If BaseAttachment IsNot Nothing Then
            BaseAttachment.GLDraw()
        End If
    End Sub

    Public Sub UpdateAttachments()

        BaseAttachment = New clsUnitType.clsAttachment

        If Body Is Nothing Then
            AlwaysDrawTextLabel = True
            Exit Sub
        End If

        Dim NewBody As clsUnitType.clsAttachment = BaseAttachment.AddCopyOfAttachment(Body.Attachment)

        AlwaysDrawTextLabel = (NewBody.Models.Count = 0)

        If Propulsion IsNot Nothing Then
            If Body.ObjectDataLink.IsConnected Then
                BaseAttachment.AddCopyOfAttachment(Propulsion.Bodies(Body.ObjectDataLink.ArrayPosition).LeftAttachment)
                BaseAttachment.AddCopyOfAttachment(Propulsion.Bodies(Body.ObjectDataLink.ArrayPosition).RightAttachment)
            End If
        End If

        If NewBody.Models.Count = 0 Then
            Exit Sub
        End If

        If NewBody.Models.Item(0).ConnectorCount <= 0 Then
            Exit Sub
        End If

        Dim TurretConnector As sXYZ_sng

        TurretConnector = Body.Attachment.Models.Item(0).Connectors(0)

        If TurretCount >= 1 Then
            If Turret1 IsNot Nothing Then
                Dim NewTurret As clsUnitType.clsAttachment = NewBody.AddCopyOfAttachment(Turret1.Attachment)
                NewTurret.Pos_Offset = TurretConnector
            End If
        End If

        If Body.Attachment.Models.Item(0).ConnectorCount <= 1 Then
            Exit Sub
        End If

        TurretConnector = Body.Attachment.Models.Item(0).Connectors(1)

        If TurretCount >= 2 Then
            If Turret2 IsNot Nothing Then
                Dim NewTurret As clsUnitType.clsAttachment = NewBody.AddCopyOfAttachment(Turret2.Attachment)
                NewTurret.Pos_Offset = TurretConnector
            End If
        End If
    End Sub

    Public Function GetMaxHitPoints() As Integer
        Dim Result As Integer

        'this is inaccurate

        If Body Is Nothing Then
            Return 0
        End If
        Result = Body.Hitpoints
        If Propulsion Is Nothing Then
            Return Result
        End If
        Result += CInt(Body.Hitpoints * Propulsion.HitPoints / 100.0#)
        If Turret1 Is Nothing Then
            Return Result
        End If
        Result += Body.Hitpoints + Turret1.HitPoints
        If TurretCount < 2 Or Turret2 Is Nothing Then
            Return Result
        End If
        If Turret2.TurretType <> clsTurret.enumTurretType.Weapon Then
            Return Result
        End If
        Result += Body.Hitpoints + Turret2.HitPoints
        If TurretCount < 3 Or Turret3 Is Nothing Then
            Return Result
        End If
        If Turret3.TurretType <> clsTurret.enumTurretType.Weapon Then
            Return Result
        End If
        Result += Body.Hitpoints + Turret3.HitPoints
        Return Result
    End Function

    Public Structure sLoadPartsArgs
        Public Body As clsBody
        Public Propulsion As clsPropulsion
        Public Construct As clsConstruct
        Public Sensor As clsSensor
        Public Repair As clsRepair
        Public Brain As clsBrain
        Public ECM As clsECM
        Public Weapon1 As clsWeapon
        Public Weapon2 As clsWeapon
        Public Weapon3 As clsWeapon
    End Structure

    Public Function LoadParts(Args As sLoadPartsArgs) As Boolean
        Dim TurretConflict As Boolean

        Body = Args.Body
        Propulsion = Args.Propulsion

        TurretConflict = False
        If Args.Construct IsNot Nothing Then
            If Args.Construct.Code <> "ZNULLCONSTRUCT" Then
                If Turret1 IsNot Nothing Then
                    TurretConflict = True
                End If
                TurretCount = 1
                Turret1 = Args.Construct
            End If
        End If
        If Args.Repair IsNot Nothing Then
            If Args.Repair.Code <> "ZNULLREPAIR" Then
                If Turret1 IsNot Nothing Then
                    TurretConflict = True
                End If
                TurretCount = 1
                Turret1 = Args.Repair
            End If
        End If
        If Args.Brain IsNot Nothing Then
            If Args.Brain.Code <> "ZNULLBRAIN" Then
                If Turret1 IsNot Nothing Then
                    TurretConflict = True
                End If
                TurretCount = 1
                Turret1 = Args.Brain
            End If
        End If
        If Args.Weapon1 IsNot Nothing Then
            Dim UseWeapon As Boolean
            If Turret1 IsNot Nothing Then
                If Turret1.TurretType = clsTurret.enumTurretType.Brain Then
                    UseWeapon = False
                Else
                    UseWeapon = True
                    TurretConflict = True
                End If
            Else
                UseWeapon = True
            End If
            If UseWeapon Then
                TurretCount = 1
                Turret1 = Args.Weapon1
                If Args.Weapon2 IsNot Nothing Then
                    Turret2 = Args.Weapon2
                    TurretCount += CByte(1)
                    If Args.Weapon3 IsNot Nothing Then
                        Turret3 = Args.Weapon3
                        TurretCount += CByte(1)
                    End If
                End If
            End If
        End If
        If Args.Sensor IsNot Nothing Then
            If Args.Sensor.Location = clsSensor.enumLocation.Turret Then
                If Turret1 IsNot Nothing Then
                    TurretConflict = True
                End If
                TurretCount = 1
                Turret1 = Args.Sensor
            End If
        End If
        UpdateAttachments()

        Return (Not TurretConflict) 'return if all is ok
    End Function

    Public Function GenerateName() As String
        Dim Result As String = ""

        If Propulsion IsNot Nothing Then
            If Result.Length > 0 Then
                Result = " "c & Result
            End If
            Result = Propulsion.Name & Result
        End If

        If Body IsNot Nothing Then
            If Result.Length > 0 Then
                Result = " "c & Result
            End If
            Result = Body.Name & Result
        End If

        If TurretCount >= 3 Then
            If Turret3 IsNot Nothing Then
                If Result.Length > 0 Then
                    Result = " "c & Result
                End If
                Result = Turret3.Name & Result
            End If
        End If

        If TurretCount >= 2 Then
            If Turret2 IsNot Nothing Then
                If Result.Length > 0 Then
                    Result = " "c & Result
                End If
                Result = Turret2.Name & Result
            End If
        End If

        If TurretCount >= 1 Then
            If Turret1 IsNot Nothing Then
                If Result.Length > 0 Then
                    Result = " "c & Result
                End If
                Result = Turret1.Name & Result
            End If
        End If

        Return Result
    End Function

    Public Function GetDroidType() As enumDroidType
        Dim Result As enumDroidType

        If TemplateDroidType Is TemplateDroidType_Null Then
            Result = enumDroidType.Default_
        ElseIf TemplateDroidType Is TemplateDroidType_Person Then
            Result = enumDroidType.Person
        ElseIf TemplateDroidType Is TemplateDroidType_Cyborg Then
            Result = enumDroidType.Cyborg
        ElseIf TemplateDroidType Is TemplateDroidType_CyborgSuper Then
            Result = enumDroidType.Cyborg_Super
        ElseIf TemplateDroidType Is TemplateDroidType_CyborgConstruct Then
            Result = enumDroidType.Cyborg_Construct
        ElseIf TemplateDroidType Is TemplateDroidType_CyborgRepair Then
            Result = enumDroidType.Cyborg_Repair
        ElseIf TemplateDroidType Is TemplateDroidType_Transporter Then
            Result = enumDroidType.Transporter
        ElseIf Turret1 Is Nothing Then
            Result = enumDroidType.Default_
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Brain Then
            Result = enumDroidType.Command
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Sensor Then
            Result = enumDroidType.Sensor
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.ECM Then
            Result = enumDroidType.ECM
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Construct Then
            Result = enumDroidType.Construct
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Repair Then
            Result = enumDroidType.Repair
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Weapon Then
            Result = enumDroidType.Weapon
        Else
            Result = enumDroidType.Default_
        End If
        Return Result
    End Function

    Public Function SetDroidType(DroidType As enumDroidType) As Boolean

        Select Case DroidType
            Case enumDroidType.Weapon
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Sensor
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.ECM
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Construct
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Person
                TemplateDroidType = TemplateDroidType_Person
            Case enumDroidType.Cyborg
                TemplateDroidType = TemplateDroidType_Cyborg
            Case enumDroidType.Transporter
                TemplateDroidType = TemplateDroidType_Transporter
            Case enumDroidType.Command
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Repair
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Default_
                TemplateDroidType = TemplateDroidType_Null
            Case enumDroidType.Cyborg_Construct
                TemplateDroidType = TemplateDroidType_CyborgConstruct
            Case enumDroidType.Cyborg_Repair
                TemplateDroidType = TemplateDroidType_CyborgRepair
            Case enumDroidType.Cyborg_Super
                TemplateDroidType = TemplateDroidType_CyborgSuper
            Case Else
                TemplateDroidType = Nothing
                Return False
        End Select
        Return True
    End Function

    Public Function GetConstructCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.Construct Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLCONSTRUCT"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Function GetRepairCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.Repair Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLREPAIR"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Function GetSensorCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.Sensor Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLSENSOR"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Function GetBrainCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.Brain Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLBRAIN"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Function GetECMCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.ECM Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLECM"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Overrides Function GetName() As String

        Return Name
    End Function
End Class

Public Class clsDroidTemplate
    Inherits clsDroidDesign

    Public DroidTemplate_ObjectDataLink As New ConnectedListLink(Of clsDroidTemplate, clsObjectData)(Me)

    Public Code As String = ""

    Public Sub New()

        IsTemplate = True
        Name = "Unknown"
    End Sub
End Class

Public Class clsWallType

    Public WallType_ObjectDataLink As New ConnectedListLink(Of clsWallType, clsObjectData)(Me)

    Public Code As String = ""
    Public Name As String = "Unknown"

    Public TileWalls_Segment() As Integer = {0, 0, 0, 0, 0, 3, 3, 2, 0, 3, 3, 2, 0, 2, 2, 1}
    Private Const d0 As Integer = 0
    Private Const d1 As Integer = 90
    Private Const d2 As Integer = 180
    Private Const d3 As Integer = 270
    Public TileWalls_Direction() As Integer = {d0, d0, d2, d0, d3, d0, d3, d0, d1, d1, d2, d2, d3, d1, d3, d0}

    Public Segments As New ConnectedList(Of clsStructureType, clsWallType)(Me)

    Public Sub New()

        Segments.MaintainOrder = True
    End Sub
End Class
