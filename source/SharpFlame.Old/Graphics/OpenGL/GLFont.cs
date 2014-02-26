#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Old.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

#endregion

namespace SharpFlame.Old.Graphics.OpenGL
{
    public class GLFont
    {
        public Font BaseFont;

        public sCharacter[] Character = new sCharacter[256];
        public int Height;

        public GLFont(Font baseFont)
        {
            GLTextures_Generate(baseFont);
        }

        private void GLTextures_Generate(Font newBaseFont)
        {
            var a = 0;

            BaseFont = newBaseFont;
            Height = BaseFont.Height;
            for ( a = 0; a <= 255; a++ )
            {
                var text = ((char)a).ToString();
                var tempBitmap = new Bitmap(Height * 2, Height, PixelFormat.Format32bppArgb);
                var gfx = System.Drawing.Graphics.FromImage(tempBitmap);
                gfx.Clear(Color.Transparent);
                gfx.DrawString(text, BaseFont, Brushes.White, 0.0F, 0.0F);
                gfx.Dispose();
                var x = 0;
                var y = 0;
                for ( x = 0; x <= tempBitmap.Width - 1; x++ )
                {
                    for ( y = 0; y <= tempBitmap.Height - 1; y++ )
                    {
                        if ( tempBitmap.GetPixel(x, y).A > 0 )
                        {
                            break;
                        }
                    }
                    if ( y < tempBitmap.Height )
                    {
                        break;
                    }
                }
                var startX = x;
                for ( x = tempBitmap.Width - 1; x >= 0; x-- )
                {
                    for ( y = 0; y <= tempBitmap.Height - 1; y++ )
                    {
                        if ( tempBitmap.GetPixel(x, y).A > 0 )
                        {
                            break;
                        }
                    }
                    if ( y < tempBitmap.Height )
                    {
                        break;
                    }
                }
                var finishX = x;
                var newSizeX = finishX - startX + 1;
                BitmapData bitmapData;
                Bitmap texBitmap;
                if ( newSizeX <= 0 )
                {
                    newSizeX = Math.Max((int)(Math.Round(Height / 4.0F)), 1);
                    Character[a].TexSize = (int)(Math.Round(Math.Pow(2.0D, Math.Ceiling(Math.Log(Math.Max(newSizeX, tempBitmap.Height)) / Math.Log(2.0D)))));
                    texBitmap = new Bitmap(Character[a].TexSize, Convert.ToInt32(Character[a].TexSize), PixelFormat.Format32bppArgb);
                    gfx = System.Drawing.Graphics.FromImage(texBitmap);
                    gfx.Clear(Color.Transparent);
                    gfx.Dispose();
                    bitmapData = texBitmap.LockBits(new Rectangle(0, 0, texBitmap.Width, texBitmap.Height), ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);
                    GL.GenTextures(1, out Character[a].GLTexture);
                    GL.BindTexture(TextureTarget.Texture2D, Convert.ToInt32(Character[a].GLTexture));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texBitmap.Width, texBitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, bitmapData.Scan0);
                    texBitmap.UnlockBits(bitmapData);
                    Character[a].Width = newSizeX;
                }
                else
                {
                    Character[a].TexSize = (int)(Math.Round(Math.Pow(2.0D, Math.Ceiling(Math.Log(Math.Max(newSizeX, tempBitmap.Height)) / Math.Log(2.0D)))));
                    texBitmap = new Bitmap(Convert.ToInt32(Character[a].TexSize), Character[a].TexSize, PixelFormat.Format32bppArgb);
                    gfx = System.Drawing.Graphics.FromImage(texBitmap);
                    gfx.Clear(Color.Transparent);
                    gfx.Dispose();
                    for ( y = 0; y <= tempBitmap.Height - 1; y++ )
                    {
                        for ( x = startX; x <= finishX; x++ )
                        {
                            texBitmap.SetPixel(x - startX, y, tempBitmap.GetPixel(x, y));
                        }
                    }
                    bitmapData = texBitmap.LockBits(new Rectangle(0, 0, texBitmap.Width, texBitmap.Height), ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);
                    GL.GenTextures(1, out Character[a].GLTexture);
                    GL.BindTexture(TextureTarget.Texture2D, Character[a].GLTexture);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texBitmap.Width, texBitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, bitmapData.Scan0);
                    texBitmap.UnlockBits(bitmapData);
                    Character[a].Width = newSizeX;
                }
                tempBitmap.Dispose();
                texBitmap.Dispose();
            }
        }

        public void Deallocate()
        {
            var a = 0;

            for ( a = 0; a <= 255; a++ )
            {
                GL.DeleteTexture(Convert.ToInt32(Character[a].GLTexture));
            }
        }

        public struct sCharacter
        {
            public int GLTexture;
            public int TexSize;
            public int Width;
        }
    }

    public class clsTextLabel
    {
        public SRgba Colour;
        public XYInt Pos;
        public float SizeY;
        public string Text;
        public GLFont TextFont;

        public float GetSizeX()
        {
            float sizeX = 0;
            var charSpacing = SizeY / 10.0F;
            var charSize = SizeY / TextFont.Height;
            var a = 0;

            for ( a = 0; a <= Text.Length - 1; a++ )
            {
                float charWidth = TextFont.Character[Text[a]].Width * charSize;
                sizeX += charWidth;
            }
            sizeX += charSpacing * (Text.Length - 1);

            return sizeX;
        }

        public void Draw()
        {
            if ( Text == null )
            {
                return;
            }
            if ( Text.Length == 0 )
            {
                return;
            }
            if ( TextFont == null )
            {
                return;
            }

            var texRatio = new XYDouble();
            float letterPosA = 0;
            float posY1 = 0;
            float posY2 = 0;
            float charSpacing = 0;
            var a = 0;

            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
            posY1 = Pos.Y;
            posY2 = Pos.Y + SizeY;
            charSpacing = SizeY / 10.0F;
            letterPosA = Pos.X;
            for ( a = 0; a <= Text.Length - 1; a++ )
            {
                int charCode = Text[a];
                if ( charCode >= 0 & charCode <= 255 )
                {
                    float charWidth = SizeY * TextFont.Character[charCode].Width / TextFont.Height;
                    texRatio.X = (float)((double)TextFont.Character[charCode].Width / TextFont.Character[charCode].TexSize);
                    texRatio.Y = (float)((double)TextFont.Height / TextFont.Character[charCode].TexSize);
                    float letterPosB = letterPosA + charWidth;
                    GL.BindTexture(TextureTarget.Texture2D, TextFont.Character[charCode].GLTexture);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(0.0F, 0.0F);
                    GL.Vertex2(letterPosA, posY1);
                    GL.TexCoord2(0.0F, texRatio.Y);
                    GL.Vertex2(letterPosA, posY2);
                    GL.TexCoord2(texRatio.X, texRatio.Y);
                    GL.Vertex2(letterPosB, posY2);
                    GL.TexCoord2(texRatio.X, 0.0F);
                    GL.Vertex2(letterPosB, posY1);
                    GL.End();
                    letterPosA = letterPosB + charSpacing;
                }
            }
        }
    }

    public class clsTextLabels
    {
        public int ItemCount = 0;
        public clsTextLabel[] Items;
        public int MaxCount;

        public clsTextLabels(int maxItemCount)
        {
            MaxCount = maxItemCount;
            Items = new clsTextLabel[MaxCount];
        }

        public bool AtMaxCount()
        {
            return ItemCount >= MaxCount;
        }

        public void Add(clsTextLabel newItem)
        {
            if ( ItemCount == MaxCount )
            {
                Debugger.Break();
                return;
            }

            Items[ItemCount] = newItem;
            ItemCount++;
        }

        public void Draw()
        {
            var a = 0;

            for ( a = 0; a <= ItemCount - 1; a++ )
            {
                Items[a].Draw();
            }
        }
    }
}