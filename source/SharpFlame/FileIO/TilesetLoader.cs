using System;
using System.Drawing;
using System.IO;
using NLog;
using SharpFlame.Bitmaps;
using SharpFlame.Colors;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Core.Extensions;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Util;
using OpenTK.Graphics.OpenGL;


namespace SharpFlame.FileIO
{
    public class TilesetLoader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Tileset tileset;

        public TilesetLoader (ref Tileset tilesetToLoad)
        {
            tileset = tilesetToLoad;
        }

        public Result Load(string path)
        {
            var returnResult = new Result("Loading tileset from '{0}'".Format2(path), false);
            logger.Info("Loading tileset from '{0}'".Format2(path));

            tileset.Directory = path;

            Bitmap bitmap = null;
            var SplitPath = new sSplitPath(path);
            var slashPath = PathUtil.EndWithPathSeperator(path);
            var result = new SimpleResult();

            if ( SplitPath.FileTitle != "" )
            {
                tileset.Name = SplitPath.FileTitle;
            }
            else if ( SplitPath.PartCount >= 2 )
            {
                tileset.Name = SplitPath.Parts[SplitPath.PartCount - 2];
            }

            var ttpFileName = Path.ChangeExtension(tileset.Name, ".ttp");

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

            for ( tileNum = 0; tileNum < tileset.Tiles.Count; tileNum++ )
            {
                var tile = tileset.Tiles[tileNum];

                strTile = "tile-" + App.MinDigits(tileNum, 2) + ".png";

                //-------- 128 --------

                var tileDir = Path.Combine(tileset.Name + "-128", strTile);
                graphicPath = Path.Combine(slashPath, tileDir);


                result = BitmapUtil.LoadBitmap(graphicPath, ref bitmap);
                if ( !result.Success )
                {
                    tileset.Tiles.RemoveRange (tileNum, tileset.Tiles.Count - tileNum);
                    //ignore and exit, since not all tile types have a corresponding tile graphic
                    return returnResult;
                }

                if ( bitmap.Width != 128 | bitmap.Height != 128 )
                {
                    returnResult.WarningAdd (string.Format ("Tile graphic {0} from tileset {1} is not 128x128.", graphicPath, tileset.Name));
                    return returnResult;
                }

                if ( App.SettingsManager.Mipmaps )
                {
                    tile.GlTextureNum = BitmapUtil.CreateGLTexture (bitmap, 0, 0,
                                                                              TextureMagFilter.Nearest, 
                                                                              TextureMinFilter.LinearMipmapLinear);
                }
                else
                {
                    tile.GlTextureNum = BitmapUtil.CreateGLTexture (bitmap, 0, 0, 
                                                                              TextureMagFilter.Nearest, 
                                                                              TextureMinFilter.Nearest);
                }

                if ( App.SettingsManager.Mipmaps )
                {
                    if ( App.SettingsManager.MipmapsHardware )
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                        GL.Disable(EnableCap.Texture2D);
                    }
                    else
                    {
                        var MipmapResult = default(Result);
                        MipmapResult = generateMipMaps(slashPath, strTile, 
                                                       tileset.Tiles[tileNum].GlTextureNum, tileNum);
                        returnResult.Add(MipmapResult);
                        if ( MipmapResult.HasProblems )
                        {
                            return returnResult;
                        }
                    }
                    GL.GetTexImage(TextureTarget.Texture2D, 7, PixelFormat.Rgba, PixelType.Float, AverageColour);
                    tile.AverageColour.Red = AverageColour[0];
                    tile.AverageColour.Green = AverageColour[1];
                    tile.AverageColour.Blue = AverageColour[2];
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
                    tile.AverageColour.Red = (float)(redTotal / 4177920.0D);
                    tile.AverageColour.Green = (float)(greenTotal / 4177920.0D);
                    tile.AverageColour.Blue = (float)(blueTotal / 4177920.0D);
                }

                tileset.Tiles[tileNum] = tile;
            }

