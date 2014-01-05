namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public class GLFont
    {
        public Font BaseFont;
        public sCharacter[] Character = new sCharacter[0x100];
        public int Height;

        public GLFont(Font BaseFont)
        {
            this.GLTextures_Generate(BaseFont);
        }

        public void Deallocate()
        {
            int index = 0;
            do
            {
                GL.DeleteTexture(this.Character[index].GLTexture);
                index++;
            }
            while (index <= 0xff);
        }

        private void GLTextures_Generate(Font NewBaseFont)
        {
            this.BaseFont = NewBaseFont;
            this.Height = this.BaseFont.Height;
            int charCode = 0;
            do
            {
                BitmapData data;
                System.Drawing.Bitmap bitmap2;
                int num6;
                Rectangle rectangle;
                string s = Conversions.ToString(Strings.ChrW(charCode));
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(this.Height * 2, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics graphics = Graphics.FromImage(image);
                graphics.Clear(Color.Transparent);
                graphics.DrawString(s, this.BaseFont, Brushes.White, (float) 0f, (float) 0f);
                graphics.Dispose();
                int num7 = image.Width - 1;
                int x = 0;
                while (x <= num7)
                {
                    int num8 = image.Height - 1;
                    num6 = 0;
                    while (num6 <= num8)
                    {
                        if (image.GetPixel(x, num6).A > 0)
                        {
                            break;
                        }
                        num6++;
                    }
                    if (num6 < image.Height)
                    {
                        break;
                    }
                    x++;
                }
                int num4 = x;
                x = image.Width - 1;
                while (x >= 0)
                {
                    int num9 = image.Height - 1;
                    num6 = 0;
                    while (num6 <= num9)
                    {
                        if (image.GetPixel(x, num6).A > 0)
                        {
                            break;
                        }
                        num6++;
                    }
                    if (num6 < image.Height)
                    {
                        break;
                    }
                    x += -1;
                }
                int num2 = x;
                int num3 = (num2 - num4) + 1;
                if (num3 <= 0)
                {
                    num3 = Math.Max((int) Math.Round(Math.Round((double) (((float) this.Height) / 4f))), 1);
                    this.Character[charCode].TexSize = (int) Math.Round(Math.Round(Math.Pow(2.0, Math.Ceiling((double) (Math.Log((double) Math.Max(num3, image.Height)) / Math.Log(2.0))))));
                    bitmap2 = new System.Drawing.Bitmap(this.Character[charCode].TexSize, this.Character[charCode].TexSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    graphics = Graphics.FromImage(bitmap2);
                    graphics.Clear(Color.Transparent);
                    graphics.Dispose();
                    rectangle = new Rectangle(0, 0, bitmap2.Width, bitmap2.Height);
                    data = bitmap2.LockBits(rectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.GenTextures(1, out this.Character[charCode].GLTexture);
                    GL.BindTexture(TextureTarget.Texture2D, this.Character[charCode].GLTexture);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 0x2601);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 0x2601);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap2.Width, bitmap2.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    bitmap2.UnlockBits(data);
                    this.Character[charCode].Width = num3;
                }
                else
                {
                    this.Character[charCode].TexSize = (int) Math.Round(Math.Round(Math.Pow(2.0, Math.Ceiling((double) (Math.Log((double) Math.Max(num3, image.Height)) / Math.Log(2.0))))));
                    bitmap2 = new System.Drawing.Bitmap(this.Character[charCode].TexSize, this.Character[charCode].TexSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    graphics = Graphics.FromImage(bitmap2);
                    graphics.Clear(Color.Transparent);
                    graphics.Dispose();
                    int num10 = image.Height - 1;
                    for (num6 = 0; num6 <= num10; num6++)
                    {
                        int num11 = num2;
                        for (x = num4; x <= num11; x++)
                        {
                            bitmap2.SetPixel(x - num4, num6, image.GetPixel(x, num6));
                        }
                    }
                    rectangle = new Rectangle(0, 0, bitmap2.Width, bitmap2.Height);
                    data = bitmap2.LockBits(rectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.GenTextures(1, out this.Character[charCode].GLTexture);
                    GL.BindTexture(TextureTarget.Texture2D, this.Character[charCode].GLTexture);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 0x2601);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 0x2601);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap2.Width, bitmap2.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    bitmap2.UnlockBits(data);
                    this.Character[charCode].Width = num3;
                }
                image.Dispose();
                bitmap2.Dispose();
                charCode++;
            }
            while (charCode <= 0xff);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sCharacter
        {
            public int GLTexture;
            public int TexSize;
            public int Width;
        }
    }
}

