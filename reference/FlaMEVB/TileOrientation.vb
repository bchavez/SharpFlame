
Public Module TileOrientation

    Public Orientation_Clockwise As New sTileOrientation(True, False, True)
    Public Orientation_CounterClockwise As New sTileOrientation(False, True, True)
    Public Orientation_FlipX As New sTileOrientation(True, False, False)
    Public Orientation_FlipY As New sTileOrientation(False, True, False)

    Public Structure sTileOrientation
        Public ResultXFlip As Boolean
        Public ResultYFlip As Boolean
        Public SwitchedAxes As Boolean

        Public Sub New(ResultXFlip As Boolean, ResultZFlip As Boolean, SwitchedAxes As Boolean)

            Me.ResultXFlip = ResultXFlip
            Me.ResultYFlip = ResultZFlip
            Me.SwitchedAxes = SwitchedAxes
        End Sub

        Public Function GetRotated(Orientation As sTileOrientation) As sTileOrientation
            Dim ReturnResult As sTileOrientation

            ReturnResult.SwitchedAxes = (SwitchedAxes Xor Orientation.SwitchedAxes)

            If Orientation.SwitchedAxes Then
                If Orientation.ResultXFlip Then
                    ReturnResult.ResultXFlip = Not ResultYFlip
                Else
                    ReturnResult.ResultXFlip = ResultYFlip
                End If
                If Orientation.ResultYFlip Then
                    ReturnResult.ResultYFlip = Not ResultXFlip
                Else
                    ReturnResult.ResultYFlip = ResultXFlip
                End If
            Else
                If Orientation.ResultXFlip Then
                    ReturnResult.ResultXFlip = Not ResultXFlip
                Else
                    ReturnResult.ResultXFlip = ResultXFlip
                End If
                If Orientation.ResultYFlip Then
                    ReturnResult.ResultYFlip = Not ResultYFlip
                Else
                    ReturnResult.ResultYFlip = ResultYFlip
                End If
            End If

            Return ReturnResult
        End Function

        Public Sub Reverse()

            If SwitchedAxes Then
                If ResultXFlip Xor ResultYFlip Then
                    ResultXFlip = Not ResultXFlip
                    ResultYFlip = Not ResultYFlip
                End If
            End If
        End Sub

        Public Sub RotateClockwise()

            SwitchedAxes = Not SwitchedAxes
            If ResultXFlip Xor ResultYFlip Then
                ResultYFlip = Not ResultYFlip
            Else
                ResultXFlip = Not ResultXFlip
            End If
        End Sub

        Public Sub RotateAnticlockwise()

            SwitchedAxes = Not SwitchedAxes
            If ResultXFlip Xor ResultYFlip Then
                ResultXFlip = Not ResultXFlip
            Else
                ResultYFlip = Not ResultYFlip
            End If
        End Sub
    End Structure

    Public Structure sTileDirection
        Public X As Byte '0-2, 1=middle
        Public Y As Byte '0-2, 1=middle

        Public Sub New(NewX As Byte, NewY As Byte)

            X = NewX
            Y = NewY
        End Sub

        Public Function GetRotated(Orientation As sTileOrientation) As sTileDirection
            Dim ReturnResult As sTileDirection

            If Orientation.SwitchedAxes Then
                If Orientation.ResultXFlip Then
                    ReturnResult.X = CByte(2) - Y
                Else
                    ReturnResult.X = Y
                End If
                If Orientation.ResultYFlip Then
                    ReturnResult.Y = CByte(2) - X
                Else
                    ReturnResult.Y = X
                End If
            Else
                If Orientation.ResultXFlip Then
                    ReturnResult.X = CByte(2) - X
                Else
                    ReturnResult.X = X
                End If
                If Orientation.ResultYFlip Then
                    ReturnResult.Y = CByte(2) - Y
                Else
                    ReturnResult.Y = Y
                End If
            End If

            Return ReturnResult
        End Function

        Public Sub FlipX()

            X = CByte(2) - X
        End Sub

        Public Sub FlipY()

            Y = CByte(2) - Y
        End Sub

        Public Sub RotateClockwise()
            Dim byteTemp As Byte

            byteTemp = X
            X = CByte(2) - Y
            Y = byteTemp
        End Sub

        Public Sub RotateAnticlockwise()
            Dim byteTemp As Byte

            byteTemp = X
            X = Y
            Y = CByte(2) - byteTemp
        End Sub

        Public Sub SwitchAxes()
            Dim byteTemp As Byte

            byteTemp = X
            X = Y
            Y = byteTemp
        End Sub
    End Structure

    Public TileDirection_TopLeft As New sTileDirection(0, 0)
    Public TileDirection_Top As New sTileDirection(1, 0)
    Public TileDirection_TopRight As New sTileDirection(2, 0)
    Public TileDirection_Right As New sTileDirection(2, 1)
    Public TileDirection_BottomRight As New sTileDirection(2, 2)
    Public TileDirection_Bottom As New sTileDirection(1, 2)
    Public TileDirection_BottomLeft As New sTileDirection(0, 2)
    Public TileDirection_Left As New sTileDirection(0, 1)
    Public TileDirection_None As New sTileDirection(1, 1)

    Public Function OrientateTile(ByRef TileChance As clsPainter.clsTileList.sTileOrientationChance, NewDirection As sTileDirection) As clsMap.clsTerrain.Tile.sTexture
        Dim ReturnResult As clsMap.clsTerrain.Tile.sTexture

        'use random for empty tiles
        If TileChance.TextureNum < 0 Then
            ReturnResult.Orientation.ResultXFlip = (Rnd() >= 0.5F)
            ReturnResult.Orientation.ResultYFlip = (Rnd() >= 0.5F)
            ReturnResult.Orientation.SwitchedAxes = (Rnd() >= 0.5F)
            ReturnResult.TextureNum = -1
            Return ReturnResult
        End If
        'stop invalid numbers
        If TileChance.Direction.X > 2 Or TileChance.Direction.Y > 2 Or NewDirection.X > 2 Or NewDirection.Y > 2 Then
            Stop
            Return ReturnResult
        End If
        'stop different direction types
        If (NewDirection.X = 1 Xor NewDirection.Y = 1) Xor (TileChance.Direction.X = 1 Xor TileChance.Direction.Y = 1) Then
            Stop
            Return ReturnResult
        End If

        ReturnResult.TextureNum = TileChance.TextureNum

        'if a direction is neutral then give a random orientation
        If (NewDirection.X = 1 And NewDirection.Y = 1) Or (TileChance.Direction.X = 1 And TileChance.Direction.Y = 1) Then
            ReturnResult.Orientation.SwitchedAxes = (Rnd() >= 0.5F)
            ReturnResult.Orientation.ResultXFlip = (Rnd() >= 0.5F)
            ReturnResult.Orientation.ResultYFlip = (Rnd() >= 0.5F)
            Return ReturnResult
        End If

        Dim IsDiagonal As Boolean

        IsDiagonal = (NewDirection.X <> 1 And NewDirection.Y <> 1)
        If IsDiagonal Then
            ReturnResult.Orientation.SwitchedAxes = False
            'use flips to match the directions
            If TileChance.Direction.X = 0 Xor NewDirection.X = 0 Then
                ReturnResult.Orientation.ResultXFlip = True
            Else
                ReturnResult.Orientation.ResultXFlip = False
            End If
            If TileChance.Direction.Y = 0 Xor NewDirection.Y = 0 Then
                ReturnResult.Orientation.ResultYFlip = True
            Else
                ReturnResult.Orientation.ResultYFlip = False
            End If
            'randomly switch to the alternate orientation
            If Rnd() >= 0.5F Then
                ReturnResult.Orientation.SwitchedAxes = Not ReturnResult.Orientation.SwitchedAxes
                If (NewDirection.X = 0 Xor NewDirection.Y = 0) Xor (ReturnResult.Orientation.ResultXFlip Xor ReturnResult.Orientation.ResultYFlip) Then
                    ReturnResult.Orientation.ResultXFlip = Not ReturnResult.Orientation.ResultXFlip
                    ReturnResult.Orientation.ResultYFlip = Not ReturnResult.Orientation.ResultYFlip
                End If
            End If
        Else
            'switch axes if the directions are on different axes
            ReturnResult.Orientation.SwitchedAxes = (TileChance.Direction.X = 1 Xor NewDirection.X = 1)
            'use a flip to match the directions
            If ReturnResult.Orientation.SwitchedAxes Then
                If TileChance.Direction.Y <> NewDirection.X Then
                    ReturnResult.Orientation.ResultXFlip = True
                Else
                    ReturnResult.Orientation.ResultXFlip = False
                End If
                If TileChance.Direction.X <> NewDirection.Y Then
                    ReturnResult.Orientation.ResultYFlip = True
                Else
                    ReturnResult.Orientation.ResultYFlip = False
                End If
            Else
                If TileChance.Direction.X <> NewDirection.X Then
                    ReturnResult.Orientation.ResultXFlip = True
                Else
                    ReturnResult.Orientation.ResultXFlip = False
                End If
                If TileChance.Direction.Y <> NewDirection.Y Then
                    ReturnResult.Orientation.ResultYFlip = True
                Else
                    ReturnResult.Orientation.ResultYFlip = False
                End If
            End If
            'randomly switch to the alternate orientation
            If Rnd() >= 0.5F Then
                If NewDirection.X = 1 Then
                    ReturnResult.Orientation.ResultXFlip = Not ReturnResult.Orientation.ResultXFlip
                Else
                    ReturnResult.Orientation.ResultYFlip = Not ReturnResult.Orientation.ResultYFlip
                End If
            End If
        End If

        Return ReturnResult
    End Function

    Public Sub RotateDirection(InitialDirection As sTileDirection, Orientation As sTileOrientation, ByRef ResultDirection As sTileDirection)

        ResultDirection = InitialDirection
        If Orientation.SwitchedAxes Then
            ResultDirection.SwitchAxes()
        End If
        If Orientation.ResultXFlip Then
            ResultDirection.FlipX()
        End If
        If Orientation.ResultYFlip Then
            ResultDirection.FlipY()
        End If
    End Sub

    Public Function GetTileRotatedOffset(TileOrientation As sTileOrientation, Pos As sXY_int) As sXY_int
        Dim Result As sXY_int

        If TileOrientation.SwitchedAxes Then
            If TileOrientation.ResultXFlip Then
                Result.X = TerrainGridSpacing - Pos.Y
            Else
                Result.X = Pos.Y
            End If
            If TileOrientation.ResultYFlip Then
                Result.Y = TerrainGridSpacing - Pos.X
            Else
                Result.Y = Pos.X
            End If
        Else
            If TileOrientation.ResultXFlip Then
                Result.X = TerrainGridSpacing - Pos.X
            Else
                Result.X = Pos.X
            End If
            If TileOrientation.ResultYFlip Then
                Result.Y = TerrainGridSpacing - Pos.Y
            Else
                Result.Y = Pos.Y
            End If
        End If

        Return Result
    End Function

    Public Function GetTileRotatedPos_sng(TileOrientation As sTileOrientation, Pos As sXY_sng) As sXY_sng
        Dim ReturnResult As sXY_sng

        If TileOrientation.SwitchedAxes Then
            If TileOrientation.ResultXFlip Then
                ReturnResult.X = 1.0F - Pos.Y
            Else
                ReturnResult.X = Pos.Y
            End If
            If TileOrientation.ResultYFlip Then
                ReturnResult.Y = 1.0F - Pos.X
            Else
                ReturnResult.Y = Pos.X
            End If
        Else
            If TileOrientation.ResultXFlip Then
                ReturnResult.X = 1.0F - Pos.X
            Else
                ReturnResult.X = Pos.X
            End If
            If TileOrientation.ResultYFlip Then
                ReturnResult.Y = 1.0F - Pos.Y
            Else
                ReturnResult.Y = Pos.Y
            End If
        End If

        Return ReturnResult
    End Function

    Public Function GetTileRotatedPos_dbl(TileOrientation As sTileOrientation, Pos As Matrix3D.XY_dbl) As Matrix3D.XY_dbl
        Dim ReturnResult As Matrix3D.XY_dbl

        If TileOrientation.SwitchedAxes Then
            If TileOrientation.ResultXFlip Then
                ReturnResult.X = 1.0# - Pos.Y
            Else
                ReturnResult.X = Pos.Y
            End If
            If TileOrientation.ResultYFlip Then
                ReturnResult.Y = 1.0# - Pos.X
            Else
                ReturnResult.Y = Pos.X
            End If
        Else
            If TileOrientation.ResultXFlip Then
                ReturnResult.X = 1.0# - Pos.X
            Else
                ReturnResult.X = Pos.X
            End If
            If TileOrientation.ResultYFlip Then
                ReturnResult.Y = 1.0# - Pos.Y
            Else
                ReturnResult.Y = Pos.Y
            End If
        End If

        Return ReturnResult
    End Function

    Public Function GetRotatedPos(Orientation As sTileOrientation, Pos As sXY_int, Limits As sXY_int) As sXY_int
        Dim Result As sXY_int

        If Orientation.SwitchedAxes Then
            If Orientation.ResultXFlip Then
                Result.X = Limits.Y - Pos.Y
            Else
                Result.X = Pos.Y
            End If
            If Orientation.ResultYFlip Then
                Result.Y = Limits.X - Pos.X
            Else
                Result.Y = Pos.X
            End If
        Else
            If Orientation.ResultXFlip Then
                Result.X = Limits.X - Pos.X
            Else
                Result.X = Pos.X
            End If
            If Orientation.ResultYFlip Then
                Result.Y = Limits.Y - Pos.Y
            Else
                Result.Y = Pos.Y
            End If
        End If

        Return Result
    End Function

    Public Function GetRotatedAngle(Orientation As sTileOrientation, Angle As Double) As Double
        Dim XY_dbl As Matrix3D.XY_dbl

        XY_dbl = GetTileRotatedPos_dbl(Orientation, New Matrix3D.XY_dbl((Math.Cos(Angle) + 1.0#) / 2.0#, (Math.Sin(Angle) + 1.0#) / 2.0#))
        XY_dbl.X = XY_dbl.X * 2.0# - 1.0#
        XY_dbl.Y = XY_dbl.Y * 2.0# - 1.0#
        Return Math.Atan2(XY_dbl.Y, XY_dbl.X)
    End Function

    Public Sub GetTileRotatedTexCoords(TileOrientation As sTileOrientation, ByRef CoordA As sXY_sng, ByRef CoordB As sXY_sng, ByRef CoordC As sXY_sng, ByRef CoordD As sXY_sng)
        Dim ReverseOrientation As sTileOrientation

        ReverseOrientation = TileOrientation
        ReverseOrientation.Reverse()

        If ReverseOrientation.SwitchedAxes Then
            If ReverseOrientation.ResultXFlip Then
                CoordA.X = 1.0F
                CoordB.X = 1.0F
                CoordC.X = 0.0F
                CoordD.X = 0.0F
            Else
                CoordA.X = 0.0F
                CoordB.X = 0.0F
                CoordC.X = 1.0F
                CoordD.X = 1.0F
            End If
            If ReverseOrientation.ResultYFlip Then
                CoordA.Y = 1.0F
                CoordB.Y = 0.0F
                CoordC.Y = 1.0F
                CoordD.Y = 0.0F
            Else
                CoordA.Y = 0.0F
                CoordB.Y = 1.0F
                CoordC.Y = 0.0F
                CoordD.Y = 1.0F
            End If
        Else
            If ReverseOrientation.ResultXFlip Then
                CoordA.X = 1.0F
                CoordB.X = 0.0F
                CoordC.X = 1.0F
                CoordD.X = 0.0F
            Else
                CoordA.X = 0.0F
                CoordB.X = 1.0F
                CoordC.X = 0.0F
                CoordD.X = 1.0F
            End If
            If ReverseOrientation.ResultYFlip Then
                CoordA.Y = 1.0F
                CoordB.Y = 1.0F
                CoordC.Y = 0.0F
                CoordD.Y = 0.0F
            Else
                CoordA.Y = 0.0F
                CoordB.Y = 0.0F
                CoordC.Y = 1.0F
                CoordD.Y = 1.0F
            End If
        End If
    End Sub

    Public Sub TileOrientation_To_OldOrientation(TileOrientation As sTileOrientation, ByRef OutputRotation As Byte, ByRef OutputFlipX As Boolean)

        If TileOrientation.SwitchedAxes Then
            If TileOrientation.ResultXFlip Then
                OutputRotation = 1
            Else
                OutputRotation = 3
            End If
            OutputFlipX = Not (TileOrientation.ResultXFlip Xor TileOrientation.ResultYFlip)
        Else
            If TileOrientation.ResultYFlip Then
                OutputRotation = 2
            Else
                OutputRotation = 0
            End If
            OutputFlipX = (TileOrientation.ResultXFlip Xor TileOrientation.ResultYFlip)
        End If
    End Sub

    Public Sub OldOrientation_To_TileOrientation(OldRotation As Byte, OldFlipX As Boolean, OldFlipZ As Boolean, ByRef Result As sTileOrientation)

        If OldRotation = 0 Then
            Result.SwitchedAxes = False
            Result.ResultXFlip = False
            Result.ResultYFlip = False
        ElseIf OldRotation = 1 Then
            Result.SwitchedAxes = True
            Result.ResultXFlip = True
            Result.ResultYFlip = False
        ElseIf OldRotation = 2 Then
            Result.SwitchedAxes = False
            Result.ResultXFlip = True
            Result.ResultYFlip = True
        ElseIf OldRotation = 3 Then
            Result.SwitchedAxes = True
            Result.ResultXFlip = False
            Result.ResultYFlip = True
        End If
        If OldFlipX Then
            If Result.SwitchedAxes Then
                Result.ResultYFlip = Not Result.ResultYFlip
            Else
                Result.ResultXFlip = Not Result.ResultXFlip
            End If
        End If
        If OldFlipZ Then
            If Result.SwitchedAxes Then
                Result.ResultXFlip = Not Result.ResultXFlip
            Else
                Result.ResultYFlip = Not Result.ResultYFlip
            End If
        End If
    End Sub

    Public Function IdenticalTileDirections(TileOrientationA As sTileDirection, TileOrientationB As sTileDirection) As Boolean

        Return (TileOrientationA.X = TileOrientationB.X And TileOrientationA.Y = TileOrientationB.Y)
    End Function

    Public Function DirectionsOnSameSide(DirectionA As sTileDirection, DirectionB As sTileDirection) As Boolean

        If DirectionA.X = 0 Then
            If DirectionB.X = 0 Then
                Return True
            End If
        End If
        If DirectionA.X = 2 Then
            If DirectionB.X = 2 Then
                Return True
            End If
        End If
        If DirectionA.Y = 0 Then
            If DirectionB.Y = 0 Then
                Return True
            End If
        End If
        If DirectionA.Y = 2 Then
            If DirectionB.Y = 2 Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function DirectionsAreInLine(DirectionA As sTileDirection, DirectionB As sTileDirection) As Boolean

        If DirectionA.X = DirectionB.X Then
            Return True
        End If
        If DirectionA.Y = DirectionB.Y Then
            Return True
        End If
        Return False
    End Function
End Module
