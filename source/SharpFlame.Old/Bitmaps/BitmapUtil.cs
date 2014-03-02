#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NLog;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using SharpFlame.Old.Util;

#endregion

namespace SharpFlame.Old.Bitmaps
{
    public sealed class BitmapUtil
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static SimpleResult LoadBitmap(string path, ref Bitmap resultBitmap)
        {
            var returnResult = new SimpleResult();
            returnResult.Problem = "";
            returnResult.Success = false;

            if (!File.Exists(path)) {
                returnResult.Problem = string.Format("File \"{0}\" does not exists.", path);
                resultBitmap = null;
                return returnResult;
            }

            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(path);
            }
            catch ( Exception ex )
            {
                returnResult.Problem = ex.Message;
                resultBitmap = null;
                return returnResult;
            }

            resultBitmap = new Bitmap(bitmap); //copying the bitmap is needed so it doesn't lock access to the file

            returnResult.Success = true;
            return returnResult;
        }

        public static SimpleResult SaveBitmap(string path, bool overwrite, Bitmap bitmapToSave)
        {
            var returnResult = new SimpleResult();
            returnResult.Problem = "";
            returnResult.Success = false;

            try
            {
                if ( File.Exists(path) )
                {
                    if ( overwrite )
                    {
                        File.Delete(path);
                    }
                    else
                    {
                        returnResult.Problem = "File already exists.";
                        return returnResult;
                    }
                }
                bitmapToSave.Save(path);
            }
            catch ( Exception ex )
            {
                returnResult.Problem = ex.Message;
                return returnResult;
            }

            returnResult.Success = true;
            return returnResult;
        }

        public static Result BitmapIsGlCompatible(Bitmap bitmapToCheck)
        {
            var returnResult = new Result("Compatibility check", false);
            logger.Debug("Compatibility check");

            if ( !App.SizeIsPowerOf2(bitmapToCheck.Width) )
            {
                returnResult.WarningAdd("Image width is not a power of 2.");
            }
            if ( !App.SizeIsPowerOf2(bitmapToCheck.Height) )
            {
                returnResult.WarningAdd("Image height is not a power of 2.");
            }
            if ( bitmapToCheck.Width != bitmapToCheck.Height )
            {
                returnResult.WarningAdd("Image is not square.");
            }

            return returnResult;
        }

        public static int CreateGLTexture(Bitmap texture, int mipMapLevel, int inTextureNum = 0,
                                 TextureMagFilter magFilter = TextureMagFilter.Nearest, 
                                 TextureMinFilter minFilter = TextureMinFilter.Nearest) 
        {
            var bitmapData = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int textureNum;
            if (mipMapLevel == 0)
            {
                GL.GenTextures (1, out textureNum);
            } else
            {
                textureNum = inTextureNum;
            }

            GL.BindTexture(TextureTarget.Texture2D, textureNum);

            GL.TexImage2D(TextureTarget.Texture2D, mipMapLevel, PixelInternalFormat.Rgba, bitmapData.Width, bitmapData.Height, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            if ( mipMapLevel == 0 )
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            }

            texture.UnlockBits(bitmapData);

            return textureNum;
        }
    }
}