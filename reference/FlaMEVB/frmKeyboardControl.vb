Public Class frmKeyboardControl

    Public Results As New SimpleList(Of clsContainer(Of Keys))

    Public Sub New()
        InitializeComponent()

        Icon = ProgramIcon

        UpdateLabel()
    End Sub

    Private Sub UpdateLabel()

        lblKeys.Text = ""
        For i As Integer = 0 To Results.Count - 1
            Dim key As Keys = Results(i).Item
            Dim text As String = [Enum].GetName(GetType(Keys), key)
            If text Is Nothing Then
                lblKeys.Text &= InvariantToString_int(Results(i).Item)
            Else
                lblKeys.Text &= text
            End If
            lblKeys.Text &= " "
        Next
    End Sub

    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click

        DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click

        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub frmKeyboardControl_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        If Results.Count > 8 Then
            Exit Sub
        End If
        For Each key As clsContainer(Of Keys) In Results
            If key.Item = e.KeyCode Then
                Exit Sub
            End If
        Next
        Results.Add(New clsContainer(Of Keys)(e.KeyCode))
        UpdateLabel()
    End Sub
End Class