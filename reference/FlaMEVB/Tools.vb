
Public Module modTools

    Public Structure sEachTool
        Public ObjectSelect As clsTool
        Public TextureBrush As clsTool
        Public TerrainBrush As clsTool
        Public TerrainFill As clsTool
        Public RoadPlace As clsTool
        Public RoadLines As clsTool
        Public RoadRemove As clsTool
        Public CliffTriangle As clsTool
        Public CliffBrush As clsTool
        Public CliffRemove As clsTool
        Public HeightSetBrush As clsTool
        Public HeightChangeBrush As clsTool
        Public HeightSmoothBrush As clsTool
        Public ObjectPlace As clsTool
        Public ObjectLines As clsTool
        Public TerrainSelect As clsTool
        Public Gateways As clsTool
    End Structure
    Public Tools As sEachTool

    Public Sub CreateTools()

        Dim newTool As clsTool

        newTool = New clsTool
        Tools.ObjectSelect = newTool

        newTool = New clsTool
        Tools.TextureBrush = newTool

        newTool = New clsTool
        Tools.TerrainBrush = newTool

        newTool = New clsTool
        Tools.TerrainFill = newTool

        newTool = New clsTool
        Tools.RoadPlace = newTool

        newTool = New clsTool
        Tools.RoadLines = newTool

        newTool = New clsTool
        Tools.RoadRemove = newTool

        newTool = New clsTool
        Tools.CliffTriangle = newTool

        newTool = New clsTool
        Tools.CliffBrush = newTool

        newTool = New clsTool
        Tools.CliffRemove = newTool

        newTool = New clsTool
        Tools.HeightSetBrush = newTool

        newTool = New clsTool
        Tools.HeightChangeBrush = newTool

        newTool = New clsTool
        Tools.HeightSmoothBrush = newTool

        newTool = New clsTool
        Tools.ObjectPlace = newTool

        newTool = New clsTool
        Tools.ObjectLines = newTool

        newTool = New clsTool
        Tools.TerrainSelect = newTool

        newTool = New clsTool
        Tools.Gateways = newTool

        _Tool = Tools.TextureBrush
        _PreviousTool = _Tool
    End Sub

    Private _Tool As clsTool
    Public Property Tool As clsTool
        Get
            Return _Tool
        End Get
        Set(value As clsTool)
            _PreviousTool = _Tool
            _Tool = value
        End Set
    End Property
    Private _PreviousTool As clsTool
    Public ReadOnly Property PreviousTool As clsTool
        Get
            Return _PreviousTool
        End Get
    End Property
End Module

Public Class clsTool

End Class
