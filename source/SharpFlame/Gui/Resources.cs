

using System.Reflection;
using Eto.Drawing;
using Eto;

namespace SharpFlame.Gui
{
	public class Resources
	{
		static string prefix;
		public static string Prefix { get { return prefix = prefix ?? "SharpFlame.Resources."; } }

		public static string SharpFlameIconName = "flaME.ico";
		public static string BtnRotateAntiClockwiseName = "btnrotateanticlockwise.png";
		public static string BtnRotateClockwiseName = "btnrotateclockwise.png";
		public static string BtnFlipXName = "btnflipx.png";
        public static string IconProblemName = "problem.png";
        public static string IconWarningmName = "warning.png";

		public static Icon SharpFlameIcon()
		{
			return Icon.FromResource (string.Format ("{0}{1}", Prefix, SharpFlameIconName));
		}

		public static Bitmap BtnRotateAntiClockwise()
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnRotateAntiClockwiseName), Assembly.GetExecutingAssembly());
		}

		public static Bitmap BtnRotateClockwise()
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnRotateClockwiseName), Assembly.GetExecutingAssembly());
		}

		public static Bitmap BtnFlipX()
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnFlipXName), Assembly.GetExecutingAssembly());
		}

        public static Bitmap IconProblem()
        {
            return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, IconProblemName), Assembly.GetExecutingAssembly());
        }

        public static Bitmap IconWarning()
        {
            return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, IconWarningmName), Assembly.GetExecutingAssembly());
        }
	}
}

