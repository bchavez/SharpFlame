
Public Class clsOptionGroup

    Public Options As New ConnectedList(Of clsOptionInterface, clsOptionGroup)(Me)
End Class

Public MustInherit Class clsOptionInterface

    MustOverride ReadOnly Property GroupLink As ConnectedListLink(Of clsOptionInterface, clsOptionGroup)

    MustOverride ReadOnly Property SaveKey As String
    MustOverride ReadOnly Property DefaultValueObject As Object
    MustOverride Function IsValueValid(value As Object) As Boolean
End Class

Public Class clsOption(Of ValueType)
    Inherits clsOptionInterface

    Private _GroupLink As New ConnectedListLink(Of clsOptionInterface, clsOptionGroup)(Me)
    Public Overrides ReadOnly Property GroupLink As ConnectedListLink(Of clsOptionInterface, clsOptionGroup)
        Get
            Return _GroupLink
        End Get
    End Property

    Private _SaveKey As String
    Private _DefaultValue As ValueType
    Public ReadOnly Property DefaultValue As ValueType
        Get
            Return _DefaultValue
        End Get
    End Property

    Public Sub New(saveKey As String, defaultValue As ValueType)

        Me._SaveKey = saveKey
        Me._DefaultValue = defaultValue
    End Sub

    Public Overrides ReadOnly Property DefaultValueObject As Object
        Get
            Return _DefaultValue
        End Get
    End Property

    Public Overrides ReadOnly Property SaveKey As String
        Get
            Return _SaveKey
        End Get
    End Property

    Public Overrides Function IsValueValid(value As Object) As Boolean

        Return True
    End Function
End Class

Public Class clsOptionCreator(Of ValueType)

    Public SaveKey As String
    Public DefaultValue As ValueType

    Public Overridable Function Create() As clsOption(Of ValueType)
        Return New clsOption(Of ValueType)(SaveKey, DefaultValue)
    End Function
End Class

