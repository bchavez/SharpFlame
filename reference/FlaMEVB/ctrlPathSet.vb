
Public Class ctrlPathSet

    Public Property SelectedNum As Integer
        Get
            Return lstPaths.SelectedIndex
        End Get
        Set(value As Integer)
            lstPaths.SelectedIndex = value
        End Set
    End Property

    Public ReadOnly Property SelectedPath As String
        Get
            If lstPaths.SelectedIndex < 0 Then
                Return Nothing
            Else
                Return CStr(lstPaths.Items.Item(lstPaths.SelectedIndex))
            End If
        End Get
    End Property

    Public ReadOnly Property GetPaths As String()
        Get
            Dim Paths(lstPaths.Items.Count - 1) As String
            Dim A As Integer
            For A = 0 To lstPaths.Items.Count - 1
                Paths(A) = CStr(lstPaths.Items.Item(A))
            Next
            Return Paths
        End Get
    End Property

    Public Sub New(Title As String)
        InitializeComponent()

        gbxTitle.Text = Title
    End Sub

    Public Sub SetPaths(NewPaths As SimpleList(Of String))
        Dim A As Integer

        lstPaths.Items.Clear()
        For A = 0 To NewPaths.Count - 1
            lstPaths.Items.Add(NewPaths(A))
        Next
    End Sub

    Private Sub btnAdd_Click(sender As System.Object, e As System.EventArgs) Handles btnAdd.Click
        Dim DirSelect As New Windows.Forms.FolderBrowserDialog

        If lstPaths.Items.Count > 0 Then
            DirSelect.SelectedPath = CType(lstPaths.Items(lstPaths.Items.Count - 1), String)
        End If

        If DirSelect.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        lstPaths.Items.Add(CStr(DirSelect.SelectedPath))
        lstPaths.SelectedIndex = lstPaths.Items.Count - 1
    End Sub

    Private Sub btnRemove_Click(sender As System.Object, e As System.EventArgs) Handles btnRemove.Click

        If lstPaths.SelectedIndex < 0 Then
            Exit Sub
        End If

        lstPaths.Items.RemoveAt(lstPaths.SelectedIndex)
    End Sub
End Class
