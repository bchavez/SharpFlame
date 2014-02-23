using System.Reflection;
using Eto.Drawing;
using Eto;

namespace SharpFlame.Gui
{
	public class Resources
	{
		static string prefix;
		public static string Prefix { get { return prefix = prefix ?? string.Format ("{0}.Resources.", Assembly.GetExecutingAssembly ().GetName ().Name); } }

		public static string SharpFlameIconName = "flaME.ico";
		public static string BtnRotateAntiClockwiseName = "btnrotateanticlockwise.png";
		public static string BtnRotateClockwiseName = "btnrotateclockwise.png";
		public static string BtnFlipXName = "btnflipx.png";

		public static Icon SharpFlameIcon(Generator generator = null)
		{
			return Icon.FromResource (string.Format ("{0}{1}", Prefix, SharpFlameIconName), generator);
		}

		public static Bitmap BtnRotateAntiClockwise(Generator generator = null)
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnRotateAntiClockwiseName), Assembly.GetExecutingAssembly(), generator);
		}

		public static Bitmap BtnRotateClockwise(Generator generator = null)
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnRotateClockwiseName), Assembly.GetExecutingAssembly(), generator);
		}

		public static Bitmap BtnFlipX(Generator generator = null)
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnFlipXName), Assembly.GetExecutingAssembly(), generator);
		}
	}
}

