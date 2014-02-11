using System.Drawing;

namespace SharpFlame.Colors
{
    public sealed class ColorUtil
    {
        public static int OSRGB(int red, int green, int blue)
        {		
#if !Mono
            Color c = Color.FromArgb(red, green, blue);
#else
			Color c = Color.FromArgb(blue, green, red);
#endif
			return c.ToArgb ();
        }
	}
}