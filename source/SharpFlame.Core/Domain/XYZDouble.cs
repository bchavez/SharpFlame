using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpFlame.Core.Domain
{
    [DebuggerDisplay("X={X}, Y={Y}, Z={Z}")]
    [StructLayout(LayoutKind.Sequential)]
    public struct XYZDouble
    {
        public double X;
        public double Y;
        public double Z;
        public static XYZDouble operator +(XYZDouble ValueA, XYZDouble ValueB)
        {
            XYZDouble _dbl;
            _dbl.X = ValueA.X + ValueB.X;
            _dbl.Y = ValueA.Y + ValueB.Y;
            _dbl.Z = ValueA.Z + ValueB.Z;
            return _dbl;
        }

        public static XYZDouble operator -(XYZDouble ValueA, XYZDouble ValueB)
        {
            XYZDouble _dbl;
            _dbl.X = ValueA.X - ValueB.X;
            _dbl.Y = ValueA.Y - ValueB.Y;
            _dbl.Z = ValueA.Z - ValueB.Z;
            return _dbl;
        }

        public static XYZDouble operator *(XYZDouble ValueA, XYZDouble ValueB)
        {
            XYZDouble _dbl;
            _dbl.X = ValueA.X * ValueB.X;
            _dbl.Y = ValueA.Y * ValueB.Y;
            _dbl.Z = ValueA.Z * ValueB.Z;
            return _dbl;
        }

        public static XYZDouble operator *(XYZDouble ValueA, double ValueB)
        {
            XYZDouble _dbl;
            _dbl.X = ValueA.X * ValueB;
            _dbl.Y = ValueA.Y * ValueB;
            _dbl.Z = ValueA.Z * ValueB;
            return _dbl;
        }

        public static XYZDouble operator *(double ValueA, XYZDouble ValueB)
        {
            return ValueB * ValueA;
        }

        public static XYZDouble operator /(XYZDouble ValueA, XYZDouble ValueB)
        {
            XYZDouble _dbl;
            _dbl.X = ValueA.X / ValueB.X;
            _dbl.Y = ValueA.Y / ValueB.Y;
            _dbl.Z = ValueA.Z / ValueB.Z;
            return _dbl;
        }

        public static XYZDouble operator /(XYZDouble ValueA, double ValueB)
        {
            XYZDouble _dbl;
            _dbl.X = ValueA.X / ValueB;
            _dbl.Y = ValueA.Y / ValueB;
            _dbl.Z = ValueA.Z / ValueB;
            return _dbl;
        }

        public static XYZDouble operator /(double ValueA, XYZDouble ValueB)
        {
            XYZDouble _dbl;
            _dbl.X = ValueA / ValueB.X;
            _dbl.Y = ValueA / ValueB.Y;
            _dbl.Z = ValueA / ValueB.Z;
            return _dbl;
        }

        public XYZDouble(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double GetMagnitude()
        {
            return Math.Sqrt(((X * X) + (Y * Y)) + (Z * Z));
        }
    }
}