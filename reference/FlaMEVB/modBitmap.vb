Imports OpenTK.Graphics.OpenGL

Public Module modBitmap

    Public Function LoadBitmap(Path As String, ByRef ResultBitmap As Bitmap) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Problem = ""
        ReturnResult.Success = False

        Dim Bitmap As Bitmap

        Try
            Bitmap = New Bitmap(Path)
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            ResultBitmap = Nothing
            Return ReturnResult
        End Try

        ResultBitmap = New Bitmap(Bitmap) 'copying the bitmap is needed so it doesn't lock access to the file

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function SaveBitmap(Path As String, Overwrite As Boolean, BitmapToSave As Bitmap) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Problem = ""
        ReturnResult.Success = False

        Try

            If IO.File.Exists(Path) Then
                If Overwrite Then
                    IO.File.Delete(Path)
                Else
                    ReturnResult.Problem = "File already exists."
                    Return ReturnResult
                End If
            End If
            BitmapToSave.Save(Path)

        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Structure sBitmapGLTexture
        Public Texture As Bitmap
        Public TextureNum As Integer
        Public MipMapLevel As Integer
        Public MinFilter As TextureMinFilter
        Public MagFilter As TextureMagFilter

        Public Sub Perform()

            Dim BitmapData As Drawing.Imaging.BitmapData = Texture.LockBits(New Rectangle(0, 0, Texture.Width, Texture.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)

            If MipMapLevel = 0 Then
                GL.GenTextures(1, TextureNum)
            End If
            GL.BindTexture(TextureTarget.Texture2D, TextureNum)
            If MipMapLevel = 0 Then
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, MagFilter)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, MinFilter)
            End If
            GL.TexImage2D(TextureTarget.Texture2D, MipMapLevel, PixelInternalFormat.Rgba8, Texture.Width, Texture.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, BitmapData.Scan0)

            Texture.UnlockBits(BitmapData)
        End Sub
    End Structure

    Public Function BitmapIsGLCompatible(BitmapToCheck As Bitmap) As clsResult
        Dim ReturnResult As New clsResult("Compatability check")

        If Not SizeIsPowerOf2(BitmapToCheck.Width) Then
            ReturnResult.WarningAdd("Image width is not a power of 2.")
        End If
        If Not SizeIsPowerOf2(BitmapToCheck.Height) Then
            ReturnResult.WarningAdd("Image height is not a power of 2.")
        End If
        If BitmapToCheck.Width <> BitmapToCheck.Height Then
            ReturnResult.WarningAdd("Image is not square.")
        End If

        Return ReturnResult
    End Function
End Module