Public Class clsOptionProfile
    Inherits clsINIRead.clsTranslator

    Public MustInherit Class clsChangeInterface
        MustOverride ReadOnly Property ValueObject As Object
        MustOverride Function GetCopy() As clsChangeInterface
    End Class

    Public Class clsChange(Of ValueType)
        Inherits clsChangeInterface

        Public Value As ValueType
        Public Overrides ReadOnly Property ValueObject As Object
            Get
                Return Value
            End Get
        End Property

        Public Sub New(value As ValueType)

            Me.Value = value
        End Sub

        Public Overrides Function GetCopy() As clsChangeInterface

            Return New clsChange(Of ValueType)(Value)
        End Function
    End Class

    Public ReadOnly Property IsAnythingChanged As Boolean
        Get
            For Each item As clsOptionInterface In _Options.Options
                If Changes(item) IsNot Nothing Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    Private _Options As clsOptionGroup

    Public ReadOnly Property Options As clsOptionGroup
        Get
            Return _Options
        End Get
    End Property

    Private _Changes() As clsChangeInterface

    Public Property Changes(optionItem As clsOptionInterface) As clsChangeInterface
        Get
            Return _Changes(optionItem.GroupLink.ArrayPosition)
        End Get
        Set(value As clsChangeInterface)
            _Changes(optionItem.GroupLink.ArrayPosition) = value
        End Set
    End Property

    Public ReadOnly Property Value(optionItem As clsOptionInterface) As Object
        Get
            Dim index As Integer = optionItem.GroupLink.ArrayPosition
            Dim change As clsChangeInterface = _Changes(index)
            If change Is Nothing Then
                Return optionItem.DefaultValueObject
            Else
                Return change.ValueObject
            End If
        End Get
    End Property

    Public Sub New(options As clsOptionGroup)

        _Options = options
        ReDim _Changes(options.Options.Count - 1)
    End Sub

    Public Overridable Function GetCopy(creator As clsOptionProfileCreator) As clsOptionProfile

        creator.Options = _Options
        Dim result As clsOptionProfile = creator.Create

        For i As Integer = 0 To _Options.Options.Count - 1
            If _Changes(i) IsNot Nothing Then
                result._Changes(i) = _Changes(i).GetCopy
            End If
        Next

        Return result
    End Function

    Public Function INIWrite(file As clsINIWrite) As clsResult
        Dim returnResult As New clsResult("Writing options to INI")

        For Each item As clsOptionInterface In _Options.Options
            If Changes(item) Is Nothing Then
                Continue For
            End If
            Dim optionValue As Object = Value(item)
            Dim valueText As String = Nothing
            If TypeOf item Is clsOption(Of clsKeyboardControl) Then
                Dim control As clsKeyboardControl = CType(optionValue, clsKeyboardControl)
                valueText = ""
                For i As Integer = 0 To control.Keys.GetUpperBound(0)
                    Dim key As Keys = control.Keys(i)
                    valueText &= InvariantToString_int(key)
                    If i < control.Keys.GetUpperBound(0) Then
                        valueText &= ","c
                    End If
                Next
                If control.UnlessKeys.GetUpperBound(0) >= 0 Then
                    valueText &= "unless "
                    For i As Integer = 0 To control.UnlessKeys.GetUpperBound(0)
                        Dim key As Keys = control.UnlessKeys(i)
                        valueText &= InvariantToString_int(key)
                        If i < control.UnlessKeys.GetUpperBound(0) Then
                            valueText &= ","c
                        End If
                    Next
                End If
            ElseIf TypeOf item Is clsOption(Of SimpleList(Of String)) Then
                Dim list As SimpleList(Of String) = CType(optionValue, SimpleList(Of String))
                For i As Integer = 0 To list.Count - 1
                    file.Property_Append(item.SaveKey, list(i))
                Next
            ElseIf TypeOf item Is clsOption(Of clsRGB_sng) Then
                valueText = CType(optionValue, clsRGB_sng).GetINIOutput
            ElseIf TypeOf item Is clsOption(Of clsRGBA_sng) Then
                valueText = CType(optionValue, clsRGBA_sng).GetINIOutput
            ElseIf TypeOf item Is clsOption(Of FontFamily) Then
                valueText = CType(optionValue, FontFamily).Name
            ElseIf TypeOf item Is clsOption(Of Boolean) Then
                valueText = InvariantToString_bool(CType(optionValue, Boolean))
            ElseIf TypeOf item Is clsOption(Of Byte) Then
                valueText = InvariantToString_byte(CType(optionValue, Byte))
            ElseIf TypeOf item Is clsOption(Of Short) Then
                valueText = InvariantToString_int(CType(optionValue, Short))
            ElseIf TypeOf item Is clsOption(Of Integer) Then
                valueText = InvariantToString_int(CType(optionValue, Integer))
            ElseIf TypeOf item Is clsOption(Of UInteger) Then
                valueText = InvariantToString_uint(CType(optionValue, UInteger))
            ElseIf TypeOf item Is clsOption(Of Single) Then
                valueText = InvariantToString_sng(CType(optionValue, Single))
            ElseIf TypeOf item Is clsOption(Of Double) Then
                valueText = InvariantToString_dbl(CType(optionValue, Double))
            ElseIf TypeOf item Is clsOption(Of String) Then
                valueText = CType(optionValue, String)
            Else
                returnResult.ProblemAdd("Value for option " & ControlChars.Quote & item.SaveKey & ControlChars.Quote & " could not be written because it is of type " & optionValue.GetType.FullName)
            End If
            If valueText IsNot Nothing Then
                file.Property_Append(item.SaveKey, valueText)
            End If
        Next

        Return returnResult
    End Function

    Public Overrides Function Translate(INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

        For Each item As clsOptionInterface In _Options.Options
            If item.SaveKey.ToLower <> INIProperty.Name Then
                Continue For
            End If
            If TypeOf item Is clsOption(Of clsKeyboardControl) Then
                Dim unlessIndex As Integer = INIProperty.Value.ToLower.IndexOf("unless")
                Dim keysText() As String
                Dim unlessKeysText() As String
                If unlessIndex < 0 Then
                    keysText = INIProperty.Value.Split(","c)
                    ReDim unlessKeysText(-1)
                Else
                    keysText = INIProperty.Value.Substring(0, unlessIndex - 1).Split(","c)
                    unlessKeysText = INIProperty.Value.Substring(unlessIndex + 6, INIProperty.Value.Length - (unlessIndex + 6)).Split(","c)
                End If
                Dim keys(keysText.GetUpperBound(0)) As Keys
                Dim valid As Boolean = True
                For j As Integer = 0 To keysText.GetUpperBound(0)
                    Dim number As Integer
                    If InvariantParse_int(keysText(j), number) Then
                        keys(j) = CType(number, Keys)
                    Else
                        valid = False
                    End If
                Next
                Dim unlessKeys(unlessKeysText.GetUpperBound(0)) As Keys
                For j As Integer = 0 To unlessKeysText.GetUpperBound(0)
                    Dim number As Integer
                    If InvariantParse_int(unlessKeysText(j), number) Then
                        unlessKeys(j) = CType(number, Keys)
                    Else
                        valid = False
                    End If
                Next
                If Not valid Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Dim control As New clsKeyboardControl(keys, unlessKeys)
                If Not item.IsValueValid(control) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of clsKeyboardControl)(control)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of SimpleList(Of String)) Then
                Dim list As SimpleList(Of String)
                If Changes(item) Is Nothing Then
                    list = New SimpleList(Of String)
                    Changes(item) = New clsChange(Of SimpleList(Of String))(list)
                Else
                    list = CType(Changes(item).ValueObject, SimpleList(Of String))
                End If
                list.Add(INIProperty.Value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of FontFamily) Then
                Dim fontFamily As FontFamily
                Try
                    fontFamily = New FontFamily(INIProperty.Value)
                Catch
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End Try
                If Not item.IsValueValid(fontFamily) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of FontFamily)(fontFamily)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of clsRGB_sng) Then
                Dim value As New clsRGB_sng(0.0F, 0.0F, 0.0F)
                If Not value.ReadINIText(New clsSplitCommaText(INIProperty.Value)) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of clsRGB_sng)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of clsRGBA_sng) Then
                Dim value As New clsRGBA_sng(0.0F, 0.0F, 0.0F, 0.0F)
                If Not value.ReadINIText(New clsSplitCommaText(INIProperty.Value)) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of clsRGBA_sng)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of Boolean) Then
                Dim value As Boolean
                If Not InvariantParse_bool(INIProperty.Value, value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of Boolean)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of Byte) Then
                Dim value As Byte
                If Not InvariantParse_byte(INIProperty.Value, value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of Byte)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of Short) Then
                Dim value As Short
                If Not InvariantParse_short(INIProperty.Value, value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of Short)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of Integer) Then
                Dim value As Integer
                If Not InvariantParse_int(INIProperty.Value, value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of Integer)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of UInteger) Then
                Dim value As UInteger
                If Not InvariantParse_uint(INIProperty.Value, value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of UInteger)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of Single) Then
                Dim value As Single
                If Not InvariantParse_sng(INIProperty.Value, value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of Single)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of Double) Then
                Dim value As Double
                If Not InvariantParse_dbl(INIProperty.Value, value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of Double)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            ElseIf TypeOf item Is clsOption(Of String) Then
                Dim value As String = INIProperty.Value
                If Not item.IsValueValid(value) Then
                    Return clsINIRead.enumTranslatorResult.ValueInvalid
                End If
                Changes(item) = New clsChange(Of String)(value)
                Return clsINIRead.enumTranslatorResult.Translated
            Else
                Return clsINIRead.enumTranslatorResult.ValueInvalid
            End If
        Next

        Return clsINIRead.enumTranslatorResult.ValueInvalid
    End Function
End Class

Public Class clsOptionProfileCreator

    Public Options As clsOptionGroup

    Public Sub New()

    End Sub

    Public Sub New(options As clsOptionGroup)

        Me.Options = options
    End Sub

    Public Overridable Function Create() As clsOptionProfile

        Return New clsOptionProfile(Options)
    End Function
End Class
