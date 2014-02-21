using SharpFlame.FileIO;

namespace SharpFlame.Colors
{
    public struct sRGB_sng
    {
        public float Red;
        public float Green;
        public float Blue;

        public sRGB_sng(float red, float green, float blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }
    }

    public struct sRGBA_sng
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;

        public sRGBA_sng( float red, float green, float blue, float alpha )
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }
    }

    public class clsRGB_sng
    {
        public float Red;
        public float Green;
        public float Blue;

        public clsRGB_sng( float red, float green, float blue )
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }
    }

    public class clsRGBA_sng : clsRGB_sng
    {
        public float Alpha;

        public clsRGBA_sng( float red, float green, float blue, float alpha )
            : base( red, green, blue )
        {
            this.Alpha = alpha;
        }

        public clsRGBA_sng( clsRGBA_sng copyItem )
            : base( copyItem.Red, copyItem.Green, copyItem.Blue )
        {
            Alpha = copyItem.Alpha;
        }      
    }
}