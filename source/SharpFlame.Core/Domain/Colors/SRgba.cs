namespace SharpFlame.Core.Domain.Colors
{
    public struct SRgba
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;

        public SRgba( float red, float green, float blue, float alpha )
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }
    }
}