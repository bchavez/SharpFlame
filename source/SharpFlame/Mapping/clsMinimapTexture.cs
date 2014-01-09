using SharpFlame.Colors;
using SharpFlame.Maths;

namespace SharpFlame.Mapping
{
    public class clsMinimapTexture
    {
        public sRGBA_sng[] InlinePixels;
        public sXY_int Size;

        public sRGBA_sng get_Pixels(int X, int Y)
        {
            return InlinePixels[Y * Size.X + X];
        }

        public void set_Pixels(int X, int Y, sRGBA_sng value)
        {
            InlinePixels[Y * Size.X + X] = value;
        }

        public clsMinimapTexture(sXY_int Size)
        {
            this.Size = Size;
            InlinePixels = new sRGBA_sng[Size.X * Size.Y];
        }
    }
}