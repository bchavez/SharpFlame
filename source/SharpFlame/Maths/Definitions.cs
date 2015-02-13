

using System;
using System.Diagnostics;


namespace SharpFlame.Maths
{
    [DebuggerDisplay("X={X}, Y={Y}")]
    public struct sXY_uint
    {
        public UInt32 X;
        public UInt32 Y;

        public sXY_uint(UInt32 X, UInt32 Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}