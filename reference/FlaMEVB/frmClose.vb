
Public Class frmClose

    Public Sub New(WindowTitle As String)
        InitializeComponent()

        Text = WindowTitle

        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub
End Class
