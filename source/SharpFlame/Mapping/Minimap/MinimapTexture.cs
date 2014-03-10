#region

using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;

#endregion

namespace SharpFlame.Mapping.Minimap
{
    public class MinimapTexture
    {
        public SRgba[] InlinePixels;
        public XYInt Size;

        public MinimapTexture(XYInt Size)
        {
            this.Size = Size;
            InlinePixels = new SRgba[Size.X * Size.Y];
        }

        public SRgba get(int X, int Y)
        {
            return InlinePixels[Y * Size.X + X];
        }

        public void set(int X, int Y, SRgba value)
        {
            InlinePixels[Y * Size.X + X] = value;
        }
    }
}