namespace FlaME
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct sRGB_sng
    {
        public float Red;
        public float Green;
        public float Blue;
        public sRGB_sng(float Red, float Green, float Blue)
        {
            this = new sRGB_sng();
            this.Red = Red;
            this.Green = Green;
            this.Blue = Blue;
        }
    }
}

