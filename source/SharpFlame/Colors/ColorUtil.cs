using Microsoft.VisualBasic;

namespace SharpFlame.Colors
{
    public sealed class ColorUtil
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

   
}