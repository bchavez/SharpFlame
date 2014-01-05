using System;
using System.Drawing;
using System.IO;

namespace SharpFlame.Bitmaps
{
    public sealed class BitmapUtil
    {
        public static modProgram.sResult LoadBitmap(string Path, ref Bitmap ResultBitmap)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Problem = "";
            ReturnResult.Success = false;

            Bitmap Bitmap = default(Bitmap);

            try
            {
                Bitmap = new Bitmap(Path);
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                ResultBitmap = null;
                return ReturnResult;
            }

            ResultBitmap = new Bitmap(Bitmap); //copying the bitmap is needed so it doesn't lock access to the file

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public static modProgram.sResult SaveBitmap(string Path, bool Overwrite, Bitmap BitmapToSave)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Problem = "";
            ReturnResult.Success = false;

            try
            {
                if ( File.Exists(Path) )
                {
                    if ( Overwrite )
                    {
                        File.Delete(Path);
                    }
                    else
                    {
                        ReturnResult.Problem = "File already exists.";
                        return ReturnResult;
                    }
                }
                BitmapToSave.Save(Path);
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public static clsResult BitmapIsGLCompatible(Bitmap BitmapToCheck)
        {
            clsResult ReturnResult = new clsResult("Compatability check");

            if ( !modProgram.SizeIsPowerOf2(BitmapToCheck.Width) )
            {
                ReturnResult.WarningAdd("Image width is not a power of 2.");
            }
            if ( !modProgram.SizeIsPowerOf2(BitmapToCheck.Height) )
            {
                ReturnResult.WarningAdd("Image height is not a power of 2.");
            }
            if ( BitmapToCheck.Width != BitmapToCheck.Height )
            {
                ReturnResult.WarningAdd("Image is not square.");
            }

            return ReturnResult;
        }
    }
}