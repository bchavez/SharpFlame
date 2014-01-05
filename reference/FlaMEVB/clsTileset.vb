Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class clsTileset

    Public Name As String

    Public IsOriginal As Boolean

    Public Structure sTile
        Public MapView_GL_Texture_Num As Integer
        Public TextureView_GL_Texture_Num As Integer
        Public AverageColour As sRGB_sng
        Public Default_Type As Byte
    End Structure
    Public Tiles() As sTile
    Public TileCount As Integer

    Public BGColour As New sRGB_sng(0.5F, 0.5F, 0.5F)

    Public Function Default_TileTypes_Load(Path As String) As sResult
        Dim ReturnResult As sResult
        Dim File As IO.BinaryReader

        Try
            File = New IO.BinaryReader(New IO.FileStream(Path, IO.FileMode.Open))
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try
        ReturnResult = Default_TileTypes_Read(File)
        File.Close()
        Return ReturnResult
    End Function

    Private Function Default_TileTypes_Read(File As IO.BinaryReader) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim uintTemp As UInteger
        Dim A As Integer
        Dim ushortTemp As UShort
        Dim strTemp As String = ""

        Try
            strTemp = ReadOldTextOfLength(File, 4)
            If strTemp <> "ttyp" Then
                ReturnResult.Problem = "Bad identifier."
                Return ReturnResult
            End If

            uintTemp = File.ReadUInt32
            If Not uintTemp = 8UI Then
                ReturnResult.Problem = "Unknown version."
                Return ReturnResult
            End If

            uintTemp = File.ReadUInt32
            TileCount = CInt(uintTemp)
            ReDim Tiles(TileCount - 1)

            For A = 0 To Math.Min(CInt(uintTemp), TileCount) - 1
                ushortTemp = File.ReadUInt16
                If ushortTemp > TileTypes.Count Then
                    ReturnResult.Problem = "Unknown tile type."
                    Return ReturnResult
                End If
                Tiles(A).Default_Type = CByte(ushortTemp)
            Next
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function LoadDirectory(Path As String) As clsResult
        Dim ReturnResult As New clsResult("Loading tileset from " & ControlChars.Quote & Path & ControlChars.Quote)

        Dim Bitmap As Bitmap = Nothing
        Dim SplitPath As New sSplitPath(Path)
        Dim SlashPath As String = EndWithPathSeperator(Path)
        Dim Result As sResult

        If SplitPath.FileTitle <> "" Then
            Name = SplitPath.FileTitle
        ElseIf SplitPath.PartCount >= 2 Then
            Name = SplitPath.Parts(SplitPath.PartCount - 2)
        End If

        Result = Default_TileTypes_Load(SlashPath & Name & ".ttp")
        If Not Result.Success Then
            ReturnResult.ProblemAdd("Loading tile types: " & Result.Problem)
            Return ReturnResult
        End If

        Dim RedTotal As Integer
        Dim GreenTotal As Integer
        Dim BlueTotal As Integer
        Dim TileNum As Integer
        Dim strTile As String
        Dim BitmapTextureArgs As sBitmapGLTexture
        Dim AverageColour(3) As Single
        Dim X As Integer
        Dim Y As Integer
        Dim Pixel As Color

        Dim GraphicPath As String

        'tile count has been set by the ttp file

        For TileNum = 0 To TileCount - 1
            strTile = "tile-" & MinDigits(TileNum, 2) & ".png"

            '-------- 128 --------

            GraphicPath = SlashPath & Name & "-128" & PlatformPathSeparator & strTile

            Result = LoadBitmap(GraphicPath, Bitmap)
            If Not Result.Success Then
                'ignore and exit, since not all tile types have a corresponding tile graphic
                Return ReturnResult
            End If

            If Bitmap.Width <> 128 Or Bitmap.Height <> 128 Then
                ReturnResult.WarningAdd("Tile graphic " & GraphicPath & " from tileset " & Name & " is not 128x128.")
                Return ReturnResult
            End If

            BitmapTextureArgs.Texture = Bitmap
            BitmapTextureArgs.MipMapLevel = 0
            BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest
            BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
            BitmapTextureArgs.TextureNum = 0
            BitmapTextureArgs.Perform()
            Tiles(TileNum).TextureView_GL_Texture_Num = BitmapTextureArgs.TextureNum

            BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest
            If Settings.Mipmaps Then
                BitmapTextureArgs.MinFilter = TextureMinFilter.LinearMipmapLinear
            Else
                BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
            End If
            BitmapTextureArgs.TextureNum = 0

            BitmapTextureArgs.Perform()
            Tiles(TileNum).MapView_GL_Texture_Num = BitmapTextureArgs.TextureNum

            If Settings.Mipmaps Then
                If Settings.MipmapsHardware Then
                    GL.Enable(EnableCap.Texture2D)
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D)
                    GL.Disable(EnableCap.Texture2D)
                Else
                    Dim MipmapResult As clsResult
                    MipmapResult = GenerateMipMaps(SlashPath, strTile, BitmapTextureArgs, TileNum)
                    ReturnResult.Add(MipmapResult)
                    If MipmapResult.HasProblems Then
                        Return ReturnResult
                    End If
                End If
                GL.GetTexImage(Of Single)(TextureTarget.Texture2D, 7, PixelFormat.Rgba, PixelType.Float, AverageColour)
                Tiles(TileNum).AverageColour.Red = AverageColour(0)
                Tiles(TileNum).AverageColour.Green = AverageColour(1)
                Tiles(TileNum).AverageColour.Blue = AverageColour(2)
            Else
                RedTotal = 0
                GreenTotal = 0
                BlueTotal = 0
                For Y = 0 To Bitmap.Height - 1
                    For X = 0 To Bitmap.Width - 1
                        Pixel = Bitmap.GetPixel(X, Y)
                        RedTotal += Pixel.R
                        GreenTotal += Pixel.G
                        BlueTotal += Pixel.B
                    Next
                Next
                Tiles(TileNum).AverageColour.Red = CSng(RedTotal / 4177920.0#)
                Tiles(TileNum).AverageColour.Green = CSng(GreenTotal / 4177920.0#)
                Tiles(TileNum).AverageColour.Blue = CSng(BlueTotal / 4177920.0#)
            End If

        Next

        Return ReturnResult
    End Function

    Public Function GenerateMipMaps(SlashPath As String, strTile As String, BitmapTextureArgs As sBitmapGLTexture, TileNum As Integer) As clsResult
        Dim ReturnResult As New clsResult("Generating mipmaps")
        Dim GraphicPath As String
        Dim PixX As Integer
        Dim PixY As Integer
        Dim PixelColorA As Color
        Dim PixelColorB As Color
        Dim PixelColorC As Color
        Dim PixelColorD As Color
        Dim X1 As Integer
        Dim Y1 As Integer
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim Red As Integer
        Dim Green As Integer
        Dim Blue As Integer
        Dim Bitmap8 As Bitmap
        Dim Bitmap4 As Bitmap
        Dim Bitmap2 As Bitmap
        Dim Bitmap1 As Bitmap
        Dim Bitmap As Bitmap = Nothing
        Dim Result As sResult

        '-------- 64 --------

        GraphicPath = SlashPath & Name & "-64" & PlatformPathSeparator & strTile

        Result = LoadBitmap(GraphicPath, Bitmap)
        If Not Result.Success Then
            ReturnResult.WarningAdd("Unable to load tile graphic: " & Result.Problem)
            Return ReturnResult
        End If

        If Bitmap.Width <> 64 Or Bitmap.Height <> 64 Then
            ReturnResult.WarningAdd("Tile graphic " & GraphicPath & " from tileset " & Name & " is not 64x64.")
            Return ReturnResult
        End If

        BitmapTextureArgs.Texture = Bitmap
        BitmapTextureArgs.MipMapLevel = 1
        BitmapTextureArgs.Perform()

        '-------- 32 --------

        GraphicPath = SlashPath & Name & "-32" & PlatformPathSeparator & strTile

        Result = LoadBitmap(GraphicPath, Bitmap)
        If Not Result.Success Then
            ReturnResult.WarningAdd("Unable to load tile graphic: " & Result.Problem)
            Return ReturnResult
        End If

        If Bitmap.Width <> 32 Or Bitmap.Height <> 32 Then
            ReturnResult.WarningAdd("Tile graphic " & GraphicPath & " from tileset " & Name & " is not 32x32.")
            Return ReturnResult
        End If

        BitmapTextureArgs.Texture = Bitmap
        BitmapTextureArgs.MipMapLevel = 2
        BitmapTextureArgs.Perform()

        '-------- 16 --------

        GraphicPath = SlashPath & Name & "-16" & PlatformPathSeparator & strTile

        Result = LoadBitmap(GraphicPath, Bitmap)
        If Not Result.Success Then
            ReturnResult.WarningAdd("Unable to load tile graphic: " & Result.Problem)
            Return ReturnResult
        End If

        If Bitmap.Width <> 16 Or Bitmap.Height <> 16 Then
            ReturnResult.WarningAdd("Tile graphic " & GraphicPath & " from tileset " & Name & " is not 16x16.")
            Return ReturnResult
        End If

        BitmapTextureArgs.Texture = Bitmap
        BitmapTextureArgs.MipMapLevel = 3
        BitmapTextureArgs.Perform()

        '-------- 8 --------

        Bitmap8 = New Bitmap(8, 8, Imaging.PixelFormat.Format32bppArgb)
        For PixY = 0 To 7
            Y1 = PixY * 2
            Y2 = Y1 + 1
            For PixX = 0 To 7
                X1 = PixX * 2
                X2 = X1 + 1
                PixelColorA = Bitmap.GetPixel(X1, Y1)
                PixelColorB = Bitmap.GetPixel(X2, Y1)
                PixelColorC = Bitmap.GetPixel(X1, Y2)
                PixelColorD = Bitmap.GetPixel(X2, Y2)
                Red = CInt((CInt(PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F)
                Green = CInt((CInt(PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F)
                Blue = CInt((CInt(PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F)
                Bitmap8.SetPixel(PixX, PixY, ColorTranslator.FromOle(OSRGB(Red, Green, Blue)))
            Next
        Next

        BitmapTextureArgs.Texture = Bitmap8
        BitmapTextureArgs.MipMapLevel = 4
        BitmapTextureArgs.Perform()

        '-------- 4 --------

        Bitmap = Bitmap8
        Bitmap4 = New Bitmap(4, 4, Imaging.PixelFormat.Format32bppArgb)
        For PixY = 0 To 3
            Y1 = PixY * 2
            Y2 = Y1 + 1
            For PixX = 0 To 3
                X1 = PixX * 2
                X2 = X1 + 1
                PixelColorA = Bitmap.GetPixel(X1, Y1)
                PixelColorB = Bitmap.GetPixel(X2, Y1)
                PixelColorC = Bitmap.GetPixel(X1, Y2)
                PixelColorD = Bitmap.GetPixel(X2, Y2)
                Red = CInt((CInt(PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F)
                Green = CInt((CInt(PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F)
                Blue = CInt((CInt(PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F)
                Bitmap4.SetPixel(PixX, PixY, ColorTranslator.FromOle(OSRGB(Red, Green, Blue)))
            Next
        Next

        BitmapTextureArgs.Texture = Bitmap4
        BitmapTextureArgs.MipMapLevel = 5
        BitmapTextureArgs.Perform()

        '-------- 2 --------

        Bitmap = Bitmap4
        Bitmap2 = New Bitmap(2, 2, Imaging.PixelFormat.Format32bppArgb)
        For PixY = 0 To 1
            Y1 = PixY * 2
            Y2 = Y1 + 1
            For PixX = 0 To 1
                X1 = PixX * 2
                X2 = X1 + 1
                PixelColorA = Bitmap.GetPixel(X1, Y1)
                PixelColorB = Bitmap.GetPixel(X2, Y1)
                PixelColorC = Bitmap.GetPixel(X1, Y2)
                PixelColorD = Bitmap.GetPixel(X2, Y2)
                Red = CInt((CInt(PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F)
                Green = CInt((CInt(PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F)
                Blue = CInt((CInt(PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F)
                Bitmap2.SetPixel(PixX, PixY, ColorTranslator.FromOle(OSRGB(Red, Green, Blue)))
            Next
        Next

        BitmapTextureArgs.Texture = Bitmap2
        BitmapTextureArgs.MipMapLevel = 6
        BitmapTextureArgs.Perform()

        '-------- 1 --------

        Bitmap = Bitmap2
        Bitmap1 = New Bitmap(1, 1, Imaging.PixelFormat.Format32bppArgb)
        PixX = 0
        PixY = 0
        Y1 = PixY * 2
        Y2 = Y1 + 1
        X1 = PixX * 2
        X2 = X1 + 1
        PixelColorA = Bitmap.GetPixel(X1, Y1)
        PixelColorB = Bitmap.GetPixel(X2, Y1)
        PixelColorC = Bitmap.GetPixel(X1, Y2)
        PixelColorD = Bitmap.GetPixel(X2, Y2)
        Red = CInt((CInt(PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F)
        Green = CInt((CInt(PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F)
        Blue = CInt((CInt(PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F)
        Bitmap1.SetPixel(PixX, PixY, ColorTranslator.FromOle(OSRGB(Red, Green, Blue)))

        BitmapTextureArgs.Texture = Bitmap1
        BitmapTextureArgs.MipMapLevel = 7
        BitmapTextureArgs.Perform()

        Return ReturnResult
    End Function
End Class
