Public Class clsBooleanMap

    Public Class clsValueData
        Public Size As sXY_int
        Public Value(,) As Boolean
    End Class
    Public ValueData As New clsValueData

    Public Sub Blank(SizeX As Integer, SizeY As Integer)

        ValueData.Size.X = SizeX
        ValueData.Size.Y = SizeY
        ReDim ValueData.Value(SizeY - 1, SizeX - 1)
    End Sub

    Public Sub SizeCopy(Source As clsBooleanMap)

        ValueData.Size.X = Source.ValueData.Size.X
        ValueData.Size.Y = Source.ValueData.Size.Y
        ReDim ValueData.Value(ValueData.Size.Y - 1, ValueData.Size.X - 1)
    End Sub

    Public Sub Copy(Source As clsBooleanMap)
        Dim X As Integer
        Dim Y As Integer

        SizeCopy(Source)
        For Y = 0 To Source.ValueData.Size.Y - 1
            For X = 0 To Source.ValueData.Size.X - 1
                ValueData.Value(Y, X) = Source.ValueData.Value(Y, X)
            Next
        Next
    End Sub

    Public Sub Convert_Heightmap(Source As clsHeightmap, AtOrAboveThisHeightEqualsTrue As Long)
        Dim X As Integer
        Dim Y As Integer

        ValueData.Size.X = Source.HeightData.SizeX
        ValueData.Size.Y = Source.HeightData.SizeY
        ReDim ValueData.Value(ValueData.Size.Y - 1, ValueData.Size.X - 1)
        For Y = 0 To Source.HeightData.SizeY - 1
            For X = 0 To Source.HeightData.SizeX - 1
                ValueData.Value(Y, X) = (Source.HeightData.Height(Y, X) >= AtOrAboveThisHeightEqualsTrue)
            Next
        Next
    End Sub

    Public Sub Remove_Diagonals()
        Dim X As Integer
        Dim Y As Integer
        Dim Flag As Boolean

        X = 0
        Y = 0
        Do While Y < ValueData.Size.Y - 1
            X = 0
            Do While X < ValueData.Size.X - 1
                Flag = False
                If ValueData.Value(Y, X) Then
                    If ValueData.Value(Y, X + 1) Then
                        If ValueData.Value(Y + 1, X) Then
                            If ValueData.Value(Y + 1, X + 1) Then
                                'i i i i
                            ElseIf Not ValueData.Value(Y + 1, X + 1) Then
                                'i i i o
                            End If
                        ElseIf Not ValueData.Value(Y + 1, X) Then
                            If ValueData.Value(Y + 1, X + 1) Then
                                'i i o i
                            ElseIf Not ValueData.Value(Y + 1, X + 1) Then
                                'i i o o
                            End If
                        End If
                    ElseIf Not ValueData.Value(Y, X + 1) Then
                        If ValueData.Value(Y + 1, X) Then
                            If ValueData.Value(Y + 1, X + 1) Then
                                'i o i i
                            ElseIf Not ValueData.Value(Y + 1, X + 1) Then
                                'i o i o
                            End If
                        ElseIf Not ValueData.Value(Y + 1, X) Then
                            If ValueData.Value(Y + 1, X + 1) Then
                                'i o o i
                                If Rnd() < 0.5F Then
                                    ValueData.Value(Y, X) = False
                                Else
                                    ValueData.Value(Y + 1, X + 1) = False
                                End If
                                Flag = True
                            ElseIf Not ValueData.Value(Y + 1, X + 1) Then
                                'i o o o
                            End If
                        End If
                    End If
                ElseIf Not ValueData.Value(Y, X) Then
                    If ValueData.Value(Y, X + 1) Then
                        If ValueData.Value(Y + 1, X) Then
                            If ValueData.Value(Y + 1, X + 1) Then
                                'o i i i
                            ElseIf Not ValueData.Value(Y + 1, X + 1) Then
                                'o i i o
                                If Rnd() < 0.5F Then
                                    ValueData.Value(Y, X + 1) = False
                                Else
                                    ValueData.Value(Y + 1, X) = False
                                End If
                                Flag = True
                            End If
                        ElseIf Not ValueData.Value(Y + 1, X) Then
                            If ValueData.Value(Y + 1, X + 1) Then
                                'o i o i
                            ElseIf Not ValueData.Value(Y + 1, X + 1) Then
                                'o i o o
                            End If
                        End If
                    ElseIf Not ValueData.Value(Y, X + 1) Then
                        If ValueData.Value(Y + 1, X) Then
                            If ValueData.Value(Y + 1, X + 1) Then
                                'o o i i
                            ElseIf Not ValueData.Value(Y + 1, X + 1) Then
                                'o o i o
                            End If
                        ElseIf Not ValueData.Value(Y + 1, X) Then
                            If ValueData.Value(Y + 1, X + 1) Then
                                'o o o i
                            ElseIf Not ValueData.Value(Y + 1, X + 1) Then
                                'o o o o
                            End If
                        End If
                    End If
                End If
                'when flag, go back one in each direction, incase a new diagonal was created
                If Flag Then
                    If X > 0 Then X -= 1
                    Exit Do
                Else
                    X += 1
                End If
            Loop
            If Flag Then
                If Y > 0 Then Y -= 1
            Else
                Y += 1
            End If
        Loop
    End Sub

    Public Sub Expand_One_Tile(Source As clsBooleanMap)
        Dim X As Integer
        Dim Y As Integer

        SizeCopy(Source)
        For Y = 0 To ValueData.Size.Y - 1
            For X = 0 To Source.ValueData.Size.X - 1
                If Source.ValueData.Value(Y, X) Then
                    ValueData.Value(Y, X) = True
                    If Y > 0 Then
                        If X > 0 Then
                            ValueData.Value(Y - 1, X - 1) = True
                        End If
                        ValueData.Value(Y - 1, X) = True
                        If X < Source.ValueData.Size.X - 1 Then
                            ValueData.Value(Y - 1, X + 1) = True
                        End If
                    End If
                    If X > 0 Then
                        ValueData.Value(Y, X - 1) = True
                    End If
                    ValueData.Value(Y, X) = True
                    If X < ValueData.Size.X - 1 Then
                        ValueData.Value(Y, X + 1) = True
                    End If
                    If Y < ValueData.Size.Y - 1 Then
                        If X > 0 Then
                            ValueData.Value(Y + 1, X - 1) = True
                        End If
                        ValueData.Value(Y + 1, X) = True
                        If X < ValueData.Size.X - 1 Then
                            ValueData.Value(Y + 1, X + 1) = True
                        End If
                    End If
                End If
            Next
        Next
    End Sub

    Public Sub Remove(Source As clsBooleanMap, Remove As clsBooleanMap)
        Dim X As Integer
        Dim Y As Integer

        SizeCopy(Source)
        For Y = 0 To Source.ValueData.Size.Y - 1
            For X = 0 To Source.ValueData.Size.X - 1
                If Remove.ValueData.Value(Y, X) Then
                    ValueData.Value(Y, X) = False
                Else
                    ValueData.Value(Y, X) = Source.ValueData.Value(Y, X)
                End If
            Next
        Next
    End Sub

    Public Sub Combine(Source As clsBooleanMap, Insert As clsBooleanMap)
        Dim X As Integer
        Dim Y As Integer

        SizeCopy(Source)
        For Y = 0 To Source.ValueData.Size.Y - 1
            For X = 0 To Source.ValueData.Size.X - 1
                If Insert.ValueData.Value(Y, X) Then
                    ValueData.Value(Y, X) = True
                Else
                    ValueData.Value(Y, X) = Source.ValueData.Value(Y, X)
                End If
            Next
        Next
    End Sub

    Public Sub Within(Interior As clsBooleanMap, Exterior As clsBooleanMap)
        Dim Y As Integer
        Dim X As Integer
        Dim Flag As Boolean

        SizeCopy(Interior)
        For Y = 0 To Interior.ValueData.Size.Y - 1
            For X = 0 To Interior.ValueData.Size.X - 1
                If Interior.ValueData.Value(Y, X) Then
                    Flag = False
                    If Y > 0 Then
                        If X > 0 Then
                            If Not Exterior.ValueData.Value(Y - 1, X - 1) Then Flag = True
                        End If
                        If Not Exterior.ValueData.Value(Y - 1, X) Then Flag = True
                        If X < Interior.ValueData.Size.X - 1 Then
                            If Not Exterior.ValueData.Value(Y - 1, X + 1) Then Flag = True
                        End If
                    End If
                    If X > 0 Then
                        If Not Exterior.ValueData.Value(Y, X - 1) Then Flag = True
                    End If
                    If Not Exterior.ValueData.Value(Y, X) Then Flag = True
                    If X < Interior.ValueData.Size.X - 1 Then
                        If Not Exterior.ValueData.Value(Y, X + 1) Then Flag = True
                    End If
                    If Y < Interior.ValueData.Size.Y - 1 Then
                        If X > 0 Then
                            If Not Exterior.ValueData.Value(Y + 1, X - 1) Then Flag = True
                        End If
                        If Not Exterior.ValueData.Value(Y + 1, X) Then Flag = True
                        If X < Interior.ValueData.Size.X - 1 Then
                            If Not Exterior.ValueData.Value(Y + 1, X + 1) Then Flag = True
                        End If
                    End If
                    ValueData.Value(Y, X) = Not Flag
                End If
            Next
        Next
    End Sub
End Class