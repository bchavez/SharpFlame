using Microsoft.VisualBasic;

namespace SharpFlame
{
	public sealed class modColour
	{
		
		public static int OSRGB(int Red, int Green, int Blue)
		{
			
#if !Mono
			return Information.RGB(Red, Green, Blue);
#else
//			return Information.RGB(Blue, Green, Red);
#endif
		}
	}
	
	public struct sRGB_sng
	{
		public float Red;
		public float Green;
		public float Blue;
		
		public sRGB_sng(float Red, float Green, float Blue)
		{
			
			this.Red = Red;
			this.Green = Green;
			this.Blue = Blue;
		}
	}
	
	public struct sRGBA_sng
	{
		public float Red;
		public float Green;
		public float Blue;
		public float Alpha;
		
		public sRGBA_sng(float Red, float Green, float Blue, float Alpha)
		{
			
			this.Red = Red;
			this.Green = Green;
			this.Blue = Blue;
			this.Alpha = Alpha;
		}
	}
	
	public class clsRGB_sng
	{
		public float Red;
		public float Green;
		public float Blue;
		
		public clsRGB_sng(float Red, float Green, float Blue)
		{
			
			this.Red = Red;
			this.Green = Green;
			this.Blue = Blue;
		}
		
		public virtual string GetINIOutput()
		{
			
			return modIO.InvariantToString_sng(Red) + ", " + modIO.InvariantToString_sng(Green) + ", " + modIO.InvariantToString_sng(Blue);
		}
		
		public virtual bool ReadINIText(clsSplitCommaText SplitText)
		{
			
			if (SplitText.PartCount < 3)
			{
				return false;
			}
			
			sRGB_sng Colour = new sRGB_sng();
			
			if (!modIO.InvariantParse_sng(SplitText.Parts[0], ref Colour.Red))
			{
				return false;
			}
			if (!modIO.InvariantParse_sng(SplitText.Parts[1], ref Colour.Green))
			{
				return false;
			}
			if (!modIO.InvariantParse_sng(SplitText.Parts[2], ref Colour.Blue))
			{
				return false;
			}
			
			Red = Colour.Red;
			Green = Colour.Green;
			Blue = Colour.Blue;
			
			return true;
		}
	}
	
	public class clsRGBA_sng : clsRGB_sng
	{
		
		public float Alpha;
		
		public clsRGBA_sng(float Red, float Green, float Blue, float Alpha) : base(Red, Green, Blue)
		{
			
			this.Alpha = Alpha;
		}
		
		public clsRGBA_sng(clsRGBA_sng CopyItem) : base(CopyItem.Red, CopyItem.Green, CopyItem.Blue)
		{
			
			Alpha = CopyItem.Alpha;
		}
		
		public override string GetINIOutput()
		{
			
			return base.GetINIOutput() + ", " + modIO.InvariantToString_sng(Alpha);
		}
		
		public override bool ReadINIText(clsSplitCommaText SplitText)
		{
			
			if (!base.ReadINIText(SplitText))
			{
				return false;
			}
			
			if (SplitText.PartCount < 4)
			{
				return false;
			}
			
			if (!modIO.InvariantParse_sng(SplitText.Parts[3], ref Alpha))
			{
				Alpha = 1.0F;
			}
			
			return true;
		}
	}
	
}
