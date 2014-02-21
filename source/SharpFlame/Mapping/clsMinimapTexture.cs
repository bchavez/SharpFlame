#region

using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;

#endregion

namespace SharpFlame.Mapping
{
    public class clsMinimapTexture
    {
        public SRgba[] InlinePixels;
        public XYInt Size;

        public clsMinimapTexture(XYInt Size)
        {
            this.Size = Size;
            InlinePixels = new SRgba[Size.X * Size.Y];
        }

        public SRgba get_Pixels(int X, int Y)
        {
            return InlinePixels[Y * Size.X + X];
        }

        public void set_Pixels(int X, int Y, SRgba value)
        {
            InlinePixels[Y * Size.X + X] = value;
        }
    }
}