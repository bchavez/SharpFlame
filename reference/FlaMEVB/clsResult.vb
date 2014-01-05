
Public MustInherit Class clsResultItemInterface
    Public MustOverride ReadOnly Property GetText As String
    Public MustOverride Sub DoubleClicked()
End Class

Public Class clsResult
    Inherits clsResultItemInterface

    Public Text As String

    Public Overrides ReadOnly Property GetText As String
        Get
            Return Text
        End Get
    End Property

    Public Class clsProblem
        Inherits clsResultItemInterface

        Public Text As String

        Public Overrides ReadOnly Property GetText As String
            Get
                Return Text
            End Get
        End Property

        Public Overrides Sub DoubleClicked()

        End Sub
    End Class

    Public Class clsWarning
        Inherits clsResultItemInterface

        Public Text As String

        Public Overrides ReadOnly Property GetText As String
            Get
                Return Text
            End Get
        End Property

        Public Overrides Sub DoubleClicked()

        End Sub
    End Class

    Private Items As New SimpleList(Of clsResultItemInterface)
    Private Bad As Boolean = False

    Public ReadOnly Property HasWarnings As Boolean
        Get
            Return (Items.Count > 0)
        End Get
    End Property

    Public ReadOnly Property HasProblems As Boolean
        Get
            Return Bad
        End Get
    End Property

    Public Sub AddBypass(ResultToAdd As clsResult)

        If ResultToAdd.HasWarnings Then
            Items.Add(ResultToAdd)
        End If
    End Sub

    Public Sub Add(ResultToAdd As clsResult)

        If ResultToAdd.HasProblems Then
            Bad = True
        End If
        If ResultToAdd.HasWarnings Then
            Items.Add(ResultToAdd)
        End If
    End Sub

    Public Sub Take(ResultToMerge As clsResult)

        If ResultToMerge.HasProblems Then
            Bad = True
        End If
        Items.AddSimpleList(ResultToMerge.Items)
    End Sub

    Public Sub ProblemAdd(Text As String)

        Dim Problem As New clsProblem
        Problem.Text = Text
        ItemAdd(Problem)
    End Sub

    Public Sub WarningAdd(Text As String)

        Dim Warning As New clsWarning
        Warning.Text = Text
        ItemAdd(Warning)
    End Sub

    Public Sub ItemAdd(item As clsResultItemInterface)

        If TypeOf item Is clsProblem Then
            Bad = True
        End If
        Items.Add(item)
    End Sub

    Public Sub New(Text As String)

        Items.MaintainOrder = True

        Me.Text = Text
    End Sub

    Public Function MakeNodes(owner As TreeNodeCollection) As TreeNode
        Dim node As New TreeNode
        node.Text = Text
        owner.Add(node)
        Dim item As clsResultItemInterface
        For i As Integer = 0 To Items.Count - 1
            item = Items(i)
            Dim ChildNode As New TreeNode
            ChildNode.Tag = item
            If TypeOf item Is clsProblem Then
                ChildNode.Text = item.GetText
                node.Nodes.Add(ChildNode)
                ChildNode.StateImageKey = "problem"
            ElseIf TypeOf item Is clsWarning Then
                ChildNode.Text = item.GetText
                node.Nodes.Add(ChildNode)
                ChildNode.StateImageKey = "warning"
            ElseIf TypeOf item Is clsResult Then
                ChildNode = CType(item, clsResult).MakeNodes(node.Nodes)
            End If
        Next
        Return node
    End Function

    Public Overrides Sub DoubleClicked()


    End Sub
End Class

Public Class clsResultWarningGoto(Of GotoType As clsResultItemGotoInterface)
    Inherits clsResult.clsWarning

    Public MapGoto As GotoType

    Public Overrides Sub DoubleClicked()
        MyBase.DoubleClicked()

        MapGoto.Perform()
    End Sub
End Class

Public Class clsResultProblemGoto(Of GotoType As clsResultItemGotoInterface)
    Inherits clsResult.clsProblem

    Public MapGoto As GotoType

    Public Overrides Sub DoubleClicked()
        MyBase.DoubleClicked()

        MapGoto.Perform()
    End Sub
End Class

Public MustInherit Class clsResultItemGotoInterface
    Public MustOverride Sub Perform()
End Class

Public Class clsResultItemTileGoto
    Inherits clsResultItemGotoInterface

    Public View As clsViewInfo
    Public TileNum As sXY_int

    Public Overrides Sub Perform()

        View.LookAtTile(TileNum)
    End Sub
End Class

Public Class clsResultItemPosGoto
    Inherits clsResultItemGotoInterface

    Public View As clsViewInfo
    Public Horizontal As sXY_int

    Public Overrides Sub Perform()

        View.LookAtPos(Horizontal)
    End Sub
End Class

Public Module modResults

    Public Function CreateResultProblemGotoForObject(unit As clsMap.clsUnit) As clsResultProblemGoto(Of clsResultItemPosGoto)

        Dim resultGoto As New clsResultItemPosGoto
        resultGoto.View = unit.MapLink.Source.ViewInfo
        resultGoto.Horizontal = unit.Pos.Horizontal
        Dim resultProblem As New clsResultProblemGoto(Of clsResultItemPosGoto)
        resultProblem.MapGoto = resultGoto
        Return resultProblem
    End Function
End Module