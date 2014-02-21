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
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }
    }
}