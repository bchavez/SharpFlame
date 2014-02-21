#region

using System;
using System.Drawing;
using System.IO;
using NLog;
using OpenTK.Graphics.OpenGL;
using SharpFlame.AppSettings;
using SharpFlame.Bitmaps;
using SharpFlame.Colors;
using SharpFlame.Core.Extensions;
using SharpFlame.FileIO;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Mapping.Tiles
{
    public class clsTileset
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public sRGB_sng BGColour = new sRGB_sng(0.5F, 0.5F, 0.5F);

        public bool IsOriginal { get; set; }

        public struct Tile
        {
            public sRGB_sng AverageColour;
            public byte DefaultType;
            public int GlTextureNum;
        }

        public Tile[] Tiles { get; set; }

        public int TileCount { get; set; }

        public string Name { get; set; }

        private sResult loadTileType(string path)
        {
            var returnResult = new sResult();
            BinaryReader file;

            try
            {
                file = new BinaryReader(new FileStream(path, FileMode.Open));
            }
            catch ( Exception ex )
            {
                returnResult.Problem = ex.Message;
                return returnResult;
            }
            returnResult = readTileType(file);
            file.Close();
            return returnResult;
        }

        private sResult readTileType(BinaryReader file)
        {
            var returnResult = new sResult();
            returnResult.Success = false;
            returnResult.Problem = "";

            UInt32 uintTemp = 0;
            var i = 0;
            UInt16 ushortTemp = 0;
            var strTemp = "";

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(file, 4);
                if ( strTemp != "ttyp" )
                {
                    returnResult.Problem = "Bad identifier.";
                    return returnResult;
                }

                uintTemp = file.ReadUInt32();
                if ( !(uintTemp == 8U) )
                {
                    returnResult.Problem = "Unknown version.";
                    return returnResult;
                }

                uintTemp = file.ReadUInt32();
                TileCount = Convert.ToInt32(uintTemp);
                Tiles = new Tile[TileCount];

                for ( i = 0; i < Math.Min((Int32)uintTemp, TileCount); i++ )
                {
                    ushortTemp = file.ReadUInt16();
                    if ( ushortTemp > App.TileTypes.Count )
                    {
                        returnResult.Problem = "Unknown tile type.";
                        return returnResult;
                    }
                    Tiles[i].DefaultType = (byte)ushortTemp;
                }
            }
            catch ( Exception ex )
            {
                returnResult.Problem = ex.Message;
                return returnResult;
            }

            returnResult.Success = true;
            return returnResult;
        }

        public clsResult LoadDirectory(string path)
        {
            var returnResult = new clsResult("Loading tileset from '{0}'".Format2(path), false);
            logger.Info("Loading tileset from '{0}'".Format2(path));

            Bitmap bitmap = null;
            var SplitPath = new sSplitPath(path);
            var slashPath = PathUtil.EndWithPathSeperator(path);
            var result = new sResult();

            if ( SplitPath.FileTitle != "" )
            {
                Name = SplitPath.FileTitle;
            }
            else if ( SplitPath.PartCount >= 2 )
            {
                Name = SplitPath.Parts[SplitPath.PartCount - 2];
            }

            var ttpFileName = Path.ChangeExtension(Name, ".ttp");

            result = loadTileType(Path.Combine(slashPath, ttpFileName));

            if ( !result.Success )
            {
                returnResult.ProblemAdd("Loading tile types: " + result.Problem);
                return returnResult;
            }

            var redTotal = 0;
            var greenTotal = 0;
            var blueTotal = 0;
            var tileNum = 0;
            var strTile = "";
            var AverageColour = new float[4];
            var x = 0;
            var y = 0;
            var Pixel = new Color();

            var graphicPath = "";

            //tile count has been set by the ttp file

            for ( tileNum = 0; tileNum <= TileCount - 1; tileNum++ )
            {
                strTile = "tile-" + App.MinDigits(tileNum, 2) + ".png";

                //-------- 128 --------

                var tileDir = Path.Combine(Name + "-128", strTile);
                graphicPath = Path.Combine(slashPath, tileDir);


                result = BitmapUtil.LoadBitmap(graphicPath, ref bitmap);
                if ( !result.Success )
                {
                    //ignore and exit, since not all tile types have a corresponding tile graphic
                    return returnResult;
                }

                if ( bitmap.Width != 128 | bitmap.Height != 128 )
                {
                    returnResult.WarningAdd("Tile graphic " + graphicPath + " from tileset " + Name + " is not 128x128.");
                    return returnResult;
                }

                if ( SettingsManager.Settings.Mipmaps )
                {
                    Tiles[tileNum].GlTextureNum = BitmapUtil.CreateGLTexture (bitmap, 0, 0,
                                                         TextureMagFilter.Nearest, 
                                                         TextureMinFilter.LinearMipmapLinear);
                }
                else
                {
                    Tiles[tileNum].GlTextureNum = BitmapUtil.CreateGLTexture (bitmap, 0, 0, 
                                                         TextureMagFilter.Nearest, 
                                                         TextureMinFilter.Nearest);
                }

                if ( SettingsManager.Settings.Mipmaps )
                {
                    if ( SettingsManager.Settings.MipmapsHardware )
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                        GL.Disable(EnableCap.Texture2D);
                    }
                    else
                    {
                        var MipmapResult = default(clsResult);
                        MipmapResult = generateMipMaps(slashPath, strTile, 
                                                       Tiles[tileNum].GlTextureNum, tileNum);
                        returnResult.Add(MipmapResult);
                        if ( MipmapResult.HasProblems )
                        {
                            return returnResult;
                        }
                    }
                    GL.GetTexImage(TextureTarget.Texture2D, 7, PixelFormat.Rgba, PixelType.Float, AverageColour);
                    Tiles[tileNum].AverageColour.Red = AverageColour[0];
                    Tiles[tileNum].AverageColour.Green = AverageColour[1];
                    Tiles[tileNum].AverageColour.Blue = AverageColour[2];
                }
                else
                {
                    redTotal = 0;
                    greenTotal = 0;
                    blueTotal = 0;
                    for ( y = 0; y <= bitmap.Height - 1; y++ )
                    {
                        for ( x = 0; x <= bitmap.Width - 1; x++ )
                        {
                            Pixel = bitmap.GetPixel(x, y);
                            redTotal += Pixel.R;
                            greenTotal += Pixel.G;
                            blueTotal += Pixel.B;
                        }
                    }
                    Tiles[tileNum].AverageColour.Red = (float)(redTotal / 4177920.0D);
                    Tiles[tileNum].AverageColour.Green = (float)(greenTotal / 4177920.0D);
                    Tiles[tileNum].AverageColour.Blue = (float)(blueTotal / 4177920.0D);
                }
            }

            return returnResult;
        }

        private clsResult generateMipMaps(string slashPath, string strTile, int textureNum, int tileNum)
        {
            var ReturnResult = new clsResult("Generating mipmaps", false);
            logger.Info("Generating mipmaps");
            var graphicPath = "";
            var pixX = 0;
            var pixY = 0;
            var pixelColorA = new Color();
            var pixelColorB = new Color();
            var pixelColorC = new Color();
            var pixelColorD = new Color();
            var x1 = 0;
            var y1 = 0;
            var x2 = 0;
            var y2 = 0;
            var red = 0;
            var green = 0;
            var blue = 0;
            var bitmap8 = default(Bitmap);
            var bitmap4 = default(Bitmap);
            var bitmap2 = default(Bitmap);
            var bitmap1 = default(Bitmap);
            Bitmap bitmap = null;
            var result = new sResult();

            //-------- 64 --------

            graphicPath = string.Format ("{0}{1}-64{2}{3}", slashPath, Name, App.PlatformPathSeparator, strTile);

            result = BitmapUtil.LoadBitmap(graphicPath, ref bitmap);
            if ( !result.Success )
            {
                ReturnResult.WarningAdd("Unable to load tile graphic: " + result.Problem);
                return ReturnResult;
            }

            if ( bitmap.Width != 64 | bitmap.Height != 64 )
            {
                ReturnResult.WarningAdd("Tile graphic " + graphicPath + " from tileset " + Name + " is not 64x64.");
                return ReturnResult;
            }

            BitmapUtil.CreateGLTexture (bitmap, 1, textureNum);

            //-------- 32 --------

            graphicPath = slashPath + Name + "-32" + Convert.ToString(App.PlatformPathSeparator) + strTile;

            result = BitmapUtil.LoadBitmap(graphicPath, ref bitmap);
            if ( !result.Success )
            {
                ReturnResult.WarningAdd("Unable to load tile graphic: " + result.Problem);
                return ReturnResult;
            }

            if ( bitmap.Width != 32 | bitmap.Height != 32 )
            {
                ReturnResult.WarningAdd("Tile graphic " + graphicPath + " from tileset " + Name + " is not 32x32.");
                return ReturnResult;
            }

            BitmapUtil.CreateGLTexture (bitmap, 2, textureNum);

            //-------- 16 --------

            graphicPath = slashPath + Name + "-16" + Convert.ToString(App.PlatformPathSeparator) + strTile;

            result = BitmapUtil.LoadBitmap(graphicPath, ref bitmap);
            if ( !result.Success )
            {
                ReturnResult.WarningAdd("Unable to load tile graphic: " + result.Problem);
                return ReturnResult;
            }

            if ( bitmap.Width != 16 | bitmap.Height != 16 )
            {
                ReturnResult.WarningAdd("Tile graphic " + graphicPath + " from tileset " + Name + " is not 16x16.");
                return ReturnResult;
            }

            BitmapUtil.CreateGLTexture (bitmap, 3, textureNum);

            //-------- 8 --------

            bitmap8 = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for ( pixY = 0; pixY <= 7; pixY++ )
            {
                y1 = pixY * 2;
                y2 = y1 + 1;
                for ( pixX = 0; pixX <= 7; pixX++ )
                {
                    x1 = pixX * 2;
                    x2 = x1 + 1;
                    pixelColorA = bitmap.GetPixel(x1, y1);
                    pixelColorB = bitmap.GetPixel(x2, y1);
                    pixelColorC = bitmap.GetPixel(x1, y2);
                    pixelColorD = bitmap.GetPixel(x2, y2);
                    red = Convert.ToInt32(((pixelColorA.R) + pixelColorB.R + pixelColorC.R + pixelColorD.R) / 4.0F);
                    green = Convert.ToInt32(((pixelColorA.G) + pixelColorB.G + pixelColorC.G + pixelColorD.G) / 4.0F);
                    blue = Convert.ToInt32(((pixelColorA.B) + pixelColorB.B + pixelColorC.B + pixelColorD.B) / 4.0F);
                    bitmap8.SetPixel(pixX, pixY, ColorTranslator.FromOle(ColorUtil.OsRgb(red, green, blue)));
                }
            }

            BitmapUtil.CreateGLTexture (bitmap8, 4, textureNum);

            //-------- 4 --------

            bitmap = bitmap8;
            bitmap4 = new Bitmap(4, 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for ( pixY = 0; pixY <= 3; pixY++ )
            {
                y1 = pixY * 2;
                y2 = y1 + 1;
                for ( pixX = 0; pixX <= 3; pixX++ )
                {
                    x1 = pixX * 2;
                    x2 = x1 + 1;
                    pixelColorA = bitmap.GetPixel(x1, y1);
                    pixelColorB = bitmap.GetPixel(x2, y1);
                    pixelColorC = bitmap.GetPixel(x1, y2);
                    pixelColorD = bitmap.GetPixel(x2, y2);
                    red = Convert.ToInt32(((pixelColorA.R) + pixelColorB.R + pixelColorC.R + pixelColorD.R) / 4.0F);
                    green = Convert.ToInt32(((pixelColorA.G) + pixelColorB.G + pixelColorC.G + pixelColorD.G) / 4.0F);
                    blue = Convert.ToInt32(((pixelColorA.B) + pixelColorB.B + pixelColorC.B + pixelColorD.B) / 4.0F);
                    bitmap4.SetPixel(pixX, pixY, ColorTranslator.FromOle(ColorUtil.OsRgb(red, green, blue)));
                }
            }

            BitmapUtil.CreateGLTexture (bitmap4, 5, textureNum);

            //-------- 2 --------

            bitmap = bitmap4;
            bitmap2 = new Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for ( pixY = 0; pixY <= 1; pixY++ )
            {
                y1 = pixY * 2;
                y2 = y1 + 1;
                for ( pixX = 0; pixX <= 1; pixX++ )
                {
                    x1 = pixX * 2;
                    x2 = x1 + 1;
                    pixelColorA = bitmap.GetPixel(x1, y1);
                    pixelColorB = bitmap.GetPixel(x2, y1);
                    pixelColorC = bitmap.GetPixel(x1, y2);
                    pixelColorD = bitmap.GetPixel(x2, y2);
                    red = Convert.ToInt32(((pixelColorA.R) + pixelColorB.R + pixelColorC.R + pixelColorD.R) / 4.0F);
                    green = Convert.ToInt32(((pixelColorA.G) + pixelColorB.G + pixelColorC.G + pixelColorD.G) / 4.0F);
                    blue = Convert.ToInt32(((pixelColorA.B) + pixelColorB.B + pixelColorC.B + pixelColorD.B) / 4.0F);
                    bitmap2.SetPixel(pixX, pixY, ColorTranslator.FromOle(ColorUtil.OsRgb(red, green, blue)));
                }
            }

            BitmapUtil.CreateGLTexture (bitmap2, 6, textureNum);

            //-------- 1 --------

            bitmap = bitmap2;
            bitmap1 = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            pixX = 0;
            pixY = 0;
            y1 = pixY * 2;
            y2 = y1 + 1;
            x1 = pixX * 2;
            x2 = x1 + 1;
            pixelColorA = bitmap.GetPixel(x1, y1);
            pixelColorB = bitmap.GetPixel(x2, y1);
            pixelColorC = bitmap.GetPixel(x1, y2);
            pixelColorD = bitmap.GetPixel(x2, y2);
            red = Convert.ToInt32(((pixelColorA.R) + pixelColorB.R + pixelColorC.R + pixelColorD.R) / 4.0F);
            green = Convert.ToInt32(((pixelColorA.G) + pixelColorB.G + pixelColorC.G + pixelColorD.G) / 4.0F);
            blue = Convert.ToInt32(((pixelColorA.B) + pixelColorB.B + pixelColorC.B + pixelColorD.B) / 4.0F);
            bitmap1.SetPixel(pixX, pixY, ColorTranslator.FromOle(ColorUtil.OsRgb(red, green, blue)));

            BitmapUtil.CreateGLTexture (bitmap1, 7, textureNum);

            return ReturnResult;
        }       
    }
}