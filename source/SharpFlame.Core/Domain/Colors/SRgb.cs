using System.Runtime.InteropServices;

namespace SharpFlame.Core.Domain.Colors
{
    public struct SRgb
    {
        public float Red;
        public float Green;
        public float Blue;

        public SRgb(float red, float green, float blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }
    }
}