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
		public static string SelectionRotateAntiClockwiseName = "selectionrotateanticlockwise.png";
		public static string SelectionRotateClockwiseName = "selectionrotateclockwise.png";

		public static Icon SharpFlameIcon(Generator generator = null)
		{
			return Icon.FromResource (string.Format ("{0}{1}", Prefix, SharpFlameIconName), generator);
		}

		public static Bitmap SelectionRotateAntiClockwise(Generator generator = null)
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, SelectionRotateAntiClockwiseName), Assembly.GetExecutingAssembly(), generator);
		}

		public static Bitmap SelectionRotateClockwise(Generator generator = null)
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, SelectionRotateClockwiseName), Assembly.GetExecutingAssembly(), generator);
		}
	}
}

