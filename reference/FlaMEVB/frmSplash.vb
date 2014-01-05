
Public Class frmSplash

    Public Sub New()
        InitializeComponent()

        Text = ProgramName & " " & ProgramVersionNumber & " Loading"
        lblVersion.Text = ProgramVersionNumber
    End Sub
End Class
