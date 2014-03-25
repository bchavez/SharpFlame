using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpFlame.Core.Domain
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("X={X}, Y={Y}")]
    public struct XYDouble
    {
        public double X;
        public double Y;
        public static XYDouble operator +(XYDouble ValueA, XYDouble ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA.X + ValueB.X;
            _dbl.Y = ValueA.Y + ValueB.Y;
            return _dbl;
        }

        public static XYDouble operator -(XYDouble ValueA, XYDouble ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA.X - ValueB.X;
            _dbl.Y = ValueA.Y - ValueB.Y;
            return _dbl;
        }

        public static XYDouble operator *(XYDouble ValueA, double ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA.X * ValueB;
            _dbl.Y = ValueA.Y * ValueB;
            return _dbl;
        }

        public static XYDouble operator *(double ValueA, XYDouble ValueB)
        {
            return ValueB * ValueA;
        }

        public static XYDouble operator /(XYDouble ValueA, double ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA.X / ValueB;
            _dbl.Y = ValueA.Y / ValueB;
            return _dbl;
        }

        public static XYDouble operator /(double ValueA, XYDouble ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA / ValueB.X;
            _dbl.Y = ValueA / ValueB.Y;
            return _dbl;
        }

        public XYDouble(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double GetMagnitude()
        {
            return Math.Sqrt((X * X) + (Y * Y));
        }

        public double GetAngle()
        {
            return Math.Atan2(Y, X);
        }
    }
}