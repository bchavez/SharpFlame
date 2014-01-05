
Public Class ctrlColour

    Private Colour As clsRGB_sng
    Private ColourColor As Color

    Private ColourBoxGraphics As Graphics

    Public Sub New(NewColour As clsRGB_sng)
        InitializeComponent()

        If NewColour Is Nothing Then
            Stop
            Hide()
            Exit Sub
        End If

        Colour = NewColour
        Dim Red As Integer = CInt(Clamp_dbl(Colour.Red * 255.0#, 0.0#, 255.0#))
        Dim Green As Integer = CInt(Clamp_dbl(Colour.Green * 255.0#, 0.0#, 255.0#))
        Dim Blue As Integer = CInt(Clamp_dbl(Colour.Blue * 255.0#, 0.0#, 255.0#))
        ColourColor = ColorTranslator.FromOle(OSRGB(Red, Green, Blue))

        If TypeOf Colour Is clsRGBA_sng Then
            nudAlpha.Value = CDec(CType(Colour, clsRGBA_sng).Alpha)
            AddHandler nudAlpha.ValueChanged, AddressOf nudAlpha_Changed
            AddHandler nudAlpha.Leave, AddressOf nudAlpha_Changed
        Else
            nudAlpha.Hide()
        End If

        ColourBoxGraphics = pnlColour.CreateGraphics

        ColourBoxRedraw()
    End Sub

    Private Sub SelectColour(sender As System.Object, e As System.EventArgs) Handles pnlColour.Click
        Dim ColourSelect As New Windows.Forms.ColorDialog

        ColourSelect.Color = ColourColor
        Dim Result As DialogResult = ColourSelect.ShowDialog()
        If Result <> DialogResult.OK Then
            Exit Sub
        End If
        ColourColor = ColourSelect.Color
        Colour.Red = CSng(ColourColor.R / 255.0#)
        Colour.Green = CSng(ColourColor.G / 255.0#)
        Colour.Blue = CSng(ColourColor.B / 255.0#)
        ColourBoxRedraw()
    End Sub

    Private Sub nudAlpha_Changed(sender As Object, e As EventArgs)

        CType(Colour, clsRGBA_sng).Alpha = nudAlpha.Value
    End Sub

    Private Sub pnlColour_Paint(sender As Object, e As PaintEventArgs) Handles pnlColour.Paint

        ColourBoxRedraw()
    End Sub

    Private Sub ColourBoxRedraw()

        ColourBoxGraphics.Clear(ColourColor)
    End Sub
End Class
