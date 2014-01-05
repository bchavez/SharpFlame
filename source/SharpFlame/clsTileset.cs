using System;
using System.Drawing;
using Microsoft.VisualBasic;
using OpenTK.Graphics.OpenGL;

namespace SharpFlame
{
    public class clsTileset
    {
        public string Name;

        public bool IsOriginal;

        public struct sTile
        {
            public int MapView_GL_Texture_Num;
            public int TextureView_GL_Texture_Num;
            public sRGB_sng AverageColour;
            public byte Default_Type;
        }

        public sTile[] Tiles;
        public int TileCount;

        public sRGB_sng BGColour = new sRGB_sng(0.5F, 0.5F, 0.5F);

        public modProgram.sResult Default_TileTypes_Load(string Path)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            System.IO.BinaryReader File = default(System.IO.BinaryReader);

            try
            {
                File = new System.IO.BinaryReader(new System.IO.FileStream(Path, System.IO.FileMode.Open));
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                return ReturnResult;
            }
            ReturnResult = Default_TileTypes_Read(File);
            File.Close();
            return ReturnResult;
        }

        private modProgram.sResult Default_TileTypes_Read(System.IO.BinaryReader File)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            UInt32 uintTemp = 0;
            int A = 0;
            UInt16 ushortTemp = 0;
            string strTemp = "";

