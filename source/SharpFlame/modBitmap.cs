using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace SharpFlame
{
	public sealed class modBitmap
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
			catch (Exception ex)
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
				
				if (System.IO.File.Exists(Path))
				{
					if (Overwrite)
					{
						System.IO.File.Delete(Path);
					}
					else
					{
						ReturnResult.Problem = "File already exists.";
						return ReturnResult;
					}
				}
				BitmapToSave.Save(Path);
				
			}
			catch (Exception ex)
			{
				ReturnResult.Problem = ex.Message;
				return ReturnResult;
			}
			
			ReturnResult.Success = true;
			return ReturnResult;
		}
		
		public struct sBitmapGLTexture
		{
			public Bitmap Texture;
			public int TextureNum;
			public int MipMapLevel;
			public TextureMinFilter MinFilter;
			public TextureMagFilter MagFilter;
			
			public void Perform()
			{
				
				System.Drawing.Imaging.BitmapData BitmapData = Texture.LockBits(new Rectangle(0, 0, Texture.Width, Texture.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				
				if (MipMapLevel == 0)
				{
					GL.GenTextures(1, out TextureNum);
				}
				GL.BindTexture(TextureTarget.Texture2D, TextureNum);
				if (MipMapLevel == 0)
				{
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);
				}
				OpenTK.Graphics.OpenGL.GL.TexImage2D(TextureTarget.Texture2D, MipMapLevel, PixelInternalFormat.Rgba8, Texture.Width, Texture.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, BitmapData.Scan0);
				
				Texture.UnlockBits(BitmapData);
			}
		}
		
		public static clsResult BitmapIsGLCompatible(Bitmap BitmapToCheck)
		{
			clsResult ReturnResult = new clsResult("Compatability check");
			
			if (!modProgram.SizeIsPowerOf2(BitmapToCheck.Width))
			{
				ReturnResult.WarningAdd("Image width is not a power of 2.");
			}
			if (!modProgram.SizeIsPowerOf2(BitmapToCheck.Height))
			{
				ReturnResult.WarningAdd("Image height is not a power of 2.");
			}
			if (BitmapToCheck.Width != BitmapToCheck.Height)
			{
				ReturnResult.WarningAdd("Image is not square.");
			}
			
			return ReturnResult;
		}
	}
	
}
