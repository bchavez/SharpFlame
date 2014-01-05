Imports ICSharpCode.SharpZipLib
Imports System.Globalization

Public Module modIO

    Public Function ZipMakeEntry(ZipOutputStream As Zip.ZipOutputStream, Path As String, Result As clsResult) As Zip.ZipEntry

        Try
            Dim NewZipEntry As New Zip.ZipEntry(Path)
            NewZipEntry.DateTime = Now
            ZipOutputStream.PutNextEntry(NewZipEntry)
            Return NewZipEntry
        Catch ex As Exception
            Result.ProblemAdd("Zip entry " & ControlChars.Quote & Path & ControlChars.Quote & " failed: " & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function InvariantToString_bool(Value As Boolean) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_byte(Value As Byte) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_short(Value As Short) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_int(Value As Integer) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_uint(Value As UInteger) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_sng(Value As Single) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_dbl(Value As Double) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantParse_bool(Text As String, ByRef Result As Boolean) As Boolean

        Return Boolean.TryParse(Text, Result)
    End Function

    Public Function InvariantParse_byte(Text As String, ByRef Result As Byte) As Boolean

        Return Byte.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_short(Text As String, ByRef Result As Short) As Boolean

        Return Short.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_ushort(Text As String, ByRef Result As UShort) As Boolean

        Return UShort.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_int(Text As String, ByRef Result As Integer) As Boolean

        Return Integer.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_uint(Text As String, ByRef Result As UInteger) As Boolean

        Return UInteger.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_sng(Text As String, ByRef Result As Single) As Boolean

        Return Single.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_dbl(Text As String, ByRef Result As Double) As Boolean

        Return Double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function ReadOldText(File As IO.BinaryReader) As String
        Dim Result As String = ""
        Dim A As Integer
        Dim Length As Integer = CInt(File.ReadUInt32)

        For A = 0 To Length - 1
            Result &= Chr(File.ReadByte)
        Next
        Return Result
    End Function

    Public Function ReadOldTextOfLength(File As IO.BinaryReader, Length As Integer) As String
        Dim Result As String = ""
        Dim A As Integer

        For A = 0 To Length - 1
            Result &= Chr(File.ReadByte)
        Next
        Return Result
    End Function

    Public Sub WriteText(File As IO.BinaryWriter, WriteLength As Boolean, Text As String)

        If WriteLength Then
            File.Write(CUInt(Text.Length))
        End If
        Dim A As Integer
        For A = 0 To Text.Length - 1
            File.Write(CByte(Asc(Text(A))))
        Next
    End Sub

    Public Sub WriteTextOfLength(File As IO.BinaryWriter, Length As Integer, Text As String)
        Dim A As Integer

        For A = 0 To Math.Min(Text.Length, Length) - 1
            File.Write(CByte(Asc(Text(A))))
        Next
        For A = Text.Length To Length - 1
            File.Write(CByte(0))
        Next
    End Sub

    Public Function WriteMemoryToNewFile(Memory As IO.MemoryStream, Path As String) As clsResult
        Dim ReturnResult As New clsResult("Writing to " & ControlChars.Quote & Path & ControlChars.Quote)

        Dim NewFile As IO.FileStream
        Try
            NewFile = New IO.FileStream(Path, IO.FileMode.CreateNew)
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try
        Try
            Memory.WriteTo(NewFile)
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        Memory.Close()
        NewFile.Close()

        Return ReturnResult
    End Function

    Public Function WriteMemoryToZipEntryAndFlush(Memory As IO.MemoryStream, Stream As Zip.ZipOutputStream) As clsResult
        Dim ReturnResult As New clsResult("Writing to zip stream")

        Try
            Memory.WriteTo(Stream)
            Memory.Flush()
            Stream.Flush()
            Stream.CloseEntry()
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        Return ReturnResult
    End Function

    Public Function TryOpenFileStream(Path As String, ByRef Output As IO.FileStream) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Try
            Output = New IO.FileStream(Path, IO.FileMode.Open)
        Catch ex As Exception
            Output = Nothing
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function WZAngleFromINIText(Text As String, ByRef Result As sWZAngle) As Boolean
        Dim VectorText As New clsSplitCommaText(Text)
        Dim WZAngle As sWZAngle

        If VectorText.PartCount <> 3 Then
            Return False
        End If

        If Not InvariantParse_ushort(VectorText.Parts(0), WZAngle.Direction) Then
            Dim ErrorValue As Integer
            If Not InvariantParse_int(VectorText.Parts(0), ErrorValue) Then
                Return False
            End If
            Dim Remainder As Integer
            Dim Multiplier As Integer = Math.DivRem(ErrorValue, INIRotationMax, Remainder)
            Try
                If Remainder < 0 Then
                    WZAngle.Direction = CUShort(Remainder + INIRotationMax)
                Else
                    WZAngle.Direction = CUShort(Remainder)
                End If
            Catch ex As Exception
                Return False
            End Try
            Return True
        End If
        If Not InvariantParse_ushort(VectorText.Parts(1), WZAngle.Pitch) Then
            Return False
        End If
        If Not InvariantParse_ushort(VectorText.Parts(2), WZAngle.Roll) Then
            Return False
        End If
        Result = WZAngle
        Return True
    End Function

    Public Function HealthFromINIText(Text As String, ByRef Result As Integer) As Boolean
        Dim A As Integer
        Dim Health As Integer

        A = Text.IndexOf("%"c)
        If A < 0 Then
            Return False
        End If
        Text = Text.Replace("%", "")
        If Not InvariantParse_int(Text, Health) Then
            Return False
        End If
        If Health < 0 Or Health > 100 Then
            Return False
        End If
        Result = Health
        Return True
    End Function

    Public Function WorldPosFromINIText(Text As String, ByRef Result As clsWorldPos) As Boolean
        Dim VectorText As New clsSplitCommaText(Text)
        Dim A As Integer
        Dim B As Integer

        If VectorText.PartCount <> 3 Then
            Return False
        End If
        Dim Positions(2) As Integer
        For A = 0 To 2
            If InvariantParse_int(VectorText.Parts(A), B) Then
                Positions(A) = B
            Else
                Return False
            End If
        Next
        Result = New clsWorldPos(New sWorldPos(New sXY_int(Positions(0), Positions(1)), Positions(2)))
        Return True
    End Function

    Public Function FindZipEntryFromPath(Path As String, ZipPathToFind As String) As clsZipStreamEntry
        Dim ZipStream As Zip.ZipInputStream
        Dim ZipEntry As Zip.ZipEntry
        Dim FindPath As String = ZipPathToFind.ToLower.Replace("\"c, "/"c)
        Dim ZipPath As String

        ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))
        Do
            Try
                ZipEntry = ZipStream.GetNextEntry
            Catch ex As Exception
                Exit Do
            End Try
            If ZipEntry Is Nothing Then
                Exit Do
            End If

            ZipPath = ZipEntry.Name.ToLower.Replace("\"c, "/"c)
            If ZipPath = FindPath Then
                Dim Result As New clsZipStreamEntry
                Result.Stream = ZipStream
                Result.Entry = ZipEntry
                Return Result
            End If
        Loop
        ZipStream.Close()

        Return Nothing
    End Function

    Public Function BytesToLinesRemoveComments(reader As IO.BinaryReader) As SimpleList(Of String)
        Dim CurrentChar As Char
        Dim CurrentCharExists As Boolean
        Dim InLineComment As Boolean
        Dim InCommentBlock As Boolean
        Dim PrevChar As Char
        Dim PrevCharExists As Boolean
        Dim Line As String = ""
        Dim Result As New SimpleList(Of String)

        Do
MonoContinueDo:
            PrevChar = CurrentChar
            PrevCharExists = CurrentCharExists
            Try
                CurrentChar = reader.ReadChar
                CurrentCharExists = True
            Catch ex As Exception
                CurrentCharExists = False
            End Try
            If CurrentCharExists Then
                Select Case CurrentChar
                    Case ControlChars.Cr, ControlChars.Lf
                        InLineComment = False
                        If PrevCharExists Then
                            Line &= PrevChar
                        End If
                        CurrentCharExists = False

                        If Line.Length > 0 Then
                            Result.Add(Line)
                            Line = ""
                        End If

                        GoTo MonoContinueDo
                    Case "*"c
                        If PrevCharExists And PrevChar = "/"c Then
                            InCommentBlock = True
                            CurrentCharExists = False
                            GoTo MonoContinueDo
                        End If
                    Case "/"c
                        If PrevCharExists Then
                            If PrevChar = "/"c Then
                                InLineComment = True
                                CurrentCharExists = False
                                GoTo MonoContinueDo
                            ElseIf PrevChar = "*"c Then
                                InCommentBlock = False
                                CurrentCharExists = False
                                GoTo MonoContinueDo
                            End If
                        End If
                End Select
            Else
                InLineComment = False
                If PrevCharExists Then
                    Line &= PrevChar
                End If
                If Line.Length > 0 Then
                    Result.Add(Line)
                    Line = ""
                End If

                Exit Do
            End If
            If PrevCharExists Then
                If Not (InCommentBlock Or InLineComment) Then
                    Line &= PrevChar
                End If
            End If
        Loop

        Return Result
    End Function
End Module

Public Class clsPositionFromText

    Public Pos As sXY_int

    Public Function Translate(Text As String) As Boolean
        Dim A As Integer
        Dim Positions As New clsSplitCommaText(Text)

        If Positions.PartCount < 2 Then
            Return False
        End If
        If InvariantParse_int(Positions.Parts(0), A) Then
            Pos.X = A
        Else
            Return False
        End If
        If InvariantParse_int(Positions.Parts(1), A) Then
            Pos.Y = A
        Else
            Return False
        End If
        Return True
    End Function
End Class

Public Class clsZipStreamEntry
    Public Stream As Zip.ZipInputStream
    Public Entry As Zip.ZipEntry
End Class

Public Class clsSplitCommaText

    Public Parts() As String
    Public PartCount As Integer

    Public Sub New(Text As String)
        Dim A As Integer

        Parts = Text.Split(","c)
        PartCount = Parts.GetUpperBound(0) + 1
        For A = 0 To PartCount - 1
            Parts(A) = Parts(A).Trim()
        Next
    End Sub
End Class
