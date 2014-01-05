
Public Module modColour

    Public Function OSRGB(Red As Integer, Green As Integer, Blue As Integer) As Integer

#If Not Mono Then
        Return RGB(Red, Green, Blue)
#Else
        Return RGB(Blue, Green, Red)
#End If
    End Function
End Module

Public Structure sRGB_sng
    Public Red As Single
    Public Green As Single
    Public Blue As Single

    Public Sub New(Red As Single, Green As Single, Blue As Single)

        Me.Red = Red
        Me.Green = Green
        Me.Blue = Blue
    End Sub
End Structure

Public Structure sRGBA_sng
    Public Red As Single
    Public Green As Single
    Public Blue As Single
    Public Alpha As Single

    Public Sub New(Red As Single, Green As Single, Blue As Single, Alpha As Single)

        Me.Red = Red
        Me.Green = Green
        Me.Blue = Blue
        Me.Alpha = Alpha
    End Sub
End Structure

Public Class clsRGB_sng
    Public Red As Single
    Public Green As Single
    Public Blue As Single

    Public Sub New(Red As Single, Green As Single, Blue As Single)

        Me.Red = Red
        Me.Green = Green
        Me.Blue = Blue
    End Sub

    Public Overridable Function GetINIOutput() As String

        Return InvariantToString_sng(Red) & ", " & InvariantToString_sng(Green) & ", " & InvariantToString_sng(Blue)
    End Function

    Public Overridable Function ReadINIText(SplitText As clsSplitCommaText) As Boolean

        If SplitText.PartCount < 3 Then
            Return False
        End If

        Dim Colour As sRGB_sng

        If Not InvariantParse_sng(SplitText.Parts(0), Colour.Red) Then
            Return False
        End If
        If Not InvariantParse_sng(SplitText.Parts(1), Colour.Green) Then
            Return False
        End If
        If Not InvariantParse_sng(SplitText.Parts(2), Colour.Blue) Then
            Return False
        End If

        Red = Colour.Red
        Green = Colour.Green
        Blue = Colour.Blue

        Return True
    End Function
End Class

Public Class clsRGBA_sng
    Inherits clsRGB_sng

    Public Alpha As Single

    Public Sub New(Red As Single, Green As Single, Blue As Single, Alpha As Single)
        MyBase.New(Red, Green, Blue)

        Me.Alpha = Alpha
    End Sub

    Public Sub New(CopyItem As clsRGBA_sng)
        MyBase.New(CopyItem.Red, CopyItem.Green, CopyItem.Blue)

        Alpha = CopyItem.Alpha
    End Sub

    Public Overrides Function GetINIOutput() As String

        Return MyBase.GetINIOutput() & ", " & InvariantToString_sng(Alpha)
    End Function

    Public Overrides Function ReadINIText(SplitText As clsSplitCommaText) As Boolean

        If Not MyBase.ReadINIText(SplitText) Then
            Return False
        End If

        If SplitText.PartCount < 4 Then
            Return False
        End If

        If Not InvariantParse_sng(SplitText.Parts(3), Alpha) Then
            Alpha = 1.0F
        End If

        Return True
    End Function
End Class
