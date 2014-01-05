namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    public class clsTileset
    {
        public sRGB_sng BGColour = new sRGB_sng(0.5f, 0.5f, 0.5f);
        public bool IsOriginal;
        public string Name;
        public int TileCount;
        public sTile[] Tiles;

        public modProgram.sResult Default_TileTypes_Load(string Path)
        {
            BinaryReader reader;
            modProgram.sResult result2;
            try
            {
                reader = new BinaryReader(new FileStream(Path, FileMode.Open));
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2 = this.Default_TileTypes_Read(reader);
            reader.Close();
            return result2;
        }

        private modProgram.sResult Default_TileTypes_Read(BinaryReader File)
        {
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                if (modIO.ReadOldTextOfLength(File, 4) != "ttyp")
                {
                    result2.Problem = "Bad identifier.";
                    return result2;
                }
                if (File.ReadUInt32() != 8)
                {
                    result2.Problem = "Unknown version.";
                    return result2;
                }
                uint num2 = File.ReadUInt32();
                this.TileCount = (int) num2;
                this.Tiles = new sTile[(this.TileCount - 1) + 1];
                int num4 = Math.Min((int) num2, this.TileCount) - 1;
                for (int i = 0; i <= num4; i++)
                {
                    ushort num3 = File.ReadUInt16();
                    if (num3 > modProgram.TileTypes.Count)
                    {
                        result2.Problem = "Unknown tile type.";
                        return result2;
                    }
                    this.Tiles[i].Default_Type = (byte) num3;
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Success = true;
            return result2;
        }

        public clsResult GenerateMipMaps(string SlashPath, string strTile, modBitmap.sBitmapGLTexture BitmapTextureArgs, int TileNum)
        {
            int num;
            int num2;
            Color pixel;
            Color color2;
            Color color3;
            Color color4;
            int num3;
            int num5;
            int num6;
            int num7;
            int num8;
            int num9;
            clsResult result3 = new clsResult("Generating mipmaps");
            System.Drawing.Bitmap resultBitmap = null;
            string path = SlashPath + this.Name + "-64" + Conversions.ToString(modProgram.PlatformPathSeparator) + strTile;
            modProgram.sResult result2 = modBitmap.LoadBitmap(path, ref resultBitmap);
            if (!result2.Success)
            {
                result3.WarningAdd("Unable to load tile graphic: " + result2.Problem);
                return result3;
            }
            if ((resultBitmap.Width != 0x40) | (resultBitmap.Height != 0x40))
            {
                result3.WarningAdd("Tile graphic " + path + " from tileset " + this.Name + " is not 64x64.");
                return result3;
            }
            BitmapTextureArgs.Texture = resultBitmap;
            BitmapTextureArgs.MipMapLevel = 1;
            BitmapTextureArgs.Perform();
            path = SlashPath + this.Name + "-32" + Conversions.ToString(modProgram.PlatformPathSeparator) + strTile;
            result2 = modBitmap.LoadBitmap(path, ref resultBitmap);
            if (!result2.Success)
            {
                result3.WarningAdd("Unable to load tile graphic: " + result2.Problem);
                return result3;
            }
            if ((resultBitmap.Width != 0x20) | (resultBitmap.Height != 0x20))
            {
                result3.WarningAdd("Tile graphic " + path + " from tileset " + this.Name + " is not 32x32.");
                return result3;
            }
            BitmapTextureArgs.Texture = resultBitmap;
            BitmapTextureArgs.MipMapLevel = 2;
            BitmapTextureArgs.Perform();
            path = SlashPath + this.Name + "-16" + Conversions.ToString(modProgram.PlatformPathSeparator) + strTile;
            result2 = modBitmap.LoadBitmap(path, ref resultBitmap);
            if (!result2.Success)
            {
                result3.WarningAdd("Unable to load tile graphic: " + result2.Problem);
                return result3;
            }
            if ((resultBitmap.Width != 0x10) | (resultBitmap.Height != 0x10))
            {
                result3.WarningAdd("Tile graphic " + path + " from tileset " + this.Name + " is not 16x16.");
                return result3;
            }
            BitmapTextureArgs.Texture = resultBitmap;
            BitmapTextureArgs.MipMapLevel = 3;
            BitmapTextureArgs.Perform();
            System.Drawing.Bitmap bitmap5 = new System.Drawing.Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int y = 0;
            do
            {
                num8 = y * 2;
                num9 = num8 + 1;
                num3 = 0;
                do
                {
                    num6 = num3 * 2;
                    num7 = num6 + 1;
                    pixel = resultBitmap.GetPixel(num6, num8);
                    color2 = resultBitmap.GetPixel(num7, num8);
                    color3 = resultBitmap.GetPixel(num6, num9);
                    color4 = resultBitmap.GetPixel(num7, num9);
                    num5 = (int) Math.Round((double) (((float) (((pixel.R + color2.R) + color3.R) + color4.R)) / 4f));
                    num2 = (int) Math.Round((double) (((float) (((pixel.G + color2.G) + color3.G) + color4.G)) / 4f));
                    num = (int) Math.Round((double) (((float) (((pixel.B + color2.B) + color3.B) + color4.B)) / 4f));
                    bitmap5.SetPixel(num3, y, ColorTranslator.FromOle(modColour.OSRGB(num5, num2, num)));
                    num3++;
                }
                while (num3 <= 7);
                y++;
            }
            while (y <= 7);
            BitmapTextureArgs.Texture = bitmap5;
            BitmapTextureArgs.MipMapLevel = 4;
            BitmapTextureArgs.Perform();
            resultBitmap = bitmap5;
            System.Drawing.Bitmap bitmap4 = new System.Drawing.Bitmap(4, 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            y = 0;
            do
            {
                num8 = y * 2;
                num9 = num8 + 1;
                num3 = 0;
                do
                {
                    num6 = num3 * 2;
                    num7 = num6 + 1;
                    pixel = resultBitmap.GetPixel(num6, num8);
                    color2 = resultBitmap.GetPixel(num7, num8);
                    color3 = resultBitmap.GetPixel(num6, num9);
                    color4 = resultBitmap.GetPixel(num7, num9);
                    num5 = (int) Math.Round((double) (((float) (((pixel.R + color2.R) + color3.R) + color4.R)) / 4f));
                    num2 = (int) Math.Round((double) (((float) (((pixel.G + color2.G) + color3.G) + color4.G)) / 4f));
                    num = (int) Math.Round((double) (((float) (((pixel.B + color2.B) + color3.B) + color4.B)) / 4f));
                    bitmap4.SetPixel(num3, y, ColorTranslator.FromOle(modColour.OSRGB(num5, num2, num)));
                    num3++;
                }
                while (num3 <= 3);
                y++;
            }
            while (y <= 3);
            BitmapTextureArgs.Texture = bitmap4;
            BitmapTextureArgs.MipMapLevel = 5;
            BitmapTextureArgs.Perform();
            resultBitmap = bitmap4;
            System.Drawing.Bitmap bitmap3 = new System.Drawing.Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            y = 0;
            do
            {
                num8 = y * 2;
                num9 = num8 + 1;
                num3 = 0;
                do
                {
                    num6 = num3 * 2;
                    num7 = num6 + 1;
                    pixel = resultBitmap.GetPixel(num6, num8);
                    color2 = resultBitmap.GetPixel(num7, num8);
                    color3 = resultBitmap.GetPixel(num6, num9);
                    color4 = resultBitmap.GetPixel(num7, num9);
                    num5 = (int) Math.Round((double) (((float) (((pixel.R + color2.R) + color3.R) + color4.R)) / 4f));
                    num2 = (int) Math.Round((double) (((float) (((pixel.G + color2.G) + color3.G) + color4.G)) / 4f));
                    num = (int) Math.Round((double) (((float) (((pixel.B + color2.B) + color3.B) + color4.B)) / 4f));
                    bitmap3.SetPixel(num3, y, ColorTranslator.FromOle(modColour.OSRGB(num5, num2, num)));
                    num3++;
                }
                while (num3 <= 1);
                y++;
            }
            while (y <= 1);
            BitmapTextureArgs.Texture = bitmap3;
            BitmapTextureArgs.MipMapLevel = 6;
            BitmapTextureArgs.Perform();
            resultBitmap = bitmap3;
            System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            num3 = 0;
            y = 0;
            num8 = y * 2;
            num9 = num8 + 1;
            num6 = num3 * 2;
            num7 = num6 + 1;
            pixel = resultBitmap.GetPixel(num6, num8);
            color2 = resultBitmap.GetPixel(num7, num8);
            color3 = resultBitmap.GetPixel(num6, num9);
            color4 = resultBitmap.GetPixel(num7, num9);
            num5 = (int) Math.Round((double) (((float) (((pixel.R + color2.R) + color3.R) + color4.R)) / 4f));
            num2 = (int) Math.Round((double) (((float) (((pixel.G + color2.G) + color3.G) + color4.G)) / 4f));
            num = (int) Math.Round((double) (((float) (((pixel.B + color2.B) + color3.B) + color4.B)) / 4f));
            bitmap2.SetPixel(num3, y, ColorTranslator.FromOle(modColour.OSRGB(num5, num2, num)));
            BitmapTextureArgs.Texture = bitmap2;
            BitmapTextureArgs.MipMapLevel = 7;
            BitmapTextureArgs.Perform();
            return result3;
        }

        public clsResult LoadDirectory(string Path)
        {
            clsResult result3 = new clsResult("Loading tileset from \"" + Path + "\"");
            System.Drawing.Bitmap resultBitmap = null;
            modProgram.sSplitPath path = new modProgram.sSplitPath(Path);
            string slashPath = modProgram.EndWithPathSeperator(Path);
            if (path.FileTitle != "")
            {
                this.Name = path.FileTitle;
            }
            else if (path.PartCount >= 2)
            {
                this.Name = path.Parts[path.PartCount - 2];
            }
            modProgram.sResult result2 = this.Default_TileTypes_Load(slashPath + this.Name + ".ttp");
            if (!result2.Success)
            {
                result3.ProblemAdd("Loading tile types: " + result2.Problem);
                return result3;
            }
            float[] pixels = new float[4];
            int num7 = this.TileCount - 1;
            for (int i = 0; i <= num7; i++)
            {
                modBitmap.sBitmapGLTexture texture;
                string strTile = "tile-" + modProgram.MinDigits(i, 2) + ".png";
                string str = slashPath + this.Name + "-128" + Conversions.ToString(modProgram.PlatformPathSeparator) + strTile;
                if (!modBitmap.LoadBitmap(str, ref resultBitmap).Success)
                {
                    return result3;
                }
                if ((resultBitmap.Width != 0x80) | (resultBitmap.Height != 0x80))
                {
                    result3.WarningAdd("Tile graphic " + str + " from tileset " + this.Name + " is not 128x128.");
                    return result3;
                }
                texture.Texture = resultBitmap;
                texture.MipMapLevel = 0;
                texture.MagFilter = TextureMagFilter.Nearest;
                texture.MinFilter = TextureMinFilter.Nearest;
                texture.TextureNum = 0;
                texture.Perform();
                this.Tiles[i].TextureView_GL_Texture_Num = texture.TextureNum;
                texture.MagFilter = TextureMagFilter.Nearest;
                if (modSettings.Settings.Mipmaps)
                {
                    texture.MinFilter = TextureMinFilter.LinearMipmapLinear;
                }
                else
                {
                    texture.MinFilter = TextureMinFilter.Nearest;
                }
                texture.TextureNum = 0;
                texture.Perform();
                this.Tiles[i].MapView_GL_Texture_Num = texture.TextureNum;
                if (modSettings.Settings.Mipmaps)
                {
                    if (modSettings.Settings.MipmapsHardware)
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                        GL.Disable(EnableCap.Texture2D);
                    }
                    else
                    {
                        clsResult resultToAdd = this.GenerateMipMaps(slashPath, strTile, texture, i);
                        result3.Add(resultToAdd);
                        if (resultToAdd.HasProblems)
                        {
                            return result3;
                        }
                    }
                    GL.GetTexImage<float>(TextureTarget.Texture2D, 7, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.Float, pixels);
                    this.Tiles[i].AverageColour.Red = pixels[0];
                    this.Tiles[i].AverageColour.Green = pixels[1];
                    this.Tiles[i].AverageColour.Blue = pixels[2];
                }
                else
                {
                    int num3 = 0;
                    int num2 = 0;
                    int num = 0;
                    int num8 = resultBitmap.Height - 1;
                    for (int j = 0; j <= num8; j++)
                    {
                        int num9 = resultBitmap.Width - 1;
                        for (int k = 0; k <= num9; k++)
                        {
                            Color pixel = resultBitmap.GetPixel(k, j);
                            num3 += pixel.R;
                            num2 += pixel.G;
                            num += pixel.B;
                        }
                    }
                    this.Tiles[i].AverageColour.Red = (float) (((double) num3) / 4177920.0);
                    this.Tiles[i].AverageColour.Green = (float) (((double) num2) / 4177920.0);
                    this.Tiles[i].AverageColour.Blue = (float) (((double) num) / 4177920.0);
                }
            }
            return result3;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sTile
        {
            public int MapView_GL_Texture_Num;
            public int TextureView_GL_Texture_Num;
            public sRGB_sng AverageColour;
            public byte Default_Type;
        }
    }
}

