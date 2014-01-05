namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    [StandardModule]
    public sealed class modBitmap
    {
        public static clsResult BitmapIsGLCompatible(System.Drawing.Bitmap BitmapToCheck)
        {
            clsResult result2 = new clsResult("Compatability check");
            if (!modProgram.SizeIsPowerOf2(BitmapToCheck.Width))
            {
                result2.WarningAdd("Image width is not a power of 2.");
            }
            if (!modProgram.SizeIsPowerOf2(BitmapToCheck.Height))
            {
                result2.WarningAdd("Image height is not a power of 2.");
            }
            if (BitmapToCheck.Width != BitmapToCheck.Height)
            {
                result2.WarningAdd("Image is not square.");
            }
            return result2;
        }

        public static modProgram.sResult LoadBitmap(string Path, ref System.Drawing.Bitmap ResultBitmap)
        {
            System.Drawing.Bitmap bitmap;
            modProgram.sResult result2;
            result2.Problem = "";
            result2.Success = false;
            try
            {
                bitmap = new System.Drawing.Bitmap(Path);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                ResultBitmap = null;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            ResultBitmap = new System.Drawing.Bitmap(bitmap);
            result2.Success = true;
            return result2;
        }

        public static modProgram.sResult SaveBitmap(string Path, bool Overwrite, System.Drawing.Bitmap BitmapToSave)
        {
            modProgram.sResult result;
            result.Problem = "";
            result.Success = false;
            try
            {
                if (File.Exists(Path))
                {
                    if (!Overwrite)
                    {
                        result.Problem = "File already exists.";
                        return result;
                    }
                    File.Delete(Path);
                }
                BitmapToSave.Save(Path);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.Problem = exception.Message;
                modProgram.sResult result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            result.Success = true;
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sBitmapGLTexture
        {
            public System.Drawing.Bitmap Texture;
            public int TextureNum;
            public int MipMapLevel;
            public TextureMinFilter MinFilter;
            public TextureMagFilter MagFilter;
            public void Perform()
            {
                Rectangle rect = new Rectangle(0, 0, this.Texture.Width, this.Texture.Height);
                BitmapData bitmapdata = this.Texture.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                if (this.MipMapLevel == 0)
                {
                    GL.GenTextures(1, out this.TextureNum);
                }
                GL.BindTexture(TextureTarget.Texture2D, this.TextureNum);
                if (this.MipMapLevel == 0)
                {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, 0x812f);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, 0x812f);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) this.MagFilter);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) this.MinFilter);
                }
                GL.TexImage2D(TextureTarget.Texture2D, this.MipMapLevel, PixelInternalFormat.Rgba8, this.Texture.Width, this.Texture.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapdata.Scan0);
                this.Texture.UnlockBits(bitmapdata);
            }
        }
    }
}