            return returnResult;
        }

        private SimpleResult loadTileType(string path)
        {
            var returnResult = new SimpleResult();

            try
            {
                using (var file = new BinaryReader(new FileStream(path, FileMode.Open))) {
                    returnResult = readTileType(file);
                }
            }
            catch ( Exception ex )
            {
                returnResult.Problem = ex.Message;
                return returnResult;
            }
            return returnResult;
        }

        private SimpleResult readTileType(BinaryReader file)
        {
            var returnResult = new SimpleResult();
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
                tileset.TileCount = Convert.ToInt32(uintTemp);
                // tileset.Tiles = new Tile[tileset.TileCount];

                for ( i = 0; i < Math.Min((Int32)uintTemp,tileset.TileCount); i++ )
                {
                    var tile = new Tile();
                    ushortTemp = file.ReadUInt16();
                    if ( ushortTemp > App.TileTypes.Count )
                    {
                        returnResult.Problem = "Unknown tile type.";
                        return returnResult;
                    }
                    tile.DefaultType = (byte)ushortTemp;
                    tileset.Tiles.Add(tile);
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

        private Result generateMipMaps(string slashPath, string strTile, int textureNum, int tileNum)
        {
            var returnResult = new Result("Generating mipmaps", false);
            logger.Info("Generating mipmaps");
            var graphicPath = "";
            var pixX = 0;
            var pixY = 0;
            Color pixelColorA;
            Color pixelColorB;
            Color pixelColorC;
            Color pixelColorD;
            var x1 = 0;
            var y1 = 0;
            var x2 = 0;
            var y2 = 0;
            var red = 0;
            var green = 0;
            var blue = 0;
            Bitmap bitmap = null;

            //-------- 64 --------

            graphicPath = string.Format ("{0}{1}-64{2}{3}", slashPath, tileset.Name, Path.DirectorySeparatorChar, strTile);

            var result = BitmapUtil.LoadBitmap(graphicPath, ref bitmap);
            if ( !result.Success )
            {
                returnResult.WarningAdd("Unable to load tile graphic: " + result.Problem);
                return returnResult;
            }

            if ( bitmap.Width != 64 | bitmap.Height != 64 )
            {
                returnResult.WarningAdd (string.Format ("Tile graphic {0} from tileset {1} is not 64x64.", graphicPath, tileset.Name));
                return returnResult;
            }

            BitmapUtil.CreateGLTexture (bitmap, 1, textureNum);

            //-------- 32 --------

            graphicPath = string.Format ("{0}{1}-32{2}{3}", slashPath, tileset.Name, Path.DirectorySeparatorChar, strTile);

            result = BitmapUtil.LoadBitmap(graphicPath, ref bitmap);
            if ( !result.Success )
            {
                returnResult.WarningAdd (string.Format ("Unable to load tile graphic: {0}", result.Problem));
                return returnResult;
            }

            if ( bitmap.Width != 32 | bitmap.Height != 32 )
            {
                returnResult.WarningAdd (string.Format ("Tile graphic {0} from tileset {1} is not 32x32.", graphicPath, tileset.Name));
                return returnResult;
            }

            BitmapUtil.CreateGLTexture (bitmap, 2, textureNum);

            //-------- 16 --------

            graphicPath = string.Format ("{0}{1}-16{2}{3}", slashPath, tileset.Name, Path.DirectorySeparatorChar, strTile);

            result = BitmapUtil.LoadBitmap(graphicPath, ref bitmap);
            if ( !result.Success )
            {
                returnResult.WarningAdd("Unable to load tile graphic: " + result.Problem);
                return returnResult;
            }

            if ( bitmap.Width != 16 | bitmap.Height != 16 )
            {
                returnResult.WarningAdd (string.Format ("Tile graphic {0} from tileset {1} is not 16x16.", graphicPath, tileset.Name));
                return returnResult;
            }

            BitmapUtil.CreateGLTexture (bitmap, 3, textureNum);

            //-------- 8 --------

            var bitmap8 = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
            var bitmap4 = new Bitmap(4, 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
            var bitmap2 = new Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
            var bitmap1 = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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

            return returnResult;
        }    
    }
}

