#region

using System;
using System.Drawing;
using System.IO;
using NLog;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Bitmaps
{
    public sealed class BitmapUtil
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static sResult LoadBitmap(string path, ref Bitmap resultBitmap)
        {
            var returnResult = new sResult();
            returnResult.Problem = "";
            returnResult.Success = false;

            var bitmap = default(Bitmap);
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

        public static sResult SaveBitmap(string path, bool overwrite, Bitmap bitmapToSave)
        {
            var returnResult = new sResult();
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

        public static clsResult BitmapIsGlCompatible(Bitmap bitmapToCheck)
        {
            var returnResult = new clsResult("Compatability check", false);
            logger.Debug("Compatability check");

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
    }
}