            try
            {
                strTemp = modIO.ReadOldTextOfLength(File, 4);
                if ( strTemp != "ttyp" )
                {
                    ReturnResult.Problem = "Bad identifier.";
                    return ReturnResult;
                }

                uintTemp = File.ReadUInt32();
                if ( !(uintTemp == 8U) )
                {
                    ReturnResult.Problem = "Unknown version.";
                    return ReturnResult;
                }

                uintTemp = File.ReadUInt32();
                TileCount = System.Convert.ToInt32(uintTemp);
                Tiles = new sTile[TileCount - 1 + 1];

                for ( A = 0; A <= Math.Min(System.Convert.ToInt32(uintTemp), TileCount) - 1; A++ )
                {
                    ushortTemp = File.ReadUInt16();
                    if ( ushortTemp > modProgram.TileTypes.Count )
                    {
                        ReturnResult.Problem = "Unknown tile type.";
                        return ReturnResult;
                    }
                    Tiles[A].Default_Type = (byte)ushortTemp;
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public clsResult LoadDirectory(string Path)
        {
            clsResult ReturnResult =
                new clsResult("Loading tileset from " + System.Convert.ToString(ControlChars.Quote) + Path + System.Convert.ToString(ControlChars.Quote));

            Bitmap Bitmap = null;
            modProgram.sSplitPath SplitPath = new modProgram.sSplitPath(Path);
            string SlashPath = modProgram.EndWithPathSeperator(Path);
            modProgram.sResult Result = new modProgram.sResult();

            if ( SplitPath.FileTitle != "" )
            {
                Name = SplitPath.FileTitle;
            }
            else if ( SplitPath.PartCount >= 2 )
            {
                Name = SplitPath.Parts[SplitPath.PartCount - 2];
            }

            Result = Default_TileTypes_Load(SlashPath + Name + ".ttp");
            if ( !Result.Success )
            {
                ReturnResult.ProblemAdd("Loading tile types: " + Result.Problem);
                return ReturnResult;
            }

            int RedTotal = 0;
            int GreenTotal = 0;
            int BlueTotal = 0;
            int TileNum = 0;
            string strTile = "";
            modBitmap.sBitmapGLTexture BitmapTextureArgs = new modBitmap.sBitmapGLTexture();
            float[] AverageColour = new float[4];
            int X = 0;
            int Y = 0;
            Color Pixel = new Color();

            string GraphicPath = "";

            //tile count has been set by the ttp file

            for ( TileNum = 0; TileNum <= TileCount - 1; TileNum++ )
            {
                strTile = "tile-" + modProgram.MinDigits(TileNum, 2) + ".png";

                //-------- 128 --------

                GraphicPath = SlashPath + Name + "-128" + System.Convert.ToString(modProgram.PlatformPathSeparator) + strTile;

                Result = modBitmap.LoadBitmap(GraphicPath, ref Bitmap);
                if ( !Result.Success )
                {
                    //ignore and exit, since not all tile types have a corresponding tile graphic
                    return ReturnResult;
                }

                if ( Bitmap.Width != 128 | Bitmap.Height != 128 )
                {
                    ReturnResult.WarningAdd("Tile graphic " + GraphicPath + " from tileset " + Name + " is not 128x128.");
                    return ReturnResult;
                }

                BitmapTextureArgs.Texture = Bitmap;
                BitmapTextureArgs.MipMapLevel = 0;
                BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest;
                BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest;
                BitmapTextureArgs.TextureNum = 0;
                BitmapTextureArgs.Perform();
                Tiles[TileNum].TextureView_GL_Texture_Num = BitmapTextureArgs.TextureNum;

                BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest;
                if ( modSettings.Settings.Mipmaps )
                {
                    BitmapTextureArgs.MinFilter = TextureMinFilter.LinearMipmapLinear;
                }
                else
                {
                    BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest;
                }
                BitmapTextureArgs.TextureNum = 0;

                BitmapTextureArgs.Perform();
                Tiles[TileNum].MapView_GL_Texture_Num = BitmapTextureArgs.TextureNum;

                if ( modSettings.Settings.Mipmaps )
                {
                    if ( modSettings.Settings.MipmapsHardware )
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                        GL.Disable(EnableCap.Texture2D);
                    }
                    else
                    {
                        clsResult MipmapResult = default(clsResult);
                        MipmapResult = GenerateMipMaps(SlashPath, strTile, BitmapTextureArgs, TileNum);
                        ReturnResult.Add(MipmapResult);
                        if ( MipmapResult.HasProblems )
                        {
                            return ReturnResult;
                        }
                    }
                    GL.GetTexImage<Single>(TextureTarget.Texture2D, 7, PixelFormat.Rgba, PixelType.Float, AverageColour);
                    Tiles[TileNum].AverageColour.Red = AverageColour[0];
                    Tiles[TileNum].AverageColour.Green = AverageColour[1];
                    Tiles[TileNum].AverageColour.Blue = AverageColour[2];
                }
                else
                {
                    RedTotal = 0;
                    GreenTotal = 0;
                    BlueTotal = 0;
                    for ( Y = 0; Y <= Bitmap.Height - 1; Y++ )
                    {
                        for ( X = 0; X <= Bitmap.Width - 1; X++ )
                        {
                            Pixel = Bitmap.GetPixel(X, Y);
                            RedTotal += Pixel.R;
                            GreenTotal += Pixel.G;
                            BlueTotal += Pixel.B;
                        }
                    }
                    Tiles[TileNum].AverageColour.Red = (float)(RedTotal / 4177920.0D);
                    Tiles[TileNum].AverageColour.Green = (float)(GreenTotal / 4177920.0D);
                    Tiles[TileNum].AverageColour.Blue = (float)(BlueTotal / 4177920.0D);
                }
            }

            return ReturnResult;
        }

        public clsResult GenerateMipMaps(string SlashPath, string strTile, modBitmap.sBitmapGLTexture BitmapTextureArgs, int TileNum)
        {
            clsResult ReturnResult = new clsResult("Generating mipmaps");
            string GraphicPath = "";
            int PixX = 0;
            int PixY = 0;
            Color PixelColorA = new Color();
            Color PixelColorB = new Color();
            Color PixelColorC = new Color();
            Color PixelColorD = new Color();
            int X1 = 0;
            int Y1 = 0;
            int X2 = 0;
            int Y2 = 0;
            int Red = 0;
            int Green = 0;
            int Blue = 0;
            Bitmap Bitmap8 = default(Bitmap);
            Bitmap Bitmap4 = default(Bitmap);
            Bitmap Bitmap2 = default(Bitmap);
            Bitmap Bitmap1 = default(Bitmap);
            Bitmap Bitmap = null;
            modProgram.sResult Result = new modProgram.sResult();

            //-------- 64 --------

            GraphicPath = SlashPath + Name + "-64" + System.Convert.ToString(modProgram.PlatformPathSeparator) + strTile;

            Result = modBitmap.LoadBitmap(GraphicPath, ref Bitmap);
            if ( !Result.Success )
            {
                ReturnResult.WarningAdd("Unable to load tile graphic: " + Result.Problem);
                return ReturnResult;
            }

            if ( Bitmap.Width != 64 | Bitmap.Height != 64 )
            {
                ReturnResult.WarningAdd("Tile graphic " + GraphicPath + " from tileset " + Name + " is not 64x64.");
                return ReturnResult;
            }

            BitmapTextureArgs.Texture = Bitmap;
            BitmapTextureArgs.MipMapLevel = 1;
            BitmapTextureArgs.Perform();

            //-------- 32 --------

            GraphicPath = SlashPath + Name + "-32" + System.Convert.ToString(modProgram.PlatformPathSeparator) + strTile;

            Result = modBitmap.LoadBitmap(GraphicPath, ref Bitmap);
            if ( !Result.Success )
            {
                ReturnResult.WarningAdd("Unable to load tile graphic: " + Result.Problem);
                return ReturnResult;
            }

            if ( Bitmap.Width != 32 | Bitmap.Height != 32 )
            {
                ReturnResult.WarningAdd("Tile graphic " + GraphicPath + " from tileset " + Name + " is not 32x32.");
                return ReturnResult;
            }

            BitmapTextureArgs.Texture = Bitmap;
            BitmapTextureArgs.MipMapLevel = 2;
            BitmapTextureArgs.Perform();

            //-------- 16 --------

            GraphicPath = SlashPath + Name + "-16" + System.Convert.ToString(modProgram.PlatformPathSeparator) + strTile;

            Result = modBitmap.LoadBitmap(GraphicPath, ref Bitmap);
            if ( !Result.Success )
            {
                ReturnResult.WarningAdd("Unable to load tile graphic: " + Result.Problem);
                return ReturnResult;
            }

            if ( Bitmap.Width != 16 | Bitmap.Height != 16 )
            {
                ReturnResult.WarningAdd("Tile graphic " + GraphicPath + " from tileset " + Name + " is not 16x16.");
                return ReturnResult;
            }

            BitmapTextureArgs.Texture = Bitmap;
            BitmapTextureArgs.MipMapLevel = 3;
            BitmapTextureArgs.Perform();

            //-------- 8 --------

            Bitmap8 = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for ( PixY = 0; PixY <= 7; PixY++ )
            {
                Y1 = PixY * 2;
                Y2 = Y1 + 1;
                for ( PixX = 0; PixX <= 7; PixX++ )
                {
                    X1 = PixX * 2;
                    X2 = X1 + 1;
                    PixelColorA = Bitmap.GetPixel(X1, Y1);
                    PixelColorB = Bitmap.GetPixel(X2, Y1);
                    PixelColorC = Bitmap.GetPixel(X1, Y2);
                    PixelColorD = Bitmap.GetPixel(X2, Y2);
                    Red = System.Convert.ToInt32(((PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F);
                    Green = System.Convert.ToInt32(((PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F);
                    Blue = System.Convert.ToInt32(((PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F);
                    Bitmap8.SetPixel(PixX, PixY, ColorTranslator.FromOle(modColour.OSRGB(Red, Green, Blue)));
                }
            }

            BitmapTextureArgs.Texture = Bitmap8;
            BitmapTextureArgs.MipMapLevel = 4;
            BitmapTextureArgs.Perform();

            //-------- 4 --------

            Bitmap = Bitmap8;
            Bitmap4 = new Bitmap(4, 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for ( PixY = 0; PixY <= 3; PixY++ )
            {
                Y1 = PixY * 2;
                Y2 = Y1 + 1;
                for ( PixX = 0; PixX <= 3; PixX++ )
                {
                    X1 = PixX * 2;
                    X2 = X1 + 1;
                    PixelColorA = Bitmap.GetPixel(X1, Y1);
                    PixelColorB = Bitmap.GetPixel(X2, Y1);
                    PixelColorC = Bitmap.GetPixel(X1, Y2);
                    PixelColorD = Bitmap.GetPixel(X2, Y2);
                    Red = System.Convert.ToInt32(((PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F);
                    Green = System.Convert.ToInt32(((PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F);
                    Blue = System.Convert.ToInt32(((PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F);
                    Bitmap4.SetPixel(PixX, PixY, ColorTranslator.FromOle(modColour.OSRGB(Red, Green, Blue)));
                }
            }

            BitmapTextureArgs.Texture = Bitmap4;
            BitmapTextureArgs.MipMapLevel = 5;
            BitmapTextureArgs.Perform();

            //-------- 2 --------

            Bitmap = Bitmap4;
            Bitmap2 = new Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for ( PixY = 0; PixY <= 1; PixY++ )
            {
                Y1 = PixY * 2;
                Y2 = Y1 + 1;
                for ( PixX = 0; PixX <= 1; PixX++ )
                {
                    X1 = PixX * 2;
                    X2 = X1 + 1;
                    PixelColorA = Bitmap.GetPixel(X1, Y1);
                    PixelColorB = Bitmap.GetPixel(X2, Y1);
                    PixelColorC = Bitmap.GetPixel(X1, Y2);
                    PixelColorD = Bitmap.GetPixel(X2, Y2);
                    Red = System.Convert.ToInt32(((PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F);
                    Green = System.Convert.ToInt32(((PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F);
                    Blue = System.Convert.ToInt32(((PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F);
                    Bitmap2.SetPixel(PixX, PixY, ColorTranslator.FromOle(modColour.OSRGB(Red, Green, Blue)));
                }
            }

            BitmapTextureArgs.Texture = Bitmap2;
            BitmapTextureArgs.MipMapLevel = 6;
            BitmapTextureArgs.Perform();

            //-------- 1 --------

            Bitmap = Bitmap2;
            Bitmap1 = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            PixX = 0;
            PixY = 0;
            Y1 = PixY * 2;
            Y2 = Y1 + 1;
            X1 = PixX * 2;
            X2 = X1 + 1;
            PixelColorA = Bitmap.GetPixel(X1, Y1);
            PixelColorB = Bitmap.GetPixel(X2, Y1);
            PixelColorC = Bitmap.GetPixel(X1, Y2);
            PixelColorD = Bitmap.GetPixel(X2, Y2);
            Red = System.Convert.ToInt32(((PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F);
            Green = System.Convert.ToInt32(((PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F);
            Blue = System.Convert.ToInt32(((PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F);
            Bitmap1.SetPixel(PixX, PixY, ColorTranslator.FromOle(modColour.OSRGB(Red, Green, Blue)));

            BitmapTextureArgs.Texture = Bitmap1;
            BitmapTextureArgs.MipMapLevel = 7;
            BitmapTextureArgs.Perform();

            return ReturnResult;
        }
    }
}