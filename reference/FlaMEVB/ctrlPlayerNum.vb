
Public Class ctrlPlayerNum

    Public tsbNumber(10) As ToolStripButton

    Private _Target As clsMap.clsUnitGroupContainer

    Public Const ScavButtonNum As Integer = 10

    Public Sub New()
        InitializeComponent()

        Dim A As Integer
        Dim B As Integer
        Dim ButtonsPerRow As Integer = 5

        For A = 0 To ButtonsPerRow - 1
            tsbNumber(A) = New ToolStripButton
            tsbNumber(A).DisplayStyle = ToolStripItemDisplayStyle.Text
            tsbNumber(A).Text = InvariantToString_int(A)
            tsbNumber(A).AutoToolTip = False
            AddHandler tsbNumber(A).Click, AddressOf tsbNumber_Clicked
            tsPlayerNum1.Items.Add(tsbNumber(A))

            B = A + ButtonsPerRow
            tsbNumber(B) = New ToolStripButton
            tsbNumber(B).DisplayStyle = ToolStripItemDisplayStyle.Text
            tsbNumber(B).Text = InvariantToString_int(B)
            tsbNumber(B).AutoToolTip = False
            AddHandler tsbNumber(B).Click, AddressOf tsbNumber_Clicked
            tsPlayerNum2.Items.Add(tsbNumber(B))
        Next

        A = 10

        tsbNumber(A) = New ToolStripButton
        tsbNumber(A).DisplayStyle = ToolStripItemDisplayStyle.Text
        tsbNumber(A).Text = "S"
        tsbNumber(A).AutoToolTip = False
        AddHandler tsbNumber(A).Click, AddressOf tsbNumber_Clicked
        tsPlayerNum2.Items.Add(tsbNumber(A))

        Width = 24 * 6
        Height = 25 * 2
    End Sub

    Private Sub tsbNumber_Clicked(sender As Object, e As EventArgs)

        If _Target Is Nothing Then
            Exit Sub
        End If

        Dim tsb As ToolStripButton = CType(sender, ToolStripButton)
        Dim UnitGroup As clsMap.clsUnitGroup = CType(tsb.Tag, clsMap.clsUnitGroup)

        _Target.Item = UnitGroup
    End Sub

    Public Property Target As clsMap.clsUnitGroupContainer
        Get
            Return _Target
        End Get
        Set(value As clsMap.clsUnitGroupContainer)
            If value Is _Target Then
                Exit Property
            End If
            If _Target IsNot Nothing Then
                RemoveHandler _Target.Changed, AddressOf SelectedChanged
            End If
            _Target = value
            If _Target IsNot Nothing Then
                AddHandler _Target.Changed, AddressOf SelectedChanged
            End If
            SelectedChanged()
        End Set
    End Property

    Private Sub SelectedChanged()

        Dim A As Integer
        Dim UnitGroup As clsMap.clsUnitGroup

        If _Target Is Nothing Then
            UnitGroup = Nothing
        Else
            UnitGroup = _Target.Item
        End If

        If UnitGroup Is Nothing Then
            For A = 0 To 10
                tsbNumber(A).Checked = False
            Next
        Else
            For A = 0 To 10
                tsbNumber(A).Checked = (CType(tsbNumber(A).Tag, clsMap.clsUnitGroup) Is UnitGroup)
            Next
        End If
    End Sub

    Public Sub SetMap(NewMap As clsMap)
        Dim A As Integer

        If NewMap Is Nothing Then
            For A = 0 To PlayerCountMax - 1
                tsbNumber(A).Tag = Nothing
            Next
            tsbNumber(ScavButtonNum).Tag = Nothing
        Else
            For A = 0 To PlayerCountMax - 1
                tsbNumber(A).Tag = NewMap.UnitGroups.Item(A)
            Next
            tsbNumber(ScavButtonNum).Tag = NewMap.ScavengerUnitGroup
        End If
        SelectedChanged()
    End Sub
End Class
