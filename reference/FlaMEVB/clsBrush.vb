
Public Class clsBrush

    Public Structure sPosNum
        Public Normal As sXY_int
        Public Alignment As sXY_int
    End Structure

    Private _Radius As Double
    Public Enum enumShape As Byte
        Circle
        Square
    End Enum
    Private _Shape As enumShape = enumShape.Circle

    Public Tiles As sBrushTiles

    Public ReadOnly Property Alignment As Boolean
        Get
            Return _Alignment
        End Get
    End Property
    Private _Alignment As Boolean

    Public Property Radius As Double
        Get
            Return _Radius
        End Get
        Set(value As Double)
            If _Radius = value Then
                Exit Property
            End If
            _Radius = value
            CreateTiles()
        End Set
    End Property

    Public Property Shape As enumShape
        Get
            Return _Shape
        End Get
        Set(value As enumShape)
            If _Shape = value Then
                Exit Property
            End If
            _Shape = value
            CreateTiles()
        End Set
    End Property

    Private Sub CreateTiles()
        Dim AlignmentOffset As Double = _Radius - Int(_Radius)
        Dim RadiusB As Double = Int(_Radius + 0.25#)

        _Alignment = (AlignmentOffset >= 0.25# And AlignmentOffset < 0.75#)
        Select Case _Shape
            Case enumShape.Circle
                Tiles.CreateCircle(RadiusB, 1.0#, _Alignment)
            Case enumShape.Square
                Tiles.CreateSquare(RadiusB, 1.0#, _Alignment)
        End Select
    End Sub

    Public Sub New(InitialRadius As Double, InitialShape As enumShape)

        _Radius = InitialRadius
        _Shape = InitialShape
        CreateTiles()
    End Sub

    Public Sub PerformActionMapTiles(Tool As clsMap.clsAction, Centre As sPosNum)

        PerformAction(Tool, Centre, New sXY_int(Tool.Map.Terrain.TileSize.X - 1, Tool.Map.Terrain.TileSize.Y - 1))
    End Sub

    Public Sub PerformActionMapVertices(Tool As clsMap.clsAction, Centre As sPosNum)

        PerformAction(Tool, Centre, Tool.Map.Terrain.TileSize)
    End Sub

    Public Sub PerformActionMapSectors(Tool As clsMap.clsAction, Centre As sPosNum)

        PerformAction(Tool, Centre, New sXY_int(Tool.Map.SectorCount.X - 1, Tool.Map.SectorCount.Y - 1))
    End Sub

    Public Function GetPosNum(PosNum As sPosNum) As sXY_int

        If _Alignment Then
            Return PosNum.Alignment
        Else
            Return PosNum.Normal
        End If
    End Function

    Private Sub PerformAction(Action As clsMap.clsAction, PosNum As sPosNum, LastValidNum As sXY_int)
        Dim XNum As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Centre As sXY_int

        If Action.Map Is Nothing Then
            Stop
            Exit Sub
        End If

        Centre = GetPosNum(PosNum)

        Action.Effect = 1.0#
        For Y = Clamp_int(Tiles.YMin + Centre.Y, 0, LastValidNum.Y) - Centre.Y To Clamp_int(Tiles.YMax + Centre.Y, 0, LastValidNum.Y) - Centre.Y
            Action.PosNum.Y = Centre.Y + Y
            XNum = Y - Tiles.YMin
            For X = Clamp_int(Tiles.XMin(XNum) + Centre.X, 0, LastValidNum.X) - Centre.X To Clamp_int(Tiles.XMax(XNum) + Centre.X, 0, LastValidNum.X) - Centre.X
                Action.PosNum.X = Centre.X + X
                If Action.UseEffect Then
                    If Tiles.ResultRadius > 0.0# Then
                        Select Case _Shape
                            Case clsBrush.enumShape.Circle
                                If _Alignment Then
                                    Action.Effect = 1.0# - (New Matrix3D.XY_dbl(Action.PosNum.X, Action.PosNum.Y) - New Matrix3D.XY_dbl(Centre.X - 0.5#, Centre.Y - 0.5#)).GetMagnitude / (Tiles.ResultRadius + 0.5#)
                                Else
                                    Action.Effect = 1.0# - (Centre - Action.PosNum).ToDoubles.GetMagnitude / (Tiles.ResultRadius + 0.5#)
                                End If
                            Case clsBrush.enumShape.Square
                                If _Alignment Then
                                    Action.Effect = 1.0# - Math.Max(Math.Abs(Action.PosNum.X - (Centre.X - 0.5#)), Math.Abs(Action.PosNum.Y - (Centre.Y - 0.5#))) / (Tiles.ResultRadius + 0.5#)
                                Else
                                    Action.Effect = 1.0# - Math.Max(Math.Abs(Action.PosNum.X - Centre.X), Math.Abs(Action.PosNum.Y - Centre.Y)) / (Tiles.ResultRadius + 0.5#)
                                End If
                        End Select
                    End If
                End If
                Action.ActionPerform()
            Next
        Next
    End Sub
End Class

Public Structure sBrushTiles
    Public XMin() As Integer
    Public XMax() As Integer
    Public YMin As Integer
    Public YMax As Integer
    Public ResultRadius As Double

    Public Sub CreateCircle(Radius As Double, TileSize As Double, Alignment As Boolean)
        Dim X As Integer
        Dim Y As Integer
        Dim dblX As Double
        Dim dblY As Double
        Dim RadiusB As Double
        Dim RadiusC As Double
        Dim A As Integer
        Dim B As Integer

        RadiusB = Radius / TileSize
        If Alignment Then
            RadiusB += 1.0#
            Y = CInt(Int(RadiusB))
            YMin = -Y
            YMax = Y - 1
            B = YMax - YMin
            ReDim XMin(B)
            ReDim XMax(B)
            RadiusC = RadiusB * RadiusB
            For Y = YMin To YMax
                dblY = Y + 0.5#
                dblX = Math.Sqrt(RadiusC - dblY * dblY) + 0.5#
                A = Y - YMin
                X = CInt(Int(dblX))
                XMin(A) = -X
                XMax(A) = X - 1
            Next
        Else
            RadiusB += 0.125#
            Y = CInt(Int(RadiusB))
            YMin = -Y
            YMax = Y
            B = YMax - YMin
            ReDim XMin(B)
            ReDim XMax(B)
            RadiusC = RadiusB * RadiusB
            For Y = YMin To YMax
                dblY = Y
                dblX = Math.Sqrt(RadiusC - dblY * dblY)
                A = Y - YMin
                X = CInt(Int(dblX))
                XMin(A) = -X
                XMax(A) = X
            Next
        End If

        ResultRadius = B / 2.0#
    End Sub

    Public Sub CreateSquare(Radius As Double, TileSize As Double, Alignment As Boolean)
        Dim Y As Integer
        Dim A As Integer
        Dim B As Integer
        Dim RadiusB As Double

        RadiusB = Radius / TileSize + 0.5#
        If Alignment Then
            RadiusB += 0.5#
            A = CInt(Int(RadiusB))
            YMin = -A
            YMax = A - 1
        Else
            A = CInt(Int(RadiusB))
            YMin = -A
            YMax = A
        End If
        B = YMax - YMin
        ReDim XMin(B)
        ReDim XMax(B)
        For Y = 0 To B
            XMin(Y) = YMin
            XMax(Y) = YMax
        Next

        ResultRadius = B / 2.0#
    End Sub
End Structure
