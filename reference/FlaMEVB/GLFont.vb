Imports OpenTK.Graphics.OpenGL

Public Class GLFont

    Public BaseFont As Font
    Public Structure sCharacter
        Public GLTexture As Integer
        Public TexSize As Integer
        Public Width As Integer
    End Structure
    Public Character(255) As sCharacter
    Public Height As Integer

    Public Sub New(BaseFont As Font)

        GLTextures_Generate(BaseFont)
    End Sub

    Private Sub GLTextures_Generate(NewBaseFont As Font)
        Dim A As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim TempBitmap As Bitmap
        Dim gfx As Graphics
        Dim BitmapData As Drawing.Imaging.BitmapData
        Dim NewSizeX As Integer
        Dim StartX As Integer
        Dim FinishX As Integer
        Dim TexBitmap As Bitmap
        Dim Text As String

        BaseFont = NewBaseFont
        Height = BaseFont.Height
        For A = 0 To 255
            Text = ChrW(A)
            TempBitmap = New Bitmap(Height * 2, Height, Imaging.PixelFormat.Format32bppArgb)
            gfx = Graphics.FromImage(TempBitmap)
            gfx.Clear(Color.Transparent)
            gfx.DrawString(Text, BaseFont, Brushes.White, 0.0F, 0.0F)
            gfx.Dispose()
            For X = 0 To TempBitmap.Width - 1
                For Y = 0 To TempBitmap.Height - 1
                    If TempBitmap.GetPixel(X, Y).A > 0 Then
                        Exit For
                    End If
                Next
                If Y < TempBitmap.Height Then Exit For
            Next
            StartX = X
            For X = TempBitmap.Width - 1 To 0 Step -1
                For Y = 0 To TempBitmap.Height - 1
                    If TempBitmap.GetPixel(X, Y).A > 0 Then
                        Exit For
                    End If
                Next
                If Y < TempBitmap.Height Then Exit For
            Next
            FinishX = X
            NewSizeX = FinishX - StartX + 1
            If NewSizeX <= 0 Then
                NewSizeX = Math.Max(CInt(Math.Round(Height / 4.0F)), 1)
                Character(A).TexSize = CInt(Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(NewSizeX, TempBitmap.Height)) / Math.Log(2.0#))))
                TexBitmap = New Bitmap(Character(A).TexSize, Character(A).TexSize, Imaging.PixelFormat.Format32bppArgb)
                gfx = Graphics.FromImage(TexBitmap)
                gfx.Clear(Color.Transparent)
                gfx.Dispose()
                BitmapData = TexBitmap.LockBits(New Rectangle(0, 0, TexBitmap.Width, TexBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                GL.GenTextures(1, Character(A).GLTexture)
                GL.BindTexture(TextureTarget.Texture2D, Character(A).GLTexture)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TexBitmap.Width, TexBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, BitmapData.Scan0)
                TexBitmap.UnlockBits(BitmapData)
                Character(A).Width = NewSizeX
            Else
                Character(A).TexSize = CInt(Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(NewSizeX, TempBitmap.Height)) / Math.Log(2.0#))))
                TexBitmap = New Bitmap(Character(A).TexSize, Character(A).TexSize, Imaging.PixelFormat.Format32bppArgb)
                gfx = Graphics.FromImage(TexBitmap)
                gfx.Clear(Color.Transparent)
                gfx.Dispose()
                For Y = 0 To TempBitmap.Height - 1
                    For X = StartX To FinishX
                        TexBitmap.SetPixel(X - StartX, Y, TempBitmap.GetPixel(X, Y))
                    Next
                Next
                BitmapData = TexBitmap.LockBits(New Rectangle(0, 0, TexBitmap.Width, TexBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                GL.GenTextures(1, Character(A).GLTexture)
                GL.BindTexture(TextureTarget.Texture2D, Character(A).GLTexture)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TexBitmap.Width, TexBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, BitmapData.Scan0)
                TexBitmap.UnlockBits(BitmapData)
                Character(A).Width = NewSizeX
            End If
            TempBitmap.Dispose()
            TexBitmap.Dispose()
        Next
    End Sub

    Public Sub Deallocate()
        Dim A As Integer

        For A = 0 To 255
            GL.DeleteTexture(Character(A).GLTexture)
        Next
    End Sub
End Class

Public Class clsTextLabel
    Public Text As String
    Public TextFont As GLFont
    Public SizeY As Single
    Public Colour As sRGBA_sng
    Public Pos As sXY_int

    Public Function GetSizeX() As Single
        Dim SizeX As Single
        Dim CharWidth As Single
        Dim CharSpacing As Single = SizeY / 10.0F
        Dim CharSize As Single = SizeY / TextFont.Height
        Dim A As Integer

        For A = 0 To Text.Length - 1
            CharWidth = TextFont.Character(Asc(Text.Chars(A))).Width * CharSize
            SizeX += CharWidth
        Next
        SizeX += CharSpacing * (Text.Length - 1)

        Return SizeX
    End Function

    Public Sub Draw()

        If Text Is Nothing Then
            Exit Sub
        End If
        If Text.Length = 0 Then
            Exit Sub
        End If
        If TextFont Is Nothing Then
            Exit Sub
        End If

        Dim CharCode As Integer
        Dim CharWidth As Single
        Dim TexRatio As sXY_sng
        Dim LetterPosA As Single
        Dim LetterPosB As Single
        Dim PosY1 As Single
        Dim PosY2 As Single
        Dim CharSpacing As Single
        Dim A As Integer

        GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha)
        PosY1 = Pos.Y
        PosY2 = Pos.Y + SizeY
        CharSpacing = SizeY / 10.0F
        LetterPosA = Pos.X
        For A = 0 To Text.Length - 1
            CharCode = AscW(Text(A))
            If CharCode >= 0 And CharCode <= 255 Then
                CharWidth = SizeY * TextFont.Character(CharCode).Width / TextFont.Height
                TexRatio.X = CSng(TextFont.Character(CharCode).Width / TextFont.Character(CharCode).TexSize)
                TexRatio.Y = CSng(TextFont.Height / TextFont.Character(CharCode).TexSize)
                LetterPosB = LetterPosA + CharWidth
                GL.BindTexture(TextureTarget.Texture2D, TextFont.Character(CharCode).GLTexture)
                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)
                GL.Begin(BeginMode.Quads)
                GL.TexCoord2(0.0F, 0.0F)
                GL.Vertex2(LetterPosA, PosY1)
                GL.TexCoord2(0.0F, TexRatio.Y)
                GL.Vertex2(LetterPosA, PosY2)
                GL.TexCoord2(TexRatio.X, TexRatio.Y)
                GL.Vertex2(LetterPosB, PosY2)
                GL.TexCoord2(TexRatio.X, 0.0F)
                GL.Vertex2(LetterPosB, PosY1)
                GL.End()
                LetterPosA = LetterPosB + CharSpacing
            End If
        Next
    End Sub
End Class

Public Class clsTextLabels
    Public Items() As clsTextLabel
    Public ItemCount As Integer = 0
    Public MaxCount As Integer

    Public Sub New(MaxItemCount As Integer)

        MaxCount = MaxItemCount
        ReDim Items(MaxCount - 1)
    End Sub

    Public Function AtMaxCount() As Boolean

        Return (ItemCount >= MaxCount)
    End Function

    Public Sub Add(NewItem As clsTextLabel)

        If ItemCount = MaxCount Then
            Stop
            Exit Sub
        End If

        Items(ItemCount) = NewItem
        ItemCount += 1
    End Sub

    Public Sub Draw()
        Dim A As Integer

        For A = 0 To ItemCount - 1
            Items(A).Draw()
        Next
    End Sub
End Class
