#region

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

#endregion

namespace SharpFlame.Bitmaps
{
    public struct BitmapGLTexture
    {
        public TextureMagFilter MagFilter;
        public TextureMinFilter MinFilter;
        public int MipMapLevel;
        public Bitmap Texture;
        public int TextureNum;

        public void Perform()
        {
            var BitmapData = Texture.LockBits(new Rectangle(0, 0, Texture.Width, Texture.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            if ( MipMapLevel == 0 )
            {
                GL.GenTextures(1, out TextureNum);
            }
            GL.BindTexture(TextureTarget.Texture2D, TextureNum);

            GL.TexImage2D(TextureTarget.Texture2D, MipMapLevel, PixelInternalFormat.Rgba, BitmapData.Width, BitmapData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, BitmapData.Scan0);

            if ( MipMapLevel == 0 )
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);
            }

            Texture.UnlockBits(BitmapData);
        }
    }
}