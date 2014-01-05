Public Class clsHeightmap

    Public HeightScale As Double = 0.0001#

    Public Structure sMinMax
        Public Min As Long
        Public Max As Long
    End Structure

    Public Class clsHeightData
        Public SizeX As Integer
        Public SizeY As Integer
        Public Height(,) As Long
    End Class
    Public HeightData As New clsHeightData

    Public Sub Blank(SizeY As Integer, SizeX As Integer)

        HeightData.SizeX = SizeX
        HeightData.SizeY = SizeY
        ReDim HeightData.Height(SizeY - 1, SizeX - 1)
    End Sub

    Public Sub Randomize(HeightMultiplier As Double)
        Dim X As Integer
        Dim Y As Integer
        Dim HeightMultiplierHalved As Long

        HeightMultiplierHalved = CLng(HeightMultiplier / 2.0#)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(Rnd() * HeightMultiplier - HeightMultiplierHalved)
            Next
        Next
    End Sub

    Public Sub GenerateNew(SizeY As Integer, SizeX As Integer, Inflations As Integer, NoiseFactor As Double, HeightMultiplier As Double)
        Dim Temp As New clsHeightmap

        Blank(SizeY, SizeX)
        Randomize(HeightMultiplier / HeightScale)
        Temp.HeightScale = HeightScale
        Temp.Generate(Me, Inflations, NoiseFactor, HeightMultiplier / HeightScale)
        HeightData = Temp.HeightData 'steal the temporary heightmap's data
    End Sub

    Public Sub Generate(Source As clsHeightmap, Inflations As Integer, NoiseFactor As Double, HeightMultiplier As Double)
        Dim Temp As New clsHeightmap
        Dim A As Integer

        If Inflations >= 1 Then
            Temp.Inflate(Source, NoiseFactor, HeightMultiplier, 1)
            HeightData = Temp.HeightData
            Temp.HeightData = New clsHeightmap.clsHeightData
            For A = 2 To Inflations
                Temp.Inflate(Me, NoiseFactor, HeightMultiplier, A)
                HeightData = Temp.HeightData
                Temp.HeightData = New clsHeightmap.clsHeightData
            Next
        ElseIf Inflations = 0 Then
            Copy(Source)
        Else
            Exit Sub
        End If
    End Sub

    Public Sub Inflate(Source As clsHeightmap, NoiseFactor As Double, HeightMultiplier As Double, VariationReduction As Integer)

        Dim A As Integer
        Dim Y As Integer
        Dim X As Integer

        Dim Variation As Double
        Dim VariationHalved As Long
        Dim Mean As Long
        Dim Dist As Double
        Dim LayerFactor As Double

        'make a larger copy of heightmap
        If Source.HeightData.SizeY = 0 Or Source.HeightData.SizeX = 0 Then
            Exit Sub
        End If
        Blank((Source.HeightData.SizeY - 1) * 2 + 1, (Source.HeightData.SizeX - 1) * 2 + 1)
        For Y = 0 To Source.HeightData.SizeY - 1
            For X = 0 To Source.HeightData.SizeX - 1
                HeightData.Height((Y + 1) * 2 - 2, (X + 1) * 2 - 2) = Source.HeightData.Height(Y, X)
            Next
        Next

        If NoiseFactor = 0.0# Then
            LayerFactor = 0.0#
        Else
            LayerFactor = (2.0# / NoiseFactor) ^ (-VariationReduction)
        End If

        'centre points
        Dist = RootTwo
        Variation = Dist * LayerFactor * HeightMultiplier
        VariationHalved = CLng(Variation / 2.0#)
        For Y = 1 To HeightData.SizeY - 2 Step 2
            For X = 1 To HeightData.SizeX - 2 Step 2
                Mean = CLng((HeightData.Height(Y - 1, X - 1) + HeightData.Height(Y - 1, X + 1) + HeightData.Height(Y + 1, X - 1) + HeightData.Height(Y + 1, X + 1)) / 4.0#)
                HeightData.Height(Y, X) = Mean + CLng(Rnd() * Variation) - VariationHalved
            Next
        Next

        'side points
        Dist = 1.0#
        Variation = Dist * LayerFactor * HeightMultiplier
        VariationHalved = CLng(Variation / 2.0#)
        'inner side points
        For Y = 1 To HeightData.SizeY - 2
            A = Y - CInt(Int(Y / 2.0#)) * 2
            For X = 1 + A To HeightData.SizeX - 2 - A Step 2
                Mean = CLng((HeightData.Height(Y - 1, X) + HeightData.Height(Y, X - 1) + HeightData.Height(Y, X + 1) + HeightData.Height(Y + 1, X)) / 4.0#)
                HeightData.Height(Y, X) = Mean + CLng(Rnd() * Variation) - VariationHalved
            Next
        Next
        'top side points
        Y = 0
        For X = 1 To HeightData.SizeX - 2 Step 2
            Mean = CLng((HeightData.Height(Y, X - 1) + HeightData.Height(Y, X + 1) + HeightData.Height(Y + 1, X)) / 3.0#)
            HeightData.Height(Y, X) = Mean + CLng(Rnd() * Variation) - VariationHalved
        Next
        'left side points
        X = 0
        For Y = 1 To HeightData.SizeY - 2 Step 2
            Mean = CLng((HeightData.Height(Y - 1, X) + HeightData.Height(Y, X + 1) + HeightData.Height(Y + 1, X)) / 3.0#)
            HeightData.Height(Y, X) = Mean + CLng(Rnd() * Variation) - VariationHalved
        Next
        'right side points
        X = HeightData.SizeX - 1
        For Y = 1 To HeightData.SizeY - 2 Step 2
            Mean = CLng((HeightData.Height(Y - 1, X) + HeightData.Height(Y, X - 1) + HeightData.Height(Y + 1, X)) / 3.0#)
            HeightData.Height(Y, X) = Mean + CLng(Rnd() * Variation) - VariationHalved
        Next
        'bottom side points
        Y = HeightData.SizeY - 1
        For X = 1 To HeightData.SizeX - 2 Step 2
            Mean = CLng((HeightData.Height(Y - 1, X) + HeightData.Height(Y, X - 1) + HeightData.Height(Y, X + 1)) / 3.0#)
            HeightData.Height(Y, X) = Mean + CLng(Rnd() * Variation) - VariationHalved
        Next
    End Sub

    Public Sub MinMaxGet(ByRef MinMax_Output As sMinMax)

        Dim HeightMin As Long
        Dim HeightMax As Long
        Dim lngTemp As Long
        Dim Y As Integer
        Dim X As Integer

        If Not (HeightData.SizeY = 0 Or HeightData.SizeX = 0) Then
            HeightMin = HeightData.Height(0, 0)
            HeightMax = HeightData.Height(0, 0)
            For Y = 0 To HeightData.SizeY - 1
                For X = 0 To HeightData.SizeX - 1
                    lngTemp = HeightData.Height(Y, X)
                    If lngTemp < HeightMin Then HeightMin = lngTemp
                    If lngTemp > HeightMax Then HeightMax = lngTemp
                Next
            Next
        End If
        MinMax_Output.Min = HeightMin
        MinMax_Output.Max = HeightMax
    End Sub

    Public Sub Copy(Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer

        HeightScale = Source.HeightScale
        SizeCopy(Source)
        For Y = 0 To Source.HeightData.SizeY - 1
            For X = 0 To Source.HeightData.SizeX - 1
                HeightData.Height(Y, X) = Source.HeightData.Height(Y, X)
            Next
        Next
    End Sub

    Public Function IsSizeSame(Source As clsHeightmap) As Boolean

        Return ((HeightData.SizeX = Source.HeightData.SizeX) And (HeightData.SizeY = Source.HeightData.SizeY))
    End Function

    Public Sub Multiply2(SourceA As clsHeightmap, SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(SourceA.HeightData.Height(Y, X) * SourceA.HeightScale * SourceB.HeightData.Height(Y, X) * SourceB.HeightScale / HeightScale)
            Next
        Next
    End Sub

    Public Sub Multiply(Source As clsHeightmap, Multiplier As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale * Multiplier / HeightScale

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(Source.HeightData.Height(Y, X) * dblTemp)
            Next
        Next
    End Sub

    Public Sub Divide2(SourceA As clsHeightmap, SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = SourceA.HeightScale / (SourceB.HeightScale * HeightScale)

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(SourceA.HeightData.Height(Y, X) / SourceB.HeightData.Height(Y, X) * dblTemp)
            Next
        Next
    End Sub

    Public Sub Divide(Source As clsHeightmap, Denominator As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale / (Denominator * HeightScale)

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(Source.HeightData.Height(Y, X) * dblTemp)
            Next
        Next
    End Sub

    Public Sub Intervalise(Source As clsHeightmap, Interval As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale / Interval
        Dim dblTemp2 As Double = Interval / HeightScale

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(Int(Source.HeightData.Height(Y, X) * dblTemp) * dblTemp2)
            Next
        Next
    End Sub

    Public Sub Add2(SourceA As clsHeightmap, SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTempA As Double = SourceA.HeightScale / HeightScale
        Dim dblTempB As Double = SourceB.HeightScale / HeightScale

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(SourceA.HeightData.Height(Y, X) * dblTempA + SourceB.HeightData.Height(Y, X) * dblTempB)
            Next
        Next
    End Sub

    Public Sub Add(Source As clsHeightmap, Amount As Double)
        Dim Y As Integer
        Dim X As Integer

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng((Source.HeightData.Height(Y, X) * Source.HeightScale + Amount) / HeightScale)
            Next
        Next
    End Sub

    Public Sub Subtract2(SourceA As clsHeightmap, SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTempA As Double = SourceA.HeightScale / HeightScale
        Dim dblTempB As Double = SourceB.HeightScale / HeightScale

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(SourceA.HeightData.Height(Y, X) * dblTempA - SourceB.HeightData.Height(Y, X) * dblTempB)
            Next
        Next
    End Sub

    Public Sub Subtract(Source As clsHeightmap, Amount As Double)
        Dim Y As Integer
        Dim X As Integer

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng((Source.HeightData.Height(Y, X) * Source.HeightScale - Amount) / HeightScale)
            Next
        Next
    End Sub

    Public Sub Highest2(SourceA As clsHeightmap, SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTempA As Double
        Dim dblTempB As Double
        Dim dblTempC As Double = SourceA.HeightScale / HeightScale
        Dim dblTempD As Double = SourceB.HeightScale / HeightScale

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTempA = SourceA.HeightData.Height(Y, X) * dblTempC
                dblTempB = SourceB.HeightData.Height(Y, X) * dblTempD
                If dblTempA >= dblTempB Then
                    HeightData.Height(Y, X) = CLng(dblTempA)
                Else
                    HeightData.Height(Y, X) = CLng(dblTempB)
                End If
            Next
        Next
    End Sub

    Public Sub Highest(Source As clsHeightmap, Value As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale / HeightScale
        Dim dblTemp2 As Double = Value / HeightScale
        Dim dblTemp3 As Double

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTemp3 = Source.HeightData.Height(Y, X) * dblTemp
                If dblTemp3 >= dblTemp2 Then
                    HeightData.Height(Y, X) = CLng(dblTemp3)
                Else
                    HeightData.Height(Y, X) = CLng(dblTemp2)
                End If
            Next
        Next
    End Sub

    Public Sub Lowest2(SourceA As clsHeightmap, SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTempA As Double
        Dim dblTempB As Double
        Dim dblTempC As Double = SourceA.HeightScale / HeightScale
        Dim dblTempD As Double = SourceB.HeightScale / HeightScale

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTempA = SourceA.HeightData.Height(Y, X) * dblTempC
                dblTempB = SourceB.HeightData.Height(Y, X) * dblTempD
                If dblTempA <= dblTempB Then
                    HeightData.Height(Y, X) = CLng(dblTempA)
                Else
                    HeightData.Height(Y, X) = CLng(dblTempB)
                End If
            Next
        Next
    End Sub

    Public Sub Lowest(Source As clsHeightmap, Value As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale / HeightScale
        Dim dblTemp2 As Double = Value / HeightScale
        Dim dblTemp3 As Double

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTemp3 = Source.HeightData.Height(Y, X) * dblTemp
                If dblTemp3 <= dblTemp2 Then
                    HeightData.Height(Y, X) = CLng(dblTemp3)
                Else
                    HeightData.Height(Y, X) = CLng(dblTemp2)
                End If
            Next
        Next
    End Sub

    Public Sub Swap3(SourceA As clsHeightmap, SourceB As clsHeightmap, Swapper As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim Ratio As Double

        If Not (Swapper.IsSizeSame(SourceA) And Swapper.IsSizeSame(SourceB)) Then
            Stop
        End If
        SizeCopy(Swapper)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                Ratio = Swapper.HeightData.Height(Y, X) * Swapper.HeightScale
                HeightData.Height(Y, X) = CLng((SourceA.HeightData.Height(Y, X) * SourceA.HeightScale * (1.0# - Ratio) + SourceB.HeightData.Height(Y, X) * Ratio * SourceB.HeightScale) / HeightScale)
            Next
        Next
    End Sub

    Public Sub Clamp(Source As clsHeightmap, HeightMin As Double, HeightMax As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTemp = Source.HeightData.Height(Y, X) * Source.HeightScale
                If dblTemp < HeightMin Then
                    HeightData.Height(Y, X) = CLng(HeightMin / HeightScale)
                ElseIf dblTemp > HeightMax Then
                    HeightData.Height(Y, X) = CLng(HeightMax / HeightScale)
                Else
                    HeightData.Height(Y, X) = CLng(dblTemp / HeightScale)
                End If
            Next
        Next
    End Sub

    Public Sub Invert(Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = -Source.HeightScale / HeightScale

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(Source.HeightData.Height(Y, X) * dblTemp)
            Next
        Next
    End Sub

    Public Sub WaveLow(Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim HeightRange As Long
        Dim HeightMin As Long
        Dim MinMax As New sMinMax

        Source.MinMaxGet(MinMax)
        HeightRange = MinMax.Max - MinMax.Min
        HeightMin = MinMax.Min

        If HeightRange = 0.0# Then Exit Sub

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng(((1.0# - Math.Sin((1.0# - (Source.HeightData.Height(Y, X) - HeightMin) / HeightRange) * RadOf90Deg)) * HeightRange + HeightMin) * Source.HeightScale / HeightScale)
            Next
        Next
    End Sub

    Public Sub WaveHigh(Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim HeightRange As Long
        Dim HeightMin As Long
        Dim MinMax As New sMinMax

        Source.MinMaxGet(MinMax)
        HeightRange = MinMax.Max - MinMax.Min
        HeightMin = MinMax.Min

        If HeightRange = 0.0# Then Exit Sub

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng((Math.Sin((Source.HeightData.Height(Y, X) - HeightMin) / HeightRange * RadOf90Deg) * HeightRange + HeightMin) * Source.HeightScale / HeightScale)
            Next
        Next
    End Sub

    Public Sub Rescale(Source As clsHeightmap, HeightMin As Double, HeightMax As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim MinMax As New sMinMax

        Source.MinMaxGet(MinMax)

        Dim HeightRange As Long
        Dim Offset As Long
        Dim Ratio As Double
        Dim lngTemp As Long

        SizeCopy(Source)
        HeightRange = MinMax.Max - MinMax.Min
        Offset = 0L - MinMax.Min
        If HeightRange > 0L Then
            Ratio = (HeightMax - HeightMin) / (HeightRange * HeightScale)
            lngTemp = CLng(HeightMin / HeightScale)
            For Y = 0 To HeightData.SizeY - 1
                For X = 0 To HeightData.SizeX - 1
                    HeightData.Height(Y, X) = lngTemp + CLng((Offset + Source.HeightData.Height(Y, X)) * Ratio)
                Next
            Next
        Else
            lngTemp = CLng((HeightMin + HeightMax) / 2.0#)
            For Y = 0 To HeightData.SizeY - 1
                For X = 0 To HeightData.SizeX - 1
                    HeightData.Height(Y, X) = lngTemp
                Next
            Next
        End If
    End Sub

    Public Sub ShiftToZero(Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim MinMax As New sMinMax
        Dim dblTemp As Double = Source.HeightScale / HeightScale

        Source.MinMaxGet(MinMax)

        Dim Offset As Long
        SizeCopy(Source)
        Offset = 0L - MinMax.Min
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = CLng((Offset + Source.HeightData.Height(Y, X)) * dblTemp)
            Next
        Next
    End Sub

    Public Sub Resize(Source As clsHeightmap, OffsetY As Integer, OffsetX As Integer, SizeY As Integer, SizeX As Integer)
        Dim StartY As Integer
        Dim StartX As Integer
        Dim EndY As Integer
        Dim EndX As Integer
        Dim Y As Integer
        Dim X As Integer

        Blank(SizeY, SizeX)
        StartX = Math.Max(0 - OffsetX, 0)
        StartY = Math.Max(0 - OffsetY, 0)
        EndX = Math.Min(Source.HeightData.SizeX - OffsetX, HeightData.SizeX) - 1
        EndY = Math.Min(Source.HeightData.SizeY - OffsetY, HeightData.SizeY) - 1
        For Y = StartY To EndY
            For X = StartX To EndX
                HeightData.Height(Y, X) = Source.HeightData.Height(OffsetY + Y, OffsetX + X)
            Next
        Next
    End Sub

    Public Sub SizeCopy(Source As clsHeightmap)

        HeightData.SizeX = Source.HeightData.SizeX
        HeightData.SizeY = Source.HeightData.SizeY
        ReDim HeightData.Height(HeightData.SizeY - 1, HeightData.SizeX - 1)
    End Sub

    Public Sub Insert(Source As clsHeightmap, Y1 As Integer, X1 As Integer)
        Dim Y As Integer
        Dim X As Integer

        For Y = 0 To Source.HeightData.SizeY - 1
            For X = 0 To Source.HeightData.SizeX - 1
                HeightData.Height(Y1 + Y, X1 + X) = Source.HeightData.Height(Y, X)
            Next
        Next
    End Sub

    Public Function Load_Image(Path As String) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim HeightmapBitmap As Bitmap = Nothing
        Dim Result As sResult

        Result = LoadBitmap(Path, HeightmapBitmap)
        If Not Result.Success Then
            ReturnResult.Problem = Result.Problem
            Return ReturnResult
        End If

        Blank(HeightmapBitmap.Height, HeightmapBitmap.Width)
        Dim X As Integer
        Dim Y As Integer
        For Y = 0 To HeightmapBitmap.Height - 1
            For X = 0 To HeightmapBitmap.Width - 1
                With HeightmapBitmap.GetPixel(X, Y)
                    HeightData.Height(Y, X) = CLng((CShort(.R) + .G + .B) / (3.0# * HeightScale))
                End With
            Next
        Next

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Sub GenerateNewOfSize(Final_SizeY As Integer, Final_SizeX As Integer, Scale As Single, HeightMultiplier As Double)
        Dim Inflations As Integer
        Dim SizeY As Integer
        Dim SizeX As Integer
        Dim Log2 As Double
        Dim intTemp As Integer
        Dim hmTemp As New clsHeightmap
        Dim Ratio As Double

        Log2 = Math.Log(2.0#)
        If Final_SizeX > Final_SizeY Then
            Inflations = CInt(Math.Ceiling(Math.Log(Final_SizeX - 1) / Log2))
        Else
            Inflations = CInt(Math.Ceiling(Math.Log(Final_SizeY - 1) / Log2))
        End If
        Inflations = CInt(Math.Ceiling(Scale))
        If Inflations < 0 Then Stop
        Ratio = 2.0# ^ (Scale - Inflations)
        intTemp = CInt(2.0# ^ Inflations)
        SizeX = CInt(Math.Ceiling((Final_SizeX / Ratio - 1) / intTemp)) + 1
        SizeY = CInt(Math.Ceiling((Final_SizeY / Ratio - 1) / intTemp)) + 1

        GenerateNew(SizeY, SizeX, Inflations, 1.0#, HeightMultiplier)
        If Inflations > Scale Then
            hmTemp.Stretch(Me, CInt(HeightData.SizeX * Ratio), CInt(HeightData.SizeY * Ratio))
            HeightData = hmTemp.HeightData
            hmTemp.HeightData = New clsHeightData
        End If
        If HeightData.SizeX <> Final_SizeX Or HeightData.SizeY <> Final_SizeY Then
            'If HeightData.SizeX / Final_SizeX > HeightData.SizeY / Final_SizeY Then
            '    hmTemp.Resize(Me, 0, 0, HeightData.SizeY, Final_SizeX * HeightData.SizeY / Final_SizeY)
            'Else
            '    hmTemp.Resize(Me, 0, 0, Final_SizeY * HeightData.SizeX / Final_SizeX, HeightData.SizeX)
            'End If
            'StretchPixelated(hmTemp, Final_SizeX, Final_SizeY)
            hmTemp.Resize(Me, 0, 0, Final_SizeY, Final_SizeX)
            HeightData = hmTemp.HeightData
        End If
    End Sub

    Public Sub Stretch(hmSource As clsHeightmap, SizeX As Integer, SizeY As Integer)
        Dim OldSizeX As Integer
        Dim OldSizeY As Integer
        Dim New_Per_OldX As Single
        Dim New_Per_OldY As Single
        Dim OldPixStartX As Single
        Dim OldPixStartY As Single
        Dim OldPixEndX As Single
        Dim OldPixEndY As Single
        Dim Ratio As Single
        Dim NewPixelX As Integer
        Dim NewPixelY As Integer
        Dim OldPixelX As Integer
        Dim OldPixelY As Integer
        Dim XTemp As Single
        Dim YTemp As Single
        Dim Temp As Single = CSng(hmSource.HeightScale / HeightScale)

        OldSizeX = hmSource.HeightData.SizeX
        OldSizeY = hmSource.HeightData.SizeY

        Blank(SizeY, SizeX)
        'new ratios convert original image positions into new image positions
        New_Per_OldX = CSng(SizeX / OldSizeX)
        New_Per_OldY = CSng(SizeY / OldSizeY)
        'cycles through each pixel in the new image
        For OldPixelY = 0 To OldSizeY - 1
            For OldPixelX = 0 To OldSizeX - 1
                'find where the old pixel goes on the new image
                OldPixStartX = OldPixelX * New_Per_OldX
                OldPixStartY = OldPixelY * New_Per_OldY
                OldPixEndX = (OldPixelX + 1) * New_Per_OldX
                OldPixEndY = (OldPixelY + 1) * New_Per_OldY
                'cycles through each new image pixel that is to be influenced
                For NewPixelY = CInt(Int(OldPixStartY)) To CInt(-Int(-OldPixEndY))
                    If NewPixelY >= SizeY Then Exit For
                    For NewPixelX = CInt(Int(OldPixStartX)) To CInt(-Int(-OldPixEndX))
                        If NewPixelX >= SizeX Then Exit For
                        'ensure that the original pixel imposes on the new pixel
                        If Not (OldPixEndY > NewPixelY And OldPixStartY < NewPixelY + 1 And OldPixEndX > NewPixelX And OldPixStartX < NewPixelX + 1) Then
                            'Stop
                        Else
                            'measure the amount of original pixel in the new pixel
                            XTemp = 1.0#
                            YTemp = 1.0#
                            If OldPixStartY > NewPixelY Then YTemp -= OldPixStartY - NewPixelY
                            If OldPixStartX > NewPixelX Then XTemp -= OldPixStartX - NewPixelX
                            If OldPixEndY < NewPixelY + 1 Then YTemp -= (NewPixelY + 1) - OldPixEndY
                            If OldPixEndX < NewPixelX + 1 Then XTemp -= (NewPixelX + 1) - OldPixEndX
                            Ratio = XTemp * YTemp
                            'add the neccessary fraction of the original pixel's color into the new pixel
                            HeightData.Height(NewPixelY, NewPixelX) = CLng(HeightData.Height(NewPixelY, NewPixelX) + hmSource.HeightData.Height(OldPixelY, OldPixelX) * Ratio * Temp)
                        End If
                    Next
                Next
            Next
        Next
    End Sub

    Public Structure sHeights
        Public Heights() As Single
    End Structure

    Public Sub FadeMultiple(hmSource As clsHeightmap, ByRef AlterationMaps As sHeightmaps, ByRef AlterationHeights As sHeights)
        Dim Level As Integer
        Dim Y As Integer
        Dim X As Integer
        Dim srcHeight As Single
        Dim Ratio As Single
        Dim AlterationHeight_Ubound As Integer = AlterationHeights.Heights.GetUpperBound(0)
        Dim intTemp As Integer
        Dim TempA As Single
        Dim TempB As Single

        SizeCopy(hmSource)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                srcHeight = CSng(hmSource.HeightData.Height(Y, X) * hmSource.HeightScale)
                For Level = 0 To AlterationHeight_Ubound
                    If srcHeight <= AlterationHeights.Heights(Level) Then
                        Exit For
                    End If
                Next
                If Level = 0 Then
                    HeightData.Height(Y, X) = CLng(AlterationMaps.Heightmaps(Level).HeightData.Height(Y, X) * AlterationMaps.Heightmaps(Level).HeightScale / HeightScale)
                ElseIf Level > AlterationHeight_Ubound Then
                    HeightData.Height(Y, X) = CLng(AlterationMaps.Heightmaps(AlterationHeight_Ubound).HeightData.Height(Y, X) * AlterationMaps.Heightmaps(AlterationHeight_Ubound).HeightScale / HeightScale)
                Else
                    intTemp = Level - 1
                    TempA = CSng(AlterationHeights.Heights(intTemp))
                    TempB = CSng(AlterationHeights.Heights(Level))
                    Ratio = (srcHeight - TempA) / (TempB - TempA)
                    HeightData.Height(Y, X) = CLng((AlterationMaps.Heightmaps(intTemp).HeightData.Height(Y, X) * AlterationMaps.Heightmaps(intTemp).HeightScale * (1.0F - Ratio) + AlterationMaps.Heightmaps(Level).HeightData.Height(Y, X) * AlterationMaps.Heightmaps(Level).HeightScale * Ratio) / HeightScale)
                End If
            Next
        Next
    End Sub
End Class

Public Structure sHeightmaps
    Public Heightmaps() As clsHeightmap
End Structure