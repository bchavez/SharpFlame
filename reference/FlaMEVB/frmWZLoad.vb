
Public Class frmWZLoad

    Public Class clsOutput
        Public Result As Integer
    End Class

    Public lstMap_MapName() As String

    Public Output As clsOutput

    Public Structure sMapNameList
        Public Names() As String
    End Structure

    Public Sub New(MapNames() As String, NewOutput As clsOutput, FormTitle As String)
        InitializeComponent()

        Icon = ProgramIcon

        Output = NewOutput
        Output.Result = -1

        Dim A As Integer

        lstMap.Items.Clear()
        lstMap_MapName = MapNames
        For A = 0 To MapNames.GetUpperBound(0)
            lstMap.Items.Add(MapNames(A))
        Next

        Text = FormTitle
    End Sub

    Private Sub lstMaps_DoubleClick(sender As Object, e As System.EventArgs) Handles lstMap.DoubleClick

        If lstMap.SelectedIndex >= 0 Then
            Output.Result = lstMap.SelectedIndex
            Close()
        End If
    End Sub
End Class
