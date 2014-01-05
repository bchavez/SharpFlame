namespace FlaME
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct sRGBA_sng
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;
        public sRGBA_sng(float Red, float Green, float Blue, float Alpha)
        {
            this = new sRGBA_sng();
            this.Red = Red;
            this.Green = Green;
            this.Blue = Blue;
            this.Alpha = Alpha;
        }
    }
}

