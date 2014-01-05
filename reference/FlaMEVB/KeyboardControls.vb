
Public Module modControls

    Public Class clsKeyboardProfile
        Inherits clsOptionProfile

        Public Function Active(control As clsOption(Of clsKeyboardControl)) As Boolean

            Return CType(Value(control), clsKeyboardControl).Active
        End Function

        Public Sub New(options As clsOptionGroup)
            MyBase.New(options)
        End Sub
    End Class
    Public Class clsKeyboardProfileCreator
        Inherits clsOptionProfileCreator

        Public Overrides Function Create() As clsOptionProfile
            Return New clsKeyboardProfile(Options)
        End Function
    End Class

    Public Options_KeyboardControls As New clsOptionGroup
    Public KeyboardProfile As clsKeyboardProfile

    'interface controls
    Public Control_Deselect As clsOption(Of clsKeyboardControl)
    Public Control_PreviousTool As clsOption(Of clsKeyboardControl)
    'selected unit controls
    Public Control_Unit_Move As clsOption(Of clsKeyboardControl)
    Public Control_Unit_Delete As clsOption(Of clsKeyboardControl)
    Public Control_Unit_Multiselect As clsOption(Of clsKeyboardControl)
    'generalised controls
    Public Control_Slow As clsOption(Of clsKeyboardControl)
    Public Control_Fast As clsOption(Of clsKeyboardControl)
    'picker controls
    Public Control_Picker As clsOption(Of clsKeyboardControl)
    'view controls
    Public Control_View_Textures As clsOption(Of clsKeyboardControl)
    Public Control_View_Lighting As clsOption(Of clsKeyboardControl)
    Public Control_View_Wireframe As clsOption(Of clsKeyboardControl)
    Public Control_View_Units As clsOption(Of clsKeyboardControl)
    Public Control_View_ScriptMarkers As clsOption(Of clsKeyboardControl)
    Public Control_View_Move_Type As clsOption(Of clsKeyboardControl)
    Public Control_View_Rotate_Type As clsOption(Of clsKeyboardControl)
    Public Control_View_Move_Left As clsOption(Of clsKeyboardControl)
    Public Control_View_Move_Right As clsOption(Of clsKeyboardControl)
    Public Control_View_Move_Forward As clsOption(Of clsKeyboardControl)
    Public Control_View_Move_Backward As clsOption(Of clsKeyboardControl)
    Public Control_View_Move_Up As clsOption(Of clsKeyboardControl)
    Public Control_View_Move_Down As clsOption(Of clsKeyboardControl)
    Public Control_View_Zoom_In As clsOption(Of clsKeyboardControl)
    Public Control_View_Zoom_Out As clsOption(Of clsKeyboardControl)
    Public Control_View_Left As clsOption(Of clsKeyboardControl)
    Public Control_View_Right As clsOption(Of clsKeyboardControl)
    Public Control_View_Forward As clsOption(Of clsKeyboardControl)
    Public Control_View_Backward As clsOption(Of clsKeyboardControl)
    Public Control_View_Up As clsOption(Of clsKeyboardControl)
    Public Control_View_Down As clsOption(Of clsKeyboardControl)
    Public Control_View_Reset As clsOption(Of clsKeyboardControl)
    Public Control_View_Roll_Left As clsOption(Of clsKeyboardControl)
    Public Control_View_Roll_Right As clsOption(Of clsKeyboardControl)
    'texture controls
    Public Control_Clockwise As clsOption(Of clsKeyboardControl)
    Public Control_CounterClockwise As clsOption(Of clsKeyboardControl)
    Public Control_Texture_Flip As clsOption(Of clsKeyboardControl)
    Public Control_Tri_Flip As clsOption(Of clsKeyboardControl)
    'gateway controls
    Public Control_Gateway_Delete As clsOption(Of clsKeyboardControl)
    'undo controls
    Public Control_Undo As clsOption(Of clsKeyboardControl)
    Public Control_Redo As clsOption(Of clsKeyboardControl)
    'script marker controls
    Public Control_ScriptPosition As clsOption(Of clsKeyboardControl)

    Public Function KeyboardControlOptionCreate(saveKey As String, keys() As Keys) As clsOption(Of clsKeyboardControl)

        Dim result As New clsOption(Of clsKeyboardControl)(saveKey, New clsKeyboardControl(keys, New Keys() {}))
        Options_KeyboardControls.Options.Add(result.GroupLink)
        Return result
    End Function

    Public Function KeyboardControlOptionCreate(saveKey As String, keys() As Keys, unlessKeys() As Keys) As clsOption(Of clsKeyboardControl)

        Dim result As New clsOption(Of clsKeyboardControl)(saveKey, New clsKeyboardControl(keys, unlessKeys))
        Options_KeyboardControls.Options.Add(result.GroupLink)
        Return result
    End Function

    Public Function KeyboardControlOptionCreate(saveKey As String, defaultValue As clsKeyboardControl) As clsOption(Of clsKeyboardControl)

        Dim result As New clsOption(Of clsKeyboardControl)(saveKey, defaultValue)
        Options_KeyboardControls.Options.Add(result.GroupLink)
        Return result
    End Function

    Public Sub CreateControls()

        'interface controls

        Control_Deselect = KeyboardControlOptionCreate("ObjectSelectTool", New Keys() {Keys.Escape})
        Control_PreviousTool = KeyboardControlOptionCreate("PreviousTool", New Keys() {Keys.Oemtilde})

        'selected unit controls

        Control_Unit_Move = KeyboardControlOptionCreate("MoveObjects", New Keys() {Keys.M})
        Control_Unit_Delete = KeyboardControlOptionCreate("DeleteObjects", New Keys() {Keys.Delete})
        Control_Unit_Multiselect = KeyboardControlOptionCreate("Multiselect", New Keys() {Keys.ShiftKey})

        'generalised controls

        Control_Slow = KeyboardControlOptionCreate("ViewSlow", New Keys() {Keys.R})
        Control_Fast = KeyboardControlOptionCreate("ViewFast", New Keys() {Keys.F})

        'picker controls

        Control_Picker = KeyboardControlOptionCreate("Picker", New Keys() {Keys.ControlKey})

        'view controls

        Control_View_Textures = KeyboardControlOptionCreate("ShowTextures", New Keys() {Keys.F5})
        Control_View_Lighting = KeyboardControlOptionCreate("ShowLighting", New Keys() {Keys.F8})
        Control_View_Wireframe = KeyboardControlOptionCreate("ShowWireframe", New Keys() {Keys.F6})
        Control_View_Units = KeyboardControlOptionCreate("ShowObjects", New Keys() {Keys.F7})
        Control_View_ScriptMarkers = KeyboardControlOptionCreate("ShowLabels", New Keys() {Keys.F4})
        Control_View_Move_Type = KeyboardControlOptionCreate("ViewMoveMode", New Keys() {Keys.F1})
        Control_View_Rotate_Type = KeyboardControlOptionCreate("ViewRotateMode", New Keys() {Keys.F2})
        Control_View_Move_Left = KeyboardControlOptionCreate("ViewMoveLeft", New Keys() {Keys.A})
        Control_View_Move_Right = KeyboardControlOptionCreate("ViewMoveRight", New Keys() {Keys.D})
        Control_View_Move_Forward = KeyboardControlOptionCreate("ViewMoveForwards", New Keys() {Keys.W})
        Control_View_Move_Backward = KeyboardControlOptionCreate("ViewMoveBackwards", New Keys() {Keys.S})
        Control_View_Move_Up = KeyboardControlOptionCreate("ViewMoveUp", New Keys() {Keys.E})
        Control_View_Move_Down = KeyboardControlOptionCreate("ViewMoveDown", New Keys() {Keys.C})
        Control_View_Zoom_In = KeyboardControlOptionCreate("ViewZoomIn", New Keys() {Keys.Home})
        Control_View_Zoom_Out = KeyboardControlOptionCreate("ViewZoomOut", New Keys() {Keys.End})
        Control_View_Left = KeyboardControlOptionCreate("ViewRotateLeft", New Keys() {Keys.Left})
        Control_View_Right = KeyboardControlOptionCreate("ViewRotateRight", New Keys() {Keys.Right})
        Control_View_Forward = KeyboardControlOptionCreate("ViewRotateForwards", New Keys() {Keys.Up})
        Control_View_Backward = KeyboardControlOptionCreate("ViewRotateBackwards", New Keys() {Keys.Down})
        Control_View_Up = KeyboardControlOptionCreate("ViewRotateUp", New Keys() {Keys.PageUp})
        Control_View_Down = KeyboardControlOptionCreate("ViewRotateDown", New Keys() {Keys.PageDown})
        Control_View_Roll_Left = KeyboardControlOptionCreate("ViewRollLeft", New Keys() {Keys.OemOpenBrackets})
        Control_View_Roll_Right = KeyboardControlOptionCreate("ViewRollRight", New Keys() {Keys.OemCloseBrackets})
        Control_View_Reset = KeyboardControlOptionCreate("ViewReset", New Keys() {Keys.Back})
        
        'texture controls

        Control_CounterClockwise = KeyboardControlOptionCreate("CounterClockwise", New Keys() {Keys.Oemcomma})
        Control_Clockwise = KeyboardControlOptionCreate("Clockwise", New Keys() {Keys.OemPeriod})
        Control_Texture_Flip = KeyboardControlOptionCreate("TextureFlip", New Keys() {Keys.OemQuestion})
        Control_Tri_Flip = KeyboardControlOptionCreate("TriangleFlip", New Keys() {Keys.OemPipe}) '\|
        Control_Gateway_Delete = KeyboardControlOptionCreate("GatewayDelete", New Keys() {Keys.ShiftKey})

        'undo controls

        Control_Undo = KeyboardControlOptionCreate("Undo", New Keys() {Keys.ControlKey, Keys.Z})
        Control_Redo = KeyboardControlOptionCreate("Redo", New Keys() {Keys.ControlKey, Keys.Y})
        Control_ScriptPosition = KeyboardControlOptionCreate("PositionLabel", New Keys() {Keys.P})

        KeyboardProfile = New clsKeyboardProfile(Options_KeyboardControls)
    End Sub
End Module

Public Class clsKeyboardControl

    Public Keys() As Keys
    Public UnlessKeys() As Keys

    Private Function IsPressed(KeysDown As clsKeysActive) As Boolean

        For Each Key As Keys In Keys
            If Not KeysDown.Keys(Key) Then
                Return False
            End If
        Next
        For Each Key As Keys In UnlessKeys
            If KeysDown.Keys(Key) Then
                Return False
            End If
        Next

        Return True
    End Function

    Public Sub New(Keys() As Keys)

        Me.Keys = Keys
        ReDim UnlessKeys(-1)
    End Sub

    Public Sub New(Keys() As Keys, UnlessKeys() As Keys)

        Me.Keys = Keys
        Me.UnlessKeys = UnlessKeys
    End Sub

    Private _Active As Boolean
    Public ReadOnly Property Active As Boolean
        Get
            Return _Active
        End Get
    End Property

    Public Sub KeysChanged(KeysDown As clsKeysActive)

        _Active = IsPressed(KeysDown)
    End Sub
End Class
