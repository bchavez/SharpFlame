
Public Module modMath

    Public Const RadOf1Deg As Double = Math.PI / 180.0#
    Public Const RadOf90Deg As Double = Math.PI / 2.0#
    Public Const RadOf360Deg As Double = Math.PI * 2.0#

    Public Const RootTwo As Double = 1.4142135623730951#

    Public Structure sXY_int
        Public X As Integer
        Public Y As Integer

        Public Sub New(X As Integer, Y As Integer)
            Me.X = X
            Me.Y = Y
        End Sub

        Public Shared Operator =(a As sXY_int, b As sXY_int) As Boolean

            Return ((a.X = b.X) And (a.Y = b.Y))
        End Operator

        Public Shared Operator <>(a As sXY_int, b As sXY_int) As Boolean

            Return ((a.X <> b.X) Or (a.Y <> b.Y))
        End Operator

        Public Shared Operator +(a As sXY_int, b As sXY_int) As sXY_int
            Dim result As sXY_int

            result.X = a.X + b.X
            result.Y = a.Y + b.Y

            Return result
        End Operator

        Public Shared Operator -(a As sXY_int, b As sXY_int) As sXY_int
            Dim result As sXY_int

            result.X = a.X - b.X
            result.Y = a.Y - b.Y

            Return result
        End Operator

        Public Shared Operator *(a As sXY_int, b As Double) As Matrix3D.XY_dbl
            Dim result As Matrix3D.XY_dbl

            result.X = a.X * b
            result.Y = a.Y * b

            Return result
        End Operator

        Public Shared Operator /(a As sXY_int, b As Double) As Matrix3D.XY_dbl
            Dim result As Matrix3D.XY_dbl

            result.X = a.X / b
            result.Y = a.Y / b

            Return result
        End Operator

        Public Function ToDoubles() As Matrix3D.XY_dbl
            Dim result As Matrix3D.XY_dbl

            result.X = X
            result.Y = Y

            Return result
        End Function

        Public Shared Function Min(a As sXY_int, b As sXY_int) As sXY_int
            Dim result As sXY_int

            result.X = Math.Min(a.X, b.X)
            result.Y = Math.Min(a.Y, b.Y)

            Return result
        End Function

        Public Shared Function Max(a As sXY_int, b As sXY_int) As sXY_int
            Dim result As sXY_int

            result.X = Math.Max(a.X, b.X)
            result.Y = Math.Max(a.Y, b.Y)

            Return result
        End Function

        Public Function IsInRange(Minimum As sXY_int, Maximum As sXY_int) As Boolean

            Return (X >= Minimum.X And X <= Maximum.X _
                    And Y >= Minimum.Y And Y <= Maximum.Y)
        End Function

    End Structure

    Public Class clsXY_int
        Public XY As sXY_int

        Public Property X As Integer
            Get
                Return XY.X
            End Get
            Set(value As Integer)
                XY.X = value
            End Set
        End Property
        Public Property Y As Integer
            Get
                Return XY.Y
            End Get
            Set(value As Integer)
                XY.Y = value
            End Set
        End Property

        Public Sub New(XY As sXY_int)

            Me.XY = XY
        End Sub
    End Class

    Public Structure sXY_uint
        Public X As UInteger
        Public Y As UInteger

        Public Sub New(X As UInteger, Y As UInteger)
            Me.X = X
            Me.Y = Y
        End Sub
    End Structure

    Public Structure sXY_sng
        Public X As Single
        Public Y As Single

        Public Sub New(X As Single, Y As Single)
            Me.X = X
            Me.Y = Y
        End Sub
    End Structure

    Public Structure sXYZ_int
        Public X As Integer
        Public Y As Integer
        Public Z As Integer

        Public Sub New(X As Integer, Y As Integer, Z As Integer)
            Me.X = X
            Me.Y = Y
            Me.Z = Z
        End Sub

        Public Sub Add_dbl(XYZ As Matrix3D.XYZ_dbl)
            X += CInt(XYZ.X)
            Y += CInt(XYZ.Y)
            Z += CInt(XYZ.Z)
        End Sub

        Public Sub Set_dbl(XYZ As Matrix3D.XYZ_dbl)
            X = CInt(XYZ.X)
            Y = CInt(XYZ.Y)
            Z = CInt(XYZ.Z)
        End Sub
    End Structure

    Public Structure sXYZ_sng
        Public X As Single
        Public Y As Single
        Public Z As Single

        Public Sub New(X As Single, Y As Single, Z As Single)
            Me.X = X
            Me.Y = Y
            Me.Z = Z
        End Sub
    End Structure

    Public Function AngleClamp(Angle As Double) As Double
        Dim ReturnResult As Double

        ReturnResult = Angle
        If ReturnResult < -Math.PI Then
            ReturnResult += RadOf360Deg
        ElseIf ReturnResult >= Math.PI Then
            ReturnResult -= RadOf360Deg
        End If
        Return ReturnResult
    End Function

    Public Function Clamp_dbl(Amount As Double, Minimum As Double, Maximum As Double) As Double
        Dim ReturnResult As Double

        ReturnResult = Amount
        If ReturnResult < Minimum Then
            ReturnResult = Minimum
        ElseIf ReturnResult > Maximum Then
            ReturnResult = Maximum
        End If
        Return ReturnResult
    End Function

    Public Function Clamp_sng(Amount As Single, Minimum As Single, Maximum As Single) As Single
        Dim ReturnResult As Single

        ReturnResult = Amount
        If ReturnResult < Minimum Then
            ReturnResult = Minimum
        ElseIf ReturnResult > Maximum Then
            ReturnResult = Maximum
        End If
        Return ReturnResult
    End Function

    Public Function Clamp_int(Amount As Integer, Minimum As Integer, Maximum As Integer) As Integer
        Dim ReturnResult As Integer

        ReturnResult = Amount
        If ReturnResult < Minimum Then
            ReturnResult = Minimum
        ElseIf ReturnResult > Maximum Then
            ReturnResult = Maximum
        End If
        Return ReturnResult
    End Function

    Public Structure sIntersectPos
        Public Exists As Boolean
        Public Pos As sXY_int
    End Structure

    Public Function GetLinesIntersectBetween(A1 As sXY_int, A2 As sXY_int, B1 As sXY_int, B2 As sXY_int) As sIntersectPos
        Dim Result As sIntersectPos

        If (A1.X = A2.X And A1.Y = A2.Y) Or (B1.X = B2.X And B1.Y = B2.Y) Then
            Result.Exists = False
        Else
            Dim y1dif As Double
            Dim x1dif As Double
            Dim adifx As Double
            Dim adify As Double
            Dim bdifx As Double
            Dim bdify As Double
            Dim m As Double
            Dim ar As Double
            Dim br As Double

            y1dif = B1.Y - A1.Y
            x1dif = B1.X - A1.X
            adifx = A2.X - A1.X
            adify = A2.Y - A1.Y
            bdifx = B2.X - B1.X
            bdify = B2.Y - B1.Y
            m = adifx * bdify - adify * bdifx
            If m = 0.0# Then
                Result.Exists = False
            Else
                ar = (x1dif * bdify - y1dif * bdifx) / m
                br = (x1dif * adify - y1dif * adifx) / m
                If ar <= 0.0# Or ar >= 1.0# Or br <= 0.0# Or br >= 1.0# Then
                    Result.Exists = False
                Else
                    Result.Pos.X = A1.X + CInt(ar * adifx)
                    Result.Pos.Y = A1.Y + CInt(ar * adify)
                    Result.Exists = True
                End If
            End If
        End If
        Return Result
    End Function

    Public Function PointGetClosestPosOnLine(LinePointA As sXY_int, LinePointB As sXY_int, Point As sXY_int) As sXY_int
        Dim x1dif As Double = Point.X - LinePointA.X
        Dim y1dif As Double = Point.Y - LinePointA.Y
        Dim adifx As Double = LinePointB.X - LinePointA.X
        Dim adify As Double = LinePointB.Y - LinePointA.Y
        Dim m As Double

        m = adifx * adifx + adify * adify
        If m = 0.0# Then
            Return LinePointA
        Else
            Dim ar As Double
            ar = (x1dif * adifx + y1dif * adify) / m
            If ar <= 0.0# Then
                Return LinePointA
            ElseIf ar >= 1.0# Then
                Return LinePointB
            Else
                Dim Result As sXY_int
                Result.X = LinePointA.X + CInt(adifx * ar)
                Result.Y = LinePointA.Y + CInt(adify * ar)
                Return Result
            End If
        End If
    End Function

    Public Sub ReorderXY(A As sXY_int, B As sXY_int, ByRef Lesser As sXY_int, ByRef Greater As sXY_int)

        If A.X <= B.X Then
            Lesser.X = A.X
            Greater.X = B.X
        Else
            Lesser.X = B.X
            Greater.X = A.X
        End If
        If A.Y <= B.Y Then
            Lesser.Y = A.Y
            Greater.Y = B.Y
        Else
            Lesser.Y = B.Y
            Greater.Y = A.Y
        End If
    End Sub
End Module
